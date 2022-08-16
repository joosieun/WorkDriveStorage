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

namespace WorkDriveStorage.Popup
{
    /// <summary>
    /// ProjectFileAddWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProjectFileAddWindow : Window
    {
        private string _projectName = string.Empty;
        private Constant.FileType? _fileType;

        private string _mainPath = string.Empty;

        public ProjectFileAddWindow(string projectName, Constant.FileType? fileType)
        {
            InitializeComponent();
            _projectName = projectName;
            _fileType = fileType;
        }

        private void btnGetFilePath_Click(object sender, RoutedEventArgs e)
        {
            if (Constant.FileType.Document == _fileType)
            {
                List<FileInfo> temp = Utility.GetFilesPath();
                _mainPath = temp[0].DirectoryName;

                string viewText = string.Empty;
                foreach (FileInfo fi in temp)
                {
                    viewText = viewText + fi.Name + ";";
                }
                txtFilePath.Text = viewText;
            }
            else
                txtFilePath.Text = Utility.GetFilePath("sln", "Visual Studio Solution (*.sln) | *.sln");
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            List<bool> resultList = new List<bool>();
            bool finalResult = true;
            try
            {
                string[] files = txtFilePath.Text.Split(';');

                int emptyIndex = 0;

                for (int a = 0; a < files.Length; a++)
                {
                    if(string.IsNullOrEmpty(files[a]))
                    {
                        emptyIndex = a;
                    }
                }

                files = files.Where((source, index) => index != emptyIndex).ToArray();

                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(_mainPath + "\\" + file);

                    if (fi.Exists)
                    {
                        bool result = Utility.SetFileAdd(fi, _fileType, _projectName, txtGroupNmae.Text);

                        resultList.Add(result);
                    }
                    else
                    {
                        CustomControlMessageBox.ShowError("The file could not be found.\r\n" + fi);
                    }
                }

                for(int i=0; i<resultList.Count; i++)
                {
                    if (!resultList[i])
                    {
                        finalResult = false;
                    }
                }

                switch(finalResult)
                {
                    case true:
                        CustomControlMessageBox.Show("파일 추가 성공");
                        this.Close();
                        break;
                    case false:
                        CustomControlMessageBox.ShowError("파일 추가 실패");
                        break;
                }
                
            }
            catch (Exception ex)
            {
                CustomControlMessageBox.ShowError(ex.Message);
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}