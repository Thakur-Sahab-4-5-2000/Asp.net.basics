namespace Learning_Backend.Databases
{
    using Learning_Backend.Models.LearningDatabaseModels;
    using Microsoft.EntityFrameworkCore;

    public class LearningDatabase : DbContext
    {
        public LearningDatabase(DbContextOptions<LearningDatabase> options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<LogsTable> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            DbIntializerLearningDatabase dbIntializer = new DbIntializerLearningDatabase(modelBuilder);
            //dbIntializer.CreateUsersTable();
            dbIntializer.Seed();
        }
    }
}
