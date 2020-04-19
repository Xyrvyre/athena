using Microsoft.EntityFrameworkCore;
using Athena.Models;

namespace Athena.Data
{
    public class AthenaContext : DbContext
    {
     
        public AthenaContext(DbContextOptions<AthenaContext> options)
            : base(options)
        {
        }

        public DbSet<Label> Label { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Template> Template { get; set; }
    }
}
 
 