using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using FaceRecognition.Help;

namespace FaceRecognition.FeatureExtraction
{
    public class FeatureExtractionBase
    {
        public Bitmap Face { get; set; }

        #region

        private int w, h;
        private bool[,] mask;
        private int[,] labels;

        private List<Area> areas = new List<Area>();
        private List<Point> points = new List<Point>();

        #endregion

        public FeatureExtractionBase(Bitmap face)
        {
            Face = face;
            SetupParameters();
        }

        public FeatureExtractionBase(String name)
        {
            Face = new Bitmap(name);
            SetupParameters();
        }

        private void SetupParameters()
        {
            Rectangle faceRect = new Rectangle(0, (int) (Face.Height/7),
                Face.Width, (int) (5*Face.Height/7));
            Face = Face.Clone(faceRect, Face.PixelFormat);

            w = Face.Width;
            h = Face.Height;
            
            mask = new bool[w, h];

            SelectionAreas();

            SearchPoint();
        }

        /* hhhhhhhhh
        private void RGBSetup(Bitmap bitmap, byte[,] red, byte[,] blue, byte[,] green)
        {
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    Color pxl = bitmap.GetPixel(i, j);
                    red[i, j] = pxl.R;
                    blue[i, j] = pxl.B;
                    green[i, j] = pxl.G;
                }
            }
        }

        private void SkinColorMask()
        {
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    if (red[i, j] > 95 && blue[i, j] > 20 && green[i, j] > 40 &&
                        Math.Abs(red[i, j] - green[i, j]) > 15 && red[i, j] > green[i, j] &&
                        red[i, j] > blue[i, j])
                    {
                        mask[i, j] = true;
                    }
                    else
                    {
                        mask[i, j] = false;
                    }
                }
            }
        }

        private void SkinColorBitmap()
        {
            Bitmap maskBMP = new Bitmap(w, h);
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    if (mask[i, j])
                    {
                        maskBMP.SetPixel(i, j, Color.Black);
                    }
                    else
                    {
                        maskBMP.SetPixel(i, j, Color.White);
                    }
                }
            }
            maskBMP.Save(String.Concat("Masks/", String.Concat((new Random()).Next(Int32.MaxValue)), ".jpg"),
                ImageFormat.Jpeg);
        }

        private void Labeling(bool[,] mask, int[,] labels)
        {
            int L = 1;
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    Fill(mask, labels, i, j, L++);
                }
            }
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    if (labels[i, j] > 1 && !mask[i, j])
                    {
                        mask[i, j] = true;
                    }
                }
            }
        }

        private void Fill(bool[,] mask, int[,] labels, int x, int y, int L)
        {
            if (labels[x, y] == 0 && !mask[x, y])
            {
                labels[x, y] = L;
                if (x > 0)
                    Fill(mask, labels, x - 1, y, L);
                if (x < w - 1)
                    Fill(mask, labels, x + 1, y, L);
                if (y > 0)
                    Fill(mask, labels, x, y - 1, L);
                if (y < h - 1)
                    Fill(mask, labels, x, y + 1, L);
            }
        }

        public void SearchForObjects()
        {
            List<List<int>> aa = new List<List<int>>();
            bool flag = false;
            for (int i = 0; i < h-1; i++)
            {
                List<int> a = new List<int>();
                for (int j = 0; j < w; j++)
                {
                    //if (mask[j, i] != mask[j, i + 1])
                    //{
                    //    a.Add(j);
                    //    flag = false;
                    //}
                    if (!flag)
                    {
                        if (!mask[j, i] && mask[j, i + 1])
                        {
                            a.Add(j);
                            flag = false;
                        }
                    }
                    else
                    {
                        if (mask[j, i] && !mask[j, i + 1])
                        {
                            a.Add(j);
                            flag = true;
                        }
                    }
                        //labels[i, j] = !mask[i, j] ? 1 : 0;
                }
                aa.Add(a);
            }
            int yyyy = 0;
            foreach (var y in aa)
            {
                if (y.Count()%2 == 1)
                {
                    y.RemoveAt(y.Count() - 1);
                }
                for (int i = 0; i < y.Count; i=i+1)
                {
                    int l = i;
                    int r = i != (y.Count-1) ? i + 1 : i;
                    for (int index = y[l]; index < y[r]; index++)
                    {
                        mask[index, yyyy] = true;
                    }
                }
                yyyy++;
            }

            int kn = 0, km = 0;
            int cur = 1;
            int A, B, C;
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    kn = j - 1;
                    if (kn <= 0)
                    {
                        kn = 0;
                        B = 0;
                    }
                    else
                    {
                        B = labels[i, kn];
                    }
                    km = i - 1;
                    if (km <= 0)
                    {
                        km = 0;
                        C = 0;
                    }
                    else
                    {
                        C = labels[km, j];
                    }
                    A = labels[i, j];
                    if (A == 0)
                    {
                        continue;
                    }
                    else
                    {
                        if (B == 0 && C == 0)
                        {
                            cur++;
                            labels[i, j] = cur;
                        }
                        if (B != 0 && C == 0)
                        {
                            labels[i, j] = B;
                        }
                        if (B == 0 && C != 0)
                        {
                            labels[i, j] = C;
                        }
                        if (B != 0 && C != 0)
                        {
                            labels[i, j] = B;
                        }
                    }
                }
            }
        }
        */

        private void PointOnTheFace(Bitmap bitmap, List<Point> points)
        {
            foreach (var point in points)
            {
                bitmap.SetPixel(point.X, point.Y, Color.White);
                bitmap.SetPixel(point.X, point.Y - 1, Color.Red);
                bitmap.SetPixel(point.X, point.Y + 1, Color.Red);
                bitmap.SetPixel(point.X - 1, point.Y, Color.Red);
                bitmap.SetPixel(point.X - 1, point.Y - 1, Color.Red);
                bitmap.SetPixel(point.X - 1, point.Y + 1, Color.Red);
                bitmap.SetPixel(point.X + 1, point.Y, Color.Red);
                bitmap.SetPixel(point.X + 1, point.Y - 1, Color.Red);
                bitmap.SetPixel(point.X + 1, point.Y + 1, Color.Red);
            }
            bitmap.Save(String.Concat("PointOnTheFace/", 
                String.Concat((new Random()).Next(Int32.MaxValue)), ".jpg"),
                ImageFormat.Jpeg);
        }

        private void SelectionAreas()
        {
            Rectangle rectagle = new Rectangle(0, (int) (Face.Height/7),
                (int) (Face.Width/2), (int) (3*Face.Height/7));
            Area area = new Area(Face.Clone(rectagle, Face.PixelFormat), 
                rectagle.X, rectagle.Y, rectagle.Width, rectagle.Height);
            areas.Add(area);
            rectagle = new Rectangle((int)(Face.Width / 2), (int)(Face.Height / 7),
                (int)(Face.Width / 2), (int)(3 * Face.Height / 7));
            area = new Area(Face.Clone(rectagle, Face.PixelFormat),
                rectagle.X, rectagle.Y, rectagle.Width, rectagle.Height);
            areas.Add(area);
            rectagle = new Rectangle((int)(Face.Width / 4), (int)(3*Face.Height / 7),
                (int)(Face.Width / 2), (int)(3 * Face.Height / 7));
            area = new Area(Face.Clone(rectagle, Face.PixelFormat),
                rectagle.X, rectagle.Y, rectagle.Width, rectagle.Height);
            areas.Add(area);
            rectagle = new Rectangle((int)(Face.Width / 4), (int)(6 * Face.Height / 8),
                (int)(Face.Width / 2), (int)(2 * Face.Height / 8));
            area = new Area(Face.Clone(rectagle, Face.PixelFormat),
                rectagle.X, rectagle.Y, rectagle.Width, rectagle.Height);
            areas.Add(area);

            int aaa = (new Random()).Next(Int32.MaxValue-10);
            foreach (var a in areas)
            {
                a.AreaFace.Save(String.Concat("Areas/", aaa++, ".jpg"), ImageFormat.Jpeg);
            }
        }

        private void SearchPoint()
        {
            int index = 0;
            foreach (var area in areas)
            {
                //SearchPointOnArea(area, index++);
                SearchBox(area, index++);
            }
            PointOnTheFace(Face, points);
        }

        private void SearchBox(Area area, int index)
        {
            int box = 4;
            List<List<double>> dx = new List<List<double>>();
            for (int i = box/2; i < (area.Width - box/2); i++)
            {
                List<double> dxi = new List<double>();
                for (int j = box/2; j < (area.Height - box/2); j++)
                {
                    double dxij = 0;
                    for (int iBox = i; iBox < (i + box/2); iBox++)
                    {
                        for (int jBox = j; jBox < (j + box/2); jBox++)
                        {
                            Color px = area.AreaFace.GetPixel(iBox, jBox);
                            byte cb = YCbCrColor.GetCb(px);
                            byte cr = YCbCrColor.GetCr(px);
                            dxij += cb*cb + cr*cr;
                            //dxij += 0.3 * px.R + 0.59 * px.G + 0.11 * px.B;
                        }
                    }
                    dxi.Add(dxij);
                }
                dx.Add(dxi);
            }
            SearchMinBox(area, dx, index, box);
        }

        private void SearchMinBox(Area area, List<List<double>> dx, int index, int box)
        {
            if (index == 0 || index == 1)
            {

            }
            else
            {
                if (index == 2)
                {
                    int y = dx[(area.Width - box)/2-1].IndexOf(dx[(area.Width - box)/2-1].Min());
                    points.Add(new Point(area.Width/2 + area.X, y + area.Y - box/2));
                }
                else
                {
                    List<double> colum = dx[(area.Width - box)/2 - 1];
                    int y = dx[(area.Width - box)/2-1].IndexOf(dx[(area.Width - box)/2-1].Min());
                    points.Add(new Point(area.Width/2 + area.X, y + area.Y - box/2));
                }
            }
        }

        private void SearchPointOnArea(Area area, int index)
        {
            double[] wp = new double[area.Width];
            double[] hp = new double[area.Height];

            //for (int i = 0; i < area.Width; i++)
            //{
            //    for (int j = 0; j < area.Height; j++)
            //    {
            //        Color px = area.AreaFace.GetPixel(i, j);
            //        byte cb = YCbCrColor.GetCb(px);
            //        byte cr = YCbCrColor.GetCr(px);
            //        wp[i] += cb * cb + cr * cr;
            //    }
            //}
            //for (int i = 0; i < area.Height; i++)
            //{
            //    for (int j = 0; j < area.Width; j++)
            //    {
            //        Color px = area.AreaFace.GetPixel(j, i);
            //        byte cb = YCbCrColor.GetCb(px);
            //        byte cr = YCbCrColor.GetCr(px);
            //        hp[i] += cb * cb + cr * cr;
            //    }
            //}
            for (int i = 0; i < area.Width; i++)
            {
                for (int j = 0; j < area.Height; j++)
                {
                    Color px = area.AreaFace.GetPixel(i, j);
                    wp[i] += 0.3*px.R + 0.59*px.G + 0.11*px.B;
                }
            }
            for (int i = 0; i < area.Height; i++)
            {
                for (int j = 0; j < area.Width; j++)
                {
                    Color px = area.AreaFace.GetPixel(j, i);
                    hp[i] += 0.3 * px.R + 0.59 * px.G + 0.11 * px.B;
                }
            }
            //for (int i = 0; i < area.Width; i++)
            //{
            //    double[] line = new double[area.Height];
            //    for (int j = 0; j < area.Height; j++)
            //    {
            //        Color px = area.AreaFace.GetPixel(i, j);
            //        byte cb = YCbCrColor.GetCb(px);
            //        byte cr = YCbCrColor.GetCr(px);
            //        line[j] += cb * cb + cr * cr;
            //    }
            //    double mx = line.Sum() / line.Count();
            //    for (int j = 0; j < area.Height; j++)
            //    {
            //        wp[i] += (line[j] - mx) * (line[j] - mx);
            //    }
            //    wp[i] = wp[i] / (line.Count() - 1);
            //}
            //for (int i = 0; i < area.Height; i++)
            //{
            //    double[] line = new double[area.Width];
            //    for (int j = 0; j < area.Width; j++)
            //    {
            //        Color px = area.AreaFace.GetPixel(j, i);
            //        byte cb = YCbCrColor.GetCb(px);
            //        byte cr = YCbCrColor.GetCr(px);
            //        line[j] += cb * cb + cr * cr;
            //    }
            //    double mx = line.Sum() / line.Count();
            //    for (int j = 0; j < area.Width; j++)
            //    {
            //        hp[i] += (line[j] - mx) * (line[j] - mx);
            //    }
            //    hp[i] = hp[i] / (line.Count() - 1);
            //}
            SearchMin(area, wp, hp, index);
        }

        private void SearchMin(Area area, double[] wp, double[] hp, int index)
        {
            if (index == 0 || index == 1)
            { 
                int[] bb = Index2Min(hp, 10);
                points.Add(new Point((int)(area.X + IndexMin(wp)),
                    area.Y + bb[0]));
                points.Add(new Point((int)(area.X + IndexMin(wp)),
                    area.Y + bb[1]));
            }
            else
            {
                if (index == 2)
                {
                    int[] bb = Index2Min(wp, 10);
                    points.Add(new Point((int)(area.X + bb[0]),
                        area.Y + IndexMin(hp)));
                    points.Add(new Point((int)(area.X + bb[1]),
                        area.Y + IndexMin(hp)));
                }
                else
                {
                    points.Add(new Point((int)(area.X + area.Width / 2),
                        area.Y + IndexMin(hp)));
                }
            }
            //if (index == 0 || index == 1)
            //{
            //    int[] bb = IndexMaxNull(hp, 10);
            //    points.Add(new Point((int)(IndexMax(wp)),
            //         bb[0]));
            //    points.Add(new Point((int)(IndexMax(wp)),
            //        bb[1]));
            //}
            //else
            //{
            //    if (index == 2)
            //    {
            //        int[] bb = IndexMaxNull(wp, 10);
            //        points.Add(new Point((int)(bb[0]),
            //            IndexMax(hp)));
            //        points.Add(new Point((int)(bb[1]),
            //            IndexMax(hp)));
            //    }
            //    else
            //    {
            //        points.Add(new Point((int)(area.Width / 2),
            //             IndexMax(hp)));
            //    }
            //}
            //PointOnTheFace(area.AreaFace, points);
            //points = new List<Point>();
        }

        private int IndexMax(double[] array)
        {
            double max = array[0];
            int indexMax = 0;
            for (int i = 1; i < array.Count(); i++)
            {
                if (max < array[i])
                {
                    max = array[i];
                    indexMax = i;
                }
            }
            return indexMax;
        }
        private int[] Index2Max(double[] array, int offset)
        {
            double[] max = new[] {array[0], array[0]};
            int[] indexMax = new[] {0, 0};
            for (int i = 1; i < array.Count(); i++)
            {
                if (max[0] < array[i])
                {
                    max[0] = array[i];
                    indexMax[0] = i;
                }
            }
            for (int i = 1; i < array.Count(); i++)
            {
                if (max[1] < array[i] && i != indexMax[0]
                    && Math.Abs(i - indexMax[0]) > offset)
                {
                    max[1] = array[i];
                    indexMax[1] = i;
                }
            }
            return indexMax;
        }

        private int IndexMin(double[] array)
        {
            double max = array[0];
            int indexMax = 0;
            for (int i = 1; i < array.Count(); i++)
            {
                if (max > array[i])
                {
                    max = array[i];
                    indexMax = i;
                }
            }
            return indexMax;
        }
        private int[] Index2Min(double[] array, int offset)
        {
            double[] max = new[] { array[0], array[0] };
            int[] indexMax = new[] { 0, 0 };
            for (int i = 1; i < array.Count(); i++)
            {
                if (max[0] > array[i])
                {
                    max[0] = array[i];
                    indexMax[0] = i;
                }
            }
            for (int i = 1; i < array.Count(); i++)
            {
                if (max[1] > array[i] && i != indexMax[0]
                    && Math.Abs(i - indexMax[0]) > offset)
                {
                    max[1] = array[i];
                    indexMax[1] = i;
                }
            }
            return indexMax;
        }

        private int[] IndexMin(double[,] array)
        {
            double max = array[0,0];
            int[] indexMax = new[] {0, 0};
            for (int i = 1; i < array.GetLength(0); i++)
            {
                for (int j = 1; j < array.GetLength(1); j++)
                {
                    if (max > array[i,j])
                    {
                        max = array[i,j];
                        indexMax = new[] { i, j };
                    }
                }
            }
            return indexMax;
        }
        private int[] Index2Min(double[,] array, int offset)
        {
            //double[] max = new[] { array[0], array[0] };
            int[] indexMax = new[] { 0, 0 };
            //for (int i = 1; i < array.Count(); i++)
            //{
            //    if (max[0] > array[i])
            //    {
            //        max[0] = array[i];
            //        indexMax[0] = i;
            //    }
            //}
            //for (int i = 1; i < array.Count(); i++)
            //{
            //    if (max[1] > array[i] && i != indexMax[0]
            //        && Math.Abs(i - indexMax[0]) > offset)
            //    {
            //        max[1] = array[i];
            //        indexMax[1] = i;
            //    }
            //}
            return indexMax;
        }
    }
}
