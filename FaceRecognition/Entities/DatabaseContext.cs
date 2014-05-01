using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognition.Entities
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\Database.mdf;Integrated Security=True")
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Feature> Features { get; set; }
    }
}
