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
using Emgu.CV;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using usb_video_app;

namespace IrAnalyse
{
    /// <summary>
    /// recordtest.xaml 的交互逻辑
    /// </summary>
    public partial class recordtest : System.Windows.Controls.UserControl
    {
        public recordtest()
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





        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        private void record_Click(object sender, RoutedEventArgs e)
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
            MediaElement aa = new MediaElement();
           
            //IntPtr nhwnd = ((HwndSource)PresentationSource.FromVisual(video)).Handle;
            //uvd.set_view_wnd(new  WindowInteropHelper(video).Handle);


            var hwndSource=(System.Windows.Interop.HwndSource)PresentationSource.FromDependencyObject(this);
            IntPtr Ptrhandle = hwndSource.Handle;
            video.Width = 640;
            video.Height = 480;
            var hPlayWnd = video.Handle;

            uvd.set_view_wnd(hPlayWnd);
            if (uvd != null)
            {
                uvd.play();
            }
         //   Process p = new Process();
         //   //ffplay -window_title myplayer -loop 2 test.mp4
         //   p.StartInfo.FileName = @"D:\迅雷下载\ffmpeg-20161122-d316b21-win32-static\bin\ffplay.exe";
         //   //p.StartInfo.FileName = @"D:\电影\[电影天堂www.dygod.net].武状元苏乞儿DVD国语中字-cd1.rmvb";
         //   p.StartInfo.Arguments = @" -autoexit D:\电影\[电影天堂www.dygod.net].武状元苏乞儿DVD国语中字-cd1.rmvb";
         //       p.StartInfo.UseShellExecute = false;
         //       p.StartInfo.CreateNoWindow = true;
         //       p.ErrorDataReceived += new DataReceivedEventHandler(p_ErrorDataReceived);
         //       p.StartInfo.RedirectStandardInput = true;
         //       p.StartInfo.RedirectStandardOutput = true;
         //       p.StartInfo.RedirectStandardError = true; 
         //   p.Start();
         //   Thread.Sleep(200);
         //    p.BeginErrorReadLine();//开始异步读取
         //p.WaitForExit();//阻塞等待进程结束

         //SetParent(ffplay.MainWindowHandle, this.Handle);




         var ffplay = new Process
         {
             StartInfo =
             {
                 FileName =  @"D:\迅雷下载\ffmpeg-20161122-d316b21-win32-static\bin\ffplay.exe",
                 Arguments = @" -autoexit D:\电影\[电影天堂www.dygod.net].武状元苏乞儿DVD国语中字-cd1.rmvb",
                 // hides the command window
                 CreateNoWindow = true,
                 // redirect input, output, and error streams..
                 RedirectStandardError = true,
                 RedirectStandardOutput = true,
                 UseShellExecute = false
             }
         };

         ffplay.EnableRaisingEvents = true;
         ffplay.ErrorDataReceived += new DataReceivedEventHandler(p_ErrorDataReceived);
         //ffplay.Start();
         Thread.Sleep(200);
         //ffplay.WaitForExit();
         //InsertWindow insertwin = new InsertWindow(panel1, appFile);
         IntPtr hwnd = ((HwndSource)PresentationSource.FromVisual(maincanvas)).Handle;
         //SetParent(ffplay.MainWindowHandle, hwnd);
 
         //MoveWindow(ffplay.MainWindowHandle, 0, 0, 320, 280, true);
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "MP4文件|*.mp4|AVI文件|*.avi|RMVB文件|*.rmvb|WMV文件|*.wmv|MKV文件|*.mkv|所有文件|*.*";

            //if (openFileDialog.ShowDialog() == DialogResult.OK)
            //{
            //    Application. += Application_Idle;
            //    cap = new Capture(openFileDialog.FileName);
            //    fps = (int)cap.GetCaptureProperty(CapProp.Fps);
            //}


        }

        void p_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                //record.Content = e.Data.ToString();
                //System.Windows.MessageBox.Show(e.Data.ToString());
            }
        }
    }
}
