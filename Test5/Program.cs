using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test5
{
    class Program
    {
        static void Main(string[] args)
        {
            //Bitmap bm = new Bitmap(@"C:\Users\xiejing\Desktop\thin\2U5.bmp");
            //Bitmap bv = new Bitmap(@"C:\Users\xiejing\Desktop\thin\2U5.bmp");
            //Bitmap bv = padding(bm, 3);
            //Bitmap bm = new Bitmap(@"C:\Users\xiejing\Desktop\20zhangoutU2319.bmp");
            //Bitmap bv = new Bitmap(@"C:\Users\xiejing\Desktop\21zhangoutU2319.bmp");
            //List<Features> f1 = getFeature(bm);
            //List<Features> f2 = getFeature(bv);
            //Console.WriteLine(compare(f1, f2));

            //DirectoryInfo di = new DirectoryInfo(@"C:\Users\xiejing\Desktop\22\");
            //FileInfo[] fi = di.GetFiles();
            //for (int i = 0; i < fi.Length; i++)
            //{
            //    batch(fi[i].FullName, @"C:\Users\xiejing\Desktop\22\");
            //}

            DirectoryInfo di = new DirectoryInfo(@"C:\Users\xiejing\Desktop\thin2\");
            FileInfo[] fi = di.GetFiles();
            for (int i = 0; i < fi.Length; i++)
            {
                batch2(fi[i].FullName, @"C:\Users\xiejing\Desktop\thin2\");
            }
        }
        static void batch2(string loc, string folder)
        {
            DirectoryInfo di = new DirectoryInfo(folder);
            FileInfo[] fi = di.GetFiles();
            for (int i = 0; i < fi.Length; i++)
            {
                Bitmap bm = new Bitmap(loc);
                Bitmap bv = new Bitmap(fi[i].FullName);
                List<Features> f1 = getFeature(bm);
                List<Features> f2 = getFeature(bv);
                int result = compare(f1, f2);
                bv.Dispose();
                //Console.WriteLine(loc.Substring(loc.IndexOf('\\')) + ":" + fi[i].Name + ":" + result);
                if(result>50)
                {
                    //File.Copy(fi[i].FullName, @"C:\Users\xiejing\Desktop\thin2\1\" + result+"-" +fi[i].Name);
                    File.Move(fi[i].FullName, @"C:\Users\xiejing\Desktop\thin2\3\" + result + "-" + fi[i].Name);

                }
                else
                {
                    //File.Copy(fi[i].FullName, @"C:\Users\xiejing\Desktop\thin2\2\" + result + "-" + fi[i].Name);
                    
                }
                bm.Dispose();
            }
            Console.Clear();
        }

        static void batch(string loc, string folder)
        {
            DirectoryInfo di = new DirectoryInfo(folder);
            FileInfo[] fi = di.GetFiles();
            for (int i = 0; i < fi.Length; i++)
            {
                Bitmap bm = new Bitmap(loc);
                Bitmap bv = new Bitmap(fi[i].FullName);
                List<Features> f1 = getFeature(bm);
                List<Features> f2 = getFeature(bv);
                Console.WriteLine(loc.Substring(loc.IndexOf('\\')) + ":" + fi[i].Name + ":" + compare(f1, f2));
                
            }
            Console.Clear();
        }
        static int compare(List<Features> f1, List<Features> f2)
        {
            int count = f2.Count > f1.Count ? f2.Count : f1.Count;
            //int score = 0;
            List<Features> pairA = new List<Features>();
            List<Features> pairB = new List<Features>();
            for (int i = 0; i < f1.Count; i++)
            {
                //Console.WriteLine(f1[i].x + " " + f1[i].y);
            }
            for (int i = 0; i < f2.Count; i++)
            {
                //Console.WriteLine(f2[i].x + " " + f2[i].y);
            }
            if (f1.Count < f2.Count)
            {
                List<Features> f3 = new List<Features>();
                f3.AddRange(f1);
                f1.Clear();
                f1.AddRange(f2);
                f2.Clear();
                f2.AddRange(f3);

                //for(int j = 0; j < f1.Count;j++)
            }
            for (int i = 0; i < f2.Count; i++)
            {
                for (int j = 0; j < f1.Count; j++)
                {
                    if (Math.Abs(Math.Max(Math.Abs(f2[i].x - f1[j].x), Math.Abs(f2[i].y - f1[j].y))) < 5)
                    {
                        pairA.Add(f1[j]);
                        pairB.Add(f2[i]);
                        f1.RemoveAt(j);
                        f2.RemoveAt(i--);
                        break;
                    }
                }
            }
            List<Pairs> pair = new List<Pairs>();
            for (int i = 0; i < pairA.Count; i++)
            {
                int tempx = 0;
                int tempy = 0;
                double dB = 0;
                double dO = 0;
                double o = 0;
                tempx = pairA[i].x - pairB[i].x;
                tempy = pairA[i].y - pairB[i].y;
                dB = Math.Abs(Math.Max(Math.Abs(tempx), Math.Abs(tempy)));
                dO = Math.Abs(Math.Sqrt(tempx * tempx + tempy * tempy));
                if (tempx == 0 || tempy == 0)
                {
                    if (tempx == 0 && tempy == 0)
                    {
                        o = 0;
                    }
                    else if (tempx == 0)
                    {
                        if (tempy > 0)
                        {
                            o = 90;
                        }
                        else
                        {
                            o = 270;
                        }
                    }
                    else
                    {
                        if (tempx > 0)
                        {
                            o = 360;
                        }
                        else
                        {
                            o = 180;
                        }
                    }
                }
                else
                {
                    if (tempx * tempy > 0)
                    {
                        if (tempx < 0)
                        {
                            o = 90;
                        }
                        else
                        {
                            o = 270;
                        }
                    }
                    else
                    {
                        if (tempx < 0)
                        {
                            o = 180;
                        }
                        else
                        {
                            o = 180;
                        }
                    }
                    o += Math.Atan((double)tempy / tempx) * (180 / Math.PI);
                    //o += d;
                }
                Pairs p = new Pairs();
                p.distancB = dB;
                p.distancO = dO;
                p.oriation = o;
                pair.Add(p);
            }
            List<int> o2 = new List<int>();
            for (int i = 0; i < pair.Count; i++)
            {
                o2.Add(0);
                for (int j = 0; j < pair.Count; j++)
                {
                    if (Math.Abs(pair[i].oriation - pair[j].oriation) < 30)
                    {
                        o2[i]++;
                    }
                }

            }
            int maxa = -1;
            int maxb = -1;
            for (int i = 0; i < o2.Count; i++)
            {
                if (o2[i] > maxa)
                {
                    maxa = o2[i];
                    maxb = i;
                }
            }
            int shiftx = 0;
            int shifty = 0;
            if (maxb != -1 && pairA.Count > 2 && maxa > pairA.Count / 3)
            {
                if (pair[maxb].distancB != 0)
                {
                    shiftx = (int)Math.Round(Math.Cos(pair[maxb].oriation / (180 / Math.PI)) * pair[maxb].distancO);
                    shifty = (int)Math.Round(Math.Sin(pair[maxb].oriation / (180 / Math.PI)) * pair[maxb].distancO);
                }

            }
            List<int> score = new List<int>();
            for (int i = 0; i < pairA.Count; i++)
            {
                int sa = 0;
                int sb = 0;
                int tempx = pairA[i].x - pairB[i].x - shiftx;
                int tempy = pairA[i].y - pairB[i].y - shifty;
                sa = 5 - ((int)Math.Abs(tempx) + (int)Math.Abs(tempy));
                sb = getAngleDistance(pairA[i].t, pairB[i].t);
                score.Add(sa + sb);
            }
            int S = 0;
            for (int i = 0; i < score.Count; i++)
            {
                S += score[i];
            }
            S *= 10;
            S /= count;
            return S;
        }
        static int getAngleDistance(int a, int b)
        {
            int[,] scoretable = {
                { 5, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 3, 5, 3 ,2, 1, 0, 1, 2, 3},
                { 3, 3, 5, 3 ,2, 1, 0, 1, 2},
                { 3, 2, 3, 5, 3 ,2, 1, 0, 1},
                { 3, 1, 2, 3, 5, 3 ,2, 1, 0},
                { 3, 0, 1, 2, 3, 5, 3 ,2, 1},
                { 3, 1, 0, 1, 2, 3, 5, 3 ,2},
                { 3, 2, 1, 0, 1, 2, 3, 5, 3 },
                { 3, 3, 2, 1, 0, 1, 2, 3, 5 },
            };
            int score = 0;
            score = scoretable[a, b];
            return score;
        }
        static List<Features> getFeature(Bitmap bm)
        {
            //Bitmap bm = new Bitmap(@"C:\Users\xiejing\Desktop\thin\2U5.bmp");
            Bitmap bn = padding(bm, 1);
            int w = bn.Width;
            int[] bnn = toBinaryArray(bn);
            List<Features> result = new List<Features>();
            for (int i = w; i < bnn.Length - w; i++)
            {
                if (bnn[i] == 1)
                {
                    if (bnn[i - 1] == 1 || bnn[i + 1] == 1 || bnn[i + w - 1] == 1 || bnn[i + w] == 1 || bnn[i + w + 1] == 1 || bnn[i - w - 1] == 1 || bnn[i - w] == 1 || bnn[i - w + 1] == 1)
                    {
                        Features f = new Features(i / w, i % w);
                        if (bnn[i - 1] == 1 && bnn[i + 1] == 1 && bnn[i + w] == 1 && bnn[i - w] == 1)
                        {
                            f.t = 0;
                            result.Add(f);
                        }
                        else
                        {
                            int count = 0;
                            int typ = -1;
                            if (bnn[i - 1] == 1)
                            {
                                count++;
                                typ = 8;
                            }
                            if (bnn[i + 1] == 1)
                            {
                                count++;
                                typ = 4;
                            }
                            if (bnn[i + w - 1] == 1)
                            {
                                count++;
                                typ = 7;
                            }
                            if (bnn[i + w] == 1)
                            {
                                count++;
                                typ = 6;
                            }
                            if (bnn[i + w + 1] == 1)
                            {
                                count++;
                                typ = 5;
                            }
                            if (bnn[i - w - 1] == 1)
                            {
                                count++;
                                typ = 1;
                            }
                            if (bnn[i - w] == 1)
                            {
                                count++;
                                typ = 2;
                            }
                            if (bnn[i - w + 1] == 1)
                            {
                                count++;
                                typ = 3;
                            }
                            if (count == 1)
                            {
                                f.t = typ;
                                result.Add(f);
                            }
                            else if (count == 2)
                            {
                                if (bnn[i - w] == 1)
                                {
                                    if (bnn[i - w - 1] == 1 || bnn[i - w + 1] == 1)
                                    {
                                        f.t = 6;
                                        result.Add(f);
                                    }
                                }
                                else if (bnn[i + w] == 1)
                                {
                                    if (bnn[i + w - 1] == 1 || bnn[i + w + 1] == 1)
                                    {
                                        f.t = 2;
                                        result.Add(f);
                                    }
                                }
                                else if (bnn[i - 1] == 1)
                                {
                                    if (bnn[i - w - 1] == 1 || bnn[i + w - 1] == 1)
                                    {
                                        f.t = 8;
                                        result.Add(f);
                                    }
                                }
                                else if (bnn[i + 1] == 1)
                                {
                                    if (bnn[i + w + 1] == 1 || bnn[i - w + 1] == 1)
                                    {
                                        f.t = 4;
                                        result.Add(f);
                                    }
                                }
                                else
                                {
                                }
                            }
                            else if (count == 3)
                            {
                                int count2 = 0;
                                if (bnn[i - w] == 1)
                                {
                                    count2++;
                                }
                                if (bnn[i + w] == 1)
                                {
                                    count2++;
                                }
                                if (bnn[i + 1] == 1)
                                {
                                    count2++;
                                }
                                if (bnn[i - 1] == 1)
                                {
                                    count2++;
                                }
                                if (count2 == 3)
                                {
                                    f.t = 0;
                                    result.Add(f);
                                }
                            }
                        }
                    }
                }
            }
            //Pen penRed = new Pen(Color.Red, 1);
            //Pen penGreen = new Pen(Color.Green, 1);
            //Graphics gf = Graphics.FromImage(bn);
            //gf.CompositingMode = CompositingMode.SourceOver;
            ////用红色的矩形标记分叉点、绿色的椭圆标记断点
            //for (int i = 0; i < result.Count; i++)
            //{
            //    if (result[i].t == 0)
            //    {
            //        gf.DrawRectangle(penRed, result[i].y - 2, result[i].x - 2, 4, 4);
            //    }
            //    else
            //    {
            //        //gf.DrawRectangle(penGreen, result[i].y - 2, result[i].x - 3, 3, 3);
            //        bn.SetPixel(result[i].y, result[i].x, Color.Green);
            //    }
            //}
            //bn.Save(@"C:\Users\xiejing\Desktop\\" + DateTime.Now.ToString("HHmmssfff") + ".bmp");
            return result;
        }
        static Bitmap GetGrayImage(Bitmap image)
        {
            // 如果直接赋值的话，会改变原来的图片。
            Bitmap result = image.Clone() as Bitmap;

            Color c = new Color();
            int ret;
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    c = result.GetPixel(i, j);
                    // 计算点i,j的灰度值
                    ret = (int)(c.R * 0.299 + c.G * 0.587 + c.B * 0.114);

                    result.SetPixel(i, j, Color.FromArgb(ret, ret, ret));
                }
            }
            return result;
        }
        static int[] toBinaryArray(Bitmap bm)
        {
            List<int> tem = new List<int>();
            for (int i = 0; i < bm.Height; i++)
            {
                for (int j = 0; j < bm.Width; j++)
                {
                    if (bm.GetPixel(j, i).R > 128)
                    {
                        tem.Add(1);
                    }
                    else
                    {
                        tem.Add(0);
                    }
                }
            }
            return tem.ToArray();
        }
        static void padding(List<int> original, int w)
        {
            List<int> result = new List<int>();
            List<int> wrapper = new List<int>();
            for (int i = 0; i < w; i++)
            {
                wrapper.Add(0);
            }
            result.AddRange(wrapper);
            result.AddRange(original);
            result.AddRange(wrapper);
            result.Insert(0, 0);
            for (int i = w + 1; i < ((w + 2) * (w + 1)); i++)
            {
                result.Insert(i++, 0);
                result.Insert(i, 0);
                i += w;
            }
            result.Add(0);

        }
        static Bitmap padding(Bitmap original, int n)
        {
            Bitmap result = new Bitmap(original.Width + 2 * n, original.Height + 2 * n);
            for (int i = n; i < original.Width + n; i++)
            {
                for (int j = n; j < original.Height + n; j++)
                {
                    result.SetPixel(i, j, original.GetPixel(i - n, j - n));
                }
            }
            return result;
        }

    }
}
