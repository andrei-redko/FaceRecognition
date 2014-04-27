using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition.FeatureExtraction
{
    public class FeatureExtractionBase
    {
        public Bitmap Face { get; set; }

        private int w, h;
        private int[,] red, green, blue;
        private bool[,] mask;
        private int[,] labels;

        private Bitmap Grayscale;
        private int[,] redGrayscale, greenGrayscale, blueGrayscale;
        private double[] WP, HP;
        private List<Point> points = new List<Point>();

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
            w = Face.Width;
            h = Face.Height;
            red = new int[w, h];
            green = new int[w, h];
            blue = new int[w, h];
            mask = new bool[w, h];

            RGBSetup(Face, red, blue, green);
            SkinColorMask();


            redGrayscale = new int[w, h];
            greenGrayscale = new int[w, h];
            blueGrayscale = new int[w, h];

            Grayscale = new Bitmap(w, h);
            GrayscaleSetup();
            Grayscale.Save(String.Concat("Grayscale/", String.Concat((new Random()).Next(Int32.MaxValue)), ".jpg"),
                ImageFormat.Jpeg);
            RGBSetup(Grayscale, redGrayscale, blueGrayscale, greenGrayscale);

            labels = new int[w, h];
            Labeling(mask, labels);
            SkinColorBitmap();
            WP = new double[w];
            HP = new double[h];
            LuminanceHistogram(WP, HP);
            SearchForPoints(WP, HP);
            PointOnTheFace();
        }

        private void RGBSetup(Bitmap bitmap, int[,] red, int[,] blue, int[,] green)
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

        private void GrayscaleSetup()
        {
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    Grayscale.SetPixel(i, j,
                        Color.FromArgb((int)(0.3 * red[i, j] +
                                              0.59 * green[i, j] + 0.11 * blue[i, j]), green[i, j], blue[i, j]));
                }
            }
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

        private void LuminanceHistogram(double[] WP, double[] HP)
        {
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    if (mask[i, j])
                    {
                        WP[i] += 0.3 * red[i, j] + 0.59 * green[i, j] + 0.11 * blue[i, j];
                    }
                }
            }
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    if (mask[j, i])
                    {
                        HP[i] += 0.3 * red[j, i] + 0.59 * green[j, i] + 0.11 * blue[j, i];
                    }
                }
            }
        }

        private void SearchForPoints(double[] WP, double[] HP)
        {
            List<int> x = new List<int>();
            List<int> y = new List<int>();
            int step = 4;
            for (int i = 0; i < w; i++)
            {
                if (i < (w - step) && i > step)
                {
                    if (WP[i - 1] > WP[i] && WP[i + 1] > WP[i] && (WP[i - step] - WP[i]) > 50 &&
                        (WP[i + step] - WP[i]) > 50)
                    {
                        x.Add(i);
                    }
                }
            }
            for (int i = 0; i < h; i++)
            {
                if (i < (h - step) && i > step)
                {
                    if (HP[i - 1] > HP[i] && HP[i + 1] > HP[i] && (HP[i - step] - HP[i]) > 50 &&
                        (HP[i + step] - HP[i]) > 50)
                    {
                        y.Add(i);
                    }
                }
            }
            foreach (var pointX in x)
            {
                foreach (var pointY in y)
                {
                    points.Add(new Point
                    {
                        X = pointX,
                        Y = pointY,
                    });
                }
            }
        }

        private void PointOnTheFace()
        {
            foreach (var point in points)
            {
                Face.SetPixel(point.X, point.Y, Color.Red);
                Face.SetPixel(point.X, point.Y - 1, Color.Red);
                Face.SetPixel(point.X, point.Y + 1, Color.Red);
                Face.SetPixel(point.X - 1, point.Y - 1, Color.Red);
                Face.SetPixel(point.X - 1, point.Y, Color.Red);
                Face.SetPixel(point.X - 1, point.Y + 1, Color.Red);
                Face.SetPixel(point.X + 1, point.Y - 1, Color.Red);
                Face.SetPixel(point.X + 1, point.Y, Color.Red);
                Face.SetPixel(point.X + 1, point.Y + 1, Color.Red);
            }
            Face.Save(String.Concat("PointOnTheFace/", String.Concat((new Random()).Next(Int32.MaxValue)), ".jpg"),
                ImageFormat.Jpeg);
        }
    }
}
