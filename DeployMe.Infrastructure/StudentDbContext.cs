using DeployMe.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DeployMe.Infrastructure
{

    public class StudentDbContext : DbContext
    {
        // Specifies the SQL schema where the tables will be added.
        // The schema is hardcoded within the context, making it more static 
        // compared to configuring it through an external file.
        public const string SqlSchema = "DeployMe";

        public StudentDbContext(DbContextOptions<StudentDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Student { get; set; }

        /// <summary>
        /// Use this code if you would like to connect this context to a specific SQL schema
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.HasDefaultSchema(SqlSchema);
    }
}
