using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FaceRecognition.FaceDetection
{
    public class FaceDetectionColor : FaceDetectionBase
    {
        private int w, h;
        private byte[,] red, green, blue;
        private double medianRed, medianGreen, medianBlue;
        private double stdRed, stdGreen, stdBlue;

        public FaceDetectionColor(ImageSource imageSource) :
            base(imageSource)
        {
            red = new byte[w, h];
            green = new byte[w, h];
            blue = new byte[w, h];
            RGBSetup();
        }

        private void RGBSetup()
        {
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    System.Drawing.Color pxl = image.GetPixel(i, j);
                    red[i, j] = pxl.R;
                    blue[i, j] = pxl.B;
                    green[i, j] = pxl.G;
                }
            }
        }
    }
}
