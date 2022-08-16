using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// CircularProgressBar.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CircularProgressBar : UserControl
    {
        public Func<double, string> Formatter { get; set; }

        public double Value 
        {
            get { return bar.Value; }
            set { bar.Value = value; } 
        }

        public CircularProgressBar()
        {
            InitializeComponent();
            Formatter = chartFormat => chartFormat + "%";
            bar.LabelFormatter = Formatter;
        }
    }
}