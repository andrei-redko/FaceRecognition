using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition.FeatureExtraction
{
    class Area
    {
        public Bitmap AreaFace { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Area(Bitmap area, int x, int y, int width, int height)
        {
            AreaFace = area;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
