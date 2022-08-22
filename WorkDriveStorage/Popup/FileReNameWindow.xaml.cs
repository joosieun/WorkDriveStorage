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
using WorkDriveStorage.CustomControls.MessageBox;
using WorkDriveStorage.FrameWork;
using WorkDriveStorage.FrameWork.Database;

namespace WorkDriveStorage.Popup
{
    /// <summary>
    /// FileReNameWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FileReNameWindow : Window
    {
        private string _fileePath = string.Empty;
        private FileInfo oldFileinfo;

        public FileReNameWindow(string filePath)
        {
            InitializeComponent();
            _fileePath = filePath;

            oldFileinfo = new FileInfo(_fileePath);
            txtName.Text = oldFileinfo.Name;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            FileInfo fi = new FileInfo(_fileePath);
            if (fi.Exists)
            {
                if (Utility.MoveFile(fi, fi.DirectoryName, txtName.Text))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    parameters.Add("name", txtName.Text);
                    parameters.Add("oldPath", oldFileinfo.FullName);
                    parameters.Add("newPath", fi.FullName);

                    bool result = ServiceProvider.StaticService().MainDatabase.SetData(GetQueryString.SQLite.SetDocumentFileReName, parameters);

                    if (result == false)
                        CustomControlMessageBox.ShowError("파일 이름 변경 완료 / DB 업데이트 실패");
                    else
                        this.Close();

                }
                else
                {
                    CustomControlMessageBox.ShowError("이름 변경 실패");
                }
            }
        }
    }
}
