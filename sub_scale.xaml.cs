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
using System.Windows.Media.Animation;
using System.IO;

namespace IrAnalyse
{
    /// <summary>
    /// sub_scale.xaml 的交互逻辑
    /// </summary>
    public partial class sub_scale : UserControl
    {
        public sub_scale()
        {
            InitializeComponent();
        }
        public ArrayList color_type = new ArrayList();//颜色表
        public float zoom_coe = 1;//缩放系数
        public ArrayList drawlist = new ArrayList();//选中的图形集合
        public int max_temp;//获取集合内最高温
        public int min_temp;//获取集合内最低温
        public int temp_max_count;//温度最大集合
        public float temp_percent;//温度百分比
        public int report_mouseDown_count = 0;//用作图形数量计算
        public sub_repot_ready report;
        public ArrayList shapes_select = new ArrayList();
        public Point oldpoint;
        public double oldmargin;
        public double old_centerpoint;
        public double center_oldpoint;
        
        sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);

        public static readonly RoutedEvent ImageMouseUpEvent = EventManager.RegisterRoutedEvent("ImageMouseUp", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(sub_scale));
        public event RoutedPropertyChangedEventHandler<object> ImageMouseUp
        {
            add { AddHandler(ImageMouseUpEvent, value); }
            remove { RemoveHandler(ImageMouseUpEvent, value); }
        }
        public static readonly RoutedEvent ReportMouseDownEvent = EventManager.RegisterRoutedEvent("ReportMouseDown", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(sub_scale));
        public event RoutedPropertyChangedEventHandler<object> ReportMouseDown
        {
            add { AddHandler(ReportMouseDownEvent, value); }
            remove { RemoveHandler(ReportMouseDownEvent, value); }
        }

        public static readonly RoutedEvent ReportMouseUpEvent = EventManager.RegisterRoutedEvent("ReportMouseUp", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(sub_scale));
        public event RoutedPropertyChangedEventHandler<object> ReportMouseUp
        {
            add { AddHandler(ReportMouseUpEvent, value); }
            remove { RemoveHandler(ReportMouseUpEvent, value); }
        }

   

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            init_color_type();
            create_tree();
            create_coordinate();
        }
        private void init_color_type()//初始化颜色表
        {
            color_type.Clear();
            color_type.Add("0066CC");
            color_type.Add("FFCC33");
            color_type.Add("9933FF");
            color_type.Add("FF3333");
            color_type.Add("006600");
            color_type.Add("003366");
            color_type.Add("CC6600");
            color_type.Add("0099FF");
            color_type.Add("330066");
            color_type.Add("00FFCC");
            color_type.Add("CC00CC");
            color_type.Add("FFFF66");
            color_type.Add("660000");
            color_type.Add("00FF00");
            color_type.Add("666699");
            color_type.Add("990000");
            color_type.Add("9999FF");
            color_type.Add("00CC66");
            color_type.Add("CC3300");
            color_type.Add("669999");
 
        }
        private void create_tree()//创建图形集合树
        {

            for (int i = 0; i < PublicClass.shapes_count.Count; i++)
            {
                PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                if (newshapes.workspace_name == PublicClass.cur_ctrl_name)
                {
                    
                    if (newshapes.shapes_type == "rectangle" || newshapes.shapes_type=="work")
                    {
                        CheckBox newcheckbox = new CheckBox();
                        newcheckbox.Content = newshapes.shapes_name;
                        newcheckbox.Name = newshapes.shapes_name;
                        //newcheckbox.Checked += new RoutedEventHandler(newcheckbox_Checked);
                        newcheckbox.Click += new RoutedEventHandler(newcheckbox_Click);
                        rectanglepanel.Children.Add(newcheckbox);
                        rectanglepanel.RegisterName(newshapes.shapes_name, newcheckbox);
                        drawlist.Add(newshapes.shapes_name);
                    }
                    else if (newshapes.shapes_type == "ellipse")
                    {
                        CheckBox newcheckbox = new CheckBox();
                        newcheckbox.Content = newshapes.shapes_name;
                        newcheckbox.Name = newshapes.shapes_name;
                        //newcheckbox.Checked += new RoutedEventHandler(newcheckbox_Checked);
                        newcheckbox.Click += new RoutedEventHandler(newcheckbox_Click);
                        ellipsepanel.Children.Add(newcheckbox);
                        ellipsepanel.RegisterName(newshapes.shapes_name, newcheckbox);
                        drawlist.Add(newshapes.shapes_name);
                    }
                    else if (newshapes.shapes_type == "polygon")
                    {
                        CheckBox newcheckbox = new CheckBox();
                        newcheckbox.Content = newshapes.shapes_name;
                        newcheckbox.Name = newshapes.shapes_name;
                        //newcheckbox.Checked += new RoutedEventHandler(newcheckbox_Checked);
                        newcheckbox.Click += new RoutedEventHandler(newcheckbox_Click);
                        polygonpanel.Children.Add(newcheckbox);
                        polygonpanel.RegisterName(newshapes.shapes_name, newcheckbox);
                        drawlist.Add(newshapes.shapes_name);
                    }
                }
            }
        }

        void newcheckbox_Click(object sender, RoutedEventArgs e)
        {
           
            //centercanvas.Children.Clear();
            center_img.Children.Clear();
            rightpanle.Children.Clear();
            check_shapes();
            //get_max_min_temp();
            max_percent_temp();
            create_coordinate();
            create_rect();
        }



        private void max_percent_temp()//比例图最高百分比
        {
            max_temp = -100000;
            min_temp = 100000;
            temp_percent = 0;
            for (int i = 0; i < PublicClass.shapes_count.Count; i++)
            {
                PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                if (newshapes.workspace_name == PublicClass.cur_ctrl_name)
                {
                    for (int j = 0; j < shapes_select.Count; j++)
                    {
                        if (newshapes.shapes_name == shapes_select[j].ToString())
                        {
                            if (max_temp < newshapes.max_temp)
                            {
                                max_temp = newshapes.max_temp;
                            }
                            if (min_temp > newshapes.min_temp)
                            {
                                min_temp = newshapes.min_temp;
                            }
                            if (temp_percent < newshapes.percent)
                            {
                                temp_percent = (float)newshapes.percent;
                            }
                        }
                    }
                }
            }
        }

        private void create_coordinate()//创建比例图坐标
        {



            for (int i = 0; i < centercanvas.Children.Count; i++)
            {
                System.Windows.Shapes.Path deletepath = centercanvas.Children[i] as System.Windows.Shapes.Path;
                if (deletepath != null)
                {
                    centercanvas.Children.Remove(deletepath);
                }
                Polygon deletepol = centercanvas.Children[i] as Polygon;
                if (deletepol != null)
                {
                    centercanvas.Children.Remove(deletepol);
                }
                TextBlock deletetxt = centercanvas.Children[i] as TextBlock;
                if (deletetxt != null)
                {
                    centercanvas.Children.Remove(deletetxt);
                }
            }



            RectangleGeometry newrect = new RectangleGeometry();
            newrect.Rect = new Rect(50, 10, 640, 300);
            System.Windows.Shapes.Path mypath = new System.Windows.Shapes.Path();
            mypath.Stroke = Brushes.Black;
            mypath.StrokeThickness = 1;
            //mypath.Fill = Brushes.White;
            mypath.Opacity = 0.6;
            mypath.Data = newrect;
            mypath.SnapsToDevicePixels = true;
            
            centercanvas.Children.Add(mypath);
            Polygon ypol = new Polygon();
            ypol.Points.Add(new Point(49,0));
            ypol.Points.Add(new Point(49, 10));
            ypol.Points.Add(new Point(54, 10));
            ypol.Stroke = Brushes.Black;
            ypol.StrokeThickness = 1;
            ypol.Fill = Brushes.Black;
            ypol.SnapsToDevicePixels = true;
            centercanvas.Children.Add(ypol);

            Polygon xpol = new Polygon();
            xpol.Points.Add(new Point(700-9, 305));
            xpol.Points.Add(new Point(700-9, 310));
            xpol.Points.Add(new Point(700, 310));
            xpol.Stroke = Brushes.Black;
            xpol.StrokeThickness = 1;
            xpol.Fill = Brushes.Black;
            xpol.SnapsToDevicePixels = true;
            centercanvas.Children.Add(xpol);
            

            TextBlock textblock = new TextBlock();
            textblock.Text = "X:温度  Y：百分比";
            textblock.Margin = new Thickness(50, 340, 0, 0);
            centercanvas.Children.Add(textblock);

            int step = 0;
            float step_low = 50;
            for (int i = 10; i < 313; i=i+6)
            {
                LineGeometry newline = new LineGeometry();
                LineGeometry newxline = new LineGeometry();
                if (step % 5 == 0)
                {
                    newline.StartPoint = new Point(40, i);
                    newline.EndPoint = new Point(50, i);

                    newxline.StartPoint = new Point(50, i);
                    newxline.EndPoint = new Point(centercanvas.Width-10, i);
                    System.Windows.Shapes.Path newxpath = new System.Windows.Shapes.Path();
                    newxpath.Stroke = Brushes.Black;
                    newxpath.Opacity = 0.3;
                    newxpath.StrokeThickness = 1;
                    newxpath.Data = newxline;
                    newxpath.SnapsToDevicePixels = true;
                    centercanvas.Children.Add(newxpath);
                    
                    if (step_low >= 0)
                    {
                        TextBlock newtext = new TextBlock();
                        newtext.Text = ((float)((int)((temp_percent / 50f * step_low) * 10f)) / 10f).ToString();
                        if (float.Parse(newtext.Text) == 100)
                        {
                            newtext.Width = 32;
                        }
                        else if (float.Parse(newtext.Text) < 10)
                        {
                            newtext.Width = 20;
                        }
                        else
                        {
                            newtext.Width = 25;
                        }
                        step_low = step_low - 5;
                        
                        newtext.Margin = new Thickness(38 - newtext.Width, i-8, 0, 0);
                        centercanvas.Children.Add(newtext);
                    }
                    

                }
                else
                {
                    newline.StartPoint = new Point(45, i);
                    newline.EndPoint = new Point(50, i);

                    newxline.StartPoint = new Point(50, i);
                    newxline.EndPoint = new Point(centercanvas.Width-10, i);
                    System.Windows.Shapes.Path newxpath = new System.Windows.Shapes.Path();
                    newxpath.Stroke = Brushes.Black;
                    newxpath.Opacity = 0.2;
                    newxpath.StrokeThickness = 1;
                    newxpath.Data = newxline;
                    newxpath.SnapsToDevicePixels = true;
                    centercanvas.Children.Add(newxpath);
                }
                step++;
                System.Windows.Shapes.Path newpath = new System.Windows.Shapes.Path();
                newpath.Stroke = Brushes.Black;
                newpath.StrokeThickness = 1;
                newpath.Data = newline;
                newpath.SnapsToDevicePixels = true;
                centercanvas.Children.Add(newpath);
            
            }

         
            step = 0;
            step_low = (float)((max_temp - min_temp) / ((640 * zoom_coe) / 12.8f));
            for (float i = 50; i < 640*zoom_coe+51; i = i + 12.8f)
            {
                LineGeometry newline = new LineGeometry();
                LineGeometry newyline = new LineGeometry();
                if (step % 5 == 0)
                {
                    newline.StartPoint = new Point(i, 310);
                    newline.EndPoint = new Point(i, 320);

                    newyline.StartPoint = new Point(i, 10);
                    newyline.EndPoint = new Point(i, 310);
                    System.Windows.Shapes.Path newypath = new System.Windows.Shapes.Path();
                    newypath.Stroke = Brushes.Black;
                    newypath.Opacity = 0.1;
                    newypath.StrokeThickness = 1;
                    newypath.Data = newyline;
                    newypath.SnapsToDevicePixels = true;
                    center_img.Children.Add(newypath);

                    TextBlock newtextblock = new TextBlock();
                    newtextblock.Text = (((float)((int)((min_temp + step_low * step)*10)))/100).ToString();
                    if (i == 50)
                    {
                        newtextblock.Margin = new Thickness(i, 320,0, 0);
                    }
                    else
                    {
                        newtextblock.Margin = new Thickness(i - 10, 320, 0, 0);
                    }

                    if (newtextblock.Margin.Left > 640 * zoom_coe)
                    {
                        int t = newtextblock.Text.Length;
                        newtextblock.Margin = new Thickness(newtextblock.Margin.Left-t*5, 320, 0, 0);
                        
                    }

                    center_img.Children.Add(newtextblock);

                }
                else
                {
                    newline.StartPoint = new Point(i, 310);
                    newline.EndPoint = new Point(i, 315);

                    //newyline.StartPoint = new Point(i, 10);
                    //newyline.EndPoint = new Point(i, 310);
                    //System.Windows.Shapes.Path newypath = new System.Windows.Shapes.Path();
                    //newypath.Stroke = Brushes.Black;
                    //newypath.Opacity = 0.2;
                    //newypath.StrokeThickness = 1;
                    //newypath.Data = newyline;
                    //newypath.SnapsToDevicePixels = true;
                    //centercanvas.Children.Add(newypath);
                }
                step++;
                System.Windows.Shapes.Path newpath = new System.Windows.Shapes.Path();
                newpath.Stroke = Brushes.Black;
                newpath.StrokeThickness = 1;
                newpath.Data = newline;
                newpath.SnapsToDevicePixels = true;
                center_img.Children.Add(newpath);
            }


 
        }
        public void shapeslist(string shapes_name, float unit_low, float unit_high, int color_value, int rect_index)//加载图片本来信息
        {

            int draw_img_index = 0;//待计算的记录下标
            int draw_img_index_start = 0;//待计算的有效数据开始位置
            for (int i = 0; i < workspace.draw_img.Count; i++)
            {
                string[] st = workspace.draw_img[i].ToString().Split(',');
                if (st[1] == shapes_name)
                {
                    draw_img_index = i;
                }
            }
            string[] s = workspace.draw_img[draw_img_index].ToString().Split(',');
            if (s[0] == "point")
            {
                draw_img_index_start = 11;
            }
            else if (s[0] == "line")
            {
                draw_img_index_start = 13;
            }
            else if (s[0] == "polyline")
            {
                for (int j = 10; j < s.Count(); j++)
                {
                    if (s[j] == "eof")
                    {
                        draw_img_index_start = j + 1;
                        break;
                    }
                }
            }
            else if (s[0] == "ellipse")
            {
                draw_img_index_start = 13;
            }
            else if (s[0] == "rectangle")
            {
                draw_img_index_start = 13;
            }
            else if (s[0] == "polygon")
            {
                for (int j = 10; j < s.Count(); j++)
                {
                    if (s[j] == "eof")
                    {
                        draw_img_index_start = j + 1;
                        break;
                    }
                }
            }
            if (draw_img_index >= 0)
            {
                string[] s1 = workspace.draw_img[draw_img_index].ToString().Split(',');
                int temp_temp_max_count = 0;
                for (int i = draw_img_index_start; i < s1.Count(); i++)
                {
                    if (int.Parse(workspace.ir_temp[int.Parse(s1[i])].ToString()) > max_temp)
                    {
                        max_temp = int.Parse(workspace.ir_temp[int.Parse(s1[i])].ToString());
                    }
                    if (int.Parse(workspace.ir_temp[int.Parse(s1[i])].ToString()) < min_temp)
                    {
                        min_temp = int.Parse(workspace.ir_temp[int.Parse(s1[i])].ToString());
                    }

                    int cur_temp = int.Parse(workspace.ir_temp[int.Parse(s1[i])].ToString());
                    if (unit_low != unit_high)
                    {
                        if ((float)cur_temp >= unit_low && (float)cur_temp <= unit_high)
                        {
                            temp_temp_max_count++;
                        }
                    }

                }

                if ((float)temp_temp_max_count / (float)s1.Count() * 100f > temp_percent)
                {
                    temp_percent = (float)temp_temp_max_count / (float)s1.Count() * 100f;
                }
                if (color_value > -1 && temp_temp_max_count > 0)
                {
                    double startx = (((double)rect_index * 12.8d * (double)zoom_coe)) + 12.8d / (double)shapes_select.Count * (double)color_value + 50d;
                    double starty = 300d - (((double)temp_temp_max_count / (double)s1.Count() * 100d) * 300d / (double)temp_percent) + 10d;
                    double endx = 12.8d * zoom_coe / shapes_select.Count;
                    if (shapes_select.Count == 1)
                    {
                        endx = endx - 3;
                    }
                    double endy = 310 - starty; ;
                    RectangleGeometry newrect = new RectangleGeometry();
                    newrect.Rect = new Rect(startx, starty, endx, endy);

                    int colorR = Convert.ToInt32(color_type[color_value].ToString().Substring(0, 2), 16);
                    int colorG = Convert.ToInt32(color_type[color_value].ToString().Substring(2, 2), 16);
                    int colorB = Convert.ToInt32(color_type[color_value].ToString().Substring(4, 2), 16);

                    System.Windows.Media.Effects.DropShadowEffect da = new System.Windows.Media.Effects.DropShadowEffect();
                    da.BlurRadius = 2;
                    da.Opacity = 0.65;
                    da.ShadowDepth = 2;
                    da.Color = Color.FromRgb(0, 0, 0);

                    System.Windows.Shapes.Path newpath = new System.Windows.Shapes.Path();
                    newpath.Stroke = new SolidColorBrush(Color.FromRgb((byte)colorR, (byte)colorG, (byte)colorB));
                    newpath.Opacity = 0.8;
                    newpath.StrokeThickness = 1;
                    newpath.Data = newrect;
                    newpath.Effect = da;
                    newpath.Fill = new SolidColorBrush(Color.FromRgb((byte)colorR, (byte)colorG, (byte)colorB));
                    newpath.SnapsToDevicePixels = true;

                    ThicknessAnimation doublestart = new ThicknessAnimation();
                    doublestart.From = new Thickness(0, 310 - newpath.ActualHeight, 0, 0);
                    doublestart.To = new Thickness(0, 0, 0, 0);
                    doublestart.Duration = TimeSpan.FromSeconds(0.02 * rect_index);

                    DoubleAnimation doubleheight = new DoubleAnimation();
                    doubleheight.From = 0;
                    doubleheight.To = 310;
                    doubleheight.Duration = TimeSpan.FromSeconds(0.02 * rect_index);


                    centercanvas.Children.Add(newpath);
                    newpath.BeginAnimation(System.Windows.Shapes.Path.MarginProperty, doublestart);
                    newpath.BeginAnimation(System.Windows.Shapes.Path.HeightProperty, doubleheight);



                    //RectangleGeometry newright = new RectangleGeometry();
                    //newright.Rect = new Rect(0, 0, 10, 5);
                    //System.Windows.Shapes.Path newrightpath = new System.Windows.Shapes.Path();
                    //newrightpath.Stroke = new SolidColorBrush(Color.FromRgb((byte)colorR, (byte)colorG, (byte)colorB));
                    //newrightpath.Opacity = 0.8;
                    //newrightpath.StrokeThickness = 1;
                    //newrightpath.Data = newright;
                    //rightpanle.Children.Add(newrightpath);

                }

            }


        }
        private void get_max_min_temp()//获取几何图形集合的最高温最低温
        {
            max_temp = -100;
            min_temp = 3000;
            temp_max_count = 0;
            temp_percent = 0;
            if (shapes_select.Count > 0)
            {
                for (int i = 0; i < shapes_select.Count; i++)//找出最低温
                {
                    shapeslist(shapes_select[i].ToString(), 0, 0, -1, 0);
                }

                for (int i = 0; i < 49; i++)//找出最大合计
                {
                    for (int j = 0; j < shapes_select.Count; j++)
                    {
                        shapeslist(shapes_select[j].ToString(), (float)min_temp + (float)(max_temp - min_temp) / 50f * (float)i, (float)min_temp + (float)(max_temp - min_temp) / 50f * ((float)i + 1f), -1, 0);
                    }
                }
            }

        }
        private void check_shapes()//判断集合选中
        {
            shapes_select.Clear();
            for (int i = 0; i < drawlist.Count; i++)
            {
               CheckBox newcheck= centercanvas.FindName(drawlist[i].ToString()) as CheckBox;
               if (newcheck != null)
               {
                   if ((bool)newcheck.IsChecked)
                   {
                       shapes_select.Add(drawlist[i].ToString());
                   }
               }
            }
       
        }

        private void create_rect()//绘制矩形
        {
            int color_value = 0;
            if (shapes_select.Count > 0)
            {
                for (int i = 0; i < shapes_select.Count; i++)
                {
                    color_value = i % 20;
                    PublicClass.shapes_property newshapes;
                    for (int t = 0; t < PublicClass.shapes_count.Count; t++)
                    {
                        newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[t];
                        if (newshapes.workspace_name == PublicClass.cur_ctrl_name && newshapes.shapes_name == shapes_select[i].ToString())
                        {

                            for (int j = 0; j < 50; j++)
                            {
                                //shapeslist(shapes_select[i].ToString(), (float)min_temp + (float)(max_temp - min_temp) / 50f * (float)j, (float)min_temp + (float)(max_temp - min_temp) / 50f * (float)(j + 1), color_value,j);
                                double unit_percent = 0;
                                double unit1 = (double)min_temp + (double)(max_temp - min_temp) / 50 * (double)j;
                                double unit2 = (double)min_temp + (double)(max_temp - min_temp) / 50 * (double)(j + 1);
                                for (int x = 0; x < newshapes.unique_temp.Count; x++)
                                {
                                    if (unit1 <= newshapes.unique_temp[x] && unit2 >= newshapes.unique_temp[x])
                                    {
                                        unit_percent = newshapes.unique_percent[x];
                                        break;
                                    }
                                    
                                }
                                draw_rect(color_value, j, unit_percent);
                            }

                            int colorR = Convert.ToInt32(color_type[color_value].ToString().Substring(0, 2), 16);
                            int colorG = Convert.ToInt32(color_type[color_value].ToString().Substring(2, 2), 16);
                            int colorB = Convert.ToInt32(color_type[color_value].ToString().Substring(4, 2), 16);
                            RectangleGeometry newrightrect = new RectangleGeometry();
                            newrightrect.Rect = new Rect(0, 3, 20, 10);
                            System.Windows.Shapes.Path mypath = new System.Windows.Shapes.Path();
                            mypath.Stroke = Brushes.Black;
                            mypath.StrokeThickness = 1;
                            mypath.Opacity = 0.6;
                            mypath.Data = newrightrect;
                            mypath.SnapsToDevicePixels = true;
                            mypath.Fill = new SolidColorBrush(Color.FromRgb((byte)colorR, (byte)colorG, (byte)colorB));
                            TextBlock newtextblock = new TextBlock();
                            newtextblock.Text = " " + newshapes.shapes_name + " Max " + (float)newshapes.max_temp / 10 + " ℃" + " Min " + (float)newshapes.min_temp / 10 + " ℃";
                            StackPanel newstackpanel = new StackPanel();
                            newstackpanel.Children.Add(mypath);
                            newstackpanel.Children.Add(newtextblock);
                            newstackpanel.Orientation = Orientation.Horizontal;
                            rightpanle.Children.Add(newstackpanel);

                        }
                       
                    }
                  

                }
            }
        }

        private void draw_rect(int color_value, int rect_index, double unit_percent)//绘制右侧比例图说明
        {
            if (unit_percent > 0)
            {
                double startx = (((double)rect_index * 12.8d * (double)zoom_coe)) + 12.8d / (double)shapes_select.Count*zoom_coe * (double)color_value + 50d;
                //double starty = 300d - (((double)temp_temp_max_count / (double)s1.Count() * 100d) * 300d / (double)temp_percent) + 10d;
                double starty = 310 - 300 * unit_percent / temp_percent;
                double endx = 12.8d * zoom_coe / shapes_select.Count;
                if (shapes_select.Count == 1)
                {
                    endx = endx - 3;
                }
                double endy = 310 - starty; ;
                RectangleGeometry newrect = new RectangleGeometry();
                newrect.Rect = new Rect(startx, starty, endx, endy);

                int colorR = Convert.ToInt32(color_type[color_value].ToString().Substring(0, 2), 16);
                int colorG = Convert.ToInt32(color_type[color_value].ToString().Substring(2, 2), 16);
                int colorB = Convert.ToInt32(color_type[color_value].ToString().Substring(4, 2), 16);

                System.Windows.Media.Effects.DropShadowEffect da = new System.Windows.Media.Effects.DropShadowEffect();
                da.BlurRadius = 2;
                da.Opacity = 0.65;
                da.ShadowDepth = 2;
                da.Color = Color.FromRgb(0, 0, 0);

                System.Windows.Shapes.Path newpath = new System.Windows.Shapes.Path();
                newpath.Stroke = new SolidColorBrush(Color.FromRgb((byte)colorR, (byte)colorG, (byte)colorB));
                newpath.Opacity = 0.8;
                newpath.StrokeThickness = 1;
                newpath.Data = newrect;
                newpath.Effect = da;
                newpath.Fill = new SolidColorBrush(Color.FromRgb((byte)colorR, (byte)colorG, (byte)colorB));
                newpath.SnapsToDevicePixels = true;

                ThicknessAnimation doublestart = new ThicknessAnimation();
                doublestart.From = new Thickness(0, 310 - newpath.ActualHeight, 0, 0);
                doublestart.To = new Thickness(0, 0, 0, 0);
                doublestart.Duration = TimeSpan.FromSeconds(0.02 * rect_index);

                DoubleAnimation doubleheight = new DoubleAnimation();
                doubleheight.From = 0;
                doubleheight.To = 310;
                doubleheight.Duration = TimeSpan.FromSeconds(0.02 * rect_index);


                center_img.Children.Add(newpath);
               // centercanvas.Children.Add(newpath); 
                newpath.BeginAnimation(System.Windows.Shapes.Path.MarginProperty, doublestart);
                newpath.BeginAnimation(System.Windows.Shapes.Path.HeightProperty, doubleheight);


            }

        }

        private void left_arr_MouseUp(object sender, MouseButtonEventArgs e)
        {
            RoutedPropertyChangedEventArgs<object> args = new RoutedPropertyChangedEventArgs<object>(this, e);
            args.RoutedEvent = sub_scale.ImageMouseUpEvent;
            this.RaiseEvent(args);
        }


        void report_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataObject data = new DataObject(typeof(sub_repot_ready), (sub_repot_ready)sender);
            DragDrop.DoDragDrop((sub_repot_ready)sender, data, DragDropEffects.Copy);
        }

        private void pre_report_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
           
            RoutedPropertyChangedEventArgs<object> args = new RoutedPropertyChangedEventArgs<object>(this, e);
            args.RoutedEvent = sub_scale.ReportMouseUpEvent;
            this.RaiseEvent(args);
        }

        private void pre_report_PreviewMouseDown(object sender, MouseButtonEventArgs e)//报告准备截图
        {
            RenderTargetBitmap repot_image;
            repot_image = new RenderTargetBitmap((int)report_panel.ActualWidth, (int)report_panel.ActualHeight, 96, 96, PixelFormats.Default);
            repot_image.Render(report_panel);
            report = new sub_repot_ready();
            report.sub_repot.repot_ready_shapes = PublicClass.shapes_count;
            report.AllowDrop = true;
            report.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(report_PreviewMouseLeftButtonDown);
            report.repot_image.Source = repot_image;
            report.img_tooltip.Source = repot_image;
            report.img_tooltip.Width = repot_image.Width * 0.7;
            report.img_tooltip.Height = repot_image.Height * 0.7;
            report.sub_repot.repot_ready_shapes = new ArrayList();
            try
            {
                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    PublicClass.shapes_property test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                    int in_shapes = 0;
                    in_shapes = (from c in workspace.selected_spot_area where c == test.shapes_name select c).Count();
                    if (in_shapes > 0 && test.workspace_name == PublicClass.cur_ctrl_name)
                    {
                        report.sub_repot.repot_ready_shapes.Add(test);
                    }
                    else if (test.shapes_type == "ellipse" || test.shapes_type == "rectangle" || test.shapes_type == "polygon" || test.shapes_type == "work")
                    {
                        report.sub_repot.repot_ready_shapes.Add(test);
                    }
                }
            }
            catch
            { 
            
            
            }
            string temp_path = System.IO.Path.GetTempPath() + "\\IrAnalyse\\" + Guid.NewGuid() + ".jpg";
            using (FileStream outStream = new FileStream(temp_path, FileMode.Create))
            {
                report.temp_path = temp_path;
                PngBitmapEncoder encoder = new PngBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create(repot_image));
                encoder.Save(outStream);
            }
            PublicClass.repot_ready_index++;
            report.sub_repot.repot_ready_name = "ReportReady" + PublicClass.repot_ready_index;
            report.repot_txt.Text = report.sub_repot.repot_ready_name;

            report.repot_count.Text = "图形数量:" + report.sub_repot.repot_ready_shapes.Count;
           
           
            RoutedPropertyChangedEventArgs<object> args = new RoutedPropertyChangedEventArgs<object>(this, e);
            args.RoutedEvent = sub_scale.ReportMouseDownEvent;
            this.RaiseEvent(args);
        }

        private void zoom_Click(object sender, RoutedEventArgs e)
        {
            Button newbtn =sender as Button;
            if(newbtn!=null)
            {
                if(newbtn.Name=="zoom_down")
                {
                    zoom_coe-=0.5f;
                }
                else
                {
                    zoom_coe+=0.5f;
                }
                if (zoom_coe < 1)
                {
                    zoom_coe = 1;
                }
                else if (zoom_coe > 5)
                {
                    zoom_coe = 5;
                }
              
                
                newcheckbox_Click(null, null);
                change_scroll();
                center_img.Margin = new Thickness(((zoom_scroll.Margin.Left - 50.0) / (640.0 - zoom_scroll.Width) * (640 - zoom_coe * 640)) - 50, 0, 0, 0);
            }
        }

        private void change_scroll()
        {
            if (zoom_coe == 1)
            {
                zoom_scroll.Opacity = 0;
            }
            else
            {
                zoom_scroll.Opacity = 0.5;
                zoom_scroll.Width = (1 - zoom_coe / 6) * 690;
            }
            if (zoom_scroll.Margin.Left + zoom_scroll.Width > 690)
            {
                zoom_scroll.Margin = new Thickness(690 - zoom_scroll.Width, 312, 0, 0);
            }
            center_img.Width = 640 * zoom_coe + 50;
        }





        private void zoom_scroll_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            oldpoint.X = e.GetPosition(centercanvas).X;
            oldmargin = zoom_scroll.Margin.Left;
                      
        }

        private void zoom_scroll_PreviewMouseMove(object sender, MouseEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                zoom_scroll.Margin = new Thickness(oldmargin+(e.GetPosition(centercanvas).X - oldpoint.X), 312, 0, 0);
      
                if (zoom_scroll.Margin.Left < 50)
                {
                    zoom_scroll.Margin = new Thickness(50, 312, 0, 0);
                }
                else if (zoom_scroll.Margin.Left > 690 - zoom_scroll.Width)
                {
                    zoom_scroll.Margin = new Thickness(690 - zoom_scroll.Width, 312, 0, 0);
                }
               
               center_img.Margin = new Thickness(((zoom_scroll.Margin.Left - 50.0) / (640.0 - zoom_scroll.Width) * (640-zoom_coe * 640)) - 50, 0, 0, 0);
               
     

            }
          
        }

        private void center_img_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            center_oldpoint = e.GetPosition(center_img).X;
        }

        private void center_img_PreviewMouseMove(object sender, MouseEventArgs e)
        {
          
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                System.Windows.Shapes.Path deletepath = center_img.FindName("centerrect") as System.Windows.Shapes.Path;
                if (deletepath != null)
                {
                    center_img.Children.Remove(deletepath);
                    center_img.UnregisterName("centerrect");
                }
                RectangleGeometry newrect = new RectangleGeometry();
                double rectw = Math.Abs(e.GetPosition(center_img).X - center_oldpoint);
                double rectx = center_oldpoint;
                if (e.GetPosition(center_img).X - center_oldpoint < 0)
                {
                    rectx = e.GetPosition(center_img).X;
                }
                newrect.Rect = new Rect(rectx, 10, rectw, 300);

                System.Windows.Shapes.Path newpath = new System.Windows.Shapes.Path();

                newpath.Opacity = 0.8;
                newpath.StrokeThickness = 1;
                newpath.Data = newrect;
                newpath.Fill = Brushes.Yellow;

                newpath.SnapsToDevicePixels = true;
                newpath.PreviewMouseUp += new MouseButtonEventHandler(center_img_PreviewMouseUp);
                center_img.Children.Add(newpath);
                center_img.RegisterName("centerrect", newpath);

            }
        }

        private void center_img_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Shapes.Path rectpath = center_img.FindName("centerrect") as System.Windows.Shapes.Path;
            if (rectpath != null)
            {
                RectangleGeometry rect = rectpath.Data as RectangleGeometry;
                double temp_zomm_coe = zoom_coe;
                zoom_coe = (float)(640.0 * zoom_coe / rect.Rect.Width);
                if (zoom_coe > 5)
                {
                    zoom_coe = 5;
                }



                /// center_img.Margin = new Thickness((-rect.Rect.X)*zoom_coe+50*zoom_coe-50, 0, 0, 0);
                /// 
                double center_fleft = ((-rect.Rect.X + 50) / temp_zomm_coe) * zoom_coe - 50;
                newcheckbox_Click(null, null);
                change_scroll();
                if (center_fleft > -50)
                {
                    center_fleft = -50;
                }
                if (center_fleft < 640 - center_img.Width)
                {
                    center_fleft = 640 - center_img.Width;
                }




                center_img.Margin = new Thickness(center_fleft, 0, 0, 0);

                zoom_scroll.Margin = new Thickness(((center_img.Margin.Left + 50) / (640 - zoom_coe * 640)) * (640 - zoom_scroll.Width) + 50, 312, 0, 0);
            }
           // zoom_scroll.Margin = new Thickness(center_img.Margin.Left / 640+50, 312, 0, 0);

            
        }


       
    }
}
