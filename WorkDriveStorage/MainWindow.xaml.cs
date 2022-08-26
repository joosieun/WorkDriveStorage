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
using WorkDriveStorage.CustomControls;
using WorkDriveStorage.CustomControls.MessageBox;
using WorkDriveStorage.FrameWork;
using WorkDriveStorage.View;

namespace WorkDriveStorage
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool initFlag = false;
        public MainWindow()
        {
            InitializeComponent();
            MaxHeight = SystemParameters.WorkArea.Height + 14;
            InitializeProgram();
        }

        private void InitializeProgram()
        {
            ServiceProvider.StaticService().Config.DocumentFileMoveType = (Constant.DocumentFileMoveType)Enum.Parse(typeof(Constant.DocumentFileMoveType), ConfigurationManager.AppSettings["CopyOrMove"], true);
            ServiceProvider.StaticService().Config.FileDataPath = ConfigurationManager.AppSettings["FileDataPath"];
            ServiceProvider.StaticService().Config.DatabasePath = ConfigurationManager.AppSettings["DatabasePath"];

            Panl_View.Children.Clear();
            if (string.IsNullOrEmpty(ServiceProvider.StaticService().Config.FileDataPath) || string.IsNullOrEmpty(ServiceProvider.StaticService().Config.DatabasePath))
            {
                CustomControlMessageBox.ShowWarning("필수값이 설정되지 않았습니다.");

                txt_View_Name.Text = "Setting";
                txt_View_Description.Text = "설정";
                Panl_View.Children.Add(new SettingPage());
            }
            else
            {
                initFlag = true;
                ServiceProvider.StaticService().MainDatabase.init();
                MemoServiceProvider.StaticService();


                txt_View_Name.Text = "MainView";
                txt_View_Description.Text = "";
                Panl_View.Children.Add(new MainPage());
            }
        }

        /// <summary>
        /// 화면 전환 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="viewName"></param>
        private void btn_Menu_Button_Click(object sender, RoutedEventArgs e, Constant.PageName viewName, string description)
        {
            txt_View_Name.Text = viewName.ToString();
            txt_View_Description.Text = description;
            Panl_View.Children.Clear();

            switch (viewName)
            {
                case Constant.PageName.MainView:
                    Panl_View.Children.Add(new MainPage());
                    break;
                case Constant.PageName.Memo:
                    Panl_View.Children.Add(new MemoPage());
                    break;
                case Constant.PageName.Project:
                    Panl_View.Children.Add(new ProjectPage());
                    break;
                case Constant.PageName.Setting:
                    Panl_View.Children.Add(new SettingPage());
                    break;
                case Constant.PageName.Calendar:
                    break;
            }
        }

        #region 윈도우 창 컨트롤

        /// <summary>
        /// 창 이동
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        /// <summary>
        /// 창 닫기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 창 최대화 / 최소화
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Window_Maximized_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }

        /// <summary>
        /// 창 내리기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Window_Minimized_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        #endregion

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                borderMain.CornerRadius = new CornerRadius(20);
                DockPanel_TitlePanel.Margin = new Thickness(0, 0, 0, 0);

            }
            else
            {
                borderMain.CornerRadius = new CornerRadius(0);
                DockPanel_TitlePanel.Margin = new Thickness(0, 10, 0, 0);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MemoServiceProvider.StaticService().Dispose();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (initFlag)
                MemoServiceProvider.StaticService().MemoWindowInit();
        }
    }
}
