using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WorkDriveStorage.CustomControls.MessageBox;
using WorkDriveStorage.FrameWork;
using WorkDriveStorage.Popup;

namespace WorkDriveStorage.View
{
    /// <summary>
    /// ProjectPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProjectPage : UserControl
    {
        public static RoutedCommand _myCommand_CtrlS = new RoutedCommand();


        public ProjectPage()
        {
            InitializeComponent();
            init();
            initComboBox();
        }

        #region Initialize.

        private void init()
        {
            _myCommand_CtrlS.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(_myCommand_CtrlS, KeySaveFileClick));

            TreeSource.init(Constant.FileType.Source);
            TreeSource.OnFileAdd += Tree_OnFileAdd;
            TreeDocument.init(Constant.FileType.Document);
            TreeDocument.OnFileAdd += Tree_OnFileAdd;
        }

        private void initComboBox()
        {
            DataTable dt = ServiceProvider.StaticService().MainDatabase.GetData("GetProjectAllName", "0001");

            List<string> targetLIst = (from r in dt.AsEnumerable()
                                       select r.Field<string>("ProjectName")).ToList();
            cbProjectList.ItemsSource = targetLIst;
        }

        private void initMemo()
        {
            TextRange txt = new TextRange(txtMemo.Document.ContentStart, txtMemo.Document.ContentEnd);
            txt.Text = "";

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("ProjectName", cbProjectList.SelectedItem.ToString());
            DataTable dt = ServiceProvider.StaticService().MainDatabase.GetData("GetProjectMemo", "0001", parameters);
            SetRTFText(dt.Rows[0]["MemoRtf"].ToString());
        }

        #endregion

        #region Event.

        private void btnColor_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            SolidColorBrush color = Brushes.Black;
            switch (btn.Tag)
            {
                case "Red":
                    color = Brushes.Red;
                    break;
                case "Blue":
                    color = Brushes.Blue;
                    break;
                case "Green":
                    color = Brushes.Green;
                    break;
            }

            SetColor(color);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string memoText = GetText();
                string memoRtf = GetRtfText();

                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("ProjectName", cbProjectList.SelectedItem.ToString());
                parameters.Add("MemoRtf", memoRtf.Replace("'", "''"));
                parameters.Add("MemoString", memoText.Replace("'", "''"));
                bool result = ServiceProvider.StaticService().MainDatabase.SetData("SetMemoUpdate", "0001", parameters);

                if (result)
                    CustomControlMessageBox.Show("저장 성공");
                else
                    CustomControlMessageBox.Show("반영 받은 행이 없습니다.");
            }
            catch (Exception ex)
            {
                CustomControlMessageBox.ShowError("저장 실패\r\n" + ex.Message, "Project Memo Save");
            }
        }

        private void ComboBoxFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (txtMemo != null)
            {
                ComboBoxItem selectItem = (ComboBoxItem)cbFontSize.SelectedItem;
                if (selectItem == null)
                {
                }
                else
                {
                    SetFontSize(txtMemo, double.Parse(selectItem.Content.ToString()));
                }
            }
        }

        private void cbProjectList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(cbProjectList.SelectedItem.ToString()) == false)
                {
                    initMemo();
                    TreeSource.LoadTreeItem(cbProjectList.SelectedItem.ToString());
                    TreeDocument.LoadTreeItem(cbProjectList.SelectedItem.ToString());

                }
            }
            catch (Exception ex)
            {
                CustomControlMessageBox.ShowError(ex.Message, "Project SelectionChanged");
            }
        }

        private void btnAddProject_Click(object sender, RoutedEventArgs e)
        {
            ProjectAddWindow projectAddWindow = new ProjectAddWindow();
            try
            {
                projectAddWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                CustomControlMessageBox.ShowError(ex.Message, "Project Add");
            }
            finally
            {
                initComboBox();
            }
        }

        private void Tree_OnFileAdd(Constant.FileType? fileType)
        {
            ProjectFileAddWindow fileAddWindow = new ProjectFileAddWindow(cbProjectList.SelectedItem.ToString(), fileType);
            try
            {
                fileAddWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                CustomControlMessageBox.ShowError(ex.Message, "File Add");
            }
            finally
            {
                if (fileType == Constant.FileType.Document)
                    TreeDocument.LoadTreeItem(cbProjectList.SelectedItem.ToString());
                else
                    TreeSource.LoadTreeItem(cbProjectList.SelectedItem.ToString());
            }
        }

        private void btnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            string fileTargetFolderPath = ServiceProvider.StaticService().Config.FileDataPath;

            if (cbProjectList.SelectedItem != null)
            {
                fileTargetFolderPath = fileTargetFolderPath + "\\" + cbProjectList.SelectedItem.ToString();
            }

            System.Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\" + "explorer.exe", fileTargetFolderPath);

        }

        #endregion

        #region Method.

        private void SetRTFText(string text)
        {
            if (string.IsNullOrEmpty(text) == false)
            {
                MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(text));
                this.txtMemo.Selection.Load(stream, DataFormats.Rtf);
            }
        }

        private string GetRtfText()
        {
            string rtfFromRtb = string.Empty;
            using (MemoryStream ms = new MemoryStream())
            {
                TextRange range2 = new TextRange(txtMemo.Document.ContentStart, txtMemo.Document.ContentEnd);
                range2.Save(ms, DataFormats.Rtf);
                ms.Seek(0, SeekOrigin.Begin);
                using (StreamReader sr = new StreamReader(ms))
                {
                    rtfFromRtb = sr.ReadToEnd();
                }
            }

            return rtfFromRtb;
        }

        private string GetText()
        {
            TextRange textRange = new TextRange(txtMemo.Document.ContentStart, txtMemo.Document.ContentEnd);
            return textRange.Text;
        }

        private void SetColor(SolidColorBrush color)
        {
            BrushConverter bc = new BrushConverter();
            TextRange tr = new TextRange(txtMemo.Selection.Start, txtMemo.Selection.End);
            try
            {
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, color);
            }
            catch (FormatException) { }
        }

        private void KeySaveFileClick(object sender, ExecutedRoutedEventArgs e)
        {
            btnSave_Click(null, null);
        }

        public void SetFontSize(RichTextBox target, double value)
        {
            // Make sure we have a richtextbox.
            if (target == null)
                return;

            // Make sure we have a selection. Should have one even if there is no text selected.
            if (target.Selection != null)
            {
                // Check whether there is text selected or just sitting at cursor
                if (target.Selection.IsEmpty)
                {
                    // Check to see if we are at the start of the textbox and nothing has been added yet
                    if (target.Selection.Start.Paragraph == null)
                    {
                        // Add a new paragraph object to the richtextbox with the fontsize
                        Paragraph p = new Paragraph();
                        p.FontSize = value;
                        target.Document.Blocks.Add(p);
                    }
                    else
                    {
                        // Get current position of cursor
                        TextPointer curCaret = target.CaretPosition;
                        // Get the current block object that the cursor is in
                        Block curBlock = target.Document.Blocks.Where(x => x.ContentStart.CompareTo(curCaret) == -1 && x.ContentEnd.CompareTo(curCaret) == 1).FirstOrDefault();
                        if (curBlock != null)
                        {
                            Paragraph curParagraph = curBlock as Paragraph;
                            // Create a new run object with the fontsize, and add it to the current block
                            Run newRun = new Run();
                            newRun.FontSize = value;
                            curParagraph.Inlines.Add(newRun);
                            // Reset the cursor into the new block. 
                            // If we don't do this, the font size will default again when you start typing.
                            target.CaretPosition = newRun.ElementStart;
                        }
                    }
                }
                else // There is selected text, so change the fontsize of the selection
                {
                    TextRange selectionTextRange = new TextRange(target.Selection.Start, target.Selection.End);
                    selectionTextRange.ApplyPropertyValue(TextElement.FontSizeProperty, value);
                }
            }
            // Reset the focus onto the richtextbox after selecting the font in a toolbar etc
            target.Focus();
        }

        #endregion

    }
}
