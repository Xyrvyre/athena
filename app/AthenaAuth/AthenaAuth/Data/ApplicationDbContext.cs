using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AthenaAuth.Models;

namespace AthenaAuth.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<AthenaAuth.Models.Group> Group { get; set; }
        public DbSet<AthenaAuth.Models.Template> Template { get; set; }
        public DbSet<AthenaAuth.Models.Group_Template> Group_Template { get; set; }
        public DbSet<AthenaAuth.Models.Group_User> Group_User { get; set; }
    }
}
