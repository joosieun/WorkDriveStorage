using System;
using System.Collections.Generic;
using System.Data;
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
using WorkDriveStorage.FrameWork;

namespace WorkDriveStorage.View
{
    /// <summary>
    /// MemoPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MemoPage : UserControl
    {
        public MemoPage()
        {
            InitializeComponent();
            ItemLoad();
        }

        private void ItemLoad()
        {
            MainPanel.Children.Clear();
            MemoServiceProvider.StaticService().MemoDelete -= MemoPage_ReLoad;
            MemoServiceProvider.StaticService().MemoDelete += MemoPage_ReLoad;

            MemoServiceProvider.StaticService().MemoValeChanged -= MemoPage_ReLoad;
            MemoServiceProvider.StaticService().MemoValeChanged += MemoPage_ReLoad;

            Dictionary<string, MemoItem> items = MemoServiceProvider.StaticService().GetMemoItemAll();
            foreach (KeyValuePair<string, MemoItem> item in items)
            {
                WrapPanel wrapPanel = (WrapPanel)item.Value.Parent;
                if (wrapPanel != null)
                    wrapPanel.Children.Clear();
            }

            foreach (KeyValuePair<string, MemoItem> item in items)
            {
                MainPanel.Children.Add(item.Value);
            }
        }

        private void MemoPage_ReLoad()
        {
            ItemLoad();
        }

        private void btnAddMemo_Click(object sender, RoutedEventArgs e)
        {
            MemoServiceProvider.StaticService().Add();
            ItemLoad();
        }
    }
}
