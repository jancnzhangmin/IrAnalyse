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
using System.Data;
using System.Text.RegularExpressions;

namespace IrAnalyse
{
    /// <summary>
    /// sub_shapes_list.xaml 的交互逻辑
    /// </summary>
    public partial class sub_shapes_list : UserControl
    {
        public sub_shapes_list()
        {
            InitializeComponent();
            shapeslist("all", 0);
        }

        
        public ArrayList test { get; set; }
        
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            shapeslist("insert", 0);
        }

        public void shapeslist(string operating_mode,int shapes_count_index)//加载图片本来信息
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name); 
            ArrayList test = new ArrayList();
            List<Data> list = new List<Data>();
            ArrayList test1 = new ArrayList();
            if (operating_mode == "insert")
            {
               
                test1 = PublicClass.shapes_count;
                PublicClass.shapes_property test2 = new PublicClass.shapes_property();
               
                    for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                    {
                        test2 = (PublicClass.shapes_property)test1[i];
                        if (test2.workspace_name == PublicClass.cur_ctrl_name && test2.shapes_type != "spot" && test2.shapes_type != "area" && test2.shapes_type != "max_temp_temp" && test2.shapes_type != "max_temp" && test2.shapes_type != "min_temp_temp" && test2.shapes_type != "min_temp")
                        {
                                list.Add(
                                   new Data()
                                   {
                                       shapes_name = test2.shapes_name,
                                       shapes_temp_max = Math.Round((float)test2.max_temp / 10,1) + " ℃",
                                       shapes_temp_min = Math.Round((float)test2.min_temp / 10,1) + " ℃",
                                       shapes_temp_avr =  Math.Round((float)test2.avr_temp / 10,1) + " ℃",
                                       shapes_emiss = ((float)(test2.Emiss / 100.0)).ToString(),
                                       shapes_distance = test2.TempDist + "m",
                                       shapes_tamb = test2.TempTamb / 10.0 + " ℃",
                                       shapes_relhum = test2.dampness + " %",
                                       shapes_temp_cor = test2.Temprevise / 10.0 + " ℃"
                                   });
                            }
                      
                    }
                    this.dataGrid.ItemsSource = list;
                    dataGrid.CanUserAddRows = false;
                    dataGrid.IsReadOnly = false;
                    dataGrid.Columns[0].IsReadOnly = true;
                    dataGrid.Columns[1].IsReadOnly = true; 
                    dataGrid.Columns[2].IsReadOnly = true;
                    dataGrid.Columns[3].IsReadOnly = true;
                
                    if (dataGrid.Items.Count > 1)
                    {
                        for (int i = 0; i < dataGrid.Items.Count; i++)
                        {
                            Data t = (Data)dataGrid.Items[i];
                            if (t.shapes_name == workspace.shapes_active)
                            {
                                dataGrid.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                    dataGrid.CellEditEnding += new EventHandler<DataGridCellEditEndingEventArgs>(dataGrid_CellEditEnding);
                    dataGrid.KeyDown += new KeyEventHandler(dataGrid_KeyDown);
             
            }


            if (operating_mode == "all")
            {

                test1 = PublicClass.shapes_count;
                PublicClass.shapes_property test2 = new PublicClass.shapes_property();
                list.Clear();
                for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                {
                    test2 = (PublicClass.shapes_property)test1[i];
                    if (test2.workspace_name == PublicClass.cur_ctrl_name && test2.shapes_type != "spot" && test2.shapes_type != "area" && test2.shapes_type != "max_temp_temp" && test2.shapes_type != "max_temp" && test2.shapes_type != "min_temp_temp" && test2.shapes_type != "min_temp")
                    {
                        list.Add(
                           new Data()
                           {
                               shapes_name = test2.shapes_name,
                               shapes_temp_max = Math.Round((float)test2.max_temp / 10, 1) + " ℃",
                               shapes_temp_min = Math.Round((float)test2.min_temp / 10, 1) + " ℃",
                               shapes_temp_avr = Math.Round((float)test2.avr_temp / 10, 1) + " ℃",
                               shapes_emiss = ((float)(test2.Emiss / 100.0)).ToString(), 
                               shapes_distance = test2.TempDist + "m",
                               shapes_tamb = test2.TempTamb/10.0 + " ℃",
                               shapes_relhum = test2.dampness + " %",
                               shapes_temp_cor = test2.Temprevise/10.0+" ℃"
                           });
                    }

                }
                dataGrid.ItemsSource = null;
                this.dataGrid.ItemsSource = list;
                dataGrid.CanUserAddRows = false;
               
                //dataGrid.IsReadOnly = false;
                //dataGrid.Columns[0].IsReadOnly = true;
                //dataGrid.Columns[1].IsReadOnly = true;
                //dataGrid.Columns[2].IsReadOnly = true;
                //dataGrid.Columns[3].IsReadOnly = true;
               

                if (dataGrid.Items.Count > 1)
                {
                    for (int i = 0; i < dataGrid.Items.Count; i++)
                    {
                        Data t = (Data)dataGrid.Items[i];
                        if (t.shapes_name == workspace.shapes_active)
                        {
                            dataGrid.SelectedIndex = i;
                            break;
                        }
                    }
                }
                //dataGrid.CellEditEnding += new EventHandler<DataGridCellEditEndingEventArgs>(dataGrid_CellEditEnding);
            }
           
      }

        void dataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            Regex re = new Regex("[^0-9.-]+");

            if (e.Key == Key.Decimal || e.Key == Key.OemPeriod || e.Key == Key.Subtract || e.Key == Key.OemMinus)
            {
            }
            else
            {
                e.Handled = re.IsMatch(e.Key.ToString().Substring(e.Key.ToString().Length - 1));
            }
            
            //DataRowView item = dataGrid.CurrentCell.Item as DataRowView;
            //if (item != null)
            //{
                
            //    string dsdfs = item[dataGrid.CurrentCell.Column.DisplayIndex].ToString();
            //}
          //  DataView aaaas = (DataView)dataGrid.CurrentCell.Item;
            
            ;
            //e.Handled = true;


        
        }

        void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
           
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
           // TextBox aa  =(e.EditingElement as TextBox);
            string col_value = (e.EditingElement as TextBox).Text;
            string new_col_value = "";
            int dian = 0;
            int fu=0;
            
            for (int i = 0; i < col_value.Length; i++)
            {
                int newchar = (int)Convert.ToChar(col_value.Substring(i, 1));
                if (newchar == 45 || newchar == 46 || (newchar > 47 && newchar < 58))
                {

                    if (newchar == 45 && i == 0)
                    {
                        new_col_value += col_value.Substring(i, 1);
                        fu++;
                    }
                    else if (newchar == 46 && i > 0 && dian<2)
                    {
                        new_col_value += col_value.Substring(i, 1);
                        dian++;
                        //break;
                    }
                    else if(fu<2)
                    {
                        new_col_value += col_value.Substring(i, 1);
                    }
                    if (dian > 1 || fu > 1)
                    {
                        break;
                    }
            
                }
                
            }
            try
            {
                col_value = new_col_value;
            }
            catch { }
                //  Regex re = new Regex("[^0-9.-]+");

                //e.Handled = re.IsMatch(e.Text);

                // leng = col_value.IndexOf("℃");
                //if (leng > -1)
                //{
                //    leng = col_value.IndexOf("℃");

                //    col_value = col_value.Substring(0, leng);
                //}
                //else
                //{
                //    leng = col_value.IndexOf("m");
                //    if (leng > -1)
                //    {
                //        leng = col_value.IndexOf("m");
                //    }
                //    else
                //    {
                //        leng = col_value.IndexOf("%");
                //    }
                //    col_value = col_value.Substring(0, leng);
                //}

            try
            {
                if (e.Row.GetIndex() >= 0)
                {
                    int v = e.Column.DisplayIndex;
                    Data newrow = dataGrid.Items[e.Row.GetIndex()] as Data;
                    for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                    {
                        PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                        {
                            if (newrow.shapes_name == newshapes.shapes_name && newshapes.workspace_name == PublicClass.cur_ctrl_name)
                            {
                                if (e.Column.DisplayIndex == 4)
                                {
                                    if (float.Parse(col_value) > 1)
                                    {
                                        col_value = "1";
                                    }
                                    else if (float.Parse(col_value) < 0)
                                    {
                                        col_value = "0";
                                    }
                                    newshapes.Emiss = float.Parse(col_value) * 100f;
                                    //workspace.ir_information[16] = float.Parse(col_value);
                                    //workspace.Emiss = float.Parse(col_value) * 100f;
                                }
                                else if (e.Column.DisplayIndex == 5)
                                {
                                    if (float.Parse(col_value) < 0)
                                    {
                                        col_value = "0";
                                    }
                                    newshapes.TempDist = float.Parse(col_value);
                                    //workspace.TempDist = UInt16.Parse(col_value);
                                }
                                else if (e.Column.DisplayIndex == 6)
                                {
                                    if (int.Parse(col_value) > 1000)
                                    {
                                        col_value = "1000";
                                    }
                                    else if (int.Parse(col_value) < -273)
                                    {
                                        col_value = "-273";
                                    }
                                    newshapes.TempTamb = float.Parse(col_value) * 10;
                                    //workspace.TempTamb = UInt16.Parse((float.Parse(col_value) * 10.0).ToString());
                                }
                                else if (e.Column.DisplayIndex == 7)
                                {
                                    if (int.Parse(col_value) > 100)
                                    {
                                        col_value = "100";
                                    }
                                    else if (int.Parse(col_value) < 0)
                                    {
                                        col_value = "0";
                                    }
                                    newshapes.dampness = float.Parse(col_value);

                                    //workspace.ir_information[11] = float.Parse(col_value);
                                    //workspace.dampness = float.Parse(col_value);
                                   // newshapes.dampness = float.Parse(col_value);

                                }
                                else if (e.Column.DisplayIndex == 8)
                                {
                                    if (int.Parse(col_value) > 1000)
                                    {
                                        col_value = "1000";
                                    }
                                    else if (int.Parse(col_value) < -273)
                                    {
                                        col_value = "-273";
                                    }
                                    newshapes.Temprevise = float.Parse(col_value) * 10;

                                }
                                newshapes.percent = 0;
                                PublicClass.shapes_count[i] = newshapes;
                                workspace.area_temp();
                                shapeslist("all", 0);
                                
                

                            }
                        }
                    }
                }
            }
            catch { }

                //newrow.shapes_emiss = aa;
                //workspace.TempDist



             //   workspace.TiEmiss = float.Parse(aa);
            //workspace.ir_information[16] = workspace.TiEmiss*100;
            //workspace.create_img();
            //workspace.rad_table();

            //workspace.create_img();
            //try
            //{
            //    sub_shapes_list shapes_list = MainWindow.FindChild<sub_shapes_list>(Application.Current.MainWindow, "shapestest");
            //    shapes_list.shapeslist("insert", 0);
            //}
            //catch { }
            //RoutedPropertyChangedEventArgs<object> args = new RoutedPropertyChangedEventArgs<object>(this, e);
            //args.RoutedEvent = sub_radtable.EmsUpEvent;
            //this.RaiseEvent(args);
          
        }




       

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name); 
                Data mySelectedElement = (Data)dataGrid.SelectedItem;
                int datagrid_index = dataGrid.SelectedIndex;
            
                if (mySelectedElement != null)
                {
                    string result = mySelectedElement.shapes_name;
                    workspace.shapes_active = result;
                    
                    for (int i = 0; i < PublicClass.shapes_count.Count; i++)
                    {
                        PublicClass.shapes_property newshapes = (PublicClass.shapes_property)PublicClass.shapes_count[i];
                        if (newshapes.shapes_name == result)
                        {
                            workspace.public_shapes_index = i;
                            break;
                        }
                    }
                    workspace.re_create_shapes();
                }
               
        }
        public void change()
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name); 
            if (workspace.ir_information[16].ToString() != dataGrid.CurrentCell.ToString())
            {
                workspace.TiEmiss = float.Parse(dataGrid.CurrentCell.ToString());
            }
            workspace.rad_table();
           
        }

     
    }
}
