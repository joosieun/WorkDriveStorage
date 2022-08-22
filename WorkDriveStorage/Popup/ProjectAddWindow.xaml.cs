using System;
using System.Collections.Generic;
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
using WorkDriveStorage.CustomControls.MessageBox;
using WorkDriveStorage.FrameWork;
using WorkDriveStorage.FrameWork.Database;

namespace WorkDriveStorage.Popup
{
    /// <summary>
    /// ProjectAddWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProjectAddWindow : Window
    {
        public ProjectAddWindow()
        {
            InitializeComponent();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) == false)
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("ProjectName", txtName.Text);
                parameters.Add("CreateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                bool result = ServiceProvider.StaticService().MainDatabase.SetData(GetQueryString.SQLite.SetProjectAdd, parameters);

                if (result)
                    CustomControlMessageBox.Show("등록 성공", "Project Add");

                this.Close();
            }
        }
    }
}
