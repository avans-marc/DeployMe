using DeployMe.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DeployMe.Infrastructure
{

    public class StudentDbContext : DbContext
    {
        public const string SQL_SCHEMA = "DeployMe";

        public StudentDbContext(DbContextOptions<StudentDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Student { get; set; }

        /// <summary>
        /// Use this code if you would like to connect this context to a specific SQL schema
        /// Create the schema first with: CREATE SCHEMA [name_of_scheme];
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.HasDefaultSchema(SQL_SCHEMA);
        }
    }
}
