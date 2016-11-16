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
using System.Windows.Media.Animation;

namespace IrAnalyse
{
    /// <summary>
    /// sub_ellipse_animation.xaml 的交互逻辑
    /// </summary>
    public partial class sub_ellipse_animation : UserControl
    {
        public sub_ellipse_animation()
        {
            InitializeComponent();
        }

        //System.Windows.Shapes.Path font_eip_path = new System.Windows.Shapes.Path();

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
            EllipseGeometry font_eip = new EllipseGeometry();
            font_eip.Center = new Point(3,3);
            font_eip.RadiusX = 6;
            font_eip.RadiusY = 6;
            font_eip_path.Data = font_eip;
            font_eip_path.Fill = new SolidColorBrush(Color.FromRgb((byte)0, (byte)0xcc, (byte)0xff));
            //System.Windows.Shapes.Path back_eip_path = new System.Windows.Shapes.Path();


            EllipseGeometry back_eip = new EllipseGeometry();
            back_eip.Center = new Point(3, 3);
            back_eip.RadiusX = 6;
            back_eip.RadiusY = 6;

            DoubleAnimation radiusanimation = new DoubleAnimation();
            radiusanimation.From = 0;
            radiusanimation.To = 20;
            radiusanimation.Duration = TimeSpan.FromSeconds(2);
            radiusanimation.RepeatBehavior = RepeatBehavior.Forever;
            back_eip.BeginAnimation(EllipseGeometry.RadiusXProperty, radiusanimation);
            back_eip.BeginAnimation(EllipseGeometry.RadiusYProperty, radiusanimation);

            back_eip_path.Data = back_eip;
            back_eip_path.Fill = new SolidColorBrush(Color.FromRgb((byte)0, (byte)0xcc, (byte)0xff));
            DoubleAnimation opacityanimation = new DoubleAnimation();
            opacityanimation.From = 1;
            opacityanimation.To = 0;
            opacityanimation.Duration = TimeSpan.FromSeconds(2);
            opacityanimation.RepeatBehavior = RepeatBehavior.Forever;
            back_eip_path.BeginAnimation(System.Windows.Shapes.Path.OpacityProperty, opacityanimation);
            
            

            //ell_grid.Children.Add(back_eip_path);
        }

        private void base_canvas_MouseEnter(object sender, MouseEventArgs e)
        {

                DoubleAnimation opacityanimation = new DoubleAnimation();
                opacityanimation.To = 0.5;
                opacityanimation.Duration = TimeSpan.FromSeconds(0.5);
                eip_canvas.BeginAnimation(Canvas.OpacityProperty, opacityanimation);
            
        }

        private void base_canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation opacityanimation = new DoubleAnimation();
            opacityanimation.To = 0;
            opacityanimation.Duration = TimeSpan.FromSeconds(0.5);
            eip_canvas.BeginAnimation(Canvas.OpacityProperty, opacityanimation);
        }




    }
}
