using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using WorkDriveStorage.Popup;

namespace WorkDriveStorage.CustomControls
{
    /// <summary>
    /// Memo.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MemoItem : UserControl
    {
        public string Contents {
            get { return txtContents.Text; }
            set { txtContents.Text = value; }
        }
        public string ContentsRTF { get; set; }
        public string Memokey { get; set; }
        public bool WallPaper { get; set; }

        public MemoItem(string key, string contents, string contentsRTF, bool wallPaper)
        {
            InitializeComponent();
            Contents = contents;
            Memokey = key;
            this.DataContext = this;
            ContentsRTF = contentsRTF;
            WallPaper = wallPaper;

            if(wallPaper)
                btnShare.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF4155EE");
        }

        private void Card_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            MemoWindow win = MemoServiceProvider.StaticService().GetMemoWindow(Memokey);

            if (win != null)
            {
                Thread startThread = new Thread(() => WindowActivate(win));
                startThread.Start();
            }
            else
            {
                MemoWindow newWin = new MemoWindow(Memokey, Contents);
                MemoServiceProvider.StaticService().MemoWindowShow(Memokey, newWin);
            }
        }

        private void WindowActivate(MemoWindow win)
        {
            Thread.Sleep(50);
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate { win.Activate(); }));
        }

        private void Card_MouseEnter(object sender, MouseEventArgs e)
        {
            this.MemoCard.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF8E99EE");
            this.MemoCard.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFFFFFF");
        }

        private void Card_MouseLeave(object sender, MouseEventArgs e)
        {
            this.MemoCard.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFFFFFF");
            this.MemoCard.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFFFFFFF");
        }

        private void btnShare_Click(object sender, RoutedEventArgs e)
        {
            if (this.WallPaper)
            {
                this.WallPaper = false;
                MemoServiceProvider.StaticService().WallPaperChanged(Memokey, this.WallPaper);
                btnShare.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF000000");

            }
            else
            {
                this.WallPaper = true;
                MemoServiceProvider.StaticService().WallPaperChanged(Memokey, this.WallPaper);
                btnShare.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF4155EE");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            MemoServiceProvider.StaticService().Delete(Memokey);
        }
    }
}
