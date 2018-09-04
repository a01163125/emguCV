using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test4//My thinning algorithm
{
    class Program
    {
        static private Bitmap GetBinaryzationImage1(Bitmap image, int thr)
        {
            Bitmap result = image.Clone() as Bitmap;
            Color color = new Color();
            for (int i = 0; i < result.Width; i++)
            {
                for (int j = 0; j < result.Height; j++)
                {
                    color = result.GetPixel(i, j);
                    if (color.R > thr)
                    {
                        result.SetPixel(i, j, Color.White);
                    }
                    else
                    {
                        result.SetPixel(i, j, Color.Black);
                    }
                }
            }

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
        static Bitmap toBmp(int[] arr, int w, int h)
        {
            Bitmap bm = new Bitmap(w, h);
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    int x = i * w + j;
                    int a = arr[x];
                    if (a >= 1)
                    {
                        a = 255;
                    }
                    else
                    {
                        a = 0;
                    }
                    bm.SetPixel(j, i, Color.FromArgb(a, a, a));
                }
            }
            return bm;
        }
        static void show(Bitmap bm, bool[] undeletable)
        {
            Bitmap result = bm.Clone() as Bitmap;
            for (int i = 0; i < result.Height; i++)
            {
                for (int j = 0; j < result.Width; j++)
                {
                    if (undeletable[i * bm.Width + j])
                    {
                        result.SetPixel(j, i, Color.White);
                    }
                    else
                    {
                        result.SetPixel(j, i, Color.Black);
                    }
                }
            }
            //result.Save(@"C:\Users\xiejing\Desktop\U2.bmp");
        }
        static Bitmap reverseColor(Bitmap whitebase)
        {
            for (int i = 0; i < whitebase.Width; i++)
            {
                for (int j = 0; j < whitebase.Height; j++)
                {
                    Color c = whitebase.GetPixel(i, j);
                    if (c.R > 128)
                    {
                        whitebase.SetPixel(i, j, Color.Black);
                    }
                    else
                    {
                        whitebase.SetPixel(i, j, Color.White);
                    }
                }
            }
            //for(int i = 0; i < whitebase.Length;i++)
            //{
            //    if(whitebase[i]==1)
            //    {
            //        whitebase[i] = 0;
            //    }
            //    else
            //    {
            //        whitebase[i] = 1;
            //    }
            //}
            return whitebase;
        }


        static void Main(string[] args)
        {
            //iterate(@"C:\Users\xiejing\Desktop\3\U5.bmp");
            DirectoryInfo dir = new DirectoryInfo(@"C:\Users\xiejing\Desktop\4\HN\WN\3238");
            foreach (FileInfo file in dir.GetFiles())//第二个参数表示搜索包含子目录中的文件；
            {
                iterate(file.FullName);
            }
            
        }
        static void iterate(string filename)
        {
            string folder = filename.Substring(0,filename.LastIndexOf('\\'));
            string folde2 = folder.Substring(0, folder.LastIndexOf('\\'));
            string name = filename.Substring(filename.LastIndexOf('\\')+1);
            Bitmap pImageBuffer;
            pImageBuffer = new Bitmap(filename);

            //灰度图像
            Bitmap srcImg;
            srcImg = new Bitmap(pImageBuffer.Width, pImageBuffer.Height);
            srcImg = GetGrayImage(pImageBuffer);
            //srcImg.Save(folde2+"\\grey" + name);
            int w = srcImg.Width;
            int h = srcImg.Height;
            int n = w * h;

            //二值化图像
            bool[] undeletable = new bool[n];
            srcImg = GetBinaryzationImage1(srcImg, 180);
            //srcImg.Save(folde2 + "\\2" + name);
            trySkeleton(srcImg, undeletable);
            show(srcImg, undeletable);
            //待细化图像
            int[] inputImg = new int[n];
            srcImg = reverseColor(srcImg);
            inputImg = toBinaryArray(srcImg);

            //显示图像
            Bitmap imgshow;
            imgshow = new Bitmap(w, h);

            int[] ZhangFastImg = new int[n];
            ZhangFastImg = ZhangFastThin(inputImg, w, h, undeletable);
            imgshow = toBmp(ZhangFastImg, w, h);

            imgshow.Save(folde2+"\\thin\\"+name);
            //inputImg = toBinaryArray(srcImg);
        }
        static void trySkeleton(Bitmap bm, bool[] undeletable)
        {
            int Tlength = bm.Height / 3;
            List<Count> results = new List<Count>();
            List<Count> results_H = new List<Count>();
            for (int i = 0; i < bm.Height; i++)
            {
                for (int j = 0; j < bm.Width; j++)
                {
                    Color c = bm.GetPixel(j, i);
                    //Console.WriteLine(c.R +":"+ c.G + ":" + c.B);
                    if (c.R == 0)
                    {
                        int count = 0;
                        int k = 0;
                        while (k + i < bm.Height && bm.GetPixel(j, i + k++).R == 0)
                        {
                            count++;
                        }
                        ;
                        if (count > Tlength)
                        {
                            //int Tlength2 = (int)(count*0.9);
                            //List<Count> result = new List<Count>();
                            ////result.Add(count);
                            //int count2 = count;
                            //int l = 0;
                            //while (count2> Tlength2)
                            //{
                            //    Count co = new Count();
                            //    co.count = count2;
                            //    co.start = i;
                            //    result.Add(co);
                            //    count2 = 0;
                            //    k = 0;
                            //    l++;
                            //    while (j + l < bm.Width&& k + i < bm.Height && bm.GetPixel(j+l, i + k++).R == 0)
                            //    {
                            //        count2++;
                            //    }
                            //}
                            ////result.Add(count2);
                            bool REMAIN = true;
                            for (int l = 0; l < results.Count; l++)
                            {
                                if (j == results[l].startY && i + count == results[l].count + results[l].startX)
                                {
                                    REMAIN = false;
                                    break;
                                }
                                else
                                {
                                    //REMAIN = true;

                                }
                            }
                            if (REMAIN)
                            {
                                Count co = new Count();
                                co.count = count;
                                co.startX = i;
                                co.startY = j;
                                results.Add(co);
                                Console.WriteLine(i + "    " + j + "count:" + count);
                            }

                        }
                    }
                }
            }
            for (int i = 0; i < bm.Height; i++)
            {
                for (int j = 0; j < bm.Width; j++)
                {
                    Color c = bm.GetPixel(j, i);
                    //Console.WriteLine(c.R +":"+ c.G + ":" + c.B);
                    if (c.R == 0)
                    {
                        int count = 0;
                        int k = 0;
                        while (k + j < bm.Width && bm.GetPixel(j + k++, i).R == 0)
                        {
                            count++;
                        }
                        ;
                        if (count > Tlength)
                        {
                            bool REMAIN = true;
                            for (int l = 0; l < results_H.Count; l++)
                            {
                                if (i == results_H[l].startX && j + count == results_H[l].count + results_H[l].startY)
                                {
                                    REMAIN = false;
                                    break;
                                }
                                else
                                {
                                    //REMAIN = true;

                                }
                            }
                            if (REMAIN)
                            {
                                Count co = new Count();
                                co.count = count;
                                co.startX = i;
                                co.startY = j;
                                co.horizontal = true;
                                results_H.Add(co);
                                Console.WriteLine(i + "    " + j + "count:" + count);
                            }
                        }
                    }
                }
            }
            List<Count> result_V = group(results, false);
            List<Count> result_H = group(results_H, true);
            lockbit(result_V, undeletable, bm.Width, bm.Height);
            lockbit(result_H, undeletable, bm.Width, bm.Height);
        }
        static List<Count> group(List<Count> count, bool horizontal)
        {
            if (horizontal)
            {
                if (count.Count > 2)
                {
                    for (int i = 1; i < count.Count - 1; i++)
                    {
                        if (count[i].startX != count[i - 1].startX + 1 && count[i].startX != count[i + 1].startX - 1)
                        {
                            count.RemoveAt(i--);
                        }
                    }
                }
            }
            else
            {
                if (count.Count > 2)
                {
                    for (int i = 0; i < count.Count -1; i++)
                    {
                        for (int j = i+1; j < count.Count ; j++)
                        {
                            if (count[i].startY > count[j].startY)
                            {
                                Count c = new Count();
                                c = count[i];
                                count[i] = count[j];
                                count[j] = c;
                            }
                        }
                    }
                    for (int i = 1; i < count.Count - 1; i++)
                    {
                        if (count[i].startY != count[i - 1].startY + 1 && count[i].startY != count[i + 1].startY - 1)
                        {
                            count.RemoveAt(i--);
                        }
                    }
                }
            }
            List<List<Count>> resultss = new List<List<Count>>();
            List<Count> result = new List<Count>();
            if (count.Count > 0)
            {
                result.Add(count[0]);
                count.RemoveAt(0);

                if (horizontal)
                {
                    while (count.Count > 0)
                    {
                        for (int i = 0; i < count.Count; i++)
                        {
                            if (count[i].startX == result[result.Count - 1].startX + 1)
                            {
                                //if (Math.Abs(count[i].startY - result[result.Count - 1].startY) <= 2)
                                {
                                    result.Add(count[i]);
                                    count.RemoveAt(i--);
                                }
                            }
                        }
                        resultss.Add(result);
                        result = new List<Count>();
                        if (count.Count > 0)
                        {
                            result.Add(count[0]);
                            count.RemoveAt(0);
                        }
                    }
                    if (result != null && result.Count > 0)
                    {
                        resultss.Add(result);
                        result = null;
                    }
                }
                else
                {
                    while (count.Count > 0)
                    {
                        for (int i = 0; i < count.Count; i++)
                        {
                            if (count[i].startY == result[result.Count - 1].startY + 1)
                            {
                                if (count[i].startX - result[result.Count - 1].startX <= 2)
                                {
                                    result.Add(count[i]);
                                    count.RemoveAt(i--);
                                }
                            }
                        }
                        resultss.Add(result);
                        result = new List<Count>();
                        if (count.Count > 0)
                        {
                            result.Add(count[0]);
                            count.RemoveAt(0);
                        }
                    }
                    if (result != null && result.Count > 0)
                    {
                        resultss.Add(result);
                        result = null;
                    }
                }
            }
            for (int i = 0; i < resultss.Count; i++)
            {
                if (resultss[i].Count == 1)
                {
                    resultss.RemoveAt(i--);
                }
            }
            return decide(resultss);
        }
        static List<Count> decide(List<List<Count>> resultss)
        {
            List<Count> count = new List<Count>();
            for (int i = 0; i < resultss.Count; i++)
            {
                Count c = resultss[i][0];
                for (int j = 1; j < resultss[i].Count; j++)
                {
                    if (resultss[i][j].count > c.count)
                    {
                        c = resultss[i][j];
                    }
                }
                count.Add(c);
            }
            return count;

        }
        static void lockbit(List<Count> decision, bool[] undeletable, int w, int h)
        {
            for (int i = 0; i < decision.Count; i++)
            {
                if (decision[i].horizontal)
                {
                    int start = decision[i].startX * w + decision[i].startY;
                    if (decision[i].count > 5)
                    {
                        for (int j = 2; j < decision[i].count-2; j++)
                        {
                            undeletable[start + j] = true;
                        }
                    }
                }
                else
                {
                    int start = decision[i].startX * w + decision[i].startY;
                    if (decision[i].count > 5)
                    {
                        for (int j = 2; j < decision[i].count-2; j++)
                        {
                            undeletable[start + j * w] = true;
                        }
                    }
                }
            }
        }
        static int[] ZhangFastThin(int[] inputImg, int w, int h, bool[] undeletable)
        {
            int sizen = w * h;
            int[] outImg = inputImg;
            //for (int i = 0; i < sizen; i++) Console.WriteLine(inputImg[i]);
            int foreground = 1; //前景点
            int background = 1 - foreground;//背景点

            //8领域编码
            //p3  p2  p1
            //p4   p   p0
            //p5  p6  p7
            int[] d = new int[8] { 1, 1 - w, -w, -1 - w, -1, w - 1, w, w + 1 };

            bool bOdd = true;
            bool bDel = true;
            bool[] mask = new bool[sizen];
            //memset(mask, 0, sizen);

            while (bDel || bOdd)
            {
                bDel = false;
                for (int i = 1; i < h - 1; i++)
                    for (int j = 1; j < w - 1; j++)
                    {
                        int k = i * w + j;

                        //条件1：p必须是前景点
                        if (outImg[k] != foreground)
                            continue;

                        //条件2：2 <= N(p) <= 6
                        int np = 0;
                        for (int l = 0; l < 8; l++)
                            if (outImg[k + d[l]] != foreground)
                                np++;
                        if (np < 2 || 6 < np)
                            continue;

                        //条件3：S(p) = 1
                        int sp = 0;
                        for (int l = 0; l < 8; l++)
                            if (outImg[k + d[l & 7]] != foreground && outImg[k + d[(l + 1) & 7]] != background)
                                sp++;
                        if (sp != 1)
                            continue;

                        if (bOdd)
                        {
                            //条件4：p2*p0*p6 = 0
                            if (outImg[k + d[2]] != background && outImg[k + d[0]] != background && outImg[k + d[6]] != background)
                                continue;
                            //条件5：p0*p6*p4 = 0
                            if (outImg[k + d[0]] != background && outImg[k + d[6]] != background && outImg[k + d[4]] != background)
                                continue;
                        }
                        else
                        {
                            //条件4：p6*p4*p2==0
                            if (outImg[k + d[6]] != background && outImg[k + d[4]] != background && outImg[k + d[2]] != background)
                                continue;
                            //条件5：p4*p2*p0==0
                            if (outImg[k + d[4]] != background && outImg[k + d[2]] != background && outImg[k + d[0]] != background)
                                continue;
                        }
                        bDel = true;
                        mask[k] = true; //标记删除
                    }

                bOdd = !bOdd;
                if (!bDel)
                    continue;
                //将标记删除的点置为背景色
                int[] copy = new int[outImg.Length];
                outImg.CopyTo(copy, 0);
                for (int i = 0; i < sizen; ++i)
                {
                    if (mask[i] && !undeletable[i])
                    {
                            outImg[i] = background;
                    }
                }
                bool changed = false;
                for (int i = 0; i < outImg.Length; i++)
                {
                    if (outImg[i] != copy[i])
                    {
                        changed = true;
                        break;
                    }
                }
                if(!changed)
                {
                    bDel = false;
                }
            }

            //delete[] mask;

            for (int i = 0; i < sizen; i++)
                outImg[i] *= 255;
            return outImg;
        }

    }
}
