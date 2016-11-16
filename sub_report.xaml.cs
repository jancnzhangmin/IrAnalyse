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
using System.IO;
using Microsoft.Win32;
using Xceed.Wpf.AvalonDock.Layout;
using PlantVisual;
using System.Windows.Media.Animation;
using System.Windows.Markup;
using C1.WPF.SpellChecker;
using C1.WPF.RichTextBox.Documents;
using C1.WPF.RichTextBox;


namespace IrAnalyse
{
    /// <summary>
    /// sub_report.xaml 的交互逻辑
    /// </summary>
    public partial class sub_report : UserControl
    {
        public sub_report()
        {
            InitializeComponent();
            richTB.AddHandler(RichTextBox.DragOverEvent, new DragEventHandler(richTB_DragOver), true);
            richTB.AddHandler(RichTextBox.DropEvent, new DragEventHandler(richTB_Drop), true);  

        }

        public static readonly RoutedEvent LoadReportInformationEvent = EventManager.RegisterRoutedEvent("LoadReportInformation", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(sub_report));
        public event RoutedPropertyChangedEventHandler<object> LoadReportInformation
        {
            add { AddHandler(LoadReportInformationEvent, value); }
            remove { RemoveHandler(LoadReportInformationEvent, value); }
        }


        public string savereporturl = "";//保存路径
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            RoutedPropertyChangedEventArgs<object> args = new RoutedPropertyChangedEventArgs<object>(this, e);
            args.RoutedEvent = sub_report.LoadReportInformationEvent;
            this.RaiseEvent(args);
        }

        private void richTB_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
            e.Handled = false;
        }

        //private void richTextBox1_Drop(object sender, DragEventArgs e)//报告图片数据分析
        //{
        //    sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name); 
        //    IDataObject data = new DataObject();
        //    data = e.Data;
        //    if (data.GetDataPresent(typeof(sub_repot_ready)))
        //    {
        //        sub_repot_ready newread = new sub_repot_ready();
        //        newread = data.GetData(typeof(sub_repot_ready)) as sub_repot_ready;
        //        Image rich_img = new Image();
        //        rich_img.Source = newread.repot_image.Source;
        //        //if (newread.repot_image.Source.Width > 800 && newread.repot_image.Source.Height > 400)
        //        //{
        //        //    rich_img.Width = newread.repot_image.Source.Width * 0.83;
        //        //    rich_img.Height = newread.repot_image.Source.Height * 0.5;
        //        //    rich_img.Stretch = Stretch.UniformToFill;
        //        //}
        //        //else if (newread.repot_image.Source.Height < 400)
        //        //{
        //        //    rich_img.Width = newread.repot_image.Source.Width * 0.83;
        //        //    rich_img.Stretch = Stretch.UniformToFill;
        //        //}
        //        //else
        //        //{
        //        // rich_img.Stretch = Stretch.None;
        //        //}
        //        //RichTextBox riche = (Ric)richTB;
        //        //new InlineUIContainer(rich_img, richTB.Selection.Start); 
        //        richTB.Document.Blocks.Add(rich_img);
        //        //if(newread.sub_repot.repot_ready_shapes.Count>0)
        //        //{
        //        //    for (int i = 0; i < newread.sub_repot.repot_ready_shapes.Count; i++)
        //        //    {
        //        //        Paragraph para = new Paragraph();
        //        //        PublicClass.shapes_property newshapes = (PublicClass.shapes_property)newread.sub_repot.repot_ready_shapes[i];
        //        //        para.Inlines.Add(newshapes.shapes_name + "{ 最高温度: " + Math.Round((double)newshapes.max_temp / 10, 1) + "℃  " + "最低温度: " + Math.Round((double)newshapes.min_temp / 10, 1) + "℃  " + "平均温度: " + Math.Round((double)newshapes.avr_temp / 10, 1) + "℃  " + "}");
        //        //        //richTextBox1.Document.Blocks.Clear();
        //        //        //richTextBox1.Selection.Text = "";
        //        //        //var pt = richTextBox1.Selection.Start.InsertParagraphBreak();
        //        //        //richTextBox1.Document.Blocks.InsertAfter(pt.Paragraph, para);
        //        //        richTB.Document.Blocks.Add(para);
        //        //    }
        //        //}
        //    }

        //}
        
        private void richTB_Drop(object sender, DragEventArgs e)
        {
            sub_workspace workspace = MainWindow.FindChild<sub_workspace>(Application.Current.MainWindow, PublicClass.cur_ctrl_name);
            IDataObject data = new DataObject();
            data = e.Data;
            sub_repot_ready newread = new sub_repot_ready();
            newread = data.GetData(typeof(sub_repot_ready)) as sub_repot_ready;
            //string Myhtml = @"<html><head><style type='text/css'>.c0 { font-family: 'Verdana'; font-size: 18.6666666666667px } .c1 { margin: 0px 0px 12px } .c2 { font-family: 'Arial'; font-size: 13.3333333333333px } .c3 { margin: 0px 0px 10px } .c4 { border-collapse: collapse; width: 100% } .c5 { width: 33% } .c6 { border-color: Black; border-style: solid; border-width: thin; padding: 0px 7px } .c7 { margin: 0px } .c8 { font-family: 'Arial'; font-size: 13.33px } </style></head><body class='c0'><p class='c1'><span class='c2'></span></p><table class='c4'><col class='c5'/><col class='c5'/><col class='c5'/><tr><td class='c6'><p class='c7'>图像信息</p></td><td class='c6'><p class='c7'>值</p></td></tr>";
            //string Myhtmlstr = "</table><p class='c1'></p></body></html>";
            //string Myhtmlst = "";
            //string Myhtmlstring = "";
            if (data.GetDataPresent(typeof(sub_repot_ready)))
            {
                if (newread.sub_repot.repot_ready_shapes.Count > 0)
                {
                    C1Table ctable = new C1Table();
                    C1TableRowGroup ctabrowgroup = new C1TableRowGroup();
                    C1TableRow[] ctablerow = new C1TableRow[newread.sub_repot.repot_ready_shapes.Count + 1];
                    C1TableCell[] leftcell = new C1TableCell[newread.sub_repot.repot_ready_shapes.Count + 1];
                    C1TableCell[] rightcell = new C1TableCell[newread.sub_repot.repot_ready_shapes.Count + 1];
                    C1Paragraph[] leftcparagraph = new C1Paragraph[newread.sub_repot.repot_ready_shapes.Count + 1];
                    C1Paragraph[] rightcparagraph = new C1Paragraph[newread.sub_repot.repot_ready_shapes.Count + 1];
                    C1Run[] leftcrun = new C1Run[newread.sub_repot.repot_ready_shapes.Count + 1];
                    C1Run[] rightcrun = new C1Run[newread.sub_repot.repot_ready_shapes.Count + 1];




                    leftcrun[0] = new C1Run();
                    leftcrun[0].Text = "项目";
                    leftcparagraph[0] = new C1Paragraph();
                    leftcparagraph[0].Children.Add(leftcrun[0]);
                    rightcrun[0] = new C1Run();
                    rightcrun[0].Text = "值";
                    rightcparagraph[0] = new C1Paragraph();
                    rightcparagraph[0].Children.Add(rightcrun[0]);
                    leftcell[0] = new C1TableCell();
                    leftcell[0].Width = new C1Length(200);
                    leftcell[0].BorderBrush = Brushes.Black;
                    leftcell[0].BorderThickness = new Thickness(1);
                    rightcell[0] = new C1TableCell();
                    rightcell[0].Width = new C1Length(200);
                    rightcell[0].BorderBrush = Brushes.Black;
                    rightcell[0].BorderThickness = new Thickness(1);
                    leftcell[0].Children.Add(leftcparagraph[0]);
                    rightcell[0].Children.Add(rightcparagraph[0]);
                    ctablerow[0] = new C1TableRow();
                    ctablerow[0].Children.Add(leftcell[0]);
                    ctablerow[0].Children.Add(rightcell[0]);
                    ctabrowgroup.Children.Add(ctablerow[0]);



                    for (int i = 0; i < newread.sub_repot.repot_ready_shapes.Count; i++)
                    {
                        PublicClass.shapes_property newshapes = (PublicClass.shapes_property)newread.sub_repot.repot_ready_shapes[i];

                        leftcrun[i + 1] = new C1Run();
                        leftcrun[i + 1].Text = newshapes.shapes_name + "最高温";
                        leftcparagraph[i + 1] = new C1Paragraph();
                        leftcparagraph[i + 1].Children.Add(leftcrun[i + 1]);
                        rightcrun[i + 1] = new C1Run();
                        rightcrun[i + 1].Text = Math.Round((float)newshapes.max_temp / 10, 1) + " ℃";
                        rightcparagraph[i + 1] = new C1Paragraph();
                        rightcparagraph[i + 1].Children.Add(rightcrun[i + 1]);
                        leftcell[i + 1] = new C1TableCell();
                        leftcell[i + 1].BorderBrush = Brushes.Black;
                        leftcell[i + 1].BorderThickness = new Thickness(1);
                        rightcell[i + 1] = new C1TableCell();
                        rightcell[i + 1].BorderBrush = Brushes.Black;
                        rightcell[i + 1].BorderThickness = new Thickness(1);
                        leftcell[i + 1].Children.Add(leftcparagraph[i + 1]);
                        rightcell[i + 1].Children.Add(rightcparagraph[i + 1]);
                        ctablerow[i + 1] = new C1TableRow();
                        ctablerow[i + 1].Children.Add(leftcell[i + 1]);
                        ctablerow[i + 1].Children.Add(rightcell[i + 1]);
                        ctabrowgroup.Children.Add(ctablerow[i + 1]);


                        //if (i == 0)
                        //{
                        //    Myhtmlst = "<tr><td class='c6'><p class='c7'>辐射率</p></td><td class='c6'><p class='c7'>" + newread.sub_repot.repot_ready_emss + "</p></td></tr>";
                        //    Myhtmlst += "<tr><td class='c6'><p class='c7'>距离</p></td><td class='c6'><p class='c7'>" + newread.sub_repot.repot_ready_distance + "</p></td></tr>";
                        //    Myhtmlst += "<tr><td class='c6'><p class='c7'>环境温度</p></td><td class='c6'><p class='c7'>" + newread.sub_repot.repot_ready_tamb + "</p></td></tr>";
                        //    Myhtmlst += "<tr><td class='c6'><p class='c7'>最高温</p></td><td class='c6'><p class='c7'>" + Math.Round((float)newshapes.max_temp / 10, 1) + " ℃" + "</p></td></tr>";
                        //    Myhtmlst += "<tr><td class='c6'><p class='c7'>最低温</p></td><td class='c6'><p class='c7'>" + Math.Round((float)newshapes.min_temp / 10, 1) + " ℃" + "</p></td></tr>";
                        //}

                        //Myhtmlst += "<tr><td class='c6'><p class='c7'>" + newshapes.shapes_name + "最高温" + "</p></td><td class='c6'><p class='c7'>" + Math.Round((float)newshapes.max_temp / 10, 1) + " ℃" + "</p></td></tr>";
                    }

                    ctable.Children.Add(ctabrowgroup);
                    ctable.BorderCollapse = true;
                    C1Run startcr = new C1Run();
                    startcr.Text = "";
                    C1Run endcr = new C1Run();
                    endcr.Text = "";
                    C1Paragraph newpara = new C1Paragraph();
                    
                    newpara.Children.Add(startcr);
                    newpara.Children.Add(ctable);
                    newpara.Children.Add(endcr);
                    //richTB.Selection.Start.InsertInline(ctable);
                    richTB.Document.Blocks.Add(newpara);
                    
                    //C1para.Children.Add(ctable);
                   // richTB.Document.Blocks.Add(cr);
                    
                    
                    //richTB.Selection.Start.InsertHardLineBreak();
                    
                }
            }
            //Myhtmlstring = Myhtml + Myhtmlst + Myhtmlstr;
            ////richTB.HtmlFilter.ConvertToDocument(Myhtmlstring);
            ////C1Document newd = new C1Document();
            //richTB.Document = richTB.HtmlFilter.ConvertToDocument(Myhtmlstring); 

       



//            string myHTMLString =
//@"<html><head><style type='text/css'>.c0 { font-family: 'Verdana'; font-size: 18.6666666666667px } .c1 { margin: 0px 0px 12px } .c2 { font-family: 'Arial'; font-size: 13.3333333333333px } .c3 { margin: 0px 0px 10px } .c4 { border-collapse: collapse; width: 100% } .c5 { width: 33% } .c6 { border-color: Black; border-style: solid; border-width: thin; padding: 0px 7px } .c7 { margin: 0px } .c8 { font-family: 'Arial'; font-size: 13.33px } </style></head><body class='c0'><p class='c1'><span class='c2'>This table has 2 rows and 3 columns. Mouse pointer wil change to hand cursor when hovered over the table text.</span></p><p class='c3'><span class='c2'>&#x200b;</span></p><table class='c4'><col class='c5'/><col class='c5'/><col class='c5'/><tr><td class='c6'><p class='c7'>row1 col1</p></td><td class='c6'><p class='c7'>row1 col2</p></td><td class='c6'><p class='c7'>row1 col3</p></td></tr><tr><td class='c6'><p class='c7'>row2 col1</p></td><td class='c6'><p class='c7'>row2 col2</p></td><td class='c6'><p class='c7'>row2 col3</p></td></tr></table><p class='c3'><span class='c2'>&#x200b;</span></p><div><p class='c1'><span class='c8'>This table has 2 rows and 3 columns. Mouse pointer wil change to hand cursor when hovered over the table text.</span></p></div><p class='c3'><span class='c2'>&#x200b;</span></p></body></html>";
//            C1Document newdocument = new C1Document();
//            newdocument = richTB.HtmlFilter.ConvertToDocument(myHTMLString);

            if (data.GetDataPresent(typeof(sub_repot_ready)))
            {
               
                ////Image test = new Image();
                ////test = newread.repot_image;
                ////BitmapImage bmp = newread.repot_image.Source as BitmapImage;
                //BitmapImage bmp = newread.repot_image.Source as BitmapImage;
                ////Stream stm = bmp.StreamSource;
                ////stm.Position = 0;
                ////BinaryReader br = new BinaryReader(stm);
                ////byte[] buff = br.ReadBytes((int)stm.Length);
                ////br.Close();
                //RenderTargetBitmap repot_image;
                //repot_image = new RenderTargetBitmap((int)(newread.repot_image.ActualWidth), (int)newread.repot_image.ActualHeight, 96, 96, PixelFormats.Default);
                //repot_image.Render(newread.repot_image);
                //var source = newread.repot_image.Source;
                ////source.UseLayoutRounding="True";
                //var newElement = new C1InlineUIContainer { Content = source, ContentTemplate = ImageAttach.ImageTemplate };

                //ImageAttach.SetFormat(source,newread.repot_image.Source.ToString());
                //using (new DocumentHistoryGroup(richTB.DocumentHistory))
                //{
                //    richTB.Selection.Delete();
                //    richTB.Selection.Start.InsertInline(newElement);
                //}

                var dialog = new C1ImageDialog();
                Uri uri = null;
                dialog.Url = newread.temp_path;
                 uri = new Uri(dialog.Url, UriKind.Absolute);


                var source = new BitmapImage();
                dialog.Stream = File.ReadAllBytes(dialog.Url);

                var newElement = new C1InlineUIContainer { Content = source, ContentTemplate = ImageAttach.ImageTemplate };
                source.BeginInit();
                source.StreamSource = new MemoryStream(dialog.Stream);
                source.EndInit();

                //byte[] bytes = new byte[newread.sm.Length];
                
                //newread.sm.Read(bytes, 0, bytes.Length);
                //newread.sm.Seek(0, SeekOrigin.Begin);
                ////bytes.Reverse();
                ImageAttach.SetStream(source, dialog.Stream);
                ////richTB.Selection.Start.InsertInline(newElement);
              
                //using (new DocumentHistoryGroup(richTB.DocumentHistory))
                //{
                //    richTB.Selection.Delete();
                    richTB.Selection.Start.InsertInline(newElement);
      
                //}
                 //richTB.Document = newdocument;
                //richTB.Selection.Start.InsertInline(newElement);

                //string t = new RtfFilter().ConvertFromDocument(richTB.Document);
                //string t = new RtfFilter().ConvertFromDocument(richTB.Document);
                
            }

          
        }

    }
}
