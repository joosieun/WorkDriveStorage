using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WorkDriveStorage.FrameWork
{
    public class Utility
    {
        /// <summary>
        /// 리소스의 이미지를 ImageSource형식으로 변환
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ImageSource GetImageSource(object value)
        {
            byte[] result = null;
            if (value != null)
            {
                if (value is Bitmap)
                {
                    Bitmap image = (Bitmap)value;
                    MemoryStream stream = new MemoryStream();
                    image.Save(stream, image.RawFormat);
                    result = stream.ToArray();
                }
                else if (value is byte[])
                {
                    result = (byte[])value;
                }
            }
            else
            {
                return null;
            }

            var bitImg = new BitmapImage();
            ImageSource imageSource = null;
            using (var stream = new MemoryStream(result))
            {
                bitImg.BeginInit();
                bitImg.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bitImg.CacheOption = BitmapCacheOption.OnLoad;
                bitImg.StreamSource = stream; bitImg.EndInit();
                imageSource = bitImg as ImageSource;
            }
            return imageSource;
        }

        /// <summary>
        /// 아이콘 이미지 추출
        /// </summary>
        /// <param name="itemPath"></param>
        /// <returns></returns>
        public static ImageSource GetIconImageSource(string itemPath)
        {
            try
            {
                System.Windows.Media.ImageSource icon;
                using (System.Drawing.Icon sysicon = System.Drawing.Icon.ExtractAssociatedIcon(itemPath))
                {
                    icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                        sysicon.Handle,
                        System.Windows.Int32Rect.Empty,
                        System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                }

                return icon;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static string GetFilePath(string DefaultExt, string Filter)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.DefaultExt = DefaultExt;
                openFileDialog.Filter = Filter;

                openFileDialog.ShowDialog();
                if (openFileDialog.FileName.Length > 0)
                {
                    return openFileDialog.FileName;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<FileInfo> GetFilesPath()
        {
            List<FileInfo> list = new List<FileInfo>();
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;
                openFileDialog.ShowDialog();
                if (openFileDialog.FileName.Length > 0)
                {
                    for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                    {
                        list.Add(new FileInfo(openFileDialog.FileNames[i]));
                    }

                    return list;
                }
                else
                {
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetFilePath()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;
                openFileDialog.ShowDialog();
                if (openFileDialog.FileName.Length > 0)
                {
                    return openFileDialog.FileName;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<FileInfo> GetFilesPath()
        {
            List<FileInfo> list = new List<FileInfo>();
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;
                openFileDialog.ShowDialog();
                if (openFileDialog.FileName.Length > 0)
                {
                    for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                    {
                        list.Add(new FileInfo(openFileDialog.FileNames[i]));
                    }

                    return list;
                }
                else
                {
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetDirectoryPath()
        {
            try
            {
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        return dialog.SelectedPath;
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 특정경로 파일 실행
        /// </summary>
        /// <param name="filePath"></param>
        public static void FileOpen(string filePath)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(filePath)
            {
                UseShellExecute = true
            };
            p.Start();
        }

        /// <summary>
        /// 파일 복사
        /// </summary>
        /// <param name="CopyFile"></param>
        /// <param name="TargetDirectory"></param>
        /// <param name="TargetFileName"></param>
        /// <returns></returns>
        public static bool CopyFile(FileInfo CopyFile, string TargetDirectory, string TargetFileName)
        {
            try
            {
                string TargetFilePath = TargetDirectory + "\\" + TargetFileName;

                if (Directory.Exists(TargetDirectory) == false)
                    Directory.CreateDirectory(TargetDirectory);

                if (File.Exists(TargetFilePath))
                {
                    File.Delete(TargetFilePath);
                }

                CopyFile.CopyTo(TargetFilePath);
                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 파일 이동
        /// </summary>
        /// <param name="MoveFile">대상 파일</param>
        /// <param name="TargetDirectory">이동할 폴더 경로</param>
        /// <param name="TargetFileName">파일 이름</param>
        /// <returns></returns>
        public static bool MoveFile(FileInfo MoveFile, string TargetDirectory, string TargetFileName)
        {
            try
            {
                string TargetFilePath = TargetDirectory + "\\" + TargetFileName;

                if (Directory.Exists(TargetDirectory) == false)
                    Directory.CreateDirectory(TargetDirectory);

                if (File.Exists(TargetFilePath) == true)
                    File.Delete(TargetFilePath);

                MoveFile.MoveTo(TargetFilePath);
                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static bool SetFileAdd(FileInfo fi, Constant.FileType? fileType, string projectName, string groupName, bool fileCopyorMove = true)
        {
            bool result = false;
            string fileTargetFolderPath = fi.DirectoryName + "\\";

            if (fileCopyorMove)
            {
                if (fileType == Constant.FileType.Document)
                {
                    fileTargetFolderPath = ServiceProvider.StaticService().Config.FileDataPath + "\\" + projectName + "\\" + groupName;

                    bool fileSaveFlag = false;
                    if (ServiceProvider.StaticService().Config.DocumentFileMoveType == Constant.DocumentFileMoveType.Move)
                    {
                        fileSaveFlag = Utility.MoveFile(fi, fileTargetFolderPath, fi.Name);
                    }
                    else
                    {
                        fileSaveFlag = Utility.CopyFile(fi, fileTargetFolderPath, fi.Name);
                    }

                    if (!fileSaveFlag)
                        throw new Exception("File Save Fail.");
                }
            }

            string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("ProjectName", projectName);
            param.Add("Name", fi.Name);
            param.Add("Path", fileTargetFolderPath + fi.Name);
            param.Add("GroupName", groupName);
            param.Add("CreateTime", nowTime);
            param.Add("LastEventTime", nowTime);

            if (Constant.FileType.Document == fileType)
                result = ServiceProvider.StaticService().MainDatabase.SetData("SetDocumentAdd", "0001", param);
            else
            {
                param.Add("Type", "C#");
                result = ServiceProvider.StaticService().MainDatabase.SetData("SetSourceAdd", "0001", param);
            }

            return result;
        }

    }
}
