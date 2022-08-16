using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace WorkDriveStorage.View
{
    /// <summary>
    /// SettingPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingPage : UserControl
    {
        public SettingPage()
        {
            InitializeComponent();
            ControlInit();
            LoadConfig();
        }

        #region init.

        private void LoadConfig()
        {
            cb_CopyOrMove.Text = ServiceProvider.StaticService().Config.DocumentFileMoveType.ToString();
            txt_DataSavePath.Text = ServiceProvider.StaticService().Config.FileDataPath.ToString();
            txt_DatabasePath.Text = ServiceProvider.StaticService().Config.DatabasePath.ToString();
        }

        private void ControlInit()
        {
            cb_CopyOrMove.ItemsSource = new string[] { "Copy", "Move" };
        }

        #endregion

        #region Event.

        private void btnGetFileDataPath_Click(object sender, RoutedEventArgs e)
        {
            txt_DataSavePath.Text = Utility.GetDirectoryPath();
        }

        private void btnGetDatabasePath_Click(object sender, RoutedEventArgs e)
        {
            txt_DatabasePath.Text = Utility.GetFilePath(System.Environment.CurrentDirectory, "Sqlite Database (*.db) | *.db");
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;

                foreach (TextBox tb in FindVisualChildren<TextBox>(this.ConfigControl))
                {
                    if (tb.Tag != null)
                    {
                        string key = tb.Tag.ToString();

                        if (settings[key] == null)
                        {
                            settings.Add(key, tb.Text);
                        }
                        else
                        {
                            settings[key].Value = tb.Text;
                        }
                    }
                }

                foreach (ComboBox cb in FindVisualChildren<ComboBox>(this.ConfigControl))
                {
                    if (cb.Tag != null)
                    {
                        string key = cb.Tag.ToString();

                        if (settings[key] == null)
                        {
                            settings.Add(key, cb.Text);
                        }
                        else
                        {
                            settings[key].Value = cb.Text;
                        }
                    }
                }

                configFile.Save(ConfigurationSaveMode.Modified);

                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

                ServiceProvider.StaticService().Config.DocumentFileMoveType = (Constant.DocumentFileMoveType)Enum.Parse(typeof(Constant.DocumentFileMoveType), cb_CopyOrMove.Text, true);
                ServiceProvider.StaticService().Config.FileDataPath = txt_DataSavePath.Text;
                ServiceProvider.StaticService().Config.DatabasePath = txt_DatabasePath.Text;
                CustomControlMessageBox.Show("저장 완료", "Setting View");
            }
            catch (Exception ex)
            {
                CustomControlMessageBox.Show("저장 실패\r\n" + ex.Message, "Setting View");
            }
        }

        private void btn_CreateDatabase_Click(object sender, RoutedEventArgs e)
        {
            if (ServiceProvider.StaticService().MainDatabase.CreateDatabase())
            {
                CustomControlMessageBox.Show("생성 완료", "Setting View");
                txt_DatabasePath.Text = AppDomain.CurrentDomain.BaseDirectory + "WorkDriveStorage.db";
            }
            else
                CustomControlMessageBox.ShowError("생성 실패", "Setting View");
        }

        private void txt_DataSavePath_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_DataSavePath.Text))
                txt_DataSavePath.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFB71C1C");
            else
                txt_DataSavePath.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#89000000");

            RequiredValueCheck(txt_DataSavePath, txt_DataSavePath.Text);
        }

        private void txt_DatabasePath_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_DatabasePath.Text))
                txt_DatabasePath.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFB71C1C");
            else
                txt_DatabasePath.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#89000000");

            RequiredValueCheck(txt_DatabasePath, txt_DatabasePath.Text);
        }

        #endregion

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void RequiredValueCheck(DependencyObject element, string value)
        {
            SolidColorBrush assistColor = null;
            string helperText = string.Empty;
            if (string.IsNullOrEmpty(value))
            {
                assistColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFB71C1C");
                helperText = "필수값입니다";
            }
            else
            {
                assistColor = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF2196F3");
            }

            MaterialDesignThemes.Wpf.HintAssist.SetForeground(element, assistColor);
            MaterialDesignThemes.Wpf.HintAssist.SetHelperText(element, helperText);

            MaterialDesignThemes.Wpf.TextFieldAssist.SetUnderlineBrush(element, assistColor);
            Type txtType = txt_DataSavePath.GetType();
        }
    }
}
