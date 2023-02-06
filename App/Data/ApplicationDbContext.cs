using ArqInf.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ArqInf.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ArqInf.Models.Role> Role { get; set; }
        public DbSet<ArqInf.Models.Assignment> Assignment { get; set; }

        public DbSet<ArqInf.Models.UserAssignments> UserAssignments { get; set; }

        public DbSet<ArqInf.Models.Project> Project { get; set; }

        public DbSet<ArqInf.Models.ProjectAssignments> ProjectAssignments { get; set; }

        public DbSet<ArqInf.Models.Occupation> Occupation { get; set; }
    }
}