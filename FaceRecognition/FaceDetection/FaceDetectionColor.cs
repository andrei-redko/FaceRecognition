using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FaceRecognition.FaceDetection
{
    public class FaceDetectionColor : FaceDetectionBase
    {
        private int w, h;
        private int[,] red, green, blue;
        private double[] medianRed, medianGreen, medianBlue;
        private double[] stdRed, stdGreen, stdBlue;
        private bool[,] mask;

        public FaceDetectionColor(ImageSource imageSource) :
            base(imageSource)
        {
            w = image.Width;
            h = image.Height;
            red = new int[w, h];
            green = new int[w, h];
            blue = new int[w, h];
            RGBSetup();
            medianRed = new double[h];
            medianGreen = new double[h];
            medianBlue = new double[h];
            stdRed = new double[h];
            stdGreen = new double[h];
            stdBlue = new double[h];
            for (int i = 0; i < h; i++)
            {
                medianRed[i] = MedianRow(red.GetRow(i), w);
                medianGreen[i] = MedianRow(green.GetRow(i), w);
                medianBlue[i] = MedianRow(blue.GetRow(i), w);
                stdRed[i] = STDRow(red.GetRow(i));
                stdGreen[i] = STDRow(green.GetRow(i));
                stdBlue[i] = STDRow(blue.GetRow(i));
            }
            mask = new bool[w, h];
            SkinColorMask();
            SkinColorBitmap();
        }

        private void RGBSetup()
        {
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    System.Drawing.Color pxl = image.GetPixel(i, j);
                    red[i, j] = (int)pxl.R;
                    blue[i, j] = pxl.B;
                    green[i, j] = pxl.G;
                }
            }
        }

        private double MedianRow(int[] array, int height)
        {
            double median = 0;
            if (height%2 == 0)
            {
                median = (array[height/2 - 1] + array[height/2 + 1])/2;
            }
            else
            {
                median = array[(int) Math.Floor((double) height/2)];
            }
            return median;
        }

        private double STDRow(int[] array)
        {
            double average = array.Average();
            double sumOfSquaresOfDifferences = array.Select(
                val => (val - average)*(val - average)).Sum();
            return Math.Sqrt(sumOfSquaresOfDifferences/array.Length);
        }

        private void SkinColorMask()
        {
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    if (Math.Abs(red[i, j] - medianRed[j]) < stdRed[j] &&
                        Math.Abs(blue[i, j] - medianBlue[j]) < stdBlue[j] &&
                        Math.Abs(green[i, j] - medianGreen[j]) < stdGreen[j] )
                    {
                    //if (red[i, j] > 220 && blue[i, j] > 170 && green[i, j] > 210 &&
                    //    Math.Abs(red[i, j] - green[i, j]) <= 15 && red[i, j] > green[i, j] && 
                    //    green[i, j] > blue[i, j])
                    //{
                    //if (red[i, j] > 95 && blue[i, j] > 20 && green[i, j] > 40 &&
                    //    Math.Abs(red[i, j] - green[i, j]) <= 15 && red[i, j] > green[i, j] &&
                    //    red[i, j] > blue[i, j])
                    //{
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
                        maskBMP.SetPixel(i, j, System.Drawing.Color.Black);
                    }
                    else
                    {
                        maskBMP.SetPixel(i, j, System.Drawing.Color.White);
                    }
                }
            }
            maskBMP.Save(String.Concat("Masks/", FileName), ImageFormat.Jpeg);
        }
    }
}
