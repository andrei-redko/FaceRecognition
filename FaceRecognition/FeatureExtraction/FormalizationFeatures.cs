using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition.FeatureExtraction
{
    public class FormalizationFeatures
    {
        private Point leftEyebrow;
        private Point rightEyebrow;
        private Point leftEye;
        private Point rightEye;
        private Point nose;
        private Point lips;

        public FormalizationFeatures(List<Point> points)
        {
            leftEyebrow = points[0];
            leftEye = points[1];
            rightEyebrow = points[2];
            rightEye = points[3];
            nose = points[4];
            lips = points[5];
        }
    }
}
