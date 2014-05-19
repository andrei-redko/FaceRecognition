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
                db.Users.Add(user);
                db.SaveChanges();
                double noseLips = nose.Y - lips.X;
                double eyeLips = (leftEye.Y + rightEye.Y)/2 - lips.Y;
                double eyeEyebrowLeft = leftEyebrow.Y - leftEye.Y;
                double eyeEyebrowRight = rightEyebrow.Y - rightEye.Y;
            }
        }
    }
}
