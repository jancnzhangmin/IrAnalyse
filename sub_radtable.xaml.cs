using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Data;

namespace IrAnalyse
{
    /// <summary>
    /// sub_radtable.xaml 的交互逻辑
    /// </summary>
    public partial class sub_radtable : UserControl
    {
        public sub_radtable()
        {
            InitializeComponent();
            add_radtable();
            this.rad_table.ItemsSource = Ems_list;
            rad_table.IsReadOnly = true;
            rad_table.CanUserAddRows = false;
            
            
        }
        public static readonly RoutedEvent EmsUpEvent = EventManager.RegisterRoutedEvent("EmsUp", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(sub_radtable));
        public event RoutedPropertyChangedEventHandler<object> EmsUp
        {
            add { AddHandler(EmsUpEvent, value); }
            remove { RemoveHandler(EmsUpEvent, value); }
        }
        public static List<Ems_Item> Ems_list = new List<Ems_Item>();
        public class Ems_Item
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Temperature1 { get; set; }
            public string Temperature2 { get; set; }
            public float Ems1 { get; set; }
            public float Ems2 { get; set; }
            public float DefaultEms { get; set; }
        }

        private void radtable(string ems_name,string ems_type,string ems_temp1,string ems_temp2,float ems1,float ems2,float default_ems)
        {
            
            Ems_Item newems = new Ems_Item();
            newems.Name = ems_name;
            newems.Type = ems_type;
            newems.Temperature1 = ems_temp1;
            newems.Temperature2=ems_temp2;
            newems.Ems1 = ems1;
            newems.Ems2 = ems2;
            newems.DefaultEms = default_ems;
            Ems_list.Add(newems);
           

        }

        private void add_radtable()//辐射率表数据
        {
            Ems_list.Clear();
            radtable("锌（400℃氧化）", "金属", "400", "400", 0.01f, 0.01f, 0.01f);
            radtable("镀锌亮铁板", "金属", "28", "28", 0.23f, 0.23f, 0.23f);
            radtable("灰氧化锌", "金属", "25", "25", 0.28f, 0.28f, 0.28f);
            radtable("砖", "非金属", "1100", "1100", 0.75f, 0.75f, 0.75f);
            radtable("耐火砖", "非金属", "1100", "1100", 0.75f, 0.75f, 0.75f);
            radtable("石墨（灯黑）", "非金属", "96", "225", 0.95f, 0.95f, 0.95f);
            radtable("搪瓷（白色）", "非金属", "18", "18", 0.9f, 0.9f, 0.9f);
            radtable("沥青", "非金属", "0", "200", 0.85f, 0.85f, 0.85f);
            radtable("玻璃（面）", "非金属", "23", "23", 0.94f, 0.94f, 0.94f);
            radtable("耐热玻璃", "非金属", "200", "540", 0.85f, 0.95f, 0.9f);
            radtable("墙粉", "非金属", "20", "20", 0.9f, 0.9f, 0.9f);
            radtable("橡木", "非金属", "20", "20", 0.9f, 0.9f, 0.9f);
            radtable("碳片", "非金属", "", "", 0.85f, 0.85f, 0.85f);
            radtable("绝缘片", "非金属", "", "", 0.91f, 0.94f, 0.93f);
            radtable("金属片", "非金属", "", "", 0.88f, 0.9f, 0.89f);
            radtable("玻璃管", "非金属", "", "", 0.9f, 0.9f, 0.9f);
            radtable("线圈型", "非金属", "", "", 0.9f, 0.9f, 0.9f);
            radtable("实心材料", "非金属", "", "", 0.8f, 0.93f, 0.86f);
            radtable("旋转式电容器", "非金属", "", "", 0.3f, 0.34f, 0.32f);
            radtable("化学式电容器", "非金属", "", "", 0.25f, 0.36f, 0.31f);
            radtable("电容器陶瓷（盘型）", "非金属", "", "", 0.9f, 0.94f, 0.92f);
            radtable("陶瓷（瓶型）", "非金属", "", "", 0.9f, 0.9f, 0.9f);
            radtable("胶片", "非金属", "", "", 0.9f, 0.93f, 0.92f);
            radtable("云母", "非金属", "", "", 0.94f, 0.95f, 0.94f);
            radtable("液槽式云母", "非金属", "", "", 0.9f, 0.93f, 0.91f);
            radtable("玻璃", "非金属", "", "", 0.91f, 0.92f, 0.91f);
            radtable("晶体管（塑封）", "非金属", "", "", 0.3f, 0.4f, 0.35f);
            radtable("二极管", "非金属", "", "", 0.89f, 0.9f, 0.89f);
            radtable("脉冲传输", "非金属", "", "", 0.91f, 0.92f, 0.91f);
            radtable("平的白垩层", "非金属", "", "", 0.88f, 0.93f, 0.9f);
            radtable("顶圈", "非金属", "", "", 0.91f, 0.92f, 0.91f);
            radtable("环氧玻璃板", "非金属", "", "", 0.86f, 0.86f, 0.86f);
            radtable("环氧酚板", "非金属", "", "", 0.8f, 0.8f, 0.8f);
            radtable("镀金铜片", "非金属", "", "", 0.3f, 0.3f, 0.3f);
            radtable("涂焊料的铜", "非金属", "", "", 0.35f, 0.35f, 0.35f);
            radtable("涂锡铅线", "非金属", "", "", 0.28f, 0.28f, 0.28f);
            radtable("铜丝", "金属", "", "", 0.87f, 0.88f, 0.87f);
            radtable("抛光铝", "金属", "100", "100", 0.09f, 0.09f, 0.09f);
            radtable("商用铝箔", "金属", "100", "100", 0.09f, 0.09f, 0.09f);
            radtable("电解渡铬氧化铝", "金属", "", "", 0.55f, 0.55f, 0.55f);
            radtable("轻度氧化铝", "金属", "25", "600", 0.1f, 0.2f, 0.15f);
            radtable("强氧化铝", "金属", "25", "600", 0.3f, 0.4f, 0.35f);
            radtable("黄铜镜面（高度抛光）", "金属", "28", "28", 0.03f, 0.03f, 0.03f);
            radtable("氧化黄铜", "金属", "200", "600", 0.59f, 0.61f, 0.6f);
            radtable("抛光铬", "金属", "40", "1090", 0.08f, 0.36f, 0.19f);
            radtable("铜镜面", "金属", "100", "100", 0.05f, 0.05f, 0.5f);
            radtable("铜水", "金属", "1080", "1280", 0.13f, 0.16f, 0.14f);
            radtable("金镜面", "金属", "230", "630", 0.02f, 0.02f, 0.02f);
            radtable("抛光铸铁", "金属", "200", "200", 0.21f, 0.21f, 0.21f);
            radtable("加工铸铁", "金属", "20", "20", 0.44f, 0.44f, 0.44f);
            radtable("抛光回火铁", "金属", "40", "250", 0.28f, 0.28f, 0.28f);
            radtable("抛光钢锭", "金属", "770", "1040", 0.52f, 0.56f, 0.54f);
            radtable("毛焊接钢", "金属", "945", "1100", 0.52f, 0.61f, 0.56f);
            radtable("完全生锈的表面", "金属", "20", "20", 0.69f, 0.69f, 0.69f);
            radtable("扎铁板", "金属", "22", "22", 0.66f, 0.66f, 0.66f);
            radtable("氧化钢", "金属", "100", "100", 0.74f, 0.74f, 0.74f);
            radtable("铸铁（在600℃氧化）", "金属", "198", "600", 0.64f, 0.78f, 0.71f);
            radtable("钢（在600℃氧化）", "金属", "198", "600", 0.79f, 0.79f, 0.79f);
            radtable("电解氧化铁", "金属", "125", "520", 0.78f, 0.82f, 0.8f);
            radtable("氧化铁", "金属", "500", "1200", 0.85f, 0.89f, 0.87f);
            radtable("铁板", "金属", "925", "1120", 0.87f, 0.95f, 0.91f);
            radtable("铸铁，重氧化铁", "金属", "25", "25", 0.8f, 0.8f, 0.8f);
            radtable("回火铁，氧化铁", "金属", "40", "250", 0.95f, 0.95f, 0.95f);
            radtable("融化表面", "金属", "22", "22", 0.94f, 0.94f, 0.94f);
            radtable("融化的铸铁", "金属", "1300", "1400", 0.29f, 0.29f, 0.29f);
            radtable("钢水", "金属", "1600", "1800", 0.28f, 0.28f, 0.28f);
            radtable("纯铁水", "金属", "1500", "1650", 0.42f, 0.53f, 0.75f);
            radtable("纯铅（非氧化）", "金属", "1515", "1680", 0.42f, 0.45f, 0.43f);
            radtable("轻度氧化铅", "金属", "125", "225", 0.06f, 0.08f, 0.07f);
            radtable("氢氧化镁", "金属", "275", "825", 0.2f, 0.55f, 0.37f);
            radtable("氧化镁", "金属", "900", "1670", 0.2f, 0.2f, 0.2f);
            radtable("汞", "金属", "0", "100", 0.09f, 0.12f, 0.11f);
            radtable("电镀抛光", "金属", "25", "25", 0.05f, 0.05f, 0.05f);
            radtable("电镀不抛光", "金属", "20", "20", 0.01f, 0.01f, 0.01f);
            radtable("镍丝", "金属", "185", "1010", 0.09f, 0.19f, 0.15f);
            radtable("镍板（氧化的）", "金属", "198", "600", 0.37f, 0.48f, 0.42f);
            radtable("氧化镍", "金属", "650", "1255", 0.59f, 0.86f, 0.73f);
            radtable("镍铬（耐热）合金线（亮）", "金属", "50", "1000", 0.65f, 0.79f, 0.72f);
            radtable("镍铬合金", "金属", "50", "1040", 0.64f, 0.76f, 0.71f);
            radtable("镍银合金", "金属", "50", "500", 0.95f, 0.98f, 0.97f);
            radtable("抛光银", "金属", "100", "100", 0.05f, 0.05f, 0.05f);
            radtable("不锈钢 304（8Cr,18Ni）", "金属", "215", "490", 0.36f, 0.44f, 0.4f);
            radtable("不锈钢 310（25Cr,20Ni）", "金属", "215", "520", 0.9f, 0.97f, 0.94f);
            radtable("商用锡板", "金属", "100", "100", 0.07f, 0.07f, 0.07f);
            radtable("强氧化", "金属", "0", "200", 0.6f, 0.6f, 0.6f);


            
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void rab_ems_Click(object sender, RoutedEventArgs e)
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
           
            if (rad_table.SelectedCells.Count > 0 && workspace!=null)
            {
                Ems_Item item = rad_table.SelectedCells[0].Item as Ems_Item;
                float ems = 1;

                switch (rad_table.SelectedCells[0].Column.DisplayIndex)
                {
                    case 0:
                        ems = item.DefaultEms;
                        break;
                    case 1:
                        ems = item.DefaultEms;
                        break;
                    case 2:
                        ems = item.DefaultEms;
                        break;
                    case 3:
                        ems = item.DefaultEms;
                        break;
                    case 4:
                        ems = item.Ems1;
                        break;
                    case 5:
                        ems = item.Ems2;
                        break;
                    case 6:
                        ems = item.DefaultEms;
                        break;
                }
                workspace.Emiss = ems*100;
                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                    if (newshapes.workspace_name == PublicClass.cur_ctrl_name)
                    {
                        newshapes.Emiss = ems * 100;
                        PublicClass.shapes_count[i] = newshapes;
                    }
                }
                  
                    workspace.rad_table();
                //workspace.all_area_temp();
                
                //workspace.grad_max_min();
                
                //try
                //{
                //    sub_shapes_list shapes_list = MainWindow.FindChild<sub_shapes_list>(Application.Current.MainWindow, "shapestest");
                //    shapes_list.shapeslist("insert", 0);
                //}
                //catch { }
                RoutedPropertyChangedEventArgs<object> args = new RoutedPropertyChangedEventArgs<object>(this, e);
                args.RoutedEvent = sub_radtable.EmsUpEvent;
                this.RaiseEvent(args);
            }



        }
                
    }



}
