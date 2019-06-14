using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace PlayCrapsAssessment
{
    class CrapsContext : DbContext
    {
        public CrapsContext()
           : base("name=CrapsContextConnectionString")
        {
        }

        public DbSet<Player> Player { get; set; }
        public DbSet<Round> Round { get; set; }
        public DbSet<Rolls> Rolls { get; set; }
        public DbSet<Game> Game { get; set; }

    }
}
