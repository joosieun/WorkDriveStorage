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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WorkDriveStorage.CustomControls
{
    /// <summary>
    /// Memo.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Memo : UserControl
    {
        public string MemoName { get; set; }
        public string Contents { get; set; }
        public string Memokey { get; set; }

        public Memo(string key, string memoName, string contents)
        {
            InitializeComponent();
            MemoName = memoName;
            Contents = contents;
            Memokey = key;
            this.DataContext = this;
        }

        public Memo()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Card_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            MemoWindow memoWindow = new MemoWindow();
            memoWindow.Show();
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
    }
}
