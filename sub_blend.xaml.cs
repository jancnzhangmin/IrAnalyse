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
    /// sub_blend.xaml 的交互逻辑
    /// </summary>
    public partial class sub_blend : UserControl
    {
        public sub_blend()
        {
            InitializeComponent();
        }
        public void get_blend()
        {
            //sub_workspace shap = list.FindName("shapestest") as sub_workspace;

           //sub_workspace work = 
           //work.create_img();
           //blend_canvas.Children.Add(work.irimg);
           sub_workspace work1 = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
           work1.re_create_shapes();
           //Image blend_img = work1.irimg;
           blend_back.Children.Add(work1);


        }
    }
}
