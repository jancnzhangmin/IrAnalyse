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

namespace IrAnalyse
{
    /// <summary>
    /// sub_repot_img.xaml 的交互逻辑
    /// </summary>
    public partial class sub_repot_img : UserControl
    {
        public sub_repot_img()
        {
            InitializeComponent();
        }
        public Point rec_point= new Point();
        sub_report sub = MainWindow.FindChild<sub_report>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
        private void rectangle_MouseMove(object sender, MouseEventArgs e)
        {
        
            Rectangle newrect = (Rectangle)sender;
            newrect.Cursor = Cursors.SizeAll;
            
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                
                //if (newrect.Name == "r0")
                {
                    repot_img.Width = repot_img.ActualWidth - (e.GetPosition(repot_canvas).X - rec_point.X);
                    repot_img.Height = repot_img.ActualHeight- (e.GetPosition(repot_canvas).Y - rec_point.Y);
                }

            }
        }

        private void rectangle_MouseDown(object sender, MouseEventArgs e)
        {
           sub = MainWindow.FindChild<sub_report>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            rec_point = e.GetPosition(repot_canvas);
            sub.richTextBox1.Focusable = false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //r0.Opacity = 0;
            //r1.Opacity = 0;
            //r2.Opacity = 0;
            //r3.Opacity = 0;
            //r4.Opacity = 0;
            //r5.Opacity = 0;
            //r6.Opacity = 0;
            //r7.Opacity = 0;
        }

       

    }
}
