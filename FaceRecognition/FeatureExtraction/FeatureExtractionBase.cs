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

        private List<Area> areas = new List<Area>();
        public List<Point> points { get; private set; }

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

            points = new List<Point>();

            SelectionAreas();
        }

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
            rectagle = new Rectangle((int) (Face.Width/2), (int) (Face.Height/7),
                (int) (Face.Width/2), (int) (3*Face.Height/7));
            area = new Area(Face.Clone(rectagle, Face.PixelFormat),
                rectagle.X, rectagle.Y, rectagle.Width, rectagle.Height);
            areas.Add(area);
            rectagle = new Rectangle((int) (Face.Width/4), (int) (3*Face.Height/7),
                (int) (Face.Width/2), (int) (3*Face.Height/7));
            area = new Area(Face.Clone(rectagle, Face.PixelFormat),
                rectagle.X, rectagle.Y, rectagle.Width, rectagle.Height);
            areas.Add(area);
            rectagle = new Rectangle((int) (Face.Width/4), (int) (6*Face.Height/8),
                (int) (Face.Width/2), (int) (2*Face.Height/8));
            area = new Area(Face.Clone(rectagle, Face.PixelFormat),
                rectagle.X, rectagle.Y, rectagle.Width, rectagle.Height);
            areas.Add(area);

            int aaa = (new Random()).Next(Int32.MaxValue - 10);
            foreach (var a in areas)
            {
                a.AreaFace.Save(String.Concat("Areas/", aaa++, ".jpg"), ImageFormat.Jpeg);
            }
        }

        public List<Point> SearchPoint()
        {
            int index = 0;
            foreach (var area in areas)
            {
                SearchBox(area, index++);
            }
            PointOnTheFace(Face, points);
            return points;
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
                            //byte cb = YCbCrColor.GetCb(px);
                            //byte cr = YCbCrColor.GetCr(px);
                            //dxij += cb * cb + cr * cr;
                            dxij += 0.3*px.R + 0.59*px.G + 0.11*px.B;
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
                List<double> sumRow = new List<double>();
                for (int i = 0; i < dx[(area.Width - box)/2 - 1].Count(); i++)
                {
                    double sum = 0;
                    foreach (var colum in dx)
                    {
                        sum += colum[i];
                    }
                    sumRow.Add(sum);
                }
                Dictionary<int, double> yMin = new Dictionary<int, double>();
                int minIndex1 = 0;
                double min1 = sumRow[minIndex1];
                for (int i = 1; i < (sumRow.Count - 1); i++)
                {
                    if (sumRow[i - 1] > sumRow[i] && sumRow[i + 1] > sumRow[i])
                    {
                        yMin.Add(i, sumRow[i]);
                        if (sumRow[i] < min1)
                        {
                            min1 = sumRow[i];
                            minIndex1 = i;
                        }
                    }
                }
                var removeIndex = yMin.Where(c => Math.Abs(c.Key - minIndex1) < 10)
                    .Select(c => c.Key).ToList();
                foreach (var remove in removeIndex)
                {
                    yMin.Remove(remove);
                }
                int minIndex2 = yMin.First().Key;
                double min2 = yMin.First().Value;
                foreach (var min in yMin)
                {
                    if (min.Value < min2)
                    {
                        min2 = min.Value;
                        minIndex2 = min.Key;
                    }
                }
                List<double> rowMin = new List<double>();
                foreach (var colum in dx)
                {
                    rowMin.Add(colum[minIndex2] + colum[minIndex1]);
                }
                int x = rowMin.IndexOf(rowMin.Min());

                points.Add(new Point(x + area.X + box/2,
                    minIndex1 + area.Y + box/2));
                points.Add(new Point(x + area.X + box/2,
                    minIndex2 + area.Y + box/2));
            }
            else
            {
                if (index == 2)
                {
                    int y = dx[(area.Width - box)/2 - 1].IndexOf(
                        dx[(area.Width - box)/2 - 1].Min());
                    points.Add(new Point(area.Width/2 + area.X,
                        y + area.Y + box/2));
                }
                else
                {
                    List<double> sumRow = new List<double>();
                    for (int i = 0; i < dx[(area.Width - box)/2 - 1].Count(); i++)
                    {
                        double sum = 0;
                        foreach (var colum in dx)
                        {
                            sum += colum[i];
                        }
                        sumRow.Add(sum);
                    }
                    points.Add(new Point(area.Width/2 + area.X,
                        sumRow.IndexOf(sumRow.Min()) + area.Y + box/2));
                }
            }
        }
    }
}
