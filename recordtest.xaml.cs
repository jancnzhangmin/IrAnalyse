using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Emgu.CV;

namespace IrAnalyse
{
    /// <summary>
    /// recordtest.xaml 的交互逻辑
    /// </summary>
    public partial class recordtest : UserControl
    {
        public recordtest()
        {
            InitializeComponent();
        }

        private void record_Click(object sender, RoutedEventArgs e)
        {
            Capture capture = new Capture();
            VideoWriter videowriter = new VideoWriter("aaa.wav",CvInvoke.
        }
    }
}
