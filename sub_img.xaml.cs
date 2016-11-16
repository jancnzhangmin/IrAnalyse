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

namespace IrAnalyse
{
    /// <summary>
    /// sub_img.xaml 的交互逻辑
    /// </summary>
    public partial class sub_img : UserControl
    {
        public sub_img()
        {
            InitializeComponent();
        }


      
        public static readonly RoutedEvent LoadImgInformationEvent = EventManager.RegisterRoutedEvent("LoadImgInformation", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(sub_img));
        public event RoutedPropertyChangedEventHandler<object> LoadImgInformation
        {
            add { AddHandler(LoadImgInformationEvent, value); }
            remove { RemoveHandler(LoadImgInformationEvent, value); }
        }

        public static readonly RoutedEvent LoadVsInformationEvent = EventManager.RegisterRoutedEvent("LoadVsInformation", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<object>), typeof(sub_img));
        public event RoutedPropertyChangedEventHandler<object> LoadVsInformation
        {
            add { AddHandler(LoadVsInformationEvent, value); }
            remove { RemoveHandler(LoadVsInformationEvent, value); }
        }

        public string filename;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
           
                RoutedPropertyChangedEventArgs<object> args = new RoutedPropertyChangedEventArgs<object>(this, e);
                args.RoutedEvent = sub_img.LoadImgInformationEvent;
                this.RaiseEvent(args);
          
                RoutedPropertyChangedEventArgs<object> vs_args = new RoutedPropertyChangedEventArgs<object>(this, e);
                vs_args.RoutedEvent = sub_img.LoadVsInformationEvent;
                this.RaiseEvent(vs_args);
        

            if (filename != null)
            {
                using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open,FileAccess.Read)))
                {

                    FileInfo fi = new FileInfo(filename);

                    byte[] bytes = reader.ReadBytes((int)fi.Length);

                    reader.Close();

                    BitmapImage bmp = new BitmapImage();

                    bmp.BeginInit();

                    bmp.StreamSource = new MemoryStream(bytes);

                    bmp.EndInit();

                    opinion_img.Source = bmp;

                    bmp.CacheOption = BitmapCacheOption.OnLoad;

                }
            }
        }




      
    }

}
