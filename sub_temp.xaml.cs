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
    /// sub_temp.xaml 的交互逻辑
    /// </summary>
    public partial class sub_temp : System.Windows.Controls.UserControl
    {
        public sub_temp()
        {
            InitializeComponent();
        }


        public static readonly RoutedEvent TempMouseUpEvent = EventManager.RegisterRoutedEvent("TempMouseUp", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(sub_temp));
        public event RoutedPropertyChangedEventHandler<object> TempMouseUp
        {
            add { AddHandler(TempMouseUpEvent, value); }
            remove { RemoveHandler(TempMouseUpEvent, value); }
        }


        public static readonly RoutedEvent Temp1MouseUpEvent = EventManager.RegisterRoutedEvent("Temp1MouseUp", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(sub_temp));
        public event RoutedPropertyChangedEventHandler<object> Temp1MouseUp
        {
            add { AddHandler(Temp1MouseUpEvent, value); }
            remove { RemoveHandler(Temp1MouseUpEvent, value); }
        }

        public static readonly RoutedEvent Temp2MouseUpEvent = EventManager.RegisterRoutedEvent("Temp2MouseUp", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(sub_temp));
        public event RoutedPropertyChangedEventHandler<object> Temp2MouseUp
        {
            add { AddHandler(Temp2MouseUpEvent, value); }
            remove { RemoveHandler(Temp2MouseUpEvent, value); }
        }

        private void max_temp_Click(object sender, RoutedEventArgs e)
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(System.Windows.Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            PublicClass.shapes_property test = new PublicClass.shapes_property();

            if ((bool)max_temp.IsChecked == true)
            {
                stack1.IsEnabled = true;
                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                    if (test.shapes_type == "max_temp_temp")
                    {
                        test.shapes_type = "max_temp";
                        PublicClass.shapes_count[i] = test;
                        workspace.re_create_shapes();
                    }
                }
            }
            else
            {
                stack1.IsEnabled = false;
                max_temp.IsChecked = false;
                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                    if (test.shapes_type == "max_temp")
                    {
                        test.shapes_type = "max_temp_temp";
                        PublicClass.shapes_count[i] = test;
                        workspace.re_create_shapes();
                    }
                }
            }



        }

        private void temp1_Click(object sender, RoutedEventArgs e)
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(System.Windows.Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            PublicClass.shapes_property test = new PublicClass.shapes_property();
            if (temp1.IsChecked == true && sign1.IsChecked == true)
            {
                workspace.spot_temp_max = 3;
                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                    if (test.shapes_type == "max_temp_temp")
                    {
                        test.shapes_type = "max_temp";
                        PublicClass.shapes_count[i] = test;
                        workspace.re_create_shapes();
                    }
                    else
                    {
                        workspace.re_create_shapes();
                    }
                }
            }
            else if (temp1.IsChecked == false && sign1.IsChecked == true)
            {
                workspace.spot_temp_max = 2;
                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                    if (test.shapes_type == "max_temp_temp")
                    {
                        test.shapes_type = "max_temp";
                        PublicClass.shapes_count[i] = test;
                        workspace.re_create_shapes();
                    }
                    else
                    {
                        workspace.re_create_shapes();
                    }
                }
                
            }
            else if (temp1.IsChecked == true && sign1.IsChecked == false)
            {
                workspace.spot_temp_max = 1;
                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                    if (test.shapes_type == "max_temp_temp")
                    {
                        test.shapes_type = "max_temp";
                        PublicClass.shapes_count[i] = test;
                        workspace.re_create_shapes();
                    }
                    else
                    {
                        workspace.re_create_shapes();
                    }
                }
            }
            else
            {
                workspace.spot_temp_max = 0;
                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                    if (test.shapes_type == "max_temp")
                    {
                        test.shapes_type = "max_temp_temp";
                        PublicClass.shapes_count[i] = test;
                        workspace.re_create_shapes();
                    }
                }
            
            }


        }


        private void min_temp_Click(object sender, RoutedEventArgs e)
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(System.Windows.Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            PublicClass.shapes_property test = new PublicClass.shapes_property();

            if ((bool)min_temp.IsChecked == true)
            {
                stack2.IsEnabled = true;
                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                    if (test.shapes_type == "min_temp_temp")
                    {
                        test.shapes_type = "min_temp";
                        PublicClass.shapes_count[i] = test;
                        workspace.re_create_shapes();
                    }
                }
            }
            else
            {
                stack2.IsEnabled = false;
                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                    if (test.shapes_type == "min_temp")
                    {
                        test.shapes_type = "min_temp_temp";
                        PublicClass.shapes_count[i] = test;
                        workspace.re_create_shapes();
                    }
                }
            }
        }

        private void center_temp_Click(object sender, RoutedEventArgs e)
        {

            if ((bool)center_temp.IsChecked)
            {
                stack3.IsEnabled = true;
                PublicClass.is_cur_temp = "true";
            }
            else
            {
                stack3.IsEnabled = false;
                PublicClass.is_cur_temp = "false";
            }


        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            read_load();
        }




        private void read_load()
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(System.Windows.Application.Current.MainWindow, PublicClass.cur_ctrl_name);

            if (new SolidColorBrush(Color.FromRgb(color9.SelectedColor.R, color9.SelectedColor.G, color9.SelectedColor.B)) != workspace.spot_max)
            {

                color9.SelectedColor = (Color)ColorConverter.ConvertFromString(workspace.spot_max.ToString());

            }
            if (new SolidColorBrush(Color.FromRgb(color10.SelectedColor.R, color10.SelectedColor.G, color10.SelectedColor.B)) != workspace.spot_min)
            {

                color10.SelectedColor = (Color)ColorConverter.ConvertFromString(workspace.spot_min.ToString());

            }
            if (new SolidColorBrush(Color.FromRgb(color11.SelectedColor.R, color11.SelectedColor.G, color11.SelectedColor.B)) != workspace.spot_cen)
            {


                color11.SelectedColor = (Color)ColorConverter.ConvertFromString(workspace.spot_cen.ToString());
            }

            PublicClass.shapes_property test = new PublicClass.shapes_property();
            if (PublicClass.is_cur_temp == "true")
            {
                center_temp.IsChecked = true;
                stack3.IsEnabled = true;
            }
            for (int i = 0; i < PublicClass.shapes_count.Count; i++)
            {
                test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                if (test.shapes_type == "min_temp")
                {
                    min_temp.IsChecked = true;
                    stack2.IsEnabled = true;
                }
                else if (test.shapes_type == "max_temp")
                {
                    max_temp.IsChecked = true;
                    stack1.IsEnabled = true;
                }
            }

            if (workspace.spot_temp_max == 0)
            {
                temp1.IsChecked = false;
                sign1.IsChecked = false;
            }
            else if (workspace.spot_temp_max == 1)
            {
                temp1.IsChecked = false;
                sign1.IsChecked = true;
            }
            else if (workspace.spot_temp_max == 2)
            {
                temp1.IsChecked = true;
                sign1.IsChecked = false;
            }
            else if (workspace.spot_temp_max == 3)
            {
                temp1.IsChecked = true;
                sign1.IsChecked = true;
            }
            if (workspace.spot_temp_min == 0)
            {
                temp2.IsChecked = false;
                sign2.IsChecked = false;

            }
            else if (workspace.spot_temp_min == 1)
            {
                temp2.IsChecked = true;
                sign2.IsChecked = false;
            }
            else if (workspace.spot_temp_min == 2)
            {
                temp2.IsChecked = false;
                sign2.IsChecked = true;
            }
            else if (workspace.spot_temp_min == 3)
            {
                temp2.IsChecked = true;
                sign2.IsChecked = true;
            }
            if (workspace.spot_temp_cen == 0)
            {

                temp3.IsChecked = false;
                sign3.IsChecked = false;
            }
            else if (workspace.spot_temp_cen == 1)
            {
                temp3.IsChecked = true;
                sign3.IsChecked = false;
            }
            else if (workspace.spot_temp_cen == 2)
            {
                temp3.IsChecked = false;
                sign3.IsChecked = true;
            }
            else if (workspace.spot_temp_cen == 3)
            {
                temp3.IsChecked = true;
                sign3.IsChecked = true;
            }
        }





        private void temp2_Click(object sender, RoutedEventArgs e)
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(System.Windows.Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            PublicClass.shapes_property test = new PublicClass.shapes_property();
            if (temp2.IsChecked == true && sign2.IsChecked == true)
            {
                workspace.spot_temp_min = 3;
                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                    if (test.shapes_type == "min_temp_temp")
                    {
                        test.shapes_type = "min_temp";
                        PublicClass.shapes_count[i] = test;
                        workspace.re_create_shapes();
                    }
                    else
                    {
                        workspace.re_create_shapes();
                    }
                }
            }
            else if (temp2.IsChecked == false && sign2.IsChecked == true)
            {
                workspace.spot_temp_min = 2;
                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                    if (test.shapes_type == "min_temp_temp")
                    {
                        test.shapes_type = "min_temp";
                        PublicClass.shapes_count[i] = test;
                        workspace.re_create_shapes();
                    }
                    else
                    {
                        workspace.re_create_shapes();
                    }
                }

            }
            else if (temp2.IsChecked == true && sign2.IsChecked == false)
            {
                workspace.spot_temp_min = 1;
                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                    if (test.shapes_type == "min_temp_temp")
                    {
                        test.shapes_type = "min_temp";
                        PublicClass.shapes_count[i] = test;
                        workspace.re_create_shapes();
                    }
                    else
                    {
                        workspace.re_create_shapes();
                    }
                }
            }
            else
            {
                workspace.spot_temp_min = 0;
                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                    if (test.shapes_type == "min_temp")
                    {
                        test.shapes_type = "min_temp_temp";
                        PublicClass.shapes_count[i] = test;
                        workspace.re_create_shapes();
                    }
                }

            }

        }


        private void temp3_Click(object sender, RoutedEventArgs e)
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(System.Windows.Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            if (temp3.IsChecked == true && sign3.IsChecked ==true)
            {
                workspace.spot_temp_cen = 3;
            }
            else if (temp3.IsChecked == true && sign3.IsChecked == false)
            {
                workspace.spot_temp_cen = 2;
            }
            else if (temp3.IsChecked == false && sign3.IsChecked == true)
            {
                workspace.spot_temp_cen = 1;
            }
            else if (temp3.IsChecked == false && sign3.IsChecked == false)
            {
                workspace.spot_temp_cen = 0;
            }

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            RoutedPropertyChangedEventArgs<object> args = new RoutedPropertyChangedEventArgs<object>(this, e);
            args.RoutedEvent = sub_temp.TempMouseUpEvent;
            this.RaiseEvent(args);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            RoutedPropertyChangedEventArgs<object> args = new RoutedPropertyChangedEventArgs<object>(this, e);
            args.RoutedEvent = sub_temp.Temp1MouseUpEvent;
            this.RaiseEvent(args);
        }

        private void color9_SelectedColorChanged(object sender, C1.WPF.PropertyChangedEventArgs<System.Windows.Media.Color> e)
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(System.Windows.Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            C1.WPF.Extended.C1ColorPicker newcolor = sender as C1.WPF.Extended.C1ColorPicker;
            string str = len_2(newcolor.SelectedColor.R) + len_2(newcolor.SelectedColor.G) + len_2(newcolor.SelectedColor.B);
            SolidColorBrush newbrush = new SolidColorBrush();
            newbrush.Color = (Color)ColorConverter.ConvertFromString("#" + str);
            workspace.spot_max = newbrush;
            workspace.re_create_shapes();
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

        private void color10_SelectedColorChanged(object sender, C1.WPF.PropertyChangedEventArgs<Color> e)
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(System.Windows.Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            C1.WPF.Extended.C1ColorPicker newcolor = sender as C1.WPF.Extended.C1ColorPicker;
            string str = len_2(newcolor.SelectedColor.R) + len_2(newcolor.SelectedColor.G) + len_2(newcolor.SelectedColor.B);
            SolidColorBrush newbrush = new SolidColorBrush();
            newbrush.Color = (Color)ColorConverter.ConvertFromString("#" + str);
            workspace.spot_min = newbrush;
            workspace.re_create_shapes();
        }

        private void color11_SelectedColorChanged(object sender, C1.WPF.PropertyChangedEventArgs<Color> e)
        {

            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(System.Windows.Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            C1.WPF.Extended.C1ColorPicker newcolor = sender as C1.WPF.Extended.C1ColorPicker;
            string str = len_2(newcolor.SelectedColor.R) + len_2(newcolor.SelectedColor.G) + len_2(newcolor.SelectedColor.B);
            SolidColorBrush newbrush = new SolidColorBrush();
            newbrush.Color = (Color)ColorConverter.ConvertFromString("#" + str);
            workspace.spot_cen = newbrush;
        }

    }
}
