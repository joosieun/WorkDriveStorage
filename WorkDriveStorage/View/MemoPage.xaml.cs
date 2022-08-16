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
            init();
        }

        private void init()
        {
            DataTable dt = ServiceProvider.StaticService().MainDatabase.GetData("GetDataMemoAll", "0001");

            foreach (DataRow row in dt.Rows)
            {
                Memo memo = new Memo(row["Sequence"].ToString(), row["Name"].ToString(), row["Contents"].ToString());                
                memo.Margin = new Thickness(7);
                MainPanel.Children.Add(memo);
            }
        }

        private void btnAddMemo_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
