using System.Drawing;
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
            Image<Bgr, Byte> currentFrame = new Image<Bgr, byte>(image);

            if (currentFrame != null)
            {
                Image<Gray, Byte> grayFrame = currentFrame.Convert<Gray, Byte>();
                HaarCascade haarCascade =
                    new HaarCascade(@"haarcascade_frontalface_default.xml");
                var detectedFaces = grayFrame.DetectHaarCascade(haarCascade)[0];

                foreach (var face in detectedFaces)
                    currentFrame.Draw(face.rect, new Bgr(0, double.MaxValue, 0), 3);

                SaveImageSourceToFile("DetectionFace/", 
                    MainWindow.ToBitmapSource(currentFrame));
            }
        }
    }
}
