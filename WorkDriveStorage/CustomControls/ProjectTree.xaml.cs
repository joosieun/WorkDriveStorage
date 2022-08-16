using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

namespace WorkDriveStorage.CustomControls
{
    /// <summary>
    /// ProjectTree.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProjectTree : UserControl
    {
        private Constant.FileType? _fileType;
        private string _projectName = string.Empty;
        private Point _lastMouseDown;
        private TreeViewItem draggedItem, _target;

        public ProjectTree()
        {
            InitializeComponent();
        }

        public void init(Constant.FileType fileType)
        {
            _fileType = fileType;
        }

        public void LoadTreeItem(string projectName)
        {
            if (_fileType == null)
                return;

            _projectName = projectName;

            TreeView.Items.Clear();

            DataTable dt;

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("ProjectName", projectName);

            if (_fileType == Constant.FileType.Document)
                dt = ServiceProvider.StaticService().MainDatabase.GetData("GetDataDocument", "0001", parameters);
            else
                dt = ServiceProvider.StaticService().MainDatabase.GetData("GetDataSource", "0001", parameters);

            Dictionary<string, TreeViewItem> groupTreeDir = new Dictionary<string, TreeViewItem>();
            DataTable dtGroup = dt.DefaultView.ToTable(true, "GroupName");

            TreeViewItem rootTree = new TreeViewItem();
            rootTree.Tag = "Root";
            rootTree.Header = _fileType.ToString();
            rootTree.IsExpanded = true;

            TreeView.Items.Add(rootTree);

            if (dtGroup.Rows.Count > 0)
            {
                foreach (DataRow row in dtGroup.Rows)
                {
                    if (!string.IsNullOrEmpty(row["GroupName"].ToString()))
                    {
                        TreeViewItem groupTree = new TreeViewItem();

                        groupTree.Tag = "Group";
                        groupTree.Header = row["GroupName"].ToString();
                        rootTree.Items.Add(groupTree);

                        groupTreeDir.Add(row["GroupName"].ToString(), groupTree);
                        groupTree.IsExpanded = true;
                    }
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                Image iconTree = new Image();
                iconTree.Source = FrameWork.Utility.GetIconImageSource(row["Path"].ToString());

                ImageTreeViewItem imageTreeItem = new ImageTreeViewItem(row[_fileType + "Name"].ToString(), iconTree);
                imageTreeItem.MouseDoubleClick += ImageTreeItem_MouseDoubleClick;
                imageTreeItem.Tag = row["Path"].ToString();
                imageTreeItem.ToolTip = row["Path"].ToString();

                if (string.IsNullOrEmpty(row["GroupName"].ToString()))
                {
                    rootTree.Items.Add(imageTreeItem);
                }
                else
                {
                    groupTreeDir[row["GroupName"].ToString()].Items.Add(imageTreeItem);
                }
            }
        }

        #region Event.

        private void File_ReName_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem targetItem = (TreeViewItem)TreeView.SelectedItem;

            FileReNameWindow fileReNameWindow = new FileReNameWindow(targetItem.Tag.ToString());
            try
            {
                fileReNameWindow.ShowDialog();
                LoadTreeItem(_projectName);
            }
            catch (Exception ex)
            {
                CustomControlMessageBox.ShowError(ex.Message, "Group Add");
            }
        }

        public delegate void OnFileAddDelegate(Constant.FileType? fileType);
        public event OnFileAddDelegate OnFileAdd;
        private void File_Add_Click(object sender, RoutedEventArgs e)
        {
            if (OnFileAdd != null)
            {
                OnFileAdd(_fileType);
            }
        }

        private void File_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TreeView.SelectedItem != null)
                {
                    TreeViewItem item = (TreeViewItem)TreeView.SelectedItem;
                    MessageBoxResult messageResult = CustomControlMessageBox.ShowWithCancel("삭제 하시겠습니까?", "Delete");
                    if (messageResult == System.Windows.MessageBoxResult.OK)
                    {
                        Dictionary<string, object> parameters = new Dictionary<string, object>();
                        parameters.Add("Path", item.Tag);

                        bool result = false;
                        if (_fileType == Constant.FileType.Document)
                            result = ServiceProvider.StaticService().MainDatabase.SetData("SetDocumentDelete", "0001", parameters);
                        else
                            result = ServiceProvider.StaticService().MainDatabase.SetData("SetSourceDelete", "0001", parameters);

                        if (result)
                        {
                            CustomControlMessageBox.Show("삭제 성공", "Delete");
                            LoadTreeItem(_projectName);
                        }
                        else
                            CustomControlMessageBox.ShowError("삭제 실패", "Delete");
                    }
                }
            }
            catch (Exception ex)
            {
                CustomControlMessageBox.ShowError(ex.Message, "Delete");
            }
        }

        private void File_Copy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeViewItem targetItem = (TreeViewItem)TreeView.SelectedItem;

                FileInfo fi = new FileInfo(targetItem.Tag.ToString());

                if (fi.Exists)
                {
                    Utility.CopyFile(fi, fi.DirectoryName, fi.Name.Replace(fi.Extension, "_Copy" + fi.Extension));

                    FileInfo copyFile = new FileInfo(fi.DirectoryName + "\\" + fi.Name.Replace(fi.Extension, "_Copy" + fi.Extension));

                    if (copyFile.Exists)
                    {
                        TreeViewItem parentItem = (TreeViewItem)targetItem.Parent;
                        string groupName = "";

                        if (parentItem.Tag.ToString() == "Group")
                            groupName = parentItem.Header.ToString();

                        bool result = Utility.SetFileAdd(copyFile, _fileType, _projectName, groupName, false);

                        if (result)
                            LoadTreeItem(_projectName);
                    }
                }

            }
            catch (Exception ex)
            {
                CustomControlMessageBox.ShowError(ex.Message);
            }
        }

        private void Group_Add_Click(object sender, RoutedEventArgs e)
        {
            GroupAddWindow groupAddWindow = new GroupAddWindow();
            try
            {
                groupAddWindow.ShowDialog();

                string groupName = groupAddWindow.GroupName;

                if (string.IsNullOrEmpty(groupName) == false)
                {
                    TreeViewItem rootItem = (TreeViewItem)TreeView.Items[0];
                    TreeViewItem addGroupItem = new TreeViewItem();
                    addGroupItem.Header = groupName;
                    addGroupItem.Tag = "Group";
                    rootItem.Items.Add(addGroupItem);
                }
            }
            catch (Exception ex)
            {
                CustomControlMessageBox.ShowError(ex.Message, "Group Add");
            }
        }

        private void Folder_Open_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeViewItem targetItem = (TreeViewItem)TreeView.SelectedItem;

                FileInfo fi = new FileInfo(targetItem.Tag.ToString());

                if (fi.Exists)
                {
                    System.Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\" + "explorer.exe", fi.DirectoryName);
                }

            }
            catch (Exception ex)
            {
                CustomControlMessageBox.ShowError(ex.Message);
            }
        }

        private void ImageTreeItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeNode = (TreeViewItem)sender;
            ProcessStart(treeNode.Tag.ToString());
        }

        private void TreeContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (string.IsNullOrEmpty(_projectName))
            {
                e.Handled = true;
                return;
            }

            if (TreeView.SelectedItem != null)
            {
                TreeViewItem item = (TreeViewItem)TreeView.SelectedItem;

                switch (item.Tag.ToString())
                {

                    case "Root":
                    case "Group":
                        btnFileReName.Visibility = Visibility.Collapsed;
                        btnFileAdd.Visibility = Visibility.Visible;
                        btnFileDelete.Visibility = Visibility.Collapsed;
                        btnFileCopy.Visibility = Visibility.Collapsed;
                        btnGroupAdd.Visibility = Visibility.Visible;
                        btnFolderOpen.Visibility = Visibility.Collapsed;
                        break;
                    default:
                        btnFileReName.Visibility = _fileType == Constant.FileType.Document ? Visibility.Visible : Visibility.Collapsed;
                        btnFileAdd.Visibility = Visibility.Visible;
                        btnFileDelete.Visibility = Visibility.Visible;
                        btnFileCopy.Visibility = _fileType == Constant.FileType.Document ? Visibility.Visible : Visibility.Collapsed;
                        btnGroupAdd.Visibility = Visibility.Visible;
                        btnFolderOpen.Visibility = Visibility.Visible;
                        break;
                }
            }
            else
            {
                btnFileReName.Visibility = Visibility.Collapsed;
                btnFileAdd.Visibility = Visibility.Visible;
                btnFileDelete.Visibility = Visibility.Collapsed;
                btnFileCopy.Visibility = Visibility.Collapsed;
                btnGroupAdd.Visibility = Visibility.Visible;
                btnFolderOpen.Visibility = Visibility.Collapsed;
            }
        }

        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                Point currentPosition = e.GetPosition(TreeView);


                if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                    (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                {
                    TreeViewItem item = GetNearestContainer(e.OriginalSource as UIElement);
                    if (CheckDropTarget(draggedItem, item))
                    {
                        e.Effects = DragDropEffects.Move;
                    }
                    else
                    {
                        e.Effects = DragDropEffects.None;
                    }
                }
                e.Handled = true;
            }
            catch (Exception)
            {
            }
        }

        private void treeView_Drop(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;

                // Verify that this is a valid drop and then store the drop target
                TreeViewItem TargetItem = GetNearestContainer(e.OriginalSource as UIElement);
                if (TargetItem != null && draggedItem != null)
                {
                    _target = TargetItem;
                    e.Effects = DragDropEffects.Move;

                }
            }
            catch (Exception)
            {
            }
        }

        private void treeView_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point currentPosition = e.GetPosition(TreeView);


                    if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                        (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                    {
                        draggedItem = (TreeViewItem)TreeView.SelectedItem;
                        if (draggedItem != null)
                        {
                            DragDropEffects finalDropEffect = DragDrop.DoDragDrop(TreeView, TreeView.SelectedValue,
                                DragDropEffects.Move);
                            //Checking target is not null and item is dragging(moving)
                            if ((finalDropEffect == DragDropEffects.Move) && (_target != null))
                            {
                                // A Move drop was accepted
                                if (!draggedItem.Header.ToString().Equals(_target.Header.ToString()))
                                {
                                    if (_target.Tag == "Group" || _target.Tag == "Root")
                                        CopyItem(draggedItem, _target);
                                    _target = null;
                                    draggedItem = null;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void TreeView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem TargetItem = GetNearestContainer(e.OriginalSource as UIElement);
            if (TargetItem != null)
            {
                TargetItem.IsSelected = true;
            }
            else
            {
                if (string.IsNullOrEmpty(_projectName) == false)
                {
                    TreeViewItem selectItem = (TreeViewItem)TreeView.SelectedItem;
                    if (selectItem != null)
                        selectItem.IsSelected = false;
                }
            }
        }

        private void TreeView_Main_Drag(object sender, DragEventArgs e)
        {
            if (string.IsNullOrEmpty(_projectName))
                return;

            try
            {
                if (e.Data.GetDataPresent("FileName") == true)
                {
                    string[] targetFileArry = e.Data.GetData(DataFormats.FileDrop) as string[];

                    bool result = false;
                    foreach (string fiPath in targetFileArry)
                    {
                        FileInfo fi = new FileInfo(fiPath);

                        if (fi.Exists)
                        {
                            result = Utility.SetFileAdd(fi, _fileType, _projectName, "");
                            if (result == false)
                                return;
                        }
                    }

                    if (result)
                    {
                        LoadTreeItem(_projectName);
                    }
                }
            }
            catch (Exception ex)
            {
                CustomControlMessageBox.ShowError(ex.Message);
            }
        }

        #endregion

        private void ProcessStart(string path)
        {
            try
            {
                FileInfo fi = new FileInfo(path);
                if (fi.Exists)
                {
                    Utility.FileOpen(path);
                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    parameters.Add("LastEventTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    parameters.Add("Path", path);

                    bool result = false;
                    if (_fileType == Constant.FileType.Document)
                        result = ServiceProvider.StaticService().MainDatabase.SetData("SetDocumentLastTimeUpdate", "0001", parameters);
                    else
                        result = ServiceProvider.StaticService().MainDatabase.SetData("SetSourceLastTimeUpdate", "0001", parameters);
                }
                else
                {
                    CustomControlMessageBox.ShowError("파일이 없습니다.\r\n경로를 확인해 주세요.\r\n" + path, "Project");
                }
            }
            catch (Exception ex)
            {
                CustomControlMessageBox.ShowError(ex.Message, "Project");
            }
        }

        private TreeViewItem GetNearestContainer(UIElement element)
        {
            // Walk up the element tree to the nearest tree view item.
            TreeViewItem container = element as TreeViewItem;
            while ((container == null) && (element != null))
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                container = element as TreeViewItem;
            }
            return container;
        }

        private bool CheckDropTarget(TreeViewItem _sourceItem, TreeViewItem _targetItem)
        {
            bool _isEqual = false;
            if (_sourceItem != null)
            {
                if (!_sourceItem.Header.ToString().Equals(_targetItem.Header.ToString()))
                {
                    _isEqual = true;
                }
            }

            return _isEqual;
        }

        private void CopyItem(TreeViewItem _sourceItem, TreeViewItem _targetItem)
        {
            try
            {
                AddChild(_sourceItem, _targetItem);
                TreeViewItem ParentItem = FindVisualParent<TreeViewItem>(_sourceItem);
                if (ParentItem == null)
                {
                    TreeView.Items.Remove(_sourceItem);
                }
                else
                {
                    ParentItem.Items.Remove(_sourceItem);
                }
            }
            catch
            {

            }
        }

        public void AddChild(TreeViewItem _sourceItem, TreeViewItem _targetItem)
        {
            ImageTreeViewItem tempItem = (ImageTreeViewItem)_sourceItem;
            string tempName = tempItem.ItemName;
            Image iconTree = new Image();
            iconTree.Source = FrameWork.Utility.GetIconImageSource(_sourceItem.Tag.ToString());

            TreeViewItem oldGroupItem = (TreeViewItem)_sourceItem.Parent;
            string groupName = (string)_targetItem.Tag == "Group" ? _targetItem.Header.ToString() : "";
            string updatePath = Constant.FileType.Document == _fileType ? GetUpdateFilePath(_sourceItem.Tag.ToString(), groupName, oldGroupItem.Header.ToString()) : _sourceItem.Tag.ToString();

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("GroupName", groupName);

            bool result = false;
            if (Constant.FileType.Document == _fileType)
            {
                parameters.Add("oldPath", _sourceItem.Tag.ToString());
                parameters.Add("newPath", updatePath);
                result = ServiceProvider.StaticService().MainDatabase.SetData("SetDocumentMove", "0001", parameters);
            }
            else
            {
                parameters.Add("Path", _sourceItem.Tag.ToString());
                result = ServiceProvider.StaticService().MainDatabase.SetData("SetSourceMove", "0001", parameters);
            }

            if (result)
            {
                if (Constant.FileType.Document == _fileType)
                {
                    FileInfo oldfi = new FileInfo(_sourceItem.Tag.ToString());
                    FileInfo fi = new FileInfo(updatePath);

                    Utility.MoveFile(oldfi, fi.DirectoryName, oldfi.Name);
                }

                ImageTreeViewItem item1 = new ImageTreeViewItem(tempName, iconTree);
                item1.MouseDoubleClick += ImageTreeItem_MouseDoubleClick;
                item1.Tag = updatePath;
                item1.ToolTip = updatePath;
                _targetItem.Items.Add(item1);
            }
        }

        private string GetUpdateFilePath(string oldPath, string newGroupName, string oldGroupName)
        {
            string[] oldPathArray = oldPath.Split('\\');
            if (newGroupName == "")
            {
                oldPathArray = oldPathArray.Where(val => val != oldGroupName).ToArray();
            }
            else
            {
                if (oldGroupName == Constant.FileType.Document.ToString() || oldGroupName == Constant.FileType.Source.ToString())
                {
                    List<string> list = new List<string>(oldPathArray.ToList());
                    list.Insert(list.Count - 1, newGroupName);
                    oldPathArray = list.ToArray();
                }
                else
                {
                    oldPathArray = oldPathArray.Where(val => val != oldGroupName).ToArray();

                    List<string> list = new List<string>(oldPathArray.ToList());
                    list.Insert(list.Count - 1, newGroupName);
                    oldPathArray = list.ToArray();
                }
            }

            return string.Join("\\", oldPathArray);
        }

        internal static TObject FindVisualParent<TObject>(UIElement child) where TObject : UIElement
        {
            if (child == null)
            {
                return null;
            }

            UIElement parent = VisualTreeHelper.GetParent(child) as UIElement;

            while (parent != null)
            {
                TObject found = parent as TObject;
                if (found != null)
                {
                    return found;
                }
                else
                {
                    parent = VisualTreeHelper.GetParent(parent) as UIElement;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// 이미지 트리 아이템
    /// </summary>
    internal class ImageTreeViewItem : TreeViewItem
    {
        public string ItemName { get; set; }
        public Image Image { get; set; }

        public ImageTreeViewItem(string name, Image image)
        {
            CreateTreeViewItemTemplate(name, image);
            ItemName = name;
            Image = image;
        }

        #region Private Methods

        private void CreateTreeViewItemTemplate(string name, Image image)
        {
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;

            image.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            image.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            image.Width = 16;
            image.Height = 16;
            image.Margin = new Thickness(2);
            stack.Children.Add(image);

            TextBlock _textBlock = new TextBlock();
            _textBlock.Margin = new Thickness(2);
            _textBlock.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            _textBlock.Text = name;

            stack.Children.Add(_textBlock);

            Header = stack;
        }

        #endregion
    }
}
