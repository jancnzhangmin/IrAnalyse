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
using System.Windows.Shapes;
using System.IO;

namespace IrAnalyse
{
    /// <summary>
    /// palette.xaml 的交互逻辑
    /// </summary>
    public partial class palette : Window
    {
        public palette()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
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

            createpalette();
            

            
        }

        private void readRGB(string filename,string palettename)//读取调色板文件的RGB值
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
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
                }else
                    if (palettename == "medical")
                    {
                        PublicClass.medical.Add(len_2(color_R) + len_2(color_G) + len_2(color_B));
                    }else
                        if (palettename == "grey")
                        {
                            PublicClass.grey.Add(len_2(color_R) + len_2(color_G) + len_2(color_B));
                        }else
                            if (palettename == "new_rainbow")
                            {
                                PublicClass.new_rainbow.Add(len_2(color_R) + len_2(color_G) + len_2(color_B));
                            }else
                                if (palettename == "rainbow")
                                {
                                    PublicClass.rainbow.Add(len_2(color_R) + len_2(color_G) + len_2(color_B));
                                }else
                                    if (palettename == "red_brown")
                                    {
                                        PublicClass.red_brown.Add(len_2(color_R) + len_2(color_G) + len_2(color_B));
                                    }else
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
            strbte = Convert.ToString(bte,16);
            if (strbte.Length < 2)
            {
                strbte = "0" + strbte;
            }
            return strbte;
        }

        private void createpalette()//生成调色板图片
        {

            int[] pixels = new int[1];
            int stride = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;


            WriteableBitmap iron_bmp = new WriteableBitmap(1, 256, 96, 96, PixelFormats.Bgr32, null);
            for (int i = 0; i < 256; i++)
            {
                int pixel = Convert.ToInt32(PublicClass.Iron_palette[i].ToString(), 16); 
                pixels[0] = pixel;
                iron_bmp.WritePixels(new Int32Rect(0, i, 1, 1), pixels, stride, 0);
            }
            iron.Source = iron_bmp;

            WriteableBitmap medical_bmp = new WriteableBitmap(1, 256, 96, 96, PixelFormats.Bgr32, null);
            for (int i = 0; i < 256; i++)
            {
                int pixel = Convert.ToInt32(PublicClass.medical[i].ToString(), 16);
                pixels[0] = pixel;
                medical_bmp.WritePixels(new Int32Rect(0, i, 1, 1), pixels, stride, 0);
            }
            medical.Source = medical_bmp;

            WriteableBitmap grey_bmp = new WriteableBitmap(1, 256, 96, 96, PixelFormats.Bgr32, null);
            for (int i = 0; i < 256; i++)
            {
                int pixel = Convert.ToInt32(PublicClass.grey[i].ToString(), 16);
                pixels[0] = pixel;
                grey_bmp.WritePixels(new Int32Rect(0, i, 1, 1), pixels, stride, 0);
            }
            grey.Source = grey_bmp;

            WriteableBitmap new_rainbow_bmp = new WriteableBitmap(1, 256, 96, 96, PixelFormats.Bgr32, null);
            for (int i = 0; i < 256; i++)
            {
                int pixel = Convert.ToInt32(PublicClass.new_rainbow[i].ToString(), 16);
                pixels[0] = pixel;
                new_rainbow_bmp.WritePixels(new Int32Rect(0, i, 1, 1), pixels, stride, 0);
            }
            new_rainbow.Source = new_rainbow_bmp;

            WriteableBitmap rainbow_bmp = new WriteableBitmap(1, 256, 96, 96, PixelFormats.Bgr32, null);
            for (int i = 0; i < 256; i++)
            {
                int pixel = Convert.ToInt32(PublicClass.rainbow[i].ToString(), 16);
                pixels[0] = pixel;
                rainbow_bmp.WritePixels(new Int32Rect(0, i, 1, 1), pixels, stride, 0);
            }
            rainbow.Source = rainbow_bmp;

            WriteableBitmap red_brown_bmp = new WriteableBitmap(1, 256, 96, 96, PixelFormats.Bgr32, null);
            for (int i = 0; i < 256; i++)
            {
                int pixel = Convert.ToInt32(PublicClass.red_brown[i].ToString(), 16);
                pixels[0] = pixel;
                red_brown_bmp.WritePixels(new Int32Rect(0, i, 1, 1), pixels, stride, 0);
            }
            red_brown.Source = red_brown_bmp;

            WriteableBitmap isothermal_bmp = new WriteableBitmap(1, 256, 96, 96, PixelFormats.Bgr32, null);
            for (int i = 0; i < 256; i++)
            {
                int pixel = Convert.ToInt32(PublicClass.isothermal[i].ToString(), 16);
                pixels[0] = pixel;
                isothermal_bmp.WritePixels(new Int32Rect(0, i, 1, 1), pixels, stride, 0);
            }
            isothermal.Source = isothermal_bmp;








        }
    }
}
