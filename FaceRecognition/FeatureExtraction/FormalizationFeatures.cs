using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaceRecognition.Entities;

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

        public void AddFeature(string userName)
        {
            using (FeaturesContext db = new FeaturesContext())
            {
                User user = new User()
                {
                    Name = userName,
                };
                //db.Users.Add(user);
                //db.SaveChanges();
                double noseLips = nose.Y - lips.X;
                double eyeLips = -(leftEye.Y + rightEye.Y)/2 + lips.Y;
                double eyeNose = -(leftEye.Y + rightEye.Y)/2 + nose.Y;
                double eyeBrowNose = -(leftEyebrow.Y + rightEyebrow.Y)/2 + nose.Y;
                double eyeBrowLips = -(leftEyebrow.Y + rightEyebrow.Y)/2 + lips.Y;
                double eyeBrowEye = -(leftEyebrow.Y + rightEyebrow.Y)/2 + (leftEye.Y + rightEye.Y)/2;
                double eyeEyebrowLeft = -leftEyebrow.Y + leftEye.Y;
                double eyeEyebrowRight = -rightEyebrow.Y + rightEye.Y;
                double leftEyeLips = Math.Sqrt(Math.Pow(lips.X - leftEye.X, 2) + Math.Pow(lips.Y - leftEye.Y, 2));
                double rightEyeLips = Math.Sqrt(Math.Pow(lips.X - rightEye.X, 2) + Math.Pow(lips.Y - rightEye.Y, 2));
                double leftBrowEyeLips =
                    Math.Sqrt(Math.Pow(nose.X - leftEyebrow.X, 2) + Math.Pow(nose.Y - leftEyebrow.Y, 2));
                double rightBrowEyeLips =
                    Math.Sqrt(Math.Pow(nose.X - rightEyebrow.X, 2) + Math.Pow(nose.Y - rightEyebrow.Y, 2));


                double f1 = eyeNose/eyeBrowLips;
                double f2 = eyeBrowEye/noseLips;
                double f3 = eyeBrowNose/eyeLips;
                double f4 = eyeEyebrowLeft/eyeEyebrowRight;
                double f5 = leftEyeLips/rightEyeLips;
                double f6 = leftBrowEyeLips/rightBrowEyeLips;
            }
        }
    }
}
