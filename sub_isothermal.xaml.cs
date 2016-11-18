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
using System.Collections;
using System.Data;


namespace IrAnalyse
{
    /// <summary>
    /// sub_isothermal.xaml 的交互逻辑
    /// </summary>
    public partial class sub_isothermal : UserControl
    {
        public sub_isothermal()
        {
            InitializeComponent();
        }

        public static readonly RoutedEvent IsoEvent = EventManager.RegisterRoutedEvent("Iso", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(sub_isothermal));
        public event RoutedPropertyChangedEventHandler<object> Iso
        {
            add { AddHandler(IsoEvent, value); }
            remove { RemoveHandler(IsoEvent, value); }
        }
        string process_mode = "all";//鼠标操作色标的位置，all移动,up色标上部大小,down色标下部大小
        string process_lable_name = "";
        Point old_point;
        Label cur_lable;
        double old_height;
        double old_margin_top;
        double old_margin_bottom;
        Thread newthread;
        public ArrayList old_isothermal = new ArrayList();

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            leftimage();
            calculate_temp_ruler();//生成温度标尺
            init_isothermal_list();
            load_data();
            for (int i = 0; i < workspace.isothermal_list.Count; i++)
            {
                PublicClass.isothermal_property work_iso = (PublicClass.isothermal_property)workspace.isothermal_list[i];
                PublicClass.isothermal_property old_iso = new PublicClass.isothermal_property();
                old_iso.color = work_iso.color;
                old_iso.is_checked = work_iso.is_checked;
                old_iso.is_opacity = work_iso.is_opacity;
                old_iso.level = work_iso.level;
                old_iso.max_temp = work_iso.max_temp;
                old_iso.min_temp = work_iso.min_temp;
                old_isothermal.Add(old_iso);

                //System.Windows.Forms.Application.DoEvents();
                //StackPanel temstack = isothermal_list.FindName("stackpanel" + i) as StackPanel;
                //if (temstack != null)
                //{


                    //int step = 0;
                    //foreach (var c in temstack.Children)
                    //{
                    //    CheckBox tembox = c as CheckBox;
                    //    if (tembox != null)
                    //    {
                    //        if (step == 0 &&work_iso.is_checked)
                    //        {
                    //            tembox.IsChecked = true;
                    //            step++;
                    //        }
                    //        else if (step == 1 && work_iso.is_opacity)
                    //        {
                    //            tembox.IsChecked = true;
                    //            break;
                    //        }
                    //    }
                    //}


                    //CheckBox temselect = temstack.FindName("isselected" + i) as CheckBox;
                    //if (work_iso.is_checked)
                    //{
                    //    temselect.IsChecked = true;
                    //}
                    //CheckBox temopacity = temstack.FindName("is_opacity" + i) as CheckBox;
                    //if (work_iso.is_opacity)
                    //{
                    //    temopacity.IsChecked = true;
                    //}
                //}


                    
            }
            iso_combox();
            
        }



        private void iso_combox()
        {
            int cbx_index = 0;
           
            for(int i=0;i<PublicClass.shapes_count.Count;i++)
            {
                PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                if (newshapes.shapes_name != "max_temp" && newshapes.shapes_name != "min_temp" &&PublicClass.cur_ctrl_name ==newshapes.workspace_name)
                {
                    CheckBox newcheckbox = new CheckBox();
                    var select_cbx = from c in PublicClass.Iso_Shapes_list where c.shapes_name == newshapes.shapes_name && c.workspace_name == PublicClass.cur_ctrl_name select c;
                    if (select_cbx.Count() > 0)
                    {
                        newcheckbox.IsChecked = true;
                    }
                    newcheckbox.Name = newshapes.shapes_name;
                    newcheckbox.Content = newshapes.shapes_name;
                    if (cbx_index == 0)
                    {
                        //newcheckbox.IsChecked = true;
                    }
                    iso_ComboBox.Items.Insert(cbx_index, newcheckbox);
                    

                    cbx_index++;
                    newcheckbox.Checked += new RoutedEventHandler(newcheckbox_Checked);
                    newcheckbox.Unchecked += new RoutedEventHandler(newcheckbox_Unchecked);
            
                }
               
               
            }
            iso_ComboBox.SelectedIndex = 0;
            
        }

        void newcheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            aa(sender, e);
        }

        void newcheckbox_Checked(object sender, RoutedEventArgs e)
        {
            aa(sender, e);
        }


        private void aa(object sender, RoutedEventArgs e)
        {
           // List<int> list2 = new List<int>();
            List<int> list1 = new List<int>();
            //list1.Clear();
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            CheckBox newcheckbox = sender as CheckBox;

            PublicClass.Iso_Shapes_list.Clear();
            for (int i = 0; i < PublicClass.Iso_Shapes_list.Count; i++)
            {
                if (PublicClass.Iso_Shapes_list[i].workspace_name == PublicClass.cur_ctrl_name)
                {
                    PublicClass.Iso_Shapes_list.RemoveAt(i);
                    i--;
                }
            }
            


            foreach (var c in iso_ComboBox.Items)
            {
                CheckBox newch = c as CheckBox;
                
                if (newch != null)
                {

                    if ((bool)newch.IsChecked)
                    {

                        iso_ComboBox.SelectedIndex = 0;
                        for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                        {
                            PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                            if (newch.Name.ToString() == newshapes.shapes_name && newshapes.workspace_name==PublicClass.cur_ctrl_name)
                            {
                                list1.AddRange(newshapes.pixel_coordinate);
                                PublicClass.Iso_Shapes new_iso_shapes = new PublicClass.Iso_Shapes();
                                new_iso_shapes.shapes_name = newshapes.shapes_name;
                                new_iso_shapes.workspace_name = newshapes.workspace_name;
                                new_iso_shapes.max_temp = workspace.grad_max / 10.0;
                                new_iso_shapes.min_temp = workspace.grad_min / 10.0;
                                new_iso_shapes.pixel_coordinate.AddRange(newshapes.pixel_coordinate);
                                PublicClass.Iso_Shapes_list.Add(new_iso_shapes);
                            }
                            


                        }

                       

                    }
                }

            }
            PublicClass.list2.Clear();
            PublicClass.list2.AddRange(list1.Union(list1));
            PublicClass.list2.Sort();

            workspace.filliso();
            workspace.create_img();

        }
     
        
      

        private void load_data()
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);

            double max_temp = workspace.grad_max / 10.0;
            double min_temp = workspace.grad_min / 10.0;
            for (int i = 0; i < 16; i++)
            {
                PublicClass.isothermal_property newiso = (PublicClass.isothermal_property)workspace.isothermal_list[i];
                if (newiso.is_checked)
                {
                    StackPanel newstackpanel = (StackPanel)isothermal_list.Items[i + 1];
                    for (int j = 0; j < newstackpanel.Children.Count; j++)
                    {
                        CheckBox isselected = newstackpanel.Children[j] as CheckBox;
                        if (isselected != null)
                        {
                            if (isselected.Name == "isselected" + (i + 1))
                            {
                                isselected.IsChecked = true;
                            }
                            if (isselected.Name == "is_opacity" + (i + 1) && newiso.is_opacity)
                            {
                                isselected.IsChecked = true;
                            }
                        }
                    }


                    Label colorlable = new Label();
                    colorlable.Width = 30;
                    double temp_margin_top = 40;
                    //double temp_height;
                    double temp_margin_bottom = 375;



                    if (max_temp == min_temp)
                    {
                        if (newiso.max_temp > max_temp)
                        {
                            temp_margin_top = 40;
                            temp_margin_bottom = 41;
                        }
                        if (newiso.max_temp < max_temp)
                        {
                            temp_margin_top = 374;
                            temp_margin_bottom = 375;
                        }
                        
                        //if (newiso.min_temp < min_temp)
                        //{
                        //    temp_margin_bottom = 375;
                        //}
                        //else
                        //{
                        //    temp_margin_top = 40;
                        //    temp_margin_bottom = 375;
                        //}
                    }
                    else
                    {
                        temp_margin_top = (max_temp - newiso.max_temp) / (max_temp - min_temp) * 335 + 40;
                        temp_margin_bottom = (max_temp - newiso.min_temp) / (max_temp - min_temp) * 335 + 40;

                        if ((max_temp - newiso.max_temp) / (max_temp - min_temp) * 335 + 40 < 0)
                        {
                            temp_margin_top = 40;
                        }
                        if ((max_temp - newiso.min_temp) / (max_temp - min_temp) * 335 + 40 <= 0)
                        {
                            temp_margin_bottom = 41;
                        }

                        if ((max_temp - newiso.max_temp) / (max_temp - min_temp) * 335 + 40 > 375)
                        {
                            temp_margin_top = 374;
                        }
                        if ((max_temp - newiso.min_temp) / (max_temp - min_temp) * 335 + 40 > 375)
                        {
                            temp_margin_bottom = 375;
                        }
                        

                      
                        

                    }
                    if (temp_margin_top < 40)
                    {
                        temp_margin_top = 40;
                        temp_margin_bottom = temp_margin_bottom - (40 - temp_margin_top);
                    }
                    if (temp_margin_top >= temp_margin_bottom)
                    {
                        temp_margin_bottom = temp_margin_top + 1;
                    }
                    colorlable.Margin = new Thickness(120, temp_margin_top, 0, 0);
                    colorlable.Height = temp_margin_bottom - temp_margin_top;



                    
                   
                    

                    

                    //if (max_temp == min_temp)
                    //{
                    //    temp_height = 335;
                    //    temp_margin_top=40;
                    //}

    
 


                    SolidColorBrush newbrush = new SolidColorBrush();

                    newbrush.Color = (Color)ColorConverter.ConvertFromString("#" + newiso.color);
                    colorlable.Background = newbrush;
                    colorlable.Name = "colorlable" + (i + 1);
                    colorlable.MouseDown += new MouseButtonEventHandler(main_grid_MouseDown);
                    colorlable.MouseMove += new MouseEventHandler(main_grid_MouseMove);
                    left_canvas.Children.Add(colorlable);
                    left_canvas.RegisterName("colorlable" + (i + 1), colorlable);
                    cur_lable = left_canvas.FindName(colorlable.Name) as Label;
                    //color_lable.MouseDown += new MouseButtonEventHandler(main_grid_MouseDown);
                    //color_lable.MouseMove += new MouseEventHandler(main_grid_MouseMove);
                    num_up.Value = newiso.max_temp;
                    num_down.Value = newiso.min_temp;
                    num_up.Margin = new Thickness(70, temp_margin_top - 20, 0, 0);
                    num_down.Margin = new Thickness(70, temp_margin_bottom, 0, 0);
                    num_up.Opacity = 1;
                    num_down.Opacity = 1;
                }
            }
            

        }


        private void leftimage()
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
           workspace.cur_palette.Reverse();
            int[] pixels = new int[1];
            int stride = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;


            WriteableBitmap cur_palette_bmp = new WriteableBitmap(1, 256, 96, 96, PixelFormats.Bgr32, null);
            for (int i = 0; i < 256; i++)
            {
                int pixel = Convert.ToInt32(workspace.cur_palette[i].ToString(), 16);
                pixels[0] = pixel;
                cur_palette_bmp.WritePixels(new Int32Rect(0, i, 1, 1), pixels, stride, 0);
            }
            leftimg.Source = cur_palette_bmp;
            workspace.cur_palette.Reverse();
        }





        int txtcot = 0;//文本元素下标
        int iecot = 0;//刻度元素下标
        public void calculate_temp_ruler()//生成温度标尺
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            for (int i = 0; i < txtcot; i++)
            {
                TextBlock deletetxt = left_canvas.FindName("txt" + i) as TextBlock;
                if (deletetxt != null)
                {
                    left_canvas.Children.Remove(deletetxt);
                    left_canvas.UnregisterName("txt" + i);
                }
            }
            for (int i = 0; i < iecot; i++)
            {
                System.Windows.Shapes.Path deletepath = left_canvas.FindName("ie" + i) as System.Windows.Shapes.Path;
                if (deletepath != null)
                {
                    left_canvas.Children.Remove(deletepath);
                    left_canvas.UnregisterName("ie" + i);
                }
            }
            txtcot = 0;
            iecot = 0;

            TextBlock txtmax_temp = new TextBlock();
            //txtmax_temp.Text = float.Parse(workspace.temp_max_min_avr[0].ToString()) / 10 + " ℃";
            txtmax_temp.Text = (int)((float)(workspace.grad_max / 10) * 10) / 10.0 + " ℃";
            txtmax_temp.Margin = new Thickness(120, 25, 0, 0);
            left_canvas.Children.Add(txtmax_temp);
            left_canvas.RegisterName("txt" + txtcot, txtmax_temp);
            txtcot++;

            TextBlock txtmin_temp = new TextBlock();
            txtmin_temp.Text = float.Parse(workspace.temp_max_min_avr[2].ToString()) / 10 + " ℃";
            txtmin_temp.Text = (int)((float)(workspace.grad_min / 10) * 10) / 10.0 + " ℃";
            txtmin_temp.Margin = new Thickness(120, leftimg.Height+40, 0, 0);
            left_canvas.Children.Add(txtmin_temp);
            left_canvas.RegisterName("txt" + txtcot, txtmin_temp);
            txtcot++;

            LineGeometry baseline = new LineGeometry();
            baseline.StartPoint = new Point(152, 40);
            baseline.EndPoint = new Point(152, leftimg.Height+40);

            System.Windows.Shapes.Path mypath = new System.Windows.Shapes.Path();
            mypath.Stroke = Brushes.Black;
            mypath.StrokeThickness = 1;
            mypath.SnapsToDevicePixels = true;
            mypath.Data = baseline;
            left_canvas.Children.Add(mypath);
            left_canvas.RegisterName("ie" + iecot, mypath);
            iecot++;

            int step = (int)leftimg.Height % 5;
            if (step == 0)
            {
                step = 4;
            }
            int ruler_count = (int)(leftimg.Height+40 ) / 10;//刻度统计
            float min_ruler_temp = (float)(workspace.grad_max - workspace.grad_min) / ruler_count;
            int ruler_coe = 1;//刻度系数
            for (int i = 40; i >= 0; i = i + 10)
            {
                if (i > leftimg.Height+40 )
                {
                    break;
                }
                System.Windows.Shapes.Path rulerpath = new System.Windows.Shapes.Path();
                rulerpath.Stroke = Brushes.Black;
                rulerpath.SnapsToDevicePixels = true;
                if (step == 5)
                {

                    TextBlock rule_temp = new TextBlock();
                    int temp1 = (int)(workspace.grad_max - min_ruler_temp * ruler_coe);
                    rule_temp.Text = (float)temp1/10 + " ℃";
                    rule_temp.Margin = new Thickness(163, i-9, 0, 0);
                    left_canvas.Children.Add(rule_temp);
                    left_canvas.RegisterName("txt" + txtcot, rule_temp);
                    txtcot++;

                    LineGeometry ruler = new LineGeometry();
                    ruler.StartPoint = new Point(152, i);
                    ruler.EndPoint = new Point(162, i);
                    rulerpath.StrokeThickness = 2;
                    rulerpath.Data = ruler;
                }
                else
                {
                    LineGeometry ruler = new LineGeometry();
                    ruler.StartPoint = new Point(152, i);
                    ruler.EndPoint = new Point(157, i);
                    rulerpath.StrokeThickness = 1;
                    rulerpath.Data = ruler;
                }
                left_canvas.Children.Add(rulerpath);
                left_canvas.RegisterName("ie" + iecot, rulerpath);
                iecot++;

                if (step == 5)
                {
                    step = 0;
                }
                step++;
                ruler_coe++;
            }

        }

        private void init_isothermal_list()
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);

            StackPanel newstackpanl1 = new StackPanel();
            newstackpanl1.VerticalAlignment = VerticalAlignment.Center;
            newstackpanl1.Orientation = Orientation.Horizontal;
            Label bianhao = new Label();
            bianhao.Content = "编号";
            bianhao.Width = 55;
            newstackpanl1.Children.Add(bianhao);

            Label touming = new Label();
            touming.Content = "透明";
            touming.Width = 40;
            newstackpanl1.Children.Add(touming);

            Label yanse = new Label();
            yanse.Content = "颜色";
            yanse.Width = 45;
            newstackpanl1.Children.Add(yanse);

            Label max_tmp = new Label();
            max_tmp.Content = "最高温";
            max_tmp.Width = 55;
            newstackpanl1.Children.Add(max_tmp);

            Label min_tmp = new Label();
            min_tmp.Content = "最低温";
            min_tmp.Width = 55;
            newstackpanl1.Children.Add(min_tmp);

            isothermal_list.Items.Add(newstackpanl1);

            for (int i = 1; i <= 16; i++)
            {
                StackPanel newstackpanel = new StackPanel();
                newstackpanel.Name = "stackpanel" + i;
                newstackpanel.VerticalAlignment = VerticalAlignment.Center;
                newstackpanel.Orientation = Orientation.Horizontal;
                //newstackpanel.HorizontalAlignment = HorizontalAlignment.Center;
                newstackpanel.Height = 20;


                Label num = new Label();
                num.Name = "num" + i;
                num.Width = 23;
                num.Content = i;
                num.Margin = new Thickness(0, -5, 0, 0);
                newstackpanel.Children.Add(num);




                CheckBox isselected = new CheckBox();
                isselected.Name = "isselected" + i;
                
                isselected.FontSize = 15;
                isselected.Width = 40;
                isselected.Content = "";
                //newcheckbox.Margin = new Thickness(15, 0, 0, 0);
                isselected.Click += new RoutedEventHandler(isselected_Click);
                newstackpanel.Children.Add(isselected);
                //newstackpanel.RegisterName("isselected" + i, isselected);
                


                CheckBox isopacity = new CheckBox();
                isopacity.Name = "is_opacity" + i;
                isopacity.FontSize = 15;
                isopacity.Width = 40;
                isopacity.Content = "";
                //newcheckbox1.Margin = new Thickness(25,-10,0,0);
                
                isopacity.Click += new RoutedEventHandler(isopacity_Click);
                newstackpanel.Children.Add(isopacity);
                //newstackpanel.RegisterName(isopacity.Name, isopacity);



                //Label isoth_color = new Label();
                //isoth_color.Width = 30;
                //isoth_color.Height = 20;
                PublicClass.isothermal_property newiso_color = (PublicClass.isothermal_property)workspace.isothermal_list[i-1];
                //isoth_color.Name = "color_temp" + i;
                ////isoth_color.Margin = new Thickness(100, 0, 0, 0);
                //// isoth_color.Content = newiso_color.color;
                ////  string col = newiso_color.color;

                //SolidColorBrush newbrush = new SolidColorBrush();
                //newbrush.Color = (Color)ColorConverter.ConvertFromString("#" + newiso_color.color);
                //isoth_color.Background = newbrush;

                //newstackpanel.Children.Add(isoth_color);


                C1.WPF.Extended.C1ColorPicker colorpicker = new C1.WPF.Extended.C1ColorPicker();
                colorpicker.Width = 40;
                colorpicker.Height = 20;
                colorpicker.Name = "color_temp" + i;
                colorpicker.SelectedColor = (Color)ColorConverter.ConvertFromString("#" + newiso_color.color);
                colorpicker.SelectedColorChanged += new EventHandler<C1.WPF.PropertyChangedEventArgs<Color>>(colorpicker_SelectedColorChanged);
                newstackpanel.Children.Add(colorpicker);






                Label max_temp = new Label();
                PublicClass.isothermal_property newiso_max = (PublicClass.isothermal_property)workspace.isothermal_list[i-1];
                max_temp.Name = "max_temp" + i;
                max_temp.Width = 50;
                max_temp.Margin = new Thickness(10, -5, 0, 0);
                max_temp.Content = (newiso_max.max_temp);
                newstackpanel.Children.Add(max_temp);


                Label min_temp = new Label();
                PublicClass.isothermal_property newiso_min = (PublicClass.isothermal_property)workspace.isothermal_list[i-1];
                min_temp.Name = "min_temp" + i;
                min_temp.Width = 50;
                min_temp.Margin = new Thickness(0, -5, 0, 0);
                min_temp.Content = (newiso_min.min_temp);
                newstackpanel.Children.Add(min_temp);





                
                isothermal_list.Items.Add(newstackpanel);
                isothermal_list.RegisterName(newstackpanel.Name, newstackpanel);


            }
        }

        void isopacity_Click(object sender, RoutedEventArgs e)
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            CheckBox newcheckbox = sender as CheckBox;
            if (newcheckbox != null)
            {
                int control_name = int.Parse(newcheckbox.Name.ToString().Substring(10, newcheckbox.Name.ToString().Length - 10)) - 1;
                PublicClass.isothermal_property newiso = (PublicClass.isothermal_property)workspace.isothermal_list[control_name];
                /////ISO//////
                var isolist = from c in PublicClass.Iso_Shapes_list where c.shapes_name == workspace.shapes_name && c.workspace_name == PublicClass.cur_ctrl_name select c;
                /////ISOEND///

                if ((bool)newcheckbox.IsChecked)
                {
                    
                    newiso.is_opacity = true;
                }
                else
                {
                    newiso.is_opacity = false;
                }

                if (isolist.Count() > 0)
                {
                    isolist.First().is_opacity = newiso.is_opacity;
                }
                ///ISO//////
                ///ISOEND///

                workspace.isothermal_list[control_name] = newiso;
                
            }
            workspace.create_img();
        }

        void colorpicker_SelectedColorChanged(object sender, C1.WPF.PropertyChangedEventArgs<Color> e)
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            C1.WPF.Extended.C1ColorPicker newcolor = sender as C1.WPF.Extended.C1ColorPicker;
            int control_name = int.Parse(newcolor.Name.ToString().Substring(10, newcolor.Name.ToString().Length - 10)) - 1;
            PublicClass.isothermal_property newiso = (PublicClass.isothermal_property)workspace.isothermal_list[control_name];
            newiso.color = len_2(newcolor.SelectedColor.R) + len_2(newcolor.SelectedColor.G) + len_2(newcolor.SelectedColor.B);
            workspace.isothermal_list[control_name] = newiso;

            Label newlable = left_canvas.FindName("colorlable" + (control_name + 1)) as Label;
            if (newlable != null)
            {
                SolidColorBrush newbrush = new SolidColorBrush();
                newbrush.Color = (Color)ColorConverter.ConvertFromString("#" + newiso.color);
                newlable.Background = newbrush;
                thread_creat_img();
            }

            
        }


        private string len_2(byte bte)//转换16进制补满两位
        {
            string strbte;
            strbte = Convert.ToString(bte, 16);
            if (strbte.Length < 2)
            {
                strbte = "0" + strbte;
            }
            return strbte;
        }


        void isselected_Click(object sender, RoutedEventArgs e)
        {
            CheckBox newcheckbox = (CheckBox)sender;
            string control_name = newcheckbox.Name.ToString().Substring(10, newcheckbox.Name.ToString().Length - 10);
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);

            double max_temp = (int)(((float)workspace.grad_max / 10)*10)/10.0;


            double min_temp = (int)(((float)workspace.grad_min / 10) * 10) / 10.0;
            PublicClass.isothermal_property newiso = (PublicClass.isothermal_property)workspace.isothermal_list[int.Parse(control_name) - 1];
            if ((bool)newcheckbox.IsChecked)
            {
                Label color_lable = new Label();
                color_lable.Name = "colorlable" + control_name;
                SolidColorBrush newbrush = new SolidColorBrush();

                newbrush.Color = (Color)ColorConverter.ConvertFromString("#" + newiso.color);
                color_lable.Background = newbrush;
                color_lable.Width = 30;



                double temp_margin_top = 40;
                //double temp_height;
                double temp_margin_bottom = 375;



                if (max_temp == min_temp)
                {
                    if (newiso.max_temp > max_temp)
                    {
                        temp_margin_top = 40;
                        temp_margin_bottom = 41;
                    }
                    if (newiso.max_temp < max_temp)
                    {
                        temp_margin_top = 374;
                        temp_margin_bottom = 375;
                    }

                    //if (newiso.min_temp < min_temp)
                    //{
                    //    temp_margin_bottom = 375;
                    //}
                    if (newiso.max_temp == max_temp)
                    {
                        temp_margin_top = 40;
                        temp_margin_bottom = 375;
                    }
                }
                else
                {

                    temp_margin_top = (max_temp - newiso.max_temp) / (max_temp - min_temp) * 335 + 40;
                    temp_margin_bottom = (max_temp - newiso.min_temp) / (max_temp - min_temp) * 335 + 40;
                    if ((max_temp - newiso.max_temp) / (max_temp - min_temp) * 335 + 40 < 0)
                    {
                        temp_margin_top = 40;
                    }
                     if ((max_temp - newiso.min_temp) / (max_temp - min_temp) * 335 + 40 <= 0)
                    {
                        temp_margin_bottom = 41;
                    }

                     if ((max_temp - newiso.max_temp) / (max_temp - min_temp) * 335 + 40 > 375)
                    {
                        temp_margin_top = 374;
                    }
                     if ((max_temp - newiso.min_temp) / (max_temp - min_temp) * 335 + 40 > 375)
                    {
                        temp_margin_bottom = 375;
                    }
                

                }

                if (temp_margin_top == temp_margin_bottom)
                {
                    temp_margin_bottom = temp_margin_top + 1;
                }
                color_lable.Margin = new Thickness(120, temp_margin_top, 0, 0);
                color_lable.Height = temp_margin_bottom - temp_margin_top;


               



                color_lable.MouseDown += new MouseButtonEventHandler(main_grid_MouseDown);
                color_lable.MouseMove += new MouseEventHandler(main_grid_MouseMove);

                left_canvas.Children.Add(color_lable);
                left_canvas.RegisterName(color_lable.Name, color_lable);
                cur_lable = left_canvas.FindName(color_lable.Name) as Label;
                newiso.is_checked = true;
                workspace.isothermal_list[int.Parse(control_name) - 1] = newiso;
                set_Zindex();
                num_up.Opacity = 1;
                num_down.Opacity = 1;
                num_up.Value = newiso.max_temp;
                num_down.Value = newiso.min_temp;
                num_up.Margin = new Thickness(70, temp_margin_top - 20, 0, 0);
                num_down.Margin = new Thickness(70, temp_margin_bottom, 0, 0);
            }

            if (!(bool)newcheckbox.IsChecked)
            {
                Label deletelable = (Label)left_canvas.FindName("colorlable" + control_name);
                if (deletelable != null)
                {
                    left_canvas.Children.Remove(deletelable);
                    left_canvas.UnregisterName("colorlable" + control_name);
                }
                newiso.is_checked = false;
                workspace.isothermal_list[int.Parse(control_name) - 1] = newiso;
                num_up.Opacity = 0;
                num_down.Opacity = 0;
            }

            workspace.create_img();

        }

        private void main_grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            cur_lable = (Label)left_canvas.FindName(process_lable_name);
            if (cur_lable != null)
            {
                old_point.Y = e.GetPosition(main_grid).Y - cur_lable.Margin.Top;
                old_height = cur_lable.Height;
                old_margin_top = cur_lable.Margin.Top;
                old_margin_bottom = cur_lable.Margin.Top + cur_lable.Height;
                set_Zindex();

            }
            
            
        }


        private void set_Zindex()
        {
            for (int i = 1; i <= 16; i++)
            {
                Label newlable = left_canvas.FindName("colorlable" + i) as Label;
                if (newlable != null)
                {
                    Panel.SetZIndex(newlable, i);
                }
            }

            Panel.SetZIndex(cur_lable, 20);

            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
             int control_name =int.Parse(  cur_lable.Name.ToString().Substring(10,cur_lable.Name.ToString().Length-10))-1;
             PublicClass.isothermal_property newiso = (PublicClass.isothermal_property)workspace.isothermal_list[control_name];
             num_up.Value = newiso.max_temp;
             num_down.Value = newiso.min_temp;
             num_up.Margin = new Thickness(70, cur_lable.Margin.Top - 20, 0, 0);
             num_down.Margin = new Thickness(70, cur_lable.Margin.Top + cur_lable.Height, 0, 0);
 
            

            
        }

        private void main_grid_MouseMove(object sender, MouseEventArgs e)
        {
            List<string> select_lable = new List<string>();
            bool change_cursor = false;




            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (process_lable_name != "")
                {
                 
                    if (process_mode == "all")
                    {
                        cur_lable.Margin = new Thickness(120, e.GetPosition(main_grid).Y - old_point.Y, 0, 0);
                        num_up.Margin = new Thickness(70, cur_lable.Margin.Top - 20, 0, 0);
                        num_down.Margin = new Thickness(70, cur_lable.Margin.Top +cur_lable.Height, 0, 0);
                        if (cur_lable.Margin.Top < 40)
                        {
                            cur_lable.Margin = new Thickness(120, 40, 0, 0);
                            num_up.Margin = new Thickness(70, 20, 0, 0);
                            num_down.Margin = new Thickness(70, cur_lable.Margin.Top+cur_lable.Height, 0, 0);
                        }
                        else if (cur_lable.Margin.Top > 375 - cur_lable.Height)
                        {
                            cur_lable.Margin = new Thickness(120, 375 - cur_lable.Height, 0, 0);
                            num_up.Margin = new Thickness(70, 375 - cur_lable.Height - 20, 0, 0);
                            num_down.Margin = new Thickness(70, cur_lable.Margin.Top+cur_lable.Height, 0, 0);
                        }

                       
                    }
                    else if (process_mode == "up")
                    {
     ;
           
                                cur_lable.Margin = new Thickness(120, e.GetPosition(main_grid).Y - old_point.Y, 0, 0);
                                num_up.Margin = new Thickness(70, e.GetPosition(main_grid).Y - old_point.Y-20, 0, 0);
                                if (old_height + (old_margin_top - cur_lable.Margin.Top) > 0)
                                {
                                    cur_lable.Height = old_height + (old_margin_top - cur_lable.Margin.Top);
                                }
                                if (old_margin_bottom < (cur_lable.Margin.Top + 10))
                                {
                                    cur_lable.Margin = new Thickness(120, old_margin_bottom - 10, 0, 0);
                                    cur_lable.Height = 10;

                                    num_up.Margin = new Thickness(70, old_margin_bottom - 30, 0, 0);
                                  
                                }
                                else if (cur_lable.Margin.Top < 40)
                                {
                                    cur_lable.Margin = new Thickness(120, 40, 0, 0);
                                    num_up.Margin = new Thickness(70, 20, 0, 0);
                                    cur_lable.Height = old_height + (old_margin_top - cur_lable.Margin.Top);
                                }
                             
                        
                    }
                    else if (process_mode == "down" )
                    {

                        if (old_height + (e.GetPosition(main_grid).Y - old_margin_top - old_point.Y) > 0)
                        {
                            cur_lable.Height = old_height + (e.GetPosition(main_grid).Y - old_margin_top - old_point.Y);
                            num_down.Margin = new Thickness(70, cur_lable.Margin.Top+ cur_lable.Height, 0, 0);
                        }
                        if (cur_lable.Height < 10)
                        {
                            cur_lable.Height = 10;
                            num_down.Margin = new Thickness(70, cur_lable.Margin.Top+10, 0, 0);
                            
                        }
                        if (old_margin_top + cur_lable.Height > 375)
                        {
                            cur_lable.Height = 375 - old_margin_top;
                            num_down.Margin = new Thickness(70, 375, 0, 0);
                        }
                       
                    }

                    sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);

                    double max_temp = workspace.grad_max / 10.0;
                    double min_temp = workspace.grad_min / 10.0;
                    //string control_name = newcheckbox.Name.ToString().Substring(10, newcheckbox.Name.ToString().Length - 10);
                    int control_name =int.Parse(  cur_lable.Name.ToString().Substring(10,cur_lable.Name.ToString().Length-10))-1;
                    PublicClass.isothermal_property newiso=(PublicClass.isothermal_property)workspace.isothermal_list[control_name];

                    newiso.max_temp = Math.Round(max_temp - ((cur_lable.Margin.Top - 40) / 335 * (max_temp - min_temp) + min_temp) + min_temp, 1);
                    newiso.min_temp = Math.Round(max_temp - ((cur_lable.Margin.Top + cur_lable.Height - 40) / 335 * (max_temp - min_temp) + min_temp) + min_temp, 1);
                    num_up.Value = newiso.max_temp;
                    num_down.Value = newiso.min_temp;

                  //workspace.create_img();
                    try
                    {
                        newthread.Abort();
                    }
                    catch { }
                    thread_creat_img();

                    lable_change();

                }
            }

            else
            {
                for (int i = 1; i <= 16; i++)
                {
                    Label newlable = (Label)left_canvas.FindName("colorlable" + i.ToString());
                    if (newlable != null)
                    {
                        if (e.GetPosition(main_grid).X > newlable.Margin.Left && e.GetPosition(main_grid).X < newlable.Margin.Left + newlable.Width && e.GetPosition(main_grid).Y > newlable.Margin.Top && e.GetPosition(main_grid).Y < newlable.Margin.Top + newlable.Height)
                        {
                            main_grid.Cursor = Cursors.SizeAll;
                            process_mode = "all";
                            if (e.GetPosition(main_grid).Y < newlable.Margin.Top + 3)
                            {
                                main_grid.Cursor = Cursors.SizeNS;
                                process_mode = "up";
                            }
                            if (e.GetPosition(main_grid).Y > newlable.Margin.Top + newlable.Height - 3)
                            {
                                main_grid.Cursor = Cursors.SizeNS;
                                process_mode = "down";
                            }
                            process_lable_name = newlable.Name;
                            change_cursor = true;




                        }
                    }
                    else if (!change_cursor)
                    {
                        main_grid.Cursor = Cursors.Arrow;
                        process_lable_name = "";
                    }
                }

                if (cur_lable != null)
                {
                    if (e.GetPosition(main_grid).X > cur_lable.Margin.Left && e.GetPosition(main_grid).X < cur_lable.Margin.Left + cur_lable.Width && e.GetPosition(main_grid).Y > cur_lable.Margin.Top && e.GetPosition(main_grid).Y < cur_lable.Margin.Top + cur_lable.Height)
                    {
                        main_grid.Cursor = Cursors.SizeAll;
                        process_mode = "all";
                        if (e.GetPosition(main_grid).Y < cur_lable.Margin.Top + 3)
                        {
                            main_grid.Cursor = Cursors.SizeNS;
                            process_mode = "up";
                        }
                        if (e.GetPosition(main_grid).Y > cur_lable.Margin.Top + cur_lable.Height - 3)
                        {
                            main_grid.Cursor = Cursors.SizeNS;
                            process_mode = "down";
                        }
                        process_lable_name = cur_lable.Name;
                        change_cursor = true;




                    }
                }
                else if (!change_cursor)
                {
                    main_grid.Cursor = Cursors.Arrow;
                    process_lable_name = "";
                }



            }



        }


        private void thread_creat_img()
        {
             newthread = new Thread(new ThreadStart(() =>
        {
            Dispatcher.Invoke(new Action(() =>
                {


                    sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
                    workspace.create_img();
                    




                }));



        }));
             newthread.SetApartmentState(ApartmentState.MTA);
             newthread.IsBackground = true;
            
             newthread.Priority = ThreadPriority.Lowest;
             newthread.Start();
          


        }

        private void num_MouseUp(object sender, MouseButtonEventArgs e)
        {
            C1.WPF.C1NumericBox newlable = sender as C1.WPF.C1NumericBox;
            num_change(newlable.Name);
            lable_change();
        }


        private void num_change(string labename)
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);

            double max_temp = workspace.grad_max / 10.0;
            double min_temp = workspace.grad_min / 10.0;

            if (labename == "num_up")
            {
                if (num_up.Value > max_temp)
                {
                    num_up.Value = max_temp;
                }
                if (num_up.Value < num_down.Value)
                {
                    num_up.Value = num_down.Value;
                }
            }
            else
            {
                if (num_down.Value < min_temp)
                {
                    num_down.Value = min_temp;
                }
                if (num_down.Value > num_up.Value)
                {
                    num_down.Value = num_up.Value;
                }
            }
    


            int control_name = int.Parse(cur_lable.Name.ToString().Substring(10, cur_lable.Name.ToString().Length - 10)) - 1;
            PublicClass.isothermal_property newiso = (PublicClass.isothermal_property)workspace.isothermal_list[control_name];
            newiso.max_temp = Math.Round( num_up.Value,1);
            newiso.min_temp = Math.Round( num_down.Value,1);
            workspace.isothermal_list[control_name] = newiso;



            double temp_margin_top = 40;
            //double temp_height;
            double temp_margin_bottom = 375;



            if (max_temp == min_temp)
            {
                if (newiso.max_temp > max_temp)
                {
                    temp_margin_top = 40;
                    temp_margin_bottom = 41;
                }
                if (newiso.max_temp < max_temp)
                {
                    temp_margin_top = 374;
                    temp_margin_bottom = 375;
                }

                //if (newiso.min_temp < min_temp)
                //{
                //    temp_margin_bottom = 375;
                //}
                //else
                //{
                //    temp_margin_top = 40;
                //    temp_margin_bottom = 375;
                //}
            }
            else
            {
                temp_margin_top = (max_temp - newiso.max_temp) / (max_temp - min_temp) * 335 + 40;
                temp_margin_bottom = (max_temp - newiso.min_temp) / (max_temp - min_temp) * 335 + 40;

                if ((max_temp - newiso.max_temp) / (max_temp - min_temp) * 335 + 40 < 0)
                {
                    temp_margin_top = 40;
                }
                if ((max_temp - newiso.min_temp) / (max_temp - min_temp) * 335 + 40 <= 0)
                {
                    temp_margin_bottom = 41;
                }

                if ((max_temp - newiso.max_temp) / (max_temp - min_temp) * 335 + 40 > 375)
                {
                    temp_margin_top = 374;
                }
                if ((max_temp - newiso.min_temp) / (max_temp - min_temp) * 335 + 40 > 375)
                {
                    temp_margin_bottom = 375;
                }





            }

            if (temp_margin_top == temp_margin_bottom)
            {
                temp_margin_bottom = temp_margin_top + 1;
            }
            cur_lable.Margin = new Thickness(120, temp_margin_top, 0, 0);
            cur_lable.Height = temp_margin_bottom - temp_margin_top;
            num_up.Margin = new Thickness(70, temp_margin_top - 20, 0, 0);
            num_down.Margin = new Thickness(70, temp_margin_bottom, 0, 0);




           


        }

        private void num_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                C1.WPF.C1NumericBox newlable = sender as C1.WPF.C1NumericBox;
                num_change(newlable.Name);
                lable_change();
            }
        }
        private void lable_change()
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            for (int i = 0; i < 16; i++)
            {
                PublicClass.isothermal_property newiso = (PublicClass.isothermal_property)workspace.isothermal_list[i];
                if (newiso.is_checked)
                {
                    StackPanel newstackpanel = (StackPanel)isothermal_list.Items[i + 1];
                    for (int j = 0; j < newstackpanel.Children.Count; j++)
                    {
                        Label isselected = newstackpanel.Children[j] as Label;
                        if (isselected != null)
                        {
                            if (isselected.Name == "max_temp" + (i + 1))
                            {
                                isselected.Content = newiso.max_temp;
                            }
                            if (isselected.Name == "min_temp" + (i + 1))
                            {
                                isselected.Content = newiso.min_temp;
                            }

                        }
                    }
                }
            }
        }

        private void isothermal_btn_Click(object sender, RoutedEventArgs e)
        {
            RoutedPropertyChangedEventArgs<object> args = new RoutedPropertyChangedEventArgs<object>(this, e);
            args.RoutedEvent = sub_isothermal.IsoEvent;
            this.RaiseEvent(args);
           
        }

 
  
    }
}
