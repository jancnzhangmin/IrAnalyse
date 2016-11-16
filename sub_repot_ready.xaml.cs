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
using System.Collections;
using System.IO;

namespace IrAnalyse
{
    /// <summary>
    /// sub_repot_ready.xaml 的交互逻辑
    /// </summary>
    public partial class sub_repot_ready : UserControl
    {
        public sub_repot_ready()
        {
            InitializeComponent();
        }
        public static readonly RoutedEvent ReportReadyMouseDownEvent = EventManager.RegisterRoutedEvent("ReportReadyMouseDown", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(sub_repot_ready));
        public event RoutedPropertyChangedEventHandler<object> ReportReadyMouseDown
        {
            add { AddHandler(ReportReadyMouseDownEvent, value); }
            remove { RemoveHandler(ReportReadyMouseDownEvent, value); }
        }
        public class repot_ready
        {
            public string repot_ready_emss { get; set; }//红外图像的辐射率
            public string repot_ready_distance { get; set; }//红外图像的距离
            public string repot_ready_tamb { get; set; }//红外图像的环境温度
            public string repot_ready_name { get; set; }//报告名字
            public Image repot_ready_image { get; set; }//报告图片
            public ArrayList repot_ready_shapes { get; set; }//报告数据数组
        }
        public string temp_path { get; set; }
        public PngBitmapEncoder encoder = new PngBitmapEncoder();
        public MemoryStream sm;
       public   repot_ready sub_repot = new repot_ready();

       private void repot_image_MouseDown(object sender, MouseButtonEventArgs e)
       {
           Image objImage = sender as Image;
           DragDrop.DoDragDrop(objImage, objImage.Source, DragDropEffects.Copy);
       }

       private void repot_image_MouseUp(object sender, MouseButtonEventArgs e)
       {

       }

       private void delete_report_img_PreviewMouseDown(object sender, MouseButtonEventArgs e)
       {
           RoutedPropertyChangedEventArgs<object> args = new RoutedPropertyChangedEventArgs<object>(this, e);
           args.RoutedEvent = sub_repot_ready.ReportReadyMouseDownEvent;
           this.RaiseEvent(args);
           this.Opacity = 0;
           this.Width = 0;
           this.Height = 0;
       }


      

    }
}
