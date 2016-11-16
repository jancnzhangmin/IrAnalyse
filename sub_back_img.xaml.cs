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
    /// sub_back_img.xaml 的交互逻辑
    /// </summary>
    public partial class sub_back_img : UserControl
    {
        public sub_back_img()
        {
            InitializeComponent();
        }

        private void ir_back_touming_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sub_workspace work = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            work.ir_canvas_font.Opacity = ir_back_touming.Value;
            work.create_img();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            sub_workspace work = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);

           RenderTargetBitmap repot_image = new RenderTargetBitmap((int)work.ir_canvas.ActualWidth, (int)work.ir_canvas.ActualHeight, 96, 96, PixelFormats.Default);
            repot_image.Render(work.ir_canvas);

            //report_shapes.Clear();
            //zoom_coe = repot_zoomcoe;
            //zoom();
          report_image.Source = repot_image;
          img_tooltip.Source = repot_image;
         img_tooltip.Width = repot_image.Width * 0.7;
          img_tooltip.Height = repot_image.Height * 0.7;
        }

       


    }
}
