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
using System.Text.RegularExpressions;

namespace IrAnalyse
{
    /// <summary>
    /// sub_bc_adjust.xaml 的交互逻辑（亮度/对比度）
    /// </summary>
    public partial class sub_bc_adjust : UserControl
    {
        public sub_bc_adjust()
        {
            InitializeComponent();
        }
        sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
        private void b_adjust_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
           
            b_textBox.Text =((int)b_adjust.Value).ToString();
            
            
        }

        private void c_adjust_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
             c_textBox.Text =((int)c_adjust.Value).ToString();
            
            
        }

        private void b_textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {

                if (int.Parse(b_textBox.Text) > 100)
                {
                    b_textBox.Text = "100";
                }
                if (int.Parse(b_textBox.Text) < -100)
                {
                    b_textBox.Text = "-100";
                }
                workspace.b_value = int.Parse(b_textBox.Text);
                workspace.last_b_value = int.Parse(b_textBox.Text);
                workspace.create_img();
                workspace.grad_max_min();
                workspace.calculate_temp_ruler();
                workspace.init_isothermal();
            }
            catch { }
        }

        private void c_textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {

                if (int.Parse(c_textBox.Text) > 100)
                {
                    c_textBox.Text = "100";
                }
                if (int.Parse(c_textBox.Text) < -100)
                {
                    c_textBox.Text = "-100";
                }
                workspace.c_value = int.Parse(c_textBox.Text);
                workspace.last_c_value = int.Parse(c_textBox.Text);
                workspace.create_img();
                workspace.grad_max_min();
                workspace.calculate_temp_ruler();
                workspace.init_isothermal();
            }
            catch
            {
            }
        }
        private void read_bc_adjust()
        {
            b_adjust.Value = workspace.b_value;
            b_textBox.Text = workspace.last_b_value.ToString();
            c_adjust.Value = workspace.c_value;
            c_textBox.Text = workspace.last_c_value.ToString();
            if (workspace.is_auto_adjust)
            {
                radioauto.IsChecked = true;
                b_adjust.IsEnabled = false;
                c_adjust.IsEnabled = false;
                b_textBox.IsEnabled = false;
                c_textBox.IsEnabled = false;
                workspace.b_value = 0;
                workspace.c_value = 0;
                b_adjust.Value = int.Parse(b_textBox.Text);
                c_adjust.Value = int.Parse(c_textBox.Text);
            }
            else
            {
                radiomanual.IsChecked = true;
                b_adjust.IsEnabled = true;
                c_adjust.IsEnabled = true;
                b_textBox.IsEnabled = true;
                c_textBox.IsEnabled = true;
            }
            workspace.create_img();
            workspace.grad_max_min();
            workspace.calculate_temp_ruler();
            workspace.init_isothermal();

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            read_bc_adjust();
        }

        private void b_textBox_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                b_adjust.Value = int.Parse(b_textBox.Text);
                workspace.last_b_value = int.Parse(b_textBox.Text);
            }
            catch { }
        }

        private void c_textBox_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                c_adjust.Value = int.Parse(c_textBox.Text);
                workspace.last_c_value = int.Parse(c_textBox.Text);
            }
            catch { }
        }

        private void radioauto_Click(object sender, RoutedEventArgs e)
        {
            b_adjust.IsEnabled = false;
            c_adjust.IsEnabled = false;
            b_textBox.IsEnabled = false;
            c_textBox.IsEnabled = false;
            workspace.b_value = 0;
            workspace.c_value = 0;
            workspace.is_auto_adjust = true;
            workspace.create_img();
            workspace.grad_max_min();
            workspace.calculate_temp_ruler();
            workspace.init_isothermal();
            
            
        }

        private void radiomanual_Click(object sender, RoutedEventArgs e)
        {
            b_adjust.IsEnabled = true;
            c_adjust.IsEnabled = true;
            b_textBox.IsEnabled = true;
            c_textBox.IsEnabled = true;
            workspace.b_value = workspace.last_b_value;
            workspace.c_value = workspace.last_c_value;
            b_textBox.Text = workspace.last_b_value.ToString();
            c_textBox.Text = workspace.last_c_value.ToString();
            b_adjust.Value = workspace.last_b_value;
            c_adjust.Value = workspace.last_c_value;
            workspace.is_auto_adjust = false;
            workspace.create_img();
            workspace.grad_max_min();
            workspace.calculate_temp_ruler();
            workspace.init_isothermal();
        }

        private void bc_textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9.-]+");
            e.Handled = re.IsMatch(e.Text);
        }
        
    }
}
