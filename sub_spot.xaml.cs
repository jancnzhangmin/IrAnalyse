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
using System.IO;
using System.Collections;
using System.ComponentModel;

namespace IrAnalyse
{
    /// <summary>
    /// sub_spot.xaml 的交互逻辑
    /// </summary>
    public partial class sub_spot : UserControl
    {
        public sub_spot()
        {
            InitializeComponent();
        }

        public ArrayList spot_information { set; get; }
        sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
        
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
        public CheckBox check { set; get; }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            if (workspace != null)
            {
                get_spot_area();
            }
        }
        public void get_spot_area()//图片spot点和area区域信息
        {
            PublicClass.shapes_property test = new PublicClass.shapes_property();
            ArrayList test1 = new ArrayList();
            test1 = PublicClass.shapes_count;
            spot.Children.Clear();
            area.Children.Clear();
            for (int i = 0; i < test1.Count; i++)
            {
                test = (PublicClass.shapes_property)test1[i];
                if (test.shapes_type == "spot" && test.workspace_name == PublicClass.cur_ctrl_name)
                {
                    CheckBox check = new CheckBox();
                    check.Content = test.shapes_name + "                        " + (float)test.max_temp / 10 + " ℃" + " { X=" + test.vertex[0] + "," + "Y=" + +test.vertex[1] + "}";
                    check.Name = test.shapes_name;
                    check.Margin = new Thickness(5,3,0,3);
                    check.Click += new RoutedEventHandler(check_Click);
                    int in_shapes = 0;
                    if (workspace.selected_spot_area != null)
                    {
                        in_shapes = (from c in workspace.selected_spot_area where check.Name == c select c).Count();
                    }
                    if (in_shapes>0)
                    {
                        check.IsChecked = true;
                    }
                    spot.Children.Add(check);

                }
                else if (test.shapes_type == "area" && test.workspace_name == PublicClass.cur_ctrl_name)
                {
                    CheckBox check = new CheckBox();
                    check.Content = test.shapes_name + "                        " + (float)test.max_temp / 10 + " ℃" + " { X=" + test.vertex[0] + "," + "Y=" + +test.vertex[1] + "}";
                    check.Name = test.shapes_name;
                    check.Margin = new Thickness(5, 3, 0, 3);
                    check.Click += new RoutedEventHandler(check_Click);
                    int in_shapes = 0;
                    if (workspace.selected_spot_area != null)
                    {
                        in_shapes = (from c in workspace.selected_spot_area where check.Name == c select c).Count();
                    }   
                    if (in_shapes > 0)
                    {
                        check.IsChecked = true;
                    }
                    area.Children.Add(check);
                
                }
            }
        
        
        }

        void check_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chek = (CheckBox)sender;
            if ((bool)chek.IsChecked)
            {
                workspace.selected_spot_area.Add(chek.Name);
            }
            else
            {
                for (int i = 0; i < workspace.selected_spot_area.Count;i++ )
                {
                    if(workspace.selected_spot_area[i]==chek.Name)
                    {
                        workspace.selected_spot_area.RemoveAt(i);
                    }
                }
            
            }
            workspace.re_create_shapes();
        }
    }
}
