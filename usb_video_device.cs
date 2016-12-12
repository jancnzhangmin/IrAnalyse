using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace usb_video_app
{
    // 该类用于封装usb视频设备的相关操作
    public class usb_video_device
    {
        public delegate void USB_DATA_CALLBACK(IntPtr pData, int size, IntPtr arg);

        // 创建一个用于操作usb设备的对象
        [DllImport(@"usb_video.dll", EntryPoint = "usb_video_create")]
        extern static private int usb_video_create(int index, USB_DATA_CALLBACK callback, IntPtr arg);

        // 关闭设备
        [DllImport(@"usb_video.dll", EntryPoint = "usb_video_close")]
        extern static private void usb_video_close(int h);

        // 开始播放数据
        [DllImport(@"usb_video.dll", EntryPoint = "usb_video_start")]
        extern static private void usb_video_start(int h);

        // 停止播放
        [DllImport(@"usb_video.dll", EntryPoint = "usb_video_stop")]
        extern static private void usb_video_stop(int h);

        // 设置视频图像绘制的窗口
        [DllImport(@"usb_video.dll", EntryPoint = "usb_video_set_view_wnd")]
        extern static private void usb_video_set_view_wnd(int h, IntPtr hwnd);

        // 将视频数据绘制到窗口上
        [DllImport(@"usb_video.dll", EntryPoint = "usb_video_draw_to_wnd")]
        extern static private void usb_video_draw_to_wnd(int h, IntPtr data, int size);

        [DllImport(@"usb_video.dll", EntryPoint = "usb_video_set_keycmd")]
        extern static private void usb_video_set_keycmd(int h, int keycmd);

        // 内部设备操作的句柄
        protected int h_usb_video = 0;
        // 内部数据回调的函数
        USB_DATA_CALLBACK data_callback;

        // 外部数据接收处理委托
        Usb_Video_Data_Revice_Event_Handler data_revice_handler = null;

        public usb_video_device()
        { 
        }

        /// <summary>
        /// 打开指定索引的设备
        /// </summary>
        /// <param name="index">设备索引</param>
        /// <returns>成功返回真，失败返回假</returns>
        public bool create( int index, Usb_Video_Data_Revice_Event_Handler eh )
        {
            data_revice_handler = eh;

            // 指定数据回调的函数
            data_callback = usb_data_callback;


            // 指定数据回调的参数，用于区分是哪个对象调用的
            usb_data_callback_arg t = new usb_data_callback_arg();
            t.fthis = this;

            IntPtr sp = Marshal.AllocCoTaskMem(Marshal.SizeOf(t));
            Marshal.StructureToPtr(t, sp, true);

            // 创建一个设备，用于后续的操作
            h_usb_video = usb_video_create(index, data_callback, sp);

            return h_usb_video != 0;
        }


        /// <summary>
        /// 关闭设备，停止内部的播放线程，并释放内部相关的数据缓存
        /// </summary>
        public void close()
        {
            if(h_usb_video!=0)
            {
                usb_video_close(h_usb_video);
                h_usb_video = 0;
            }
        }

        /// <summary>
        /// 播放图像，内部创建数据获取线程，并进行数据回调
        /// </summary>
        public void play()
        {
            if (h_usb_video != 0)
            {
                usb_video_start(h_usb_video);
            }
        }

        /// <summary>
        /// 停止播放，停止内部数据获取线程
        /// </summary>
        public void stop()
        {
            if (h_usb_video != 0)
            {
                usb_video_stop(h_usb_video);
            }
        }


        /// <summary>
        /// 设置图像绘制窗口
        /// </summary>
        /// <param name="hwnd"></param>
        public void set_view_wnd( IntPtr hwnd)
        {
            if (h_usb_video != 0)
            {
                usb_video_set_view_wnd(h_usb_video, hwnd);
            }
        }

        /// <summary>
        /// 将图像数据绘制到窗口上
        /// </summary>
        /// <param name="data">图像数据</param>
        /// <param name="size">数据大小</param>
        public void draw_to_wnd(IntPtr data, int size)
        {
            if (h_usb_video != 0)
            {
                usb_video_draw_to_wnd( h_usb_video, data, size );
            }
        }

        /// <summary>
        /// 设置键命令
        /// </summary>
        /// <param name="keycmd"></param>
        public void set_keycmd(USB_VIDEO_KEY_CMD keycmd)
        {
            if (h_usb_video != 0)
            {
                usb_video_set_keycmd( h_usb_video, (int)keycmd);
            }
        }

        /// <summary>
        /// 内部图像数据回调部分
        /// </summary>
        /// <param name="pData">图像数据</param>
        /// <param name="size">图像数据大小</param>
        /// <param name="arg">创建设备时指定的附加参数，用于区分设备</param>
        static private void usb_data_callback(IntPtr pData, int size, IntPtr arg)
        {
            byte[] data = new byte[size];
            Marshal.Copy(pData, data, 0, size);

            usb_data_callback_arg t = (usb_data_callback_arg)Marshal.PtrToStructure(arg, typeof(usb_data_callback_arg));
            t.fthis.usb_data_callback(data, size);

        }

        private void usb_data_callback(byte[] data, int size)
        {
            // 内部数据回调
            if (data_revice_handler!=null)
            {
                data_revice_handler(this, data, size);
            }
            
        }

        
    }
    
    // 设备键命令
    public enum USB_VIDEO_KEY_CMD
    {
        KEY_UP = 0x0FD,
        KEY_DOWN = 0xFB,
        KEY_LEFT = 0xF7,
        KEY_RIGHT = 0xEF,
        KEY_OK = 0xFE,
        KEY_ESC = 0x7F,
        KEY_SAVE = 0xBF,
        KEY_AUTO = 0xDF,
        KEY_ESC_SAVE = 	KEY_ESC&KEY_SAVE,
        KEY_ESC_AUTO = 	KEY_ESC&KEY_AUTO,
        KEY_ESC_DOWN_UP_ENTRN = KEY_ESC&KEY_DOWN&KEY_UP&KEY_OK //标定状态  
    };

    public struct usb_data_callback_arg
    {
        public usb_video_device fthis;
    }

    // usb数据接收委托
    public delegate void Usb_Video_Data_Revice_Event_Handler(usb_video_device ob, byte[] data, int size);
}
