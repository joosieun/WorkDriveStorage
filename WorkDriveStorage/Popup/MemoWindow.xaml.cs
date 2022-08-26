using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using WorkDriveStorage.FrameWork;
using WorkDriveStorage.FrameWork.Database;

namespace WorkDriveStorage.Popup
{
    /// <summary>
    /// MemoWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MemoWindow : Window
    {


        private string _memoKey = string.Empty;

        public MemoWindow(string key, string contents)
        {
            InitializeComponent();
            this.Title = "WorkDriveStorage_Memo_" + key;
            _memoKey = key;
            SetRTFText(contents);
            SetFontSize(txtMemo, 12);

            //this.WindowStyle = WindowStyle.ToolWindow;
        }

        private void txtMemo_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (txtMemo.IsKeyboardFocused)
            {
                System.Windows.Media.Animation.Storyboard goBikeSB = this.Resources["OpenMenu"] as System.Windows.Media.Animation.Storyboard;
                Action action = () => { this.BeginStoryboard(goBikeSB); };
                Dispatcher.BeginInvoke(action, DispatcherPriority.Render);
            }
            else
            {
                System.Windows.Media.Animation.Storyboard goBikeSB = this.Resources["CloseMenu"] as System.Windows.Media.Animation.Storyboard;
                Action action = () => { this.BeginStoryboard(goBikeSB); };
                Dispatcher.BeginInvoke(action, DispatcherPriority.Render);
                DataSet();
            }
        }

        private void DataSet()
        {
            string memoText = GetText();
            string memoRtf = GetRtfText();
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("WallPaper", "False");
            param.Add("Contents", memoText.Replace("'", "''"));
            param.Add("ContentRTF", memoRtf.Replace("'", "''"));
            param.Add("Sequence", _memoKey);
            bool result = ServiceProvider.StaticService().MainDatabase.SetData(GetQueryString.SQLite.SetStickerMemoUpdate, param);
            MemoServiceProvider.StaticService().MemoWindow_MemoValeChanged(_memoKey, GetText());

        }

        private void DockPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
                MemoLocationChanged();
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

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
            int index = textRange.Text.LastIndexOf(Environment.NewLine);
            return textRange.Text.Substring(0,index);
        }

        private void MemoLocationChanged()
        {
            MemoServiceProvider.StaticService().MemoWindowLocationChanged(_memoKey, this.Top.ToString()+","+this.Left.ToString());
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

        public void SetSize(double width, double height)
        {
            this.Width = width;
            this.Height = height;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MemoServiceProvider.StaticService().MemoWindowClose(_memoKey);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            MemoServiceProvider.StaticService().Add(true);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MemoServiceProvider.StaticService().Delete(_memoKey);
            this.Close();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MemoServiceProvider.StaticService().MemoWindowSizeChanged(_memoKey, this.Width + "," + this.Height);            
        }
    }
}
