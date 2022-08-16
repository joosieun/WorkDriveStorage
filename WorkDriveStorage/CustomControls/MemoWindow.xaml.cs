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
using System.Windows.Threading;

namespace WorkDriveStorage.CustomControls
{
    /// <summary>
    /// MemoWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MemoWindow : Window
    {
        public MemoWindow()
        {
            InitializeComponent();
        }

        private void txtMemo_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void DockPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Media.Animation.Storyboard goBikeSB = this.Resources["CloseMenu"] as System.Windows.Media.Animation.Storyboard;
            Action action = () => { this.BeginStoryboard(goBikeSB); };
            Dispatcher.BeginInvoke(action, DispatcherPriority.Render);
        }

        private void txtMemo_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Windows.Media.Animation.Storyboard goBikeSB = this.Resources["OpenMenu"] as System.Windows.Media.Animation.Storyboard;
            Action action = () => { this.BeginStoryboard(goBikeSB); };
            Dispatcher.BeginInvoke(action, DispatcherPriority.Render);
        }
    }
}
