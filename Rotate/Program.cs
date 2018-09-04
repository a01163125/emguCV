using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rotate
{
    class Program
    {
        static void Main(string[] args)
        {
            Bitmap bm = new Bitmap(@"D:\BaiduNetdiskDownload\00521020.bmp");
            int h1 = getHeight(bm);
            int height = 40;
            int height2 = bm.Height;
            double angle = (double)(height2 - height) / (double)bm.Width;
            angle = Math.Atan(angle);
            angle *= (-180 / Math.PI);
            //Bitmap bm1 = Rotate(bm, (float)angle);
            //bm1.Save(@"C:\Users\xiejing\Desktop\thin1.bmp");
            Bitmap bm2 = Rotate(bm, (float)angle, Color.White);
            bm2.Save(@"C:\Users\xiejing\Desktop\thin2.bmp");
            int h2 = getHeight(bm2);
            if (h2 > h1)
            {
                angle *= -1;
            }
            int besti = 0;
            int besth = h2;
            for (int i = -5; i < 5; i++)
            {
                double offset = i * 0.05;
                Bitmap bm3 = Rotate(bm, (float)(offset+angle), Color.White);
                int h3 = getHeight(bm3);
                if(h3<besth)
                {
                    besti = i;
                    besth = h3;
                }
            }

            Bitmap bm4 = Rotate(bm, (float)angle, Color.White);
            
            bm4 = cutEdge(bm4);
            bm4.Save(@"C:\Users\xiejing\Desktop\thin3.bmp");
        }


        public static Bitmap cutEdge(Bitmap bm)
        {
            int T = 450;
            int Tw = 10;
            int up = 0;
            int down = 0;
            for(int i = 0; i < bm.Height; i++)
            {
                int count = 0;
                for(int j =0; j < bm.Width;j++)
                {
                    Color c = bm.GetPixel(j, i);
                    if(c.R+c.G+c.B<T)
                    {
                        count++;
                    }
                }
                if(count<Tw)
                {
                    up++;
                }
                else
                {
                    break;
                }
            }

            for (int i = bm.Height-1; i>0; i--)
            {
                int count = 0;
                for (int j = 0; j < bm.Width; j++)
                {
                    Color c = bm.GetPixel(j, i);
                    if (c.R + c.G + c.B < T)
                    {
                        count++;
                    }
                }
                if (count < Tw)
                {
                    down++;
                }
                else
                {
                    break;
                }
            }
            if(up>2)
            {
                up -= 2;
            }
            if (down > 2)
            {
                down -= 2;
            }
            return KiCut(bm, 0, up, bm.Width, bm.Height - up - down);
        }
        /// <summary>
        /// 对图像进行任意角度的旋转
        /// </summary>
        static public Bitmap Rotate(Bitmap bmp, float angle)
        {
            return Rotate(bmp, angle, Color.Transparent);
        }
        /// <summary>
        /// 任意角度旋转,此函数非原创
        /// </summary>
        static public Bitmap Rotate(Bitmap bmp, float angle, Color bkColor)
        {
            int w = bmp.Width + 2;
            int h = bmp.Height + 2;

            PixelFormat pf;

            if (bkColor == Color.Transparent)
            {
                pf = PixelFormat.Format32bppArgb;
            }
            else
            {
                pf = bmp.PixelFormat;
            }

            Bitmap tmp = new Bitmap(w, h, pf);
            Graphics g = Graphics.FromImage(tmp);
            g.Clear(bkColor);
            g.DrawImageUnscaled(bmp, 1, 1);
            g.Dispose();

            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new RectangleF(0f, 0f, w, h));
            Matrix mtrx = new Matrix();
            mtrx.Rotate(angle);
            RectangleF rct = path.GetBounds(mtrx);

            Bitmap dst = new Bitmap((int)rct.Width, (int)rct.Height, pf);
            g = Graphics.FromImage(dst);
            g.Clear(bkColor);
            g.TranslateTransform(-rct.X, -rct.Y);
            g.RotateTransform(angle);
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.DrawImageUnscaled(tmp, 0, 0);
            g.Dispose();

            tmp.Dispose();

            return dst;
        }
        //获取图像pic旋转angle角度后的图像
        static public Bitmap Rotate2(Image pic, float angle)
        {
            //创建图像
            int size = pic.Width > pic.Height ? pic.Width * 3 : pic.Height * 3;

            Bitmap tmp = new Bitmap(size, size);                           //按指定大小创建位图
            Rectangle Rect = new Rectangle(0, 0, pic.Width, pic.Height);   //pic的整个区域

            //绘制
            Graphics g = Graphics.FromImage(tmp);                   //从位图创建Graphics对象
            g.Clear(Color.FromArgb(0, 0, 0, 0));                    //清空

            g.TranslateTransform(Rect.Width / 2, Rect.Height / 2);  //设置为绕中心处旋转
            g.RotateTransform(angle);                               //控制旋转角度

            Point pos = new Point((int)((size - pic.Width) / 2), (int)((size - pic.Height) / 2));  //中心对齐
            g.DrawImage(pic, pos);                                  //绘制图像

            g.TranslateTransform(-Rect.Width / 2, -Rect.Height / 2);//还原锚点为左上角

            return tmp;     //返回构建的新图像
        }
        static int getHeight(Bitmap bm)
        {
            int T = 550;
            int Tw = 10;
            int continuous = 0;
            int temp = 0;
            List<int> count = new List<int>();
            for (int i = 0; i < bm.Height; i++)
            {
                int count0 = 0;
                for (int j = 0; j < bm.Width; j++)
                {
                    Color c = bm.GetPixel(j, i);
                    if (c.R + c.G + c.B < T)
                    {
                        count0++;
                    }
                }
                count.Add(count0);
            }
            for (int i = 0; i < count.Count; i++)
            {

                if(count[i]>Tw/2)
                {
                    continuous++;
                }
                else
                {
                    if (continuous>temp)
                    {
                        temp = continuous;
                    }
                        continuous = 0;
                    
                }
            }
            if(temp>continuous)
            {
                continuous = temp;
            }

            return continuous;
        }
        public static Bitmap KiCut(Bitmap b, int StartX, int StartY, int iWidth, int iHeight)
        {
            if (b == null)
            {
                return null;
            }
            int w = b.Width;
            int h = b.Height;
            if (StartX >= w || StartY >= h)
            {
                return null;
            }
            if (StartX + iWidth > w)
            {
                iWidth = w - StartX;
            }
            if (StartY + iHeight > h)
            {
                iHeight = h - StartY;
            }
            try
            {
                Bitmap bmpOut = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage(bmpOut);
                g.DrawImage(b, new Rectangle(0, 0, iWidth, iHeight), new Rectangle(StartX, StartY, iWidth, iHeight), GraphicsUnit.Pixel);
                g.Dispose();
                return bmpOut;
            }
            catch
            {
                return null;
            }
        }
    }
}
