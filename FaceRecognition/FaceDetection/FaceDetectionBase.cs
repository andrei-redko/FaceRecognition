using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FaceRecognition.FaceDetection
{
    public class FaceDetectionBase
    {
        protected ImageSource ImageFace { get; set; }

        protected Bitmap image = null;

        public String FileName { get; private set; }

        public FaceDetectionBase(ImageSource imageSource)
        {
            ImageFace = imageSource;
            image = GetBitmap((BitmapSource)imageSource);
            FileName = String.Concat(DateTime.Now.Millisecond.ToString(),
                DateTime.Now.Second.ToString(), ".jpeg");
        }

        protected void SaveImageSourceToFile(string filePath, 
            ImageSource imageFace)
        {
            using (var fileStream = new FileStream(String.Concat(filePath,
                FileName), FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource) imageFace));
                encoder.Save(fileStream);
            }
        }

        private Bitmap GetBitmap(BitmapSource source)
        {
            Bitmap bmp = new Bitmap(
              source.PixelWidth,
              source.PixelHeight,
              System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(
              new Rectangle(System.Drawing.Point.Empty, bmp.Size),
              ImageLockMode.WriteOnly,
              System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            source.CopyPixels(
              Int32Rect.Empty,
              data.Scan0,
              data.Height * data.Stride,
              data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }

        public virtual void DetectionFace()
        {
            SaveImageSourceToFile("DetectionFace/", ImageFace);
        }

        public virtual Bitmap DetectionFaceGetBitmap()
        {
            return GetBitmap((BitmapSource)ImageFace);
        }
    }
}
