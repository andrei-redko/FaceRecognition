using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition.Entities
{
    public class Feature
    {
        public int FeatureId { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
