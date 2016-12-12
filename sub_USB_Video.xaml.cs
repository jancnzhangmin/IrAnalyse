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
using usb_video_app;
using System.Runtime.InteropServices;
using SharpFFmpeg;


namespace IrAnalyse
{
    /// <summary>
    /// sub_USB_Video.xaml 的交互逻辑
    /// </summary>
    public partial class sub_USB_Video : UserControl
    {
        public sub_USB_Video()
        {
            InitializeComponent();
        }


        public delegate void useb_data_delegate(usb_video_device ob, byte[] data, int size);
        usb_video_device uvd = new usb_video_device();


        public void Usb_Video_Data_Revice(usb_video_device ob, byte[] data, int size)
        {
            // 由于内部数据回调采用的是多线程，根据C#的规定，不要在多线程里操作界面，所以这里采用委托的方式将数据转到主线程进行操作

            if (this.CheckAccess())
            {
                useb_data_delegate ubd = new useb_data_delegate(Usb_Video_Data_Revice);
                this.Dispatcher.Invoke(ubd, new object[] { ob, data, size });
            }
            else
            {
                IntPtr p = Marshal.AllocHGlobal(data.Length);
                Marshal.Copy(data, 0, p, data.Length);

                uvd.draw_to_wnd(p, data.Length);
                Marshal.FreeHGlobal(p);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // 创建设备

            bool ret = uvd.create(0, new Usb_Video_Data_Revice_Event_Handler(Usb_Video_Data_Revice));
            if (!ret)
            {
                //MessageBox.Show(this, "创建失败！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                System.Windows.MessageBox.Show("创建失败！");
                return;
            }

            // 设置图像绘制窗口，如果是自己处理图像数据，则可以不调用，这里为了演示
            var hPlayWnd = video.Handle;
            uvd.set_view_wnd(hPlayWnd);
            if (uvd != null)
            {
                uvd.play();
            }
        }

        private void record_Click(object sender, RoutedEventArgs e)
        {
            FFmpeg.av_register_all();
        }
    }
}
