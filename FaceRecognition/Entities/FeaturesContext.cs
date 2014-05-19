using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition.Entities
{
    public class FeaturesContext : DbContext 
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Feature> Features { get; set; }
    }
}
