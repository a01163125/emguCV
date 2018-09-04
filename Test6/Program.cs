using PublicClassLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test6//Test for count
{
    class Program
    {
        static void Main(string[] args)
        {
            string folder = @"C:\Users\xiejing\Desktop\4\HN\WN\2431\";
            DirectoryInfo TheFolder = new DirectoryInfo(folder);
            DirectoryInfo[] di = TheFolder.GetDirectories();
            for (int i = 0; i < di.Length; i++)
            {
                //string s = @"D:\Ted\新建文件夹 (4)";
                abc(di[i].FullName);
            }
        }
        static void abc(string sss)
        {
            DirectoryInfo TheFolder = new DirectoryInfo(sss);
            FileInfo[] fi = TheFolder.GetFiles();
            if (fi.Length > 0)
            {
                int max = 0;
                int min = 99999;
                int std = 0;
                for (int i = 0; i < fi.Length - 1; i++)
                {
                    Bitmap bm = new Bitmap(fi[i].FullName);
                    Bitmap bn = new Bitmap(fi[i + 1].FullName);

                    int s = getCount(bm, 600);
                    int[,] aab = BMP2Matrix(bm, 600);
                    int[,] aac = BMP2Matrix(bn, 600);
                    int t = getCount(aab);
                    if (s > max)
                    {
                        max = s;
                    }
                    if (s < min)
                    {
                        min = s;
                    }
                    if (fi[i].Name.StartsWith("-100"))
                    {
                        std = s;
                    }


                    int r1 = PicOperator.Compare_Naive(aab, aac);
                    int r2 = PicOperator.Compare_Naive(bm, bn);
                    if (r1 != r2)
                    {
                        Console.WriteLine("r1 : " + r1);
                        Console.WriteLine("r2 : " + r2);
                        bool a = temp(bm, aab);
                        bool b = temp(bn, aac);
                    }
                    bm.Dispose();
                }

                //Console.WriteLine(Math.Abs(min - std) / (double)std);
                //Console.WriteLine(Math.Abs(max - std) / (double)std);

            }
        }
        static int getCount(Bitmap bm, int T)
        {
            int count = 0;
            int w = bm.Width;
            int h = bm.Height;
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    Color c = bm.GetPixel(i, j);
                    if (c.R + c.G + c.B < T)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        static int getCount(int[,] matrix)
        {
            int count = 0;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j] == 1)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        static int[,] BMP2Matrix(Bitmap bm, int T)
        {
            int w = bm.Width;
            int h = bm.Height;
            int[,] result = new int[w, h];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    Color c = bm.GetPixel(i, j);
                    if (c.R + c.G + c.B < T)
                    {
                        result[i, j] = 1;
                    }
                }
            }
            return result;
        }
        static bool temp(Bitmap bm, int[,] bn)
        {
            bool result = true;
            if (bm.Width != bn.GetLength(0) || bm.Height != bn.GetLength(1))
            {
                return false;
            }
            for (int i = 0; i < bm.Width; i++)
            {
                for (int j = 0; j < bm.Height; j++)
                {
                    Color c = bm.GetPixel(i, j);
                    int a = 0;
                    if (c.R + c.B + c.G < 600)
                    {
                        a = 1;
                    }
                    if (a != bn[i, j])
                    {

                        return false;
                    }
                }
            }
            return result;
        }
    }
}
