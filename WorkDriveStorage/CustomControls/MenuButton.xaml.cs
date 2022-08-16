using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WorkDriveStorage.FrameWork;

namespace WorkDriveStorage.CustomControls
{
    /// <summary>
    /// MenuButton.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MenuButton : UserControl
    {
        public MenuButton()
        {
            InitializeComponent();
        }
        
        public string ButtonText
        {
            get { return TextView.Text.ToString(); }
            set { TextView.Text = value; }
        }

        public string ButtonIconImage
        {
            get { return GetIconName(); }
            set { SetIconName(value); }
        }

        public string Description
        {
            get; set;
        }

        private void btn_MouseEnter(object sender, MouseEventArgs e)
        {
            TextView.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#515EC1");
            //btn.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#EFF2F8");
        }

        private void btn_MouseLeave(object sender, MouseEventArgs e)
        {
            //EFF2F8
            TextView.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF2A2A2A");
            //SolidColorBrush Transparent = new SolidColorBrush(System.Windows.Media.Colors.Transparent);
            //btn.Background = Transparent;
        }


        private void btn_Click(object sender, RoutedEventArgs e)
        {
            Button_Event(sender, e);
        }

        private string GetIconName()
        {
            return PackIcon.Kind.ToString();
        }

        private void SetIconName(string name)
        {
            PackIcon.Kind = (MaterialDesignThemes.Wpf.PackIconKind)Enum.Parse(typeof(MaterialDesignThemes.Wpf.PackIconKind), name, true);
        }


        public delegate void OnButtonClickDelegate(object sender, RoutedEventArgs e, Constant.PageName name, string description);
        public event OnButtonClickDelegate Button_Click;
        private void Button_Event(object sender, RoutedEventArgs e)
        {
            Constant.PageName pageName = (Constant.PageName)Enum.Parse(typeof(Constant.PageName), ButtonText, true);

            Button_Click?.Invoke(sender, e, pageName, this.Description);
        }
    }
}
