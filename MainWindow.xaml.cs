using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Win32;
using Xceed.Wpf.AvalonDock.Layout;
using System.Data;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;
using System.Collections;
using System.Windows.Media.Media3D;
using C1.WPF.RichTextBox.Documents;
using C1.WPF.RichTextBox;
using System.Text.RegularExpressions;
using System.Threading;
using C1.WPF.SpellChecker;
using System.Reflection;


namespace IrAnalyse
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }


       
        sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
        sub_shapes_list shapes_list = MainWindow.FindChild<sub_shapes_list>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);

        public sub_repot_ready report;
        public string img_path;//图片路径
       
        public ArrayList img_file = new ArrayList();//图片完整路径
        public ArrayList img_name = new ArrayList();//图片名字
        public string fileName { get; set; }
        List<byte> yuv_data = new List<byte>();
        public List<YUV> yuv_list = new List<YUV>();
        public string[] files;
        public int file_index = -1;
        public List<int> rgb_list = new List<int>();
        System.Timers.Timer newtime = new System.Timers.Timer();
       
        public ArrayList vr_information { get; set; }
       

        //YUV数据量
        private static int YUV_WIDTH = 768;
        private static int YUV_HEIGHT = 576;
        public string filename;
        public string safe_filename;
        public bool is_visible = false;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            newtime.Elapsed += new System.Timers.ElapsedEventHandler(newtime_Elapsed);
            //tool1.DockAsDocument();
            init_toolbar();
            initpalette();
            submainwindow.Width = maingrid.ActualWidth;
            submainwindow.Height = maingrid.ActualHeight;
            sub_shapes_list shapeslist1 = new sub_shapes_list();
            shapeslist1.Name = "shapestest";
            list.Children.Add(shapeslist1);
            list.RegisterName("shapestest",shapeslist1);
            sub_spot spot1 = new sub_spot();
            spot1.Name = "spottest";
            spot.Children.Add(spot1);
            if (!System.IO.Directory.Exists(System.IO.Path.GetTempPath()+"\\IrAnalyse"))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetTempPath()+"\\IrAnalyse");
            }

          //  repot_ready.Height = tool4.

        }

        void newtime_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string kmyz = "";
                is_visible = false;

               
                    file_index++;
                    if (file_index < files.Count())
                    {
                        string file = files[file_index];

                        FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
                        byte[] byData = new byte[fs.Length];
                        BinaryReader br = new BinaryReader(fs);
                        fs.Seek(0x18, SeekOrigin.Begin);//从18h字开始
                        string jpg_size_str = len_2(br.ReadByte());
                        jpg_size_str += len_2(br.ReadByte());
                        jpg_size_str += len_2(br.ReadByte());
                        jpg_size_str += len_2(br.ReadByte());
                        int jpg_size = Convert.ToInt32(jpg_size_str, 16);//获取JPG 文件长度

                        try
                        {
                            ////生产商字串////
                           


                            fs.Seek(jpg_size + 0x6, SeekOrigin.Begin);
                            for (int i = 0; i < 16; i++)
                            {
                                int asc_code = int.Parse(br.ReadByte().ToString());
                                if (asc_code > 0)
                                {
                                    kmyz += ((char)asc_code).ToString();

                                }
                                else
                                {
                                    break;
                                }
                            }

                        }
                        catch { }

                   


                        fs.Seek(0x24, SeekOrigin.Begin);//从23h字开始
                        string jpg_str = len_2(br.ReadByte());
                        if (jpg_str == "01")
                        {
                            is_visible = true;
                        }

                        fs.Seek(jpg_size + 6144, SeekOrigin.Begin);
                        bool is_ir = true;
                        try
                        {
                            br.ReadByte();
                        }
                        catch
                        {
                            is_ir = false;
                        }
                        br.Close();
                        fs.Close();

                        if (is_ir && System.IO.Path.GetExtension(file).ToLower() == ".jpg" && !is_visible&&kmyz.ToLower()=="kmyz")
                        {






                                MemoryStream ms = new MemoryStream(byData);
                                string fn = @file;
                                img_file.Add(fn);




                                Thread newthread = new Thread(new ThreadStart(() =>
        {
            Dispatcher.Invoke(new Action(() =>
                {

                    try
                    {
                        test23.Title = file.Substring(file.LastIndexOf("\\") + 1, file.Length - file.LastIndexOf("\\") - 1);
                    }
                    catch { }

                    sub_open subana = new sub_open();

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(fn);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    subana.open_img.Source = bitmap;
                    subana.open_text.Text = System.IO.Path.GetFileName(file);
                    int i = 0;
                    subana.Name = "sub" + i;
                    i++;
                    //subana.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(subana_PreviewMouseLeftButtonDown);
                    img_name.Add(System.IO.Path.GetFileName(file));
                    Image_query.Items.Add(subana);
                    ms.Dispose();


                    if (file_index == files.Count() - 1)
                    {
                        test23.Title = "打开热图";
                    }




                }));



        }));
                                newthread.SetApartmentState(ApartmentState.STA);
                                newthread.IsBackground = true;
                                //newthread.Priority = ThreadPriority.AboveNormal;
                                newthread.Start();









                            }

                        }
                        else
                        {
                            newtime.Stop();
                            file_index = -1;
                            //test23.Title = "打开热图";
                        }







             
              

         
        }

        public void create_ir_information()  //生成图片信息
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            //新建列
            DataColumn col1 = new DataColumn("名称", typeof(string));
            DataColumn col2 = new DataColumn("值", typeof(string));
            DataColumn col3 = new DataColumn("名称", typeof(string));
            DataColumn col4 = new DataColumn("值", typeof(string));
            DataColumn col5 = new DataColumn("名称", typeof(string));
            DataColumn col6 = new DataColumn("值", typeof(string));
            dt1.Columns.Add(col1);
            dt1.Columns.Add(col2);
            dt2.Columns.Add(col3);
            dt2.Columns.Add(col4);
            dt3.Columns.Add(col5);
            dt3.Columns.Add(col6);
            //新建行
            DataRow row1 = dt1.NewRow();
            //行赋值
            row1["名称"] = "生产商";
            row1["值"] = workspace.ir_information[0];
            DataRow row2 = dt1.NewRow();
            row2["名称"] = "序列号";
            row2["值"] = workspace.ir_information[1];
            //DataRow row3 = dt1.NewRow();
            //row3["名称"] = "机器生产日期";
            //row3["值"] = workspace.ir_information[2];
            DataRow row4 = dt1.NewRow();
            row4["名称"] = "拍摄时间";
            row4["值"] = "20" + workspace.ir_information[3].ToString().Substring(0, 2) + "-" + workspace.ir_information[3].ToString().Substring(2, 2) + "-" + workspace.ir_information[3].ToString().Substring(4, 2) + " " + workspace.ir_information[3].ToString().Substring(6, 2) + ":" + workspace.ir_information[3].ToString().Substring(8, 2) + ":" + workspace.ir_information[3].ToString().Substring(10, 2);
            DataRow row13 = dt1.NewRow();
            row13["名称"] = "图片大小";
            row13["值"] = workspace.ir_information[6]+"*"+workspace.ir_information[7];
            DataRow row5 = dt1.NewRow();
            row5["名称"] = "镜头号";
            row5["值"] = workspace.ir_information[4];
            DataRow row6 = dt1.NewRow();
            row6["名称"] = "测温档";
            row6["值"] = workspace.ir_information[5];
            DataRow row7 = dt3.NewRow();
            row7["名称"] = "环境温度";
            row7["值"] =(float.Parse( workspace.ir_information[10].ToString())/10).ToString() + " ℃";
            DataRow row8 = dt3.NewRow();
            row8["名称"] = "相对湿度";
            row8["值"] = workspace.ir_information[11]+" %";
            DataRow row9 = dt2.NewRow();
            row9["名称"] = "传感器温度";
            row9["值"] = Convert.ToInt32(workspace.ir_information[14].ToString().Substring(0, 2), 16) + (float)(Convert.ToInt32(workspace.ir_information[14].ToString().Substring(2, 2), 16)) / 16 * 0.0625 + " ℃";
            //DataRow row10 = dt.NewRow();
            //row10["名称"] = "传感器温度修正";
            //row10["值"] = Convert.ToInt32(workspace.ir_information[15].ToString().Substring(0, 2), 16) + (float)(Convert.ToInt32(workspace.ir_information[15].ToString().Substring(2, 2), 16)) / 16 * 0.0625 + " ℃";
            DataRow row11 = dt3.NewRow();
            row11["名称"] = "辐射率";
            row11["值"] = float.Parse(workspace.ir_information[16].ToString())/100;
            DataRow row12 = dt3.NewRow();
            row12["名称"] = "距离";
            row12["值"] = workspace.ir_information[17]+" m";

            DataRow row14 = dt2.NewRow();
            row14["名称"] = "最高温度";
            row14["值"] = float.Parse(workspace.temp_max_min_avr[0].ToString()) / 10 + " ℃" + " { X=" + int.Parse(workspace.temp_max_min_avr[1].ToString()) % int.Parse(workspace.ir_information[6].ToString()) + "," + "Y=" + int.Parse(workspace.temp_max_min_avr[1].ToString()) / int.Parse(workspace.ir_information[6].ToString())+" }";
            DataRow row15 = dt2.NewRow();
            row15["名称"] = "最低温度";
            row15["值"] = float.Parse(workspace.temp_max_min_avr[2].ToString()) / 10 + " ℃" + " { X=" + int.Parse(workspace.temp_max_min_avr[3].ToString()) % int.Parse(workspace.ir_information[6].ToString()) + "," + "Y=" + int.Parse(workspace.temp_max_min_avr[3].ToString()) / int.Parse(workspace.ir_information[6].ToString()) + " }";
            DataRow row16 = dt2.NewRow();
            row16["名称"] = "平均温度";
            row16["值"] = float.Parse(workspace.temp_max_min_avr[4].ToString()) / 10 + " ℃";
            DataRow row17 = dt2.NewRow();
            row17["名称"] = "中心点温度";
            row17["值"] = float.Parse(workspace.temp_max_min_avr[5].ToString()) / 10 + " ℃" + " { X=" + int.Parse(workspace.temp_max_min_avr[6].ToString()) % int.Parse(workspace.ir_information[6].ToString()) + "," + "Y=" + int.Parse(workspace.temp_max_min_avr[6].ToString()) / int.Parse(workspace.ir_information[6].ToString()) + " }";
            DataRow row18 = dt3.NewRow();
            row18["名称"] = "温度修正";
            row18["值"] = workspace.modify_temp + " ℃";
            //添加行
            dt1.Rows.Add(row1);
            dt1.Rows.Add(row2);
           // dt.Rows.Add(row3);
            dt1.Rows.Add(row4);
            dt1.Rows.Add(row13);
            dt1.Rows.Add(row5);
            dt1.Rows.Add(row6);
            dt3.Rows.Add(row7);
            dt3.Rows.Add(row8);
            dt2.Rows.Add(row9);
           // dt.Rows.Add(row10);
            dt3.Rows.Add(row11);
            dt3.Rows.Add(row12);
            dt2.Rows.Add(row14);
            dt2.Rows.Add(row15);
            dt2.Rows.Add(row16);
            dt2.Rows.Add(row17);
            dt3.Rows.Add(row18);
            pic_information.ItemsSource = null;
            
            //数据绑定
            pic_information.ColumnWidth = (pic_information.ActualWidth - 2) / 2;
            pic_information.ItemsSource = dt1.DefaultView;
            pic_information.CanUserAddRows = false;
            pic_information.IsReadOnly = true;

            pic_temp_tag.ItemsSource = null;
            
            //数据绑定
            pic_temp_tag.ColumnWidth = (pic_information.ActualWidth - 2) / 2;
            pic_temp_tag.ItemsSource = dt2.DefaultView;
            pic_temp_tag.CanUserAddRows = false;
            pic_temp_tag.IsReadOnly = true;

            //pic_temp_para.ItemsSource = null;
            
            //数据绑定
            if (pic_temp_para.ItemsSource == null)
            {
                pic_temp_para.ColumnWidth = (pic_information.ActualWidth - 2) / 2;
                pic_temp_para.ItemsSource = dt3.DefaultView;
                pic_temp_para.CanUserAddRows = false;
                pic_temp_para.IsReadOnly = false;
            }

        }

        public void create_vr_information()  //生成图片信息
        {
           // sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            //新建列
            DataColumn col1 = new DataColumn("名称", typeof(string));
            DataColumn col2 = new DataColumn("值", typeof(string));
            DataColumn col3 = new DataColumn("名称", typeof(string));
            DataColumn col4 = new DataColumn("值", typeof(string));
            DataColumn col5 = new DataColumn("名称", typeof(string));
            DataColumn col6 = new DataColumn("值", typeof(string));
            dt1.Columns.Add(col1);
            dt1.Columns.Add(col2);
            dt2.Columns.Add(col3);
            dt2.Columns.Add(col4);
            dt3.Columns.Add(col5);
            dt3.Columns.Add(col6);
            //新建行
            DataRow row1 = dt1.NewRow();
            //行赋值
            row1["名称"] = "生产商";
            row1["值"] = vr_information[0];
            DataRow row2 = dt1.NewRow();
            row2["名称"] = "序列号";
            row2["值"] = vr_information[1];
            //DataRow row3 = dt1.NewRow();
            //row3["名称"] = "机器生产日期";
            //row3["值"] = vr_information[2];
            DataRow row4 = dt1.NewRow();
            row4["名称"] = "拍摄时间";
            row4["值"] = "20" + vr_information[3].ToString().Substring(0, 2) + "-" + vr_information[3].ToString().Substring(2, 2) + "-" +vr_information[3].ToString().Substring(4, 2) + " " + vr_information[3].ToString().Substring(6, 2) + ":" + vr_information[3].ToString().Substring(8, 2) + ":" + vr_information[3].ToString().Substring(10, 2);
            DataRow row13 = dt1.NewRow();
            row13["名称"] = "图片大小";
            row13["值"] = vr_information[6] + "*" + vr_information[7];
            DataRow row5 = dt1.NewRow();
            row5["名称"] = "镜头号";
            row5["值"] = vr_information[4];
            DataRow row6 = dt1.NewRow();
            row6["名称"] = "测温档";
            row6["值"] = vr_information[5];
            DataRow row7 = dt3.NewRow();
            row7["名称"] = "环境温度";
            row7["值"] = (float.Parse(vr_information[10].ToString()) / 10).ToString() + " ℃";
            DataRow row8 = dt3.NewRow();
            row8["名称"] = "相对湿度";
            row8["值"] = vr_information[11] + " %";
            DataRow row9 = dt2.NewRow();
            row9["名称"] = "传感器温度";
            row9["值"] = Convert.ToInt32(vr_information[14].ToString().Substring(0, 2), 16) + (float)(Convert.ToInt32(vr_information[14].ToString().Substring(2, 2), 16)) / 16 * 0.0625 + " ℃";
            //DataRow row10 = dt.NewRow();
            //row10["名称"] = "传感器温度修正";
            //row10["值"] = Convert.ToInt32(workspace.vr_information[15].ToString().Substring(0, 2), 16) + (float)(Convert.ToInt32(workspace.vr_information[15].ToString().Substring(2, 2), 16)) / 16 * 0.0625 + " ℃";
            DataRow row11 = dt3.NewRow();
            row11["名称"] = "辐射率";
            row11["值"] = float.Parse(vr_information[16].ToString()) / 100;
            DataRow row12 = dt3.NewRow();
            row12["名称"] = "距离";
            row12["值"] =vr_information[17] + " m";

        
            //添加行
            dt1.Rows.Add(row1);
            dt1.Rows.Add(row2);
            // dt.Rows.Add(row3);
            dt1.Rows.Add(row4);
            dt1.Rows.Add(row13);
            dt1.Rows.Add(row5);
            dt1.Rows.Add(row6);
            dt3.Rows.Add(row7);
            dt3.Rows.Add(row8);
            dt2.Rows.Add(row9);
            // dt.Rows.Add(row10);
            dt3.Rows.Add(row11);
            dt3.Rows.Add(row12);
      
            pic_information.ItemsSource = null;

            //数据绑定
            pic_information.ColumnWidth = (pic_information.ActualWidth - 2) / 2;
            pic_information.ItemsSource = dt1.DefaultView;
            pic_information.CanUserAddRows = false;
            pic_information.IsReadOnly = true;

            pic_temp_tag.ItemsSource = null;

            //数据绑定
            pic_temp_tag.ColumnWidth = (pic_information.ActualWidth - 2) / 2;
            pic_temp_tag.ItemsSource = dt2.DefaultView;
            pic_temp_tag.CanUserAddRows = false;
            pic_temp_tag.IsReadOnly = true;

            //pic_temp_para.ItemsSource = null;

            //数据绑定
           
                pic_temp_para.ColumnWidth = (pic_information.ActualWidth - 2) / 2;
                pic_temp_para.ItemsSource = dt3.DefaultView;
                pic_temp_para.CanUserAddRows = false;
                pic_temp_para.IsReadOnly = true;
            

        }

        private void window_init()//初始化
        {
            menu1.Width = this.Width;
        }

        private void palette_Click(object sender, RoutedEventArgs e)//调用调色板
        {
            //palette newpalette = new palette();
            //newpalette.Owner = this;
            //newpalette.ShowDialog();


            //WindowInteropHelper parentHelper = new WindowInteropHelper(this);
            //WindowInteropHelper childHelper = new WindowInteropHelper(newpalette);
            //Win32Native.SetParent(childHelper.Handle, parentHelper.Handle);



            subwindow_content.Children.Clear();
            sub_palette newpalette = new sub_palette();
            subwindow_content.Children.Add(newpalette);
            subwindow.Width = 480;
            subwindow.Height = 450;
            subwindow.Opacity = 0.9;
            subwindow.Margin = new Thickness((submainwindow.Width - subwindow.Width) / 2, (submainwindow.Height - subwindow.Height) / 2, 0, 0);
            Panel.SetZIndex(submainwindow, 3000);
            //sub_workspace newworkspace = new sub_workspace();
            //subwindow_content.Children.Add(newworkspace);
            


        }


        private void sub_window_close_Click(object sender, RoutedEventArgs e)//关闭弹出窗口
        {
            Panel.SetZIndex(submainwindow, -1);
            subwindow.Opacity = 0;
            PublicClass.report_type = "workreport";
            if (PublicClass.open_newisothermal)
            {
                sub_isothermal newisothermal = MainWindow.FindChild<sub_isothermal>(Application.Current.MainWindow, "newisothermal");
                if (newisothermal != null)
                {
                    sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);

                    workspace.isothermal_list.Clear();
                    workspace.isothermal_list = (ArrayList)newisothermal.old_isothermal.Clone();
                    workspace.create_img();
                }
                PublicClass.open_newisothermal = false;
            }
        }


        public static T FindChild<T>(DependencyObject parent, string childName)//查找控件
    where T : DependencyObject
        {
            if (parent == null) return null;
            T foundChild = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // 如果子控件不是需查找的控件类型 
                T childType = child as T;
                if (childType == null)
                {
                    // 在下一级控件中递归查找 
                    foundChild = FindChild<T>(child, childName);
                    // 找到控件就可以中断递归操作  
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // 如果控件名称符合参数条件 
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // 查找到了控件 
                    foundChild = (T)child;
                    break;
                }
            }
            return foundChild;
        }


        private void open_visible()//打开可见光图
        {



            int yuv_y1;
            int yuv_y2;
            int yuv_u;
            int yuv_v;

            int sumY1 = 0;
            int sumY2 = 0;
            int sumU = 0;
            int sumV = 0;

            int[] pixels = new int[YUV_WIDTH * YUV_HEIGHT];

            for (int i = 0; i < YUV_HEIGHT; i++)
            {
                for (int j = 0; j < YUV_WIDTH / 2; j++)
                {
                    yuv_y1 = yuv_data[i * YUV_WIDTH * 2 + 4 * j + 0];
                    yuv_u = yuv_data[i * YUV_WIDTH * 2 + 4 * j + 1];
                    yuv_y2 = yuv_data[i * YUV_WIDTH * 2 + 4 * j + 2];
                    yuv_v = yuv_data[i * YUV_WIDTH * 2 + 4 * j + 3];

                    //计算Y值，U值，V值总和
                    sumY1 += yuv_y1;
                    sumY2 += yuv_y2;
                    sumU += yuv_u;
                    sumV += yuv_v;

                    pixels[i * YUV_WIDTH + 2 * j] = yuv_to_rgb(yuv_y1, yuv_u, yuv_v);
                    pixels[i * YUV_WIDTH + 2 * j + 1] = yuv_to_rgb(yuv_y2, yuv_u, yuv_v);

                }
            }

            //计算Y U V平均值
            avg_Y1 = sumY1 / (YUV_WIDTH * YUV_HEIGHT / 2);
            avg_U = sumU / (YUV_WIDTH * YUV_HEIGHT / 2);
            avg_Y2 = sumY2 / (YUV_WIDTH * YUV_HEIGHT / 2);
            avg_V = sumV / (YUV_WIDTH * YUV_HEIGHT / 2);

            splitYUVdata();
            //caculateKvalue(avg_Y, avg_U, avg_V);

            create_visible_img(pixels);


        }

    

        private int avg_Y1;
        private int avg_U;
        private int avg_Y2;
        private int avg_V;




        private int yuv_to_rgb(int Y, int U, int V)
        {

            double R = 0;
            double G = 0;
            double B = 0;

            R = Y + 1.371 * (V - 128);
            G = Y - 0.336 * (U - 128) - 0.698 * (V - 128);
            B = Y + 1.732 * (U - 128);


            if (R < 0) R = 0;
            if (R > 255) R = 255;
            if (G < 0) G = 0;
            if (G > 255) G = 255;
            if (B < 0) B = 0;
            if (B > 255) B = 255;

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

            int temp = Convert.ToInt32(R_str + G_str + B_str, 16);

            return temp;
        }

        public class YUV
        {
            public byte Y1, U, Y2, V;
        }




        private void create_visible_img(int[] pixels)
        {
            sub_img newima = new sub_img();
            int stride = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
            WriteableBitmap ir_bmp = new WriteableBitmap(YUV_WIDTH, YUV_HEIGHT, 96, 96, PixelFormats.Bgr32, null);
            ir_bmp.Lock();
            ir_bmp.WritePixels(new Int32Rect(0, 0, YUV_WIDTH, YUV_HEIGHT), pixels, stride * YUV_WIDTH, 0);
            ir_bmp.Unlock();
            newima.opinion_img.Source = ir_bmp;
            //img.Source = ir_bmp;
          
            LayoutDocument newaaa = new LayoutDocument();
            newaaa.Content = newima;
            newaaa.Title = "[可见光图像]" + safe_filename;
            newaaa.IsSelected = true;
            
            newima.LoadVsInformation += new RoutedPropertyChangedEventHandler<object>(newimage_LoadVsInformation);
           // newaaa.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(newaaa_Closing);

            mainpanel.Children.Add(newaaa); 
           

        }

        void newimage_LoadVsInformation(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

            
            get_visible();
            create_vr_information();
            Panel.SetZIndex(submainwindow, -1);
            subwindow.Opacity = 0;

            //submainwindow.Opacity = 0;
          //  PublicClass.cur_ctrl_type = "image";
            init_toolbar();
            saveimg.Source = new BitmapImage(new Uri(@"toolbar_img\save_back.png", UriKind.Relative));
            repot_ready_img.Source = new BitmapImage(new Uri(@"toolbar_img\report_ready_black.png", UriKind.Relative));
            max_temp_img.Source = new BitmapImage(new Uri(@"toolbar_img\temp_max_back.png", UriKind.Relative));
            min_temp_img.Source = new BitmapImage(new Uri(@"toolbar_img\temp_min_back.png", UriKind.Relative));
            cur_temp_img.Source = new BitmapImage(new Uri(@"toolbar_img\temp_cur_back.png", UriKind.Relative));
            zoomin_img.Source = new BitmapImage(new Uri(@"toolbar_img\zoom_in_back.png", UriKind.Relative));
            zoomout_img.Source = new BitmapImage(new Uri(@"toolbar_img\zoom_out_back.png", UriKind.Relative));
            deleteshapes_img.Source = new BitmapImage(new Uri(@"toolbar_img\delete_back.png", UriKind.Relative));

            undo_img.Source = new BitmapImage(new Uri(@"toolbar_img\undo_back.png", UriKind.Relative));
            redo_img.Source = new BitmapImage(new Uri(@"toolbar_img\redo_back.png", UriKind.Relative));


            draw_adjust_img.Source = new BitmapImage(new Uri(@"toolbar_img\adjust_back.png", UriKind.Relative));
            draw_point_img.Source = new BitmapImage(new Uri(@"toolbar_img\point_back.png", UriKind.Relative));
            draw_line_img.Source = new BitmapImage(new Uri(@"toolbar_img\line_back.png", UriKind.Relative));
            draw_brokenline_img.Source = new BitmapImage(new Uri(@"toolbar_img\cur_back.png", UriKind.Relative));
            draw_circular_img.Source = new BitmapImage(new Uri(@"toolbar_img\ellipse_back.png", UriKind.Relative));
            draw_rectangle_img.Source = new BitmapImage(new Uri(@"toolbar_img\rectangle_back.png", UriKind.Relative));
            draw_polygon_img.Source = new BitmapImage(new Uri(@"toolbar_img\pol_back.png", UriKind.Relative));

            draw_Isothermal_img.Source = new BitmapImage(new Uri(@"toolbar_img\isothermal_back.png", UriKind.Relative));
            draw_palette_img.Source = new BitmapImage(new Uri(@"toolbar_img\palette_back.png", UriKind.Relative));
            draw_bc_adjust_img.Source = new BitmapImage(new Uri(@"toolbar_img\bc_adjust_back.png", UriKind.Relative));
            draw_dismap_img.Source = new BitmapImage(new Uri(@"toolbar_img\fenbu_back.png", UriKind.Relative));
            draw_scale_img.Source = new BitmapImage(new Uri(@"toolbar_img\bili_back.png", UriKind.Relative));
            draw_3d_img.Source = new BitmapImage(new Uri(@"toolbar_img\3d_back.png", UriKind.Relative));


            stackpanel_tool.Opacity = 1;
            spot.Opacity = 0;
            list.Opacity = 0;
            

        }



        private void splitYUVdata()
        {
            if (yuv_list.Capacity > 0)
                yuv_list.Clear();

            for (int i = 0; i < YUV_HEIGHT; i++)
            {
                for (int j = 0; j < YUV_WIDTH / 2; j++)
                {
                    YUV yuv = new YUV();
                    yuv.Y1 = yuv_data[i * YUV_WIDTH * 2 + 4 * j + 0];
                    yuv.U = yuv_data[i * YUV_WIDTH * 2 + 4 * j + 1];
                    yuv.Y2 = yuv_data[i * YUV_WIDTH * 2 + 4 * j + 2];
                    yuv.V = yuv_data[i * YUV_WIDTH * 2 + 4 * j + 3];

                    yuv_list.Add(yuv);

                }
            }
        }

        private void get_visible()
        {
            
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            fs.Seek(0x18, SeekOrigin.Begin);//从18h字开始
            string jpg_size_str = len_2(br.ReadByte());
            jpg_size_str += len_2(br.ReadByte());
            jpg_size_str += len_2(br.ReadByte());
            jpg_size_str += len_2(br.ReadByte());
            int jpg_size = Convert.ToInt32(jpg_size_str, 16);//获取JPG 文件长度


            ////生产商字串ir_information[0]////
            vr_information = new ArrayList();
            vr_information.Add("");
            fs.Seek(jpg_size + 0x6, SeekOrigin.Begin);
            for (int i = 0; i < 16; i++)
            {
                int asc_code = int.Parse(br.ReadByte().ToString());
                if (asc_code > 0)
                {
                    vr_information[0] += ((char)asc_code).ToString();
                }
                else
                {
                    break;
                }
            }


            ///机器序列号字串ir_information[1]/////
            vr_information.Add("");
            fs.Seek(jpg_size + 0x16, SeekOrigin.Begin);
            for (int i = 0; i < 16; i++)
            {
                int asc_code = int.Parse(br.ReadByte().ToString());
                if (asc_code > 0)
                {
                    vr_information[1] += ((char)asc_code).ToString();
                }
                else
                {
                    break;
                }
            }


            ///机器生产日期字串 ir_information[2]////
            vr_information.Add("");
            fs.Seek(jpg_size + 0x26, SeekOrigin.Begin);
            for (int i = 0; i < 16; i++)
            {
                int asc_code = int.Parse(br.ReadByte().ToString());
                if (asc_code > 0)
                {
                    vr_information[2] += ((char)asc_code).ToString();
                }
                else
                {
                    break;
                }
            }


            ////图片存储时间ir_information[3]////
            vr_information.Add("");
            fs.Seek(jpg_size + 0x46, SeekOrigin.Begin);
            for (int i = 0; i < 6; i++)
            {
                vr_information[3] += len_2(br.ReadByte());
            }
            /////镜头号ir_information[4]/////
            vr_information.Add("");
            fs.Seek(jpg_size + 0x56, SeekOrigin.Begin);
            vr_information[4] += int.Parse(len_2(br.ReadByte()) + len_2(br.ReadByte())).ToString();
            ///////测温档,ir_information[5]/////
            vr_information.Add("");
            fs.Seek(jpg_size + 0x58, SeekOrigin.Begin);
            vr_information[5] += int.Parse(len_2(br.ReadByte()) + len_2(br.ReadByte())).ToString();
            //////图象宽度ir_information[6]/////
            vr_information.Add("");
            fs.Seek(0xbc, SeekOrigin.Begin);
            vr_information[6] += len_2(br.ReadByte());
            vr_information[6] = vr_information[6] + len_2(br.ReadByte());
            vr_information[6] = Convert.ToInt32(vr_information[6].ToString(), 16).ToString();
            //vr_information[6] = 768;
            //////图象高度ir_information[7]/////
            vr_information.Add("");
            fs.Seek(0xba, SeekOrigin.Begin);
            vr_information[7] += len_2(br.ReadByte());
            vr_information[7] = vr_information[7] + len_2(br.ReadByte());
            vr_information[7] = Convert.ToInt32(vr_information[7].ToString(), 16).ToString();
            /////CCD图象宽度 ir_information[8]///////
            vr_information.Add("");
            fs.Seek(jpg_size + 0x6a, SeekOrigin.Begin);
            vr_information[8] += len_2(br.ReadByte());
            vr_information[8] = len_2(br.ReadByte()) + vr_information[8];
            vr_information[8] = Convert.ToInt32(vr_information[8].ToString(), 16).ToString();
            /////CCD图象高度 ir_information[9]///////
            vr_information.Add("");
            fs.Seek(jpg_size + 0x6c, SeekOrigin.Begin);
            vr_information[9] += len_2(br.ReadByte());
            vr_information[9] = len_2(br.ReadByte()) + vr_information[9];
            vr_information[9] = Convert.ToInt32(vr_information[9].ToString(), 16).ToString();
            /////环境温度 ir_information[10]/////
            vr_information.Add("");
            fs.Seek(jpg_size + 0x100 + 32, SeekOrigin.Begin);
            vr_information[10] += (len_2(br.ReadByte()) + len_2(br.ReadByte())).ToString();
            vr_information[10] = (Convert.ToInt32(vr_information[10].ToString(), 16) - 600).ToString();
            //////相对湿度 ir_information[11]//////
            vr_information.Add("");
            fs.Seek(jpg_size + 0x100 + 34, SeekOrigin.Begin);
            vr_information[11] += Convert.ToInt32(len_2(br.ReadByte()), 16).ToString();
            ///////level值 ir_information[12]//////
            vr_information.Add("");
            fs.Seek(jpg_size + 0x100 + 42, SeekOrigin.Begin);
            vr_information[12] += Convert.ToInt32(len_2(br.ReadByte()) + len_2(br.ReadByte()), 16).ToString();
            ///////Width值 ir_information[13]//////
            vr_information.Add("");
            fs.Seek(jpg_size + 0x100 + 44, SeekOrigin.Begin);
            vr_information[13] += Convert.ToInt32(len_2(br.ReadByte()) + len_2(br.ReadByte()), 16).ToString();
            ///////传感器温度ir_information[14]///////
            vr_information.Add("");
            fs.Seek(jpg_size + 0x100 + 48, SeekOrigin.Begin);
            vr_information[14] += len_2(br.ReadByte()) + len_2(br.ReadByte());
            ///////传感器温度修正值ir_information[15]///////
            vr_information.Add("");
            fs.Seek(jpg_size + 0x100 + 50, SeekOrigin.Begin);
            vr_information[15] += Convert.ToInt32(len_2(br.ReadByte()) + len_2(br.ReadByte()), 16).ToString();
            //vr_information[15] += modify_temp.ToString();
            //////辐射率 ir_information[16]////
            vr_information.Add("");
            fs.Seek(jpg_size + 0x100 + 95 + 9 + 9, SeekOrigin.Begin);
            vr_information[16] += Convert.ToInt32(len_2(br.ReadByte()), 16).ToString();
            /////////距离n ir_information[17]//////
            vr_information.Add("");
            fs.Seek(jpg_size + 0x100 + 95 + 9 + 10, SeekOrigin.Begin);
            vr_information[17] += Convert.ToInt32(len_2(br.ReadByte()) + len_2(br.ReadByte()), 16).ToString();

        }




        private void open_img_Click(object sender, RoutedEventArgs e)//打开图片文件
        {
            is_visible = false;
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "jpg文件|*.jpg|RTF 文件 | *.rtf|所有文件|*.*";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "zip";

            if (openFileDialog.ShowDialog() == true)
            {
                safe_filename = openFileDialog.SafeFileName;
                FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open,FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                fs.Seek(0x18, SeekOrigin.Begin);//从18h字开始
                string jpg_size_str = len_2(br.ReadByte());
                jpg_size_str += len_2(br.ReadByte());
                jpg_size_str += len_2(br.ReadByte());
                jpg_size_str += len_2(br.ReadByte());
                int jpg_size = Convert.ToInt32(jpg_size_str, 16);//获取JPG 文件长度


 


               
                fs.Seek(0x24, SeekOrigin.Begin);//从23h字开始
                string jpg_str = len_2(br.ReadByte());
                if (jpg_str == "01")
                {
                    is_visible = true;
                }
                fs.Seek(jpg_size + 6144, SeekOrigin.Begin);
                    bool is_ir = true;

                    try
                    {
                        br.ReadByte();

                    }
                    catch
                    {
                        is_ir = false;
                    }





                    if (System.IO.Path.GetExtension(openFileDialog.SafeFileName).ToLower() == ".rtf")
                    {


                        sub_report open_report = new sub_report();

                        var rtf = new StreamReader(openFileDialog.FileName).ReadToEnd();
                        open_report.richTB.Document = new RtfFilter().ConvertToDocument(rtf);

                        LayoutDocument newreport = new LayoutDocument();

                        PublicClass.report_name_step++;
                        newreport.Title = safe_filename;

                        open_report.Name = "IR_Report" + safe_filename.Substring(0, safe_filename.Length - 5);
                        open_report.LoadReportInformation += new RoutedPropertyChangedEventHandler<object>(report_LoadReportInformation);
                        //i++;
                        newreport.IsSelected = true;
                        newreport.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(newreport_Closing);


                        //report.richTextBox1.Height = this.ActualHeight - 170;
                        newreport.Content = open_report;
                        newreport.CanFloat = false;
                        mainpanel.Children.Add(newreport);

                        open_report.savereporturl = openFileDialog.FileName;


                    }
               








                    if (is_visible&&is_ir)
                    {


                        filename = openFileDialog.FileName;
                        get_visible();
                       
                        if (yuv_data.Capacity > 0)
                        {
                            yuv_data.Clear();
                        }

                        fs.Seek(jpg_size + 6144, SeekOrigin.Begin);
                        for (int i = 0; i < YUV_WIDTH * YUV_HEIGHT * 2; i++)//获取AD值
                        {
                            yuv_data.Add(br.ReadByte());
                        }

                        br.Close();
                        fs.Close();
                        open_visible();
                        create_vr_information();
                        //filename = null;
                    }


                    fs.Close();




                    if (is_ir && System.IO.Path.GetExtension(openFileDialog.SafeFileName).ToLower() == ".jpg" && !is_visible)
                    {


                    
                        sub_workspace newworkspace = new sub_workspace();
                        PublicClass.report_type = "workreport";
                        PublicClass.ctrl_name++;
                        newworkspace.Name = "work" + PublicClass.ctrl_name.ToString();
                        newworkspace.LoadIrInformation += new RoutedPropertyChangedEventHandler<object>(newworkspace_LoadIrInformation);
                        newworkspace.WorkMouseWheel += new RoutedPropertyChangedEventHandler<object>(newworkspace_WorkMouseWheel);
                        newworkspace.WorkMouseUp += new RoutedPropertyChangedEventHandler<object>(newworkspace_WorkMouseUp);
                        newworkspace.filename = openFileDialog.FileName;
                        newworkspace.cur_file_name = openFileDialog.SafeFileName;
                        LayoutDocument newaaa = new LayoutDocument();
                        newaaa.Content = newworkspace;
                        newaaa.Title = openFileDialog.SafeFileName;
                        newaaa.IsSelected = true;
                        //newaaa.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(newaaa_Closing);
                        newaaa.CanFloat = false;
                        mainpanel.Children.Add(newaaa);

                    }




                    else if (System.IO.Path.GetExtension(openFileDialog.SafeFileName).ToLower() == ".jpg" && !is_visible)
                    {
                
                        sub_img newimage = new sub_img();
                        PublicClass.ctrl_name++;
                        newimage.Name = "image" + PublicClass.ctrl_name.ToString();
                        newimage.filename = openFileDialog.FileName;
                        newimage.LoadImgInformation += new RoutedPropertyChangedEventHandler<object>(newimage_LoadImgInformation);
                        LayoutDocument newaaa = new LayoutDocument();
                        newaaa.Content = newimage;
                        newaaa.Title = "[JPG图像]" + openFileDialog.SafeFileName;
                        newaaa.IsSelected = true;
                        newaaa.CanFloat = false;
                        mainpanel.Children.Add(newaaa);

                    }

               
                
            }
            else
            {
                //MessageBox.Show("asdfsadf");
              //  return "";
            }

         
        }













        void newaaa_Closing(object sender, System.ComponentModel.CancelEventArgs e)// 关闭当前工作区
        {
            Panel.SetZIndex(submainwindow, -1);
            subwindow.Opacity = 0;
            init_toolbar();
            stackpanel_tool.Opacity = 0;
            spot.Opacity = 0;
            list.Opacity = 0;
            saveimg.Source = new BitmapImage(new Uri(@"toolbar_img\save_back.png", UriKind.Relative));
            repot_ready_img.Source = new BitmapImage(new Uri(@"toolbar_img\report_ready_black.png", UriKind.Relative));
            max_temp_img.Source = new BitmapImage(new Uri(@"toolbar_img\temp_max_back.png", UriKind.Relative));
            min_temp_img.Source = new BitmapImage(new Uri(@"toolbar_img\temp_min_back.png", UriKind.Relative));
            cur_temp_img.Source = new BitmapImage(new Uri(@"toolbar_img\temp_cur_back.png", UriKind.Relative));
            zoomin_img.Source = new BitmapImage(new Uri(@"toolbar_img\zoom_in_back.png", UriKind.Relative));
            zoomout_img.Source = new BitmapImage(new Uri(@"toolbar_img\zoom_out_back.png", UriKind.Relative));
            deleteshapes_img.Source = new BitmapImage(new Uri(@"toolbar_img\delete_back.png", UriKind.Relative));

            undo_img.Source = new BitmapImage(new Uri(@"toolbar_img\undo_back.png", UriKind.Relative));
            redo_img.Source = new BitmapImage(new Uri(@"toolbar_img\redo_back.png", UriKind.Relative));


            draw_adjust_img.Source = new BitmapImage(new Uri(@"toolbar_img\adjust_back.png", UriKind.Relative));
            draw_point_img.Source = new BitmapImage(new Uri(@"toolbar_img\point_back.png", UriKind.Relative));
            draw_line_img.Source = new BitmapImage(new Uri(@"toolbar_img\line_back.png", UriKind.Relative));
            draw_brokenline_img.Source = new BitmapImage(new Uri(@"toolbar_img\cur_back.png", UriKind.Relative));
            draw_circular_img.Source = new BitmapImage(new Uri(@"toolbar_img\ellipse_back.png", UriKind.Relative));
            draw_rectangle_img.Source = new BitmapImage(new Uri(@"toolbar_img\rectangle_back.png", UriKind.Relative));
            draw_polygon_img.Source = new BitmapImage(new Uri(@"toolbar_img\pol_back.png", UriKind.Relative));
            draw_Isothermal_img.Source = new BitmapImage(new Uri(@"toolbar_img\isothermal_back.png", UriKind.Relative));
            draw_palette_img.Source = new BitmapImage(new Uri(@"toolbar_img\palette_back.png", UriKind.Relative));
            draw_bc_adjust_img.Source = new BitmapImage(new Uri(@"toolbar_img\bc_adjust_back.png", UriKind.Relative));
            draw_dismap_img.Source = new BitmapImage(new Uri(@"toolbar_img\fenbu_back.png", UriKind.Relative));
            draw_scale_img.Source = new BitmapImage(new Uri(@"toolbar_img\bili_back.png", UriKind.Relative));
            draw_3d_img.Source = new BitmapImage(new Uri(@"toolbar_img\3d_back.png", UriKind.Relative));



        }

        void newworkspace_WorkMouseWheel(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            zoom_slider.Value = workspace.zoom_coe;
            zoom_textblock.Text = (int)(workspace.zoom_coe * 100)+"%";
        }

        void newimage_LoadImgInformation(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Panel.SetZIndex(submainwindow, -1);
            subwindow.Opacity = 0;
            
            //submainwindow.Opacity = 0;
            PublicClass.cur_ctrl_type = "image";
            init_toolbar();
            saveimg.Source = new BitmapImage(new Uri(@"toolbar_img\save_back.png", UriKind.Relative));
            repot_ready_img.Source = new BitmapImage(new Uri(@"toolbar_img\report_ready_black.png", UriKind.Relative));
            max_temp_img.Source = new BitmapImage(new Uri(@"toolbar_img\temp_max_back.png", UriKind.Relative));
            min_temp_img.Source = new BitmapImage(new Uri(@"toolbar_img\temp_min_back.png", UriKind.Relative));
            cur_temp_img.Source = new BitmapImage(new Uri(@"toolbar_img\temp_cur_back.png", UriKind.Relative));
            zoomin_img.Source = new BitmapImage(new Uri(@"toolbar_img\zoom_in_back.png", UriKind.Relative));
            zoomout_img.Source = new BitmapImage(new Uri(@"toolbar_img\zoom_out_back.png", UriKind.Relative));
            deleteshapes_img.Source = new BitmapImage(new Uri(@"toolbar_img\delete_back.png", UriKind.Relative));

            undo_img.Source = new BitmapImage(new Uri(@"toolbar_img\undo_back.png", UriKind.Relative));
            redo_img.Source = new BitmapImage(new Uri(@"toolbar_img\redo_back.png", UriKind.Relative));


            draw_adjust_img.Source = new BitmapImage(new Uri(@"toolbar_img\adjust_back.png", UriKind.Relative));
            draw_point_img.Source = new BitmapImage(new Uri(@"toolbar_img\point_back.png", UriKind.Relative));
            draw_line_img.Source = new BitmapImage(new Uri(@"toolbar_img\line_back.png", UriKind.Relative));
            draw_brokenline_img.Source = new BitmapImage(new Uri(@"toolbar_img\cur_back.png", UriKind.Relative));
            draw_circular_img.Source = new BitmapImage(new Uri(@"toolbar_img\ellipse_back.png", UriKind.Relative));
            draw_rectangle_img.Source = new BitmapImage(new Uri(@"toolbar_img\rectangle_back.png", UriKind.Relative));
            draw_polygon_img.Source = new BitmapImage(new Uri(@"toolbar_img\pol_back.png", UriKind.Relative));

            draw_Isothermal_img.Source = new BitmapImage(new Uri(@"toolbar_img\isothermal_back.png", UriKind.Relative));
            draw_palette_img.Source = new BitmapImage(new Uri(@"toolbar_img\palette_back.png", UriKind.Relative));
            draw_bc_adjust_img.Source = new BitmapImage(new Uri(@"toolbar_img\bc_adjust_back.png", UriKind.Relative));
            draw_dismap_img.Source = new BitmapImage(new Uri(@"toolbar_img\fenbu_back.png", UriKind.Relative));
            draw_scale_img.Source = new BitmapImage(new Uri(@"toolbar_img\bili_back.png", UriKind.Relative));
            draw_3d_img.Source = new BitmapImage(new Uri(@"toolbar_img\3d_back.png", UriKind.Relative));


            stackpanel_tool.Opacity = 0;
            spot.Opacity = 0;
            list.Opacity = 0;



        }

        void newworkspace_WorkMouseUp(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            if (workspace.histroy_shapes_index>=0)
            {
                undo_img.Source = new BitmapImage(new Uri(@"toolbar_img\undo_active.png", UriKind.Relative));
                
                undo.IsEnabled = true;
                toolundo.IsEnabled = true;
            }
            else 
            {
                undo_img.Source = new BitmapImage(new Uri(@"toolbar_img\undo_back.png", UriKind.Relative));
                
            }
            if (workspace.histroy_shapes_index == workspace.histroy_shapes_count.Count - 1)
            {
                redo_img.Source = new BitmapImage(new Uri(@"toolbar_img\redo_back.png", UriKind.Relative));
                redo.IsEnabled = false;
                toolredo.IsEnabled = false;
            }
            else
            {
                redo_img.Source = new BitmapImage(new Uri(@"toolbar_img\redo_active.png", UriKind.Relative));
                redo.IsEnabled = true;
                toolredo.IsEnabled = true;
            }
            if (workspace.public_shapes_index > -1)
            {
                PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[workspace.public_shapes_index];
                if (newshapes.shapes_type != "work" && newshapes.shapes_type != "spot" && newshapes.shapes_type != "area" && newshapes.shapes_type != "max_temp_temp" && newshapes.shapes_type != "min_temp_temp")
                {
                    deleteshapes.IsEnabled = true;
                    deleteshapes_img.Source = new BitmapImage(new Uri(@"toolbar_img\delete_active.png", UriKind.Relative));

                }
            }
            else
            {
                deleteshapes.IsEnabled = false;
                deleteshapes_img.Source = new BitmapImage(new Uri(@"toolbar_img\delete_back.png", UriKind.Relative));
            }


            if ((bool)adjust.IsChecked)
            {
                PublicClass.is_draw_type = "adjust";

            }
            else if ((bool)point.IsChecked)
            {
                PublicClass.is_draw_type = "point";

            }
            else if ((bool)line.IsChecked)
            {
                PublicClass.is_draw_type = "line";

            }
            else if ((bool)polyline.IsChecked)
            {
                PublicClass.is_draw_type = "polyline";

            }
            else if ((bool)ellipse.IsChecked)
            {
                PublicClass.is_draw_type = "ellipse";
            }
            else if ((bool)rectangle.IsChecked)
            {
                PublicClass.is_draw_type = "rectangle";
            }
            else if ((bool)polygon.IsChecked)
            {
                PublicClass.is_draw_type = "polygon";
            }

        }

        void newworkspace_LoadIrInformation(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Panel.SetZIndex(submainwindow, -1);
            PublicClass.cur_ctrl_type = "work";
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);

           
            stackpanel_tool.Opacity = 1;
            spot.Opacity = 1;
            list.Opacity = 1;


            create_ir_information();


            tool1.IsSelected = true;

            save_img.IsEnabled = true;
            save_as_img.IsEnabled = true;
            undo.IsEnabled = false;
            redo.IsEnabled = false;
            toolundo.IsEnabled = false;
            toolredo.IsEnabled = false;
            palette.IsEnabled = true;
            bc_adjust.IsEnabled = true;
            tool_max_zoom.IsEnabled = true;
            tool_min_zoom.IsEnabled = true;
            tool_yuanshi_zoom.IsEnabled = true;
            toolpoint.IsEnabled = true;
            toolline.IsEnabled = true;
            toolpolyline.IsEnabled = true;
            toolellipse.IsEnabled = true;
            toolrectangle.IsEnabled = true;
            toolpolygon.IsEnabled = true;
            tooldelete.IsEnabled = true;
            adjust.IsEnabled = true;
            point.IsEnabled = true;
            line.IsEnabled = true;
            polyline.IsEnabled = true;
            ellipse.IsEnabled = true;
            rectangle.IsEnabled = true;
            polygon.IsEnabled = true;
            deleteshapes.IsEnabled = true;
            save.IsEnabled = true;
            tool_max_temp.IsEnabled = true;
            tool_min_temp.IsEnabled = true;
            tool_cur_temp.IsEnabled = true;
            max_temp.IsEnabled = true;
            min_temp.IsEnabled = true;
            cur_temp.IsEnabled = true;
            dismap.IsEnabled = true;
            scale.IsEnabled = true;
            zoomin.IsEnabled = true;
            zoomout.IsEnabled = true;
            repot_ready_btn.IsEnabled = true;
           // draw_cursor.IsEnabled = true;
           // toolyoubiao.IsEnabled = true;

            newreport.IsEnabled = true; ;
            too_per_report.IsEnabled = true;

            tool_Isothermal.IsEnabled = true;
            tool_palette.IsEnabled = true;
            tool_bc_adjust.IsEnabled = true;
            tool_dismap.IsEnabled = true;
            tool_scale.IsEnabled = true;
            temp_mark.IsEnabled = true;
            pic_3d.IsEnabled = true;
            radiance_table.IsEnabled = true;
            print_report.IsEnabled = false;
            tool_pic_3d.IsEnabled = true;

            zoom_textblock.Text = (int)(workspace.zoom_coe * 100) + "%";
            try
            {
                if (workspace.histroy_shapes_index < workspace.histroy_shapes_count.Count - 1)
                {
                    redo.IsEnabled = true;
                    toolredo.IsEnabled = true;
                    redo_img.Source = new BitmapImage(new Uri(@"toolbar_img\redo_active.png", UriKind.Relative));
                }
                if (workspace.histroy_shapes_index > -1)
                {
                    undo.IsEnabled = true;
                    toolundo.IsEnabled = true;
                    undo_img.Source = new BitmapImage(new Uri(@"toolbar_img\undo_active.png", UriKind.Relative));
                }
            

            }
            catch { }

            if (PublicClass.is_draw_type == "adjust")
            {
                adjust.IsChecked = true;
                draw_adjust_img.Source = new BitmapImage(new Uri(@"toolbar_img\adjust_active.png", UriKind.Relative));
            }
            else if (PublicClass.is_draw_type == "point")
            {
                point.IsChecked = true;
                draw_point_img.Source = new BitmapImage(new Uri(@"toolbar_img\point_active.png", UriKind.Relative));
            }
            else if (PublicClass.is_draw_type == "line")
            {
                line.IsChecked = true;
                draw_line_img.Source = new BitmapImage(new Uri(@"toolbar_img\line_active.png", UriKind.Relative));
            }
            else if (PublicClass.is_draw_type == "polyline")
            {
                polyline.IsChecked = true;
                draw_brokenline_img.Source = new BitmapImage(new Uri(@"toolbar_img\cur_active.png", UriKind.Relative));
            }
            else if (PublicClass.is_draw_type == "ellipse")
            {
                ellipse.IsChecked = true;
                draw_circular_img.Source = new BitmapImage(new Uri(@"toolbar_img\ellipse_active.png", UriKind.Relative));
            }
            else if (PublicClass.is_draw_type == "rectangle")
            {
                rectangle.IsChecked = true;
                draw_rectangle_img.Source = new BitmapImage(new Uri(@"toolbar_img\rectangle_active.png", UriKind.Relative));
            }
            else if (PublicClass.is_draw_type == "polygon")
            {
                polygon.IsChecked = true;
                draw_polygon_img.Source = new BitmapImage(new Uri(@"toolbar_img\pol_active.png", UriKind.Relative));
            }
            else if (PublicClass.is_cur_temp == "true")
            {
             cur_temp.IsChecked = true;
                tool_cur_temp.IsChecked = true;
          
            }
           
            for (int i = 0; i < PublicClass.shapes_count.Count; i++)
            {
                PublicClass.shapes_property test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                if (test.shapes_type == "min_temp")
                {
                    tool_min_temp.IsChecked = true;
                    min_temp.IsChecked = true;
                }
                // if (test.shapes_type == "min_temp" && test.workspace_name != PublicClass.cur_ctrl_name)
                //{
                //    tool_min_temp.IsChecked = false;
                //    min_temp.IsChecked = false;
                   
                //}

                 if (test.shapes_type == "max_temp")
                {
                    tool_max_temp.IsChecked = true;
                    max_temp.IsChecked = true;
                }
                 //if ((bool)max_temp.IsChecked)
                 //{
                 //    PublicClass.shapes_property newmin = new PublicClass.shapes_property();
                 //    newmin.shapes_type = "max_temp_temp";
                     
                 //}

                // if (test.shapes_type == "max_temp" && test.workspace_name != PublicClass.cur_ctrl_name)
                //{
                //    tool_max_temp.IsChecked = false;
                //    max_temp.IsChecked = false;
                //}
              
            }
            


            saveimg.Source = new BitmapImage(new Uri(@"toolbar_img\save_active.png", UriKind.Relative));
            repot_ready_img.Source = new BitmapImage(new Uri(@"toolbar_img\report_ready_active.png", UriKind.Relative));
            max_temp_img.Source = new BitmapImage(new Uri(@"toolbar_img\temp_max_active.png", UriKind.Relative));
            min_temp_img.Source = new BitmapImage(new Uri(@"toolbar_img\temp_min_active.png", UriKind.Relative));
            cur_temp_img.Source = new BitmapImage(new Uri(@"toolbar_img\temp_cur_active.png", UriKind.Relative));
            zoomin_img.Source = new BitmapImage(new Uri(@"toolbar_img\zoom_in_active.png", UriKind.Relative));
            zoomout_img.Source = new BitmapImage(new Uri(@"toolbar_img\zoom_out_active.png", UriKind.Relative));
            draw_Isothermal_img.Source = new BitmapImage(new Uri(@"toolbar_img\isothermal_active.png", UriKind.Relative));
            draw_palette_img.Source = new BitmapImage(new Uri(@"toolbar_img\palette_active.png", UriKind.Relative));
            draw_bc_adjust_img.Source = new BitmapImage(new Uri(@"toolbar_img\bc_adjust_active.png", UriKind.Relative));

            draw_dismap_img.Source = new BitmapImage(new Uri(@"toolbar_img\fenbu_active.png", UriKind.Relative));
            draw_scale_img.Source = new BitmapImage(new Uri(@"toolbar_img\bili_active.png", UriKind.Relative));
            draw_3d_img.Source = new BitmapImage(new Uri(@"toolbar_img\3d_active.png", UriKind.Relative));

            //sub_newreport_ready checkreportready = repot_ready.FindName(workspace.Name) as sub_newreport_ready;
            //if (checkreportready == null)
            //{
            //    sub_newreport_ready newreportready = new sub_newreport_ready();
            //    newreportready.report_img.Source = workspace.irimg.Source;
            //    newreportready.report_textblock.Text = workspace.cur_file_name;
            //    newreportready.PreviewMouseDown += new MouseButtonEventHandler(newreportready_PreviewMouseDown);
            //    newreportready.AllowDrop = true;
            //    repot_ready.Items.Add(newreportready);
            //    repot_ready.RegisterName(workspace.Name, newreportready);

            //}
            pic_temp_para_infermation();



        }

        private void pic_temp_para_infermation()
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
          
           
            DataTable dt3 = new DataTable();
            
            DataColumn col5 = new DataColumn("名称", typeof(string));
            
            DataColumn col6 = new DataColumn("值", typeof(string));
            dt3.Columns.Add(col5);
            dt3.Columns.Add(col6);
            DataRow row7 = dt3.NewRow();
            row7["名称"] = "环境温度";
            row7["值"] = (float.Parse(workspace.ir_information[10].ToString()) / 10).ToString() + " ℃";
            DataRow row8 = dt3.NewRow();
            row8["名称"] = "相对湿度";
            row8["值"] = workspace.ir_information[11] + " %";

            DataRow row11 = dt3.NewRow();
            row11["名称"] = "辐射率";
            row11["值"] = float.Parse(workspace.ir_information[16].ToString()) / 100;
            DataRow row12 = dt3.NewRow();
            row12["名称"] = "距离";
            row12["值"] = workspace.ir_information[17] + " m";


            DataRow row18 = dt3.NewRow();
            row18["名称"] = "温度修正";
            row18["值"] = workspace.modify_temp /10.0+ " ℃";
            dt3.Rows.Add(row7);
            dt3.Rows.Add(row8);

            // dt.Rows.Add(row10);
            dt3.Rows.Add(row11);
            dt3.Rows.Add(row12);

            dt3.Rows.Add(row18);
            pic_temp_para.ColumnWidth = (pic_information.ActualWidth - 2) / 2;
            pic_temp_para.ItemsSource = dt3.DefaultView;
            pic_temp_para.Columns[0].IsReadOnly = true;
            pic_temp_para.CanUserAddRows = false;
            pic_temp_para.IsReadOnly = false;
        }
        void newreportready_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DataObject data = new DataObject(typeof(sub_newreport_ready), (sub_newreport_ready)sender);
            DragDrop.DoDragDrop((sub_newreport_ready)sender, data, DragDropEffects.Copy);
        }


        private void readRGB(string filename, string palettename)//读取调色板文件的RGB值
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            fs.Seek(66, SeekOrigin.Begin);//从41h字开始
            for (int i = 0; i < 256; i++)
            {
                byte color_B = br.ReadByte();//蓝色
                byte color_G = br.ReadByte();//绿色
                byte color_R = br.ReadByte();//红色
                if (palettename == "iron")
                {
                    PublicClass.Iron_palette.Add(len_2(color_R) + len_2(color_G) + len_2(color_B));
                }
                else
                    if (palettename == "medical")
                    {
                        PublicClass.medical.Add(len_2(color_R) + len_2(color_G) + len_2(color_B));
                    }
                    else
                        if (palettename == "grey")
                        {
                            PublicClass.grey.Add(len_2(color_R) + len_2(color_G) + len_2(color_B));
                        }
                        else
                            if (palettename == "new_rainbow")
                            {
                                PublicClass.new_rainbow.Add(len_2(color_R) + len_2(color_G) + len_2(color_B));
                            }
                            else
                                if (palettename == "rainbow")
                                {
                                    PublicClass.rainbow.Add(len_2(color_R) + len_2(color_G) + len_2(color_B));
                                }
                                else
                                    if (palettename == "red_brown")
                                    {
                                        PublicClass.red_brown.Add(len_2(color_R) + len_2(color_G) + len_2(color_B));
                                    }
                                    else
                                        if (palettename == "isothermal")
                                        {
                                            PublicClass.isothermal.Add(len_2(color_R) + len_2(color_G) + len_2(color_B));
                                        }

                fs.Seek(97, SeekOrigin.Current);
            }
            //PublicClass.Iron_palette.Reverse();
            br.Close();
            fs.Close();
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

        private void initpalette()
        {
            string filename;

            if (PublicClass.Iron_palette.Count != 256)
            {
                filename = AppDomain.CurrentDomain.BaseDirectory + "palette/0_Iron.pal";
                readRGB(filename, "iron");
                PublicClass.Iron_palette.Reverse();
            }

            if (PublicClass.medical.Count != 256)
            {
                filename = AppDomain.CurrentDomain.BaseDirectory + "palette/1_Feather.pal";
                readRGB(filename, "medical");
                PublicClass.medical.Reverse();
            }
            if (PublicClass.grey.Count != 256)
            {
                filename = AppDomain.CurrentDomain.BaseDirectory + "palette/2_Gray.pal";
                readRGB(filename, "grey");
                PublicClass.grey.Reverse();
            }
            if (PublicClass.new_rainbow.Count != 256)
            {
                filename = AppDomain.CurrentDomain.BaseDirectory + "palette/3_NewRainbow.pal";
                readRGB(filename, "new_rainbow");
                PublicClass.new_rainbow.Reverse();
            }
            if (PublicClass.rainbow.Count != 256)
            {
                filename = AppDomain.CurrentDomain.BaseDirectory + "palette/4_Rainbow.pal";
                readRGB(filename, "rainbow");
                PublicClass.rainbow.Reverse();
            }
            if (PublicClass.red_brown.Count != 256)
            {
                filename = AppDomain.CurrentDomain.BaseDirectory + "palette/5_RedBrown.pal";
                readRGB(filename, "red_brown");
                PublicClass.red_brown.Reverse();
            }
            if (PublicClass.isothermal.Count != 256)
            {
                filename = AppDomain.CurrentDomain.BaseDirectory + "palette/6_Isothermal.pal";
                readRGB(filename, "isothermal");
                PublicClass.isothermal.Reverse();
            }
        }

        private void bc_adjust_Click(object sender, RoutedEventArgs e)
        {
            subwindow_content.Children.Clear();
            sub_bc_adjust bc_adjust = new sub_bc_adjust();
            subwindow.Width = 450;
            subwindow.Height = 340;
            subwindow_content.Children.Add(bc_adjust);
            subwindow.Opacity = 0.9;
            Panel.SetZIndex(submainwindow, 3000);
            PublicClass.open_newisothermal = false;

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            submainwindow.Width = maingrid.ActualWidth;
            submainwindow.Height = maingrid.ActualHeight;
        }

        Point movepoint=new Point();
        bool can_move = false;
        private void subwindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            movepoint = e.GetPosition(null);
            if (e.GetPosition(subwindow).X < subwindow.Width && e.GetPosition(subwindow).Y < 20)
            {
                can_move = true;
            }
        }

        private void subwindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed &&can_move)
            {
                subwindow.Margin = new Thickness(subwindow.Margin.Left + e.GetPosition(null).X - movepoint.X, subwindow.Margin.Top + e.GetPosition(null).Y - movepoint.Y, 0, 0);
                movepoint = e.GetPosition(null);
            }
        }

        private void subwindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            can_move = false;
        }



        private void zoom_Click(object sender, RoutedEventArgs e)
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            Button newbutton = (Button)sender;
            if (newbutton.Name == "zoomin")
            {

                workspace.zoom_coe -= 0.2f;
            }
            else
            {
                workspace.zoom_coe += 0.2f;
            }
            workspace.zoom();
            zoom_slider.Value = workspace.zoom_coe;
            zoom_textblock.Text = (int)(workspace.zoom_coe * 100) + "%";
            

        }

        private void draw_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton newtoggle = (ToggleButton)sender;
            adjust.IsChecked = false;
            point.IsChecked = false;
            line.IsChecked = false;
            ellipse.IsChecked = false;
            rectangle.IsChecked = false;
            polyline.IsChecked = false;
            polygon.IsChecked = false;
            toolpoint.IsChecked = false;
            toolline.IsChecked = false;
            toolpolyline.IsChecked = false;
            toolellipse.IsChecked = false;
            toolpolygon.IsChecked = false;
            toolrectangle.IsChecked = false;
            draw_adjust_img.Source = new BitmapImage(new Uri(@"toolbar_img\adjust_back.png", UriKind.Relative));
            draw_point_img.Source = new BitmapImage(new Uri(@"toolbar_img\point_back.png", UriKind.Relative));
            draw_line_img.Source = new BitmapImage(new Uri(@"toolbar_img\line_back.png", UriKind.Relative));
            draw_brokenline_img.Source = new BitmapImage(new Uri(@"toolbar_img\cur_back.png", UriKind.Relative));
            draw_circular_img.Source = new BitmapImage(new Uri(@"toolbar_img\ellipse_back.png", UriKind.Relative));
            draw_rectangle_img.Source = new BitmapImage(new Uri(@"toolbar_img\rectangle_back.png", UriKind.Relative));
            draw_polygon_img.Source = new BitmapImage(new Uri(@"toolbar_img\pol_back.png", UriKind.Relative));
            if (PublicClass.is_draw_type == "point" && newtoggle.Name == "point")
            {
                point.IsChecked = false;
                PublicClass.is_draw_type = "adjust";
                draw_adjust_img.Source = new BitmapImage(new Uri(@"toolbar_img\adjust_active.png", UriKind.Relative));
            }
            else if (newtoggle.Name == "point")
            {
                PublicClass.is_draw_type = "point";
                point.IsChecked = true;
                toolpoint.IsChecked = true;
                draw_point_img.Source = new BitmapImage(new Uri(@"toolbar_img\point_active.png", UriKind.Relative));
            }
            else if (PublicClass.is_draw_type == "line" && newtoggle.Name == "line")
            {
                line.IsChecked = false;
                PublicClass.is_draw_type = "adjust";
                draw_adjust_img.Source = new BitmapImage(new Uri(@"toolbar_img\adjust_active.png", UriKind.Relative));
             
            }
            else if (newtoggle.Name == "line")
            {
                
                PublicClass.is_draw_type = "line";
                line.IsChecked = true;
                toolline.IsChecked = true;
                draw_line_img.Source = new BitmapImage(new Uri(@"toolbar_img\line_active.png", UriKind.Relative));
            }
            else if (PublicClass.is_draw_type == "polyline" && newtoggle.Name == "polyline")
            {
                polyline.IsChecked = false;
                PublicClass.is_draw_type = "adjust";
                draw_adjust_img.Source = new BitmapImage(new Uri(@"toolbar_img\adjust_active.png", UriKind.Relative));

            }
            else if (newtoggle.Name == "polyline")
            {
                PublicClass.is_draw_type = "polyline";
                polyline.IsChecked = true;
                toolpolyline.IsChecked = true;
                draw_brokenline_img.Source = new BitmapImage(new Uri(@"toolbar_img\cur_active.png", UriKind.Relative));

            }
            else if (PublicClass.is_draw_type == "ellipse" && newtoggle.Name == "ellipse")
            {
                ellipse.IsChecked = false;
                PublicClass.is_draw_type = "adjust";
                draw_adjust_img.Source = new BitmapImage(new Uri(@"toolbar_img\adjust_active.png", UriKind.Relative));

            }
            else if (newtoggle.Name == "ellipse")
            {
                PublicClass.is_draw_type = "ellipse";
                ellipse.IsChecked = true;
                toolellipse.IsChecked = true;
                draw_circular_img.Source = new BitmapImage(new Uri(@"toolbar_img\ellipse_active.png", UriKind.Relative));

            }
            else if (PublicClass.is_draw_type == "rectangle" && newtoggle.Name == "rectangle")
            {
                rectangle.IsChecked = false;
                PublicClass.is_draw_type = "adjust";
                draw_adjust_img.Source = new BitmapImage(new Uri(@"toolbar_img\adjust_active.png", UriKind.Relative));
            }
            else if (newtoggle.Name == "rectangle")
            {
                PublicClass.is_draw_type = "rectangle";
                rectangle.IsChecked = true;
                toolrectangle.IsChecked = true;
                draw_rectangle_img.Source = new BitmapImage(new Uri(@"toolbar_img\rectangle_active.png", UriKind.Relative));

            }
            else if (PublicClass.is_draw_type == "polygon" && newtoggle.Name == "polygon")
            {
                polygon.IsChecked = false;
                PublicClass.is_draw_type = "adjust";
                draw_adjust_img.Source = new BitmapImage(new Uri(@"toolbar_img\adjust_active.png", UriKind.Relative));

            }
            else if (newtoggle.Name == "polygon")
            {
                PublicClass.is_draw_type = "polygon";
                polygon.IsChecked = true;
                toolpolygon.IsChecked = true;
                draw_polygon_img.Source = new BitmapImage(new Uri(@"toolbar_img\pol_active.png", UriKind.Relative));

            }

            else if (newtoggle.Name == "adjust")
            {
                adjust.IsChecked = true;
                PublicClass.is_draw_type = "adjust";
                draw_adjust_img.Source = new BitmapImage(new Uri(@"toolbar_img\adjust_active.png", UriKind.Relative));
            }
           
        }

        

        private void scale_Click(object sender, RoutedEventArgs e) 
        {
            subwindow_content.Children.Clear();
            sub_scale newscale = new sub_scale();
            newscale.ImageMouseUp += new RoutedPropertyChangedEventHandler<object>(newscale_ImageMouseUp);
            newscale.ReportMouseDown += new RoutedPropertyChangedEventHandler<object>(newscale_ReportMouseDown);
            newscale.ReportMouseUp += new RoutedPropertyChangedEventHandler<object>(newscale_ReportMouseUp);
            newscale.leftpanel.Width = 0;
            subwindow.Width = 950;
            subwindow.Height = 400;
            newscale.Name = "newscale";
            PublicClass.report_type = "scalereport";
            subwindow_content.Children.Add(newscale);
            subwindow.Opacity = 0.9;
           
            Panel.SetZIndex(submainwindow, 3000);
            PublicClass.open_newisothermal = false;

            //空动画//
            //sub_scale anicale = MainWindow.FindChild<sub_scale>(Application.Current.MainWindow, "newscale");
            DoubleAnimation leftpanel_animation = new DoubleAnimation();
            DoubleAnimation subwindow_animation = new DoubleAnimation();
            ThicknessAnimation newthickness = new ThicknessAnimation();

            leftpanel_animation.FillBehavior = FillBehavior.Stop;
            subwindow_animation.FillBehavior = FillBehavior.Stop;
            newthickness.FillBehavior = FillBehavior.Stop;

            leftpanel_animation.Duration = TimeSpan.FromSeconds(0.1);
            subwindow_animation.Duration = TimeSpan.FromSeconds(0.1);
            newthickness.Duration = TimeSpan.FromSeconds(0.1);

            subwindow.BeginAnimation(StackPanel.MarginProperty, newthickness);
            newscale.leftpanel.BeginAnimation(StackPanel.WidthProperty, leftpanel_animation);
            subwindow.BeginAnimation(StackPanel.WidthProperty, subwindow_animation);

            //空动画结束//


        }

        void newscale_ReportMouseUp(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DoubleAnimation opacity_ani = new DoubleAnimation();
            opacity_ani.To = 0;
            opacity_ani.Duration = TimeSpan.FromSeconds(0.8);
            opacity_ani.FillBehavior = FillBehavior.Stop;

            DoubleAnimation width_ani = new DoubleAnimation();
            width_ani.To = 150;
            width_ani.Duration = TimeSpan.FromSeconds(0.8);
            width_ani.FillBehavior = FillBehavior.Stop;

            DoubleAnimation height_ani = new DoubleAnimation();
            height_ani.To = 80;
            height_ani.Duration = TimeSpan.FromSeconds(0.8);
            height_ani.FillBehavior = FillBehavior.Stop;

            ThicknessAnimation thick_ani = new ThicknessAnimation();
            thick_ani.To = new Thickness(this.ActualWidth - 280, this.ActualHeight - 100, 0, 0);
            thick_ani.Duration = TimeSpan.FromSeconds(0.8);
            thick_ani.FillBehavior = FillBehavior.Stop;

            img_animi.BeginAnimation(Image.OpacityProperty, opacity_ani);
            img_animi.BeginAnimation(Image.WidthProperty, width_ani);
            img_animi.BeginAnimation(Image.HeightProperty, height_ani);
            img_animi.BeginAnimation(Image.MarginProperty, thick_ani);
            img_animi.Opacity = 0;
            img_animi.Width = 0;
            img_animi.Height = 0;
            sub_scale anicale = MainWindow.FindChild<sub_scale>(Application.Current.MainWindow, "newscale");
            RenderTargetBitmap repot_image;
            repot_image = new RenderTargetBitmap((int)anicale.report_panel.ActualWidth, (int)anicale.report_panel.ActualHeight + 250, 96, 96, PixelFormats.Default);
            repot_image.Render(anicale.report_panel);
   
        }


        void newscale_ReportMouseDown(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            sub_scale anicale = MainWindow.FindChild<sub_scale>(Application.Current.MainWindow, "newscale");
            repot_ready.Items.Add(anicale.report);
            repot_scrol.ScrollToBottom();
            RenderTargetBitmap repot_image;
            repot_image = new RenderTargetBitmap((int)anicale.report_panel.ActualWidth, (int)anicale.report_panel.ActualHeight + 250, 96, 96, PixelFormats.Default);
            repot_image.Render(anicale.report_panel);
            img_animi.Source = repot_image;
            img_animi.Width = repot_image.Width;
            img_animi.Height = repot_image.Height;
            img_animi.Opacity = 1;
      

            if (subwindow.Width == 950)
            {
                img_animi.Margin = new Thickness(subwindow.Margin.Left, subwindow.Margin.Top + 31, 0, 0);
            }
            else
            {
                img_animi.Margin = new Thickness(subwindow.Margin.Left + 200, subwindow.Margin.Top + 31, 0, 0);
            }




            DoubleAnimation opacity_ani = new DoubleAnimation();
            opacity_ani.To = img_animi.Opacity;
            opacity_ani.Duration = TimeSpan.FromSeconds(0.1);
            opacity_ani.FillBehavior = FillBehavior.Stop;

            DoubleAnimation width_ani = new DoubleAnimation();
            width_ani.To = img_animi.Width;
            width_ani.Duration = TimeSpan.FromSeconds(0.1);
            width_ani.FillBehavior = FillBehavior.Stop;

            DoubleAnimation height_ani = new DoubleAnimation();
            height_ani.To = img_animi.Height;
            height_ani.Duration = TimeSpan.FromSeconds(0.1);
            height_ani.FillBehavior = FillBehavior.Stop;

            ThicknessAnimation thick_ani = new ThicknessAnimation();
            thick_ani.To = new Thickness(img_animi.Margin.Left, img_animi.Margin.Top, 0, 0);
            thick_ani.Duration = TimeSpan.FromSeconds(0.1);
            thick_ani.FillBehavior = FillBehavior.Stop;

            img_animi.BeginAnimation(Image.OpacityProperty, opacity_ani);
            img_animi.BeginAnimation(Image.WidthProperty, width_ani);
            img_animi.BeginAnimation(Image.HeightProperty, height_ani);
            img_animi.BeginAnimation(Image.MarginProperty, thick_ani);
        }



      

        void newscale_ImageMouseUp(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            sub_scale newscale = MainWindow.FindChild<sub_scale>(Application.Current.MainWindow, "newscale");
            DoubleAnimation leftpanel_animation = new DoubleAnimation();
            DoubleAnimation subwindow_animation = new DoubleAnimation();
            ThicknessAnimation newthickness = new ThicknessAnimation();

            leftpanel_animation.FillBehavior = FillBehavior.Stop;
            subwindow_animation.FillBehavior = FillBehavior.Stop;
            newthickness.FillBehavior = FillBehavior.Stop;
            leftpanel_animation.Duration = TimeSpan.FromSeconds(0.1);
            subwindow_animation.Duration = TimeSpan.FromSeconds(0.1);
            newthickness.Duration = TimeSpan.FromSeconds(0.1);
            if (newscale.leftpanel.Width == 0)
            {
                leftpanel_animation.To = 200;
                subwindow_animation.To = subwindow.Width + 200;
                newthickness.To = new Thickness(subwindow.Margin.Left - 200, subwindow.Margin.Top, subwindow.Margin.Right, subwindow.Margin.Bottom);
                newscale.left_arr.Source = new BitmapImage(new Uri(@"images\arr_right.png", UriKind.Relative)); 
            }
            else
            {
                leftpanel_animation.To = 0;
                subwindow_animation.To = subwindow.Width - 200;
                newthickness.To = new Thickness(subwindow.Margin.Left + 200, subwindow.Margin.Top, subwindow.Margin.Right, subwindow.Margin.Bottom);
                newscale.left_arr.Source = new BitmapImage(new Uri(@"images\arr_left.png", UriKind.Relative)); 
                
            }
            subwindow.BeginAnimation(StackPanel.MarginProperty, newthickness);
            newscale.leftpanel.BeginAnimation(StackPanel.WidthProperty, leftpanel_animation);
            subwindow.BeginAnimation(StackPanel.WidthProperty, subwindow_animation);
            



            if (newscale.leftpanel.Width == 0)
            {
                subwindow.Margin = new Thickness(subwindow.Margin.Left - 200, subwindow.Margin.Top, subwindow.Margin.Right, subwindow.Margin.Bottom);
                newscale.leftpanel.Width = 200;
                subwindow.Width = subwindow.Width + 200;

            }
            else
            {
                subwindow.Margin = new Thickness(subwindow.Margin.Left + 200, subwindow.Margin.Top, subwindow.Margin.Right, subwindow.Margin.Bottom);
                newscale.leftpanel.Width = 0;
                subwindow.Width = subwindow.Width - 200;

            }
            


        }


    

        private void open_report_Click(object sender, RoutedEventArgs e) //新建报告
        {



            sub_report report = new sub_report();
            LayoutDocument newreport = new LayoutDocument();
            //report.richTextBox1.Height = this.ActualHeight - 170;
            newreport.Content = report;
            PublicClass.report_name_step++;
            newreport.Title = "IR_Report" + PublicClass.report_name_step;

            report.Name = "IR_Report" + PublicClass.report_name_step;
            report.LoadReportInformation += new RoutedPropertyChangedEventHandler<object>(report_LoadReportInformation);
            //i++;
            newreport.IsSelected = true;
            newreport.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(newreport_Closing);
            mainpanel.Children.Add(newreport);





            //sub_report newreport = new sub_report();
            ////newreport.base_canvas.Height = this.ActualHeight - 170;
            //LayoutDocument newdocument = new LayoutDocument();
            //newdocument.Content = newreport;
            //PublicClass.report_name_step++;
            //newdocument.Title = "IR_Report" + PublicClass.report_name_step;
            //newdocument.IsSelected = true;
            //mainpanel.Children.Add(newdocument);

        }

        void newreport_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            saveimg.Source = new BitmapImage(new Uri(@"toolbar_img\save_back.png", UriKind.Relative));
            save.IsEnabled = false;
            save_as_img.IsEnabled = false;
            save_img.IsEnabled = false;
            print_report.IsEnabled = false;


        }

        void report_LoadReportInformation(object sender, RoutedPropertyChangedEventArgs<object> e)//初始化报告
        {
            Panel.SetZIndex(submainwindow, -1);
            PublicClass.cur_ctrl_type = "report";
            PublicClass.cur_ctrl_name = ((sub_report)sender).Name;
            adjust.IsChecked = false;
            point.IsChecked = false;
            line.IsChecked = false;
            polyline.IsChecked = false;
            ellipse.IsChecked = false;
            rectangle.IsChecked = false;
            polygon.IsChecked = false;

            save_img.IsEnabled = true;
            save_as_img.IsEnabled = true;
            undo.IsEnabled = false;
            redo.IsEnabled = false;
            toolundo.IsEnabled = false;
            toolredo.IsEnabled = false;
            palette.IsEnabled = false;
            bc_adjust.IsEnabled = false;
            tool_max_zoom.IsEnabled = false;
            tool_min_zoom.IsEnabled = false;
            tool_yuanshi_zoom.IsEnabled = false;
            toolpoint.IsEnabled = false;
            toolline.IsEnabled = false;
            toolpolyline.IsEnabled = false;
            toolellipse.IsEnabled = false;
            toolrectangle.IsEnabled = false;
            toolpolygon.IsEnabled = false;
            tooldelete.IsEnabled = false;
            adjust.IsEnabled = false;
            point.IsEnabled = false;
            line.IsEnabled = false;
            polyline.IsEnabled = false;
            ellipse.IsEnabled = false;
            rectangle.IsEnabled = false;
            polygon.IsEnabled = false;
            deleteshapes.IsEnabled = false;
            save.IsEnabled = true;
            undo.IsEnabled = false;
            redo.IsEnabled = false;
            tool_max_temp.IsEnabled = false;
            tool_min_temp.IsEnabled = false;
            tool_cur_temp.IsEnabled = false;
            max_temp.IsChecked = false;
            min_temp.IsChecked = false;
            cur_temp.IsChecked = false;
            max_temp.IsEnabled = false;
            min_temp.IsEnabled = false;
            cur_temp.IsEnabled = false;
            dismap.IsEnabled = false;
            scale.IsEnabled = false;
            zoomin.IsEnabled = false;
            zoomout.IsEnabled = false;
            repot_ready_btn.IsEnabled = false;
            //draw_cursor.IsEnabled = false;
            //toolyoubiao.IsEnabled = false;

            tool_Isothermal.IsEnabled = false;
            tool_palette.IsEnabled = false;
            tool_bc_adjust.IsEnabled = false;
            tool_dismap.IsEnabled = false;
            tool_scale.IsEnabled = false;
            newreport.IsEnabled = true; ;
            too_per_report.IsEnabled = false;
            print_report.IsEnabled = true;
            temp_mark.IsEnabled = false;
            pic_3d.IsEnabled = false;
            radiance_table.IsEnabled = false;

            tool_pic_3d.IsEnabled = false;
            saveimg.Source = new BitmapImage(new Uri(@"toolbar_img\save_active.png", UriKind.Relative));
            repot_ready_img.Source = new BitmapImage(new Uri(@"toolbar_img\report_ready_black.png", UriKind.Relative));
            max_temp_img.Source = new BitmapImage(new Uri(@"toolbar_img\temp_max_back.png", UriKind.Relative));
            min_temp_img.Source = new BitmapImage(new Uri(@"toolbar_img\temp_min_back.png", UriKind.Relative));
            cur_temp_img.Source = new BitmapImage(new Uri(@"toolbar_img\temp_cur_back.png", UriKind.Relative));
            zoomin_img.Source = new BitmapImage(new Uri(@"toolbar_img\zoom_in_back.png", UriKind.Relative));
            zoomout_img.Source = new BitmapImage(new Uri(@"toolbar_img\zoom_out_back.png", UriKind.Relative));
            deleteshapes_img.Source = new BitmapImage(new Uri(@"toolbar_img\delete_back.png", UriKind.Relative));
            undo_img.Source = new BitmapImage(new Uri(@"toolbar_img\undo_back.png", UriKind.Relative));
            redo_img.Source = new BitmapImage(new Uri(@"toolbar_img\redo_back.png", UriKind.Relative));
            draw_adjust_img.Source = new BitmapImage(new Uri(@"toolbar_img\adjust_back.png", UriKind.Relative));
            draw_point_img.Source = new BitmapImage(new Uri(@"toolbar_img\point_back.png", UriKind.Relative));
            draw_line_img.Source = new BitmapImage(new Uri(@"toolbar_img\line_back.png", UriKind.Relative));
            draw_brokenline_img.Source = new BitmapImage(new Uri(@"toolbar_img\cur_back.png", UriKind.Relative));
            draw_circular_img.Source = new BitmapImage(new Uri(@"toolbar_img\ellipse_back.png", UriKind.Relative));
            draw_rectangle_img.Source = new BitmapImage(new Uri(@"toolbar_img\rectangle_back.png", UriKind.Relative));
            draw_polygon_img.Source = new BitmapImage(new Uri(@"toolbar_img\pol_back.png", UriKind.Relative));

            draw_Isothermal_img.Source = new BitmapImage(new Uri(@"toolbar_img\isothermal_back.png", UriKind.Relative));
            draw_palette_img.Source = new BitmapImage(new Uri(@"toolbar_img\palette_back.png", UriKind.Relative));
            draw_bc_adjust_img.Source = new BitmapImage(new Uri(@"toolbar_img\bc_adjust_back.png", UriKind.Relative));

            draw_dismap_img.Source = new BitmapImage(new Uri(@"toolbar_img\fenbu_back.png", UriKind.Relative));
            draw_scale_img.Source = new BitmapImage(new Uri(@"toolbar_img\bili_back.png", UriKind.Relative));
            draw_3d_img.Source = new BitmapImage(new Uri(@"toolbar_img\3d_back.png", UriKind.Relative));


            stackpanel_tool.Opacity = 0;
            spot.Opacity = 0;
            list.Opacity = 0;

        }

        private void dismap_Click(object sender, RoutedEventArgs e)//分布图
        {
            subwindow_content.Children.Clear();
            sub_dismap newdismap = new sub_dismap();
            newdismap.ImageMouseUp += new RoutedPropertyChangedEventHandler<object>(newdismap_ImageMouseUp);
            newdismap.ReportMouseDown += new RoutedPropertyChangedEventHandler<object>(newdismap_ReportMouseDown);
            newdismap.ReportMouseUp += new RoutedPropertyChangedEventHandler<object>(newdismap_ReportMouseUp);
            newdismap.leftpanel.Width = 0;
            subwindow.Width = 930;
            subwindow.Height = 400;
            newdismap.Name = "newdismap";
            PublicClass.report_type = "dismapreport";
            subwindow_content.Children.Add(newdismap);
            subwindow.Opacity = 0.9;

            Panel.SetZIndex(submainwindow, 3000);

            //C1Window newwindow = new C1Window();
            //newwindow.Content = newdismap;
            //newwindow.ShowModal();

            PublicClass.open_newisothermal = false;


            //空动画//
            DoubleAnimation leftpanel_animation = new DoubleAnimation();
            DoubleAnimation subwindow_animation = new DoubleAnimation();
            ThicknessAnimation newthickness = new ThicknessAnimation();

            leftpanel_animation.FillBehavior = FillBehavior.Stop;
            subwindow_animation.FillBehavior = FillBehavior.Stop;
            newthickness.FillBehavior = FillBehavior.Stop;

            leftpanel_animation.Duration = TimeSpan.FromSeconds(0.1);
            subwindow_animation.Duration = TimeSpan.FromSeconds(0.1);
            newthickness.Duration = TimeSpan.FromSeconds(0.1);

            subwindow.BeginAnimation(StackPanel.MarginProperty, newthickness);
            newdismap.leftpanel.BeginAnimation(StackPanel.WidthProperty, leftpanel_animation);
            subwindow.BeginAnimation(StackPanel.WidthProperty, subwindow_animation);

            //空动画结束//


        }

        void newdismap_ReportMouseUp(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DoubleAnimation opacity_ani = new DoubleAnimation();
            opacity_ani.To = 0;
            opacity_ani.Duration = TimeSpan.FromSeconds(0.8);
            opacity_ani.FillBehavior = FillBehavior.Stop;

            DoubleAnimation width_ani = new DoubleAnimation();
            width_ani.To = 150;
            width_ani.Duration = TimeSpan.FromSeconds(0.8);
            width_ani.FillBehavior = FillBehavior.Stop;

            DoubleAnimation height_ani = new DoubleAnimation();
            height_ani.To = 80;
            height_ani.Duration = TimeSpan.FromSeconds(0.8);
            height_ani.FillBehavior = FillBehavior.Stop;

            ThicknessAnimation thick_ani = new ThicknessAnimation();
            thick_ani.To = new Thickness(this.ActualWidth - 280, this.ActualHeight - 100, 0, 0);
            thick_ani.Duration = TimeSpan.FromSeconds(0.8);
            thick_ani.FillBehavior = FillBehavior.Stop;

            img_animi.BeginAnimation(Image.OpacityProperty, opacity_ani);
            img_animi.BeginAnimation(Image.WidthProperty, width_ani);
            img_animi.BeginAnimation(Image.HeightProperty, height_ani);
            img_animi.BeginAnimation(Image.MarginProperty, thick_ani);
            img_animi.Opacity = 0;
            img_animi.Width = 0;
            img_animi.Height = 0;
            sub_dismap newdismap = MainWindow.FindChild<sub_dismap>(Application.Current.MainWindow, "newdismap");
            RenderTargetBitmap repot_image;
            repot_image = new RenderTargetBitmap((int)(newdismap.report_panel.ActualWidth), (int)newdismap.report_panel.ActualHeight, 96, 96, PixelFormats.Default);
            repot_image.Render(newdismap.report_panel);     
        }

        void newdismap_ReportMouseDown(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            sub_dismap newdismap = MainWindow.FindChild<sub_dismap>(Application.Current.MainWindow, "newdismap");
            repot_ready.Items.Add(newdismap.report);
            repot_scrol.ScrollToBottom();
            RenderTargetBitmap repot_image;
            repot_image = new RenderTargetBitmap((int)(newdismap.report_panel.ActualWidth), (int)newdismap.report_panel.ActualHeight, 96, 96, PixelFormats.Default);
            repot_image.Render(newdismap.report_panel);
            img_animi.Source = repot_image;
            img_animi.Width = repot_image.Width;
            img_animi.Height = repot_image.Height;
            img_animi.Opacity = 1;
            if (subwindow.Width == 930)
            {
                img_animi.Margin = new Thickness(subwindow.Margin.Left, subwindow.Margin.Top + 31, 0, 0);
            }
            else
            {
                img_animi.Margin = new Thickness(subwindow.Margin.Left+200, subwindow.Margin.Top + 31, 0, 0);
            }
            

            DoubleAnimation opacity_ani = new DoubleAnimation();
            opacity_ani.To = img_animi.Opacity;
            opacity_ani.Duration = TimeSpan.FromSeconds(0.1);
            opacity_ani.FillBehavior = FillBehavior.Stop;

            DoubleAnimation width_ani = new DoubleAnimation();
                width_ani.To = img_animi.Width;
            width_ani.Duration = TimeSpan.FromSeconds(0.1);
            width_ani.FillBehavior = FillBehavior.Stop;

            DoubleAnimation height_ani = new DoubleAnimation();
                height_ani.To = img_animi.Height;
            height_ani.Duration = TimeSpan.FromSeconds(0.1);
            height_ani.FillBehavior = FillBehavior.Stop;

            ThicknessAnimation thick_ani = new ThicknessAnimation();
            thick_ani.To = new Thickness(img_animi.Margin.Left, img_animi.Margin.Top, 0, 0);
            thick_ani.Duration = TimeSpan.FromSeconds(0.1);
            thick_ani.FillBehavior = FillBehavior.Stop;

            img_animi.BeginAnimation(Image.OpacityProperty, opacity_ani);
            img_animi.BeginAnimation(Image.WidthProperty, width_ani);
            img_animi.BeginAnimation(Image.HeightProperty, height_ani);
            img_animi.BeginAnimation(Image.MarginProperty, thick_ani);
        }


        void newdismap_ImageMouseUp(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            sub_dismap newdismap = MainWindow.FindChild<sub_dismap>(Application.Current.MainWindow, "newdismap");
            DoubleAnimation leftpanel_animation = new DoubleAnimation();
            DoubleAnimation subwindow_animation = new DoubleAnimation();
            ThicknessAnimation newthickness = new ThicknessAnimation();
            
            leftpanel_animation.FillBehavior = FillBehavior.Stop;
            subwindow_animation.FillBehavior = FillBehavior.Stop;
            newthickness.FillBehavior = FillBehavior.Stop;
            leftpanel_animation.Duration = TimeSpan.FromSeconds(0.1);
            subwindow_animation.Duration = TimeSpan.FromSeconds(0.1);
            newthickness.Duration = TimeSpan.FromSeconds(0.1);
            if (newdismap.leftpanel.Width == 0)
            {
                leftpanel_animation.To = 200;
                subwindow_animation.To = subwindow.Width + 200;
                newthickness.To = new Thickness(subwindow.Margin.Left - 200, subwindow.Margin.Top, subwindow.Margin.Right, subwindow.Margin.Bottom);

            }
            else
            {
                leftpanel_animation.To = 0;
                subwindow_animation.To = subwindow.Width - 200;
                newthickness.To = new Thickness(subwindow.Margin.Left + 200, subwindow.Margin.Top, subwindow.Margin.Right, subwindow.Margin.Bottom);


            }
            subwindow.BeginAnimation(StackPanel.MarginProperty, newthickness);
            newdismap.leftpanel.BeginAnimation(StackPanel.WidthProperty, leftpanel_animation);
            subwindow.BeginAnimation(StackPanel.WidthProperty, subwindow_animation);




            if (newdismap.leftpanel.Width == 0)
            {
                subwindow.Margin = new Thickness(subwindow.Margin.Left - 200, subwindow.Margin.Top, subwindow.Margin.Right, subwindow.Margin.Bottom);
                newdismap.leftpanel.Width = 200;
                subwindow.Width = subwindow.Width + 200;
                newdismap.left_arr.Source = new BitmapImage(new Uri(@"images\arr_right.png", UriKind.Relative)); 
            }
            else
            {
                subwindow.Margin = new Thickness(subwindow.Margin.Left + 200, subwindow.Margin.Top, subwindow.Margin.Right, subwindow.Margin.Bottom);
                newdismap.leftpanel.Width = 0;
                subwindow.Width = subwindow.Width - 200;
                newdismap.left_arr.Source = new BitmapImage(new Uri(@"images\arr_left.png", UriKind.Relative));
            }



        }



        private void zoom_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)//放大缩小滚动条
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            if (workspace != null)
            {
                workspace.zoom_coe = (float)zoom_slider.Value;
                zoom_textblock.Text = (int)(zoom_slider.Value * 100) + "%";
                workspace.zoom();
            }
        }

        private void max_temp_Click(object sender, RoutedEventArgs e)//图像最高温
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
               PublicClass.shapes_property test = new PublicClass.shapes_property();

               if ((bool)max_temp.IsChecked)
               {
                   for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                   {
                       test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                       if (test.shapes_type == "max_temp_temp" && test.workspace_name==PublicClass.cur_ctrl_name)
                       {
                           test.shapes_type = "max_temp";
                           PublicClass.shapes_count[i] = test;
                           workspace.re_create_shapes();
                       }
                   }
                   tool_max_temp.IsChecked = true;
               }
               else
               {
                   for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                   {
                       test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                       if (test.shapes_type == "max_temp" && test.workspace_name == PublicClass.cur_ctrl_name)
                       {
                           test.shapes_type = "max_temp_temp";
                           PublicClass.shapes_count[i] = test;
                           workspace.re_create_shapes();
                       }
                   }
                   tool_max_temp.IsChecked = false;
               }

        }

        private void min_temp_Click(object sender, RoutedEventArgs e)//图像最低温
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            PublicClass.shapes_property test = new PublicClass.shapes_property();

            if ((bool)min_temp.IsChecked)
            {
                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                    if (test.shapes_type == "min_temp_temp" && test.workspace_name == PublicClass.cur_ctrl_name)
                    {
                        test.shapes_type = "min_temp";
                        PublicClass.shapes_count[i] = test;
                        workspace.re_create_shapes();
                    }
                }
                tool_min_temp.IsChecked = true;
            }
            else
            {
                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                    if (test.shapes_type == "min_temp" && test.workspace_name == PublicClass.cur_ctrl_name)
                    {
                        test.shapes_type = "min_temp_temp";
                        PublicClass.shapes_count[i] = test;
                        workspace.re_create_shapes();
                    }
                }
                tool_min_temp.IsChecked = false;
            }
        }

        private void cur_temp_Click(object sender, RoutedEventArgs e)//鼠标光标处温度
        {
            if ((bool)cur_temp.IsChecked)
            {
                PublicClass.is_cur_temp = "true";
            }
            else
            {
                PublicClass.is_cur_temp = "false";
            }
        }

        private void deleteshapes_Click(object sender, RoutedEventArgs e)//删除图形
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            for (int i = 0; i < PublicClass.shapes_count.Count; i++)
            {
                PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                if (newshapes.workspace_name == PublicClass.cur_ctrl_name && newshapes.shapes_name == workspace.shapes_active)
                {
                   workspace.deleteshapes();
                   workspace.histroy_shapes_count.Add(newshapes);
                   workspace.histroy_shapes_operation.Add("delete");
                   workspace.histroy_shapes_index = workspace.histroy_shapes_count.Count - 1;
                    PublicClass.shapes_count.RemoveAt(i);
                    workspace.public_shapes_index = -1;
                    //workspace.shapes_active = "0";

                    workspace.re_create_shapes();
                    try
                    {
                        sub_shapes_list shap = MainWindow.FindChild<sub_shapes_list>(Application.Current.MainWindow, "shapestest");
                        shap.shapeslist("insert", 0);
                    }
                    catch { }
                    break;
                }
            }



            for (int i = 0; i < workspace.vertex_shapes_name.Count; i++)
            {
                sub_ellipse_animation deleteell = workspace.ir_canvas_font.FindName(workspace.vertex_shapes_name[i]) as sub_ellipse_animation;
                if (deleteell != null)
                {
                    workspace.ir_canvas_font.Children.Remove(deleteell);
                    workspace.ir_canvas_font.UnregisterName(workspace.vertex_shapes_name[i]);
                }

            }
            workspace.vertex_shapes_name.Clear();
            
              
                    deleteshapes.IsEnabled = false;
                    deleteshapes_img.Source = new BitmapImage(new Uri(@"toolbar_img\delete_back.png", UriKind.Relative));
              
            
        }

        private void undo_Click(object sender, RoutedEventArgs e)//撤销操作
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            if (workspace.histroy_shapes_index >= 0 && workspace.histroy_shapes_index<workspace.histroy_shapes_count.Count)
            {
                if (workspace.histroy_shapes_operation[workspace.histroy_shapes_index] == "insert")
                {
                  
                    PublicClass.shapes_property histroyshapes = (PublicClass.shapes_property)workspace.histroy_shapes_count[workspace.histroy_shapes_index];
                    for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                    {
                        PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                        if (newshapes.shapes_name == histroyshapes.shapes_name && newshapes.workspace_name == PublicClass.cur_ctrl_name && histroyshapes.shapes_type != "work" && histroyshapes.shapes_type != "max_temp" && histroyshapes.shapes_type != "min_temp" && histroyshapes.shapes_type != "spot" && histroyshapes.shapes_type != "area")
                        {
                            workspace.deleteshapes();
                            PublicClass.shapes_count.RemoveAt(i);
                            workspace.public_shapes_index = -1;
                            //workspace.shapes_active = "0";

                            workspace.re_create_shapes();
                            try
                            {
                                sub_shapes_list shap = MainWindow.FindChild<sub_shapes_list>(Application.Current.MainWindow, "shapestest");
                                shap.shapeslist("insert", 0);
                            }
                            catch{}
                            break;
                        }
                    }
                }
                else if (workspace.histroy_shapes_operation[workspace.histroy_shapes_index] == "update")
                {
                    PublicClass.shapes_property histroyshapes = (PublicClass.shapes_property)workspace.histroy_shapes_count[workspace.histroy_shapes_index];
                    for (int i = workspace.histroy_shapes_index-1; i > -1; i--)
                    {
                        PublicClass.shapes_property histroyshapes_temp = (PublicClass.shapes_property)workspace.histroy_shapes_count[i];
                        if (histroyshapes.shapes_name == histroyshapes_temp.shapes_name)
                        {
                            histroyshapes = histroyshapes_temp;
                            break;
                        }
                    }
                        for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                        {
                            PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                            if (newshapes.shapes_name == histroyshapes.shapes_name && newshapes.workspace_name == PublicClass.cur_ctrl_name)
                            {
                            
                                PublicClass.shapes_count.RemoveAt(i);
                                workspace.public_shapes_index = -1;
                                PublicClass.shapes_count.Add(histroyshapes);
                                workspace.canvas_font_mouseup();
                                break;
                            }
                        }
                }
                else if (workspace.histroy_shapes_operation[workspace.histroy_shapes_index] == "delete")
                {
                    PublicClass.shapes_property histroyshapes = (PublicClass.shapes_property)workspace.histroy_shapes_count[workspace.histroy_shapes_index];
                    workspace.public_shapes_index = -1;
                    PublicClass.shapes_count.Add(histroyshapes);
                    workspace.re_create_shapes();
                    try
                    {

                        sub_shapes_list shap = MainWindow.FindChild<sub_shapes_list>(Application.Current.MainWindow, "shapestest");
                        shap.shapeslist("insert", 0);
                    }
                    catch { }


               }

            }
                     
            workspace.histroy_shapes_index--;
      
          
            if (workspace.histroy_shapes_index < 0)
            {
                undo_img.Source = new BitmapImage(new Uri(@"toolbar_img\undo_back.png", UriKind.Relative));
                undo.IsEnabled = false;
                toolundo.IsEnabled = false;
                
            }
            if (workspace.histroy_shapes_index < workspace.histroy_shapes_count.Count)
            {
                redo_img.Source = new BitmapImage(new Uri(@"toolbar_img\redo_active.png", UriKind.Relative));
                redo.IsEnabled = true;
                toolredo.IsEnabled = true;
            }
            workspace.calculate_percent();
            if (workspace.public_shapes_index > -1)
            {
                PublicClass.shapes_property newshapesT = (PublicClass.shapes_property)PublicClass.shapes_count[workspace.public_shapes_index];
                if (newshapesT.shapes_type != "work" && newshapesT.shapes_type != "spot" && newshapesT.shapes_type != "area" && newshapesT.shapes_type != "max_temp_temp" && newshapesT.shapes_type != "min_temp_temp")
                {
                    deleteshapes.IsEnabled = true;
                    deleteshapes_img.Source = new BitmapImage(new Uri(@"toolbar_img\delete_active.png", UriKind.Relative));

                }
                
            }
            else
            {
                deleteshapes.IsEnabled = false;
                deleteshapes_img.Source = new BitmapImage(new Uri(@"toolbar_img\delete_back.png", UriKind.Relative));
            }
        }

        private void redo_Click(object sender, RoutedEventArgs e)//重做操作
        {

            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            workspace.histroy_shapes_index++;
            if (workspace.histroy_shapes_index >= workspace.histroy_shapes_count.Count)
            {
                workspace.histroy_shapes_index = workspace.histroy_shapes_count.Count - 1;
                
            }

            if (workspace.histroy_shapes_index == workspace.histroy_shapes_count.Count - 1)
            {
                redo_img.Source = new BitmapImage(new Uri(@"toolbar_img\redo_back.png", UriKind.Relative));
                redo.IsEnabled = false;
                toolredo.IsEnabled = false;
                
            }
            if (workspace.histroy_shapes_index > 0)
            {
                undo_img.Source = new BitmapImage(new Uri(@"toolbar_img\undo_active.png", UriKind.Relative));
                undo.IsEnabled = true;
                toolundo.IsEnabled = true;
            }


            if (workspace.histroy_shapes_index < workspace.histroy_shapes_count.Count)
            {

                if (workspace.histroy_shapes_operation[workspace.histroy_shapes_index] == "insert")
                {
                    PublicClass.shapes_property histroyshapes = (PublicClass.shapes_property)workspace.histroy_shapes_count[workspace.histroy_shapes_index];
                    PublicClass.shapes_count.Add(histroyshapes);
                    workspace.re_create_shapes();
                }
               else if (workspace.histroy_shapes_operation[workspace.histroy_shapes_index] == "update")
                {
                    PublicClass.shapes_property histroyshapes = (PublicClass.shapes_property)workspace.histroy_shapes_count[workspace.histroy_shapes_index];
                    for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                    {
                        PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                        if (newshapes.shapes_name == histroyshapes.shapes_name && newshapes.workspace_name == PublicClass.cur_ctrl_name)
                        {

                            PublicClass.shapes_count.RemoveAt(i);
                            workspace.public_shapes_index = -1;
                            PublicClass.shapes_count.Add(histroyshapes);
                            workspace.canvas_font_mouseup();
                            break;
                        }
                    }
                }

                else if (workspace.histroy_shapes_operation[workspace.histroy_shapes_index] == "delete")
                {
                    PublicClass.shapes_property histroyshapes = (PublicClass.shapes_property)workspace.histroy_shapes_count[workspace.histroy_shapes_index];
                    for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                    {
                        PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                        if (newshapes.shapes_name == histroyshapes.shapes_name && newshapes.workspace_name == PublicClass.cur_ctrl_name && histroyshapes.shapes_type != "work" && histroyshapes.shapes_type != "max_temp" && histroyshapes.shapes_type != "min_temp" && histroyshapes.shapes_type != "spot" && histroyshapes.shapes_type != "area")
                        {
                            workspace.deleteshapes();
                            PublicClass.shapes_count.RemoveAt(i);
                            workspace.public_shapes_index = -1;
                            workspace.re_create_shapes();
                            sub_shapes_list shap = MainWindow.FindChild<sub_shapes_list>(Application.Current.MainWindow, "shapestest");
                            shap.shapeslist("insert", 0);
                            break;
                        }
                    }

                }
                workspace.canvas_font_mouseup();
            }
            workspace.calculate_percent();
            if (workspace.public_shapes_index > -1)
            {
                PublicClass.shapes_property newshapesT = (PublicClass.shapes_property)PublicClass.shapes_count[workspace.public_shapes_index];
                if (newshapesT.shapes_type != "work" && newshapesT.shapes_type != "spot" && newshapesT.shapes_type != "area" && newshapesT.shapes_type != "max_temp_temp" && newshapesT.shapes_type != "min_temp_temp")
                {
                    deleteshapes.IsEnabled = true;
                    deleteshapes_img.Source = new BitmapImage(new Uri(@"toolbar_img\delete_active.png", UriKind.Relative));

                }
              
            }
            else
            {
                deleteshapes.IsEnabled = false;
                deleteshapes_img.Source = new BitmapImage(new Uri(@"toolbar_img\delete_back.png", UriKind.Relative));
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (toolpoint.IsChecked == false)
            {
                point.IsChecked = true;
                line.IsChecked = false;
                adjust.IsChecked = false;
                ellipse.IsChecked = false;
                rectangle.IsChecked = false;
                polyline.IsChecked = false;
                polygon.IsChecked = false;
                draw_adjust_img.Source = new BitmapImage(new Uri(@"toolbar_img\adjust_back.png", UriKind.Relative));
                draw_point_img.Source = new BitmapImage(new Uri(@"toolbar_img\point_active.png", UriKind.Relative));
                draw_line_img.Source = new BitmapImage(new Uri(@"toolbar_img\line_back.png", UriKind.Relative));
                draw_brokenline_img.Source = new BitmapImage(new Uri(@"toolbar_img\cur_back.png", UriKind.Relative));
                draw_circular_img.Source = new BitmapImage(new Uri(@"toolbar_img\ellipse_back.png", UriKind.Relative));
                draw_rectangle_img.Source = new BitmapImage(new Uri(@"toolbar_img\rectangle_back.png", UriKind.Relative));
                draw_polygon_img.Source = new BitmapImage(new Uri(@"toolbar_img\pol_back.png", UriKind.Relative));
                PublicClass.is_draw_type = "point";
                toolpoint.IsChecked = true;
                toolline.IsChecked = false;
                toolpolyline.IsChecked = false;
                toolellipse.IsChecked = false;
                toolpolygon.IsChecked = false;
                toolrectangle.IsChecked = false;
            }
            else
            {
                point.IsChecked = false;
                toolpoint.IsChecked = false;
                adjust.IsChecked = true;
                draw_point_img.Source = new BitmapImage(new Uri(@"toolbar_img\point_back.png", UriKind.Relative));
                PublicClass.is_draw_type = "adjust";
               
            }
       
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if ((bool)toolline.IsChecked == false)
            {
                point.IsChecked = false;
                line.IsChecked = true;
                adjust.IsChecked = false;
                ellipse.IsChecked = false;
                rectangle.IsChecked = false;
                polyline.IsChecked = false;
                polygon.IsChecked = false;
                draw_adjust_img.Source = new BitmapImage(new Uri(@"toolbar_img\adjust_back.png", UriKind.Relative));
                draw_point_img.Source = new BitmapImage(new Uri(@"toolbar_img\point_back.png", UriKind.Relative));
                draw_line_img.Source = new BitmapImage(new Uri(@"toolbar_img\line_active.png", UriKind.Relative));
                draw_brokenline_img.Source = new BitmapImage(new Uri(@"toolbar_img\cur_back.png", UriKind.Relative));
                draw_circular_img.Source = new BitmapImage(new Uri(@"toolbar_img\ellipse_back.png", UriKind.Relative));
                draw_rectangle_img.Source = new BitmapImage(new Uri(@"toolbar_img\rectangle_back.png", UriKind.Relative));
                draw_polygon_img.Source = new BitmapImage(new Uri(@"toolbar_img\pol_back.png", UriKind.Relative));
                PublicClass.is_draw_type = "line";
                toolpoint.IsChecked = false;
                toolline.IsChecked = true;
                toolpolyline.IsChecked = false;
                toolellipse.IsChecked = false;
                toolpolygon.IsChecked = false;
                toolrectangle.IsChecked = false;
            }
            else
            {
                line.IsChecked = false;
                toolline.IsChecked = false;
                adjust.IsChecked = true;
                draw_line_img.Source = new BitmapImage(new Uri(@"toolbar_img\line_back.png", UriKind.Relative));
                PublicClass.is_draw_type = "adjust";
            }
           
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if ((bool)(toolpolyline.IsChecked == false))
            {
                point.IsChecked = false;
                line.IsChecked = false;
                adjust.IsChecked = false;
                ellipse.IsChecked = false;
                rectangle.IsChecked = false;
                polyline.IsChecked = true;
                polygon.IsChecked = false;
                draw_adjust_img.Source = new BitmapImage(new Uri(@"toolbar_img\adjust_back.png", UriKind.Relative));
                draw_point_img.Source = new BitmapImage(new Uri(@"toolbar_img\point_back.png", UriKind.Relative));
                draw_line_img.Source = new BitmapImage(new Uri(@"toolbar_img\line_back.png", UriKind.Relative));
                draw_brokenline_img.Source = new BitmapImage(new Uri(@"toolbar_img\cur_active.png", UriKind.Relative));
                draw_circular_img.Source = new BitmapImage(new Uri(@"toolbar_img\ellipse_back.png", UriKind.Relative));
                draw_rectangle_img.Source = new BitmapImage(new Uri(@"toolbar_img\rectangle_back.png", UriKind.Relative));
                draw_polygon_img.Source = new BitmapImage(new Uri(@"toolbar_img\pol_back.png", UriKind.Relative));
                PublicClass.is_draw_type = "polyline";
                toolpoint.IsChecked = false;
                toolline.IsChecked = false;
                toolpolyline.IsChecked = true;
                toolellipse.IsChecked = false;
                toolpolygon.IsChecked = false;
                toolrectangle.IsChecked = false;
            }
            else
            {
                polyline.IsChecked = false;
                toolpolyline.IsChecked = false;
                adjust.IsChecked = true;
                draw_brokenline_img.Source = new BitmapImage(new Uri(@"toolbar_img\cur_back.png", UriKind.Relative));
                PublicClass.is_draw_type = "adjust";

            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            if ((bool)toolellipse.IsChecked == false)
            {
                point.IsChecked = false;
                line.IsChecked = false;
                adjust.IsChecked = false;
                ellipse.IsChecked = true;
                rectangle.IsChecked = false;
                polyline.IsChecked = false;
                polygon.IsChecked = false;
                draw_adjust_img.Source = new BitmapImage(new Uri(@"toolbar_img\adjust_back.png", UriKind.Relative));
                draw_point_img.Source = new BitmapImage(new Uri(@"toolbar_img\point_back.png", UriKind.Relative));
                draw_line_img.Source = new BitmapImage(new Uri(@"toolbar_img\line_back.png", UriKind.Relative));
                draw_brokenline_img.Source = new BitmapImage(new Uri(@"toolbar_img\cur_back.png", UriKind.Relative));
                draw_circular_img.Source = new BitmapImage(new Uri(@"toolbar_img\ellipse_active.png", UriKind.Relative));
                draw_rectangle_img.Source = new BitmapImage(new Uri(@"toolbar_img\rectangle_back.png", UriKind.Relative));
                draw_polygon_img.Source = new BitmapImage(new Uri(@"toolbar_img\pol_back.png", UriKind.Relative));
                PublicClass.is_draw_type = "ellipse";
                toolpoint.IsChecked = false;
                toolline.IsChecked = false;
                toolpolyline.IsChecked = false;
                toolellipse.IsChecked = true;
                toolpolygon.IsChecked = false;
                toolrectangle.IsChecked = false;
            }
            else
            {
                ellipse.IsChecked = false;
                toolellipse.IsChecked = false;
                adjust.IsChecked = true;
                draw_circular_img.Source = new BitmapImage(new Uri(@"toolbar_img\ellipse_back.png", UriKind.Relative));
                PublicClass.is_draw_type = "adjust";
            }
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            if ((bool)toolrectangle.IsChecked == false)
            {
                point.IsChecked = false;
                line.IsChecked = false;
                adjust.IsChecked = false;
                ellipse.IsChecked = false;
                rectangle.IsChecked = true;
                polyline.IsChecked = false;
                polygon.IsChecked = false;
                draw_adjust_img.Source = new BitmapImage(new Uri(@"toolbar_img\adjust_back.png", UriKind.Relative));
                draw_point_img.Source = new BitmapImage(new Uri(@"toolbar_img\point_back.png", UriKind.Relative));
                draw_line_img.Source = new BitmapImage(new Uri(@"toolbar_img\line_back.png", UriKind.Relative));
                draw_brokenline_img.Source = new BitmapImage(new Uri(@"toolbar_img\cur_back.png", UriKind.Relative));
                draw_circular_img.Source = new BitmapImage(new Uri(@"toolbar_img\ellipse_back.png", UriKind.Relative));
                draw_rectangle_img.Source = new BitmapImage(new Uri(@"toolbar_img\rectangle_active.png", UriKind.Relative));
                draw_polygon_img.Source = new BitmapImage(new Uri(@"toolbar_img\pol_back.png", UriKind.Relative));
                PublicClass.is_draw_type = "rectangle";
                toolpoint.IsChecked = false;
                toolline.IsChecked = false;
                toolpolyline.IsChecked = false;
                toolellipse.IsChecked = false;
                toolpolygon.IsChecked = false;
                toolrectangle.IsChecked = true;
            }
            else
            {
                rectangle.IsChecked = false;
                toolrectangle.IsChecked = false;
                adjust.IsChecked = true;
                draw_rectangle_img.Source = new BitmapImage(new Uri(@"toolbar_img\rectangle_back.png", UriKind.Relative));
                PublicClass.is_draw_type = "adjust";

            }
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            if ((bool)toolpolygon.IsChecked == false)
            {
                point.IsChecked = false;
                line.IsChecked = false;
                adjust.IsChecked = false;
                ellipse.IsChecked = false;
                rectangle.IsChecked = false;
                polyline.IsChecked = false;
                polygon.IsChecked = true;
                draw_adjust_img.Source = new BitmapImage(new Uri(@"toolbar_img\adjust_back.png", UriKind.Relative));
                draw_point_img.Source = new BitmapImage(new Uri(@"toolbar_img\point_back.png", UriKind.Relative));
                draw_line_img.Source = new BitmapImage(new Uri(@"toolbar_img\line_back.png", UriKind.Relative));
                draw_brokenline_img.Source = new BitmapImage(new Uri(@"toolbar_img\cur_back.png", UriKind.Relative));
                draw_circular_img.Source = new BitmapImage(new Uri(@"toolbar_img\ellipse_back.png", UriKind.Relative));
                draw_rectangle_img.Source = new BitmapImage(new Uri(@"toolbar_img\rectangle_back.png", UriKind.Relative));
                draw_polygon_img.Source = new BitmapImage(new Uri(@"toolbar_img\pol_active.png", UriKind.Relative));
                PublicClass.is_draw_type = "polygon";
                toolpoint.IsChecked = false;
                toolline.IsChecked = false;
                toolpolyline.IsChecked = false;
                toolellipse.IsChecked = false;
                toolpolygon.IsChecked = true;
                toolrectangle.IsChecked = false;
            }
            else
            {
                polygon.IsChecked = false;
                toolpolygon.IsChecked = false;
                adjust.IsChecked=true;
                draw_polygon_img.Source = new BitmapImage(new Uri(@"toolbar_img\pol_back.png", UriKind.Relative));
                PublicClass.is_draw_type = "adjust";
                point.IsChecked = false;
                line.IsChecked = false;
                adjust.IsChecked = false;
                ellipse.IsChecked = false;
                rectangle.IsChecked = false;
                polyline.IsChecked = false;
            }
        }

   


        private void tool_max_temp_Click(object sender, RoutedEventArgs e)
        {
            
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            PublicClass.shapes_property test = new PublicClass.shapes_property();

            if ((bool)tool_max_temp.IsChecked==false)
            {
                tool_max_temp.IsChecked = true;
                max_temp.IsChecked = true;
                
                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                    if (test.shapes_type == "max_temp_temp" && test.workspace_name == PublicClass.cur_ctrl_name)
                    {
                        test.shapes_type = "max_temp";
                        PublicClass.shapes_count[i] = test;
                        workspace.re_create_shapes();
                    }
                }
            }
            else
            {
                tool_max_temp.IsChecked = false;
                max_temp.IsChecked = false;
                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                    if (test.shapes_type == "max_temp" && test.workspace_name == PublicClass.cur_ctrl_name)
                    {
                        test.shapes_type = "max_temp_temp";
                        PublicClass.shapes_count[i] = test;
                        workspace.re_create_shapes();
                    }
                }
            }
        }

        private void tool_min_temp_Click(object sender, RoutedEventArgs e)
        {
            
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            PublicClass.shapes_property test = new PublicClass.shapes_property();

            if ((bool)tool_min_temp.IsChecked==false)
            {
                tool_min_temp.IsChecked = true;
                min_temp.IsChecked = true;
                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                    if (test.shapes_type == "min_temp_temp" && test.workspace_name == PublicClass.cur_ctrl_name)
                    {
                        test.shapes_type = "min_temp";
                        PublicClass.shapes_count[i] = test;
                        workspace.re_create_shapes();
                    }
                }
            }
            else
            {
                tool_min_temp.IsChecked = false;
                min_temp.IsChecked = false;
                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                    if (test.shapes_type == "min_temp" && test.workspace_name == PublicClass.cur_ctrl_name)
                    {
                        test.shapes_type = "min_temp_temp";
                        PublicClass.shapes_count[i] = test;
                        workspace.re_create_shapes();
                    }
                }
            }

        }

        private void tool_cur_temp_Click(object sender, RoutedEventArgs e)
        {
          
            if ((bool)tool_cur_temp.IsChecked==false)
            {
                cur_temp.IsChecked = true;
                tool_cur_temp.IsChecked = true;
                PublicClass.is_cur_temp = "true";
            }
            else
            {
                cur_temp.IsChecked = false;
                tool_cur_temp.IsChecked = false;
                PublicClass.is_cur_temp = "false";
            }
        }

        private void tool_max_zoom_Click(object sender, RoutedEventArgs e)
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            workspace.zoom_coe += 0.2f;
            workspace.zoom();
            zoom_slider.Value = workspace.zoom_coe;
            zoom_textblock.Text = (int)(workspace.zoom_coe * 100) + "%";
            if (zoom_slider.Value == 5)
            {
                tool_max_zoom.IsEnabled = false;
            }
        }

        private void tool_min_zoom_Click(object sender, RoutedEventArgs e)
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            workspace.zoom_coe -= 0.2f;
            workspace.zoom();
            zoom_slider.Value = workspace.zoom_coe;
            zoom_textblock.Text = (int)(workspace.zoom_coe * 100) + "%";
            if (zoom_slider.Value == 0.3)
            {
                tool_min_zoom.IsEnabled = false;
            }
        }

        private void tool_yuanshi_zoom_Click(object sender, RoutedEventArgs e)//原始图片大小
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            workspace.zoom_coe = 1f;
            workspace.zoom();
            zoom_slider.Value = workspace.zoom_coe;
            zoom_textblock.Text = (int)(workspace.zoom_coe * 100) + "%";
        }

        private void shutdown_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();

        }

        private void radiance_table_Click(object sender, RoutedEventArgs e)
        {
            subwindow_content.Children.Clear();
            sub_radtable newradtable = new sub_radtable();
            subwindow.Width = 795;
            subwindow.Height = 470;
            newradtable.Name = "newdismap";
            newradtable.EmsUp += new RoutedPropertyChangedEventHandler<object>(newradtable_EmsUp);
            subwindow_content.Children.Add(newradtable);
            subwindow.Opacity = 0.9;
            
            Panel.SetZIndex(submainwindow, 3000);
            PublicClass.open_newisothermal = false;        
        }

        void newradtable_EmsUp(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            create_ir_information();
            pic_temp_para_infermation();
        }

        private void repotreadybtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            workspace.repot_ready();
        }

        private void repotreadybtn_MouseUp(object sender, MouseButtonEventArgs e)
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            workspace.report_img();
            sub_repot_ready repot = new sub_repot_ready();
            
            repot.sub_repot.repot_ready_image = new Image();
            repot.sub_repot.repot_ready_image.Source = workspace.repot_image;
            repot.img_tooltip.Width = workspace.repot_image.Width * 0.7;
            repot.img_tooltip.Height = workspace.repot_image.Height * 0.7;
            PublicClass.repot_ready_index++;
            repot.sub_repot.repot_ready_name = "ReportReady" + PublicClass.repot_ready_index;
            repot.sub_repot.repot_ready_shapes = (ArrayList)workspace.report_shapes.Clone();
            repot.repot_image.Source = workspace.repot_image;
            repot.repot_txt.Text = repot.sub_repot.repot_ready_name;
            repot.repot_count.Text = "图形数量：" + repot.sub_repot.repot_ready_shapes.Count;
            repot_ready.Items.Add(repot);
            repot_scrol.ScrollToBottom();
        }

        private void repot_ready_btn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
          
                RenderTargetBitmap repot_image;
                repot_image = new RenderTargetBitmap((int)workspace.ActualWidth, (int)workspace.ActualHeight, 96, 96, PixelFormats.Default);
                repot_image.Render(workspace);
                //img_animi = new Image();
                img_animi.Source = repot_image;
                img_animi.Width = repot_image.Width;
                img_animi.Height = repot_image.Height;
                img_animi.Opacity = 1;
                img_animi.Margin = new Thickness(5, 76, 0, 0);
                workspace.repot_ready();
                DoubleAnimation opacity_ani = new DoubleAnimation();
                opacity_ani.To = img_animi.Opacity;
                opacity_ani.Duration = TimeSpan.FromSeconds(0.1);
                opacity_ani.FillBehavior = FillBehavior.Stop;

                DoubleAnimation width_ani = new DoubleAnimation();
                if (workspace.zoom_coe < 1)
                {
                    width_ani.To = workspace.ir_width;
                }
                else
                {
                    width_ani.To = img_animi.Width;
                }

                width_ani.Duration = TimeSpan.FromSeconds(0.1);
                width_ani.FillBehavior = FillBehavior.Stop;

                DoubleAnimation height_ani = new DoubleAnimation();
                if (workspace.zoom_coe < 1)
                {
                    height_ani.To = workspace.ir_height;
                }
                else
                {
                    height_ani.To = img_animi.Height;
                }

                height_ani.Duration = TimeSpan.FromSeconds(0.1);
                height_ani.FillBehavior = FillBehavior.Stop;

                ThicknessAnimation thick_ani = new ThicknessAnimation();
                thick_ani.To = new Thickness(img_animi.Margin.Left, img_animi.Margin.Top, 0, 0);
                thick_ani.Duration = TimeSpan.FromSeconds(0.1);
                thick_ani.FillBehavior = FillBehavior.Stop;

                img_animi.BeginAnimation(Image.OpacityProperty, opacity_ani);
                img_animi.BeginAnimation(Image.WidthProperty, width_ani);
                img_animi.BeginAnimation(Image.HeightProperty, height_ani);
                img_animi.BeginAnimation(Image.MarginProperty, thick_ani);
          

        }

        private void repot_ready_btn_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            DoubleAnimation opacity_ani = new DoubleAnimation();
            opacity_ani.To = 0;
            opacity_ani.Duration = TimeSpan.FromSeconds(0.8);
            opacity_ani.FillBehavior = FillBehavior.Stop;

            DoubleAnimation width_ani = new DoubleAnimation();
            width_ani.To = 150;
            width_ani.Duration = TimeSpan.FromSeconds(0.8);
            width_ani.FillBehavior = FillBehavior.Stop;

            DoubleAnimation height_ani = new DoubleAnimation();
            height_ani.To = 80;
            height_ani.Duration = TimeSpan.FromSeconds(0.8);
            height_ani.FillBehavior = FillBehavior.Stop;

            ThicknessAnimation thick_ani = new ThicknessAnimation();
            thick_ani.To = new Thickness(this.ActualWidth - 280, this.ActualHeight - 100, 0, 0);
            thick_ani.Duration = TimeSpan.FromSeconds(0.8);
            thick_ani.FillBehavior = FillBehavior.Stop;

            img_animi.BeginAnimation(Image.OpacityProperty, opacity_ani);
            img_animi.BeginAnimation(Image.WidthProperty, width_ani);
            img_animi.BeginAnimation(Image.HeightProperty, height_ani);
            img_animi.BeginAnimation(Image.MarginProperty, thick_ani);
            img_animi.Opacity = 0;
            img_animi.Width = 0;
            img_animi.Height = 0;

            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            workspace.report_img();
            sub_repot_ready repot = new sub_repot_ready();
            repot.AllowDrop = true;
            repot.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(repot_PreviewMouseLeftButtonDown);
            repot.sub_repot.repot_ready_image = new Image();
            repot.sub_repot.repot_ready_image.Source = workspace.repot_image;
            PublicClass.repot_ready_index++;
            repot.sub_repot.repot_ready_name = "ReportReady" + PublicClass.repot_ready_index;
            repot.sub_repot.repot_ready_shapes = (ArrayList)workspace.report_shapes.Clone();
            repot.sub_repot.repot_ready_distance = workspace.ir_information[17].ToString() + "m";
            repot.sub_repot.repot_ready_emss = (float.Parse(workspace.ir_information[16].ToString()) / 100).ToString();
            repot.sub_repot.repot_ready_tamb = (float.Parse(workspace.ir_information[10].ToString()) / 10).ToString() + " ℃";
            repot.repot_image.Source = workspace.repot_image;
            repot.img_tooltip.Source = workspace.repot_image;

            string temp_path=System.IO.Path.GetTempPath()+"\\IrAnalyse\\"+Guid.NewGuid()+".jpg";
            using (FileStream outStream = new FileStream(temp_path, FileMode.Create))
            {
                repot.temp_path = temp_path;
                PngBitmapEncoder encoder = new PngBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create(workspace.repot_image));
                encoder.Save(outStream);
            }


            repot.img_tooltip.Width = workspace.repot_image.Width * 0.7;
            repot.img_tooltip.Height = workspace.repot_image.Height * 0.7;
            repot.repot_txt.Text = repot.sub_repot.repot_ready_name;
            repot.repot_count.Text = "图形数量：" + repot.sub_repot.repot_ready_shapes.Count;
            repot_ready.Items.Add(repot);
            repot_scrol.ScrollToBottom();
        }

        void repot_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataObject data = new DataObject(typeof(sub_repot_ready), (sub_repot_ready)sender);
            DragDrop.DoDragDrop((sub_repot_ready)sender, data, DragDropEffects.Copy);

        }

        private void tool3_IsSelectedChanged(object sender, EventArgs e)
        {
          
                try
                {
                    sub_shapes_list shap = list.FindName("shapestest") as sub_shapes_list;
                    shap.shapeslist("all", 0);
                }
                catch
                { 
                
                }
            
        }

        private void tool_base_infermation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)tool_base_infermation.IsChecked)
                {
                    tool_base_infermation.IsChecked = false;
                    tool1.Hide();
                }
                else
                {
                    tool_base_infermation.IsChecked = true;
                    tool1.Show();
                }
            }
            catch { }
          
           
        }

        private void tool_area_infermation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)tool_area_infermation.IsChecked)
                {
                    tool_area_infermation.IsChecked = false;
                    tool2.Hide();
                }
                else
                {
                    tool_area_infermation.IsChecked = true;
                    tool2.Show();
                }
            }
            catch { }
        }

        private void tool_count_infermation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)tool_count_infermation.IsChecked)
                {
                    tool_count_infermation.IsChecked = false;
                    tool3.Hide();
                }
                else
                {
                    tool_count_infermation.IsChecked = true;
                    tool3.Show();
                }
            }
            catch { }
        }

        private void tool_report_infermation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)tool_report_infermation.IsChecked)
                {
                    tool_report_infermation.IsChecked = false;
                    tool4.Hide();
                }
                else
                {
                    tool_report_infermation.IsChecked = true;
                    tool4.Show();
                }
            }
            catch { }
        }

        private void save_img_Click(object sender, RoutedEventArgs e)//保存
        {
            if (PublicClass.cur_ctrl_type == "work")
            {
                sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
                RenderTargetBitmap renderbitmap = new RenderTargetBitmap((int)workspace.ir_canvas.ActualWidth, (int)workspace.ir_canvas.ActualHeight, 96d, 96d, PixelFormats.Pbgra32);
                renderbitmap.Render(workspace.ir_canvas);

                if (workspace.saveurl == "")
                {
                    SaveFileDialog save_img = new SaveFileDialog();

                    save_img.Filter = "PNG文件(*.png)|*.png|JPG文件(*.jpg)|*.jpg";


                    if (save_img.ShowDialog() == true)
                    {
                        string extensionString = System.IO.Path.GetExtension(save_img.SafeFileName.ToLower());


                        using (FileStream outStream = new FileStream(save_img.FileName.ToString(), FileMode.Create))
                        {

                            MemoryStream ms = new MemoryStream();
                            //BitmapEncoder encoder = null;//new PngBitmapEncoder();
                            switch (extensionString)
                            {
                                case ".jpg":
                                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                                    encoder.Frames.Add(BitmapFrame.Create(renderbitmap));
                                    encoder.Save(ms);
                                    break;
                                case ".png":
                                    PngBitmapEncoder encoder1 = new PngBitmapEncoder();
                                    encoder1.Frames.Add(BitmapFrame.Create(renderbitmap));
                                    encoder1.Save(ms);
                                    break;



                            }



                        }
                        workspace.saveurl = save_img.FileName.ToString();
                    }


                }


                if (workspace.saveurl != "")
                {
                    SaveFileDialog save_img = new SaveFileDialog();
                    save_img.FileName = string.Empty; // Default file name
                    string extensionString = System.IO.Path.GetExtension(workspace.saveurl.ToLower());
                    if (extensionString == ".jpg")
                    {
                        using (FileStream outStream = new FileStream(workspace.saveurl, FileMode.Create))
                        {

                            JpegBitmapEncoder encoder = new JpegBitmapEncoder();

                            encoder.Frames.Add(BitmapFrame.Create(renderbitmap));
                            encoder.Save(outStream);
                        }
                    }
                    if (extensionString == ".png")
                    {
                        using (FileStream outStream = new FileStream(workspace.saveurl, FileMode.Create))
                        {

                            PngBitmapEncoder encoder = new PngBitmapEncoder();

                            encoder.Frames.Add(BitmapFrame.Create(renderbitmap));
                            encoder.Save(outStream);
                        }
                    }
                }
            }
            else if (PublicClass.cur_ctrl_type == "report")
            {
                sub_report subrepot = MainWindow.FindChild<sub_report>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
                if (subrepot.savereporturl == "")
                {
                    SaveFileDialog saveFile = new SaveFileDialog();
                    saveFile.Filter = "RichText Files (*.rtf)|*.rtf";
                    if (saveFile.ShowDialog() == true)
                    {
                        RichTextBox doc = new RichTextBox();
                        TextRange documentTextRange = new TextRange(doc.Document.ContentStart, doc.Document.ContentEnd);
                        using (MemoryStream rtfMemoryStream = new MemoryStream())
                        {
                            using (StreamWriter rtfStreamWriter = new StreamWriter(rtfMemoryStream))
                            {
                                string t = new RtfFilter().ConvertFromDocument(subrepot.richTB.Document);
                                string s = t.Replace("\\pict", "\\pict\\pngblip");
                                rtfStreamWriter.Write(s);
                                
                               
                                rtfStreamWriter.Flush();
                                rtfMemoryStream.Seek(0, SeekOrigin.Begin);
                                //Load the MemoryStream into TextRange ranging from start to end of RichTextBox.
                                documentTextRange.Load(rtfMemoryStream, DataFormats.Rtf);
                                using (FileStream fs = File.Create(saveFile.FileName))
                                {
                                    if (System.IO.Path.GetExtension(saveFile.FileName).ToLower() == ".rtf")
                                    {
                                        documentTextRange.Save(fs, DataFormats.Rtf);
                                    }
                                }
                            }
                        }

                        subrepot.savereporturl = saveFile.FileName.ToString();
                    }

                }
                if (subrepot.savereporturl != "")
                {
                    SaveFileDialog save_img = new SaveFileDialog();
                    save_img.FileName = string.Empty; // Default file name
                    string extensionString = System.IO.Path.GetExtension(subrepot.savereporturl.ToLower());
                    RichTextBox doc = new RichTextBox();
                        TextRange documentTextRange = new TextRange(doc.Document.ContentStart, doc.Document.ContentEnd);
                     if (extensionString == ".rtf")
                    {
                        using (MemoryStream rtfMemoryStream = new MemoryStream())
                        {
                            using (StreamWriter rtfStreamWriter = new StreamWriter(rtfMemoryStream))
                            {
                                string t = new RtfFilter().ConvertFromDocument(subrepot.richTB.Document);
                                string s = t.Replace("\\pict", "\\pict\\pngblip");
                                rtfStreamWriter.Write(s);


                                rtfStreamWriter.Flush();
                                rtfMemoryStream.Seek(0, SeekOrigin.Begin);
                                //Load the MemoryStream into TextRange ranging from start to end of RichTextBox.
                                documentTextRange.Load(rtfMemoryStream, DataFormats.Rtf);

                                using (FileStream outStream = new FileStream(subrepot.savereporturl, FileMode.Create))
                                {

                                    documentTextRange.Save(outStream, DataFormats.Rtf);
                                }
                            }
                        }
                    }
                }
           }
        }

        private void save_as_img_Click(object sender, RoutedEventArgs e)//另存
        {
            if (PublicClass.cur_ctrl_type == "work")
            {
                sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);

                RenderTargetBitmap renderbitmap = new RenderTargetBitmap((int)workspace.ir_canvas.ActualWidth, (int)workspace.ir_canvas.ActualHeight, 96d, 96d, PixelFormats.Pbgra32);
                renderbitmap.Render(workspace.ir_canvas);
                SaveFileDialog save_img = new SaveFileDialog();

                save_img.Filter = "PNG文件(*.png)|*.png|JPG文件(*.jpg)|*.jpg";
                string imagFile = save_img.FileName;
                if (save_img.ShowDialog() == true)
                {
                    string extensionString = System.IO.Path.GetExtension(save_img.SafeFileName.ToLower());


                    using (FileStream outStream = new FileStream(save_img.FileName.ToString(), FileMode.Create))
                    {


                        //JpegBitmapEncoder encoder = null;//new PngBitmapEncoder();
                        switch (extensionString)
                        {
                            case ".jpg":
                                PngBitmapEncoder encoder = new PngBitmapEncoder();
                                encoder.Frames.Add(BitmapFrame.Create(renderbitmap));
                                encoder.Save(outStream);
                                break;
                            case ".png":
                                PngBitmapEncoder encoder1 = new PngBitmapEncoder();
                                encoder1.Frames.Add(BitmapFrame.Create(renderbitmap));
                                encoder1.Save(outStream);
                                break;



                        }



                    }
                    workspace.saveurl = save_img.FileName.ToString();
                }
            }


            else if (PublicClass.cur_ctrl_type == "report")
            {
                sub_report subrepot = MainWindow.FindChild<sub_report>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.Filter = "RichText Files (*.rtf)|*.rtf";
                if (saveFile.ShowDialog() == true)
                {
                    RichTextBox doc = new RichTextBox();
                    TextRange documentTextRange = new TextRange(doc.Document.ContentStart, doc.Document.ContentEnd);
                    using (MemoryStream rtfMemoryStream = new MemoryStream())
                    {
                        using (StreamWriter rtfStreamWriter = new StreamWriter(rtfMemoryStream))
                        {
                            string t = new RtfFilter().ConvertFromDocument(subrepot.richTB.Document);
                            string s = t.Replace("\\pict", "\\pict\\pngblip");
                            rtfStreamWriter.Write(s);
                            rtfStreamWriter.Flush();
                            rtfMemoryStream.Seek(0, SeekOrigin.Begin);
                            //Load the MemoryStream into TextRange ranging from start to end of RichTextBox.
                            documentTextRange.Load(rtfMemoryStream, DataFormats.Rtf);
                            using (FileStream fs = File.Create(saveFile.FileName))
                            {
                                if (System.IO.Path.GetExtension(saveFile.FileName).ToLower() == ".rtf")
                                {
                                    documentTextRange.Save(fs, DataFormats.Rtf);
                                }
                            }
                        }
                    }
                }
            }
        }
            



        private void tool1_IsVisibleChanged(object sender, EventArgs e)
        {
            if (tool1.IsHidden)
            {
                tool_base_infermation.IsChecked = false;
            }
        
        }

        private void tool2_IsVisibleChanged(object sender, EventArgs e)
        {
            if (tool2.IsHidden)
            {
                tool_area_infermation.IsChecked = false;
            }
        }

        private void tool3_IsVisibleChanged(object sender, EventArgs e)
        {
            if (tool3.IsHidden)
            {
                tool_count_infermation.IsChecked = false;
            }
        }

        private void tool4_IsVisibleChanged(object sender, EventArgs e)
        {
            if (tool4.IsHidden)
            {
                tool_report_infermation.IsChecked = false;
            }
        }
        private void init_toolbar()//初始化禁用功能
        {


            adjust.IsChecked = false;
            point.IsChecked = false;
            line.IsChecked = false;
            polyline.IsChecked = false;
            ellipse.IsChecked = false;
            rectangle.IsChecked = false;
            polygon.IsChecked = false;
            max_temp.IsChecked = false;
            min_temp.IsChecked = false;

            save_img.IsEnabled = false;
            save_as_img.IsEnabled = false;
            undo.IsEnabled = false;
            redo.IsEnabled = false;
            toolundo.IsEnabled = false;
            toolredo.IsEnabled = false;
            palette.IsEnabled = false;
            bc_adjust.IsEnabled = false;
            tool_max_zoom.IsEnabled = false;
            tool_min_zoom.IsEnabled = false;
            tool_yuanshi_zoom.IsEnabled = false;
            toolpoint.IsEnabled = false;
            toolline.IsEnabled = false;
            toolpolyline.IsEnabled = false;
            toolellipse.IsEnabled = false;
            toolrectangle.IsEnabled = false;
            toolpolygon.IsEnabled = false;
            tooldelete.IsEnabled = false;
            adjust.IsEnabled = false;
            point.IsEnabled = false;
            line.IsEnabled = false;
            polyline.IsEnabled = false;
            ellipse.IsEnabled = false;
            rectangle.IsEnabled = false;
            polygon.IsEnabled = false;
            deleteshapes.IsEnabled = false;
            save.IsEnabled = false;
            undo.IsEnabled = false;
            redo.IsEnabled = false;
            tool_max_temp.IsEnabled = false;
            tool_min_temp.IsEnabled = false;
            tool_cur_temp.IsEnabled = false;
            max_temp.IsEnabled = false;
            min_temp.IsEnabled = false;
            cur_temp.IsEnabled = false;
            dismap.IsEnabled = false;
            scale.IsEnabled = false;
            zoomin.IsEnabled = false;
            zoomout.IsEnabled = false;
            repot_ready_btn.IsEnabled = false;
            //ronghe_btn.IsEnabled = false;
            //toumingdu_slider.IsEnabled = false;
            //draw_cursor.IsEnabled = false;
            //toolyoubiao.IsEnabled = false;

            newreport.IsEnabled = true;
            too_per_report.IsEnabled = false;
            print_report.IsEnabled = false;

            tool_Isothermal.IsEnabled = false;
            tool_palette.IsEnabled = false;
            tool_bc_adjust.IsEnabled = false;
            tool_dismap.IsEnabled = false;
            tool_scale.IsEnabled = false;
            temp_mark.IsEnabled = false;
            pic_3d.IsEnabled = false;
            tool_pic_3d.IsEnabled = false;
            radiance_table.IsEnabled = false;


        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            sub_report subrepot = MainWindow.FindChild<sub_report>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            var dlg = new PrintDialog();
            if (dlg.ShowDialog() == true)
            {

                var paginator = new C1RichTextBoxPaginator(
                                   subrepot.richTB,
                                    new Size(dlg.PrintableAreaWidth, dlg.PrintableAreaHeight),
                                    new Thickness(40, 60, 40, 60) /* hardcoded margin */
                                );
                dlg.PrintDocument(paginator, "Test");
            }

        }

        private void minwindow_KeyDown(object sender, KeyEventArgs e)//快捷键
        {

            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && key == Key.O)
            {
                open_img_Click(null,null);
            }
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && key == Key.S)
            {
                save_img_Click(null, null);
            }
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && key == Key.E)
            {
                radiance_table_Click(null, null);
            }
            else if (key == Key.F1)
            {
               help_Click(null, null);
            }
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && key == Key.Z&&undo.IsEnabled==true)
            {
                undo_Click(null, null);
            }
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && key == Key.Y&&redo.IsEnabled==true)
            {
                redo_Click(null, null);
            }
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && key == Key.D0&&zoomin.IsEnabled==true)
            {
                tool_yuanshi_zoom_Click(null, null);
            }
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Alt && key == Key.X&&tool_max_temp.IsEnabled==true)
            {
                tool_max_temp_Click(null, null);
            }
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Alt && key == Key.N&&tool_min_temp.IsEnabled==true)
            {
                tool_min_temp_Click(null, null);
            }
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Alt && key == Key.M&&tool_cur_temp.IsEnabled==true)
            {
                tool_cur_temp_Click(null, null);
            }
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && key == Key.N&&newreport.IsEnabled==true)
            {
                open_report_Click(null, null);
            }

            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && key == Key.P && print_report.IsEnabled==true)
            {
                MenuItem_Click_6(null, null);
            }
            else if (key == Key.Delete && deleteshapes.IsEnabled)
            {
                deleteshapes_Click(null, null);
            }
        
          
        }


        private void pic_3d_Click(object sender, RoutedEventArgs e)
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);


            sub_3D new3d = new sub_3D();
            subwindow_content.Children.Clear();
            subwindow.Width = 768;
            subwindow.Height = 576;
            new3d.Name = "new3d";
            //Point3DCollection collection = new Point3DCollection();

            for (int y = 0; y < workspace.ir_height; y++)
            {
                for (int x = 0; x < workspace.ir_width; x++)
                {
                    new3d.mesh3d.Positions.Add(new Point3D(x, y, -(int)(int.Parse(workspace.ir_temp[y * workspace.ir_width + x].ToString())) / 10));

                }
            }


            


            for (int y = 0; y < workspace.ir_height - 1; y++)

            {
                for (int x = 0; x < workspace.ir_width - 1; x++)
                {
                    //new3d.mesh3d.Positions.Add(new Point3D(x, y, int.Parse(workspace.ir_temp[y * workspace.ir_width + x].ToString())));
                    new3d.mesh3d.TriangleIndices.Add(y * workspace.ir_width + x);
                    new3d.mesh3d.TriangleIndices.Add((y + 1) * workspace.ir_width + x);
                    new3d.mesh3d.TriangleIndices.Add((y + 1) * workspace.ir_width + x + 1);


                    new3d.mesh3d.TriangleIndices.Add(y * workspace.ir_width + x);
                    new3d.mesh3d.TriangleIndices.Add((y + 1) * workspace.ir_width + x + 1);
                    new3d.mesh3d.TriangleIndices.Add(y * workspace.ir_width + x + 1);


                    new3d.mesh3d.TextureCoordinates.Add(new Point(x, y));
                    if (x == workspace.ir_width - 2)
                    {
                        new3d.mesh3d.TextureCoordinates.Add(new Point(x, y));
                    }
                    //Point3D v0 = new Point3D(new3d.mesh3d.Positions[(y + 1) * workspace.ir_width + x].X - new3d.mesh3d.Positions[y * workspace.ir_width + x].X, new3d.mesh3d.Positions[(y + 1) * workspace.ir_width + x].Y - new3d.mesh3d.Positions[y * workspace.ir_width + x].Y, new3d.mesh3d.Positions[(y + 1) * workspace.ir_width + x].Z - new3d.mesh3d.Positions[y * workspace.ir_width + x].Z);
                    //Point3D v1 = new Point3D(new3d.mesh3d.Positions[(y + 1) * workspace.ir_width + x + 1].X - new3d.mesh3d.Positions[(y + 1) * workspace.ir_width + x].X, new3d.mesh3d.Positions[(y + 1) * workspace.ir_width + x + 1].Y - new3d.mesh3d.Positions[(y + 1) * workspace.ir_width + x].Y, new3d.mesh3d.Positions[(y + 1) * workspace.ir_width + x + 1].Z - new3d.mesh3d.Positions[(y + 1) * workspace.ir_width + x].Z);

                    //new3d.mesh3d.Normals.Add(new Vector3D(0, 1, 0));
                }
            }
            new3d.mesh_img.ImageSource = workspace.irimg.Source;
            new3d.back_brush.ImageSource = workspace.irimg.Source; 

            subwindow_content.Children.Add(new3d);

        
            subwindow.Opacity = 0.9;
            Panel.SetZIndex(submainwindow, 3000);
       


        }

        private void MenuItem_Click_7(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "jpg文件|*.jpg|所有文件|*.*";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "zip";

            if (openFileDialog.ShowDialog() == true)
            {



                FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open,FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                fs.Seek(0x18, SeekOrigin.Begin);//从18h字开始
                string jpg_size_str = len_2(br.ReadByte());
                jpg_size_str += len_2(br.ReadByte());
                jpg_size_str += len_2(br.ReadByte());
                jpg_size_str += len_2(br.ReadByte());
                int jpg_size = Convert.ToInt32(jpg_size_str, 16);//获取JPG 文件长度

                fs.Seek(jpg_size + 90, SeekOrigin.Begin);
                bool is_ir = true;
                try
                {
                    br.ReadByte();
                }
                catch
                {
                    is_ir = false;
                }
                fs.Close();

                if (is_ir && System.IO.Path.GetExtension(openFileDialog.SafeFileName).ToLower() == ".jpg")
                {
                    sub_workspace newworkspace = new sub_workspace();
                    PublicClass.ctrl_name++;
                    newworkspace.Name = "work" + PublicClass.ctrl_name.ToString();
                    newworkspace.LoadIrInformation += new RoutedPropertyChangedEventHandler<object>(newworkspace_LoadIrInformation);
                    newworkspace.WorkMouseWheel += new RoutedPropertyChangedEventHandler<object>(newworkspace_WorkMouseWheel);
                    newworkspace.WorkMouseUp += new RoutedPropertyChangedEventHandler<object>(newworkspace_WorkMouseUp);
                    newworkspace.filename = openFileDialog.FileName;
                    newworkspace.cur_file_name = openFileDialog.SafeFileName;
                    LayoutDocument newaaa = new LayoutDocument();
                    newaaa.Content = newworkspace;
                    newaaa.Title = openFileDialog.SafeFileName;
                    newaaa.IsSelected = true;
                    newaaa.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(newaaa_Closing);
                    mainpanel.Children.Add(newaaa);
                }
                else if (System.IO.Path.GetExtension(openFileDialog.SafeFileName).ToLower() == ".jpg")
                {
                    sub_workspace subwork = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
                    subwork.filename = openFileDialog.FileName;
                    using (BinaryReader reader = new BinaryReader(File.Open(openFileDialog.FileName, FileMode.Open)))
                    {

                        FileInfo fi = new FileInfo(openFileDialog.FileName);

                        byte[] bytes = reader.ReadBytes((int)fi.Length);

                        reader.Close();

                        BitmapImage bmp = new BitmapImage();

                        bmp.BeginInit();

                        bmp.StreamSource = new MemoryStream(bytes);

                        bmp.EndInit();

                        subwork.ir_back_img.Source = bmp;

                        bmp.CacheOption = BitmapCacheOption.OnLoad;

                    } 
                    subwork.ronghewidth = subwork.ir_back_img.Source.Width;
                    subwork.rongheheight = subwork.ir_back_img.Source.Height;
                    subwork.back_canvas.Width = subwork.irimg.ActualWidth;
                    subwork.back_canvas.Height = subwork.irimg.ActualHeight;
                    subwork.zoom();
                    subwork.ir_back_img.Margin = new Thickness((subwork.irimg.ActualWidth - subwork.ir_back_img.Source.Width*subwork.zoom_coe) / 2, (subwork.irimg.ActualHeight - subwork.ir_back_img.Source.Height*subwork.zoom_coe) / 2,0,0);   
                    subwork.right_panel.Opacity = 0;
                    subwork.ir_canvas.AllowDrop = true;
                    //ronghe_btn.IsChecked = true;
                    subwork.back_canvas.ClipToBounds = false;
                    PublicClass.ronghe_type = true;
                }



            }
            else
            {
                //  return "";
            }


        }


        //private void ronghe_btn_Click(object sender, RoutedEventArgs e)
        //{
        //    sub_workspace subwork = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
        //    if ((bool)ronghe_btn.IsChecked)
        //    {
        //        subwork.back_canvas.ClipToBounds = false;
        //        PublicClass.ronghe_type = true;

        //    }
        //    else
        //    {
        //        subwork.back_canvas.ClipToBounds = true;
        //        PublicClass.ronghe_type = false;
        //        subwork.right_panel.Opacity = 1;
        //    }
        //}

        //private void toumingdu_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    sub_workspace work = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
        //    work.ir_canvas_font.Opacity = toumingdu_slider.Value;
        //    work.create_img();
        //}


        private void too_per_report_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            if (PublicClass.report_type == "workreport")
            {

                sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);

                RenderTargetBitmap repot_image;
                repot_image = new RenderTargetBitmap((int)workspace.ActualWidth, (int)workspace.ActualHeight, 96, 96, PixelFormats.Default);
                repot_image.Render(workspace);
                //img_animi = new Image();
                img_animi.Source = repot_image;
                img_animi.Width = repot_image.Width;
                img_animi.Height = repot_image.Height;
                img_animi.Opacity = 1;
                img_animi.Margin = new Thickness(5, 76, 0, 0);
                workspace.repot_ready();
                DoubleAnimation opacity_ani = new DoubleAnimation();
                opacity_ani.To = img_animi.Opacity;
                opacity_ani.Duration = TimeSpan.FromSeconds(0.1);
                opacity_ani.FillBehavior = FillBehavior.Stop;

                DoubleAnimation width_ani = new DoubleAnimation();
                if (workspace.zoom_coe < 1)
                {
                    width_ani.To = workspace.ir_width;
                }
                else
                {
                    width_ani.To = img_animi.Width;
                }

                width_ani.Duration = TimeSpan.FromSeconds(0.1);
                width_ani.FillBehavior = FillBehavior.Stop;

                DoubleAnimation height_ani = new DoubleAnimation();
                if (workspace.zoom_coe < 1)
                {
                    height_ani.To = workspace.ir_height;
                }
                else
                {
                    height_ani.To = img_animi.Height;
                }

                height_ani.Duration = TimeSpan.FromSeconds(0.1);
                height_ani.FillBehavior = FillBehavior.Stop;

                ThicknessAnimation thick_ani = new ThicknessAnimation();
                thick_ani.To = new Thickness(img_animi.Margin.Left, img_animi.Margin.Top, 0, 0);
                thick_ani.Duration = TimeSpan.FromSeconds(0.1);
                thick_ani.FillBehavior = FillBehavior.Stop;

                img_animi.BeginAnimation(Image.OpacityProperty, opacity_ani);
                img_animi.BeginAnimation(Image.WidthProperty, width_ani);
                img_animi.BeginAnimation(Image.HeightProperty, height_ani);
                img_animi.BeginAnimation(Image.MarginProperty, thick_ani);

            }
            else if (PublicClass.report_type == "scalereport")
            {
            sub_scale newscale = MainWindow.FindChild<sub_scale>(Application.Current.MainWindow, "newscale");
            RenderTargetBitmap repot_image;
            repot_image = new RenderTargetBitmap((int)newscale.report_panel.ActualWidth, (int)newscale.report_panel.ActualHeight, 96, 96, PixelFormats.Default);
            repot_image.Render(newscale.report_panel);
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
              
                PublicClass.repot_ready_index++;
                report.sub_repot.repot_ready_name = "ReportReady" + PublicClass.repot_ready_index;
                report.repot_txt.Text = report.sub_repot.repot_ready_name;

                report.repot_count.Text = "图形数量:" + report.sub_repot.repot_ready_shapes.Count;

                //newscale.ReportMouseDown += new RoutedPropertyChangedEventHandler<object>(newscale_ReportMouseDown);

                //RoutedPropertyChangedEventArgs<object> args = new RoutedPropertyChangedEventArgs<object>(this, e);
                //args.RoutedEvent = sub_scale.ReportMouseDownEvent;
                //this.RaiseEvent(args);



              //  sub_scale anicale = MainWindow.FindChild<sub_scale>(Application.Current.MainWindow, "newscale");
                repot_ready.Items.Add(report);
                repot_scrol.ScrollToBottom();
                //RenderTargetBitmap repot_image1;
                //repot_image1 = new RenderTargetBitmap((int)anicale.report_panel.ActualWidth, (int)anicale.report_panel.ActualHeight + 250, 96, 96, PixelFormats.Default);
                repot_image.Render(newscale.report_panel);
                img_animi.Source = repot_image;
                img_animi.Width = repot_image.Width;
                img_animi.Height = repot_image.Height;
                img_animi.Opacity = 1;


                if (subwindow.Width == 950)
                {
                    img_animi.Margin = new Thickness(subwindow.Margin.Left, subwindow.Margin.Top + 31, 0, 0);
                }
                else
                {
                    img_animi.Margin = new Thickness(subwindow.Margin.Left + 200, subwindow.Margin.Top + 31, 0, 0);
                }




                DoubleAnimation opacity_ani = new DoubleAnimation();
                opacity_ani.To = img_animi.Opacity;
                opacity_ani.Duration = TimeSpan.FromSeconds(0.1);
                opacity_ani.FillBehavior = FillBehavior.Stop;

                DoubleAnimation width_ani = new DoubleAnimation();
                width_ani.To = img_animi.Width;
                width_ani.Duration = TimeSpan.FromSeconds(0.1);
                width_ani.FillBehavior = FillBehavior.Stop;

                DoubleAnimation height_ani = new DoubleAnimation();
                height_ani.To = img_animi.Height;
                height_ani.Duration = TimeSpan.FromSeconds(0.1);
                height_ani.FillBehavior = FillBehavior.Stop;

                ThicknessAnimation thick_ani = new ThicknessAnimation();
                thick_ani.To = new Thickness(img_animi.Margin.Left, img_animi.Margin.Top, 0, 0);
                thick_ani.Duration = TimeSpan.FromSeconds(0.1);
                thick_ani.FillBehavior = FillBehavior.Stop;

                img_animi.BeginAnimation(Image.OpacityProperty, opacity_ani);
                img_animi.BeginAnimation(Image.WidthProperty, width_ani);
                img_animi.BeginAnimation(Image.HeightProperty, height_ani);
                img_animi.BeginAnimation(Image.MarginProperty, thick_ani);
                PublicClass.report_type = "saclereport";

            }

            else if (PublicClass.report_type == "dismapreport")
            {
                sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
                sub_dismap newdismap = MainWindow.FindChild<sub_dismap>(Application.Current.MainWindow, "newdismap");
                RenderTargetBitmap repot_image;
                repot_image = new RenderTargetBitmap((int)(newdismap.report_panel.ActualWidth), (int)newdismap.report_panel.ActualHeight, 96, 96, PixelFormats.Default);
                repot_image.Render(newdismap.report_panel);
                report = new sub_repot_ready();
                report.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(report_PreviewMouseLeftButtonDown);
                report.repot_image.Source = repot_image;
                report.img_tooltip.Source = repot_image;
                report.img_tooltip.Width = repot_image.Width * 0.7;
                report.img_tooltip.Height = repot_image.Height * 0.7;
                report.sub_repot.repot_ready_shapes = new ArrayList();
                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    PublicClass.shapes_property test = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                    int in_shapes = 0;
                    in_shapes = (from c in workspace.selected_spot_area where c == test.shapes_name select c).Count();
                    if (in_shapes > 0 && test.workspace_name == PublicClass.cur_ctrl_name)
                    {
                        report.sub_repot.repot_ready_shapes.Add(test);
                    }
                    else if (test.shapes_type == "line" || test.shapes_type == "polyline")
                    {
                        report.sub_repot.repot_ready_shapes.Add(test);
                    }
                }
                PublicClass.repot_ready_index++;
                report.sub_repot.repot_ready_name = "ReportReady" + PublicClass.repot_ready_index;
                report.repot_txt.Text = report.sub_repot.repot_ready_name;

                report.repot_count.Text = "图形数量:" + report.sub_repot.repot_ready_shapes.Count;




                repot_ready.Items.Add(report);
                repot_scrol.ScrollToBottom();
                //RenderTargetBitmap repot_image;
                //repot_image = new RenderTargetBitmap((int)(newdismap.report_panel.ActualWidth), (int)newdismap.report_panel.ActualHeight, 96, 96, PixelFormats.Default);
                repot_image.Render(newdismap.report_panel);
                img_animi.Source = repot_image;
                img_animi.Width = repot_image.Width;
                img_animi.Height = repot_image.Height;
                img_animi.Opacity = 1;
                if (subwindow.Width == 930)
                {
                    img_animi.Margin = new Thickness(subwindow.Margin.Left, subwindow.Margin.Top + 31, 0, 0);
                }
                else
                {
                    img_animi.Margin = new Thickness(subwindow.Margin.Left + 200, subwindow.Margin.Top + 31, 0, 0);
                }


                DoubleAnimation opacity_ani = new DoubleAnimation();
                opacity_ani.To = img_animi.Opacity;
                opacity_ani.Duration = TimeSpan.FromSeconds(0.1);
                opacity_ani.FillBehavior = FillBehavior.Stop;

                DoubleAnimation width_ani = new DoubleAnimation();
                width_ani.To = img_animi.Width;
                width_ani.Duration = TimeSpan.FromSeconds(0.1);
                width_ani.FillBehavior = FillBehavior.Stop;

                DoubleAnimation height_ani = new DoubleAnimation();
                height_ani.To = img_animi.Height;
                height_ani.Duration = TimeSpan.FromSeconds(0.1);
                height_ani.FillBehavior = FillBehavior.Stop;

                ThicknessAnimation thick_ani = new ThicknessAnimation();
                thick_ani.To = new Thickness(img_animi.Margin.Left, img_animi.Margin.Top, 0, 0);
                thick_ani.Duration = TimeSpan.FromSeconds(0.1);
                thick_ani.FillBehavior = FillBehavior.Stop;

                img_animi.BeginAnimation(Image.OpacityProperty, opacity_ani);
                img_animi.BeginAnimation(Image.WidthProperty, width_ani);
                img_animi.BeginAnimation(Image.HeightProperty, height_ani);
                img_animi.BeginAnimation(Image.MarginProperty, thick_ani);
                PublicClass.report_type = "dismapreport";
            }
        }

        void report_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataObject data = new DataObject(typeof(sub_repot_ready), (sub_repot_ready)sender);
            DragDrop.DoDragDrop((sub_repot_ready)sender, data, DragDropEffects.Copy);
        }

        private void too_per_report_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (PublicClass.report_type == "workreport")
            {
                DoubleAnimation opacity_ani = new DoubleAnimation();
                opacity_ani.To = 0;
                opacity_ani.Duration = TimeSpan.FromSeconds(0.8);
                opacity_ani.FillBehavior = FillBehavior.Stop;

                DoubleAnimation width_ani = new DoubleAnimation();
                width_ani.To = 150;
                width_ani.Duration = TimeSpan.FromSeconds(0.8);
                width_ani.FillBehavior = FillBehavior.Stop;

                DoubleAnimation height_ani = new DoubleAnimation();
                height_ani.To = 80;
                height_ani.Duration = TimeSpan.FromSeconds(0.8);
                height_ani.FillBehavior = FillBehavior.Stop;

                ThicknessAnimation thick_ani = new ThicknessAnimation();
                thick_ani.To = new Thickness(this.ActualWidth - 280, this.ActualHeight - 100, 0, 0);
                thick_ani.Duration = TimeSpan.FromSeconds(0.8);
                thick_ani.FillBehavior = FillBehavior.Stop;

                img_animi.BeginAnimation(Image.OpacityProperty, opacity_ani);
                img_animi.BeginAnimation(Image.WidthProperty, width_ani);
                img_animi.BeginAnimation(Image.HeightProperty, height_ani);
                img_animi.BeginAnimation(Image.MarginProperty, thick_ani);
                img_animi.Opacity = 0;
                img_animi.Width = 0;
                img_animi.Height = 0;

                sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
                workspace.report_img();
                sub_repot_ready repot = new sub_repot_ready();
                repot.AllowDrop = true;
                repot.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(repot_PreviewMouseLeftButtonDown);
                repot.sub_repot.repot_ready_image = new Image();
                repot.sub_repot.repot_ready_image.Source = workspace.repot_image;
                PublicClass.repot_ready_index++;
                repot.sub_repot.repot_ready_name = "ReportReady" + PublicClass.repot_ready_index;
                repot.sub_repot.repot_ready_shapes = (ArrayList)workspace.report_shapes.Clone();
                repot.repot_image.Source = workspace.repot_image;
                repot.img_tooltip.Source = workspace.repot_image;
                repot.img_tooltip.Width = workspace.repot_image.Width * 0.7;
                repot.img_tooltip.Height = workspace.repot_image.Height * 0.7;
                repot.repot_txt.Text = repot.sub_repot.repot_ready_name;
                repot.repot_count.Text = "图形数量：" + repot.sub_repot.repot_ready_shapes.Count;
                repot_ready.Items.Add(repot);
                repot_scrol.ScrollToBottom();
            }

            //else if (PublicClass.report_type == "scalereport")
            //{
            //sub_scale newscale = MainWindow.FindChild<sub_scale>(Application.Current.MainWindow, "newscale");
            //newscale.ReportMouseUp += new RoutedPropertyChangedEventHandler<object>(newscale_ReportMouseUp);
            //RoutedPropertyChangedEventArgs<object> args = new RoutedPropertyChangedEventArgs<object>(this, e);
            //args.RoutedEvent = sub_scale.ReportMouseUpEvent;
            //this.RaiseEvent(args);
            //}


           // newscale.ReportMouseDown += new RoutedPropertyChangedEventHandler<object>(newscale_ReportMouseDown);
            //sub_scale newscale = MainWindow.FindChild<sub_scale>(Application.Current.MainWindow, "newscale");
            //newscale.ReportMouseUp += new RoutedPropertyChangedEventHandler<object>(newscale_ReportMouseUp);

            else if (PublicClass.report_type == "saclereport")
            {


                DoubleAnimation opacity_ani = new DoubleAnimation();
                opacity_ani.To = 0;
                opacity_ani.Duration = TimeSpan.FromSeconds(0.8);
                opacity_ani.FillBehavior = FillBehavior.Stop;

                DoubleAnimation width_ani = new DoubleAnimation();
                width_ani.To = 150;
                width_ani.Duration = TimeSpan.FromSeconds(0.8);
                width_ani.FillBehavior = FillBehavior.Stop;

                DoubleAnimation height_ani = new DoubleAnimation();
                height_ani.To = 80;
                height_ani.Duration = TimeSpan.FromSeconds(0.8);
                height_ani.FillBehavior = FillBehavior.Stop;

                ThicknessAnimation thick_ani = new ThicknessAnimation();
                thick_ani.To = new Thickness(this.ActualWidth - 280, this.ActualHeight - 100, 0, 0);
                thick_ani.Duration = TimeSpan.FromSeconds(0.8);
                thick_ani.FillBehavior = FillBehavior.Stop;

                img_animi.BeginAnimation(Image.OpacityProperty, opacity_ani);
                img_animi.BeginAnimation(Image.WidthProperty, width_ani);
                img_animi.BeginAnimation(Image.HeightProperty, height_ani);
                img_animi.BeginAnimation(Image.MarginProperty, thick_ani);
                img_animi.Opacity = 0;
                img_animi.Width = 0;
                img_animi.Height = 0;
                sub_scale anicale = MainWindow.FindChild<sub_scale>(Application.Current.MainWindow, "newscale");
                RenderTargetBitmap repot_image;
                repot_image = new RenderTargetBitmap((int)anicale.report_panel.ActualWidth, (int)anicale.report_panel.ActualHeight + 250, 96, 96, PixelFormats.Default);
                repot_image.Render(anicale.report_panel);
            }

            else if (PublicClass.report_type == "dismapreport")
            {
                DoubleAnimation opacity_ani = new DoubleAnimation();
                opacity_ani.To = 0;
                opacity_ani.Duration = TimeSpan.FromSeconds(0.8);
                opacity_ani.FillBehavior = FillBehavior.Stop;

                DoubleAnimation width_ani = new DoubleAnimation();
                width_ani.To = 150;
                width_ani.Duration = TimeSpan.FromSeconds(0.8);
                width_ani.FillBehavior = FillBehavior.Stop;

                DoubleAnimation height_ani = new DoubleAnimation();
                height_ani.To = 80;
                height_ani.Duration = TimeSpan.FromSeconds(0.8);
                height_ani.FillBehavior = FillBehavior.Stop;

                ThicknessAnimation thick_ani = new ThicknessAnimation();
                thick_ani.To = new Thickness(this.ActualWidth - 280, this.ActualHeight - 100, 0, 0);
                thick_ani.Duration = TimeSpan.FromSeconds(0.8);
                thick_ani.FillBehavior = FillBehavior.Stop;

                img_animi.BeginAnimation(Image.OpacityProperty, opacity_ani);
                img_animi.BeginAnimation(Image.WidthProperty, width_ani);
                img_animi.BeginAnimation(Image.HeightProperty, height_ani);
                img_animi.BeginAnimation(Image.MarginProperty, thick_ani);
                img_animi.Opacity = 0;
                img_animi.Width = 0;
                img_animi.Height = 0;
                sub_dismap newdismap = MainWindow.FindChild<sub_dismap>(Application.Current.MainWindow, "newdismap");
                RenderTargetBitmap repot_image;
                repot_image = new RenderTargetBitmap((int)(newdismap.report_panel.ActualWidth), (int)newdismap.report_panel.ActualHeight, 96, 96, PixelFormats.Default);
                repot_image.Render(newdismap.report_panel); 
            }
        }


        private void about_Click(object sender, RoutedEventArgs e)
        {
            subwindow_content.Children.Clear();
            sub_about newabout = new sub_about();
            subwindow.Width = 450;
            subwindow.Height = 360;
            newabout.AboutMouseUp += new RoutedPropertyChangedEventHandler<object>(newabout_AboutMouseUp);
            newabout.Name = "newabout";
            subwindow.Opacity = 0.9;
            subwindow_content.Children.Add(newabout);

            PublicClass.report_type = "workreport";
            Panel.SetZIndex(submainwindow, 3000);
            PublicClass.open_newisothermal = false;
           // aboutreport.IsEnabled = false;



        }

        void newabout_AboutMouseUp(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Panel.SetZIndex(subwindow, -1);
            subwindow.Opacity = 0;
            subwindow.Width = 0;
            subwindow.Height = 0;
            PublicClass.report_type = "workreport";
        }



        private void sub_about_close_Click(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(subwindow, -1);
            subwindow.Opacity = 0;
            subwindow.Width = 0;
            subwindow.Height = 0;
            PublicClass.report_type = "workreport";
        }

        private void help_Click(object sender, RoutedEventArgs e)
        {
            string dug = System.AppDomain.CurrentDomain.BaseDirectory;
           string path = System.IO.Path.Combine(dug, @"help.pdf");//@"\IrAnalyse\helptext.rtf";
          
            System.Diagnostics.Process.Start(path);
         
        }

        private void minwindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (System.IO.Directory.Exists(System.IO.Path.GetTempPath() + "\\IrAnalyse"))
            {
                System.IO.Directory.Delete(System.IO.Path.GetTempPath() + "\\IrAnalyse",true);
            }
        }

        private void tool_Isothermal_Click(object sender, RoutedEventArgs e)
        {
            subwindow_content.Children.Clear();
            sub_isothermal newisothermal = new sub_isothermal();
            subwindow.Width = 550;
            subwindow.Height = 490;
            newisothermal.Name = "newisothermal";
            subwindow.Opacity = 0.9;
            newisothermal.Iso += new RoutedPropertyChangedEventHandler<object>(newisothermal_Iso);
            subwindow_content.Children.Add(newisothermal);

            PublicClass.report_type = "workreport";
            Panel.SetZIndex(submainwindow, 3000);
            PublicClass.open_newisothermal = true;
        }

        void newisothermal_Iso(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Panel.SetZIndex(submainwindow, -1);
            subwindow.Opacity = 0;
            PublicClass.report_type = "workreport";
            PublicClass.open_newisothermal = false;
        }






        private void image_q_Click(object sender, RoutedEventArgs e)
        {
           
                is_visible = false;
                System.Windows.Forms.FolderBrowserDialog fd = new System.Windows.Forms.FolderBrowserDialog();
                fd.ShowDialog();
                if (fd.SelectedPath != string.Empty)
                {
                    image_w.Text = fd.SelectedPath;
                    img_path = fd.SelectedPath;
                    files = System.IO.Directory.GetFiles(image_w.Text, "*.jpg", SearchOption.AllDirectories);
                    Image_query.Items.Clear();
                    img_file.Clear();
                    img_name.Clear();

                    newtime.Interval = 20;
                    newtime.Start();






                    

                }

        }


        private void pic_temp_para_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
           
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            

            TextBox newtxt = e.EditingElement as TextBox;
            string col_value = (e.EditingElement as TextBox).Text;
           
        
            try
            {
                if (e.Row.GetIndex() >= 0)
                {
                    for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                    {
                        PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                        if (newshapes.workspace_name == PublicClass.cur_ctrl_name)
                        {
                            if (e.Row.GetIndex() == 0)
                            {
                                if (int.Parse(col_value) > 1000)
                                {
                                    col_value = "1000";
                                }
                                else if (int.Parse(col_value) < -273)
                                {
                                    col_value = "-273";
                                }
                                workspace.TempTamb = (UInt16)(float.Parse(col_value) * 10.0);
                                newshapes.TempTamb = (UInt16)(float.Parse(col_value) * 10.0);
                                newtxt.Text = int.Parse(col_value) + " ℃";
                            }
                            if (e.Row.GetIndex() == 1)
                            {
                                if (int.Parse(col_value) > 100)
                                {
                                    col_value = "100";
                                }
                                else if (int.Parse(col_value) < 0)
                                {
                                    col_value = "0";
                                }
                                workspace.dampness = float.Parse(col_value);
                                newshapes.dampness = float.Parse(col_value);
                                newtxt.Text = int.Parse(col_value) + " %";
                            }
                            if (e.Row.GetIndex() == 2)
                            {
                                if (float.Parse(col_value) > 1)
                                {
                                    col_value = "1";
                                }
                                else if (float.Parse(col_value) < 0)
                                {
                                    col_value = "0";
                                }
                                workspace.Emiss = float.Parse(col_value) * 100f;
                                newshapes.Emiss = float.Parse(col_value) * 100f;
                                newtxt.Text = col_value;
                            }
                            if (e.Row.GetIndex() == 3)
                            {
                                if (int.Parse(col_value) <0)
                                {
                                    col_value = "0";
                                }
                               
                                workspace.TempDist = UInt16.Parse(col_value);
                                newshapes.TempDist = UInt16.Parse(col_value);
                                newtxt.Text = int.Parse(col_value) + " m";
                            }
                            if (e.Row.GetIndex() == 4)
                            {
                                if (int.Parse(col_value) > 1000)
                                {
                                    col_value = "1000";
                                }
                                else if (int.Parse(col_value) < -273)
                                {
                                    col_value = "-273";
                                }
                                workspace.modify_temp = int.Parse(col_value) * 10;
                                newshapes.Temprevise = float.Parse(col_value) * 10;
                                newtxt.Text = int.Parse(col_value)  + " ℃";
                            }


                            newshapes.percent = 0;
                            PublicClass.shapes_count[i] = newshapes;
                          
                        }

                    }

                    workspace.all_area_temp();
                    create_ir_information();
                   
                    try
                    {
                       

                        sub_shapes_list shap = MainWindow.FindChild<sub_shapes_list>(Application.Current.MainWindow, "shapestest");
                        shap.shapeslist("all", 0);

                    }
                    catch
                    {

                    }

                }

            }
            catch { }
        }



        private void temp_mark_Click(object sender, RoutedEventArgs e)
        {
            subwindow_content.Children.Clear();
            sub_temp temp = new sub_temp();
            subwindow.Width = 450;
            subwindow.Height = 340;
            temp.TempMouseUp += new RoutedPropertyChangedEventHandler<object>(temp_TempMouseUp);
            temp.Temp1MouseUp += new RoutedPropertyChangedEventHandler<object>(temp_Temp1MouseUp);
            subwindow_content.Children.Add(temp);
            subwindow.Opacity = 0.9;
            Panel.SetZIndex(submainwindow, 3000);
        }

        void temp_Temp1MouseUp(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Panel.SetZIndex(subwindow, -1);
            subwindow.Opacity = 0;
            subwindow.Width = 0;
            subwindow.Height = 0;
            PublicClass.report_type = "workreport";
        }

        void temp_TempMouseUp(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

            Panel.SetZIndex(subwindow, -1);
            subwindow.Opacity = 0;
            subwindow.Width = 0;
            subwindow.Height = 0;
            PublicClass.report_type = "workreport";

        }

        private void pic_temp_para_KeyDown(object sender, KeyEventArgs e)
        {
            Regex re = new Regex("[^0-9.-]+");
         

            if (e.Key == Key.Decimal || e.Key == Key.OemPeriod || e.Key==Key.Subtract || e.Key == Key.OemMinus)
            {
            }
            else
            {
                e.Handled = re.IsMatch(e.Key.ToString().Substring(e.Key.ToString().Length - 1));
            }
           // string asdas = e.Key.ToString();
        }


        private void Image_query_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string img_file1 = "";
            int i = Image_query.Items.IndexOf(Image_query.SelectedItem);
            img_file1 = img_file[i].ToString();
            //    img_file1 = img_file +"\\"+img_file1;

            FileStream fs = new FileStream(img_file1, FileMode.Open,FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            fs.Seek(0x18, SeekOrigin.Begin);//从18h字开始
            string jpg_size_str = len_2(br.ReadByte());
            jpg_size_str += len_2(br.ReadByte());
            jpg_size_str += len_2(br.ReadByte());
            jpg_size_str += len_2(br.ReadByte());
            int jpg_size = Convert.ToInt32(jpg_size_str, 16);//获取JPG 文件长度

            fs.Seek(jpg_size + 90, SeekOrigin.Begin);
          //  bool is_ir = true;

            try
            {
                br.ReadByte();
            }
            catch
            {
              //  is_ir = false;
            }
            br.Close();
            fs.Close();

            sub_workspace newworkspace = new sub_workspace();
            PublicClass.report_type = "workreport";
            PublicClass.ctrl_name++;
            newworkspace.Name = "work" + PublicClass.ctrl_name.ToString();
            newworkspace.LoadIrInformation += new RoutedPropertyChangedEventHandler<object>(newworkspace_LoadIrInformation);
            newworkspace.WorkMouseWheel += new RoutedPropertyChangedEventHandler<object>(newworkspace_WorkMouseWheel);
            newworkspace.WorkMouseUp += new RoutedPropertyChangedEventHandler<object>(newworkspace_WorkMouseUp);
            newworkspace.filename = img_file1;
            newworkspace.cur_file_name = img_name[i].ToString();
            LayoutDocument newaaa = new LayoutDocument();
            newaaa.Content = newworkspace;
            newaaa.Title = img_name[i].ToString();
            newaaa.IsSelected = true;
            newaaa.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(newaaa_Closing);
            mainpanel.Children.Add(newaaa);
        }

        private void close_all_Click(object sender, RoutedEventArgs e)
        {
            mainpanel.Children.Clear();
            PublicClass.ctrl_name = 0;
            PublicClass.shapes_count.Clear();
            try
            {
                sub_shapes_list shap = MainWindow.FindChild<sub_shapes_list>(Application.Current.MainWindow, "shapestest");
                shap.shapeslist("all", 0);
            }
            catch { }
            //pic_information.ItemsSource = null;
            //pic_temp_para.ItemsSource = null;
            //pic_temp_tag.ItemsSource = null;
            newimage_LoadImgInformation(null, null);
        }

        private void delete_report_Click(object sender, RoutedEventArgs e)
        {
            repot_ready.Items.Clear();
            PublicClass.report_name_step = 0;
            PublicClass.repot_ready_index = 0;
        }

        private void tool_gamut_Click(object sender, RoutedEventArgs e)
        {
            subwindow_content.Children.Clear();
            sub_gamut newgamut = new sub_gamut();
            subwindow.Width = 500;
            subwindow.Height = 500;

            subwindow_content.Children.Add(newgamut);
            subwindow.Opacity = 0.9;
            Panel.SetZIndex(submainwindow, 3000);
        }

 





 



       




    }
}
