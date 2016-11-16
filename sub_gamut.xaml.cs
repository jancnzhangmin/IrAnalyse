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
using System.Threading;
using System.Text.RegularExpressions;

namespace IrAnalyse
{
    /// <summary>
    /// sub_gamut.xaml 的交互逻辑
    /// </summary>
    public partial class sub_gamut : UserControl
    {
        public sub_gamut()
        {
            InitializeComponent();
        }
        List<double> percent = new List<double>();//256个像素点百分比
        int max_temp;
        int min_temp;
        double refer_max_temp;
        double refer_min_temp;
        double min_distance=400d / 256d;//线最小间距
        public Point oldpoint;
        public double distance=410d;
       // public bool lock_duibidu = false;
        public bool shoudong_checked = false;
        public string ctr_name="";
        Thickness topmargin;
        Thickness leftmargin;
        Thickness rightmargin ;
        Thickness old_top_margin;

        int old_contrast_step=0;
        int old_middle_num=0;

        double old_min_num;
        double old_max_num;
        double old_distance;
        

        private void get_percent()//计算像素点百分比
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
             max_temp = (from c in workspace.ir_temp select c).Max();
             min_temp = (from c in workspace.ir_temp select c).Min();
            double min_uint = (max_temp - min_temp) / 256d;
            for (double i = min_temp; i < max_temp; i += min_uint)
            {
                double t = (from c in workspace.ir_temp where c >= i && c <= i + min_uint select c).Count();
                double temp_percent = t / (double)workspace.ir_temp.Count;
                percent.Add(temp_percent);
            }
            
        }

        private void create_line()
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            line_canvas.Children.Clear();
            double line_height = (from c in percent select c).Max();

           // double cc = min_distance;
            
            for (int i = 0; i < percent.Count; i++)
            {
                percent[i] = percent[i] / line_height * 199d;
                Line newline = new Line();
                newline.X1 = (double)i * min_distance;
                newline.Y1 = 199 - percent[i];
                newline.X2 = (double)i * min_distance;
                newline.Y2 = 199;

                newline.StrokeThickness = min_distance;
                newline.StrokeThickness = 1;

                int pal = (int)(i * min_distance / 400 * 256);
                if (pal > 255)
                {
                    pal = 255;
                }
                else if (pal < 0)
                {
                    pal = 0;
                }

                byte R = Convert.ToByte(workspace.cur_palette[pal].ToString().Substring(0, 2), 16);
                byte G = Convert.ToByte(workspace.cur_palette[pal].ToString().Substring(2, 2), 16);
                byte B = Convert.ToByte(workspace.cur_palette[pal].ToString().Substring(4, 2), 16);
                newline.Stroke = new SolidColorBrush(Color.FromRgb(R, G, B));
                // newpolyline.Stroke = new SolidColorBrush(Color.FromRgb((byte)51, (byte)102, (byte)255));
                newline.SnapsToDevicePixels = true;
                if (newline.X1 + 1 < 399&&newline.X1+1>0)
                {
                    line_canvas.Children.Add(newline);
                }

                              
            }

            int[] pixels = new int[1];
            int stride = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;


            WriteableBitmap cur_palette_bmp = new WriteableBitmap(1, 256, 96, 96, PixelFormats.Bgr32, null);
            for (int i = 0; i < 256; i++)
            {
                int pixel = Convert.ToInt32(workspace.cur_palette[i].ToString(), 16);
                pixels[0] = pixel;
                cur_palette_bmp.WritePixels(new Int32Rect(0, i, 1, 1), pixels, stride, 0);
            }

            cur_palette_img.Source = cur_palette_bmp;





        }


        private void return_value()
        {



            //Thread newthread = new Thread(new ThreadStart(() =>
            //{
            //    Dispatcher.Invoke(new Action(() =>
            //    {





            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            if (shoudong_checked)
            {
                workspace.contrast_step = (int)slider_duibidu.Value;
                try
                {
                    //double b_coe;
                    //b_coe = (-slider_duibidu.Value + 0.0) / 100.0 * (max_temp - min_temp) / 2.0;
                    workspace.refer_middle_temp = middle_num.Value * 10;
                    workspace.grad_max =double.Parse( right_value.Content.ToString())*10d;
                    workspace.grad_min = double.Parse(left_value.Content.ToString()) * 10d;
                }
                catch { }
            }





            try
            {
                if (old_contrast_step != workspace.contrast_step || old_middle_num != (int)(workspace.refer_middle_temp / 2d) || old_max_num != max_num.Value || old_min_num != min_num.Value)
                {

                    workspace.create_img();
                    old_contrast_step = workspace.contrast_step;
                    old_middle_num = (int)(workspace.refer_middle_temp / 2d);
                    old_max_num = max_num.Value;
                    old_min_num = min_num.Value;
                    workspace.calculate_temp_ruler();

                }
            }
            catch { }
 





            

       //        }));



       //}));
       //     newthread.SetApartmentState(ApartmentState.MTA);
       //     newthread.IsBackground = false;
       //     //newthread.Priority = ThreadPriority.AboveNormal;
       //     newthread.Start();
       //    // Thread.Sleep(10);



           
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
             sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            //min_num.ValueChanged-=new EventHandler<C1.WPF.PropertyChangedEventArgs<double>>(min_num_ValueChanged);
            //middle_num.ValueChanged += new EventHandler<C1.WPF.PropertyChangedEventArgs<double>>(middle_num_ValueChanged);
            //max_num.ValueChanged += new EventHandler<C1.WPF.PropertyChangedEventArgs<double>>(max_num_ValueChanged);
            get_percent();
            min_distance = 400d / 256d;
            create_line();
            create_rect();
            refer_max_temp = workspace.refer_max_temp;
            refer_min_temp = workspace.refer_min_temp;
            min_num.Value = workspace.refer_min_temp/10d;
            middle_num.Value =  workspace.refer_middle_temp/10d;

            max_num.Value = workspace.refer_max_temp / 10d;
            left_value.Content = workspace.refer_min_temp / 10d;
            right_value.Content = workspace.refer_max_temp / 10d;
         

            workspace.refer_middle_temp = middle_num.Value * 10;
            auto_default();
            shoudong_checked = false;



            //unregister_handle();

            //min_num.ValueChanged-=new EventHandler<C1.WPF.PropertyChangedEventArgs<double>>(min_num_ValueChanged);
            //middle_num.ValueChanged-=new EventHandler<C1.WPF.PropertyChangedEventArgs<double>>(middle_num_ValueChanged);
            //max_num.ValueChanged-=new EventHandler<C1.WPF.PropertyChangedEventArgs<double>>(max_num_ValueChanged);

          
       
        }

        private void auto_default()
        {
           
                gamut_auto.IsChecked = true;
                reset.IsEnabled = false;
                slider_liangdu.IsEnabled = false;
                slider_duibidu.IsEnabled = false;
                min_num.IsEnabled = false;
                middle_num.IsEnabled = false;
                max_num.IsEnabled = false;
            
        }


        private void create_rect()
        {
            //for (int i = 0; i < 400; i += 10)
            //{
            //    Rectangle newrect = new Rectangle();
            //    newrect.Width = 10;
            //    newrect.Height = 20;
            //    newrect.Stroke = new SolidColorBrush(Color.FromRgb(120, 180, 228));
            //    newrect.SnapsToDevicePixels = true;
            //    newrect.Margin = new Thickness(i+1, 0, 0, 0);
            //    slider_canvas.Children.Add(newrect);
            //}

            Rectangle newrect = new Rectangle();
            newrect.Width = 400;
            newrect.Height = 20;
            newrect.Stroke = new SolidColorBrush(Color.FromRgb(164, 200, 224));
            newrect.SnapsToDevicePixels = true;
            slider_canvas.Children.Add(newrect);
            for (int i = 10; i < 400; i += 10)
            {
                Line newline = new Line();
                newline.X1 = i;
                newline.Y1 = 0;
                newline.X2 = i;
                newline.Y2 = 20;
                newline.SnapsToDevicePixels = true;
                newline.Stroke = new SolidColorBrush(Color.FromRgb(164, 200, 224));
                slider_canvas.Children.Add(newline);

            }
            
        }

        private void left_lab_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            oldpoint = e.GetPosition(null);
            //leftmargin = left_lab.Margin;
            old_distance = distance;
            ctr_name = "left_lab";
            unregister_handle();
            left_lab.PreviewMouseMove+=new MouseEventHandler(main_canvas_PreviewMouseMove);
          //  middle_num.ValueChanged+=new EventHandler<C1.WPF.PropertyChangedEventArgs<double>>(middle_num_ValueChanged);
            //left_lab.PreviewMouseMove -= new MouseEventHandler(main_canvas_PreviewMouseMove);
            
          
        }

        //private void left_lab_PreviewMouseMove(object sender, MouseEventArgs e)
        //{
        //    if (shoudong_checked)
        //    {
        //        if (e.LeftButton == MouseButtonState.Pressed)
        //        {

        //            if (left_lab.Margin.Left < -15)
        //            {
        //                left_lab.Margin = new Thickness(-15, -5, 0, 0);
        //            }
        //            else if (left_lab.Margin.Left > right_lab.Margin.Left - 10)
        //            {
        //                left_lab.Margin = new Thickness(right_lab.Margin.Left - 10, -5, 0, 0);
        //            }



        //            left_lab.Margin = new Thickness(oldpoint.X + (e.GetPosition(slider_canvas).X - oldpoint.X) - 10, -5, 0, 0);
        //            slider_duibidu.Value = 128d - (left_lab.Margin.Left + 15d) * 128d / 410d;



        //            change_lab_margin("left_lab");
        //        }
        //    }
        //}

        private void unregister_handle()
        {
            //left_lab.PreviewMouseMove -= new MouseEventHandler(main_canvas_PreviewMouseMove);
            //main_canvas.PreviewMouseMove-=new MouseEventHandler(main_canvas_PreviewMouseMove);
            slider_duibidu.ValueChanged-=new RoutedPropertyChangedEventHandler<double>(slider_duibidu_ValueChanged);
            slider_liangdu.ValueChanged-=new RoutedPropertyChangedEventHandler<double>(slider_liangdu_ValueChanged);
            middle_num.ValueChanged -= new EventHandler<C1.WPF.PropertyChangedEventArgs<double>>(middle_num_ValueChanged);

        }



        private void change_lab_margin(string lab_name)
        {
            //unregister_handle();
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            topmargin = top_lab.Margin;
            leftmargin = left_lab.Margin;
            rightmargin = right_lab.Margin;
  
            if (lab_name == "left_lab" || lab_name=="right_lab")
            {
                top_lab.Margin = new Thickness(left_lab.Margin.Left + (distance)/2d, -15, 0, 0);
                middle_num.Value = Math.Round((((slider_liangdu.Value + 100d) / 200d) * (refer_max_temp - refer_min_temp) + refer_min_temp) / 10d, 1);
            }
            if (lab_name == "top_lab")
            {
                
                left_lab.Margin = new Thickness(top_lab.Margin.Left - distance/2d, -5, 0, 0);
                right_lab.Margin = new Thickness(top_lab.Margin.Left+distance/2d, -5, 0, 0);


                if (left_lab.Margin.Left < -15)
                {
                    left_lab.Margin = new Thickness(-15, -5, 0, 0);
                    top_lab.Margin = new Thickness(left_lab.Margin.Left + distance / 2d, -15, 0, 0);
                    right_lab.Margin = new Thickness(left_lab.Margin.Left + distance, -5, 0, 0);
                }
                if (right_lab.Margin.Left > 395)
                {
                    right_lab.Margin = new Thickness(395, -5, 0, 0);
                    left_lab.Margin = new Thickness(right_lab.Margin.Left - distance, -5, 0, 0);
                    top_lab.Margin = new Thickness(right_lab.Margin.Left - distance / 2d, -15, 0, 0);
                   
                }
                middle_num.Value = Math.Round((((slider_liangdu.Value + 100d) / 200d) * (refer_max_temp - refer_min_temp) + refer_min_temp) / 10d, 1);

            }

            if (lab_name == "slider_liangdu")
            {
                //top_lab.Margin = new Thickness((slider_liangdu.Value + 100) / 200 * 410 - 15, -15, 0, 0);
                ////left_lab.Margin = new Thickness(top_lab.Margin.Left - distance / 2, -5, 0, 0);
                ////right_lab.Margin = new Thickness(top_lab.Margin.Left + distance / 2, -5, 0, 0);





                if (left_lab.Margin.Left == -15 && right_lab.Margin.Left == 395)
                {
                    top_lab.Margin = new Thickness(190, -15, 0, 0);
                }
                else 
                {

                    top_lab.Margin = new Thickness((slider_liangdu.Value + 100) / 200 * 410 - 15, -15, 0, 0);
                    distance = right_lab.Margin.Left - left_lab.Margin.Left;
                    left_lab.Margin = new Thickness(top_lab.Margin.Left - distance / 2, -5, 0, 0);
                    right_lab.Margin = new Thickness(top_lab.Margin.Left + distance / 2, -5, 0, 0);


                    if (left_lab.Margin.Left < -15)
                    {
                        left_lab.Margin = new Thickness(-15, -5, 0, 0);
                        top_lab.Margin = new Thickness(left_lab.Margin.Left+distance/2d, -15, 0, 0);

                        right_lab.Margin = new Thickness(top_lab.Margin.Left + distance / 2, -5, 0, 0);
                    }
                  

                    if (right_lab.Margin.Left > 390)
                    {
                        right_lab.Margin = new Thickness(395, -5, 0, 0);
                        top_lab.Margin = new Thickness(right_lab.Margin.Left-distance/2d, -15, 0, 0);
                        left_lab.Margin = new Thickness(right_lab.Margin.Left - distance, -5, 0, 0);
                    }
                 
                }
             double temp_slider_liangdu=Math.Round((((slider_liangdu.Value + 100d) / 200d) * (refer_max_temp - refer_min_temp) + refer_min_temp) / 10d, 1);
                middle_num.ValueChanged-=new EventHandler<C1.WPF.PropertyChangedEventArgs<double>>(middle_num_ValueChanged);
                if (temp_slider_liangdu != middle_num.Value)
                {
                    middle_num.Value = temp_slider_liangdu;
                }
           
          


                
               

            }



            if (lab_name == "middle_num")
            {
                //top_lab.Margin = new Thickness((slider_liangdu.Value + 100) / 200 * 410 - 15, -15, 0, 0);
                ////left_lab.Margin = new Thickness(top_lab.Margin.Left - distance / 2, -5, 0, 0);
                ////right_lab.Margin = new Thickness(top_lab.Margin.Left + distance / 2, -5, 0, 0);





                if (left_lab.Margin.Left == -15 && right_lab.Margin.Left == 395)
                {
                    top_lab.Margin = new Thickness(190, -15, 0, 0);
                }
                else
                {

                    top_lab.Margin = new Thickness((slider_liangdu.Value + 100) / 200 * 410 - 15, -15, 0, 0);
                   // distance = right_lab.Margin.Left - left_lab.Margin.Left;
                    left_lab.Margin = new Thickness(top_lab.Margin.Left - distance / 2, -5, 0, 0);
                    right_lab.Margin = new Thickness(top_lab.Margin.Left + distance / 2, -5, 0, 0);


                    if (left_lab.Margin.Left < -15)
                    {
                        left_lab.Margin = new Thickness(-15, -5, 0, 0);
                        top_lab.Margin = new Thickness(left_lab.Margin.Left + distance / 2d, -15, 0, 0);

                        right_lab.Margin = new Thickness(top_lab.Margin.Left + distance / 2, -5, 0, 0);
                    }


                    if (right_lab.Margin.Left > 390)
                    {
                        right_lab.Margin = new Thickness(395, -5, 0, 0);
                        top_lab.Margin = new Thickness(right_lab.Margin.Left - distance / 2d, -15, 0, 0);
                        left_lab.Margin = new Thickness(right_lab.Margin.Left - distance, -5, 0, 0);
                    }

                }

                //middle_num.Value = Math.Round((((slider_liangdu.Value + 100d) / 200d) * (refer_max_temp - refer_min_temp) + refer_min_temp) / 10d, 1);






            }




            if (lab_name == "slider_duibidu")
            {
                //if (left_lab.Margin.Left == top_lab.Margin.Left && right_lab.Margin.Left ==top_lab.Margin.Left)
                //{
                //    left_lab.Margin = top_lab.Margin;
                //    right_lab.Margin = top_lab.Margin;
                //}
                //else
                //{
                //    //distance = right_lab.Margin.Left - left_lab.Margin.Left;
                //    //left_lab.Margin = new Thickness(right_lab.Margin.Left - distance, -5, 0, 0);
                //    //right_lab.Margin = new Thickness(top_lab.Margin.Left + distance / 2, -5, 0, 0);
                  

                //        left_lab.Margin = new Thickness((128d - slider_duibidu.Value) / 128d * (top_lab.Margin.Left + 15d) - 15, -5, 0, 0);
                //        right_lab.Margin = new Thickness((128d - slider_duibidu.Value) / 128d * (top_lab.Margin.Left - 395d) + 395, -5, 0, 0);
                //       // distance = left_lab.Margin.Left - right_lab.Margin.Left;
                //        if (left_lab.Margin.Left < -15)
                //        {
                //            left_lab.Margin = new Thickness(-15, -5, 0, 0);
                //            right_lab.Margin = new Thickness(left_lab.Margin.Left + distance, -5, 0, 0);
                //            top_lab.Margin=new Thickness(left_lab.Margin.Left+distance/2d,-5,0,0);
                //        }

                //        if (right_lab.Margin.Left > 395)
                //        {
                //            right_lab.Margin = new Thickness(395, -5, 0, 0);
                //            left_lab.Margin = new Thickness(right_lab.Margin.Left - distance, -5, 0, 0);
                //            top_lab.Margin = new Thickness(right_lab.Margin.Left - distance / 2, -5, 0, 0);
                //        }




                //    //left_lab.Margin = new Thickness((128d - slider_duibidu.Value) / 128d * 205 - 15, -5, 0, 0);
                //    //right_lab.Margin = new Thickness((128d - slider_duibidu.Value) / 128d * (-205) + 395, -5, 0, 0);
                //}
                //left_lab.Margin = new Thickness((128d - slider_duibidu.Value) / 127d * (top_lab.Margin.Left + 15d) - 15, -5, 0, 0);
                //right_lab.Margin = new Thickness((128d - slider_duibidu.Value) / 127d * (top_lab.Margin.Left - 395d) + 395, -5, 0, 0);
               // distance = rightmargin.Left - leftmargin.Left;
                double old_distance = distance;
                distance = (double)(slider_duibidu.Value - 1) / 127d * 400d + 10d;
                left_lab.Margin = new Thickness(top_lab.Margin.Left - distance / 2d, -5, 0, 0);
                right_lab.Margin = new Thickness(top_lab.Margin.Left + distance / 2d, -5, 0, 0);


                if (left_lab.Margin.Left < -15 || right_lab.Margin.Left > 395)
                {
                    left_lab.Margin = leftmargin;
                    right_lab.Margin = rightmargin;
                    top_lab.Margin = topmargin;
                    distance = old_distance;
                    //lock_duibidu = true;

                    slider_duibidu.Value = (int)((distance-10d) / 400d * 127d + 1d);
                    

                    
                    //middle_num.Value = slider_duibidu.Value;
                }
               
               
               
            }


            if (lab_name == "min_num" || lab_name == "max_num")
            {
                refer_max_temp = max_num.Value * 10d;
                refer_min_temp = min_num.Value * 10d;
                min_distance = (refer_max_temp - refer_min_temp) / (workspace.max_temp - workspace.min_temp) * 400d / 256d;
                create_line();
            }



          
                cur_palette_img.Margin = new Thickness(left_lab.Margin.Left + 15, 20, 0, 0);
                if (right_lab.Margin.Left - left_lab.Margin.Left - 10>=0)
            {
                cur_palette_img.Height = right_lab.Margin.Left - left_lab.Margin.Left - 10;
            }

                if (lab_name != "middle_num")
                {

                    slider_liangdu.Value = (top_lab.Margin.Left + 15d) / 410d * 200d - 100d;
                    //middle_change = false;

                } 
                left_value.Content = Math.Round(((left_lab.Margin.Left + 15) / 410d * (refer_max_temp - refer_min_temp) + refer_min_temp) / 10d, 1);
                right_value.Content = Math.Round(((right_lab.Margin.Left + 15) / 410d * (refer_max_temp - refer_min_temp) + refer_min_temp) / 10d, 1);



            return_value();

                // Thread.Sleep(10);


          
        }

        private void right_lab_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            oldpoint = e.GetPosition(null);
            ctr_name = "right_lab";
            old_distance = distance;
            unregister_handle();
            right_lab.PreviewMouseMove += new MouseEventHandler(main_canvas_PreviewMouseMove);
        }

        //private void right_lab_PreviewMouseMove(object sender, MouseEventArgs e)
        //{
        //    if (shoudong_checked)
        //    {
        //        if (e.LeftButton == MouseButtonState.Pressed)
        //        {
        //            right_lab.Margin = new Thickness(oldpoint.X + (e.GetPosition(slider_canvas).X - oldpoint.X) - 10, -5, 0, 0);
        //            if (right_lab.Margin.Left > 390)
        //            {
        //                right_lab.Margin = new Thickness(395, -5, 0, 0);
        //            }
        //            else if (right_lab.Margin.Left < left_lab.Margin.Left + 10)
        //            {
        //                right_lab.Margin = new Thickness(left_lab.Margin.Left + 10, -5, 0, 0);
        //            }






        //            change_lab_margin("right_lab");
        //            slider_duibidu.Value = (right_lab.Margin.Left + 15d) * 128d / 410d;
        //        }
        //    }
        //}

        private void top_lab_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        
            oldpoint = e.GetPosition(null);
          // distance = right_lab.Margin.Left - left_lab.Margin.Left;
           // old_distance = distance;
            ctr_name = "top_lab";
            old_top_margin = top_lab.Margin;
            unregister_handle();
            top_lab.PreviewMouseMove += new MouseEventHandler(main_canvas_PreviewMouseMove);







        }

        //private void top_lab_PreviewMouseMove(object sender, MouseEventArgs e)
        //{
        //    if (shoudong_checked)
        //    {
        //        if (e.LeftButton == MouseButtonState.Pressed)
        //        {
        //            top_lab.Margin = new Thickness(oldpoint.X + (e.GetPosition(slider_canvas).X - oldpoint.X) - 20, -15, 0, 0);

        //            if (top_lab.Margin.Left > 390)
        //            {
        //                top_lab.Margin = new Thickness(395, -15, 0, 0);
        //            }
        //            else if (top_lab.Margin.Left < top_lab.Margin.Left + 10)
        //            {
        //                top_lab.Margin = new Thickness(top_lab.Margin.Left + 10, -15, 0, 0);
        //            }
        //            if (left_lab.Margin.Left < -15)
        //            {
        //                left_lab.Margin = new Thickness(-15, -5, 0, 0);
        //                top_lab.Margin = new Thickness(distance / 2d + left_lab.Margin.Left, -15, 0, 0);

        //            }
        //            if (right_lab.Margin.Left > 395)
        //            {
        //                right_lab.Margin = new Thickness(395, -5, 0, 0);
        //                top_lab.Margin = new Thickness(right_lab.Margin.Left - distance / 2d, -15, 0, 0);
        //            }
        //            change_lab_margin("top_lab");
        //        }
        //    }
        //}

        private void slider_duibidu_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //return_value();
            //e.Handled = false;
        
                change_lab_margin("slider_duibidu");
            //if (!lock_duibidu)
            //{
            //    change_lab_margin("slider_duibidu");
            //    lock_duibidu = false;
            //}
    
            
           

        }

        private void slider_liangdu_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
            //if (top_lab.Margin.Left > 390)
            //{
            //    top_lab.Margin = new Thickness(395, -15, 0, 0);
            //}
            //else if (top_lab.Margin.Left < top_lab.Margin.Left + 10)
            //{
            //    top_lab.Margin = new Thickness(top_lab.Margin.Left + 10, -15, 0, 0);
            //}
            //if (left_lab.Margin.Left < -15)
            //{
            //    left_lab.Margin = new Thickness(-15, -5, 0, 0);
            //    top_lab.Margin = new Thickness(distance / 2d + left_lab.Margin.Left, -15, 0, 0);

            //}
            //if (right_lab.Margin.Left > 395)
            //{
            //    right_lab.Margin = new Thickness(395, -5, 0, 0);
            //    top_lab.Margin = new Thickness(right_lab.Margin.Left - distance / 2d, -15, 0, 0);
            //}

           change_lab_margin("slider_liangdu");

           

         




        }

        private void middle_num_ValueChanged(object sender, C1.WPF.PropertyChangedEventArgs<double> e)
        {

          
                //return_value();

          
            if (Mouse.LeftButton == MouseButtonState.Pressed && ctr_name=="")
            {
                //if (middle_num.Value < min_num.Value)
                //{
                //    middle_num.Value = min_num.Value;
                //}
                //if (middle_num.Value > max_num.Value)
                //{
                //    middle_num.Value = max_num.Value;

                //}
            //    //    unregister_handle();
            //        //middle_num.Value = double.Parse( left_value.Content.ToString()) + (double.Parse( right_value.Content.ToString()) - double.Parse( left_value.Content.ToString())) / 2d;
            //        // middle_num.Value = Math.Round((((slider_liangdu.Value + 100d) / 200d) * (refer_max_temp - refer_min_temp) + refer_min_temp) / 10d, 1);

            //        //if (left_lab.Margin.Left == -15 && right_lab.Margin.Left == 395)
            //        //{
            //        //    top_lab.Margin = new Thickness(190, -15, 0, 0);
            //        //}
            //        //else
            //        //{
                //slider_liangdu.Value = (int)(200d * (middle_num.Value - double.Parse(left_value.Content.ToString())) / (double.Parse(right_value.Content.ToString()) - double.Parse(left_value.Content.ToString())) - 100d);

                double can_min = ((max_num.Value - min_num.Value) / 2d + min_num.Value) - (1d - (distance - 10d) / 400d) * (max_num.Value - min_num.Value) / 2d;
                double can_max = ((max_num.Value - min_num.Value) / 2d + min_num.Value) + (1d - (distance - 10d) / 400d) * (max_num.Value - min_num.Value) / 2d;
                can_min = Math.Round(can_min, 1);
                can_max = Math.Round(can_max, 1);
                if (middle_num.Value < can_min)
                {
                    
                    middle_num.Value = can_min;
                    
                }
                else if (middle_num.Value > can_max)
                {
                    middle_num.Value = can_max;
                    
                }


                //slider_liangdu.ValueChanged-=new RoutedPropertyChangedEventHandler<double>(slider_liangdu_ValueChanged);
                slider_liangdu.Value = (middle_num.Value - min_num.Value) / (max_num.Value - min_num.Value) * 200d - 100d;
                change_lab_margin("middle_num");
               // top_lab.Margin = new Thickness((slider_liangdu.Value + 100) / 200 * 410 - 15, -15, 0, 0);
               //// distance = right_lab.Margin.Left - left_lab.Margin.Left;
               // left_lab.Margin = new Thickness(top_lab.Margin.Left - distance / 2, -5, 0, 0);
               // right_lab.Margin = new Thickness(top_lab.Margin.Left + distance / 2, -5, 0, 0);


               // if (left_lab.Margin.Left < -15)
               // {
               //     left_lab.Margin = new Thickness(-15, -5, 0, 0);
               //     top_lab.Margin = new Thickness(left_lab.Margin.Left + distance / 2d, -15, 0, 0);

               //     right_lab.Margin = new Thickness(top_lab.Margin.Left + distance / 2, -5, 0, 0);
               // }


               // if (right_lab.Margin.Left > 390)
               // {
               //     right_lab.Margin = new Thickness(395, -5, 0, 0);
               //     top_lab.Margin = new Thickness(right_lab.Margin.Left - distance / 2d, -15, 0, 0);
               //     left_lab.Margin = new Thickness(right_lab.Margin.Left - distance, -5, 0, 0);
               // }

                //}

                



            }
            


            
           
        }

        private void min_num_ValueChanged(object sender, C1.WPF.PropertyChangedEventArgs<double> e)
        {
            if (min_num.Value > middle_num.Value && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                min_num.Value = middle_num.Value;
            }

            if (min_num.Value < -273)
            {
                min_num.Value = -273;
            }
        
           // left_value.Content = min_num;
            change_lab_margin("min_num");
            //min_num_KeyDown(null, null);

        }

        private void max_num_ValueChanged(object sender, C1.WPF.PropertyChangedEventArgs<double> e)
        {
           
                if (max_num.Value < middle_num.Value&&Mouse.LeftButton==MouseButtonState.Pressed)
                {
                    max_num.Value = middle_num.Value;
                }
                if (max_num.Value > 9999)
                {
                    max_num.Value = 9999;
                }
       
            //right_value.Content = max_num;
            change_lab_margin("max_num");
        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);

            min_distance = 400d / 256d;

            refer_max_temp = workspace.refer_max_temp;
            refer_min_temp = workspace.refer_min_temp;
            min_num.Value = workspace.refer_min_temp / 10d;
            middle_num.Value = (refer_min_temp+(refer_max_temp-refer_min_temp)/2d)/10d;//workspace.refer_middle_temp / 10d;
            max_num.Value = workspace.refer_max_temp / 10d;
            left_value.Content = workspace.refer_min_temp / 10d;
            right_value.Content = workspace.refer_max_temp / 10d;
            left_lab.Margin = new Thickness(-15, -5, 0, 0);
            top_lab.Margin = new Thickness(190, -15, 0, 0);
            right_lab.Margin = new Thickness(395, -5, 0, 0);
            cur_palette_img.Height = 400;
            cur_palette_img.Margin = new Thickness(0, 20, 0, 0);
            slider_duibidu.Value = 128;
            slider_liangdu.Value = 0;
            distance = 410;
            return_value();

 
           // workspace.create_img();
           
        }

        private void gamut_manual_Checked(object sender, RoutedEventArgs e)
        {

            shoudong_checked = true;

            reset.IsEnabled = true;
            slider_liangdu.IsEnabled = true;
            slider_duibidu.IsEnabled = true;
            min_num.IsEnabled = true;
            middle_num.IsEnabled = true;
            max_num.IsEnabled = true;

        }

        private void gamut_auto_Checked(object sender, RoutedEventArgs e)
        {
            shoudong_checked = false;
            auto_default();
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);

            min_distance = 400d / 256d;

            refer_max_temp = workspace.refer_max_temp;
            refer_min_temp = workspace.refer_min_temp;
            min_num.Value = workspace.refer_min_temp / 10d;
            middle_num.ValueChanged-=new EventHandler<C1.WPF.PropertyChangedEventArgs<double>>(middle_num_ValueChanged);
            middle_num.Value = (refer_min_temp + (refer_max_temp - refer_min_temp) / 2d) / 10d;//workspace.refer_middle_temp / 10d;
            max_num.Value = workspace.refer_max_temp / 10d;
            left_value.Content = workspace.refer_min_temp / 10d;
            right_value.Content = workspace.refer_max_temp / 10d;
            left_lab.Margin = new Thickness(-15, -5, 0, 0);
            top_lab.Margin = new Thickness(190, -15, 0, 0);
            right_lab.Margin = new Thickness(395, -5, 0, 0);
            cur_palette_img.Height = 400;
            cur_palette_img.Margin = new Thickness(0, 20, 0, 0);
            slider_duibidu.Value = 128;
            slider_liangdu.Value = 0;
            distance = 410;
            workspace.contrast_step = (int)slider_duibidu.Value;
            workspace.refer_middle_temp = middle_num.Value * 10;
            return_value();
         
        }

        private void main_canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {








            if (shoudong_checked )
            {
                
                if (e.LeftButton == MouseButtonState.Pressed)
                {





                    if (ctr_name == "left_lab")
                    {

                        distance = old_distance + (oldpoint.X-e.GetPosition(null).X);

                        if (distance>410)
                        {
                            distance = 410;
                        }
                        else if (distance<10)
                        {
                            distance = 10;
                        }

                        ttt.Content = e.GetPosition(null).X.ToString() + "  " + e.GetPosition(null).Y + "   " + left_lab.Margin.Left + "  " + distance;

                        //left_lab.Margin = new Thickness(leftmargin.Left + (e.GetPosition(null).X - oldpoint.X) , -5, 0, 0);

                        left_lab.Margin = new Thickness(right_lab.Margin.Left-distance, -5, 0, 0);

                        if (left_lab.Margin.Left > 384)
                        {
                            left_lab.Margin = new Thickness(384, -5, 0, 0);

                        }

                        if (left_lab.Margin.Left < -15)
                        {
                            left_lab.Margin = new Thickness(-15, -5, 0, 0);
                        }

                        if (left_lab.Margin.Left + 10 > right_lab.Margin.Left)
                        {
                            left_lab.Margin = new Thickness(right_lab.Margin.Left - 10, -5, 0, 0);
                        }

                       change_lab_margin("left_lab");
                        //slider_duibidu.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(slider_duibidu_ValueChanged);
                       slider_duibidu.Value = (distance-10d) / 400d * 127d + 1d;

                       
                    }





                    if (ctr_name == "right_lab")
                    {

                    //    right_lab.Margin = new Thickness(oldpoint.X + (e.GetPosition(slider_canvas).X - oldpoint.X) - 10, -5, 0, 0);
                    //    if (right_lab.Margin.Left > 390)
                    //    {
                    //        right_lab.Margin = new Thickness(395, -5, 0, 0);
                    //    }
                    //    else if (right_lab.Margin.Left < left_lab.Margin.Left + 10)
                    //    {
                    //        right_lab.Margin = new Thickness(left_lab.Margin.Left + 10, -5, 0, 0);
                    //    }



                        distance = old_distance + (e.GetPosition(null).X - oldpoint.X);

                        if (distance > 410)
                        {
                            distance = 410;
                        }
                        else if (distance < 10)
                        {
                            distance = 10;
                        }

                        right_lab.Margin = new Thickness(left_lab.Margin.Left+ distance, -5, 0, 0);

                        if (right_lab.Margin.Left > 395)
                        {
                            right_lab.Margin = new Thickness(395, -5, 0, 0);
                        }

                        if (right_lab.Margin.Left < left_lab.Margin.Left + 10)
                        {
                            right_lab.Margin = new Thickness(left_lab.Margin.Left + 10, -5, 0, 0);
                        }

                        change_lab_margin("right_lab");
                       // slider_duibidu.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(slider_duibidu_ValueChanged);
                        slider_duibidu.Value = (distance-10d) / 400d * 127d + 1d;
                     
                       
                    }



                



                    if (ctr_name == "top_lab")
                    {
                       // distance = old_distance + (e.GetPosition(null).X - oldpoint.X);
                        //left_lab.Margin=new Thickness()
                        top_lab.Margin = new Thickness(old_top_margin.Left + (e.GetPosition(null).X - oldpoint.X), -15, 0, 0);

                      


                        //if (top_lab.Margin.Left > 390)
                        //{
                        //    top_lab.Margin = new Thickness(395, -15, 0, 0);
                        //}
                        //else if (top_lab.Margin.Left < top_lab.Margin.Left + 10)
                        //{
                        //    top_lab.Margin = new Thickness(top_lab.Margin.Left + 10, -15, 0, 0);
                        //}
                        //if (left_lab.Margin.Left < -15)
                        //{
                        //    left_lab.Margin = new Thickness(-15, -5, 0, 0);
                        //    top_lab.Margin = new Thickness(distance / 2d + left_lab.Margin.Left, -15, 0, 0);

                        //}
                        //if (right_lab.Margin.Left > 395)
                        //{
                        //    right_lab.Margin = new Thickness(395, -5, 0, 0);
                        //    top_lab.Margin = new Thickness(right_lab.Margin.Left - distance / 2d, -15, 0, 0);
                        //}


                      

                      change_lab_margin("top_lab");
                      
 
                    }


                }


                
            }


            // Thread.Sleep(10);
        }

        private void main_canvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ctr_name = "";
           // middle_num.ValueChanged += new EventHandler<C1.WPF.PropertyChangedEventArgs<double>>(middle_num_ValueChanged);
            
        }

        private void slider_liangdu_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            slider_liangdu.ValueChanged+=new RoutedPropertyChangedEventHandler<double>(slider_liangdu_ValueChanged);
          
        }

        private void slider_duibidu_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            slider_duibidu.ValueChanged+=new RoutedPropertyChangedEventHandler<double>(slider_duibidu_ValueChanged);
          
        }

   


        private void max_num_KeyDown(object sender, KeyEventArgs e)
        {
            Regex re = new Regex("[^0-9.-]+");

            if (e.Key == Key.Decimal || e.Key == Key.OemPeriod || e.Key == Key.Subtract || e.Key == Key.OemMinus)
            {
            }
            else
            {
                e.Handled = re.IsMatch(e.Key.ToString().Substring(e.Key.ToString().Length - 1));

            }


            if (e.Key == Key.Enter)
            {
                if (max_num.Value > 9999)
                {
                    max_num.Value = 9999;
                }

                if (max_num.Value < middle_num.Value)
                {
                    max_num.Value = middle_num.Value;
                }
                right_value.Content = max_num;
                change_lab_margin("max_num");
            }
        }

        private void middle_num_KeyDown(object sender, KeyEventArgs e)
        {
           
            Regex re = new Regex("[^0-9.-]+");

            if (e.Key == Key.Decimal || e.Key == Key.OemPeriod || e.Key == Key.Subtract || e.Key == Key.OemMinus)
            {
            }
            else
            {
                e.Handled = re.IsMatch(e.Key.ToString().Substring(e.Key.ToString().Length - 1));

            }

            if (e.Key == Key.Enter)
            {
                if (middle_num.Value < min_num.Value)
                {
                    middle_num.Value = middle_num.Value;
                }
                if (middle_num.Value > max_num.Value)
                {
                    middle_num.Value = max_num.Value;

                }
            }
           // return_value();
          
        }

        private void min_num_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e != null)
            //{
                Regex re = new Regex("[^0-9.-]+");

                if (e.Key == Key.Decimal || e.Key == Key.OemPeriod || e.Key == Key.Subtract || e.Key == Key.OemMinus)
                {
                }
                else
                {
                    e.Handled = re.IsMatch(e.Key.ToString().Substring(e.Key.ToString().Length - 1));

                }

                if (min_num.Value > middle_num.Value)
                {
                    min_num.Value = middle_num.Value;
                }

                if (min_num.Value < -273)
                {
                    min_num.Value = -273;
                }


                if (e.Key == Key.Enter)
                {
                    //    if (min_num.Value > middle_num.Value)
                    //    {
                    //        min_num.Value = middle_num.Value;
                    //    }

                    //    if (min_num.Value < -273)
                    //    {
                    //        min_num.Value = -273;
                    //    }

                    left_value.Content = min_num;
                    change_lab_margin("min_num");
                }
            //}
        }

        private void middle_num_GotFocus(object sender, RoutedEventArgs e)
        {

            middle_num.ValueChanged+=new EventHandler<C1.WPF.PropertyChangedEventArgs<double>>(middle_num_ValueChanged);
            slider_liangdu.ValueChanged-=new RoutedPropertyChangedEventHandler<double>(slider_liangdu_ValueChanged);
        }

 



     











    }
}
