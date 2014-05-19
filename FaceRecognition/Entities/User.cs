using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition.Entities
{
    public class User
    {
        public User()
        {
            this.Features = new HashSet<Feature>();
        }
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Feature> Features { get; set; } 
    }
}
