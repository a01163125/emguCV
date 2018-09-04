using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//The codes are translated from c++
namespace Test3
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
        static void show(Bitmap bm)
        { }
        static void Main(string[] args)
        {
            Bitmap pImageBuffer;
            pImageBuffer = new Bitmap(@"C:\Users\xiejing\Desktop\U1grey.bmp");

            //灰度图像
            Bitmap srcImg;
            srcImg = new Bitmap(pImageBuffer.Width, pImageBuffer.Height);
            srcImg = GetGrayImage(pImageBuffer);
            //srcImg.Save(@"C:\Users\xiejing\Desktop\thin\U23Grey.bmp");
            int w = srcImg.Width;
            int h = srcImg.Height;
            int n = w * h;

            //二值化图像
            srcImg = GetBinaryzationImage1(srcImg, 180);
            //srcImg.Save(@"C:\Users\xiejing\Desktop\thin\1111U2318.bmp");
            //待细化图像
            int[] inputImg = new int[n];
            srcImg = reverseColor(srcImg);
            inputImg = toBinaryArray(srcImg);
            
            //显示图像
            Bitmap imgshow;
            imgshow = new Bitmap(w, h);


            //Zhang并行快速算法
            int[] ZhangFastImg = new int[n];
            ZhangFastImg = ZhangFastThin(inputImg, w, h);
            imgshow = toBmp(ZhangFastImg, w, h);
            show(imgshow);
            imgshow.Save(@"C:\Users\xiejing\Desktop\thin\zhangoutU2318.bmp");
            inputImg = toBinaryArray(srcImg);

            //Hilditch算法
            int[] HilditchImg = new int[n];
            HilditchImg = HilditchThin(inputImg, w, h);
            imgshow = toBmp(HilditchImg, w, h);
            show(imgshow);
            imgshow.Save(@"C:\Users\xiejing\Desktop\thin\hilditchoutU2318.bmp");
            inputImg = toBinaryArray(srcImg);


            //Pavlidis算法
            int[] PavlidisImg = new int[n];
            PavlidisImg = PavlidisThin(inputImg, w, h);
            imgshow = toBmp(PavlidisImg, w, h);
            show(imgshow);
            imgshow.Save(@"C:\Users\xiejing\Desktop\thin\pavlidisoutU2318.bmp");
            inputImg = toBinaryArray(srcImg);

            //Rosenfeld算法
            int[] RosenfeldImg = new int[n];
            RosenfeldImg = RosenfeldThin(inputImg, w, h);
            imgshow = toBmp(RosenfeldImg, w, h);
            show(imgshow);
            imgshow.Save(@"C:\Users\xiejing\Desktop\thin\renfeldoutU2318.bmp");
            inputImg = toBinaryArray(srcImg);

            //BasedIndexTableThin算法
            int[] BasedIndexTableImg = new int[n];
            BasedIndexTableImg = BasedIndexTableThin(inputImg, w, h);
            imgshow = toBmp(BasedIndexTableImg, w, h);
            imgshow.Save(@"C:\Users\xiejing\Desktop\thin\baseindexoutU2318.bmp");
            inputImg = toBinaryArray(srcImg);

        }
        static int[] ZhangFastThin(int[] inputImg, int w, int h)
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
                for (int i = 0; i < sizen; ++i)
                {
                    if (mask[i])
                        outImg[i] = background;
                }
            }

            //delete[] mask;

            for (int i = 0; i < sizen; i++)
                outImg[i] *= 255;
            return outImg;
        }
        #region HilditchThin
        static int[] HilditchThin(int[] inputImg, int w, int h)
        {
            int sizen = w * h;
            int[] outImg = inputImg;

            int foreground = 1;//前景点为1
            int background = 1 - foreground;//背景点为0

            bool[] mask = new bool[sizen];
            //memset(mask, 0, sizen);

            //8邻域编码
            //  p3  p2  p1 
            //  p4   p   p0 
            //  p5  p6  p7 
            int[] list = new int[8];

            bool loop = true;//循环标志

            while (loop)
            {
                loop = false;

                for (int i = 1; i < h - 1; i++)
                {
                    for (int j = 1; j < w - 1; j++)
                    {
                        int k = i * w + j;
                        int p = outImg[k];

                        // 条件1：p 必须是前景点 
                        if (p != foreground) continue;

                        // list 存储补集
                        FillNeighbors(inputImg, k, list, w, foreground);

                        // 条件2：p0,p2,p4,p6 不皆为前景点 
                        if (list[0] == 0 && list[2] == 0 && list[4] == 0 && list[6] == 0)
                            continue;

                        // 条件3: p0~p7至少两个是前景点 
                        int count = 0;
                        for (int l = 0; l < 8; l++)
                        {
                            count += list[l];
                        }

                        if (count > 6) continue;

                        // 条件4：联结数等于1 
                        if (DetectConnectivity(list) != 1) continue;

                        // 条件5: 假设p2已标记删除，则令p2为背景，不改变p的联结数 
                        if (mask[(i - 1) * w + j])
                        {
                            list[2] = 1;
                            if (DetectConnectivity(list) != 1)
                                continue;
                            list[2] = 0;
                        }

                        // 条件6: 假设p4已标记删除，则令p4为背景，不改变p的联结数 
                        if (mask[i * w + j - 1])
                        {
                            list[4] = 1;
                            if (DetectConnectivity(list) != 1)
                                continue;
                        }
                        mask[i * w + j] = true; // 标记删除 
                        loop = true;
                    }
                }

                //如果标记删除，则像元置0
                for (int i = 0; i < sizen; i++)
                {
                    if (mask[i])
                    {
                        outImg[i] = background;
                    }
                }
            }

            for (int i = 0; i < sizen; i++)
                outImg[i] *= 255;
            return outImg;
        }
        static void FillNeighbors(int[] p, int index, int[] list, int width, int foreground)
        {
            // list 存储的是补集，即前景点为0，背景点为1，以方便联结数的计算 

            list[0] = p[index + 1] == foreground ? 0 : 1;
            list[1] = p[index + 1 - width] == foreground ? 0 : 1;
            list[2] = p[index + -width] == foreground ? 0 : 1;
            list[3] = p[index + -1 - width] == foreground ? 0 : 1;
            list[4] = p[index + -1] == foreground ? 0 : 1;
            list[5] = p[index + -1 + width] == foreground ? 0 : 1;
            list[6] = p[index + width] == foreground ? 0 : 1;
            list[7] = p[index + 1 + width] == foreground ? 0 : 1;
        }
        static int DetectConnectivity(int[] list)
        {
            int count = list[6] - list[6] * list[7] * list[0];
            count += list[0] - list[0] * list[1] * list[2];
            count += list[2] - list[2] * list[3] * list[4];
            count += list[4] - list[4] * list[5] * list[6];
            return count;
        }
        #endregion
        #region Pavlidis
        static void FillNeighbor8(int[] p, int index, ref int[] list, int w)
        {
            list[0] = p[index + 1];
            list[1] = p[index + -w + 1];
            list[2] = p[index + -w];
            list[3] = p[index + -w - 1];
            list[4] = p[index + -1];
            list[5] = p[index + w - 1];
            list[6] = p[index + w];
            list[7] = p[index + w + 1];
        }
        static int[] PavlidisThin(int[] inputImg, int w, int h)
        {
            int sizen = w * h;
            int[] outImg = inputImg;

            //8邻域编码
            //p3  p2  p1
            //p4  p   p0
            //p5  p6  p7
            int[] list = new int[8];

            //图像边框像素值为0
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                {
                    int ij = i * w + j;

                    if (i == 0 || i == (h - 1) || j == 0 || j == (w - 1))
                        outImg[ij] = 0;
                }

            int bdr1, bdr2, bdr4, bdr5;
            int b;

            //循环标志
            bool loop = true;

            while (loop)
            {
                int img = 0;
                //第一个循环，取得前景轮廓，轮廓用2表示
                for (int i = 1; i < h - 1; i++)
                {
                    img += w;
                    for (int j = 1; j < w - 1; j++)
                    {
                        int p = outImg[img + j];

                        if (p != 1)
                            continue;

                        FillNeighbor8(inputImg, img + j, ref list, w);

                        //bdr1是2进制表示的p0...p6p7排列，10000011,p0=1,p6=p7=1
                        bdr1 = 0;
                        for (int k = 0; k < 8; k++)
                        {
                            if (list[k] >= 1)
                                bdr1 |= 0x80 >> k;
                        }

                        //内部点;p0, p2, p4, p6都是为1, 非边界点，所以继续循环
                        //0xaa
                        //  0   1   0   
                        //  1         1
                        //  0   1    0
                        if ((bdr1 & 0xaa) == 0xaa)
                            continue;

                        //不是内部点，则是边界点(轮廓), 标记为2
                        outImg[img + j] = 2;

                        b = 0;

                        for (int k = 0; k <= 7; k++)
                        {
                            b += bdr1 & (0x80 >> k);
                        }
                        //在边界点中，等于1，则是端点，等于0，则是孤立点，此时标记3
                        if (b <= 1)
                            outImg[img + j] = 3;

                        //此条件说明p点是中间点，移去会引起断裂
                        // 0x70        0x7         0x88      0xc1        0x1c      0x22      0x82     0x1      0xa0     0x40     0x28    0x10       0xa      0x4
                        // 0 0 0     0  1  1     1  0   0    0   0   0    1  1  0    0   0   1  0  0  1  0 0 0    0  0  0   0 0 0    1  0  0   0  0  0  1  0  1   0 1 0
                        // 1   0     0     1     0      0    0       1    1     0    0       0  0     0  0   1    0     0   0   0    0     0   1     0  0     0   0    0
                        // 1 1 0     0  0  0     0  0   1    0   1   1    0  0  0    1   0   1  0  0  1  0 0 0    1  0  1   0 1 0    1  0  0   0  0  0  0  0  0   0 0 0
                        if ((bdr1 & 0x70) != 0 && (bdr1 & 0x7) != 0 && (bdr1 & 0x88) == 0)
                            outImg[img + j] = 3;
                        else if ((bdr1 != 0 && 0xc1 != 0) && (bdr1 & 0x1c) != 0 && (bdr1 & 0x22) == 0)
                            outImg[img + j] = 3;
                        else if ((bdr1 & 0x82) == 0 && (bdr1 & 0x1) != 0)
                            outImg[img + j] = 3;
                        else if ((bdr1 & 0xa0) == 0 && (bdr1 & 0x40) != 0)
                            outImg[img + j] = 3;
                        else if ((bdr1 & 0x28) == 0 && (bdr1 & 0x10) != 0)
                            outImg[img + j] = 3;
                        else if ((bdr1 & 0xa) == 0 && (bdr1 & 0x4) != 0)
                            outImg[img + j] = 3;
                    }
                }

                img = 0;
                for (int i = 1; i < h - 1; i++)
                {
                    img += w;
                    for (int j = 1; j < w - 1; j++)
                    {
                        int p = outImg[img + j];

                        if (p == 0)
                            continue;

                        FillNeighbor8(inputImg, img + j, ref list, w);

                        bdr1 = bdr2 = 0;

                        //bdr1是2进制表示的当前点p的8邻域连通情况，hdr2是当前点周围轮廓点的连接情况
                        for (int k = 0; k <= 7; k++)
                        {
                            if (list[k] >= 1)
                                bdr1 |= 0x80 >> k;
                            if (list[k] >= 2)
                                bdr2 |= 0x80 >> k;
                        }

                        //相等，周围全是值为2的像素，继续
                        if (bdr1 == bdr2)
                        {
                            outImg[img + j] = 4;
                            continue;
                        }

                        //p0不为2，继续
                        if (outImg[img + j] != 2) continue;
                        //=4都是不可删除的轮廓点
                        //   0x80       0xa     0x40        0x1      0x30   0x6
                        //   0 0 0      1  0 1    0  0  0    0  0  0   0 0 0   0 1 1
                        //   0   0      0    0    0     0    0     1   1   0   0   0
                        //   0 0 1      0  0 0    0  1  0    0  0  0   1 0 0   0 0 0

                        if ((bdr2 & 0x80) != 0 && (bdr1 & 0xa) == 0 &&
                                    (((bdr1 & 0x40) != 0 || (bdr1 & 0x1) != 0) && (bdr1 & 0x30) != 0 && (bdr1 & 0x6) != 0))
                        {
                            outImg[img + j] = 4;
                        }
                        else if ((bdr2 & 0x20) != 0 && (bdr1 & 0x2) == 0 &&
                            (((bdr1 & 0x10) != 0 || (bdr1 & 0x40) != 0) && (bdr1 & 0xc) != 0 && (bdr1 & 0x81) != 0))
                        {
                            outImg[img + j] = 4;
                        }
                        else if ((bdr2 & 0x8) != 0 && (bdr1 & 0x80) == 0 &&
                                        (((bdr1 & 0x4) != 0 || (bdr1 & 0x10) != 0) && (bdr1 & 0x3) != 0 && (bdr1 & 0x60) != 0))
                        {
                            outImg[img + j] = 4;
                        }
                        else if ((bdr2 & 0x2) != 0 && (bdr1 & 0x20) == 0 &&
                                    (((bdr1 & 0x1) != 0 || (bdr1 & 0x4) != 0) && (bdr1 & 0xc0) != 0 && (bdr1 & 0x18) != 0))
                        {
                            outImg[img + j] = 4;
                        }
                    }
                }

                img = 0;
                for (int i = 1; i < h - 1; i++)
                {
                    img += w;
                    for (int j = 1; j < w - 1; j++)
                    {
                        int p = outImg[img + j];

                        if (p != 2)
                            continue;

                        FillNeighbor8(inputImg, img + j, ref list, w);

                        bdr4 = bdr5 = 0;
                        for (int k = 0; k <= 7; k++)
                        {
                            if (list[k] >= 4)
                                bdr4 |= 0x80 >> k;
                            if (list[k] >= 5)
                                bdr5 |= 0x80 >> k;
                        }
                        //值为4和5的像素
                        if ((bdr4 & 0x8) == 0)
                        {
                            outImg[img + j] = 5;
                            continue;
                        }
                        if ((bdr4 & 0x20) == 0 && bdr5 == 0)
                        {
                            outImg[img + j] = 5;
                            continue;
                        }

                    }
                }

                loop = false;

                img = 0;
                for (int i = 1; i < h - 1; i++)
                {
                    img += w;
                    for (int j = 1; j < w - 1; j++)
                    {
                        int p = outImg[img + j];
                        if (p == 2 || p == 5)
                        {
                            loop = true;
                            outImg[img + j] = 0;
                        }
                    }
                }
            }

            for (int i = 0; i < sizen; i++)
                outImg[i] *= 255;
            return outImg;
        }
        #endregion
        static int[] RosenfeldThin(int[] inputImg, int w, int h)
        {
            int sizen = w * h;
            int[] outImg = (int[])inputImg.Clone();


            int[] tempImg = new int[sizen];
            inputImg.CopyTo(tempImg, 0);

            //8邻域编码
            //p3  p2  p1
            //p4  p   p0
            //p5  p6  p7
            int[] list = new int[8];

            int[] a = new int[5] { 0, -1, 1, 0, 0 };
            int[] b = new int[5] { 0, 0, 0, 1, -1 };

            int cond, n48, n26, n24, n46, n68, n82, n123, n345, n567, n781;

            //循环标志
            bool loop = true;

            while (loop)
            {
                loop = false;

                for (int k = 1; k <= 4; k++)
                {
                    for (int i = 1; i < h - 1; i++)
                    {
                        int ii = i + a[k];

                        for (int j = 1; j < w - 1; j++)
                        {
                            int ij = i * w + j;
                            int pos = ij;

                            if (outImg[ij] != 1)
                                continue;

                            int jj = j + b[k];
                            int kk1 = ii * w + jj;

                            if (outImg[kk1] == 1)
                                continue;

                            FillNeighbor8(outImg, pos, ref list, w);

                            int nrnd = 0;
                            for (int l = 0; l < 8; l++)
                                nrnd += list[l];

                            if (nrnd <= 1)
                                continue;

                            cond = 0;
                            n48 = list[3] + list[7];
                            n26 = list[1] + list[5];
                            n24 = list[1] + list[3];
                            n46 = list[3] + list[5];
                            n68 = list[5] + list[7];
                            n82 = list[7] + list[1];
                            n123 = list[0] + list[1] + list[2];
                            n345 = list[2] + list[3] + list[4];
                            n567 = list[4] + list[5] + list[6];
                            n781 = list[6] + list[7] + list[0];

                            if (list[1] == 1 && n48 == 0 && n567 > 0)
                            {
                                if (cond != 1)
                                    continue;
                                tempImg[ij] = 0;
                                loop = true;
                                continue;
                            }

                            if (list[5] == 1 && n48 == 0 && n123 > 0)
                            {
                                if (cond != 1)
                                    continue;
                                tempImg[ij] = 0;
                                loop = true;
                                continue;
                            }

                            if (list[7] == 1 && n26 == 0 && n345 > 0)
                            {
                                if (cond != 1)
                                    continue;
                                tempImg[ij] = 0;
                                loop = true;
                                continue;
                            }

                            if (list[3] == 1 && n26 == 0 && n781 > 0)
                            {
                                if (cond != 1)
                                    continue;
                                tempImg[ij] = 0;
                                loop = true;
                                continue;
                            }

                            if (list[4] == 1 && n46 == 0)
                            {
                                if (cond != 1)
                                    continue;
                                tempImg[ij] = 0;
                                loop = true;
                                continue;
                            }

                            if (list[6] == 1 && n68 == 0)
                            {
                                if (cond != 1)
                                    continue;
                                tempImg[ij] = 0;
                                loop = true;
                                continue;
                            }

                            if (list[0] == 1 && n82 == 0)
                            {
                                if (cond != 1)
                                    continue;
                                tempImg[ij] = 0;
                                loop = true;
                                continue;
                            }

                            if (list[2] == 1 && n24 == 0)
                            {
                                if (cond != 1)
                                    continue;
                                tempImg[ij] = 0;
                                loop = true;
                                continue;
                            }

                            cond = 1;

                            tempImg[ij] = 0;

                            loop = true;
                        }
                    }

                    tempImg.CopyTo(outImg, 0);
                }
            }



            for (int i = 0; i < sizen; i++)
                outImg[i] *= 255;
            return outImg;
        }
        static int[] BasedIndexTableThin(int[] inputImg, int w, int h)
        {
            int[] outImg = (int[])inputImg.Clone();
            int[] deletemark = new int[256]{
                0,0,0,0,0,0,0,1,    0,0,1,1,0,0,1,1,
                0,0,0,0,0,0,0,0,    0,0,1,1,1,0,1,1,
                0,0,0,0,0,0,0,0,    1,0,0,0,1,0,1,1,
                0,0,0,0,0,0,0,0,    1,0,1,1,1,0,1,1,
                0,0,0,0,0,0,0,0,    0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,    0,0,0,0,0,0,0,0,
                0,0,0,0,0,0,0,0,    1,0,0,0,1,0,1,1,
                1,0,0,0,0,0,0,0,    1,0,1,1,1,0,1,1,
                0,0,1,1,0,0,1,1,    0,0,0,1,0,0,1,1,
                0,0,0,0,0,0,0,0,    0,0,0,1,0,0,1,1,
                1,1,0,1,0,0,0,1,    0,0,0,0,0,0,0,0,
                1,1,0,1,0,0,0,1,    1,1,0,0,1,0,0,0,
                0,1,1,1,0,0,1,1,    0,0,0,1,0,0,1,1,
                0,0,0,0,0,0,0,0,    0,0,0,0,0,1,1,1,
                1,1,1,1,0,0,1,1,    1,1,0,0,1,1,0,0,
                1,1,1,1,0,0,1,1,    1,1,0,0,1,1,0,0
            };//IndexTable索引表，表示某像元的8领域的256种情况，1删除，0不删除

            int sizen = w * h;
            int[] tempImg = new int[sizen];
            //memset(tempImg, 0, sizen);

            //8领域编码
            //p0  p1  p2
            //p7   p   p3
            //p6  p5  p4
            int[] list = new int[8];

            int foreground = 1;//前景点
            int background = 1 - foreground;//背景点

            //循环标志
            bool loop = true;

            while (loop)
            {
                loop = false;

                tempImg= new int[sizen];

                //首先求边缘点(并行)
                int pmid = w + 1;
                int pmidtemp = w + 1;

                for (int i = 1; i < h - 1; i++)
                {
                    for (int j = 1; j < w - 1; j++)
                    {
                        if (inputImg[pmid] == background)//0，不考虑
                        {
                            pmid++;
                            pmidtemp++;
                            continue;
                        }

                        FillNeighbor8(inputImg, pmid, ref list, w);

                        int sum = 1;
                        for (int k = 0; k < 8; k++)
                            sum &= list[k];

                        if (sum == 0)
                        {
                            tempImg[ pmidtemp] = foreground;//边缘
                        }

                        pmid++;
                        pmidtemp++;
                    }

                    //移动到下一行的第2个点开始
                    pmid = pmid + 2;
                    pmidtemp = pmidtemp + 2;
                }

                //串行删除
                pmid = w + 1;
                pmidtemp = w + 1;

                for (int i = 1; i < h - 1; i++)
                {
                    for (int j = 1; j < w - 1; j++)
                    {
                        if (tempImg[pmidtemp] == background)//1-边缘，0-中间点
                        {
                            pmid++;
                            pmidtemp++;
                            continue;
                        }

                        FillNeighbor8(inputImg, pmid, ref list, w);

                        list[2] *= 2;
                        list[1] *= 4;
                        list[0] *= 8;
                        list[7] *= 16;
                        list[6] *= 32;
                        list[5] *= 64;
                        list[4] *= 128;

                        int sum = 0;
                        for (int k = 0; k < 8; k++)
                            sum |= list[k];

                        if (deletemark[sum] == 1)
                        {
                            inputImg[pmid] = background;
                            loop = true; //本次扫描进行了细化
                        }

                        pmid++;
                        pmidtemp++;
                    }

                    //移动到下一行的第2个点开始
                    pmid = pmid + 2;
                    pmidtemp = pmidtemp + 2;
                }
            }

            //delete[] tempImg;

            for (int i = 0; i < sizen; i++)
                outImg[i] = 255 * inputImg[i];
            return outImg;
        }
        static Bitmap reverseColor(Bitmap whitebase)
        {
            for(int i = 0; i < whitebase.Width;i++)
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

        #region 形态学边缘
        static void FillNeighbor9(int[] p, int index,int[] list, int w)
        {

            list[0] = p[index+1];
            list[1] = p[index + -w + 1];
            list[2] = p[index + -w];
            list[3] = p[index + -w - 1];
            list[4] = p[index + -1];
            list[5] = p[index + w - 1];
            list[6] = p[index + w];
            list[7] = p[index + w + 1];
            list[8] = p[0];
        }
        static int[] MorphologySharp(int[] inputImg, int w, int h)
        {
            int sizen = w * h;
            int[] outImg = (int[])inputImg.Clone();

            int[] tempImg = new int[sizen];
            inputImg.CopyTo(tempImg, 0);

            ////领域编码
            //BYTE list[9];

            //BYTE* pt = tempImg;
            //BYTE* po = outImg;
            //for (int i = 1; i < h - 1; i++)
            //{
            //    for (int j = 1; j < w - 1; j++)
            //    {
            //        po = outImg + i * w + j;
            //        pt = tempImg + i * w + j;
            //        FillNeighbor9(pt, list, w);

            //        int min = 255;
            //        for (int k = 0; k < 9; k++)
            //        {
            //            min = (min >= list[k] ? list[k] : min);
            //        }

            //        (*po) = min;
            //    }
            //}

            ////2*原图-腐蚀=锐化
            //for (int i = 0; i < sizen; i++)
            //    outImg[i] = (BYTE)(2 * inputImg[i] - outImg[i]);

            //delete[] tempImg;
            return outImg;
        }
        #endregion
        static int[] MorphologyThin(int[] inputImg, int w, int h)//This method has not been tested.
        {
            //inputImg:输入的图像
            //img2: 输入图像的拷贝
            //img3:erode后的图像
            //img4:open操作的中间值
            //img5:erode-open后的图像
            //img6:作差后的图像
            //outImg:输出的图像
            int[] outImg = (int[])inputImg.Clone();
            int n = w * h;
            int[] img2 = new int[n];
            int[] img3 = new int[n];
            int[] img4 = new int[n];
            int[] img5 = new int[n];
            int[] img6 = new int[n];

            inputImg.CopyTo(img2, 0);
            outImg= new int[n];

            //loop flag
            bool flag = true;
            //erode flag
            bool flag2 = true;

            while (flag)
            {
                for (int i = 0; i < h; i++)
                    for (int j = 0; j < w; j++)
                    {
                        int ij = i * w + j;
                        if (i == 0 || j == 0 || i == h - 1 || j == w - 1)
                            img3[ij] = 0;
                        else
                        {
                            flag2 = true;
                            for (int r = i - 1; r <= i + 1; r++)
                                for (int s = j - 1; s <= j + 1; s++)
                                {

                                    if (img2[r * w + j] != 1)
                                        flag2 = false;
                                }
                        }

                        if (flag2)
                            img3[ij] = 1;
                        else
                            img3[ij] = 0;
                    }//img3 end

                for (int i = 0; i < h; i++)
                    for (int j = 0; j < w; j++)
                    {
                        int ij = i * w + j;
                        if (i == 0 || j == 0 || i == h - 1 || j == w - 1)
                            img4[ij] = 0;
                        else
                        {
                            flag2 = true;
                            for (int r = i - 1; r <= i + 1; r++)
                                for (int s = j - 1; s <= j + 1; s++)
                                {
                                    if (img3[r * w + s] != 1)
                                        flag2 = false;
                                }

                            if (flag2)
                                img4[ij] = 1;
                            else
                                img4[ij] = 0;
                        }
                    }//img4 end

                for (int i = 0; i < h; i++)
                    for (int j = 0; j < w; j++)
                    {
                        int ij = i * w + j;
                        if (i == 0 || j == 0 || i == h - 1 || j == w - 1)
                            img5[ij] = 0;
                        else
                        {
                            flag2 = true;
                            for (int r = i - 1; r <= i + 1; r++)
                                for (int s = j - 1; s <= j + 1; s++)
                                {
                                    if (img4[r * w + s] != 1)
                                        flag2 = false;
                                }

                            if (flag2)
                                img5[ij] = 1;
                            else
                                img5[ij] = 0;
                        }
                    }//img5 end

                //作差,等同于subtract函数
                for (int i = 0; i < h; i++)
                    for (int j = 0; j < w; j++)
                    {
                        int ij = i * w + j;
                        img6[ij] = img3[ij] - img5[ij];
                    }//img6 end

                //并运算
                for (int i = 0; i < h; i++)
                    for (int j = 0; j < w; j++)
                    {
                        int ij = i * w + j;
                        if (img6[ij] == 1)
                            outImg[ij] = 1;
                    }//img6 end

                img3.CopyTo(img2, 0);

                //判断循环标志
                flag = false;
                for (int i = 0; i < h; i++)
                    for (int j = 0; j < w; j++)
                    {
                        int ij = i * w + j;
                        if (img2[ij] == 1)
                            flag = true;
                    }//img6 end

            }

            for (int i = 0; i < n; i++)
                outImg[i] *= 255;


            //delete[] img2;
            //delete[] img3;
            //delete[] img4;
            //delete[] img5;
            //delete[] img6;
            return outImg;
        }

    }
}
