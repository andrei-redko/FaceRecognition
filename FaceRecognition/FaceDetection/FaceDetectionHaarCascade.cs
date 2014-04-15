using System.Drawing;
using System.Drawing.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FaceRecognition.FaceDetection
{
    public class FaceDetectionHaarCascade : FaceDetectionBase
    {
        public FaceDetectionHaarCascade(ImageSource imageSource) : 
            base(imageSource)
        {
        }

        public override void DetectionFace()
        {
            SaveImageSourceToFile("Original/", ImageFace);
            Image<Bgr, Byte> currentFrame = new Image<Bgr, byte>(image);

            if (currentFrame != null)
            {
                Image<Gray, Byte> grayFrame = currentFrame.Convert<Gray, Byte>();
                HaarCascade haarCascade =
                    new HaarCascade(@"haarcascade_frontalface_default.xml");
                var detectedFaces = grayFrame.DetectHaarCascade(haarCascade)[0];

                Dictionary<int, Rectangle> faceSize = new Dictionary<int, Rectangle>();
                foreach (var face in detectedFaces)
                {
                    faceSize.Add(face.rect.Height*face.rect.Width, face.rect);
                }
                if (faceSize != null)
                {
                    Rectangle faceRect = new Rectangle();
                    faceSize.TryGetValue(faceSize.Max(x => x.Key), out faceRect);
                    Bitmap faceBitmap = image.Clone(faceRect, image.PixelFormat);
                    faceBitmap.Save(String.Concat("DetectionFace/", fileName), ImageFormat.Jpeg);
                }
            }
        }
    }
}
