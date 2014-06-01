using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaceRecognition.Model;

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

        public void AddFeature(int id)
        {
            decimal noseLips = nose.Y - lips.X;
            decimal eyeLips = -(leftEye.Y + rightEye.Y) / 2 + lips.Y;
            decimal eyeNose = -(leftEye.Y + rightEye.Y) / 2 + nose.Y;
            decimal eyeBrowNose = -(leftEyebrow.Y + rightEyebrow.Y) / 2 + nose.Y;
            decimal eyeBrowLips = -(leftEyebrow.Y + rightEyebrow.Y) / 2 + lips.Y;
            decimal eyeBrowEye = -(leftEyebrow.Y + rightEyebrow.Y) / 2 + (leftEye.Y + rightEye.Y) / 2;
            decimal eyeEyebrowLeft = -leftEyebrow.Y + leftEye.Y;
            decimal eyeEyebrowRight = -rightEyebrow.Y + rightEye.Y;
            decimal leftEyeLips = (decimal)Math.Sqrt(Math.Pow(lips.X - leftEye.X, 2) + Math.Pow(lips.Y - leftEye.Y, 2));
            decimal rightEyeLips = (decimal)Math.Sqrt(Math.Pow(lips.X - rightEye.X, 2) + Math.Pow(lips.Y - rightEye.Y, 2));
            decimal leftBrowEyeLips =
                (decimal)Math.Sqrt(Math.Pow(nose.X - leftEyebrow.X, 2) + Math.Pow(nose.Y - leftEyebrow.Y, 2));
            decimal rightBrowEyeLips =
                (decimal)Math.Sqrt(Math.Pow(nose.X - rightEyebrow.X, 2) + Math.Pow(nose.Y - rightEyebrow.Y, 2));

            using (FeatureEntities db = new FeatureEntities())
            {
                db.Features.Add(new Feature()
                {
                    UserId = id,
                    f1 = eyeNose/eyeBrowLips,
                    f2 = eyeBrowEye/noseLips,
                    f3 = eyeBrowNose/eyeLips,
                    f4 = eyeEyebrowLeft/eyeEyebrowRight,
                    f5 = leftEyeLips/rightEyeLips,
                    f6 = leftBrowEyeLips/rightBrowEyeLips,
                });

                db.SaveChanges();
            }
        }
    }
}
