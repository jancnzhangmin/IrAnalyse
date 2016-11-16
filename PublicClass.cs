using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Controls;

namespace IrAnalyse
{
    class PublicClass
    {
        public static ArrayList Iron_palette = new ArrayList();
        public static ArrayList medical = new ArrayList();
        public static ArrayList grey = new ArrayList();
        public static ArrayList new_rainbow = new ArrayList();
        public static ArrayList rainbow = new ArrayList();
        public static ArrayList red_brown = new ArrayList();
        public static ArrayList isothermal = new ArrayList();
        public static string cur_ctrl_type;//当前控件类型
        public static int ctrl_name = 0;//控件名称增量
        public static string cur_ctrl_name;//当前控件名称
        public static bool Tempunit = false;//温度单位默认摄氏度
        public static string is_draw_type;//判断形状画图
        public static string is_cur_temp;//判断光标温度
        public static ArrayList shapes_count = new ArrayList();
        public static int repot_ready_index = 0;//报告准备名字增量
        public static int report_name_step = 0;//报告增量
        public static bool ronghe_type = false;//融合按钮标记
        public static string report_type="";
        public static bool open_newisothermal = false;
        public static List<int> list2 = new List<int>();

        public struct shapes_property 
        {
            public string shapes_name { get; set; }//图形名
            public string workspace_name { get; set; }//工作区名
            public int max_temp { get; set; }//最高温
            public int min_temp { get; set; }//最低温
            public double avr_temp { get; set; }//平均温
            public double percent { get; set; }//温度所占最大百分比
            public int pixels_count { get; set; }//温度（像素）个数
            public string shapes_type { get; set; }//图形类型
            public List<double> vertex { get; set; }//图形顶点
            public List<int> pixel_coordinate { get; set; }//像素坐标
            public List<int> unique_temp { get; set; }//去除重复的温度
            public List<double> unique_percent { get; set; }//每温度所占像素比
            public float Emiss { get; set; }//辐射率
            public float TempTamb{ get; set; }//环境温度
            public float TempDist{ get; set; }//距离
            public float dampness { get; set; }//相对湿度
            public float Temprevise { get; set; }//修正温
           
//UInt16 TempDist,UInt16 TempTamb
        }
        public class isothermal_property
        {
            public bool is_checked { get; set; }//是否选中
            public double max_temp { get; set; }//最高温
            public double min_temp { get; set; }//最低温
            public string color { get; set; }//颜色
            public bool is_opacity { get; set; }//是否透明
            public int level { get; set; }//等级

        }
    }
}
