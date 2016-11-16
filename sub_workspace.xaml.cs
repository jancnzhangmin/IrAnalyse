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
using System.Text.RegularExpressions;
using System.Globalization;
using System.Timers;
using System.Windows.Media.Animation;
using System.ComponentModel;
using System.Threading;

namespace IrAnalyse
{
    /// <summary>
    /// sub_workspace.xaml 的交互逻辑
    /// </summary>
    public partial class sub_workspace : UserControl 
    {
        public sub_workspace()
        {
            InitializeComponent();

             
        }
        public static readonly RoutedEvent WorkMouseUpEvent = EventManager.RegisterRoutedEvent("WorkMouseUp", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(sub_workspace));
        public event RoutedPropertyChangedEventHandler<object> WorkMouseUp
        {
            add { AddHandler(WorkMouseUpEvent, value); }
            remove { RemoveHandler(WorkMouseUpEvent, value); }
        }


        public static readonly RoutedEvent WorkMouseWheelEvent = EventManager.RegisterRoutedEvent("WorkMouseWheel", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(sub_workspace));
        public event RoutedPropertyChangedEventHandler<object> WorkMouseWheel
        {
            add { AddHandler(WorkMouseWheelEvent, value); }
            remove { RemoveHandler(WorkMouseWheelEvent, value); }
        }

        public static readonly RoutedEvent LoadIrInformationEvent = EventManager.RegisterRoutedEvent("LoadIrInformation", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(sub_workspace));
        public event RoutedPropertyChangedEventHandler<object> LoadIrInformation
        {
            add { AddHandler(LoadIrInformationEvent, value); }
            remove { RemoveHandler(LoadIrInformationEvent, value); }
        }





        public string filename { get; set; }//图片名称
        public ArrayList cur_palette { get; set; }//当前调色板
        public bool is_reverse { get; set; }//调色板是否反转[true反转，false正常]area_temp
        public string palette_name { get; set; }//调色板名称
        public int b_value { get; set; }//亮度值
        public int c_value { get; set; }//对比度值
        public bool is_auto_adjust { get; set; }//自动亮度对比度
        public int last_b_value { get; set; }//手动亮度值
        public int last_c_value { get; set; }//手动对比度值
        public float SensorTemp { get; set; }//传感器温度
        public float TiEmiss_H4 { get; set; }
        public float TiEmiss { get; set; }
        public float TiEmiss_H41 { get; set; }
        public float TiEmiss1 { get; set; }
        public float Emiss;//辐射率
        public float dampness;//湿度
        public float temp_def = 0;//温差
        public bool old_TiEmiss_H4 = false;
        public bool old_TiEmiss = false;
        public UInt16 TempDist;
        public UInt16 TempTamb;
        public int modify_temp = 0;
        public List<int> ir_temp { get; set; }//温度值集合
        public ArrayList temp_max_min_avr { get; set; }//[0]最高温[1]最高温坐标[2]最低温[3]最低温坐标[4]平均温
        public ArrayList ir_information { get; set; }//存图片信息
       
        public float zoom_coe { get; set; }//缩放系数
        public ArrayList draw_img { get; set; }//几何图
        public string shapes_name { get; set; }//几何名称
        public string shapes_active { get; set; }//当前选中几何图形
        public PointCollection collection = new PointCollection();//点集合
        public PointCollection collection_ing = new PointCollection();//实时点集合
        public bool is_collection = false;//判断是否正在画线
        public bool doubleclick = false;//是否双击
        public string cur_file_name { get; set; }//打开的图片名称
        public static TemCallParameters_Struct p_TemCallParameters = new TemCallParameters_Struct();
        public List<double> vertex_coordinate = new List<double>();//画图结束（类型，名称，顶点坐标）的顶点坐标
        public int ir_width { get; set; }
        public int ir_height { get; set; }
        public bool is_mousedown = false;//鼠标按下处理事件
        public List<string> histroy_shapes_active = new List<string>();//交集图形
        public int histroy_shapes_active_index = 0;//交集图形选中索引
        public int public_shapes_index = -1;//当前选中图形所在下标
        public int lock_vertex = -1;//鼠标指向的图形顶点
        public List<string> vertex_shapes_name = new List<string>();
        public List<string> selected_spot_area;
        List<int> ir_data = new List<int>();
        ArrayList abs_ir_data = new ArrayList();
        ArrayList AD_table = new ArrayList();
        ArrayList TEM_table = new ArrayList();
        ArrayList DIST_table = new ArrayList();
        public ArrayList histroy_shapes_count = new ArrayList();//撤销图形集合图形
        public List<string> histroy_shapes_operation = new List<string>();//撤销图形操作方式
        public int histroy_shapes_index = -1;//撤销图形下标
        public RenderTargetBitmap repot_image;
        public ArrayList report_shapes = new ArrayList();//报告准备的数据（排除spot和area）
        public string saveurl = "";//保存路径
        public double ronghewidth = 0.0;//融合图片的宽度
        public double rongheheight = 0.0;//融合图片的高度
        public double rongheleft = 0.0;//融合图片的左边
        public double ronghetop = 0.0;//融合图片的上边
        int abs_ir_max;
        int abs_ir_min;
        double bri_avr = 0;
        public double grad_max;
        public double grad_min;
        public double max_temp;
        public double min_temp;
        public double middle_temp;
        public double refer_max_temp;
        public double refer_min_temp;
        public double refer_middle_temp;
        public int contrast_step = -1;
        public int modify_level = 0;


        public int spot_temp_max;//最高温0是全隐藏,1是坐标,2是温度,3是显示
        public int spot_temp_min;//最高温0是全隐藏,1是坐标,2是温度,3是显示
        public int spot_temp_cen;//最高温0是全隐藏,1是坐标,2是温度,3是显示
        public Brush spot_max;//最高温的颜色
        public Brush spot_cen;//光标处的颜色
        public Brush spot_min;//最低温的颜色




  


        public ArrayList isothermal_list = new ArrayList();//等温列表
        public void init_isothermal()//初始化等温列表
        {

            double max_temp = (int)(((float)grad_max / 10)*10)/10.0;

            double min_temp = (int)(((float)grad_min / 10) * 10) / 10.0;

            if (isothermal_list.Count > 0)
            {
                for (int i = 16; i >= 1; i--)
                {
                    PublicClass.isothermal_property newisothermal = (PublicClass.isothermal_property)isothermal_list[16 - i];

                    if (!newisothermal.is_checked)
                    {
                        newisothermal.max_temp =(int)((float)( min_temp + (max_temp - min_temp) / 16 * i)*10)/10.0;
                        if (i == 1)
                        {
                            newisothermal.min_temp =(int)((float)min_temp*10)/10.0;
                        }
                        else
                        {
                            newisothermal.min_temp =(int)((float)(min_temp + (max_temp - min_temp) / 16 * (i - 1))*10)/10.0;
                        }
                        isothermal_list.Add(newisothermal);
                    }

                }
            }
            else
            {
                for (int i = 16; i >= 1; i--)
                {
                    PublicClass.isothermal_property newisothermal = new PublicClass.isothermal_property();
                    newisothermal.is_checked = false;
                    newisothermal.max_temp = (int)((float)(min_temp + (max_temp - min_temp) / 16 * i) * 10) / 10.0;
                    if (i == 1)
                    {
                        newisothermal.min_temp = (int)((float)min_temp * 10) / 10.0;
                    }
                    else
                    {
                        newisothermal.min_temp =(int)((float)(min_temp + (max_temp - min_temp) / 16 * (i - 1))*10)/10.0;
                    }

                    newisothermal.color = cur_palette[(int)((double)((16 - i) / 16.0 * 255))].ToString();
                    newisothermal.is_opacity = false;
                    newisothermal.level = i;
                    isothermal_list.Add(newisothermal);

                }
            }
        }

        public void modify_isothermal_color()//初始化等温列表
        {
            for (int i = 16; i >= 1; i--)
            {
                PublicClass.isothermal_property newisothermal = (PublicClass.isothermal_property)isothermal_list[i-1];
                if (!newisothermal.is_checked)
                {
                    newisothermal.color = cur_palette[(int)((double)((i-1) / 16.0 * 255))].ToString();
                    isothermal_list[i-1] = newisothermal;
                }
                //isothermal_list.Add(newisothermal);

            }
        }

        private void creat_color_lable()
        {
            double max_temp = Math.Round(grad_max, 1) / 10.0;

            double min_temp = Math.Round(grad_min, 1) / 10.0;

            for (int i = 0; i < 16; i++)
            {
                Label deletelable = right_canvas.FindName("colorlable" + i) as Label;
                if (deletelable != null)
                {
                    right_canvas.Children.Remove(deletelable);
                    right_canvas.UnregisterName("colorlable" + i);
                }
            }

            if (isothermal_list.Count > 0)
            {
                for (int i = 0; i < 16; i++)
                {
                    PublicClass.isothermal_property newiso = (PublicClass.isothermal_property)isothermal_list[i];
                    if (newiso.is_checked)
                    {
                        Label colorlable = new Label();
                        colorlable.Name = "colorlable" + i;
                        colorlable.Width = 30;



                        double temp_margin_top = 20;
                        //double temp_height;
                        double temp_margin_bottom = cur_palette_img.Height + 20;



                        if (max_temp == min_temp)
                        {
                            if (newiso.max_temp > max_temp)
                            {
                                temp_margin_top = 20;
                                temp_margin_bottom = 21;
                            }
                            if (newiso.max_temp < max_temp)
                            {
                                temp_margin_top = cur_palette_img.Height + 20;
                                temp_margin_bottom = cur_palette_img.Height + 21;
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
                            temp_margin_top = (max_temp - newiso.max_temp) / (max_temp - min_temp) * cur_palette_img.Height + 20;
                            temp_margin_bottom = (max_temp - newiso.min_temp) / (max_temp - min_temp) * cur_palette_img.Height + 20;

                            if ((max_temp - newiso.max_temp) / (max_temp - min_temp) * cur_palette_img.Height + 20 < 0)
                            {
                                temp_margin_top = 20;
                            }
                            if ((max_temp - newiso.min_temp) / (max_temp - min_temp) * cur_palette_img.Height + 20 <= 0)
                            {
                                temp_margin_bottom = 21;
                            }

                            if ((max_temp - newiso.max_temp) / (max_temp - min_temp) * cur_palette_img.Height + 20 > cur_palette_img.Height + 20)
                            {
                                temp_margin_top = cur_palette_img.Height + 19;
                            }
                            if ((max_temp - newiso.min_temp) / (max_temp - min_temp) * cur_palette_img.Height + 20 > cur_palette_img.Height + 20)
                            {
                                temp_margin_bottom = cur_palette_img.Height + 20;
                            }





                        }
                        if (temp_margin_top < 20)
                        {
                            temp_margin_top = 20;
                            temp_margin_bottom = temp_margin_bottom - (20 - temp_margin_top);
                        }
                        if (temp_margin_top >= temp_margin_bottom)
                        {
                            temp_margin_bottom = temp_margin_top + 1;
                        }
         
                        colorlable.Margin = new Thickness(10, temp_margin_top, 0, 0);
                        colorlable.Height = temp_margin_bottom - temp_margin_top;

                        SolidColorBrush newbrush = new SolidColorBrush();

                        newbrush.Color = (Color)ColorConverter.ConvertFromString("#" + newiso.color);
                        colorlable.Background = newbrush;


                        right_canvas.Children.Add(colorlable);
                        right_canvas.RegisterName(colorlable.Name, colorlable);

                    }
                }
            }
        }


        private void read_ir_data()//读取IR图数据
        {
         
            FileStream fs = new FileStream(filename, FileMode.Open,FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            fs.Seek(0x18, SeekOrigin.Begin);//从18h字开始
            string jpg_size_str = len_2(br.ReadByte());
             jpg_size_str += len_2(br.ReadByte());
             jpg_size_str += len_2(br.ReadByte());
             jpg_size_str += len_2(br.ReadByte());
             int jpg_size = Convert.ToInt32(jpg_size_str, 16);//获取JPG 文件长度

            ///////可见光识别字节//////////////
             //fs.Seek(0x23, SeekOrigin.Begin);//从23h字开始
             //string jpg_str = len_2(br.ReadByte());
             //jpg_str += len_2(br.ReadByte());
             //Visible = jpg_str.Substring(0, 2);
             //ir_pic = jpg_str.Substring(2, 2);


             fs.Seek(jpg_size + 90, SeekOrigin.Begin);
             string ir_width_str = len_2(br.ReadByte());
             ir_width_str = len_2(br.ReadByte()) + ir_width_str;
             ir_width = Convert.ToInt32(ir_width_str,16);//获取IR宽度




             
             string ir_height_str = len_2(br.ReadByte());
             ir_height_str = len_2(br.ReadByte()) + ir_height_str;
             ir_height = Convert.ToInt32(ir_height_str, 16);//获取IR高度

           
                 fs.Seek(jpg_size + 6144, SeekOrigin.Begin);
                 for (int i = 0; i < ir_width * ir_height; i++)//获取AD值
                 {
                     string ir_high = len_2(br.ReadByte());
                     string ir_low = len_2(br.ReadByte());
                     ir_data.Add(Convert.ToInt32(ir_low + ir_high, 16));
                     abs_ir_data.Add(Convert.ToInt32(ir_low + ir_high, 16));
                 }
               

             fs.Seek(jpg_size + 0x500, SeekOrigin.Begin);//获取AD对应表
                 for(int i=0;i<64;i++)
                 {
                     string AD_table_high=len_2(br.ReadByte());
                     string AD_table_low = len_2(br.ReadByte());
                     AD_table.Add(Convert.ToInt32(AD_table_high+AD_table_low,16));

                 }


                 fs.Seek(jpg_size + 0x600, SeekOrigin.Begin);//获取温度对应表
                 for (int i = 0; i < 64; i++)
                 {
                     string TEM_table_high = len_2(br.ReadByte());
                     string TEM_table_low = len_2(br.ReadByte());
                     TEM_table.Add(Convert.ToInt16(TEM_table_high+TEM_table_low, 16));

                 }


                 fs.Seek(jpg_size + 0x700, SeekOrigin.Begin);//获取距离温度修正表
                 for (int i = 0; i < 64; i++)
                 {
                     string DIST_table_high = len_2(br.ReadByte());
                    string DIST_table_low = len_2(br.ReadByte());
                     DIST_table.Add(Convert.ToInt32(DIST_table_high+DIST_table_low, 16));

                 }
                 fs.Seek(jpg_size + 0x100 + 48, SeekOrigin.Begin);
                 string SensorTemp_high = len_2(br.ReadByte());
                 string SensorTemp_low = len_2(br.ReadByte());
                 SensorTemp = (float)(Convert.ToInt32(SensorTemp_high + SensorTemp_low,16));//获取传感器温度



                 fs.Seek(jpg_size + 0x100 + 95 + 9 +  12, SeekOrigin.Begin);
                 byte[] intBuffer = new byte[4];
                 string data="";
                 int data10 ;
                 string data2;
                 for (int i = 0; i < 4; i++)
                 {
                     data10 = br.ReadByte();
                     data2 = Convert.ToString(data10,2);
                     data += len_8(data2);
                 }
                 string s = data.Substring(0, 1);//取出符号位
                 string e = data.Substring(1,8);//取出阶数
                 string m = data.Substring(9,23);//取出尾数
                 int e10 = Convert.ToInt32(e,2);
                 int m10 = Convert.ToInt32(m,2);
                 TiEmiss_H4 = (float)((1 + m10 * Math.Pow(2, -23)) * Math.Pow(2, e10 - 127));
                 if (s == "1")
                 {
                     TiEmiss_H4 = -1 * TiEmiss_H4;
                 }


                 data = "";
                 for (int i = 0; i < 4; i++)
                 {
                     data10 = br.ReadByte();
                     data2 = Convert.ToString(data10, 2);
                     data += len_8(data2);
                 }
                
                  s = data.Substring(0, 1);//取出符号位
                  e = data.Substring(1, 8);//取出阶数
                  m = data.Substring(9, 23);//取出尾数
                  e10 = Convert.ToInt32(e, 2);
                  m10 = Convert.ToInt32(m, 2);
                 TiEmiss = (float)((1 + m10 * Math.Pow(2, -23)) * Math.Pow(2, e10 - 127));
                 if (s == "1")
                 {
                     TiEmiss = -1 * TiEmiss;
                 }


                 ////生产商字串ir_information[0]////
                 ir_information = new ArrayList();
                
                 ir_information.Add("");
                 fs.Seek(jpg_size + 0x6, SeekOrigin.Begin);
            for(int i=0;i<16;i++)
            {
                int asc_code = int.Parse(br.ReadByte().ToString());
                if (asc_code > 0)
                {
                    ir_information[0] += ((char)asc_code).ToString();
                   
                }
                else
                {
                    break;
                }
            }


            ///机器序列号字串ir_information[1]/////
            ir_information.Add("");
            fs.Seek(jpg_size + 0x16, SeekOrigin.Begin);
            for (int i = 0; i < 16; i++)
            {
                int asc_code = int.Parse(br.ReadByte().ToString());
                if (asc_code > 0)
                {
                    ir_information[1] += ((char)asc_code).ToString();
                    
                }
                else
                {
                    break;
                }
            }


            ///机器生产日期字串 ir_information[2]////
            ir_information.Add("");
            fs.Seek(jpg_size + 0x26, SeekOrigin.Begin);
            for (int i = 0; i < 16; i++)
            {
                int asc_code = int.Parse(br.ReadByte().ToString());
                if (asc_code > 0)
                {
                    ir_information[2] += ((char)asc_code).ToString();
                    
                }
                else
                {
                    break;
                }
            }

            ////图片存储时间ir_information[3]////
            ir_information.Add("");
            fs.Seek(jpg_size + 0x46, SeekOrigin.Begin);
            for (int i = 0; i < 6; i++)
            {
                    ir_information[3] +=len_2( br.ReadByte());
                   
            }

            ///////镜头号ir_information[4]/////
            ir_information.Add("");
            fs.Seek(jpg_size + 0x56, SeekOrigin.Begin);
            ir_information[4] += int.Parse( len_2(br.ReadByte()) + len_2(br.ReadByte())).ToString();
           
            ///////测温档,ir_information[5]/////
            ir_information.Add("");
            fs.Seek(jpg_size + 0x58, SeekOrigin.Begin);
            ir_information[5] += int.Parse(len_2(br.ReadByte()) + len_2(br.ReadByte())).ToString();
           
            //////图象宽度ir_information[6]/////
            ir_information.Add("");
            fs.Seek(jpg_size + 0x5a, SeekOrigin.Begin);
            ir_information[6] += len_2(br.ReadByte());
            ir_information[6] = len_2(br.ReadByte()) + ir_information[6];
            ir_information[6] = Convert.ToInt32(ir_information[6].ToString(), 16).ToString();
           
            //////图象高度ir_information[7]/////
            ir_information.Add("");
            fs.Seek(jpg_size + 0x5c, SeekOrigin.Begin);
            ir_information[7] += len_2(br.ReadByte());
            ir_information[7] = len_2(br.ReadByte()) + ir_information[7];
            ir_information[7] = Convert.ToInt32(ir_information[7].ToString(), 16).ToString();
           
            /////CCD图象宽度 ir_information[8]///////
            ir_information.Add("");
            fs.Seek(jpg_size + 0x6a, SeekOrigin.Begin);
            ir_information[8] += len_2(br.ReadByte());
            ir_information[8] = len_2(br.ReadByte()) + ir_information[8];
            ir_information[8] = Convert.ToInt32(ir_information[8].ToString(), 16).ToString();
            
            /////CCD图象高度 ir_information[9]///////
            ir_information.Add("");
            fs.Seek(jpg_size + 0x6c, SeekOrigin.Begin);
            ir_information[9] += len_2(br.ReadByte());
            ir_information[9] = len_2(br.ReadByte()) + ir_information[9];
            ir_information[9] = Convert.ToInt32(ir_information[9].ToString(), 16).ToString();
            

            /////环境温度 ir_information[10]/////
            ir_information.Add("");
            fs.Seek(jpg_size + 0x100+32, SeekOrigin.Begin);
            ir_information[10] += (len_2(br.ReadByte()) + len_2(br.ReadByte())).ToString();
            ir_information[10] = (Convert.ToInt32(ir_information[10].ToString(), 16) - 600).ToString();
            

            //////相对湿度 ir_information[11]//////
            ir_information.Add("");
            fs.Seek(jpg_size + 0x100 + 34, SeekOrigin.Begin);
            ir_information[11] +=  Convert.ToInt32(len_2(br.ReadByte()),16).ToString();
            
            ///////level值 ir_information[12]//////
            ir_information.Add("");
            fs.Seek(jpg_size + 0x100+42, SeekOrigin.Begin);
            ir_information[12] += Convert.ToInt32(len_2(br.ReadByte()) + len_2(br.ReadByte()),16).ToString();
            
            ///////Width值 ir_information[13]//////
            ir_information.Add("");
            fs.Seek(jpg_size + 0x100 + 44, SeekOrigin.Begin);
            ir_information[13] += Convert.ToInt32(len_2(br.ReadByte()) + len_2(br.ReadByte()), 16).ToString();
            
            ///////传感器温度ir_information[14]///////
            ir_information.Add("");
            fs.Seek(jpg_size + 0x100 + 48, SeekOrigin.Begin);
            ir_information[14] += len_2(br.ReadByte()) + len_2(br.ReadByte());
            
            ///////传感器温度修正值ir_information[15]///////
            ir_information.Add("");
            fs.Seek(jpg_size + 0x100 + 50, SeekOrigin.Begin);
           // ir_information[15] +=  Convert.ToInt32(len_2(br.ReadByte()) + len_2(br.ReadByte()),16).ToString();
            
            ir_information[15] += modify_temp.ToString();
            //////辐射率 ir_information[16]////
            ir_information.Add("");
            fs.Seek(jpg_size + 0x100 + 95+9+9, SeekOrigin.Begin);
            ir_information[16] +=Convert.ToInt32( len_2(br.ReadByte()),16).ToString();
            
            /////////距离n ir_information[17]//////
            ir_information.Add("");
            fs.Seek(jpg_size + 0x100 + 95+9+10, SeekOrigin.Begin);
            ir_information[17] += Convert.ToInt32( len_2(br.ReadByte()) + len_2(br.ReadByte()),16).ToString();
            

            // int ir_left = 0;
            // int ir_top = 0;
            // int ir_right = ir_width;
            // int ir_bottom = ir_height;

            //for(int x=0;x< 9;x++)//判断IR数据左边有效列
            //{
            //    ArrayList err_count = new ArrayList();
            //    int ir_rpt = 0;
            //    for (int y = 9; y < ir_height-10; y++)
            //    {
            //        foreach (int t in err_count)
            //        {
            //            if(t == int.Parse(ir_data[y * ir_width + x].ToString()))
            //            {
            //                ir_rpt = 1;
            //            }
            //        }
            //        if (ir_rpt == 0)
            //        {
            //            err_count.Add(int.Parse(ir_data[y * ir_width + x].ToString()));
            //        }
            //    }
            //    if (err_count.Count < 3)
            //    {
            //        ir_left = x;

            //    }
            //    else
            //    {
            //        break;
            //    }
            //}

            //for (int y = 0; y < 9; y++)//判断IR数据上边有效行
            //{
            //    ArrayList err_count = new ArrayList();
            //    int ir_rpt = 0;
            //    for (int x = 9; x < ir_width - 10; x++)
            //    {
            //        foreach (int t in err_count)
            //        {
            //            if (t == int.Parse(ir_data[y * ir_width + x].ToString()))
            //            {
            //                ir_rpt = 1;
            //            }
            //        }
            //        if (ir_rpt == 0)
            //        {
            //            err_count.Add(int.Parse(ir_data[y * ir_width + x].ToString()));
            //        }

            //    }
            //    if (err_count.Count < 3)
            //    {
            //        ir_top = y;

            //    }
            //    else
            //    {
            //        break;
            //    }
 
            //}

            //for (int x = ir_width-1; x > ir_width-9; x--)//判断IR数据右边有效列
            //{
            //    ArrayList err_count = new ArrayList();
            //    int ir_rpt = 0;
            //    for (int y = 9; y < ir_height - 10; y++)
            //    {
            //        foreach (int t in err_count)
            //        {
            //            if (t == int.Parse(ir_data[y * ir_width + x].ToString()))
            //            {
            //                ir_rpt = 1;
            //            }
            //        }
            //        if (ir_rpt == 0)
            //        {
            //            err_count.Add(int.Parse(ir_data[y * ir_width + x].ToString()));
            //        }

            //    }
            //    if (err_count.Count < 3)
            //    {
            //        ir_right = x;

            //    }
            //    else
            //    {
            //        break;
            //    }
   
            //}

            //for (int y = ir_height - 1; y > ir_height - 9; y--)//判断IR数据下边有效行
            //{
            //    ArrayList err_count = new ArrayList();
            //    int ir_rpt = 0;
            //    for (int x = 9; x < ir_width - 10; x++)
            //    {
            //        foreach (int t in err_count)
            //        {
            //            if (t == int.Parse(ir_data[y * ir_width + x].ToString()))
            //            {
            //                ir_rpt = 1;
            //            }
            //        }
            //        if (ir_rpt == 0)
            //        {
            //            err_count.Add(int.Parse(ir_data[y * ir_width + x].ToString()));
            //        }

            //    }
            //    if (err_count.Count < 3)
            //    {
            //        ir_bottom = y;

            //    }
            //    else
            //    {
            //        break;
            //    }
   
   
            //}



























            
             int ir_max = 0;
             int ir_min = 1000000000;
             long ir_avr = 0;
             long ir_sum = 0;

             //for (int i = 0; i < ir_data.Count; i++)//计算IR数据的最大、最小
             //{
             //    if (int.Parse(ir_data[i].ToString()) > ir_max)
             //    {
             //        ir_max = int.Parse(ir_data[i].ToString());
             //    }
             //    if (int.Parse(ir_data[i].ToString()) < ir_min && int.Parse(ir_data[i].ToString())!=0)
             //    {
             //        ir_min = int.Parse(ir_data[i].ToString());
             //    }
             //}

             //for (int i = 0; i < ir_data.Count; i++)//丢弃AD为0的值
             //{
             //    if (int.Parse(ir_data[i].ToString()) < ir_min)
             //    {
             //        ir_data[i] = ir_min;
             //    }
             //}

             //for (int y = ir_top; y < ir_bottom; y++)//求AD最大最小值
             //{
             //    for (int x = ir_left; x < ir_right; x++)
             //    {
             //        if (int.Parse(ir_data[y*(ir_right-ir_left+1)+x].ToString()) > ir_max)
             //        {
             //            ir_max = int.Parse(ir_data[y*(ir_right-ir_left+1)+x].ToString());
             //        }
             //        if (int.Parse(ir_data[y * (ir_right - ir_left + 1) + x].ToString()) < ir_min && int.Parse(ir_data[y * (ir_right - ir_left + 1) + x].ToString()) > 0)
             //        {
             //            ir_min = int.Parse(ir_data[y * (ir_right - ir_left + 1) + x].ToString());
             //        }
             //    }
             //}

             //for (int y = ir_top; y < ir_bottom; y++)//丢弃为0值
             //{
             //    for (int x = ir_left; x < ir_right; x++)
             //    {
             //        if (int.Parse(ir_data[y * (ir_right - ir_left + 1) + x].ToString()) <= 0)
             //        {
             //            ir_data[y * (ir_right - ir_left + 1) + x] = ir_min;
             //        }
             //        ir_sum += int.Parse(ir_data[y * (ir_right - ir_left + 1) + x].ToString());
             //        ir_count++;
             //    }
             //}

             //ir_avr = ir_sum / ir_count;//平均值




             //for (int i = 0; i < ir_data.Count; i++)//求平均值
             //{
             //    ir_count += int.Parse(ir_data[i].ToString());
             //}
             //ir_avr = ir_count / Int64.Parse(ir_data.Count.ToString());


             abs_ir_max = ir_max;
             abs_ir_min = ir_min;

            /////2015-7-1最大最小值///////
             List<int> ir_data_sort = new List<int>();
             for (int i = 0; i < ir_data.Count; i++)
             {
                 if ((int)ir_data[i] > 0 && (int)ir_data[i]<65535)
                 {
                     ir_data_sort.Add((int)ir_data[i]);
                     ir_sum += (int)ir_data[i];
                 }
             }
             ir_avr = ir_sum / ir_data_sort.Count;
             ir_data_sort.Sort();
             abs_ir_min = ir_data_sort[0];
             abs_ir_max = ir_data_sort.Last();
             for (int i = 0; i < ir_data.Count; i++)
             {
                 if ((int)ir_data[i] < abs_ir_min)
                 {
                     ir_data[i] = abs_ir_min;
                 }
                 if ((int)ir_data[i] > abs_ir_max)
                 {
                     ir_data[i] = abs_ir_max;
                 }
             }




             ir_data_sort.Clear();
             for (int y = 16; y < ir_height - 15; y++)
             {
                 for (int x = 16; x < ir_width - 15; x++)
                 {
                     ir_data_sort.Add(ir_data[y * ir_width + x]);
                 }
             }
             abs_ir_max = (from c in ir_data_sort select c).Max();
             abs_ir_min = (from c in ir_data_sort select c).Min();
             ir_avr = (abs_ir_max + abs_ir_min) / 2;


             for (int i = 0; i < ir_data.Count; i++)
             {
                 if (ir_data[i] > abs_ir_max)
                 {
                     ir_data[i] = abs_ir_max;
                 }
                 else
                     if (ir_data[i] < abs_ir_min)
                     {
                         ir_data[i] = abs_ir_min;
                     }
             }


             ////2015-7-1最大最小值end////

             //if (((abs_ir_max - ir_avr) > (ir_avr - abs_ir_min)))
             //{
             //    abs_ir_max = ((int)ir_avr - abs_ir_min) + (int)ir_avr;
             //}
             //else
             //{
             //    abs_ir_min = (int)ir_avr - (abs_ir_max - (int)ir_avr);
             //}
             //for (int i = 0; i < ir_data.Count; i++)//将最大值和最小值压至安全区间
             //{


             //    if (int.Parse(ir_data[i].ToString()) > abs_ir_max)
             //    {
             //        ir_data[i] = abs_ir_max;
             //    }
             //    else
             //        if (int.Parse(ir_data[i].ToString()) < abs_ir_min)
             //        {
             //            ir_data[i] = abs_ir_min;
             //        }

             //}

             double bri_sum = 0;
             double bri_count = 0;
             
             for (int y = 0; y < ir_height; y++)
             {
                 for (int x = 0; x < ir_width; x++)
                 {
                     double color_tmp1 = ((int)ir_data[y * ir_width + x] - abs_ir_min);
                     double color_tmp2 = abs_ir_max - abs_ir_min;
                     double color_tmp3 = color_tmp1 / color_tmp2;
                     bri_sum += color_tmp3 * 255.0;
                     bri_count++;

                 }
             }
             bri_avr = bri_sum / (double)bri_count;





             br.Close();
             fs.Close();

             


        }


        private string len_8(string data)//二进制补足8位
        {
            for (int i = data.Length; i < 8; i++)
            {
                data = "0" + data;
            }
            return data;
        }


        public void zoom()//缩放系数
        {
            if (zoom_coe > 5)
            {
                zoom_coe = 5;
            }
            if (zoom_coe < 0.3)
            {
                zoom_coe = 0.3f;
            }
            ir_canvas_base.Width = ir_width * zoom_coe;
            ir_canvas_base.Height = ir_height * zoom_coe;
            ir_canvas_font.Width = ir_width * zoom_coe;
            ir_canvas_font.Height = ir_height * zoom_coe; 
            irimg.Width = ir_width * zoom_coe;
            irimg.Height = ir_height * zoom_coe;
            back_canvas.Width = irimg.Width;
            back_canvas.Height = irimg.Height;
            ir_back_img.Width = ronghewidth * zoom_coe;
            ir_back_img.Height =rongheheight * zoom_coe;
            ir_back_img.Margin = new Thickness((irimg.Width - ir_back_img.Width) / 2, (irimg.Height - ir_back_img.Height) / 2, 0, 0);
            cur_palette_img.Height = ir_height * zoom_coe - 40;
            right_panel.Margin = new Thickness(ir_canvas_base.Width, 0, 0, 0);
            ir_canvas.Width = ir_width * zoom_coe+100;
            ir_canvas.Height = ir_height * zoom_coe;
            re_create_shapes();
            calculate_temp_ruler();
            creat_color_lable();
        }

        List<int> fillisopixel = new List<int>();
        public void filliso()
        {
            int list2index = 0;
            fillisopixel.Clear();
            for (int y = 0; y < ir_height; y++)
            {
                //pixels[y] = new Color[ir_width];
                for (int x = 0; x < ir_width; x++)
                {
                    if ( y * ir_width + x == PublicClass.list2[list2index])
                    {
                        fillisopixel.Add(1);
                        if (list2index < PublicClass.list2.Count-1)
                        {
                            list2index++;
                        }
                        //if (PublicClass.list2.Count > 0)
                        //{
                            //PublicClass.list2.RemoveAt(0);
                        //}
                    }
                    else
                    {
                        fillisopixel.Add(0);
                    }

                }
            }
        }
        
        public void create_img() //生成图像
        {
            //if (PublicClass.list2.Count > 0)
            //{
            //    filliso();
            //}
            //int[] pixels = new int[1];
            //int stride = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
            
            int stride = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;

            WriteableBitmap ir_bmp = new WriteableBitmap(ir_width, ir_height, 96, 96, PixelFormats.Bgr32, null);
            int[] pixels=new int[ir_width*ir_height];
            //PublicClass.Iron_palette.Reverse();
            ir_bmp.Lock();


            ArrayList temp_palette = new ArrayList();
            temp_palette = (ArrayList)cur_palette.Clone();



            for (int i = 0; i < contrast_step; i++)
            {
                int x = 128 * i / contrast_step;
                int x1 = 128 * (i + 1) / (contrast_step);
                for (int j = x; j < x1; j++)
                {
                    temp_palette[j] = cur_palette[i];
                    temp_palette[255-j] = cur_palette[255-i];
                }
            }







                //pixels = new Color[ir_height][];
                for (int y = 0; y < ir_height; y++)
                {
                    //pixels[y] = new Color[ir_width];
                    for (int x = 0; x < ir_width; x++)
                    {
                        double color_tmp1 = ((int)ir_data[y * ir_width + x] - abs_ir_min);
                        double color_tmp2 = abs_ir_max - abs_ir_min;
                        double color_tmp3 = color_tmp1 / color_tmp2;


                        color_tmp1 = ir_temp[y * ir_width + x] - min_temp + (middle_temp - refer_middle_temp);
                        color_tmp2 = refer_max_temp - refer_min_temp;
                        color_tmp3 = color_tmp1 / color_tmp2;




                        //double color_value = color_tmp3 * 255 * (255 - bri_avr) / 128;
                        double color_value = color_tmp3 * 255;
                        //double color_value = color_tmp3 * 255 * 128/bri_avr;
                        //color_value = color_value + 128 - bri_avr;
                        //color_value = color_value * (255.0 / (255.0 + 128.0 - bri_avr));

                        //double chazhi = bri_avr - 128;//差值
                        //double max_color = 255 + chazhi;//最大值
                        //double xishu = 255 / max_color;//系数
                        //double min_xishu = xishu / (max_color*max_color);//最小系数
                        //color_value = (color_value * color_value*color_value * min_xishu);
                        //color_value = (color_value * ((double)c_value * 0.009 + 1) + (double)b_value * 2);


                        if (color_value < 0)
                        {
                            color_value = 0;
                        }
                        if (color_value > 255)
                        {
                            color_value = 255;
                        }






                        int pixel = Convert.ToInt32(cur_palette[(int)color_value].ToString(), 16);
                        //pixels[0] = pixel;

                        //byte[] color_rgb = new byte[3];
                        //color_rgb[0] = Convert.ToByte(cur_palette[(int)color_value].ToString().Substring(0, 2));
                        //color_rgb[1] = Convert.ToByte(cur_palette[(int)color_value].ToString().Substring(2, 2));
                        //color_rgb[2] = Convert.ToByte(cur_palette[(int)color_value].ToString().Substring(4, 2));
                        pixels[y * ir_width + x] = Convert.ToInt32(temp_palette[(int)color_value].ToString(), 16);

                        if (isothermal_list.Count > 0)
                        {
                            for (int i = 0; i < 16; i++)
                            {
                                PublicClass.isothermal_property newiso = (PublicClass.isothermal_property)isothermal_list[i];
                                if (newiso.is_checked)
                                {
                                    if ((double)ir_temp[y * ir_width + x] / 10 > newiso.min_temp && (double)ir_temp[y * ir_width + x] / 10 <= newiso.max_temp && fillisopixel[y * ir_width + x]==1)
                                    {


                                        if (newiso.is_opacity)
                                        {

                                            //Y'= 0.299*R' + 0.587*G' + 0.114*B'
                                            //U'= -0.147*R' - 0.289*G' + 0.436*B' = 0.492*(B'- Y')
                                            //V'= 0.615*R' - 0.515*G' - 0.100*B' = 0.877*(R'- Y')
                                            //R' = Y' + 1.140*V'
                                            //G' = Y' - 0.394*U' - 0.581*V'
                                            //B' = Y' + 2.032*U'
                                            double cur_temp = (double)ir_temp[y * ir_width + x] / 10;

                                            double R = Convert.ToInt32(newiso.color.Substring(0, 2), 16);
                                            double G = Convert.ToInt32(newiso.color.Substring(2, 2), 16);
                                            double B = Convert.ToInt32(newiso.color.Substring(4, 2), 16);

                                            double Y = 0.299 * R + 0.587 * G + 0.114 * B;
                                            double U = 0.492 * (B - Y);
                                            double V = 0.877 * (R - Y);

                                            Y = (1 - (newiso.max_temp - cur_temp) / (newiso.max_temp - newiso.min_temp)) * (255 - Y) + Y;

                                            R = Y + 1.14 * V;
                                            G = Y - 0.394 * U - 0.581 * V;
                                            B = Y + 2.032 * U;

                                            if (R > 255)
                                            {
                                                R = 255;
                                            }
                                            if (R < 0)
                                            {
                                                R = 0;
                                            }
                                            if (G > 255)
                                            {
                                                G = 255;
                                            }
                                            if (G < 0)
                                            {
                                                G = 0;
                                            }
                                            if (B > 255)
                                            {
                                                B = 255;
                                            }
                                            if (B < 0)
                                            {
                                                B = 0;
                                            }

                                            string R_str = Convert.ToString((int)R, 16).ToString();
                                            if (R_str.Length == 1)
                                            {
                                                R_str = "0" + R_str;
                                            }
                                            string G_str = Convert.ToString((int)G, 16).ToString();
                                            if (G_str.Length == 1)
                                            {
                                                G_str = "0" + G_str;
                                            }
                                            string B_str = Convert.ToString((int)B, 16).ToString();
                                            if (B_str.Length == 1)
                                            {
                                                B_str = "0" + B_str;
                                            }

                                            pixels[y * ir_width + x] = Convert.ToInt32(R_str + G_str + B_str, 16);

                                        }
                                        else
                                        {
                                            //pixels[y * ir_width + x] = Convert.ToInt32(newiso.color, 16);

                                            if (fillisopixel[y * ir_width + x] == 1)
                                            {
                                                pixels[y * ir_width + x] = Convert.ToInt32(newiso.color, 16);
                                            }
                                            

                                            //int a = pixels[y * ir_width + x];
                                            //int b = Convert.ToInt32(newiso.color, 16);


                                            //for (int j = 0; j < PublicClass.shapes_count.Count; j++)
                                            //{
                                            //    PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[j];
                                            //    if (newshapes.shapes_name == "C1")
                                            //    {
                                            //        int[] xiangshu = new int[ir_width * ir_height];
                                            //        pixels[y * ir_width + x] = Convert.ToInt32(newiso.color, 16);
                                            //    }

                                            //}


                                        }




                                    }
                                }
                            }
                        }

                        //ir_bmp.WritePixels(new Int32Rect(x, y, 1, 1), pixels, stride, 0);


                    }

                }
            ir_bmp.WritePixels(new Int32Rect(0, 0, ir_width, ir_height), pixels, stride*ir_width, 0);
            ir_bmp.Unlock();
           irimg.Source = ir_bmp;
           creat_color_lable();





      
           p_TemCallParameters.p_APT0AD = (ArrayList)AD_table.Clone();
           p_TemCallParameters.p_APT0TEMP = (ArrayList)TEM_table.Clone();
           p_TemCallParameters.p_DistTempTable = (ArrayList)DIST_table.Clone();
           p_TemCallParameters.SensorTemp =(Int16)SensorTemp;
           p_TemCallParameters.in_tempAD = 0x7fe4;
           p_TemCallParameters.TiEmiss_H4 = TiEmiss_H4 ;
           p_TemCallParameters.TiEmiss =TiEmiss;
           p_TemCallParameters.ModifyTemp = 1000;
           p_TemCallParameters.Tempunit = false;
           
           
           

            ////////////////测试/////////////////

           //p_TemCallParameters.p_APT0AD = new ArrayList();
           //p_TemCallParameters.p_APT0AD.Add(0xD200);
           //p_TemCallParameters.p_APT0AD.Add(0x7AD0);
           //p_TemCallParameters.p_APT0AD.Add(0x0000);
           //p_TemCallParameters.p_APT0AD.Add(0x2390);
           //p_TemCallParameters.p_APT0AD.Add(0x5000);
           //p_TemCallParameters.p_APT0AD.Add(0x7390);
           //p_TemCallParameters.p_APT0AD.Add(0x7570);
           //p_TemCallParameters.p_APT0AD.Add(0x7750);
           //p_TemCallParameters.p_APT0AD.Add(0x7930);
           //p_TemCallParameters.p_APT0AD.Add(0x77D8);
           //p_TemCallParameters.p_APT0AD.Add(0x7928);
           //p_TemCallParameters.p_APT0AD.Add(0x7A78);
           //p_TemCallParameters.p_APT0AD.Add(0x7BC8);
           //p_TemCallParameters.p_APT0AD.Add(0x7E88);
           //p_TemCallParameters.p_APT0AD.Add(0x7F39);
           //p_TemCallParameters.p_APT0AD.Add(0x8000);
           //p_TemCallParameters.p_APT0AD.Add(0x80D8);
           //p_TemCallParameters.p_APT0AD.Add(0x81C2);
           //p_TemCallParameters.p_APT0AD.Add(0x82C3);
           //p_TemCallParameters.p_APT0AD.Add(0x83E0);
           //p_TemCallParameters.p_APT0AD.Add(0x84FC);
           //p_TemCallParameters.p_APT0AD.Add(0x85F2);
           //p_TemCallParameters.p_APT0AD.Add(0x87A9);
           //p_TemCallParameters.p_APT0AD.Add(0x8A88);
           //p_TemCallParameters.p_APT0AD.Add(0x8DC1);
           //p_TemCallParameters.p_APT0AD.Add(0x914A);
           //p_TemCallParameters.p_APT0AD.Add(0x9360);
           //p_TemCallParameters.p_APT0AD.Add(0x9753);
           //p_TemCallParameters.p_APT0AD.Add(0xA230);
           //p_TemCallParameters.p_APT0AD.Add(0xAAD8);

           //p_TemCallParameters.p_APT0TEMP = new ArrayList();
           //p_TemCallParameters.p_APT0TEMP.Add(300);
           //p_TemCallParameters.p_APT0TEMP.Add(28);
           //p_TemCallParameters.p_APT0TEMP.Add(2730);
           //p_TemCallParameters.p_APT0TEMP.Add(2000);
           //p_TemCallParameters.p_APT0TEMP.Add(1500);
           //p_TemCallParameters.p_APT0TEMP.Add(1000);
           //p_TemCallParameters.p_APT0TEMP.Add(800);
           //p_TemCallParameters.p_APT0TEMP.Add(600);
           //p_TemCallParameters.p_APT0TEMP.Add(400);
           //p_TemCallParameters.p_APT0TEMP.Add(300);
           //p_TemCallParameters.p_APT0TEMP.Add(200);
           //p_TemCallParameters.p_APT0TEMP.Add(100);
           //p_TemCallParameters.p_APT0TEMP.Add(0);
           //p_TemCallParameters.p_APT0TEMP.Add(100);
           //p_TemCallParameters.p_APT0TEMP.Add(200);
           //p_TemCallParameters.p_APT0TEMP.Add(300);
           //p_TemCallParameters.p_APT0TEMP.Add(400);
           //p_TemCallParameters.p_APT0TEMP.Add(500);
           //p_TemCallParameters.p_APT0TEMP.Add(600);
           //p_TemCallParameters.p_APT0TEMP.Add(700);
           //p_TemCallParameters.p_APT0TEMP.Add(800);
           //p_TemCallParameters.p_APT0TEMP.Add(900);
           //p_TemCallParameters.p_APT0TEMP.Add(1000);
           //p_TemCallParameters.p_APT0TEMP.Add(1200);
           //p_TemCallParameters.p_APT0TEMP.Add(1400);
           //p_TemCallParameters.p_APT0TEMP.Add(1600);
           //p_TemCallParameters.p_APT0TEMP.Add(1800);
           //p_TemCallParameters.p_APT0TEMP.Add(2000);
           //p_TemCallParameters.p_APT0TEMP.Add(2500);
           //p_TemCallParameters.p_APT0TEMP.Add(3000);

           //p_TemCallParameters.p_DistTempTable = new ArrayList();
           //p_TemCallParameters.p_DistTempTable.Add(300);
           //p_TemCallParameters.p_DistTempTable.Add(28);
           //p_TemCallParameters.p_DistTempTable.Add(1000);
           //p_TemCallParameters.p_DistTempTable.Add(1000);
           //p_TemCallParameters.p_DistTempTable.Add(1000);
           //p_TemCallParameters.p_DistTempTable.Add(1000);
           //p_TemCallParameters.p_DistTempTable.Add(1000);
           //p_TemCallParameters.p_DistTempTable.Add(1000);
           //p_TemCallParameters.p_DistTempTable.Add(1000);
           //p_TemCallParameters.p_DistTempTable.Add(1000);
           //p_TemCallParameters.p_DistTempTable.Add(1000);
           //p_TemCallParameters.p_DistTempTable.Add(1000);
           //p_TemCallParameters.p_DistTempTable.Add(1000);
           //p_TemCallParameters.p_DistTempTable.Add(1000);
           //p_TemCallParameters.p_DistTempTable.Add(1000);
           //p_TemCallParameters.p_DistTempTable.Add(1028);
           //p_TemCallParameters.p_DistTempTable.Add(1033);
           //p_TemCallParameters.p_DistTempTable.Add(1037);
           //p_TemCallParameters.p_DistTempTable.Add(1038);
           //p_TemCallParameters.p_DistTempTable.Add(1038);
           //p_TemCallParameters.p_DistTempTable.Add(1038);
           //p_TemCallParameters.p_DistTempTable.Add(1038);
           //p_TemCallParameters.p_DistTempTable.Add(1038);
           //p_TemCallParameters.p_DistTempTable.Add(1038);
           //p_TemCallParameters.p_DistTempTable.Add(1038);
           //p_TemCallParameters.p_DistTempTable.Add(1038);
           //p_TemCallParameters.p_DistTempTable.Add(1038);
           //p_TemCallParameters.p_DistTempTable.Add(1038);
           //p_TemCallParameters.p_DistTempTable.Add(1038);
           //p_TemCallParameters.p_DistTempTable.Add(1038);

           //p_TemCallParameters.in_tempAD = 0x8000;
           //p_TemCallParameters.SensorTemp = 0;
           //p_TemCallParameters.TiEmiss_H4 = 1;
           //p_TemCallParameters.TiEmiss = 1;
           //p_TemCallParameters.Tempunit = false;
           //p_TemCallParameters.ModifyTemp = 1800;

            ///////////////测度结束/////////////


          TemCall_Main();
          //grad_max_min();
          //calculate_temp_ruler();

        }


        public void grad_max_min()
        {
            if (ir_temp != null)
            {
                double b_coe;
                double max_temp = (from c in ir_temp select c).Max();
                double min_temp = (from c in ir_temp select c).Min();
                //b_coe = (c_value + 0.0) / 100.0 * (max_temp - min_temp) / 2.0;
                b_coe = (-c_value + 0.0) / 100.0 * (max_temp - min_temp) / 2.0;
                grad_max = max_temp - b_coe;
                grad_min = min_temp + b_coe;

                double c_coe;
                c_coe = (-(b_value + 100.0) / 200.0 * (max_temp - min_temp) + (max_temp - min_temp)) - (max_temp - min_temp) / 2.0;
                grad_max += c_coe;
                grad_min += c_coe;
            }

        }


        public void create_img_back() //生成背景图像
        {
            //int[] pixels = new int[1];
            //int stride = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
            int stride = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;

            WriteableBitmap ir_bmp = new WriteableBitmap(ir_width, ir_height, 96, 96, PixelFormats.Bgr32, null);
            int[] pixels = new int[ir_width * ir_height];
            //PublicClass.Iron_palette.Reverse();
            ir_bmp.Lock();
            //pixels = new Color[ir_height][];
            for (int y = 0; y < ir_height; y++)
            {
                //pixels[y] = new Color[ir_width];
                for (int x = 0; x < ir_width; x++)
                {
                    double color_tmp1 = ((int)ir_data[y * ir_width + x] - abs_ir_min);
                    double color_tmp2 = abs_ir_max - abs_ir_min;
                    double color_tmp3 = color_tmp1 / color_tmp2;
                    double color_value = color_tmp3 * 255 * (255 - bri_avr) / 128;
                    //double color_value = color_tmp3 * 255 * 128/bri_avr;
                    //color_value = color_value + 128 - bri_avr;
                    //color_value = color_value * (255.0 / (255.0 + 128.0 - bri_avr));

                    //double chazhi = bri_avr - 128;//差值
                    //double max_color = 255 + chazhi;//最大值
                    //double xishu = 255 / max_color;//系数
                    //double min_xishu = xishu / (max_color*max_color);//最小系数
                    //color_value = (color_value * color_value*color_value * min_xishu);
                    color_value = (color_value * ((double)c_value * 0.009 + 1) + (double)b_value * 2);


                    if (color_value < 0)
                    {
                        color_value = 0;
                    }
                    if (color_value > 255)
                    {
                        color_value = 255;
                    }



                    //int pixel = Convert.ToInt32(cur_palette[(int)color_value].ToString(), 16);
                    //pixels[0] = pixel;

                    //byte[] color_rgb = new byte[3];
                    //color_rgb[0] = Convert.ToByte(cur_palette[(int)color_value].ToString().Substring(0, 2));
                    //color_rgb[1] = Convert.ToByte(cur_palette[(int)color_value].ToString().Substring(2, 2));
                    //color_rgb[2] = Convert.ToByte(cur_palette[(int)color_value].ToString().Substring(4, 2));

                    //pixels[y * ir_width + x] = Convert.ToInt32(cur_palette[(int)color_value].ToString(), 16);
                    pixels[y * ir_width + x] = Convert.ToInt32(PublicClass.grey[(int)color_value].ToString(), 16);
                    
                    //ir_bmp.WritePixels(new Int32Rect(x, y, 1, 1), pixels, stride, 0);


                }

            }
            ir_bmp.WritePixels(new Int32Rect(0, 0, ir_width, ir_height), pixels, stride * ir_width, 0);
            ir_bmp.Unlock();
            irimg_back.Source = ir_bmp;


        }


        public void creare_cur_palette()//生成右边的调色板图
        {
            cur_palette.Reverse();
            int[] pixels = new int[1];
            int stride = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;


            WriteableBitmap cur_palette_bmp = new WriteableBitmap(1, 256, 96, 96, PixelFormats.Bgr32, null);
            for (int i = 0; i < 256; i++)
            {
                int pixel = Convert.ToInt32(cur_palette[i].ToString(), 16);
                pixels[0] = pixel;
                cur_palette_bmp.WritePixels(new Int32Rect(0, i, 1, 1), pixels, stride, 0);
            }
            cur_palette_img.Source = cur_palette_bmp;
            cur_palette_img.Height = irimg.Height - 40;
            right_panel.Height = irimg.Height;
            cur_palette.Reverse();
      

        }

        private void init_temp()//初始化温度值集合
        {
            p_TemCallParameters.p_APT0AD = (ArrayList)AD_table.Clone();
            p_TemCallParameters.p_APT0TEMP = (ArrayList)TEM_table.Clone();
            p_TemCallParameters.p_DistTempTable = (ArrayList)DIST_table.Clone();
            p_TemCallParameters.SensorTemp = (Int16)SensorTemp;
            p_TemCallParameters.in_tempAD = 0x7fe4;
            p_TemCallParameters.TiEmiss_H4 = TiEmiss_H4;
            p_TemCallParameters.TiEmiss = TiEmiss;
            p_TemCallParameters.ModifyTemp = 1000;
            p_TemCallParameters.Tempunit = false;
            ir_temp = new List<int>();
            temp_max_min_avr = new ArrayList();
            temp_max_min_avr.Add(-32767);//[0]最高温
            temp_max_min_avr.Add(0);//[1]最高温坐标
            temp_max_min_avr.Add(32767);//[2]最低温
            temp_max_min_avr.Add(0);//[3]最低温坐标
            temp_max_min_avr.Add(0);//[4]平均温
            temp_max_min_avr.Add(0);//[5]中心温
            temp_max_min_avr.Add(0);//[6]中心坐标
            ir_temp.Clear();
            long temp_sum=0;
            for (int i = 0; i < ir_data.Count; i++)

            {
                p_TemCallParameters.in_tempAD =(UInt16)(int) ir_data[i];
                TemCall_Main();
                ir_temp.Add(p_TemCallParameters.Dsp_out_temp + modify_level);
                temp_sum += p_TemCallParameters.Dsp_out_temp + modify_level;
                if (int.Parse(p_TemCallParameters.Dsp_out_temp.ToString()) + modify_level > int.Parse(temp_max_min_avr[0].ToString()))
                {
                    temp_max_min_avr[0] = p_TemCallParameters.Dsp_out_temp + modify_level;
                    temp_max_min_avr[1] = i;
                }
                if (int.Parse(p_TemCallParameters.Dsp_out_temp.ToString()) + modify_level < int.Parse(temp_max_min_avr[2].ToString()))
                {
                    temp_max_min_avr[2] = p_TemCallParameters.Dsp_out_temp + modify_level;
                    temp_max_min_avr[3] = i;
                }
            }
            temp_max_min_avr[4] = temp_sum / ir_temp.Count;
         
            temp_max_min_avr[6] = (ir_height / 2 * ir_width+ir_width/2);
            temp_max_min_avr[5] = ir_temp[int.Parse(temp_max_min_avr[6].ToString())];

            max_temp = (from c in ir_temp select c).Max();
            min_temp = (from c in ir_temp select c).Min();
            middle_temp = (max_temp - min_temp) / 2d + min_temp;
        }


        public string len_2(byte bte)//转换16进制补满两位
        {
            string strbte;
            strbte = Convert.ToString(bte, 16);
            if (strbte.Length < 2)
            {
                strbte = "0" + strbte;
            }
            return strbte;
        }

        private void get_level()
        {
            int max_temp = (from c in ir_temp select c).Max();
            int min_temp = (from c in ir_temp select c).Min();
            int avr_temp = (max_temp + min_temp) / 2;
            int level = int.Parse(ir_information[13].ToString());
            modify_level = (level - 600) - avr_temp;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            PublicClass.cur_ctrl_name = this.Name;
            if (palette_name == null)
            {
                selected_spot_area = new List<string>();
                b_value = 0;
                c_value = 0;
                is_auto_adjust = true;
                cur_palette = (ArrayList)PublicClass.Iron_palette.Clone();
                //for (int i = 0; i < 256; i++)
                //{
                //    cur_palette[i] = "70" + cur_palette[i].ToString();
                //}
                cur_palette.Reverse();
                palette_name = "iron";
                is_reverse = false;
                read_ir_data();
                //  change_temp();
                TempDist = ushort.Parse(ir_information[17].ToString());
                TempTamb = ushort.Parse(ir_information[10].ToString());
                Emiss = float.Parse(ir_information[16].ToString());
                Tamb_TiEmiss_cal(TempDist, TempTamb);
                init_temp();
                get_level();
                init_temp();
                if (contrast_step < 0)
                {
                    contrast_step = 128;
                    refer_max_temp = max_temp;
                    refer_min_temp = min_temp;
                    refer_middle_temp = middle_temp;
                }
                grad_max_min();
                init_isothermal();
                creare_cur_palette();
                zoom_coe = 1;
                zoom();
                draw_img = new ArrayList();
                ir_canvas_back.Width = ir_width;
                ir_canvas_back.Height = ir_height;
                //create_img_back();

                string[] s = cur_file_name.Split('.');
                shapes_name = s[0];
                GetCharSpellCode();
              
                get_max_min();
                get_spot_area();
                create_img();
                PublicClass.is_draw_type = "work";
                vertex_coordinate.Add(0);
                vertex_coordinate.Add(0);
                vertex_coordinate.Add(ir_width);
                vertex_coordinate.Add(ir_height);
                canvas_font_mouseup();



                spot_temp_max = 3;
                spot_temp_min = 3;
                spot_temp_cen = 3;
                spot_max = Brushes.Red;
                spot_min = Brushes.Green;
                spot_cen = Brushes.White;
                //shapes_name = "0";
                //sub_spot spot = MainWindow.FindChild<sub_spot>(Application.Current.MainWindow, "spottest");
                //spot.get_spot_area();
            }

            try
            {
                sub_shapes_list shap = MainWindow.FindChild<sub_shapes_list>(Application.Current.MainWindow, "shapestest");
                shap.shapeslist("all", 0);
            }
            catch
            {

            }

            RoutedPropertyChangedEventArgs<object> args = new RoutedPropertyChangedEventArgs<object>(this, e);
            args.RoutedEvent = sub_workspace.LoadIrInformationEvent;
            this.RaiseEvent(args);
            is_mousedown = false;
        }

        public void modifytemp(bool rept)
        {
            for (int i = 0; i < PublicClass.shapes_count.Count; i++)
            {
                PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                if (newshapes.workspace_name == PublicClass.cur_ctrl_name && newshapes.percent == 0)
                {
                    for (int j = 0; j < ir_temp.Count; j++)
                    {
                        ir_temp[j] += (int)newshapes.Temprevise;
                    }
                }
                if (!rept)
                {
                    break;
                }
            }
        }

        public void area_temp()//温度参数修正
        {
            //old_TiEmiss = true;
            //old_TiEmiss_H4 = true;
            float temp_TiEmiss = TiEmiss;
            float temp_TiEmiss_H4 = TiEmiss_H4;
            UInt16 temp_TempDist = TempDist;
            UInt16 temp_TempTamb = TempTamb;
            float temp_dampness = dampness;
            float temp_Emiss = Emiss;
            int temp_modify_temp = modify_temp;

            for (int i = 0; i < PublicClass.shapes_count.Count; i++)
            {
                PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                if (newshapes.percent == 0 && PublicClass.cur_ctrl_name == newshapes.workspace_name)
                {

                    TempDist = ushort.Parse(newshapes.TempDist.ToString());
                    TempTamb = ushort.Parse(newshapes.TempTamb.ToString());
                    dampness = newshapes.dampness;
                    Emiss = newshapes.Emiss;
                    modify_temp = int.Parse(newshapes.Temprevise.ToString());
                    break;
                }
            }

                Tamb_TiEmiss_cal(TempDist, TempTamb);

            init_temp();
            modifytemp(true);



            calculate_percent();
            old_TiEmiss = false;
            old_TiEmiss_H4 = false;
            TiEmiss = temp_TiEmiss;
            TiEmiss_H4 = temp_TiEmiss_H4;
            TempDist = temp_TempDist;
            TempTamb = temp_TempTamb;
            dampness = temp_dampness;
            Emiss = temp_Emiss;
            modify_temp = temp_modify_temp;

            init_temp();
            //sub_shapes_list shap = MainWindow.FindChild<sub_shapes_list>(Application.Current.MainWindow, "shapestest");
            //shap.shapeslist("all", 0);

        }

        private void change_temp()
        {
            TiEmiss = TiEmiss1;
            TiEmiss_H4 = TiEmiss_H41;
            init_temp();
            int old_max_temp = (from c in ir_data select c).Max();
            Tamb_TiEmiss_cal(TempDist, TempTamb);
            //all_area_temp();
            init_temp();
            int new_max_temp = (from c in ir_data select c).Max();
            temp_def = old_max_temp - new_max_temp;
        }
      
            


        public void all_area_temp()//温度参数修正
        {
            ////old_TiEmiss = true;
            ////old_TiEmiss_H4 = true;
            float temp_TiEmiss = TiEmiss;
            float temp_TiEmiss_H4 = TiEmiss_H4;
            UInt16 temp_TempDist = TempDist;
            UInt16 temp_TempTamb = TempTamb;
            float temp_dampness = dampness;
            float temp_Emiss = Emiss;
            int temp_modify_temp = modify_temp;
            Tamb_TiEmiss_cal(TempDist, TempTamb);

            init_temp();
            modifytemp(false);



            calculate_percent();
            grad_max_min();
            calculate_temp_ruler();


            ir_information[16] = Emiss;
            ir_information[10] = TempTamb;
            ir_information[11] = dampness;
            ir_information[17] = TempDist;

            //old_TiEmiss = false;
            //old_TiEmiss_H4 = false;
            //TiEmiss = temp_TiEmiss;
            //TiEmiss_H4 = temp_TiEmiss_H4;
            //TempDist = temp_TempDist;
            //TempTamb = temp_TempTamb;
            //dampness = temp_dampness;
            //Emiss = temp_Emiss;
            //modify_temp = temp_modify_temp;

            //init_temp();
            //sub_shapes_list shap = MainWindow.FindChild<sub_shapes_list>(Application.Current.MainWindow, "shapestest");
            //shap.shapeslist("all", 0);

        }


        ///////////////2015-9-21//////////////////////////

        public void Tamb_TiEmiss_cal(UInt16 TempDist, UInt16 TempTamb)//传入从图片读取的距离和环境温度
        {


            UInt16 sint1;
            UInt16 sint2;
            float mfloat1;
            float mfloat2;
            float mfloat3;



            //TempDist = UInt16.Parse(ir_information[17].ToString());



            //TempTamb = UInt16.Parse(ir_information[10].ToString());



            sint1 = (UInt16)(TempTamb + 2130);   //转换为绝对温度 calScreen_para.screen_para.cal_data.Tamb

            //环境温度^4
            mfloat1 = (float)sint1;
            mfloat1 = mfloat1 * mfloat1;
            mfloat1 = mfloat1 * mfloat1;
            TiEmiss_H4 = mfloat1;

            //查表求b
            if (sint1 < 2350)     //绝对温度×10
            {
                sint2 = 2;
            }
            else
            {
                if (sint1 > 3370)
                {
                    sint2 = 13422;
                }
                else
                {
                    sint1 = (UInt16)(sint1 - 2340);
                    sint1 = (UInt16)(sint1 / 10);
                    sint2 = (UInt16)int.Parse(TEM_table[sint1].ToString());
                };
            };

            //	W_Cor=(float)(b_cor*(float)distval*relhum/100000+(float)distval*1.1106/1000)/100;
            //可凝水w
            mfloat1 = dampness;//湿度
            mfloat2 = (float)sint2;
            mfloat2 = mfloat2 * mfloat1;
            mfloat2 = mfloat2 + 11106;
            mfloat2 = mfloat2 / 1000000000;
            mfloat1 = (float)TempDist;
            mfloat2 = mfloat2 * mfloat1;

            // tao_i*Emiss
            if (mfloat2 < 0.165)
            {
                mfloat2 = (float)Math.Sqrt(mfloat2);
                mfloat3 = (float)(mfloat2 * (-0.598));
                mfloat3 = (float)Math.Exp(mfloat3);
                mfloat2 = Emiss;//图片辐射
                mfloat2 = mfloat2 / 100;
                mfloat2 = mfloat2 * mfloat3;
                TiEmiss = mfloat2;
            }
            else
            {
                mfloat3 = (float)(0.165 / mfloat2);
                //pow_cal();  //	mfloat2=pow_cal(mfloat2,0.122);
                mfloat2 = (float)(mfloat3 * 0.784);
                mfloat3 = Emiss;
                mfloat3 = mfloat3 / 100;
                mfloat2 = mfloat2 * mfloat3;
                TiEmiss = mfloat2;
            };

            // (1-TiEmiss)*环境温度^4
            mfloat3 = TiEmiss_H4;  //环境温度^4
            mfloat3 = mfloat3 * (1 - mfloat2);
            TiEmiss_H4 = mfloat3;
        }

        ////////////////2015-9-21 end////////////////////
        

        //////////////////////////////////温度计算/////////////////////////////


            public struct TemCallParameters_Struct
        {
	            public ArrayList p_APT0AD;//指向AD值表的首地址（输入）
	        public ArrayList p_APT0TEMP;//指向温度表的首地址（输入）
            public ArrayList p_DistTempTable;//指向距离修正表的首地址（输入）

	        public  Int16 SensorTemp;	//传感器的温度（输入）
	        public  UInt16  in_tempAD;	//输入的原始AD值（输入）
	        public  Int16  out_temp;		//中间变量
	        public  Int16  Dsp_out_temp;	//输出的用于显示的温度值(返回)
	        public  UInt16  ModifyTemp;	//人为已设定的修正温度（输入）
	        public  bool  Tempunit;	//单位选择（华氏度/摄氏度）（输入）

	        public UInt16  cal_big;//当温度大于测温范围时为1，否则为0(返回)
	        public  UInt16  cal_small;//当温度小于测温范围时为1，否则为0(返回)

	        public float  TiEmiss_H4;//（输入）
	        public  float  TiEmiss;//（输入）

        }

      //  void TemCall_Main( TemCallParameters_Struct p_TemCallParameters);//调用


        public void TemCall_Main( )
        {
	        //printf("p_TemCallParameters->in_tempAD_start = %x \n",p_TemCallParameters->in_tempAD);
	        //通过传感器温度修正AD值
	        AD_Call();

	        //初始化cal_big和cal_small
	        p_TemCallParameters.cal_big =0; 
	        p_TemCallParameters.cal_small =0; 

	        //判断当前的AD值有没有超出测温范围
	        if((p_TemCallParameters.in_tempAD>(int)p_TemCallParameters.p_APT0AD[0])
		        ||(p_TemCallParameters.in_tempAD<(int)p_TemCallParameters.p_APT0AD[1]))
	        {
		        if(p_TemCallParameters.in_tempAD>(int)p_TemCallParameters.p_APT0AD[1])
		        {
	    	        p_TemCallParameters.cal_big =1;           //大于最大值 
                    p_TemCallParameters.in_tempAD=(UInt16)(int)p_TemCallParameters.p_APT0AD[0] ;
		        }
		        else
		        {
	    	        p_TemCallParameters.cal_small =1;         //小于最小值 
		        }

	        }
	
	        //printf("p_TemCallParameters->in_tempAD_end = %x \n",p_TemCallParameters->in_tempAD);
	        //printf("TemCallParameters->out_temp_star = %x \n",p_TemCallParameters->out_temp);
	        //输入p_TemCallParameters->in_tempAD查温度表，
	        //并把查出温度值输出到p_TemCallParameters->out_temp中
	        Lookup_Temp();
	        //printf("Lookup_Temp_TemCallParameters->out_temp = %x \n",p_TemCallParameters->out_temp);

	        //根据TiEmiss_H4、TiEmiss把p_TemCallParameters->out_temp代入公式计算，
	        //并获得计算后的p_TemCallParameters->out_temp
	        Formula_cal();
	        //printf("Formula_cal_TemCallParameters->out_temp = %x \n",p_TemCallParameters->out_temp);

	        //查询距离修正表对温度进行修正
	        //把结果输出到p_TemCallParameters->Dsp_out_temp中
	        //printf("p_TemCallParameters->Dsp_out_temp_start = %x \n",p_TemCallParameters->Dsp_out_temp);
	        Lookup_DistTempTable();
	        //printf("Lookup_DistTempTable_p_TemCallParameters->Dsp_out_temp = %x \n",p_TemCallParameters->Dsp_out_temp);

	        //根据已设定的温度修正值对温度进行修正，并进行单位换算（华氏度/摄氏度）。
	        //获取最终的温度值输出到p_TemCallParameters->Dsp_out_temp中
	        ModifyTempOpr();
	        //printf("ModifyTempOpr_p_TemCallParameters->Dsp_out_temp = %x \n",p_TemCallParameters->Dsp_out_temp);

        }


/********************************************************
************************AD值修正*************************
*********************************************************
********************************************************/
public void AD_Call( )
{

	 int unchar21;
	 int unchar22;

	 Int16  sint42=0;
     Int16 sint01;
	 Int16  sint41=0;

     UInt16 unint31 = 0;
     UInt16 unint32 = 0;

	float 	mfloat1;
	float 	mfloat2;

	
	unchar21=(Int16)p_TemCallParameters.p_APT0TEMP[1];     //温度表的个数             
	sint42 =(Int16)p_TemCallParameters.SensorTemp;//传感器温度

	mfloat1=(float)sint42;
	mfloat1=(float)(mfloat1/25.6);
	sint01=(Int16)mfloat1;              //修正后的环境温度

	//利用环境温度查出环境温度的AD值
	for(unchar22=2;(unchar22<=unchar21)&&(unchar22<64);)
	{
		sint41=(Int16)p_TemCallParameters.p_APT0TEMP[unchar22];	 //从温度表表中获取对应的温度值
		unint31=(UInt16)(int)p_TemCallParameters.p_APT0AD[unchar22];   //从AD表中获取对应的AD值
		unchar22++;
		sint42=(Int16)p_TemCallParameters.p_APT0TEMP[unchar22];
		unint32=(UInt16)(int)p_TemCallParameters.p_APT0AD[unchar22];
		if((sint01>=sint41)&&(sint01<=sint42))	//判断跳出循环
		{
			unchar22=88;
	    }

	}
	//求出温度在对应段中的比例，从而求出对应的AD值，(t* AD)/T 
	unint32=(UInt16)(unint32-unint31);			//ad2-ad1= AD
	mfloat1=(float)unint32;

	unint32=(UInt16)(sint01-sint41);      	//t-t2= t
	mfloat2=(float)unint32;
	mfloat1=mfloat2*mfloat1;        //t*(ad2-ad1)  算式中的分子

	unint32=(UInt16)(sint42-sint41);   		//t2-t1 =T
	mfloat2=(float)unint32;
	mfloat2=mfloat1/mfloat2;        //t*AD/T   mfloat2就是得出的相对应的AD值
	

	///
	mfloat1=(float)unint31;     
	mfloat2=mfloat2+mfloat1;		 //	mfloat1(基数)+ mfloat2（小分段中的AD值）
	mfloat2=mfloat2-32768;         	//ADamb-8000H    dAD  环境相对30度的偏差


	mfloat1=(float)p_TemCallParameters.in_tempAD;
    mfloat1=mfloat1+mfloat2;
    p_TemCallParameters.in_tempAD=( UInt16)(int)mfloat1;

}


/********************************************************
***********************温度表查询************************
*********************************************************
********************************************************/
 public int Lookup_Temp()
{
	 Int16 unchar21;
	UInt16 unint31;
	UInt16 unint32;
	int unchar22;
	Int16 sint41=0;
	Int16 sint42=0;

	float 	mfloat1;
	float 	mfloat2;
	

	//查表
    unchar21=(Int16)p_TemCallParameters.p_APT0TEMP[1];     //温度表个数
	unint31=(UInt16)(int)p_TemCallParameters.p_APT0AD[0]; 
	unint32=(UInt16)(int)p_TemCallParameters.p_APT0AD[2];

	if((p_TemCallParameters.in_tempAD<=unint31)&&(p_TemCallParameters.in_tempAD>=unint32))
	{

		for(unchar22=2;(unchar22<=unchar21)&&(unchar22<64);)
		{
			sint41=(Int16)p_TemCallParameters.p_APT0TEMP[unchar22];
			unint31=(UInt16)(int)p_TemCallParameters.p_APT0AD[unchar22];
			unchar22++;
			sint42=(Int16)p_TemCallParameters.p_APT0TEMP[unchar22];
			unint32=(UInt16)(int)p_TemCallParameters.p_APT0AD[unchar22];
			if((p_TemCallParameters.in_tempAD>=unint31)&&(p_TemCallParameters.in_tempAD<=unint32))
			{
				unchar22=88;
		    }
		}
		// (t2-t1)*dAD1/(ad2-ad1)+t1 从温度表中查出对应的温度值
		p_TemCallParameters.in_tempAD=(UInt16)(p_TemCallParameters.in_tempAD-unint31);// dAD1  
		unint32=(UInt16)(unint32-unint31);   //ad2-ad1
	
		mfloat1=(float)p_TemCallParameters.in_tempAD;
		mfloat2=(float)unint32;
		mfloat1=mfloat1/mfloat2;				 //aAD1/(ad2-ad1)

		sint42=(Int16)(sint42-sint41);					// t2-t1
		mfloat2=(float)sint42;
		mfloat2=mfloat2*mfloat1;				// (t2-t1)*dAD1/(ad2-ad1)

		
		p_TemCallParameters.out_temp=(Int16)mfloat2;
		p_TemCallParameters.out_temp=(Int16)(p_TemCallParameters.out_temp+sint41);
		return 1;
	}

	return 0;
}


/********************************************************
***********************公式计算**************************
*********************************************************
********************************************************/
public void Formula_cal( )
{
	float	mfloat1;
	float	mfloat2;
	float	mfloat3;

	//cal_err=0;

	mfloat2=(float)p_TemCallParameters.out_temp+2730;    //绝对温度

	mfloat2=mfloat2*mfloat2;
	mfloat2=mfloat2*mfloat2;

	mfloat3=p_TemCallParameters.TiEmiss_H4;  //(1-tao_i*m_fEmsVal)*pow((m_fAmbTemp+273.15),4)

	mfloat1=p_TemCallParameters.TiEmiss; //Emiss*tao_i

	mfloat2=(mfloat2-mfloat3)/mfloat1;

	if(mfloat2>0)
	{
		//cal_err=0;
		mfloat2=(float)Math.Sqrt(mfloat2);
		mfloat2=(float)Math.Sqrt(mfloat2);
     

		p_TemCallParameters.out_temp=(Int16)(mfloat2-2730);

	}
}


/********************************************************
***********************距离修正温度**********************
*********************************************************
********************************************************/
 public void  Lookup_DistTempTable()
{
	Int16 unchar21;
	int unchar22;

	 Int16 sint42=0;
     float sint01;
	 Int16 sint41=0;

	 UInt16 unint31=0;
	 UInt16 unint32=0;

	float 	mfloat1;
	float 	mfloat2;
	
	
	unchar21 = (Int16)p_TemCallParameters.p_APT0TEMP[1];     //温度表个数

	sint01 = p_TemCallParameters.out_temp;              

	for(unchar22=2;(unchar22<=unchar21)&&(unchar22<64);)
	{
		sint41 = (Int16)p_TemCallParameters.p_APT0TEMP[unchar22];
		unint31 = (UInt16)(int)p_TemCallParameters.p_DistTempTable[unchar22];   //AD表
		unchar22++;
		sint42 =(Int16)p_TemCallParameters.p_APT0TEMP[unchar22];
		unint32 = (UInt16)(int)p_TemCallParameters.p_DistTempTable[unchar22];
		if((sint01>=sint41)&&(sint01<=sint42))
		{
			unchar22=88;
	    }

	}

	unint32=(UInt16)(unint32-unint31);			//ad2-ad1
	mfloat1=(float)unint32;
	unint32=(UInt16)(sint01-sint41);      	//t-t1
	mfloat2=(float)unint32;
	mfloat1=mfloat2*mfloat1;          //(t-t1)*(ad2-ad1)

	unint32=(UInt16)(sint42-sint41);   		//t2-t1
	mfloat2=(float)unint32;
	mfloat2=mfloat1/mfloat2;        	//(t-t1)*(ad2-ad1)/(t2-t1)   

	mfloat1=(float)unint31;     

	unint31 = (UInt16)(mfloat2+mfloat1) ;
    p_TemCallParameters.Dsp_out_temp = (Int16)(((float)p_TemCallParameters.out_temp * unint31) / 1000);

}

/********************************************************
***********************单位换算**************************
*********************************************************
********************************************************/
public void  ModifyTempOpr( )
{
	
	float	mfloat1;
	if(p_TemCallParameters.Tempunit) //true华氏
	{
		mfloat1 = (float)p_TemCallParameters.Dsp_out_temp ;	
		mfloat1*=(float)1.8;
		mfloat1+=(float)320.5;
        p_TemCallParameters.Dsp_out_temp = (Int16)mfloat1;
		p_TemCallParameters.Dsp_out_temp+=(Int16)(p_TemCallParameters.ModifyTemp-1800);

	}
	else
	{
		p_TemCallParameters.Dsp_out_temp+=(Int16)(p_TemCallParameters.ModifyTemp-1000);
	}
	

}

private void ir_canvas_MouseWheel(object sender, MouseWheelEventArgs e)
{

    if (e.Delta > 0)
    {
        zoom_coe += 0.1f;
    }
    if (e.Delta < 0)
    {
        zoom_coe -= 0.1f;
    }
    zoom();

    RoutedPropertyChangedEventArgs<object> args = new RoutedPropertyChangedEventArgs<object>(this, e);
    args.RoutedEvent = sub_workspace.WorkMouseWheelEvent;
    this.RaiseEvent(args);


}
Point drawpoint = new Point();//定义画图开始位置
Point rongpoint = new Point();
string move_dir = "";
private void ir_canvas_font_MouseDown(object sender, MouseButtonEventArgs e)//图形开始位置或画点
{
    rongheleft = ir_back_img.Margin.Left;
    ronghetop = ir_back_img.Margin.Top;
    rongpoint = e.GetPosition(back_canvas);
    drawpoint.X = e.GetPosition(ir_canvas_font).X;
    drawpoint.Y = e.GetPosition(ir_canvas_font).Y;
      move_dir = "";
    if (PublicClass.is_draw_type != "adjust")
    {
        //public_shapes_index = -1;
    }


    //change_shapes_active(e);
        shapes_name = "0";
        if (PublicClass.shapes_count.Count > 0)
        {
            
            
            for (int i = 0; i < PublicClass.shapes_count.Count; i++)
            {
                PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                if (newshapes.workspace_name == PublicClass.cur_ctrl_name)
                {

                    int substart = 1;
                    if (PublicClass.is_draw_type == "polyline" || PublicClass.is_draw_type == "polygon")
                    {
                        substart = 3;
                    }
                    
                    if (newshapes.shapes_type == PublicClass.is_draw_type && PublicClass.is_draw_type!= "work")
                    {
                        if (int.Parse(newshapes.shapes_name.Substring(substart, newshapes.shapes_name.Length - substart)) > int.Parse(shapes_name))
                        {
                            shapes_name = newshapes.shapes_name.Substring(substart, newshapes.shapes_name.Length - substart);
                        }

                    }

                }

            }




        }

        if (PublicClass.is_draw_type == "rectangle")
        {
            shapes_name = "R" +(int.Parse(shapes_name)+1);
        }
        else if (PublicClass.is_draw_type == "ellipse")
        {
            shapes_name = "C" + (int.Parse(shapes_name) + 1);
        }
        else if (PublicClass.is_draw_type == "line")
        {
            shapes_name = "L" + (int.Parse(shapes_name) + 1);
        }
        else if (PublicClass.is_draw_type == "point")
        {
            shapes_name = "P" + (int.Parse(shapes_name) + 1);
        }
        else if (PublicClass.is_draw_type == "polyline")
        {
            shapes_name = "Cur" + (int.Parse(shapes_name) + 1);
        }
        else if (PublicClass.is_draw_type == "polygon")
        {
            shapes_name = "Pol" + (int.Parse(shapes_name) + 1);
        }
        if (PublicClass.is_draw_type != "adjust")
        {
            shapes_active = shapes_name;
        }


        if (PublicClass.is_draw_type == "point")
        {
            for (int i1 = 1; i1 < 3; i1++)//删除点
            {
                System.Windows.Shapes.Path deletepath = ir_canvas_font.FindName(shapes_name + i1.ToString()) as System.Windows.Shapes.Path;
                deletepath = ir_canvas_font.FindName(shapes_name + i1.ToString()) as System.Windows.Shapes.Path;
                if (deletepath != null)
                {
                    ir_canvas_font.Children.Remove(deletepath);
                    ir_canvas_font.UnregisterName(shapes_name + i1.ToString());
                }
            }


            System.Windows.Media.Effects.DropShadowEffect da = new System.Windows.Media.Effects.DropShadowEffect();
            da.BlurRadius = 1;
            da.Opacity = 0.65;
            da.ShadowDepth = 1;
            //newpolyline.StrokeDashArray = new DoubleCollection(new double[2] { 4, 4 });
            LineGeometry linex = new LineGeometry();
            LineGeometry liney = new LineGeometry();
            linex.StartPoint = new Point(drawpoint.X - 9, drawpoint.Y);
            linex.EndPoint = new Point(drawpoint.X + 9, drawpoint.Y);
            liney.StartPoint = new Point(drawpoint.X, drawpoint.Y - 9);
            liney.EndPoint = new Point(drawpoint.X, drawpoint.Y + 9);

            System.Windows.Shapes.Path mypathx = new System.Windows.Shapes.Path();
            mypathx.Stroke = Brushes.White;
            mypathx.StrokeThickness = 1;
            mypathx.Data = linex;
            mypathx.SnapsToDevicePixels = true;
            if (drawpoint.X - 10 < 0)
            {
                linex.StartPoint = new Point(drawpoint.X + 3, drawpoint.Y);
              
            }
            else if (drawpoint.X + 10 > ir_width * zoom_coe)
            {
                linex.EndPoint = new Point(drawpoint.X - 3, drawpoint.Y);
              
            }
            else
            {
               
                mypathx.StrokeDashArray = new DoubleCollection(new double[2] { 6, 6 });
            }
            ir_canvas_font.Children.Add(mypathx);
            ir_canvas_font.RegisterName(shapes_name + "1", mypathx);
            mypathx.MouseUp += new MouseButtonEventHandler(ir_canvas_font_MouseUp);
            mypathx.MouseDown += new MouseButtonEventHandler(ir_canvas_font_MouseDown);
            mypathx.MouseMove += new MouseEventHandler(ir_canvas_font_MouseMove);

            System.Windows.Shapes.Path mypathy = new System.Windows.Shapes.Path();
            mypathy.Stroke = Brushes.White;
            mypathy.StrokeThickness = 1;
            mypathy.Data = liney;
            mypathy.SnapsToDevicePixels = true;
            if (drawpoint.Y - 10 < 0)
            {
                liney.StartPoint = new Point(drawpoint.X, drawpoint.Y + 3);
            }
            else if (drawpoint.Y + 10 > ir_height * zoom_coe)
            {
                liney.EndPoint = new Point(drawpoint.X, drawpoint.Y - 3);
            }
            else
            {
                mypathy.StrokeDashArray = new DoubleCollection(new double[2] { 6, 6 });
            }
            ir_canvas_font.Children.Add(mypathy);
            ir_canvas_font.RegisterName(shapes_name + "2", mypathy);
            mypathy.MouseUp += new MouseButtonEventHandler(ir_canvas_font_MouseUp);
            mypathy.MouseDown += new MouseButtonEventHandler(ir_canvas_font_MouseDown);
            mypathy.MouseMove += new MouseEventHandler(ir_canvas_font_MouseMove);
            //if (drawpoint.X - 8 > 0)
            //{
            //    LineGeometry line1 = new LineGeometry();
            //    line1.StartPoint = new Point(drawpoint.X - 8, drawpoint.Y);
            //    line1.EndPoint = new Point(drawpoint.X - 3, drawpoint.Y);
            //    System.Windows.Shapes.Path mypath = new System.Windows.Shapes.Path();
            //    mypath.Stroke = Brushes.White;
            //    mypath.StrokeThickness = 1;
            //    mypath.Data = line1;
            //    mypath.Effect = da;
            //    mypath.SnapsToDevicePixels = true;
            //    ir_canvas_font.Children.Add(mypath);
            //    ir_canvas_font.RegisterName(shapes_name + "1", mypath);
            //    mypath.MouseUp += new MouseButtonEventHandler(ir_canvas_font_MouseUp);
            //    mypath.MouseDown += new MouseButtonEventHandler(ir_canvas_font_MouseDown);
            //    mypath.MouseMove += new MouseEventHandler(ir_canvas_font_MouseMove);
            //}
            //if (drawpoint.X + 8 < ir_width * zoom_coe)
            //{
            //    LineGeometry line1 = new LineGeometry();
            //    line1.StartPoint = new Point(drawpoint.X + 3, drawpoint.Y);
            //    line1.EndPoint = new Point(drawpoint.X + 8, drawpoint.Y);
            //    System.Windows.Shapes.Path mypath = new System.Windows.Shapes.Path();
            //    mypath.Stroke = Brushes.White;
            //    mypath.StrokeThickness = 1;
            //    mypath.Data = line1;
            //    mypath.Effect = da;
            //    mypath.SnapsToDevicePixels = true;
            //    ir_canvas_font.Children.Add(mypath);
            //    ir_canvas_font.RegisterName(shapes_name + "2", mypath);
            //    mypath.MouseUp += new MouseButtonEventHandler(ir_canvas_font_MouseUp);
            //    mypath.MouseDown += new MouseButtonEventHandler(ir_canvas_font_MouseDown);
            //    mypath.MouseMove += new MouseEventHandler(ir_canvas_font_MouseMove);
            //}
            //if (drawpoint.Y - 8 > 0)
            //{
            //    LineGeometry line1 = new LineGeometry();
            //    line1.StartPoint = new Point(drawpoint.X, drawpoint.Y - 8);
            //    line1.EndPoint = new Point(drawpoint.X, drawpoint.Y - 3);
            //    System.Windows.Shapes.Path mypath = new System.Windows.Shapes.Path();
            //    mypath.Stroke = Brushes.White;
            //    mypath.StrokeThickness = 1;
            //    mypath.Data = line1;
            //    mypath.Effect = da;
            //    mypath.SnapsToDevicePixels = true;
            //    ir_canvas_font.Children.Add(mypath);
            //    ir_canvas_font.RegisterName(shapes_name + "3", mypath);
            //    mypath.MouseUp += new MouseButtonEventHandler(ir_canvas_font_MouseUp);
            //    mypath.MouseDown += new MouseButtonEventHandler(ir_canvas_font_MouseDown);
            //    mypath.MouseMove += new MouseEventHandler(ir_canvas_font_MouseMove);
            //}
            //if (drawpoint.Y + 8 < ir_height * zoom_coe)
            //{
            //    LineGeometry line1 = new LineGeometry();
            //    line1.StartPoint = new Point(drawpoint.X, drawpoint.Y + 8);
            //    line1.EndPoint = new Point(drawpoint.X, drawpoint.Y + 3);
            //    System.Windows.Shapes.Path mypath = new System.Windows.Shapes.Path();
            //    mypath.Stroke = Brushes.White;
            //    mypath.StrokeThickness = 1;
            //    mypath.Data = line1;
            //    mypath.Effect = da;
            //    mypath.SnapsToDevicePixels = true;
            //    ir_canvas_font.Children.Add(mypath);
            //    ir_canvas_font.RegisterName(shapes_name + "4", mypath);
            //    mypath.MouseUp += new MouseButtonEventHandler(ir_canvas_font_MouseUp);
            //    mypath.MouseDown += new MouseButtonEventHandler(ir_canvas_font_MouseDown);
            //    mypath.MouseMove += new MouseEventHandler(ir_canvas_font_MouseMove);
            //}

            Point txtpoint = new Point();
            txtpoint.X = drawpoint.X - 10;
            txtpoint.Y = drawpoint.Y - 25;
            if (txtpoint.X < 10)
            {
                txtpoint.X = 10;
            }
            if (txtpoint.Y < 10)
            {
                txtpoint.Y = 10;
            }
            TextBlock txt = new TextBlock();
            txt.Text = shapes_name;
            dynamic_txt(txtpoint, shapes_name);

            
            vertex_coordinate.Add(drawpoint.X / zoom_coe);
            vertex_coordinate.Add(drawpoint.Y / zoom_coe);
            is_mousedown = true;
            canvas_font_mouseup();
        }
        else if (PublicClass.is_draw_type == "polyline")
        {
            if (e.ClickCount == 1)
            {
                if (!is_collection)
                {
                    collection.Clear();
                    is_collection = true;
                }
                collection.Add(new Point(drawpoint.X / zoom_coe, drawpoint.Y / zoom_coe));
                Point txtpoint = new Point();
                txtpoint.X = collection[0].X * zoom_coe - 5;
                txtpoint.Y = collection[0].Y * zoom_coe - 15;
                if (txtpoint.X < 10)
                {
                    txtpoint.X = 10;
                }
                if (txtpoint.Y < 10)
                {
                    txtpoint.Y = 10;
                }
                dynamic_txt(txtpoint, shapes_name);
            }
        }



        else if (PublicClass.is_draw_type == "polygon")
        {
            if (e.ClickCount == 1)
            {
                if (!is_collection)
                {
                    collection.Clear();
                    is_collection = true;
                }
                collection.Add(new Point(drawpoint.X / zoom_coe, drawpoint.Y / zoom_coe));
                Point txtpoint = new Point();
                txtpoint.X = collection[0].X * zoom_coe - 5;
                txtpoint.Y = collection[0].Y * zoom_coe - 15;
                if (txtpoint.X < 10)
                {
                    txtpoint.X = 10;
                }
                if (txtpoint.Y < 10)
                {
                    txtpoint.Y = 10;
                }
                dynamic_txt(txtpoint, shapes_name);
            }
        }


        if (e.ClickCount == 2&&drawpoint==e.GetPosition(ir_canvas_font))
        {
            vertex_coordinate.Clear();
            if (PublicClass.is_draw_type == "polyline")
            {

                for (int i = 0; i < collection.Count; i++)
                {

                    vertex_coordinate.Add(collection[i].X);
                    vertex_coordinate.Add(collection[i].Y);
                }


            }
            else if (PublicClass.is_draw_type == "polygon")
            {

                for (int i = 0; i < collection.Count; i++)
                {

                    vertex_coordinate.Add(collection[i].X);
                    vertex_coordinate.Add(collection[i].Y);
                }


            }
            is_collection = false;
        }


        //if (e.ClickCount == 2)
        //{
        //    vertex_coordinate = "polygon," + shapes_name + ", , , , , , , , ,";
        //    for (int i = 0; i < collection.Count; i++)
        //    {
        //        vertex_coordinate += collection[i].X + "*" + collection[i].Y + ",";
        //    }
        //    vertex_coordinate += "eof";
        //    is_collection = false;
        //}

        if (e.ClickCount > 1 && drawpoint == e.GetPosition(ir_canvas_font))
        {
            doubleclick = true;
        }
  }

private void dynamic_txt(Point txtpoint,string local_name)//图形左上角名称
{
    TextBlock deletetxt = ir_canvas_font.FindName("txt" + local_name) as TextBlock;
    if (deletetxt != null)
    {
        ir_canvas_font.Children.Remove(deletetxt);
        ir_canvas_font.UnregisterName("txt" + local_name);
    }
    System.Windows.Media.Effects.DropShadowEffect da = new System.Windows.Media.Effects.DropShadowEffect();
    da.BlurRadius = 1;
    da.Opacity = 0.65;
    da.ShadowDepth = 1;
    TextBlock txt = new TextBlock();
    PublicClass.shapes_property test = new PublicClass.shapes_property();
    if (local_name=="max_temp")
    {
        for (int i = 0; i < PublicClass.shapes_count.Count; i++)
        {
            test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
            if (test.shapes_name == "max_temp"&&test.workspace_name==PublicClass.cur_ctrl_name)
            {
                try
                {
                    txt.Margin = new Thickness((int)(test.vertex[0] * zoom_coe),(int)(test.vertex[1] * zoom_coe)+16, 0, 0);
                    if (spot_temp_max==1)
                    {
                            //+ "[" + "Max" + ":" + test.max_temp / 10.0 + "℃" + "]";
                        txt.Text = "[" + "Max" + ":" + test.max_temp / 10.0 + "℃" + "]";
                    }
                    else if (spot_temp_max == 2)
                    {
                        txt.Text = "[" + (int)(test.vertex[0] * zoom_coe) + "," + (int)(test.vertex[1] * zoom_coe) + "]";
                    }
                    else if (spot_temp_max == 0)
                    {
                        txt.Text = "";
                    }
                    else if (spot_temp_max == 3)
                    {
                        txt.Text = "[" + (int)(test.vertex[0] * zoom_coe) + "," + (int)(test.vertex[1] * zoom_coe) + "]" + "[" + "Max" + ":" + test.max_temp / 10.0 + "℃" + "]";
                    }
                    if (spot_max != Brushes.Red)
                    {
                        Brush brush = spot_max;
                        txt.Foreground = brush;
                    }
                    else
                    {
                        txt.Foreground = Brushes.Red;
                    }
                   
                    if (ir_width * zoom_coe - 115 < (int)(test.vertex[0] * zoom_coe) && (int)(test.vertex[0] * zoom_coe) <= ir_width * zoom_coe)
                    {
                        txt.Margin = new Thickness(ir_width * zoom_coe - 115, (int)(test.vertex[1] * zoom_coe) + 16, 0, 0);
                    }
                    if (ir_height * zoom_coe - 32 < (int)(test.vertex[1] * zoom_coe) && (int)(test.vertex[1] * zoom_coe) <= ir_height * zoom_coe)
                    {
                        txt.Margin = new Thickness((int)(test.vertex[0] * zoom_coe), ir_height * zoom_coe - 16, 0, 0);
                    }
                    if (ir_width * zoom_coe - 115 <= (int)(test.vertex[0] * zoom_coe) && (int)(test.vertex[0] * zoom_coe) <= ir_width * zoom_coe && ir_height * zoom_coe - 32 <= (int)(test.vertex[1] * zoom_coe) && (int)(test.vertex[1] * zoom_coe) <= ir_height * zoom_coe)
                    {
                        txt.Margin = new Thickness(ir_width * zoom_coe - 115, ir_height * zoom_coe - 16, 0, 0);
                    }
                }
                catch
                { }
            }
        }
    }

    else if (local_name == "min_temp")
    {
        for (int i = 0; i < PublicClass.shapes_count.Count; i++)
        {
            test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
            if (test.shapes_name == "min_temp" && test.workspace_name == PublicClass.cur_ctrl_name)
            {
                try
                {
                    txt.Margin = new Thickness((int)(test.vertex[0] * zoom_coe), (int)(test.vertex[1] * zoom_coe) + 16, 0, 0);



                    if (spot_temp_min==1)
                    {

                        //+ "[" + "Max" + ":" + test.max_temp / 10.0 + "℃" + "]";
                        txt.Text = "[" + "Min" + ":" + test.max_temp / 10.0 + "℃" + "]";
                    }
                    else if (spot_temp_min == 2)
                    {
                        txt.Text = "[" + (int)(test.vertex[0] * zoom_coe) + "," + (int)(test.vertex[1] * zoom_coe) + "]";
                    }
                    else if (spot_temp_min == 0)
                    {
                        txt.Text = "";
                    }
                    else if (spot_temp_min == 3)
                    {
                        txt.Text = "[" + (int)(test.vertex[0] * zoom_coe) + "," + (int)(test.vertex[1] * zoom_coe) + "]" + "[" + "Min" + ":" + test.max_temp / 10.0 + "℃" + "]";
                    }
                    if (spot_max !=Brushes.Red )
                    {
                        Brush brush = spot_min;
                        txt.Foreground = brush;
                    }
                    else
                    {
                        txt.Foreground = Brushes.Green;
                    }
                   
                  
                    if (ir_width * zoom_coe - 115 < (int)(test.vertex[0] * zoom_coe) && (int)(test.vertex[0] * zoom_coe) <= ir_width * zoom_coe)
                    {
                        txt.Margin = new Thickness(ir_width * zoom_coe - 115, (int)(test.vertex[1] * zoom_coe) + 16, 0, 0);
                    }
                    if (ir_height * zoom_coe - 32 < (int)(test.vertex[1] * zoom_coe) && (int)(test.vertex[1] * zoom_coe) <= ir_height * zoom_coe)
                    {
                        txt.Margin = new Thickness((int)(test.vertex[0] * zoom_coe), ir_height * zoom_coe - 16, 0, 0);
                    }
                    if (ir_width * zoom_coe - 115 <= (int)(test.vertex[0] * zoom_coe) && (int)(test.vertex[0] * zoom_coe) <= ir_width * zoom_coe && ir_height * zoom_coe - 32 <= (int)(test.vertex[1] * zoom_coe) && (int)(test.vertex[1] * zoom_coe) <= ir_height * zoom_coe)
                    {
                        txt.Margin = new Thickness(ir_width * zoom_coe - 115, ir_height * zoom_coe - 16, 0, 0);
                    }
                }
                catch
                { }
            }
        }
    }
    else 
    {
    txt.Margin = new Thickness(txtpoint.X, txtpoint.Y, 0, 0);
    txt.Text = local_name;
    txt.Foreground = Brushes.White;
    }

    //    if (max_min[0] == "max_temp")
    //    {
    //        txt.Text = "[" + max_min[1] + "," + max_min[2] + "]" + "[" + "Max" + ":" + int.Parse(ir_temp[int.Parse(max_min[3].ToString())].ToString()) / 10 + "℃" + "]"; ;
    //    }
    //    if (max_min[0] == "min_temp")
    //    {
    //        txt.Text = "[" + max_min[1] + "," + max_min[2] + "]" + "[" + "Max" + ":" + int.Parse(ir_temp[int.Parse(max_min[3].ToString())].ToString()) / 10 + "℃" + "]"; ;
    //    }
    //}
    //txt.Effect = da;
    //Panel.SetZIndex(txt, 2);
    ir_canvas_font.Children.Add(txt);
    ir_canvas_font.RegisterName("txt" + local_name, txt);
    txt.MouseUp += new MouseButtonEventHandler(ir_canvas_font_MouseUp);
}

private void dynamic_shapes(Object local_shapes_type, string local_shapes_name, string local_name, bool is_fill)//绘制图形
{



    System.Windows.Media.Effects.DropShadowEffect da = new System.Windows.Media.Effects.DropShadowEffect();
    da.BlurRadius = 0;
    da.Opacity = 0.65;
    da.ShadowDepth = 1;


    if (local_shapes_name == "polyline" || local_shapes_name == "polygon")
    {
        if (local_shapes_name == "polyline")
        {
            Polyline deletepath = ir_canvas_font.FindName(local_name) as Polyline;
            if (deletepath != null)
            {
                ir_canvas_font.Children.Remove(deletepath);
                ir_canvas_font.UnregisterName(local_name);
            }
            Polyline newpolyline = (Polyline)local_shapes_type;

            //newpolyline.Effect = da;
            newpolyline.Stroke = Brushes.White;
            newpolyline.StrokeThickness = 1;
            newpolyline.SnapsToDevicePixels = true;
            //char c = local_name.Last();
            if (local_name.Last() == 'b')
            {
                newpolyline.StrokeDashArray = new DoubleCollection(new double[2] { 4, 4 });
                newpolyline.Stroke = Brushes.Black;
                //newpolyline.StrokeDashOffset = 4;
                //newpolyline.StrokeThickness = 5;
            }

            if (shapes_active == local_name.Substring(0, local_name.Length - 1) && local_name.Last() == 'b')
            {
                //newpolyline.StrokeDashArray = new DoubleCollection(new double[2] { 4, 4 });
                newpolyline.Stroke = new SolidColorBrush(Color.FromRgb((byte)51, (byte)102, (byte)255));
                DoubleAnimation strokeoffset = new DoubleAnimation();
                strokeoffset.To = -8;
                strokeoffset.Duration = TimeSpan.FromSeconds(1.5);
                strokeoffset.RepeatBehavior = RepeatBehavior.Forever;
                newpolyline.BeginAnimation(Polyline.StrokeDashOffsetProperty, strokeoffset);

            }

            ir_canvas_font.Children.Add(newpolyline);
            ir_canvas_font.RegisterName(local_name, newpolyline);
            newpolyline.MouseUp += new MouseButtonEventHandler(ir_canvas_font_MouseUp);
            newpolyline.MouseDown += new MouseButtonEventHandler(ir_canvas_font_MouseDown);
            newpolyline.MouseMove += new MouseEventHandler(ir_canvas_font_MouseMove);
            newpolyline.MouseWheel += new MouseWheelEventHandler(ir_canvas_MouseWheel);
        }
        else if (local_shapes_name == "polygon")
        {
            Polygon deletepath = ir_canvas_font.FindName(local_name) as Polygon;
            if (deletepath != null)
            {
                ir_canvas_font.Children.Remove(deletepath);
                ir_canvas_font.UnregisterName(local_name);
            }
            Polygon polygon2 = (Polygon)local_shapes_type;

            //polygon2.Effect = da;
            polygon2.Stroke = Brushes.White;
            polygon2.StrokeThickness = 1;
            polygon2.SnapsToDevicePixels = true;
            if (is_fill)
            {
                polygon2.Effect = da;
                LinearGradientBrush newbrush = new LinearGradientBrush();
                newbrush.GradientStops.Add(new GradientStop(Color.FromArgb(30, 255, 255, 255), 0));
                newbrush.GradientStops.Add(new GradientStop(Color.FromArgb(90, 255, 255, 255), 1));
                polygon2.FillRule = FillRule.Nonzero;
                polygon2.Fill = newbrush;
            }

            if (local_name.Last() == 'b')
            {
                polygon2.StrokeDashArray = new DoubleCollection(new double[2] { 4, 4 });
                polygon2.Stroke = Brushes.Black;
                //newpolyline.StrokeDashOffset = 4;
                //newpolyline.StrokeThickness = 5;
            }
            if (shapes_active == local_name.Substring(0, local_name.Length - 1) && local_name.Last() == 'b')
            {
                //newpolyline.StrokeDashArray = new DoubleCollection(new double[2] { 4, 4 });
                polygon2.Stroke = new SolidColorBrush(Color.FromRgb((byte)51, (byte)102, (byte)255));
                DoubleAnimation strokeoffset = new DoubleAnimation();
                strokeoffset.To = -8;
                strokeoffset.Duration = TimeSpan.FromSeconds(1.5);
                strokeoffset.RepeatBehavior = RepeatBehavior.Forever;
                polygon2.BeginAnimation(Polygon.StrokeDashOffsetProperty, strokeoffset);

            }

            ir_canvas_font.Children.Add(polygon2);
            ir_canvas_font.RegisterName(local_name, polygon2);
            polygon2.MouseUp += new MouseButtonEventHandler(ir_canvas_font_MouseUp);
            polygon2.MouseDown += new MouseButtonEventHandler(ir_canvas_font_MouseDown);
            polygon2.MouseMove += new MouseEventHandler(ir_canvas_font_MouseMove);
            polygon2.MouseWheel += new MouseWheelEventHandler(ir_canvas_MouseWheel);
        }
    }

    else
    {
        System.Windows.Shapes.Path deletepath = ir_canvas_font.FindName(local_name) as System.Windows.Shapes.Path;
        if (deletepath != null)
        {
            ir_canvas_font.Children.Remove(deletepath);
            ir_canvas_font.UnregisterName(local_name);
        }
        System.Windows.Shapes.Path mypath = new System.Windows.Shapes.Path();
        mypath.Stroke = Brushes.White;
        mypath.StrokeThickness = 1;
        if (local_shapes_name == "rectangle")
        {
            mypath.Data = (RectangleGeometry)local_shapes_type;

        }
        else if (local_shapes_name == "ellipse")
        {
            mypath.Data = (EllipseGeometry)local_shapes_type;
        }
        else if (local_shapes_name == "line")
        {
             if (local_name.First() == 'P'||local_name.First() == 's'||local_name.First() == 'm')
            {
                if (local_name.Length>2)
                {

                if (local_name.Substring(0, 3)=="max")
                {
                    mypath.Stroke = spot_max;
                }
                else if (local_name.Substring(0, 3) == "min")
                {

                    mypath.Stroke = spot_min;

                }
                else
                {
                    mypath.Stroke = Brushes.White;
                }
            }
                    if (local_name.Last() == '1')
                    {
                         
                        LineGeometry newline = (LineGeometry)local_shapes_type;
                        if (Math.Abs(newline.StartPoint.X - newline.EndPoint.X) > 17)
                        {
                            mypath.Data = (LineGeometry)local_shapes_type;
                            mypath.StrokeDashArray = new DoubleCollection(new double[2] { 6, 6 });
                        }
                        else
                        {
                            mypath.Data = (LineGeometry)local_shapes_type;
                        }
                    }
                    else if (local_name.Last() == '2' )
                    {
                       
                          
                        
                        LineGeometry newline = (LineGeometry)local_shapes_type;
                        if (Math.Abs(newline.StartPoint.Y - newline.EndPoint.Y) > 17)
                        {
                            mypath.Data = (LineGeometry)local_shapes_type;
                            mypath.StrokeDashArray = new DoubleCollection(new double[2] { 6, 6 });
                        }
                        else
                        {
                            mypath.Stroke = Brushes.White;
                            mypath.Data = (LineGeometry)local_shapes_type;
                        }
                    }
                    if (shapes_active == local_name.Substring(0, local_name.Length - 1))
                    {
                        mypath.Stroke = new SolidColorBrush(Color.FromRgb((byte)51, (byte)102, (byte)255));
                    }

              
            }

            else
            {
                mypath.Stroke = Brushes.White;
                mypath.Data = (LineGeometry)local_shapes_type;
            }

                }

                if (local_name.Last() == 'b')
                {
                    mypath.StrokeDashArray = new DoubleCollection(new double[2] { 4, 4 });
                    mypath.Stroke = Brushes.Black;
                    //newpolyline.StrokeDashOffset = 4;
                    //newpolyline.StrokeThickness = 5;
                }
                if (shapes_active == local_name.Substring(0, local_name.Length - 1) && local_name.Last() == 'b')
                {
                    //newpolyline.StrokeDashArray = new DoubleCollection(new double[2] { 4, 4 });

                    mypath.Stroke = new SolidColorBrush(Color.FromRgb((byte)51, (byte)102, (byte)255));
                    DoubleAnimation strokeoffset = new DoubleAnimation();
                    strokeoffset.To = -8;
                    strokeoffset.Duration = TimeSpan.FromSeconds(1.5);
                    strokeoffset.RepeatBehavior = RepeatBehavior.Forever;
                    mypath.BeginAnimation(System.Windows.Shapes.Path.StrokeDashOffsetProperty, strokeoffset);

                }
                if (is_fill)
                {
                    mypath.Effect = da;
                    LinearGradientBrush newbrush = new LinearGradientBrush();
                    newbrush.GradientStops.Add(new GradientStop(Color.FromArgb(30, 255, 255, 255), 0));
                    newbrush.GradientStops.Add(new GradientStop(Color.FromArgb(90, 255, 255, 255), 1));
                    mypath.Fill = newbrush;
                }
                mypath.SnapsToDevicePixels = true;
                if (local_name != "0")
                {
                    ir_canvas_font.Children.Add(mypath);
                    ir_canvas_font.RegisterName(local_name, mypath);
                    mypath.MouseUp += new MouseButtonEventHandler(ir_canvas_font_MouseUp);
                    mypath.MouseDown += new MouseButtonEventHandler(ir_canvas_font_MouseDown);
                    mypath.MouseMove += new MouseEventHandler(ir_canvas_font_MouseMove);
                    mypath.MouseWheel += new MouseWheelEventHandler(ir_canvas_MouseWheel);
                }

            }



        
    

}

private void create_vertex_animation()
{

    for (int i = 0; i < vertex_shapes_name.Count; i++)
    {
        sub_ellipse_animation deleteell = ir_canvas_font.FindName(vertex_shapes_name[i]) as sub_ellipse_animation;
        if (deleteell != null)
        {
            ir_canvas_font.Children.Remove(deleteell);
            ir_canvas_font.UnregisterName(vertex_shapes_name[i]);
        }

    }
    vertex_shapes_name.Clear();

    if (public_shapes_index >= 0)
    {
        PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[public_shapes_index];
        if (newshapes.shapes_type == "line")
        {
            sub_ellipse_animation ell = new sub_ellipse_animation();
            ell.Width = 40;
            ell.Height = 40;
            ell.Margin = new Thickness(newshapes.vertex[0] * zoom_coe - 3, newshapes.vertex[1] * zoom_coe - 3, 0, 0);
            ell.MouseMove += new MouseEventHandler(ir_canvas_font_MouseMove);
            Panel.SetZIndex(ell, 1);
            //ell.Name = "startpoint";
            ir_canvas_font.Children.Add(ell);
            ir_canvas_font.RegisterName("ell0", ell);
            vertex_shapes_name.Add("ell0");

            sub_ellipse_animation ell2 = new sub_ellipse_animation();
            ell2.Width = 40;
            ell2.Height = 40;
            ell2.Margin = new Thickness(newshapes.vertex[2] * zoom_coe - 3, newshapes.vertex[3] * zoom_coe - 3, 0, 0);
            ell2.MouseMove += new MouseEventHandler(ir_canvas_font_MouseMove);
            Panel.SetZIndex(ell2, 1);
            //ell2.Name = "endpoint";
            ir_canvas_font.Children.Add(ell2);
            ir_canvas_font.RegisterName("ell1", ell2);
            vertex_shapes_name.Add("ell1");
        }
        else if (newshapes.shapes_type == "ellipse")
        {
            sub_ellipse_animation[] ell = new sub_ellipse_animation[4];
            for (int i = 0; i < 4; i++)
            {
                ell[i] = new sub_ellipse_animation();
                ell[i].Width = 40;
                ell[i].Height = 40;
                if (i == 0)
                {
                    ell[i].Margin = new Thickness(newshapes.vertex[0] * zoom_coe - newshapes.vertex[2] * zoom_coe - 3, newshapes.vertex[1] * zoom_coe - 3, 0, 0);
                }
                else if (i == 1)
                {
                    ell[i].Margin = new Thickness(newshapes.vertex[0] * zoom_coe - 3, newshapes.vertex[1] * zoom_coe - newshapes.vertex[3] * zoom_coe - 3, 0, 0);
                }
                else if (i == 2)
                {
                    ell[i].Margin = new Thickness(newshapes.vertex[0] * zoom_coe + newshapes.vertex[2] * zoom_coe - 3, newshapes.vertex[1] * zoom_coe - 3, 0, 0);
                }
                else if (i == 3)
                {
                    ell[i].Margin = new Thickness(newshapes.vertex[0] * zoom_coe - 3, newshapes.vertex[1] * zoom_coe + newshapes.vertex[3] * zoom_coe - 3, 0, 0);
                }
                ell[i].MouseMove += new MouseEventHandler(ir_canvas_font_MouseMove);
                Panel.SetZIndex(ell[i], 1);
                ir_canvas_font.Children.Add(ell[i]);
                ir_canvas_font.RegisterName("ell" + i, ell[i]);
                vertex_shapes_name.Add("ell" + i);
            }

        }
        else if (newshapes.shapes_type == "rectangle")
        {
            sub_ellipse_animation[] ell = new sub_ellipse_animation[4];
            for (int i = 0; i < 4; i++)
            {
                ell[i] = new sub_ellipse_animation();
                ell[i].Width = 40;
                ell[i].Height = 40;
                if (i == 0)
                {
                    ell[i].Margin = new Thickness(newshapes.vertex[0] * zoom_coe - 3, newshapes.vertex[1] * zoom_coe - 3, 0, 0);
                }
                else if (i == 1)
                {
                    ell[i].Margin = new Thickness(newshapes.vertex[0] * zoom_coe + newshapes.vertex[2] * zoom_coe - 3, newshapes.vertex[1] * zoom_coe - 3, 0, 0);
                }
                else if (i == 2)
                {
                    ell[i].Margin = new Thickness(newshapes.vertex[0] * zoom_coe + newshapes.vertex[2] * zoom_coe - 3, newshapes.vertex[1] * zoom_coe + newshapes.vertex[3] * zoom_coe - 3, 0, 0);
                }
                else if (i == 3)
                {
                    ell[i].Margin = new Thickness(newshapes.vertex[0] * zoom_coe - 3, newshapes.vertex[1] * zoom_coe + newshapes.vertex[3] * zoom_coe - 3, 0, 0);
                }
                ell[i].MouseMove += new MouseEventHandler(ir_canvas_font_MouseMove);
                Panel.SetZIndex(ell[i], 1);
                ir_canvas_font.Children.Add(ell[i]);
                ir_canvas_font.RegisterName("ell" + i, ell[i]);
                vertex_shapes_name.Add("ell" + i);
            }

        }

        else if (newshapes.shapes_type == "polygon")
        {
            sub_ellipse_animation[] ell = new sub_ellipse_animation[newshapes.vertex.Count];
            for (int i = 0; i < newshapes.vertex.Count - 1; i = i + 2)
            {
                ell[i] = new sub_ellipse_animation();
                ell[i].Width = 40;
                ell[i].Height = 40;
                ell[i].Margin = new Thickness(newshapes.vertex[i] * zoom_coe - 3, newshapes.vertex[i + 1] * zoom_coe - 3, 0, 0);
                ell[i].MouseMove += new MouseEventHandler(ir_canvas_font_MouseMove);
                Panel.SetZIndex(ell[i], 1);
                //ell.Name = "startpoint";
                ir_canvas_font.Children.Add(ell[i]);
                ir_canvas_font.RegisterName("ell" + i, ell[i]);
                vertex_shapes_name.Add("ell" + i);
            }

        }
        else if (newshapes.shapes_type == "polyline")
        {
            sub_ellipse_animation[] ell = new sub_ellipse_animation[newshapes.vertex.Count];
            for (int i = 0; i < newshapes.vertex.Count - 1; i = i + 2)
            {
                ell[i] = new sub_ellipse_animation();
                ell[i].Width = 40;
                ell[i].Height = 40;
                ell[i].Margin = new Thickness(newshapes.vertex[i] * zoom_coe - 3, newshapes.vertex[i + 1] * zoom_coe - 3, 0, 0);
                ell[i].MouseMove += new MouseEventHandler(ir_canvas_font_MouseMove);
                Panel.SetZIndex(ell[i], 1);
                //ell.Name = "startpoint";
                ir_canvas_font.Children.Add(ell[i]);
                ir_canvas_font.RegisterName("ell" + i, ell[i]);
                vertex_shapes_name.Add("ell" + i);
            }

        }



    }
}//画图形小圆圈

private void ir_canvas_font_MouseMove(object sender, MouseEventArgs e)//调整当前选中图形
{
  if(PublicClass.is_cur_temp=="true")
  {
        canvas_temp(e); 
  }
   
    change_cursors(e);

    if (e.LeftButton == MouseButtonState.Pressed)
    {
        shapes_name = "0";
        if (shapes_name == "0")
        {



            if (PublicClass.shapes_count.Count > 0)
            {


                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                    if (newshapes.workspace_name == PublicClass.cur_ctrl_name)
                    {

                        int substart = 1;
                        if (PublicClass.is_draw_type == "polyline" || PublicClass.is_draw_type == "polygon")
                        {
                            substart = 3;
                        }

                        if (newshapes.shapes_type == PublicClass.is_draw_type && PublicClass.is_draw_type != "work")
                        {
                            if (int.Parse(newshapes.shapes_name.Substring(substart, newshapes.shapes_name.Length - substart)) > int.Parse(shapes_name))
                            {
                                shapes_name = newshapes.shapes_name.Substring(substart, newshapes.shapes_name.Length - substart);
                            }

                        }

                    }

                }




            }

            if (PublicClass.is_draw_type == "rectangle")
            {
                shapes_name = "R" + (int.Parse(shapes_name) + 1);
            }
            else if (PublicClass.is_draw_type == "ellipse")
            {
                shapes_name = "C" + (int.Parse(shapes_name) + 1);
            }
            else if (PublicClass.is_draw_type == "line")
            {
                shapes_name = "L" + (int.Parse(shapes_name) + 1);
            }
            else if (PublicClass.is_draw_type == "point")
            {
                shapes_name = "P" + (int.Parse(shapes_name) + 1);
            }
            else if (PublicClass.is_draw_type == "polyline")
            {
                shapes_name = "Cur" + (int.Parse(shapes_name) + 1);
            }
            else if (PublicClass.is_draw_type == "polygon")
            {
                shapes_name = "Pol" + (int.Parse(shapes_name) + 1);
            }
            if (PublicClass.is_draw_type != "adjust")
            {
                shapes_active = shapes_name;
            }
        
        }


        if (is_mousedown)
        {
            canvas_font_mouseup();
        }
        if (PublicClass.is_draw_type!="polyline" && PublicClass.is_draw_type!="polygon")
        {
            vertex_coordinate.Clear();
        }
        
        if (PublicClass.is_draw_type != "adjust")
        {
            System.Windows.Shapes.Path deletepath = ir_canvas_font.FindName(shapes_name) as System.Windows.Shapes.Path;
            if (deletepath != null)
            {
                ir_canvas_font.Children.Remove(deletepath);
                ir_canvas_font.UnregisterName(shapes_name);
            }
        }



  

        Point p1 = new Point();
        Point p2 = new Point();
        p1.X = drawpoint.X;
        p1.Y = drawpoint.Y;
        if (e.GetPosition(ir_canvas_font).X - drawpoint.X < 0)
        {
            p1.X = p1.X - (drawpoint.X - e.GetPosition(ir_canvas_font).X);
        }
        if (e.GetPosition(ir_canvas_font).Y - drawpoint.Y < 0)
        {
            p1.Y = p1.Y - (drawpoint.Y - e.GetPosition(ir_canvas_font).Y);
        }
        if (PublicClass.is_draw_type == "rectangle")
        {
            RectangleGeometry newrect = new RectangleGeometry();
            newrect.Rect = new Rect((double)p1.X, (double)p1.Y, (double)Math.Abs(e.GetPosition(ir_canvas_font).X - drawpoint.X), (double)Math.Abs(e.GetPosition(ir_canvas_font).Y - drawpoint.Y));
            dynamic_shapes(newrect, "rectangle",shapes_name+"f",false);
            RectangleGeometry newrectb = new RectangleGeometry();
            newrectb.Rect = new Rect((double)p1.X, (double)p1.Y, (double)Math.Abs(e.GetPosition(ir_canvas_font).X - drawpoint.X), (double)Math.Abs(e.GetPosition(ir_canvas_font).Y - drawpoint.Y));
            dynamic_shapes(newrectb, "rectangle", shapes_name + "b",false);
            Point txtpoint = p1;
            txtpoint.X -= 5;
            txtpoint.Y -= 15;
            if (txtpoint.X < 10)
            {
                txtpoint.X = 10;
            }
            if (txtpoint.Y < 10)
            {
                txtpoint.Y = 10;
            }
            dynamic_txt(txtpoint,shapes_name);
            p2.X = Math.Abs(e.GetPosition(ir_canvas_font).X - drawpoint.X);
            p2.Y = Math.Abs(e.GetPosition(ir_canvas_font).Y - drawpoint.Y);
            vertex_coordinate.Add(p1.X / zoom_coe);
            vertex_coordinate.Add(p1.Y / zoom_coe);
            vertex_coordinate.Add(p2.X / zoom_coe);
            vertex_coordinate.Add(p2.Y / zoom_coe);
        }
        else if (PublicClass.is_draw_type == "ellipse")
        {
            EllipseGeometry newellipse = new EllipseGeometry();
            //newellipse.Center = new Point(e.GetPosition(ir_canvas_font).X- p1.X+e.GetPosition(ir_canvas_font).X/2,e.GetPosition(ir_canvas_font).Y- p1.Y+e.GetPosition(ir_canvas_font).Y/2);
            newellipse.Center = new Point(drawpoint.X+(e.GetPosition(ir_canvas_font).X-drawpoint.X)/2, drawpoint.Y+(e.GetPosition(ir_canvas_font).Y-drawpoint.Y)/2);
            newellipse.RadiusX = (double)Math.Abs(e.GetPosition(ir_canvas_font).X - drawpoint.X)/2;
            newellipse.RadiusY = (double)Math.Abs(e.GetPosition(ir_canvas_font).Y - drawpoint.Y)/2;
            
            Point txtpoint = p1;
            txtpoint.X = newellipse.Center.X-newellipse.RadiusX-5;
            txtpoint.Y = newellipse.Center.Y-newellipse.RadiusY-15;
            if (txtpoint.X < 10)
            {
                txtpoint.X = 10;
            }
            if (txtpoint.Y < 10)
            {
                txtpoint.Y = 10;
            }
            dynamic_shapes(newellipse, "ellipse", shapes_name+"f",false);

            EllipseGeometry newellipseb = new EllipseGeometry();
            //newellipse.Center = new Point(e.GetPosition(ir_canvas_font).X- p1.X+e.GetPosition(ir_canvas_font).X/2,e.GetPosition(ir_canvas_font).Y- p1.Y+e.GetPosition(ir_canvas_font).Y/2);
            newellipseb.Center = new Point(drawpoint.X + (e.GetPosition(ir_canvas_font).X - drawpoint.X) / 2, drawpoint.Y + (e.GetPosition(ir_canvas_font).Y - drawpoint.Y) / 2);
            newellipseb.RadiusX = (double)Math.Abs(e.GetPosition(ir_canvas_font).X - drawpoint.X) / 2;
            newellipseb.RadiusY = (double)Math.Abs(e.GetPosition(ir_canvas_font).Y - drawpoint.Y) / 2;
            dynamic_shapes(newellipseb, "ellipse", shapes_name+"b",false);

            dynamic_txt(txtpoint,shapes_name);
            vertex_coordinate.Add(newellipse.Center.X / zoom_coe);
            vertex_coordinate.Add(newellipse.Center.Y / zoom_coe);
            vertex_coordinate.Add(newellipse.RadiusX / zoom_coe);
            vertex_coordinate.Add(newellipse.RadiusY / zoom_coe);
        }
        else if (PublicClass.is_draw_type == "line")
        {
            LineGeometry newline = new LineGeometry();
            newline.StartPoint = new Point(drawpoint.X,drawpoint.Y);
            newline.EndPoint = new Point(e.GetPosition(ir_canvas_font).X,e.GetPosition(ir_canvas_font).Y);
            
            Point txtpoint = p1;
            txtpoint.X = drawpoint.X-5;
            txtpoint.Y = drawpoint.Y - 15;
            if (txtpoint.X < 10)
            {
                txtpoint.X = 10;
            }
            if (txtpoint.Y < 10)
            {
                txtpoint.Y = 10;
            }

            
            dynamic_shapes(newline, "line", shapes_name+"f",false);

            LineGeometry newlineb = new LineGeometry();
            newlineb.StartPoint = new Point(drawpoint.X, drawpoint.Y);
            newlineb.EndPoint = new Point(e.GetPosition(ir_canvas_font).X, e.GetPosition(ir_canvas_font).Y);
            dynamic_shapes(newlineb, "line", shapes_name+"b",false);

            dynamic_txt(txtpoint, shapes_name);
            vertex_coordinate.Add(newline.StartPoint.X / zoom_coe);
            vertex_coordinate.Add(newline.StartPoint.Y / zoom_coe);
            vertex_coordinate.Add(newline.EndPoint.X / zoom_coe);
            vertex_coordinate.Add(newline.EndPoint.Y / zoom_coe);
        }

        if (PublicClass.is_draw_type == "adjust" && public_shapes_index >= 0)
        {
            List<double> temp_vertex_X = new List<double>();
            List<double> temp_vertex_Y = new List<double>();
            double max_X;
            double min_X;
            double max_Y;
            double min_Y;
            double ex = e.GetPosition(ir_canvas_font).X;
            double ey = e.GetPosition(ir_canvas_font).Y;

            PublicClass.shapes_property newshpes = (PublicClass.shapes_property)PublicClass.shapes_count[public_shapes_index];
            if (newshpes.shapes_type == "line")
            {


                LineGeometry newline = new LineGeometry();
                if (lock_vertex == 0)
                {
                    newline.StartPoint = new Point(e.GetPosition(ir_canvas_font).X, e.GetPosition(ir_canvas_font).Y);
                    newline.EndPoint = new Point(newshpes.vertex[2] * zoom_coe, newshpes.vertex[3] * zoom_coe);
                }
                else if (lock_vertex == 2)
                {
                    newline.StartPoint = new Point(newshpes.vertex[0] * zoom_coe, newshpes.vertex[1] * zoom_coe);
                    newline.EndPoint = new Point(e.GetPosition(ir_canvas_font).X, e.GetPosition(ir_canvas_font).Y);
                }
                else if (lock_vertex == -1)
                {
                    temp_vertex_X.Add(newshpes.vertex[0] * zoom_coe + e.GetPosition(ir_canvas_font).X - drawpoint.X);
                    temp_vertex_X.Add(newshpes.vertex[2] * zoom_coe + e.GetPosition(ir_canvas_font).X - drawpoint.X);
                    temp_vertex_Y.Add(newshpes.vertex[1] * zoom_coe + e.GetPosition(ir_canvas_font).Y - drawpoint.Y);
                    temp_vertex_Y.Add(newshpes.vertex[3] * zoom_coe + e.GetPosition(ir_canvas_font).Y - drawpoint.Y);
                    max_X = (from c in temp_vertex_X select c).Max();
                    min_X = (from c in temp_vertex_X select c).Min();
                    max_Y = (from c in temp_vertex_Y select c).Max();
                    min_Y = (from c in temp_vertex_Y select c).Min();
                    if (max_X > ir_width * zoom_coe)
                    {
                        ex = e.GetPosition(ir_canvas_font).X - (max_X - ir_width * zoom_coe);
                    }
                    if (min_X < 0)
                    {
                        ex = e.GetPosition(ir_canvas_font).X - min_X;
                    }
                    if (max_Y > ir_height * zoom_coe)
                    {
                        ey = e.GetPosition(ir_canvas_font).Y - (max_Y - ir_height * zoom_coe);
                    }
                    if (min_Y < 0)
                    {
                        ey = e.GetPosition(ir_canvas_font).Y - min_Y;
                    }


                    newline.StartPoint = new Point(newshpes.vertex[0] * zoom_coe + ex - drawpoint.X, newshpes.vertex[1] * zoom_coe + ey - drawpoint.Y);
                    newline.EndPoint = new Point(newshpes.vertex[2] * zoom_coe + ex - drawpoint.X, newshpes.vertex[3] * zoom_coe + ey - drawpoint.Y);
                    

                }

                Point txtpoint = newline.StartPoint;
                txtpoint.X = txtpoint.X - 5;
                txtpoint.Y = txtpoint.Y - 15;
                if (txtpoint.X < 10)
                {
                    txtpoint.X = 10;
                }
                if (txtpoint.Y < 10)
                {
                    txtpoint.Y = 10;
                }


                if (lock_vertex > -10)
                {
                    System.Windows.Shapes.Path cur_deletepath = ir_canvas_font.FindName(newshpes.shapes_name) as System.Windows.Shapes.Path;
                    if (cur_deletepath != null)
                    {
                        ir_canvas_font.Children.Remove(cur_deletepath);
                        ir_canvas_font.UnregisterName(newshpes.shapes_name);
                    }
                    shapes_name = newshpes.shapes_name;
                    shapes_active = newshpes.shapes_name;
                    dynamic_shapes(newline, "line", newshpes.shapes_name,false);
                    dynamic_txt(txtpoint, newshpes.shapes_name);
                    vertex_coordinate.Add(newline.StartPoint.X / zoom_coe);
                    vertex_coordinate.Add(newline.StartPoint.Y / zoom_coe);
                    vertex_coordinate.Add(newline.EndPoint.X / zoom_coe);
                    vertex_coordinate.Add(newline.EndPoint.Y / zoom_coe);
                    

                }
            }
            else if (newshpes.shapes_type == "ellipse")
            {

                EllipseGeometry newellipse = new EllipseGeometry();
                if(lock_vertex==-1)
                {

                    temp_vertex_X.Add(newshpes.vertex[0] * zoom_coe + e.GetPosition(ir_canvas_font).X - drawpoint.X - newshpes.vertex[2] * zoom_coe);
                    temp_vertex_X.Add(newshpes.vertex[0] * zoom_coe + e.GetPosition(ir_canvas_font).X - drawpoint.X);
                    temp_vertex_X.Add(newshpes.vertex[0] * zoom_coe + e.GetPosition(ir_canvas_font).X - drawpoint.X + newshpes.vertex[2] * zoom_coe);
                    temp_vertex_Y.Add(newshpes.vertex[1] * zoom_coe + e.GetPosition(ir_canvas_font).Y - drawpoint.Y - newshpes.vertex[3] * zoom_coe);
                    temp_vertex_Y.Add(newshpes.vertex[1] * zoom_coe + e.GetPosition(ir_canvas_font).Y - drawpoint.Y );
                    temp_vertex_Y.Add(newshpes.vertex[1] * zoom_coe + e.GetPosition(ir_canvas_font).Y - drawpoint.Y + newshpes.vertex[3] * zoom_coe);
                    max_X = (from c in temp_vertex_X select c).Max();
                    min_X = (from c in temp_vertex_X select c).Min();
                    max_Y = (from c in temp_vertex_Y select c).Max();
                    min_Y = (from c in temp_vertex_Y select c).Min();
                    if (max_X > ir_width * zoom_coe)
                    {
                        ex = e.GetPosition(ir_canvas_font).X - (max_X - ir_width * zoom_coe);
                    }
                    if (min_X < 0)
                    {
                        ex = e.GetPosition(ir_canvas_font).X - min_X;
                    }
                    if (max_Y > ir_height * zoom_coe)
                    {
                        ey = e.GetPosition(ir_canvas_font).Y - (max_Y - ir_height * zoom_coe);
                    }
                    if (min_Y < 0)
                    {
                        ey = e.GetPosition(ir_canvas_font).Y - min_Y;
                    }

                    newellipse.Center = new Point(newshpes.vertex[0] * zoom_coe + ex - drawpoint.X, newshpes.vertex[1] * zoom_coe + ey - drawpoint.Y);
                    newellipse.RadiusX = newshpes.vertex[2]*zoom_coe;
                    newellipse.RadiusY = newshpes.vertex[3]*zoom_coe;
                }
                else if (lock_vertex == 0)
                {
                    newellipse.Center = new Point(newshpes.vertex[0] * zoom_coe + (e.GetPosition(ir_canvas_font).X - drawpoint.X) / 2, newshpes.vertex[1] * zoom_coe);
                    newellipse.RadiusX = newshpes.vertex[2] * zoom_coe + (drawpoint.X - e.GetPosition(ir_canvas_font).X) / 2;
                    newellipse.RadiusY = newshpes.vertex[3] * zoom_coe;
                }
                else if (lock_vertex == 1)
                {
                    newellipse.Center = new Point(newshpes.vertex[0] * zoom_coe, newshpes.vertex[1] * zoom_coe + (e.GetPosition(ir_canvas_font).Y - drawpoint.Y) / 2);
                    newellipse.RadiusX = newshpes.vertex[2] * zoom_coe;
                    newellipse.RadiusY = newshpes.vertex[3] * zoom_coe + (drawpoint.Y - e.GetPosition(ir_canvas_font).Y) / 2;
                }
                else if (lock_vertex == 2)
                {
                    newellipse.Center = new Point(newshpes.vertex[0] * zoom_coe + (e.GetPosition(ir_canvas_font).X - drawpoint.X) / 2, newshpes.vertex[1] * zoom_coe);
                    newellipse.RadiusX = newshpes.vertex[2] * zoom_coe + (e.GetPosition(ir_canvas_font).X - drawpoint.X) / 2;
                    newellipse.RadiusY = newshpes.vertex[3] * zoom_coe;
                }
                else if (lock_vertex == 3)
                {
                    newellipse.Center = new Point(newshpes.vertex[0] * zoom_coe, newshpes.vertex[1] * zoom_coe + (e.GetPosition(ir_canvas_font).Y - drawpoint.Y) / 2);
                    newellipse.RadiusX = newshpes.vertex[2] * zoom_coe;
                    newellipse.RadiusY = newshpes.vertex[3] * zoom_coe + (e.GetPosition(ir_canvas_font).Y - drawpoint.Y) / 2;
                }
                
               
                Point txtpoint = p1;
                txtpoint.X = newellipse.Center.X - newellipse.RadiusX - 5;
                txtpoint.Y = newellipse.Center.Y - newellipse.RadiusY - 15;
                if (txtpoint.X < 10)
                {
                    txtpoint.X = 10;
                }
                if (txtpoint.Y < 10)
                {
                    txtpoint.Y = 10;
                }
                if (lock_vertex > -10)
                {
                    System.Windows.Shapes.Path cur_deletepath = ir_canvas_font.FindName(newshpes.shapes_name) as System.Windows.Shapes.Path;
                    if (cur_deletepath != null)
                    {
                        ir_canvas_font.Children.Remove(cur_deletepath);
                        ir_canvas_font.UnregisterName(newshpes.shapes_name);
                    }
                    shapes_name = newshpes.shapes_name;
                    shapes_active = newshpes.shapes_name;
                    dynamic_shapes(newellipse, "ellipse", newshpes.shapes_name,true);
                    dynamic_txt(txtpoint, newshpes.shapes_name);
                    vertex_coordinate.Add(newellipse.Center.X / zoom_coe);
                    vertex_coordinate.Add(newellipse.Center.Y / zoom_coe);
                    vertex_coordinate.Add(newellipse.RadiusX / zoom_coe);
                    vertex_coordinate.Add(newellipse.RadiusY / zoom_coe);
                }
            }
            else if (newshpes.shapes_type == "rectangle")
            {

                RectangleGeometry newrect = new RectangleGeometry();
                if (lock_vertex == -1)
                {
                    temp_vertex_X.Add(newshpes.vertex[0] * zoom_coe + e.GetPosition(ir_canvas_font).X - drawpoint.X);
                    temp_vertex_X.Add(newshpes.vertex[0] * zoom_coe + e.GetPosition(ir_canvas_font).X - drawpoint.X + newshpes.vertex[2] * zoom_coe);
                    temp_vertex_Y.Add(newshpes.vertex[1] * zoom_coe + e.GetPosition(ir_canvas_font).Y - drawpoint.Y);
                    temp_vertex_Y.Add(newshpes.vertex[1] * zoom_coe + e.GetPosition(ir_canvas_font).Y - drawpoint.Y + newshpes.vertex[3] * zoom_coe);
                    max_X = (from c in temp_vertex_X select c).Max();
                    min_X = (from c in temp_vertex_X select c).Min();
                    max_Y = (from c in temp_vertex_Y select c).Max();
                    min_Y = (from c in temp_vertex_Y select c).Min();
                    if (max_X > ir_width * zoom_coe)
                    {
                        ex = e.GetPosition(ir_canvas_font).X - (max_X - ir_width * zoom_coe);
                    }
                    if (min_X < 0)
                    {
                        ex = e.GetPosition(ir_canvas_font).X - min_X;
                    }
                    if (max_Y > ir_height * zoom_coe)
                    {
                        ey = e.GetPosition(ir_canvas_font).Y - (max_Y - ir_height * zoom_coe);
                    }
                    if (min_Y < 0)
                    {
                        ey = e.GetPosition(ir_canvas_font).Y - min_Y;
                    }


                    newrect.Rect = new Rect(newshpes.vertex[0] * zoom_coe + ex - drawpoint.X, newshpes.vertex[1] * zoom_coe + ey - drawpoint.Y, newshpes.vertex[2] * zoom_coe, newshpes.vertex[3] * zoom_coe);
                }
                else if (lock_vertex == 0)
                {
                    Point rectpoint = new Point(newshpes.vertex[0] * zoom_coe + e.GetPosition(ir_canvas_font).X - drawpoint.X, newshpes.vertex[1] * zoom_coe + e.GetPosition(ir_canvas_font).Y - drawpoint.Y);
                    if (rectpoint.X > newshpes.vertex[0] * zoom_coe + newshpes.vertex[2] * zoom_coe)
                    {
                        rectpoint.X = newshpes.vertex[0] * zoom_coe + newshpes.vertex[2] * zoom_coe;
                    }
                    if (rectpoint.Y > newshpes.vertex[1] * zoom_coe + newshpes.vertex[3] * zoom_coe)
                    {
                        rectpoint.Y = newshpes.vertex[1] * zoom_coe + newshpes.vertex[3] * zoom_coe;
                    }
                    newrect.Rect = new Rect(rectpoint.X, rectpoint.Y, Math.Abs(newshpes.vertex[2] * zoom_coe + drawpoint.X - e.GetPosition(ir_canvas_font).X), Math.Abs(newshpes.vertex[3] * zoom_coe + drawpoint.Y - e.GetPosition(ir_canvas_font).Y));
                }
                else if (lock_vertex == 1)
                {
                    Point rectpoint = new Point(newshpes.vertex[0] * zoom_coe, e.GetPosition(ir_canvas_font).Y);
                    if (rectpoint.X > e.GetPosition(ir_canvas_font).X)
                    {
                        rectpoint.X = e.GetPosition(ir_canvas_font).X;
                    }
                    if (rectpoint.Y > newshpes.vertex[1] * zoom_coe + newshpes.vertex[3] * zoom_coe)
                    {
                        rectpoint.Y = newshpes.vertex[1] * zoom_coe + newshpes.vertex[3] * zoom_coe;
                    }
                    newrect.Rect = new Rect(rectpoint.X, rectpoint.Y, Math.Abs(e.GetPosition(ir_canvas_font).X-newshpes.vertex[0]*zoom_coe), Math.Abs(newshpes.vertex[3] * zoom_coe + drawpoint.Y - e.GetPosition(ir_canvas_font).Y));
                }
                else if (lock_vertex == 2)
                {
                    Point rectpoint = new Point(newshpes.vertex[0] * zoom_coe, newshpes.vertex[1]*zoom_coe);
                    if (rectpoint.X > e.GetPosition(ir_canvas_font).X)
                    {
                        rectpoint.X = e.GetPosition(ir_canvas_font).X;
                    }
                    if (rectpoint.Y > e.GetPosition(ir_canvas_font).Y)
                    {
                        rectpoint.Y = e.GetPosition(ir_canvas_font).Y;
                    }
                    newrect.Rect = new Rect(rectpoint.X, rectpoint.Y, Math.Abs(e.GetPosition(ir_canvas_font).X - newshpes.vertex[0] * zoom_coe), Math.Abs(e.GetPosition(ir_canvas_font).Y - newshpes.vertex[1] * zoom_coe));
                }
                else if (lock_vertex == 3)
                {
                    Point rectpoint = new Point(newshpes.vertex[0] * zoom_coe+e.GetPosition(ir_canvas_font).X-newshpes.vertex[0]*zoom_coe, newshpes.vertex[1] * zoom_coe);
                    if (rectpoint.X > newshpes.vertex[0] * zoom_coe + newshpes.vertex[2] * zoom_coe)
                    {
                        rectpoint.X = newshpes.vertex[0] * zoom_coe + newshpes.vertex[2] * zoom_coe;
                    }
                    if (rectpoint.Y > e.GetPosition(ir_canvas_font).Y)
                    {
                        rectpoint.Y = e.GetPosition(ir_canvas_font).Y;
                    }
                    newrect.Rect = new Rect(rectpoint.X, rectpoint.Y, Math.Abs(newshpes.vertex[0] * zoom_coe+newshpes.vertex[2]*zoom_coe-e.GetPosition(ir_canvas_font).X), Math.Abs(e.GetPosition(ir_canvas_font).Y - newshpes.vertex[1] * zoom_coe));
                }
                else if (lock_vertex == 4)
                {
                    Point rectpoint = new Point(e.GetPosition(ir_canvas_font).X, newshpes.vertex[1] * zoom_coe);
                    if (rectpoint.X > newshpes.vertex[0] * zoom_coe + newshpes.vertex[2] * zoom_coe)
                    {
                        rectpoint.X = newshpes.vertex[0] * zoom_coe + newshpes.vertex[2] * zoom_coe;
                    }

                    newrect.Rect = new Rect(rectpoint.X, rectpoint.Y, Math.Abs(newshpes.vertex[0] * zoom_coe + newshpes.vertex[2] * zoom_coe - e.GetPosition(ir_canvas_font).X), newshpes.vertex[3]*zoom_coe);
                }
                else if (lock_vertex == 5)
                {
                    Point rectpoint = new Point(newshpes.vertex[0] * zoom_coe, e.GetPosition(ir_canvas_font).Y);
                    if (rectpoint.Y > newshpes.vertex[1] * zoom_coe + newshpes.vertex[3] * zoom_coe)
                    {
                        rectpoint.Y = newshpes.vertex[1] * zoom_coe + newshpes.vertex[3] * zoom_coe;
                    }

                    newrect.Rect = new Rect(rectpoint.X, rectpoint.Y, newshpes.vertex[2] * zoom_coe, Math.Abs(newshpes.vertex[1] * zoom_coe + newshpes.vertex[3] * zoom_coe - e.GetPosition(ir_canvas_font).Y));
                }
                else if (lock_vertex == 6)
                {
                    Point rectpoint = new Point(newshpes.vertex[0] * zoom_coe, newshpes.vertex[1] * zoom_coe);
                    if (rectpoint.X > e.GetPosition(ir_canvas_font).X)
                    {
                        rectpoint.X = e.GetPosition(ir_canvas_font).X;
                    }

                    newrect.Rect = new Rect(rectpoint.X, rectpoint.Y, Math.Abs(e.GetPosition(ir_canvas_font).X - newshpes.vertex[0] * zoom_coe), newshpes.vertex[3] * zoom_coe);
                }
                else if (lock_vertex == 7)
                {
                    Point rectpoint = new Point(newshpes.vertex[0] * zoom_coe, newshpes.vertex[1] * zoom_coe);
                    if (rectpoint.Y > e.GetPosition(ir_canvas_font).Y)
                    {
                        rectpoint.Y = e.GetPosition(ir_canvas_font).Y;
                    }

                    newrect.Rect = new Rect(rectpoint.X, rectpoint.Y, newshpes.vertex[2] * zoom_coe, Math.Abs(e.GetPosition(ir_canvas_font).Y - newshpes.vertex[1] * zoom_coe));
                }


                Point txtpoint = p1;
                txtpoint.X = newrect.Rect.X - 5;
                txtpoint.Y = newrect.Rect.Y - 15;
                if (txtpoint.X < 10)
                {
                    txtpoint.X = 10;
                }
                if (txtpoint.Y < 10)
                {
                    txtpoint.Y = 10;
                }
                if (lock_vertex > -10)
                {
                    System.Windows.Shapes.Path cur_deletepath = ir_canvas_font.FindName(newshpes.shapes_name) as System.Windows.Shapes.Path;
                    if (cur_deletepath != null)
                    {
                        ir_canvas_font.Children.Remove(cur_deletepath);
                        ir_canvas_font.UnregisterName(newshpes.shapes_name);
                    }
                    shapes_name = newshpes.shapes_name;
                    shapes_active = newshpes.shapes_name;
                    
                    dynamic_shapes(newrect, "rectangle", newshpes.shapes_name,true);
                    dynamic_txt(txtpoint, newshpes.shapes_name);
                    vertex_coordinate.Add(newrect.Rect.X / zoom_coe);
                    vertex_coordinate.Add(newrect.Rect.Y / zoom_coe);
                    vertex_coordinate.Add(newrect.Rect.Width / zoom_coe);
                    vertex_coordinate.Add(newrect.Rect.Height / zoom_coe);
                }
            }

            else if (newshpes.shapes_type == "polygon")
            {
                collection_ing.Clear();
                Polygon newpolygon = new Polygon();
                if (lock_vertex == -1)
                {

                      for (int i = 0; i < newshpes.vertex.Count - 1; i = i + 2)
                    {
                    temp_vertex_X.Add(newshpes.vertex[i] * zoom_coe + e.GetPosition(ir_canvas_font).X - drawpoint.X);
                    temp_vertex_Y.Add(newshpes.vertex[i + 1] * zoom_coe + e.GetPosition(ir_canvas_font).Y - drawpoint.Y);
                    }
                    max_X = (from c in temp_vertex_X select c).Max();
                    min_X = (from c in temp_vertex_X select c).Min();
                    max_Y = (from c in temp_vertex_Y select c).Max();
                    min_Y = (from c in temp_vertex_Y select c).Min();
                    if (max_X > ir_width * zoom_coe)
                    {
                        ex = e.GetPosition(ir_canvas_font).X - (max_X - ir_width * zoom_coe);
                    }
                    if (min_X < 0)
                    {
                        ex = e.GetPosition(ir_canvas_font).X - min_X;
                    }
                    if (max_Y > ir_height * zoom_coe)
                    {
                        ey = e.GetPosition(ir_canvas_font).Y - (max_Y - ir_height * zoom_coe);
                    }
                    if (min_Y < 0)
                    {
                        ey = e.GetPosition(ir_canvas_font).Y - min_Y;
                    }
                    for (int i = 0; i < newshpes.vertex.Count - 1; i = i + 2)
                    {
                        
                        collection_ing.Add(new Point(newshpes.vertex[i] * zoom_coe +ex - drawpoint.X, newshpes.vertex[i + 1] * zoom_coe + ey - drawpoint.Y));
                    }

                    newpolygon.Points = collection_ing;
                  
                    
                }
                else if (lock_vertex > -1)
                {

                    for (int i = 0; i < newshpes.vertex.Count - 1; i = i + 2)
                    {
                        if (i == lock_vertex)
                        {
                            collection_ing.Add(new Point(e.GetPosition(ir_canvas_font).X,e.GetPosition(ir_canvas_font).Y));
                        }
                        else
                        {
                            collection_ing.Add( new Point( newshpes.vertex[i]*zoom_coe,newshpes.vertex[i+1]*zoom_coe));
                        }
                    }
                    newpolygon.Points = collection_ing;
                }


                if (lock_vertex > -10)
                {
                    System.Windows.Shapes.Path cur_deletepath = ir_canvas_font.FindName(newshpes.shapes_name) as System.Windows.Shapes.Path;
                    if (cur_deletepath != null)
                    {
                        ir_canvas_font.Children.Remove(cur_deletepath);
                        ir_canvas_font.UnregisterName(newshpes.shapes_name);
                    }
                    shapes_name = newshpes.shapes_name;
                    shapes_active = newshpes.shapes_name;
                    dynamic_shapes(newpolygon, "polygon", newshpes.shapes_name,true);

                    Point txtpoint = p1;
                    txtpoint.X = collection_ing[0].X - 5;
                    txtpoint.Y = collection_ing[0].Y - 15;
                    if (txtpoint.X < 10)
                    {
                        txtpoint.X = 10;
                    }
                    if (txtpoint.Y < 10)
                    {
                        txtpoint.Y = 10;
                    }

                    dynamic_txt(txtpoint, newshpes.shapes_name);
                    for (int i = 0; i < collection_ing.Count; i++)
                    {
                        vertex_coordinate.Add(collection_ing[i].X/zoom_coe);
                        vertex_coordinate.Add(collection_ing[i].Y/zoom_coe);
                    }
                   
                }
          
            }

            else if (newshpes.shapes_type == "polyline")
            {
                collection_ing.Clear();
                Polyline newpolyline = new Polyline();
                if (lock_vertex == -1)
                {

                    for (int i = 0; i < newshpes.vertex.Count - 1; i = i + 2)
                    {
                        temp_vertex_X.Add(newshpes.vertex[i] * zoom_coe + e.GetPosition(ir_canvas_font).X - drawpoint.X);
                        temp_vertex_Y.Add(newshpes.vertex[i + 1] * zoom_coe + e.GetPosition(ir_canvas_font).Y - drawpoint.Y);
                    }
                    max_X = (from c in temp_vertex_X select c).Max();
                    min_X = (from c in temp_vertex_X select c).Min();
                    max_Y = (from c in temp_vertex_Y select c).Max();
                    min_Y = (from c in temp_vertex_Y select c).Min();
                    if (max_X > ir_width * zoom_coe)
                    {
                        ex = e.GetPosition(ir_canvas_font).X - (max_X - ir_width * zoom_coe);
                    }
                    if (min_X < 0)
                    {
                        ex = e.GetPosition(ir_canvas_font).X - min_X;
                    }
                    if (max_Y > ir_height * zoom_coe)
                    {
                        ey = e.GetPosition(ir_canvas_font).Y - (max_Y - ir_height * zoom_coe);
                    }
                    if (min_Y < 0)
                    {
                        ey = e.GetPosition(ir_canvas_font).Y - min_Y;
                    }


                    for (int i = 0; i < newshpes.vertex.Count - 1; i = i + 2)
                    {
                        collection_ing.Add(new Point(newshpes.vertex[i] * zoom_coe + ex - drawpoint.X, newshpes.vertex[i + 1] * zoom_coe + ey - drawpoint.Y));
                    }
                    newpolyline.Points = collection_ing;
                }
                else if(lock_vertex>-1)
                {
                    for (int i = 0; i < newshpes.vertex.Count - 1; i = i + 2)
                    {
                        if (i == lock_vertex)
                        {
                            collection_ing.Add(new Point(e.GetPosition(ir_canvas_font).X, e.GetPosition(ir_canvas_font).Y));
                        }
                        else
                        {
                            collection_ing.Add(new Point(newshpes.vertex[i] * zoom_coe, newshpes.vertex[i + 1] * zoom_coe));
                        }
                    }
                    newpolyline.Points = collection_ing;
                }
              
                if (lock_vertex > -10)
                {
                    System.Windows.Shapes.Path cur_deletepath = ir_canvas_font.FindName(newshpes.shapes_name) as System.Windows.Shapes.Path;
                    if (cur_deletepath != null)
                    {
                        ir_canvas_font.Children.Remove(cur_deletepath);
                        ir_canvas_font.UnregisterName(newshpes.shapes_name);
                    }
                    shapes_name = newshpes.shapes_name;
                    shapes_active = newshpes.shapes_name;

                    Point txtpoint = p1;
                    txtpoint.X = collection_ing[0].X - 5;
                    txtpoint.Y = collection_ing[0].Y - 15;
                    if (txtpoint.X < 10)
                    {
                        txtpoint.X = 10;
                    }
                    if (txtpoint.Y < 10)
                    {
                        txtpoint.Y = 10;
                    }

                    dynamic_shapes(newpolyline, "polyline", newshpes.shapes_name,false);
                    dynamic_txt(txtpoint, newshpes.shapes_name);
                    for (int i = 0; i < collection_ing.Count; i++)
                    {
                        vertex_coordinate.Add(collection_ing[i].X / zoom_coe);
                        vertex_coordinate.Add(collection_ing[i].Y / zoom_coe);
                    }

                }
            }


            else if (newshpes.shapes_type == "point")
            {
                if (lock_vertex == 0)
                {
                    double tx = e.GetPosition(ir_canvas_font).X;
                    double ty = e.GetPosition(ir_canvas_font).Y;
                    shapes_name = newshpes.shapes_name;
                    shapes_active = newshpes.shapes_name;
                    for (int i1 = 1; i1 < 3; i1++)//删除点
                    {
                        System.Windows.Shapes.Path deletepath = ir_canvas_font.FindName(newshpes.shapes_name + i1.ToString()) as System.Windows.Shapes.Path;
                        if (deletepath != null)
                        {
                            ir_canvas_font.Children.Remove(deletepath);
                            ir_canvas_font.UnregisterName(newshpes.shapes_name + i1.ToString());
                        }
                    }


                    LineGeometry linex = new LineGeometry();
                    LineGeometry liney = new LineGeometry();
                    linex.StartPoint = new Point(tx - 9, ty);
                    linex.EndPoint = new Point(tx + 9, ty);
                    liney.StartPoint = new Point(tx, ty - 9);
                    liney.EndPoint = new Point(tx, ty + 9);

                    //System.Windows.Shapes.Path mypathx = new System.Windows.Shapes.Path();
                    //mypathx.Stroke = Brushes.White;
                    //mypathx.Stroke = new SolidColorBrush(Color.FromRgb((byte)51, (byte)102, (byte)255));
                    //mypathx.StrokeThickness = 1;
                    //mypathx.Data = linex;
                    //mypathx.SnapsToDevicePixels = true;
                    if (tx - 10 < 0)
                    {
                        linex.StartPoint = new Point(tx + 3, ty);

                    }
                    else if (tx + 10 > ir_width * zoom_coe)
                    {
                        linex.EndPoint = new Point(tx - 3, ty);

                    }
                    //else
                    //{

                    //    mypathx.StrokeDashArray = new DoubleCollection(new double[2] { 6, 6 });
                    //}
                    //ir_canvas_font.Children.Add(mypathx);
                    //ir_canvas_font.RegisterName(shapes_name + "1", mypathx);
                    //mypathx.MouseUp += new MouseButtonEventHandler(ir_canvas_font_MouseUp);
                    //mypathx.MouseDown += new MouseButtonEventHandler(ir_canvas_font_MouseDown);
                    //mypathx.MouseMove += new MouseEventHandler(ir_canvas_font_MouseMove);
                    dynamic_shapes(linex, "line", newshpes.shapes_name + "1", false);

                    //System.Windows.Shapes.Path mypathy = new System.Windows.Shapes.Path();
                    //mypathy.Stroke = Brushes.White;
                    //mypathy.Stroke = new SolidColorBrush(Color.FromRgb((byte)51, (byte)102, (byte)255));
                    //mypathy.StrokeThickness = 1;
                    //mypathy.Data = liney;
                    //mypathy.SnapsToDevicePixels = true;
                    if (ty - 10 < 0)
                    {
                        liney.StartPoint = new Point(tx, ty + 3);
                    }
                    else if (ty + 10 > ir_height * zoom_coe)
                    {
                        liney.EndPoint = new Point(tx, ty - 3);
                    }
                    //else
                    //{
                    //    mypathy.StrokeDashArray = new DoubleCollection(new double[2] { 6, 6 });
                    //}
                    //ir_canvas_font.Children.Add(mypathy);
                    //ir_canvas_font.RegisterName(shapes_name + "2", mypathy);
                    //mypathy.MouseUp += new MouseButtonEventHandler(ir_canvas_font_MouseUp);
                    //mypathy.MouseDown += new MouseButtonEventHandler(ir_canvas_font_MouseDown);
                    //mypathy.MouseMove += new MouseEventHandler(ir_canvas_font_MouseMove);
                    
                    dynamic_shapes(liney, "line", newshpes.shapes_name + "2", false);

                    //if (tx * zoom_coe - 8 > 0)
                    //{
                    //    LineGeometry line1 = new LineGeometry();
                    //    line1.StartPoint = new Point(tx  - 8, ty );
                    //    line1.EndPoint = new Point(tx - 3, ty );
                    //    dynamic_shapes(line1, "line", newshpes.shapes_name + "1",false);

                    //}
                    //if (tx * zoom_coe + 8 < ir_width * zoom_coe)
                    //{
                    //    LineGeometry line1 = new LineGeometry();
                    //    line1.StartPoint = new Point(tx  + 3, ty );
                    //    line1.EndPoint = new Point(tx + 8, ty );
                    //    dynamic_shapes(line1, "line", newshpes.shapes_name + "2",false);
                    //}
                    //if (ty * zoom_coe - 8 > 0)
                    //{
                    //    LineGeometry line1 = new LineGeometry();
                    //    line1.StartPoint = new Point(tx , ty  - 8);
                    //    line1.EndPoint = new Point(tx, ty - 3);
                    //    dynamic_shapes(line1, "line", newshpes.shapes_name + "3",false);
                    //}
                    //if (ty * zoom_coe + 8 < ir_height * zoom_coe)
                    //{
                    //    LineGeometry line1 = new LineGeometry();
                    //    line1.StartPoint = new Point(tx , ty  + 8);
                    //    line1.EndPoint = new Point(tx, ty  + 3);
                    //    dynamic_shapes(line1, "line", newshpes.shapes_name + "4",false);
                    //}
                    Point txtpoint = new Point();
                    txtpoint.X = tx  - 10;
                    txtpoint.Y = ty  - 25;
                    if (txtpoint.X < 10)
                    {
                        txtpoint.X = 10;
                    }
                    if (txtpoint.Y < 10)
                    {
                        txtpoint.Y = 10;
                    }
                    TextBlock txt = new TextBlock();
                    txt.Text = newshpes.shapes_name;
                    dynamic_txt(txtpoint, newshpes.shapes_name);
                   
                    vertex_coordinate.Add(e.GetPosition(ir_canvas_font).X/zoom_coe);
                    vertex_coordinate.Add(e.GetPosition(ir_canvas_font).Y/zoom_coe);

                }
            }




        }


        //////////////操作背景图begin////////////////

        //if (PublicClass.is_draw_type == "adjust" && PublicClass.ronghe_type)
        //{
            
        //    //ir_back_img.Margin = new Thickness((e.GetPosition(ir_canvas_font).X - drawpoint.X)+rongheleft,  ronghetop+(e.GetPosition(ir_canvas_font).Y-drawpoint.Y), 0, 0);
        //    //if (e.GetPosition(ir_canvas_font).X - back_canvas.Margin.Right ==0)
        //    //{
        //    //    back_canvas.Cursor = Cursors.SizeWE;
        //    //    lock_vertex = 0;
        //    //}
        //    if (drawpoint.X > ir_back_img.Width + ir_back_img.Margin.Left - 20 && drawpoint.X < ir_back_img.Width + ir_back_img.Margin.Left+100)
        //    {
        //        back_canvas.Cursor = Cursors.SizeWE;
        //        ir_back_img.Width = ir_back_img.Width + (e.GetPosition(ir_canvas_font).X-drawpoint.X);
        //        drawpoint = e.GetPosition(ir_canvas_font);
        //    }
        //    //else if (lock_vertex == 1)
        //    //{
        //    //    ir_back_img.Height = e.GetPosition(back_canvas).Y - drawpoint.Y;
        //    //}
        //    //else if (lock_vertex == 2)
        //    //{
        //    //    ir_back_img.Height = drawpoint.Y-e.GetPosition(back_canvas).Y;
        //    //    ir_back_img.Width = drawpoint.X-e.GetPosition(back_canvas).X;
        //    //}
        //    //else if (lock_vertex == 3)
        //    //{
        //    //    ir_back_img.Width = drawpoint.X - e.GetPosition(back_canvas).X;
        //    //}
           
        //}


       //////////////操作背景图片end////////////////
    }
    else if (PublicClass.is_draw_type == "polyline" && is_collection)
    {
        collection_ing.Clear();
        for (int i = 0; i < collection.Count; i++)
        {
            collection_ing.Add(new Point(collection[i].X * zoom_coe, collection[i].Y * zoom_coe));
        }
        collection_ing.Add(new Point(e.GetPosition(ir_canvas_font).X, e.GetPosition(ir_canvas_font).Y));
        Polyline polylinef = new Polyline();
        polylinef.Points = collection_ing;
        Polyline polylineb = new Polyline();
        polylineb.Points = collection_ing;
        dynamic_shapes(polylinef, "polyline", shapes_name+"f",false);
        dynamic_shapes(polylineb, "polyline", shapes_name+"b",false);
    }

    else if (PublicClass.is_draw_type == "polygon" && is_collection)
    {
        collection_ing.Clear();
      
        for (int i = 0; i < collection.Count; i++)
        {
            collection_ing.Add(new Point(collection[i].X * zoom_coe, collection[i].Y * zoom_coe));
        }
        collection_ing.Add(new Point(e.GetPosition(ir_canvas_font).X, e.GetPosition(ir_canvas_font).Y));
        Polygon newpolygon = new Polygon();
        newpolygon.Points = collection_ing;
        dynamic_shapes(newpolygon, "polygon", shapes_name+"f",false);

        Polygon newpolygonb = new Polygon();
        newpolygonb.Points = collection_ing;
        dynamic_shapes(newpolygonb, "polygon", shapes_name+"b",false);
    }



}


        

private void ir_canvas_font_MouseUp(object sender, MouseButtonEventArgs e)
{
    
    if (!is_mousedown)
    {
        canvas_font_mouseup();
    }
    change_shapes_active(e);
    is_mousedown = false;
}

public void canvas_font_mouseup()
{
          



            //Thread backthread = new Thread(new ThreadStart(() =>
            //{
            //    Dispatcher.Invoke(new Action(() =>
            //    {
    
                    back_work();
                    
                    
            //    }));
            //}));



            
            //backthread.SetApartmentState(ApartmentState.MTA);
            //backthread.IsBackground = true;
            //backthread.Start();

        

}

private void back_work() 
{
    if (!is_collection)
    {
        doubleclick = false;

        BackgroundWorker backtask = new BackgroundWorker();
        backtask.DoWork += new DoWorkEventHandler(backtask_DoWork);
        backtask.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backtask_RunWorkerCompleted);
        backtask.RunWorkerAsync();

    }
}

public void backtask_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
{
    ir_canvas_back.Opacity = 1;
    var rtb = new RenderTargetBitmap(ir_width, ir_height, 96, 96, PixelFormats.Default);
    rtb.Render(ir_canvas_back);
    ir_canvas_back.Opacity = 0;
    //irimg.Source = rtb;
    byte[] data = new byte[rtb.PixelWidth * rtb.PixelHeight*4];
    //string vertex_temp="";
    List<string> tt = new List<string>();

    rtb.CopyPixels(data, rtb.PixelWidth * 4, 0);

    for (int h = 0; h < rtb.PixelHeight; h++)
    {
        for (int w = 0; w < rtb.PixelWidth; w++)
        {
            if (data[(h * rtb.PixelWidth + w) * 4] < 128)
            {
                //vertex_temp += "," + (h * rtb.PixelWidth + w);
                tt.Add((h * rtb.PixelWidth + w).ToString());

            }
        }
    }

    if (tt.Count == 0)
    {
        //canvas_font_mouseup();
    }
    //vertex_temp = string.Join(",", tt.ToArray());
    //vertex_temp = "," + vertex_temp;
    
    //draw_img.Add(vertex_coordinate + vertex_temp);

    if (vertex_coordinate.Count > 0)
    {
        //create_shapes_property(tt);

        create_shapes_property(tt);


        Thread cal_percent = new Thread(new ThreadStart(() =>
        {
            Dispatcher.Invoke(new Action(() =>
            {

                calculate_percent();

            }));
        }));
        cal_percent.SetApartmentState(ApartmentState.MTA);
        cal_percent.IsBackground = true;
        //cal_percent.Priority = ThreadPriority.BelowNormal;
        cal_percent.Start();


    }
    vertex_coordinate.Clear();
    re_create_shapes();

    try
    {
        sub_shapes_list shap = MainWindow.FindChild<sub_shapes_list>(Application.Current.MainWindow, "shapestest");
        shap.shapeslist("insert", 0);
    }
    catch
    { 
    
    }

    //create_vertex_animation();
    RoutedPropertyChangedEventArgs<object> args = new RoutedPropertyChangedEventArgs<object>(this, e);
    args.RoutedEvent = sub_workspace.WorkMouseUpEvent;
    this.RaiseEvent(args);

   
   
  



}



private void create_shapes_property(List<string> pixel_coordinate) //创建结构体
{
   
    PublicClass.shapes_property newshapes = new PublicClass.shapes_property();
    newshapes.workspace_name = PublicClass.cur_ctrl_name;
    newshapes.shapes_name = shapes_name;
    newshapes.shapes_type = PublicClass.is_draw_type;

    newshapes.dampness = float.Parse(ir_information[11].ToString());
    newshapes.Emiss = float.Parse(ir_information[16].ToString());
    newshapes.TempDist = UInt16.Parse(ir_information[17].ToString());
    newshapes.TempTamb = UInt16.Parse(ir_information[10].ToString());
    newshapes.Temprevise = float.Parse(ir_information[15].ToString());





    if (PublicClass.is_draw_type == "adjust")
    {
        newshapes.shapes_type = ((PublicClass.shapes_property)PublicClass.shapes_count[public_shapes_index]).shapes_type;
    }

    newshapes.vertex =new List<double>(vertex_coordinate);
    //List<int> all_temp = new List<int>();
    

        //////插入像素值//////
    if (pixel_coordinate.Count > 0)
    {
        newshapes.pixel_coordinate = new List<int>();
        newshapes.unique_temp = new List<int>();
        newshapes.unique_percent = new List<double>();
        newshapes.percent = 0;
        for (int i = 0; i < pixel_coordinate.Count; i++)
        {
            newshapes.pixel_coordinate.Add(int.Parse(pixel_coordinate[i]));
            newshapes.unique_temp.Add(int.Parse(ir_temp[newshapes.pixel_coordinate[i]].ToString()));
            //all_temp.Add(int.Parse(ir_temp[newshapes.pixel_coordinate[i]].ToString()));
        }
        //newshapes.unique_temp = newshapes.unique_temp.Distinct().ToList();
        newshapes.min_temp = (from c in newshapes.unique_temp select c).Min();
        newshapes.max_temp = (from c in newshapes.unique_temp select c).Max();
        newshapes.avr_temp = (from c in newshapes.unique_temp select c).Average();
        newshapes.pixels_count = pixel_coordinate.Count;
    }





    bool rept_name = false;
    for (int i = 0; i < PublicClass.shapes_count.Count; i++)
    {
        PublicClass.shapes_property tempshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
        if (tempshapes.shapes_name == shapes_name && tempshapes.workspace_name == PublicClass.cur_ctrl_name)
        {
            if (newshapes.pixels_count == 0)
            {

            }
            else if(newshapes.shapes_name!="0")
            {
                if (histroy_shapes_count.Count > 0)
                {
                    int t = histroy_shapes_count.Count;
                    for (int j = histroy_shapes_index; j < t; j++)
                    {
                        try
                        {
                            histroy_shapes_count.RemoveAt(histroy_shapes_index + 1);
                            histroy_shapes_operation.RemoveAt(histroy_shapes_index + 1);
                        }
                        catch { }
                    }
                }
                PublicClass.shapes_count[i] = newshapes;
                if (newshapes.shapes_type != "work" && newshapes.shapes_type != "spot" && newshapes.shapes_type != "area" && newshapes.shapes_type != "min_temp" && newshapes.shapes_type != "max_temp")
                {
                    histroy_shapes_count.Add(newshapes);
                    histroy_shapes_operation.Add("update");
                    histroy_shapes_index = histroy_shapes_count.Count - 1;
                }
                public_shapes_index = i;
            }

            rept_name = true;
        }
    }

            if (!rept_name)
            {
                if (newshapes.pixels_count == 0)
                {
                    
                }
                else if(newshapes.shapes_name!="0")
                {
                    PublicClass.shapes_count.Add(newshapes);
                    if (histroy_shapes_count.Count > 0)
                    {
                        int t = histroy_shapes_count.Count;
                        for (int i = histroy_shapes_index; i < t; i++)
                        {
                            try
                            {
                                histroy_shapes_count.RemoveAt(histroy_shapes_index + 1);
                                histroy_shapes_operation.RemoveAt(histroy_shapes_index + 1);
                            }
                            catch { }
                        }
                    }
                        if (newshapes.shapes_type != "work" && newshapes.shapes_type != "spot" && newshapes.shapes_type != "area" && newshapes.shapes_type != "min_temp" && newshapes.shapes_type != "max_temp")
                        {
                            histroy_shapes_count.Add(newshapes);
                            histroy_shapes_operation.Add("insert");
                            histroy_shapes_index = histroy_shapes_count.Count - 1;
                        }
                    public_shapes_index = PublicClass.shapes_count.Count - 1;
                }

            }


    

}

public void calculate_percent()//计算比例
{
    for (int i = 0; i < PublicClass.shapes_count.Count; i++)
    {
        PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
        if (newshapes.workspace_name == PublicClass.cur_ctrl_name && newshapes.percent==0)
        {
            List<int> all_temp = new List<int>();
            newshapes.unique_temp.Clear();
            for (int j = 0; j < newshapes.pixel_coordinate.Count; j++)
            {
                //newshapes.pixel_coordinate.Add(int.Parse(pixel_coordinate[i]));
                newshapes.unique_temp.Add(int.Parse(ir_temp[newshapes.pixel_coordinate[j]].ToString()));
                //all_temp.Add(int.Parse(ir_temp[newshapes.pixel_coordinate[i]].ToString()));
            }

            all_temp = newshapes.unique_temp;
            newshapes.unique_temp = newshapes.unique_temp.Distinct().ToList();
            for (int j = 0; j < newshapes.unique_temp.Count; j++)
            {
                double t;
                t = (from c in all_temp where c == newshapes.unique_temp[j] select c).Count();
                t = t / newshapes.pixel_coordinate.Count() * 100;
                newshapes.unique_percent.Add(t);
            }
            newshapes.percent = (from c in newshapes.unique_percent select c).Max();
            newshapes.min_temp = (from c in newshapes.unique_temp select c).Min();
            newshapes.max_temp = (from c in newshapes.unique_temp select c).Max();
            newshapes.avr_temp = (from c in newshapes.unique_temp select c).Average();
            PublicClass.shapes_count[i] = newshapes;
        }
    }
}

public void backtask_DoWork(object sender, DoWorkEventArgs e)
{
    Thread newthread = new Thread(new ThreadStart(() =>
        {
            Dispatcher.Invoke(new Action(() =>
                {


                    
                    ir_canvas_back.Children.Clear();
                    RectangleGeometry backdraw = new RectangleGeometry();
                    backdraw.Rect = new Rect(0, 0, ir_width, ir_height);
                    System.Windows.Shapes.Path backdraw_path = new System.Windows.Shapes.Path();
                    backdraw_path.Stroke = Brushes.White;
                    backdraw_path.StrokeThickness = 1;
                    backdraw_path.Fill = Brushes.White;
                    backdraw_path.Data = backdraw;
                    backdraw_path.SnapsToDevicePixels = true;
                    ir_canvas_back.Children.Add(backdraw_path);





                    if (vertex_coordinate.Count > 0)
                    {
                        PublicClass.shapes_property newshapes = new PublicClass.shapes_property();
                        if (public_shapes_index >= 0)
                        {
                            newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[public_shapes_index];
                        }
                        if (PublicClass.is_draw_type == "rectangle" || PublicClass.is_draw_type == "work" || (PublicClass.is_draw_type == "adjust" && newshapes.shapes_type == "rectangle"))
                        {
                            RectangleGeometry newrect = new RectangleGeometry();
                            double tx = vertex_coordinate[0];
                            double ty = vertex_coordinate[1];
                            newrect.Rect = new Rect(tx, ty, vertex_coordinate[2], vertex_coordinate[3]);
                            System.Windows.Shapes.Path fontdraw_path = new System.Windows.Shapes.Path();
                            fontdraw_path.Stroke = Brushes.Black;
                            fontdraw_path.StrokeThickness = 1;
                            fontdraw_path.Fill = Brushes.Black;
                            fontdraw_path.Data = newrect;
                            fontdraw_path.SnapsToDevicePixels = true;
                            ir_canvas_back.Children.Add(fontdraw_path);

                        }
                        else if (PublicClass.is_draw_type == "ellipse" || (PublicClass.is_draw_type == "adjust" && newshapes.shapes_type == "ellipse"))
                        {
                            EllipseGeometry newellipse = new EllipseGeometry();
                            double tx = vertex_coordinate[0];
                            double ty = vertex_coordinate[1];
                            newellipse.Center = new Point(tx, ty);
                            newellipse.RadiusX = vertex_coordinate[2];
                            newellipse.RadiusY = vertex_coordinate[3];
                            System.Windows.Shapes.Path fontdraw_path = new System.Windows.Shapes.Path();
                            fontdraw_path.Stroke = Brushes.Black;
                            fontdraw_path.StrokeThickness = 1;
                            fontdraw_path.Fill = Brushes.Black;
                            fontdraw_path.Data = newellipse;
                            fontdraw_path.SnapsToDevicePixels = true;
                            ir_canvas_back.Children.Add(fontdraw_path);
                        }

                        else if (PublicClass.is_draw_type == "line" || (PublicClass.is_draw_type == "adjust" && newshapes.shapes_type == "line"))
                        {
                            LineGeometry neweline = new LineGeometry();
                            double tx = vertex_coordinate[0];
                            double ty = vertex_coordinate[1];
                            neweline.StartPoint = new Point(tx, ty);
                            neweline.EndPoint = new Point(vertex_coordinate[2], vertex_coordinate[3]);
                            System.Windows.Shapes.Path fontdraw_path = new System.Windows.Shapes.Path();
                            fontdraw_path.Stroke = Brushes.Black;
                            fontdraw_path.StrokeThickness = 1;
                            fontdraw_path.Fill = Brushes.Black;
                            fontdraw_path.Data = neweline;
                            fontdraw_path.SnapsToDevicePixels = true;
                            ir_canvas_back.Children.Add(fontdraw_path);
                        }
                        else if (PublicClass.is_draw_type == "point" || (PublicClass.is_draw_type == "adjust" && newshapes.shapes_type == "point") || (PublicClass.is_draw_type == "point" && newshapes.shapes_type == "max_temp"))
                        {
                            LineGeometry neweline = new LineGeometry();
                            double tx = vertex_coordinate[0];
                            double ty = vertex_coordinate[1];
                            neweline.StartPoint = new Point(tx, ty);
                            neweline.EndPoint = new Point(tx + 1, ty + 1);
                            System.Windows.Shapes.Path fontdraw_path = new System.Windows.Shapes.Path();
                            fontdraw_path.Stroke = Brushes.Black;
                            fontdraw_path.StrokeThickness = 1;
                            fontdraw_path.Fill = Brushes.Black;
                            fontdraw_path.Data = neweline;
                            fontdraw_path.SnapsToDevicePixels = true;
                            ir_canvas_back.Children.Add(fontdraw_path);
                        }
                        else if (PublicClass.is_draw_type == "polyline" || (PublicClass.is_draw_type == "adjust" && newshapes.shapes_type == "polyline"))
                        {

                            PointCollection newcollection = new PointCollection();
                            double tx;
                            double ty;
                            for (int i1 = 0; i1 < vertex_coordinate.Count - 1; i1 = i1 + 2)
                            {


                                tx = vertex_coordinate[i1];
                                ty = vertex_coordinate[i1 + 1];
                                newcollection.Add(new Point(tx, ty));
                            }
                            Polyline newpolyline = new Polyline();
                            newpolyline.Points = newcollection;
                            newpolyline.Stroke = Brushes.Black;
                            newpolyline.StrokeThickness = 1;
                            newpolyline.SnapsToDevicePixels = true;
                            ir_canvas_back.Children.Add(newpolyline);
                        }

                        else if (PublicClass.is_draw_type == "polygon" || (PublicClass.is_draw_type == "adjust" && newshapes.shapes_type == "polygon"))
                        {
                            PointCollection newcollection = new PointCollection();
                            double tx;
                            double ty;
                            for (int i1 = 0; i1 < vertex_coordinate.Count - 1; i1 = i1 + 2)
                            {
                                tx = vertex_coordinate[i1];
                                ty = vertex_coordinate[i1 + 1];

                                newcollection.Add(new Point(tx, ty));
                            }
                            Polygon newpolygon = new Polygon();
                            newpolygon.Points = newcollection;
                            newpolygon.Stroke = Brushes.Black;
                            newpolygon.Fill = Brushes.Black;
                            newpolygon.FillRule = FillRule.Nonzero;
                            newpolygon.StrokeThickness = 1;
                            newpolygon.SnapsToDevicePixels = true;
                            ir_canvas_back.Children.Add(newpolygon);
                        }
                    }

                    

                }));
            


        }));
    newthread.SetApartmentState(ApartmentState.MTA);
    newthread.IsBackground = true;
    //newthread.Priority = ThreadPriority.AboveNormal;
    newthread.Start();
    Thread.Sleep(50);
}
public void deleteshapes()//删除图形
{
    for (int i = 0; i < PublicClass.shapes_count.Count; i++)
    {
        PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
        if (newshapes.workspace_name == PublicClass.cur_ctrl_name)
        {
            System.Windows.Shapes.Path deletepath = ir_canvas_font.FindName(newshapes.shapes_name + "f") as System.Windows.Shapes.Path;

            if (deletepath != null)
            {
                ir_canvas_font.Children.Remove(deletepath);
                ir_canvas_font.UnregisterName(newshapes.shapes_name + "f");
            }
            deletepath = ir_canvas_font.FindName(newshapes.shapes_name + "b") as System.Windows.Shapes.Path;

            if (deletepath != null)
            {
                ir_canvas_font.Children.Remove(deletepath);
                ir_canvas_font.UnregisterName(newshapes.shapes_name + "b");
            }

            deletepath = ir_canvas_font.FindName(newshapes.shapes_name) as System.Windows.Shapes.Path;

            if (deletepath != null)
            {
                ir_canvas_font.Children.Remove(deletepath);
                ir_canvas_font.UnregisterName(newshapes.shapes_name);
            }

            Polygon deletepolygon = ir_canvas_font.FindName(newshapes.shapes_name) as Polygon;
            if (deletepolygon != null)
            {
                ir_canvas_font.Children.Remove(deletepolygon);
                ir_canvas_font.UnregisterName(newshapes.shapes_name);
            }
            deletepolygon = ir_canvas_font.FindName(newshapes.shapes_name + "b") as Polygon;
            if (deletepolygon != null)
            {
                ir_canvas_font.Children.Remove(deletepolygon);
                ir_canvas_font.UnregisterName(newshapes.shapes_name+"b");
            }
            deletepolygon = ir_canvas_font.FindName(newshapes.shapes_name + "f") as Polygon;
            if (deletepolygon != null)
            {
                ir_canvas_font.Children.Remove(deletepolygon);
                ir_canvas_font.UnregisterName(newshapes.shapes_name + "f");
            }

            Polyline deletepolyline = ir_canvas_font.FindName(newshapes.shapes_name) as Polyline;
            if (deletepolyline != null)
            {
                ir_canvas_font.Children.Remove(deletepolyline);
                ir_canvas_font.UnregisterName(newshapes.shapes_name);
            }
            deletepolyline = ir_canvas_font.FindName(newshapes.shapes_name + "b") as Polyline;
            if (deletepolyline != null)
            {
                ir_canvas_font.Children.Remove(deletepolyline);
                ir_canvas_font.UnregisterName(newshapes.shapes_name + "b");
            }
            deletepolyline = ir_canvas_font.FindName(newshapes.shapes_name + "f") as Polyline;
            if (deletepolyline != null)
            {
                ir_canvas_font.Children.Remove(deletepolyline);
                ir_canvas_font.UnregisterName(newshapes.shapes_name + "f");
            }

            TextBlock deletetxt = ir_canvas_font.FindName("txt" + newshapes.shapes_name) as TextBlock;
            if (deletetxt != null)
            {
                ir_canvas_font.Children.Remove(deletetxt);
                ir_canvas_font.UnregisterName("txt" + newshapes.shapes_name);
            }
            for (int i1 = 1; i1 < 3; i1++)//删除点
            {
                deletepath = ir_canvas_font.FindName(newshapes.shapes_name + i1.ToString()) as System.Windows.Shapes.Path;
                if (deletepath != null)
                {
                    ir_canvas_font.Children.Remove(deletepath);
                    ir_canvas_font.UnregisterName(newshapes.shapes_name + i1.ToString());
                }
            }
        }

    }

}

public void re_create_shapes()//前一个图形重做
{
    


    if (PublicClass.shapes_count.Count > 0)
    {

        deleteshapes();

            for (int i = 0; i < PublicClass.shapes_count.Count; i++)
            {
                PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                if (newshapes.workspace_name == PublicClass.cur_ctrl_name)
                {


                    int in_shapes = (from c in selected_spot_area where c == newshapes.shapes_name select c).Count();
                    if (newshapes.shapes_type == "point" || (newshapes.shapes_type == "spot" && in_shapes > 0) || newshapes.shapes_type == "max_temp" || newshapes.shapes_type == "min_temp")
                    {
                        double tx = newshapes.vertex[0];
                        double ty = newshapes.vertex[1];

                        LineGeometry linex = new LineGeometry();
                        LineGeometry liney = new LineGeometry();
                        linex.StartPoint = new Point(tx * zoom_coe - 9, ty * zoom_coe);
                        linex.EndPoint = new Point(tx * zoom_coe + 9, ty * zoom_coe);
                        liney.StartPoint = new Point(tx * zoom_coe, ty * zoom_coe - 9);
                        liney.EndPoint = new Point(tx * zoom_coe, ty * zoom_coe + 9);

                        if (tx * zoom_coe - 10 < 0)
                        {
                            linex.StartPoint = new Point(tx*zoom_coe + 3, ty*zoom_coe);

                        }
                        else if (tx*zoom_coe + 10 > ir_width * zoom_coe)
                        {
                            linex.EndPoint = new Point(tx * zoom_coe - 3, ty * zoom_coe);

                        }

                        dynamic_shapes(linex, "line", newshapes.shapes_name + "1", false);
                       
                        if  (ty*zoom_coe - 10 < 0)
                        {
                            liney.StartPoint = new Point(tx * zoom_coe, ty * zoom_coe + 3);
                        }
                        else if (ty * zoom_coe + 10 > ir_height * zoom_coe)
                        {
                            liney.EndPoint = new Point(tx * zoom_coe, ty * zoom_coe - 3);
                        }

                        dynamic_shapes(liney, "line", newshapes.shapes_name + "2", false);
                       
                        //if (tx * zoom_coe - 8 > 0)
                        //{
                        //    LineGeometry line1 = new LineGeometry();
                        //    line1.StartPoint = new Point(tx * zoom_coe - 8, ty * zoom_coe);
                        //    line1.EndPoint = new Point(tx * zoom_coe - 3, ty * zoom_coe);
                        //    dynamic_shapes(line1, "line", newshapes.shapes_name + "1",false);
                            
                        //}
                        //if (tx * zoom_coe + 8 < ir_width * zoom_coe)
                        //{
                        //    LineGeometry line1 = new LineGeometry();
                        //    line1.StartPoint = new Point(tx * zoom_coe + 3, ty * zoom_coe);
                        //    line1.EndPoint = new Point(tx * zoom_coe + 8, ty * zoom_coe);
                        //    dynamic_shapes(line1, "line", newshapes.shapes_name + "2",false);
                        //}
                        //if (ty * zoom_coe - 8 > 0)
                        //{
                        //    LineGeometry line1 = new LineGeometry();
                        //    line1.StartPoint = new Point(tx * zoom_coe, ty * zoom_coe - 8);
                        //    line1.EndPoint = new Point(tx * zoom_coe, ty * zoom_coe - 3);
                        //    dynamic_shapes(line1, "line", newshapes.shapes_name + "3",false);
                        //}
                        //if (ty * zoom_coe + 8 < ir_height * zoom_coe)
                        //{
                        //    LineGeometry line1 = new LineGeometry();
                        //    line1.StartPoint = new Point(tx * zoom_coe, ty * zoom_coe + 8);
                        //    line1.EndPoint = new Point(tx * zoom_coe, ty * zoom_coe + 3);
                        //    dynamic_shapes(line1, "line", newshapes.shapes_name + "4",false);
                        //}
                        Point txtpoint = new Point();
                        txtpoint.X = tx * zoom_coe - 10;
                        txtpoint.Y = ty * zoom_coe - 25;
                        if (txtpoint.X < 10)
                        {
                            txtpoint.X = 10;
                        }
                        if (txtpoint.Y < 10)
                        {
                            txtpoint.Y = 10;
                        }
                        TextBlock txt = new TextBlock();
                        txt.Text = newshapes.shapes_name;
                        dynamic_txt(txtpoint, newshapes.shapes_name);
                    }
                    else if (newshapes.shapes_type == "line")
                    {
                        LineGeometry neweline = new LineGeometry();
                        double tx = newshapes.vertex[0];
                        double ty = newshapes.vertex[1];
                        neweline.StartPoint = new Point(tx * zoom_coe, ty * zoom_coe);
                        neweline.EndPoint = new Point(newshapes.vertex[2] * zoom_coe, newshapes.vertex[3] * zoom_coe);

                        dynamic_shapes(neweline, "line", newshapes.shapes_name+"f",false);

                        LineGeometry newelineb = new LineGeometry();
                        newelineb.StartPoint = new Point(tx * zoom_coe, ty * zoom_coe);
                        newelineb.EndPoint = new Point(newshapes.vertex[2] * zoom_coe, newshapes.vertex[3] * zoom_coe);
                        dynamic_shapes(newelineb, "line", newshapes.shapes_name + "b",false);

                        Point txtpoint = new Point();
                        txtpoint.X = neweline.StartPoint.X - 5;
                        txtpoint.Y = neweline.StartPoint.Y - 15;
                        if (txtpoint.X < 10)
                        {
                            txtpoint.X = 10;
                        }
                        if (txtpoint.Y < 10)
                        {
                            txtpoint.Y = 10;
                        }
                        TextBlock txt = new TextBlock();
                        txt.Text = newshapes.shapes_name;
                        dynamic_txt(txtpoint, newshapes.shapes_name);
                    }
                    else if (newshapes.shapes_type == "polyline")
                    {
                        double tx = newshapes.vertex[0];
                        double ty = newshapes.vertex[1];
                        Point txtpoint = new Point();
                        txtpoint.X = tx * zoom_coe - 5;
                        txtpoint.Y = ty * zoom_coe - 15;
                        if (txtpoint.X < 10)
                        {
                            txtpoint.X = 10;
                        }
                        if (txtpoint.Y < 10)
                        {
                            txtpoint.Y = 10;
                        }
                        TextBlock txt = new TextBlock();
                        txt.Text = newshapes.shapes_name;
                        dynamic_txt(txtpoint, newshapes.shapes_name);


                        PointCollection newcollection = new PointCollection();
                        for (int i1 = 0; i1 < newshapes.vertex.Count - 1; i1 = i1 + 2)
                        {

                            tx = newshapes.vertex[i1];
                            ty = newshapes.vertex[i1 + 1];
                            newcollection.Add(new Point(tx * zoom_coe, ty * zoom_coe));
                        }
                        Polyline polylinef = new Polyline();
                        polylinef.Points = newcollection;
                        Polyline polylineb = new Polyline();
                        polylineb.Points = newcollection;
                        dynamic_shapes(polylinef, "polyline", newshapes.shapes_name+"f",false);
                        dynamic_shapes(polylineb, "polyline", newshapes.shapes_name+"b",false);
                    }
                    else if (newshapes.shapes_type == "ellipse")
                    {
                        EllipseGeometry newellipse = new EllipseGeometry();
                        double tx = newshapes.vertex[0];
                        double ty = newshapes.vertex[1];
                        newellipse.Center = new Point(tx * zoom_coe, ty * zoom_coe);
                        newellipse.RadiusX = newshapes.vertex[2] * zoom_coe;
                        newellipse.RadiusY = newshapes.vertex[3] * zoom_coe;
                        dynamic_shapes(newellipse, "ellipse", newshapes.shapes_name+"f",false);

                        EllipseGeometry newellipseb = new EllipseGeometry();
                        newellipseb.Center = new Point(tx * zoom_coe, ty * zoom_coe);
                        newellipseb.RadiusX = newshapes.vertex[2] * zoom_coe;
                        newellipseb.RadiusY = newshapes.vertex[3] * zoom_coe;
                        dynamic_shapes(newellipseb, "ellipse", newshapes.shapes_name+"b",false);

                        Point txtpoint = new Point();
                        txtpoint.X = (newellipse.Center.X - newellipse.RadiusX) - 5;
                        txtpoint.Y = (newellipse.Center.Y - newellipse.RadiusY) - 15;
                        if (txtpoint.X < 10)
                        {
                            txtpoint.X = 10;
                        }
                        if (txtpoint.Y < 10)
                        {
                            txtpoint.Y = 10;
                        }
                        TextBlock txt = new TextBlock();
                        txt.Text = newshapes.shapes_name;
                        dynamic_txt(txtpoint, newshapes.shapes_name);
                    }
                    else if (newshapes.shapes_type == "rectangle" || (newshapes.shapes_type =="area" &&in_shapes>0) )
                    {
                        RectangleGeometry newrect = new RectangleGeometry();
                        double tx = newshapes.vertex[0];
                        double ty = newshapes.vertex[1];
                        newrect.Rect = new Rect(tx * zoom_coe, ty * zoom_coe, newshapes.vertex[2] * zoom_coe, newshapes.vertex[3] * zoom_coe);
                        dynamic_shapes(newrect, "rectangle", newshapes.shapes_name+"f",false);

                        RectangleGeometry newrectb = new RectangleGeometry();
                        newrectb.Rect = new Rect(tx * zoom_coe, ty * zoom_coe, newshapes.vertex[2] * zoom_coe, newshapes.vertex[3] * zoom_coe);
                        dynamic_shapes(newrectb, "rectangle", newshapes.shapes_name+"b",false);


                        Point txtpoint = new Point();
                        txtpoint.X = tx * zoom_coe - 5;
                        txtpoint.Y = ty * zoom_coe - 15;
                        if (txtpoint.X < 10)
                        {
                            txtpoint.X = 10;
                        }
                        if (txtpoint.Y < 10)
                        {
                            txtpoint.Y = 10;
                        }
                        TextBlock txt = new TextBlock();
                        txt.Text = newshapes.shapes_name;
                        dynamic_txt(txtpoint, newshapes.shapes_name);

                    }

                    else if (newshapes.shapes_type == "polygon")
                    {
                        double tx = newshapes.vertex[0];
                        double ty = newshapes.vertex[1];
                        Point txtpoint = new Point();
                        txtpoint.X = tx * zoom_coe - 5;
                        txtpoint.Y = ty * zoom_coe - 15;
                        if (txtpoint.X < 10)
                        {
                            txtpoint.X = 10;
                        }
                        if (txtpoint.Y < 10)
                        {
                            txtpoint.Y = 10;
                        }
                        TextBlock txt = new TextBlock();
                        txt.Text = newshapes.shapes_name;
                        dynamic_txt(txtpoint, newshapes.shapes_name);


                        PointCollection newcollection = new PointCollection();
                        for (int i1 = 0; i1 < newshapes.vertex.Count - 1; i1 = i1 + 2)
                        {

                            tx = newshapes.vertex[i1];
                            ty = newshapes.vertex[i1 + 1];
                            newcollection.Add(new Point(tx * zoom_coe, ty * zoom_coe));
                        }
                        Polygon newpolygon = new Polygon();
                        newpolygon.Points = newcollection;
                        dynamic_shapes(newpolygon, "polygon", newshapes.shapes_name+"f",false);

                        Polygon newpolygonb = new Polygon();
                        newpolygonb.Points = newcollection;
                        dynamic_shapes(newpolygonb, "polygon", newshapes.shapes_name+"b",false);
                    }
                }

            }
    }

    create_vertex_animation();
}




int txtcot = 0;//文本元素下标
int iecot = 0;//刻度元素下标
        public void calculate_temp_ruler()//生成温度标尺
        {
            for (int i = 0; i < txtcot; i++)
            {
                TextBlock deletetxt = right_canvas.FindName("txt" + i) as TextBlock;
                if (deletetxt != null)
                {
                    right_canvas.Children.Remove(deletetxt);
                    right_canvas.UnregisterName("txt" + i);
                }
            }
            for (int i = 0; i < iecot; i++)
            {
                System.Windows.Shapes.Path deletepath = right_canvas.FindName("ie" + i) as System.Windows.Shapes.Path;
                if (deletepath != null)
                {
                    right_canvas.Children.Remove(deletepath);
                    right_canvas.UnregisterName("ie" + i);
                }
            }
            txtcot = 0;
            iecot = 0;





            TextBlock txtmax_temp = new TextBlock();
            //txtmax_temp.Text = float.Parse(temp_max_min_avr[0].ToString()) / 10 + " ℃";
            txtmax_temp.Text = Math.Round(grad_max / 10d,1) + " ℃";
            //txtmax_temp.Text = (int)((float)(grad_max / 10) * 10) / 10.0 + " ℃";
            txtmax_temp.Margin = new Thickness(10, 0, 0, 0);
            right_canvas.Children.Add(txtmax_temp);
            right_canvas.RegisterName("txt" + txtcot, txtmax_temp);
            txtcot++;

            TextBlock txtmin_temp = new TextBlock();
            //txtmin_temp.Text = float.Parse(temp_max_min_avr[2].ToString()) / 10 + " ℃";
            txtmin_temp.Text = Math.Round(grad_min / 10d, 1) + " ℃";
            //txtmin_temp.Text =(int)((float)(grad_min / 10)*10)/10.0 + " ℃";
            txtmin_temp.Margin = new Thickness(10, irimg.Height-15, 0, 0);
            right_canvas.Children.Add(txtmin_temp);
            right_canvas.RegisterName("txt" + txtcot, txtmin_temp);
            txtcot++;

            LineGeometry baseline = new LineGeometry();
            baseline.StartPoint=new Point(42, 20);
            baseline.EndPoint = new Point(42, irimg.Height - 20);

            System.Windows.Shapes.Path mypath = new System.Windows.Shapes.Path();
            mypath.Stroke = Brushes.Black;
            mypath.StrokeThickness = 1;
            mypath.SnapsToDevicePixels = true;
            mypath.Data = baseline;
            right_canvas.Children.Add(mypath);
            right_canvas.RegisterName("ie" + iecot, mypath);
            iecot++;

            int step = (int)irimg.Height % 5;
            if (step == 0)
            {
                step = 4;
            }
            int ruler_count = (int)(irimg.Height - 20) / 10;//刻度统计
            //float min_ruler_temp = (float.Parse(temp_max_min_avr[0].ToString()) - float.Parse(temp_max_min_avr[2].ToString())) / ruler_count;
            float min_ruler_temp = (float)(grad_max - grad_min) / ruler_count;
            int ruler_coe = 1;//刻度系数
            for (int i = 25; i > 0; i = i + 10)
            {
                if (i > irimg.Height-20)
                {
                    break;
                }
                System.Windows.Shapes.Path rulerpath = new System.Windows.Shapes.Path();
                rulerpath.Stroke = Brushes.Black;
                rulerpath.SnapsToDevicePixels = true;
                if (step == 5)
                {

                    TextBlock rule_temp = new TextBlock();
                    int temp1 = (int)(grad_max - min_ruler_temp * ruler_coe);
                    //rule_temp.Text =(float)temp1/10 + " ℃";
                    rule_temp.Text = Math.Round(temp1/10d,1)+ " ℃";
                    rule_temp.Margin = new Thickness(55, i-8, 0, 0);
                    right_canvas.Children.Add(rule_temp);
                    right_canvas.RegisterName("txt" + txtcot, rule_temp);
                    txtcot++;
                    
                    LineGeometry ruler = new LineGeometry();
                    ruler.StartPoint = new Point(42, i);
                    ruler.EndPoint = new Point(52, i);
                    rulerpath.StrokeThickness = 2;
                    rulerpath.Data = ruler;
                }
                else
                {
                    LineGeometry ruler = new LineGeometry();
                    ruler.StartPoint = new Point(42, i);
                    ruler.EndPoint = new Point(47, i);
                    rulerpath.StrokeThickness = 1;
                    rulerpath.Data = ruler;
                }
                right_canvas.Children.Add(rulerpath);
                right_canvas.RegisterName("ie" + iecot, rulerpath);
                iecot++;
                
                if (step == 5)
                {
                    step = 0;
                }
                step++;
                ruler_coe++;
            }
            

        }

        public void change_shapes_active(MouseButtonEventArgs e)// 修改当前激活图形
        {
            if (PublicClass.is_draw_type == "adjust" && drawpoint==e.GetPosition(ir_canvas_font))
            {
                Point curpoint = new Point();
                curpoint.X = drawpoint.X / zoom_coe;
                curpoint.Y = drawpoint.Y / zoom_coe;
                List<string> temp_histroy_shapes_active = new List<string>();
                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                    if (newshapes.workspace_name == PublicClass.cur_ctrl_name && newshapes.shapes_type != "work")
                    {
                        int in_shapes = (from c in newshapes.pixel_coordinate where c % ir_width > curpoint.X - 2 && c % ir_width < curpoint.X + 2 && c / ir_width > curpoint.Y - 2 && c / ir_width < curpoint.Y + 2 select c).Count();
                        if (in_shapes > 0)
                        {
                            temp_histroy_shapes_active.Add(newshapes.shapes_name);
                        }
                    }
                }

                int sub_index = (from h in histroy_shapes_active from t in temp_histroy_shapes_active where h == t select h).Count();
                if(sub_index<histroy_shapes_active.Count || (histroy_shapes_active.Count != temp_histroy_shapes_active.Count))
                {
                    histroy_shapes_active.Clear();
                    histroy_shapes_active = temp_histroy_shapes_active;
                    histroy_shapes_active_index = 0;
                }
                if(histroy_shapes_active.Count>0)
                {
                    shapes_active = histroy_shapes_active[histroy_shapes_active_index % histroy_shapes_active.Count];
                    for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                    {
                        PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                        if (newshapes.shapes_type != "work" && newshapes.workspace_name == PublicClass.cur_ctrl_name && newshapes.shapes_name == shapes_active)
                        {
                            public_shapes_index = i;
                            //shapes_active = newshapes.shapes_name;
                            break;
                        }
                    }


                    histroy_shapes_active_index++;
                    re_create_shapes();
                    //change_cursors(e);
                }
                

            }
        }

        private void change_cursors(MouseEventArgs e)//改变鼠标图形
        {
            if (PublicClass.is_draw_type == "adjust" && e.LeftButton!=MouseButtonState.Pressed)
            {
                lock_vertex = -10;
                Point curpoint = new Point();
                curpoint.X = e.GetPosition(ir_canvas_font).X / zoom_coe;
                curpoint.Y = e.GetPosition(ir_canvas_font).Y / zoom_coe;
                PublicClass.shapes_property newshapes = new PublicClass.shapes_property();
                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                    if (newshapes.shapes_name == shapes_active && newshapes.workspace_name == PublicClass.cur_ctrl_name && newshapes.shapes_type != "work")
                    {
                        break;
                    }
                }
                ir_canvas_font.Cursor = Cursors.Arrow;
                if (newshapes.shapes_type == "point")
                {
                    if (newshapes.vertex[0] > e.GetPosition(ir_canvas_font).X / zoom_coe - 5 && newshapes.vertex[0] < e.GetPosition(ir_canvas_font).X / zoom_coe + 5 && newshapes.vertex[1] > e.GetPosition(ir_canvas_font).Y / zoom_coe - 5 && newshapes.vertex[1] < e.GetPosition(ir_canvas_font).Y / zoom_coe + 5)
                    {
                        lock_vertex = 0;
                        ir_canvas_font.Cursor = Cursors.SizeAll;
                    }

                }
                else if (newshapes.shapes_type == "line")
                {
  
                    int in_shapes = (from c in newshapes.pixel_coordinate where c % ir_width > curpoint.X - 2 && c % ir_width < curpoint.X + 2 && c / ir_width > curpoint.Y - 2 && c / ir_width < curpoint.Y + 2 select c).Count();
                    if (in_shapes > 0)
                    {
                        ir_canvas_font.Cursor = Cursors.SizeAll;
                        lock_vertex = -1;
                    }
                    for (int i = 0; i < newshapes.vertex.Count - 1; i = i + 2)
                    {
                        if (newshapes.vertex[i] * zoom_coe > e.GetPosition(ir_canvas_font).X - 3 && newshapes.vertex[i] * zoom_coe < e.GetPosition(ir_canvas_font).X + 3 && newshapes.vertex[i + 1] * zoom_coe > e.GetPosition(ir_canvas_font).Y - 3 && newshapes.vertex[i + 1] < e.GetPosition(ir_canvas_font).Y + 3)
                        {
                            lock_vertex = i;
                        }
                    }
                }

                else if (newshapes.shapes_type == "ellipse")
                {
                    int in_shapes = (from c in newshapes.pixel_coordinate where c % ir_width > curpoint.X - 2 && c % ir_width < curpoint.X + 2 && c / ir_width > curpoint.Y - 2 && c / ir_width < curpoint.Y + 2 select c).Count();
                    if (in_shapes > 0)
                    {
                        ir_canvas_font.Cursor = Cursors.SizeAll;
                        lock_vertex = -1;
                    }
                    if (e.GetPosition(ir_canvas_font).X > newshapes.vertex[0] * zoom_coe - newshapes.vertex[2] * zoom_coe - 3 && e.GetPosition(ir_canvas_font).X < newshapes.vertex[0] * zoom_coe - newshapes.vertex[2] * zoom_coe + 3 && e.GetPosition(ir_canvas_font).Y > newshapes.vertex[1] * zoom_coe - 5 && e.GetPosition(ir_canvas_font).Y < newshapes.vertex[1] * zoom_coe + 5)
                    {
                        ir_canvas_font.Cursor = Cursors.SizeWE;
                        lock_vertex = 0;
                    }
                    else if (e.GetPosition(ir_canvas_font).Y > newshapes.vertex[1] * zoom_coe - newshapes.vertex[3] * zoom_coe - 3 && e.GetPosition(ir_canvas_font).Y < newshapes.vertex[1] * zoom_coe - newshapes.vertex[3] * zoom_coe + 3 && e.GetPosition(ir_canvas_font).X > newshapes.vertex[0] * zoom_coe - 5 && e.GetPosition(ir_canvas_font).X < newshapes.vertex[0] * zoom_coe + 5)
                    {
                        ir_canvas_font.Cursor = Cursors.SizeNS;
                        lock_vertex = 1;
                    }
                    else if (e.GetPosition(ir_canvas_font).X > newshapes.vertex[0] * zoom_coe + newshapes.vertex[2] * zoom_coe - 3 && e.GetPosition(ir_canvas_font).X < newshapes.vertex[0] * zoom_coe + newshapes.vertex[2] * zoom_coe + 3 && e.GetPosition(ir_canvas_font).Y > newshapes.vertex[1] * zoom_coe - 5 && e.GetPosition(ir_canvas_font).Y < newshapes.vertex[1] * zoom_coe + 5)
                    {
                        ir_canvas_font.Cursor = Cursors.SizeWE;
                        lock_vertex = 2;
                    }
                    else if (e.GetPosition(ir_canvas_font).Y > newshapes.vertex[1] * zoom_coe + newshapes.vertex[3] * zoom_coe - 3 && e.GetPosition(ir_canvas_font).Y < newshapes.vertex[1] * zoom_coe + newshapes.vertex[3] * zoom_coe + 3 && e.GetPosition(ir_canvas_font).X > newshapes.vertex[0] * zoom_coe - 5 && e.GetPosition(ir_canvas_font).X < newshapes.vertex[0] * zoom_coe + 5)
                    {
                        ir_canvas_font.Cursor = Cursors.SizeNS;
                        lock_vertex = 3;
                    }
                }
                else if (newshapes.shapes_type == "rectangle")
                {
                    int in_shapes = (from c in newshapes.pixel_coordinate where c % ir_width > curpoint.X - 2 && c % ir_width < curpoint.X + 2 && c / ir_width > curpoint.Y - 2 && c / ir_width < curpoint.Y + 2 select c).Count();
                    if (in_shapes > 0)
                    {
                        ir_canvas_font.Cursor = Cursors.SizeAll;
                        lock_vertex = -1;
                    }
                    if (e.GetPosition(ir_canvas_font).X > newshapes.vertex[0] * zoom_coe - 3 && e.GetPosition(ir_canvas_font).X < newshapes.vertex[0] * zoom_coe + 3 && e.GetPosition(ir_canvas_font).Y > newshapes.vertex[1] * zoom_coe - 3 && e.GetPosition(ir_canvas_font).Y < newshapes.vertex[1] * zoom_coe + 3)
                    {
                        ir_canvas_font.Cursor = Cursors.SizeNWSE;
                        lock_vertex = 0;
                    }
                    else if (e.GetPosition(ir_canvas_font).X > newshapes.vertex[0] * zoom_coe + newshapes.vertex[2] * zoom_coe - 3 && e.GetPosition(ir_canvas_font).X < newshapes.vertex[0] * zoom_coe + newshapes.vertex[2] * zoom_coe + 3 && e.GetPosition(ir_canvas_font).Y > newshapes.vertex[1] * zoom_coe - 3 && e.GetPosition(ir_canvas_font).Y < newshapes.vertex[1] * zoom_coe + 3)
                    {
                        ir_canvas_font.Cursor = Cursors.SizeNESW;
                        lock_vertex = 1;
                    }
                    else if (e.GetPosition(ir_canvas_font).X > newshapes.vertex[0] * zoom_coe + newshapes.vertex[2] * zoom_coe - 3 && e.GetPosition(ir_canvas_font).X < newshapes.vertex[0] * zoom_coe + newshapes.vertex[2] * zoom_coe + 3 && e.GetPosition(ir_canvas_font).Y > newshapes.vertex[1] * zoom_coe + newshapes.vertex[3] * zoom_coe - 3 && e.GetPosition(ir_canvas_font).Y < newshapes.vertex[1] * zoom_coe + newshapes.vertex[3] * zoom_coe + 3)
                    {
                        ir_canvas_font.Cursor = Cursors.SizeNWSE;
                        lock_vertex = 2;
                    }
                    else if (e.GetPosition(ir_canvas_font).X > newshapes.vertex[0] * zoom_coe - 3 && e.GetPosition(ir_canvas_font).X < newshapes.vertex[0] * zoom_coe + 3 && e.GetPosition(ir_canvas_font).Y > newshapes.vertex[1] * zoom_coe + newshapes.vertex[3] * zoom_coe - 3 && e.GetPosition(ir_canvas_font).Y < newshapes.vertex[1] * zoom_coe + newshapes.vertex[3] * zoom_coe + 3)
                    {
                        ir_canvas_font.Cursor = Cursors.SizeNESW;
                        lock_vertex = 3;
                    }
                    else if (e.GetPosition(ir_canvas_font).X > newshapes.vertex[0] * zoom_coe - 3 && e.GetPosition(ir_canvas_font).X < newshapes.vertex[0] * zoom_coe + 3 && e.GetPosition(ir_canvas_font).Y > newshapes.vertex[1] * zoom_coe + 3 && e.GetPosition(ir_canvas_font).Y < newshapes.vertex[1] * zoom_coe + newshapes.vertex[3] * zoom_coe - 3)
                    {
                        ir_canvas_font.Cursor = Cursors.SizeWE;
                        lock_vertex = 4;
                    }
                    else if (e.GetPosition(ir_canvas_font).X > newshapes.vertex[0] * zoom_coe + 3 && e.GetPosition(ir_canvas_font).X < newshapes.vertex[0] * zoom_coe + newshapes.vertex[2] * zoom_coe - 3 && e.GetPosition(ir_canvas_font).Y > newshapes.vertex[1] * zoom_coe - 3 && e.GetPosition(ir_canvas_font).Y < newshapes.vertex[1] * zoom_coe + 3)
                    {
                        ir_canvas_font.Cursor = Cursors.SizeNS;
                        lock_vertex = 5;
                    }
                    else if (e.GetPosition(ir_canvas_font).X > newshapes.vertex[0] * zoom_coe + newshapes.vertex[2] * zoom_coe - 3 && e.GetPosition(ir_canvas_font).X < newshapes.vertex[0] * zoom_coe + newshapes.vertex[2] * zoom_coe + 3 && e.GetPosition(ir_canvas_font).Y > newshapes.vertex[1] * zoom_coe + 3 && e.GetPosition(ir_canvas_font).Y < newshapes.vertex[1] * zoom_coe + newshapes.vertex[3] * zoom_coe - 3)
                    {
                        ir_canvas_font.Cursor = Cursors.SizeWE;
                        lock_vertex = 6;
                    }
                    else if (e.GetPosition(ir_canvas_font).X > newshapes.vertex[0] * zoom_coe + 3 && e.GetPosition(ir_canvas_font).X < newshapes.vertex[0] * zoom_coe + newshapes.vertex[2] * zoom_coe - 3 && e.GetPosition(ir_canvas_font).Y > newshapes.vertex[1] * zoom_coe + newshapes.vertex[3] * zoom_coe - 3 && e.GetPosition(ir_canvas_font).Y < newshapes.vertex[1] * zoom_coe + newshapes.vertex[3] * zoom_coe + 3)
                    {
                        ir_canvas_font.Cursor = Cursors.SizeNS;
                        lock_vertex = 7;
                    }
                }

                else if (newshapes.shapes_type == "polyline")
                {
                    int in_shapes = (from c in newshapes.pixel_coordinate where c % ir_width > curpoint.X - 2 && c % ir_width < curpoint.X + 2 && c / ir_width > curpoint.Y - 2 && c / ir_width < curpoint.Y + 2 select c).Count();
                    if (in_shapes > 0)
                    {
                        ir_canvas_font.Cursor = Cursors.SizeAll;
                        lock_vertex = -1;
                    }
                    for (int i = 0; i < newshapes.vertex.Count - 1; i = i + 2)
                    {
                        if (newshapes.vertex[i] * zoom_coe > e.GetPosition(ir_canvas_font).X - 3 && newshapes.vertex[i] * zoom_coe < e.GetPosition(ir_canvas_font).X + 3 && newshapes.vertex[i + 1] * zoom_coe > e.GetPosition(ir_canvas_font).Y - 3 && newshapes.vertex[i + 1] < e.GetPosition(ir_canvas_font).Y + 3)
                        {
                            lock_vertex = i;
                        }
                    }
                }

                else if (newshapes.shapes_type == "polygon")
                {
                    int in_shapes = (from c in newshapes.pixel_coordinate where c % ir_width > curpoint.X - 2 && c % ir_width < curpoint.X + 2 && c / ir_width > curpoint.Y - 2 && c / ir_width < curpoint.Y + 2 select c).Count();
                    if (in_shapes > 0)
                    {
                        ir_canvas_font.Cursor = Cursors.SizeAll;
                        lock_vertex = -1;
                    }
                    for (int i = 0; i < newshapes.vertex.Count - 1; i = i + 2)
                    {
                        if (newshapes.vertex[i] * zoom_coe > e.GetPosition(ir_canvas_font).X - 3 && newshapes.vertex[i] * zoom_coe < e.GetPosition(ir_canvas_font).X + 3 && newshapes.vertex[i + 1] * zoom_coe > e.GetPosition(ir_canvas_font).Y - 3 && newshapes.vertex[i + 1] < e.GetPosition(ir_canvas_font).Y + 3)
                        {
                            lock_vertex = i;
                        }
                    }
                }

            }
        }
       

        private void canvas_temp(MouseEventArgs e)//光标处温度
        {

            TextBlock canv_temp = ir_canvas_font.FindName("canvtemp") as TextBlock;
            if (canv_temp == null)
            {
                System.Windows.Media.Effects.DropShadowEffect da = new System.Windows.Media.Effects.DropShadowEffect();
                da.BlurRadius = 1;
                da.Opacity = 0.65;
                da.ShadowDepth = 1;

                TextBlock newtext = new TextBlock();
                if (spot_cen != null)
                {
                    Brush brush = spot_cen;
                    newtext.Foreground = brush;
                }
                newtext.Effect = da;
                ir_canvas_font.Children.Add(newtext);
                ir_canvas_font.RegisterName("canvtemp", newtext);
            }
            else
            {
                try
                {
                    canv_temp.Margin = new Thickness(e.GetPosition(ir_canvas_font).X, e.GetPosition(ir_canvas_font).Y + 16, 0, 0);
                    

                    if (spot_temp_cen== 2)
                    {

                        //+ "[" + "Max" + ":" + test.max_temp / 10.0 + "℃" + "]";
                        canv_temp.Text = "[" + ((int)(e.GetPosition(ir_canvas_font).X)).ToString() + "," + ((int)(e.GetPosition(ir_canvas_font).Y)).ToString() + "]";
                    }
                    else if (spot_temp_cen == 1)
                    {
                        canv_temp.Text = "[" + (int.Parse(ir_temp[(int)((int)(e.GetPosition(ir_canvas_font).Y / (zoom_coe)) * ir_width + (int)(e.GetPosition(ir_canvas_font).X / (zoom_coe)))].ToString()) / 10.0).ToString() + " ℃" + "]";
                    }
                    else if (spot_temp_cen == 0)
                    {
                        canv_temp.Text =  "";
                    }
                    else if (spot_temp_cen == 3)
                    {
                        canv_temp.Text = "[" + ((int)(e.GetPosition(ir_canvas_font).X)).ToString() + "," + ((int)(e.GetPosition(ir_canvas_font).Y)).ToString() + "]" + "[" + (int.Parse(ir_temp[(int)((int)(e.GetPosition(ir_canvas_font).Y / (zoom_coe)) * ir_width + (int)(e.GetPosition(ir_canvas_font).X / (zoom_coe)))].ToString()) / 10.0).ToString() + " ℃" + "]";
                    }
                   

                   // canv_temp.Text = "[" + ((int)(e.GetPosition(ir_canvas_font).X)).ToString() + "," + ((int)(e.GetPosition(ir_canvas_font).Y)).ToString() + "]" + "[" + (int.Parse(ir_temp[(int)((int)(e.GetPosition(ir_canvas_font).Y / (zoom_coe)) * ir_width + (int)(e.GetPosition(ir_canvas_font).X / (zoom_coe)))].ToString()) / 10.0).ToString() + " ℃" + "]";

                    if (ir_width * zoom_coe - 100 < e.GetPosition(ir_canvas_font).X && e.GetPosition(ir_canvas_font).X <= ir_width * zoom_coe)
                    {
                        canv_temp.Margin = new Thickness(ir_width * zoom_coe - 100, e.GetPosition(ir_canvas_font).Y + 16, 0, 0);
                    }
                    if (ir_height * zoom_coe - 32 < e.GetPosition(ir_canvas_font).Y && e.GetPosition(ir_canvas_font).Y <= ir_height * zoom_coe)
                    {
                        canv_temp.Margin = new Thickness(e.GetPosition(ir_canvas_font).X, ir_height * zoom_coe - 16, 0, 0);
                    }
                    if (ir_width * zoom_coe - 100 <= e.GetPosition(ir_canvas_font).X && e.GetPosition(ir_canvas_font).X <= ir_width * zoom_coe && ir_height * zoom_coe - 32 <= e.GetPosition(ir_canvas_font).Y && e.GetPosition(ir_canvas_font).Y <= ir_height * zoom_coe)
                    {
                        canv_temp.Margin = new Thickness(ir_width * zoom_coe - 100, ir_height * zoom_coe - 16, 0, 0);
                    }
                  

                }

                catch
                { }
            }
            
        }


        private void get_spot_area()
        {

            FileStream fs = new FileStream(filename, FileMode.Open,FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            fs.Seek(0x18, SeekOrigin.Begin);//从18h字开始
            string jpg_size_str = len_2(br.ReadByte());
            jpg_size_str += len_2(br.ReadByte());
            jpg_size_str += len_2(br.ReadByte());
            jpg_size_str += len_2(br.ReadByte());
            int jpg_size = Convert.ToInt32(jpg_size_str, 16);//获取JPG 文件长度
            fs.Seek(jpg_size + 0x100, SeekOrigin.Begin);
            string spot_addr = len_2(br.ReadByte());
            spot_addr += len_2(br.ReadByte());
            int spot_size = Convert.ToInt32(spot_addr, 16);
            string area_addr = len_2(br.ReadByte());
            area_addr += len_2(br.ReadByte());
            int area_size = Convert.ToInt32(area_addr, 16);
            for (int i = 0; i < 10; i++)
            {
                fs.Seek(jpg_size + 0x100 + spot_size + 9 + i * 20, SeekOrigin.Begin);
                string spot = br.ReadByte().ToString();
                if (int.Parse(spot.ToString()) != 0)
                {
                    fs.Seek(jpg_size + 0x100 + spot_size + 9 + 5 + i * 20, SeekOrigin.Begin);
                    string spot_x = len_2(br.ReadByte());
                    spot_x += len_2(br.ReadByte()).ToString();        
                    int spot_x_size = Convert.ToInt32(spot_x, 16)/2;
                    string spot_y = len_2(br.ReadByte());
                    spot_y += len_2(br.ReadByte()).ToString();
                    int spot_y_size = Convert.ToInt32(spot_y, 16)/2;                  
                    PublicClass.is_draw_type = "spot";
                    //shapes_name = "spot" + (i + 1);
                    //vertex_coordinate.Add(int.Parse(spot_x));
                    //vertex_coordinate.Add(int.Parse(spot_y));
                    //canvas_font_mouseup();  

                    PublicClass.shapes_property newspot = new PublicClass.shapes_property();
                    newspot.pixel_coordinate = new List<int>();
                    newspot.shapes_name = "spot" + (i + 1);
                    newspot.workspace_name = PublicClass.cur_ctrl_name;
                    newspot.max_temp = (int.Parse(ir_temp[spot_x_size + spot_y_size*ir_width].ToString()));
                    newspot.min_temp = newspot.max_temp;
                    newspot.pixel_coordinate.Add(spot_x_size + spot_y_size * ir_width);
                    newspot.avr_temp = newspot.min_temp;
                    newspot.percent = 0;
                    newspot.pixels_count = 1;
                    newspot.shapes_type = "spot";
                    List<double> abc = new List<double>();
                    abc.Add(spot_x_size);
                    abc.Add(spot_y_size);
                    newspot.vertex = abc;
                    List<int> abc_1 = new List<int>();
                    List<double> abc_2 = new List<double>();
                    abc_2.Add(1);
                    newspot.unique_temp = abc_1;
                    newspot.unique_percent = abc_2;
                    PublicClass.shapes_count.Add(newspot);

                }
            }

            for (int i = 0; i < 5; i++)
            {
                fs.Seek(jpg_size + 0x100 + area_size + 26 + i * 25, SeekOrigin.Begin);
                string area = br.ReadByte().ToString();
                if (int.Parse(area.ToString()) != 0)
                {
                    fs.Seek(jpg_size + 0x100 + area_size + 26 + 6 + i * 25, SeekOrigin.Begin);
                    string areal_x = len_2(br.ReadByte());
                    areal_x += len_2(br.ReadByte());
                    int areal_x_size = Convert.ToInt32(areal_x, 16)/2;
                    string areal_y = len_2(br.ReadByte());
                    areal_y += len_2(br.ReadByte());
                    int areal_y_size = Convert.ToInt32(areal_y, 16)/2;
                    string arear_x = len_2(br.ReadByte());
                    arear_x += len_2(br.ReadByte());
                    int arear_x_size = Convert.ToInt32(arear_x, 16)/2;
                    string arear_y = len_2(br.ReadByte());
                    arear_y += len_2(br.ReadByte());
                    int arear_y_size = Convert.ToInt32(arear_y, 16)/2;
                    PublicClass.is_draw_type = "area";
                  

                    PublicClass.shapes_property newspot = new PublicClass.shapes_property();
                    newspot.shapes_name = "area" + (i + 1);
                    newspot.workspace_name = PublicClass.cur_ctrl_name;
                    //newspot.max_temp = (int.Parse(ir_temp[spot_x_size + spot_y_size * ir_width].ToString()));
                    //newspot.min_temp = newspot.max_temp;
                    //newspot.avr_temp = newspot.min_temp;
                    newspot.percent = 0;
                    newspot.pixels_count = 0;
                    newspot.shapes_type = "area";
                    List<double> abc = new List<double>();
                    abc.Add(areal_x_size);
                    abc.Add(areal_y_size);
                    abc.Add(arear_x_size - areal_x_size);
                    abc.Add(arear_y_size - areal_y_size);
                    newspot.vertex = abc;
                    newspot.pixel_coordinate = new List<int>();
                    newspot.unique_temp = new List<int>();
                    newspot.unique_percent = new List<double>();
                    for (int y = (int)newspot.vertex[1]; y < newspot.vertex[3] + newspot.vertex[1]; y++)
                    {
                        for (int x = (int)newspot.vertex[0]; x < newspot.vertex[2] + newspot.vertex[0]; x++)
                        {
                            newspot.pixel_coordinate.Add(y*ir_width+x);
                            newspot.unique_temp.Add(int.Parse(ir_temp[y * ir_width + x].ToString()));
                        }
                    
                    }
                    newspot.max_temp = (from c in newspot.unique_temp select c).Max();
                    newspot.min_temp =  (from c in newspot.unique_temp select c).Min();
                    newspot.avr_temp =  (from c in newspot.unique_temp select c).Average();
                    PublicClass.shapes_count.Add(newspot);
               
                }

            }
            fs.Close();
 
        }


        private void ir_canvas_font_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock canv_temp = ir_canvas_font.FindName("canvtemp") as TextBlock;
            if (canv_temp != null)
            {
                ir_canvas_font.Children.Remove(canv_temp);
                ir_canvas_font.UnregisterName("canvtemp");
            }
        }

        public void get_max_min()
        {
            int max_ad = 0;
            int min_ad = 0;
            int max_wendu = -2730;
            int min_wendu = 1000000;
            for (int i = 0; i < ir_temp.Count; i++)
            {
                if (int.Parse(ir_temp[i].ToString()) > max_wendu)
                {
                    max_ad = i;
                    max_wendu = int.Parse(ir_temp[i].ToString());
                }
                else if (int.Parse(ir_temp[i].ToString()) < min_wendu)
                {
                    min_ad = i;
                    min_wendu = int.Parse(ir_temp[i].ToString());
                }
            }


            PublicClass.shapes_property newspot = new PublicClass.shapes_property();
            newspot.pixel_coordinate = new List<int>();
            //newspot.shapes_name = "[" + max_ad % ir_width * zoom_coe + "," + max_ad / ir_width * zoom_coe + "]" + "[" + "Max" + ":" + int.Parse(ir_temp[(int)((max_ad / ir_width) * ir_width) + (int)(max_ad % ir_width)].ToString()) / 10 + "℃" + "]";
            newspot.shapes_name = "max_temp";
            newspot.workspace_name = PublicClass.cur_ctrl_name;
            newspot.max_temp = int.Parse(ir_temp[(max_ad / ir_width) * ir_width + (int)(max_ad % ir_width)].ToString());
            newspot.min_temp = newspot.max_temp;
            newspot.pixel_coordinate.Add((int)(max_ad / ir_width) * ir_width + (int)(max_ad % ir_width));
            newspot.avr_temp = newspot.min_temp;
            newspot.percent = 0;
            newspot.pixels_count = 1;
            newspot.shapes_type = "max_temp_temp";
            newspot.vertex = new List<double>();
            newspot.vertex.Add((max_ad % ir_width) * zoom_coe);
            newspot.vertex.Add((max_ad / ir_width) * zoom_coe);

            newspot.unique_temp = new List<int>();
            newspot.unique_temp.Add((int)(max_ad % ir_width * zoom_coe));
            newspot.unique_temp.Add((int)(max_ad / ir_width * zoom_coe));

            List<double> abc_2 = new List<double>();
            abc_2.Add(1);
            newspot.unique_percent = abc_2;
            PublicClass.shapes_count.Add(newspot);



            PublicClass.shapes_property newmin = new PublicClass.shapes_property();
            newmin.pixel_coordinate = new List<int>();
            //newmin.shapes_name = "[" + min_ad  % ir_width * zoom_coe + "," + min_ad  / ir_width * zoom_coe + "]" + "[" + "Min" + ":" + int.Parse(ir_temp[(int)((min_ad  / ir_width) * ir_width) + (int)(min_ad  % ir_width)].ToString()) / 10 + "℃" + "]";
            newmin.shapes_name = "min_temp";
            newmin.workspace_name = PublicClass.cur_ctrl_name;
            newmin.max_temp = int.Parse(ir_temp[(min_ad / ir_width) * ir_width + (int)(min_ad % ir_width)].ToString());
            newmin.min_temp = newspot.max_temp;
            newmin.pixel_coordinate.Add((int)(min_ad / ir_width) * ir_width + (int)(min_ad % ir_width));
            newmin.avr_temp = newspot.min_temp;
            newmin.percent = 0;
            newmin.pixels_count = 1;
            newmin.shapes_type = "min_temp_temp";
            newmin.vertex = new List<double>();
            newmin.vertex.Add((min_ad % ir_width) * zoom_coe);
            newmin.vertex.Add((min_ad / ir_width) * zoom_coe);

            newmin.unique_temp = new List<int>();
            newmin.unique_temp.Add((int)(min_ad % ir_width * zoom_coe));
            newmin.unique_temp.Add((int)(min_ad / ir_width * zoom_coe));

            List<double> abc_3 = new List<double>();
            abc_3.Add(1);
            newmin.unique_percent = abc_3;
            PublicClass.shapes_count.Add(newmin);


        }
        public float repot_zoomcoe;
        public void repot_ready()
        {
            repot_zoomcoe = zoom_coe;
            zoom_coe = 1;
            zoom();
        }
        public void report_img()//报告截图
        {

            repot_image = new RenderTargetBitmap((int)ir_canvas.ActualWidth, (int)ir_canvas.ActualHeight, 96, 96, PixelFormats.Default);
            repot_image.Render(ir_canvas);

            report_shapes.Clear();
            zoom_coe = repot_zoomcoe;
            zoom();
            for (int i = 0; i < PublicClass.shapes_count.Count; i++)
            {
                PublicClass.shapes_property test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                int in_shapes = 0;
                in_shapes = (from c in selected_spot_area where c == test.shapes_name select c).Count();
                if (in_shapes > 0 && test.workspace_name == PublicClass.cur_ctrl_name)
                {
                    report_shapes.Add(test);
                }
                else if (test.shapes_type != "spot" && test.shapes_type != "area" && test.workspace_name == PublicClass.cur_ctrl_name && test.shapes_type != "max_temp_temp" && test.shapes_type != "min_temp_temp")
                {
                    report_shapes.Add(test);
                }
            }
           
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (PublicClass.is_draw_type == "adjust" && PublicClass.ronghe_type)
                {


                    if (drawpoint.X > ir_back_img.Width + ir_back_img.Margin.Left - 20 && drawpoint.X < ir_back_img.Width + ir_back_img.Margin.Left + 20 && (move_dir == "" || move_dir == "right") && (e.GetPosition(back_canvas).Y > ir_back_img.Margin.Top + 100 || e.GetPosition(back_canvas).Y < ir_back_img.Margin.Top + ir_back_img.Height - 100))
                    {
                        back_canvas.Cursor = Cursors.SizeWE;
                        ir_back_img.Width = ir_back_img.Width + (e.GetPosition(back_canvas).X - drawpoint.X);
                        drawpoint = e.GetPosition(back_canvas);
                        move_dir = "right";
                    }
                    else if (drawpoint.X > ir_back_img.Margin.Left - 20 && drawpoint.X < ir_back_img.Margin.Left + 20 && (move_dir == "" || move_dir == "left") && (e.GetPosition(back_canvas).Y > ir_back_img.Margin.Top + 100 || e.GetPosition(back_canvas).Y < ir_back_img.Margin.Top + ir_back_img.Height - 100))
                    {
                        back_canvas.Cursor = Cursors.SizeWE;
                        ir_back_img.Width = ir_back_img.Width + (drawpoint.X - e.GetPosition(back_canvas).X);
                        ir_back_img.Margin = new Thickness(rongheleft + e.GetPosition(back_canvas).X - rongheleft, ronghetop, 0, 0);
                        drawpoint = e.GetPosition(back_canvas);
                        move_dir = "left";
                    }
                    else if (drawpoint.Y > ir_back_img.Height + ir_back_img.Margin.Top - 20 && drawpoint.Y < ir_back_img.Width + ir_back_img.Margin.Top + 20 && (move_dir == "" || move_dir == "bottom") && (e.GetPosition(back_canvas).X > ir_back_img.Margin.Left + 100 || e.GetPosition(back_canvas).X < ir_back_img.Margin.Left + ir_back_img.Width - 100))
                    {
                        back_canvas.Cursor = Cursors.SizeNS;
                        ir_back_img.Height = ir_back_img.Height + (e.GetPosition(back_canvas).Y - drawpoint.Y);
                        drawpoint = e.GetPosition(back_canvas);
                        move_dir = "bottom";
                    }
                    else if (drawpoint.Y > ir_back_img.Margin.Top - 20 && drawpoint.Y < ir_back_img.Margin.Top + 20 && (move_dir == "" || move_dir == "top") && (e.GetPosition(back_canvas).X > ir_back_img.Margin.Left + 100 || e.GetPosition(back_canvas).X < ir_back_img.Margin.Left + ir_back_img.Width - 100))
                    {
                        back_canvas.Cursor = Cursors.SizeNS;
                        ir_back_img.Height = ir_back_img.Height + (drawpoint.Y - e.GetPosition(back_canvas).Y);
                        ir_back_img.Margin = new Thickness(rongheleft, ronghetop + e.GetPosition(back_canvas).Y - ronghetop, 0, 0);
                        drawpoint = e.GetPosition(back_canvas);
                        move_dir = "top";
                    }
                    else if (move_dir == "" || move_dir == "move")
                    {
                        back_canvas.Cursor = Cursors.SizeAll;
                        ir_back_img.Margin = new Thickness((e.GetPosition(back_canvas).X - drawpoint.X) + rongheleft, ronghetop + (e.GetPosition(back_canvas).Y - drawpoint.Y), 0, 0);
                        move_dir = "move";
                    }
                }
            }
        }

        public void rad_table()
        {
            Tamb_TiEmiss_cal(TempDist, TempTamb);
            //ir_temp.Clear();
            //init_temp();
            //calculate_temp_ruler();//生成温度标尺
            for (int i = 0; i < PublicClass.shapes_count.Count; i++)
            {
                PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                {
                    if (newshapes.workspace_name == PublicClass.cur_ctrl_name)
                    {
                        newshapes.percent = 0;
                        PublicClass.shapes_count[i] = newshapes;
                    }
                }
            }
            all_area_temp();
            //calculate_percent();
                try
                {
                  
                    sub_shapes_list shap = MainWindow.FindChild<sub_shapes_list>(Application.Current.MainWindow, "shapestest");
                    //string Ems = shap.dataGrid.CurrentCell.ToString();
                    
                    shap.shapeslist("all", 0);
                }
                catch
                {

                }
          
     
        }


        private void GetCharSpellCode()
        {
           // long iCnChar;
            string ss = shapes_name;
            byte[] ZW = System.Text.Encoding.Default.GetBytes(shapes_name);

            //如果是字母，则直接返回 
            if (ZW.Length == 1)
            {
                shapes_name = shapes_name.ToUpper();
            }
            else
            {
                //   get   the     array   of   byte   from   the   single   char    
                int i1 = (short)(ZW[0]);
                int i2 = (short)(ZW[1]);
                shapes_name = shapes_name; (i1 * 256 + i2).ToString();
            }


        }



    }
}
