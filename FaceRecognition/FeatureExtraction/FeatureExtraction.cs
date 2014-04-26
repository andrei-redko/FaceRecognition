using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition.FeatureExtraction
{
    public class FeatureExtraction
    {
        public Bitmap Face { get; set; }

        private int w, h;
        private int[,] red, green, blue;
        private bool[,] mask;

        public FeatureExtraction(Bitmap face)
        {
            Face = face;
            
            w = Face.Width;
            h = Face.Height;
            red = new int[w, h];
            green = new int[w, h];
            blue = new int[w, h];
            mask = new bool[w, h];
            RGBSetup();
            SkinColorMask();
            SkinColorBitmap();
        }

        private void RGBSetup()
        {
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    Color pxl = Face.GetPixel(i, j);
                    red[i, j] = (int)pxl.R;
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
                        //if (red[i, j] > 220 && blue[i, j] > 170 && green[i, j] > 210 &&
                        //    Math.Abs(red[i, j] - green[i, j]) <= 15 && red[i, j] > green[i, j] &&
                        //    green[i, j] > blue[i, j])
                        //{
                    if (red[i, j] > 95 && blue[i, j] > 20 && green[i, j] > 40 &&
                        Math.Abs(red[i, j] - green[i, j]) <= 15 && red[i, j] > green[i, j] &&
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
            maskBMP.Save(String.Concat("Masks/", (new Random()).Next(Int32.MaxValue)), ImageFormat.Jpeg);
        }
    }
}
