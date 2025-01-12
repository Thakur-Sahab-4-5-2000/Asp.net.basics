namespace Learning_Backend.Databases
{
    using Learning_Backend.Models.LearningDatabaseModels;
    using Microsoft.EntityFrameworkCore;

    public class DbIntializerLearningDatabase
    {
        private ModelBuilder modelBuilder;

        public DbIntializerLearningDatabase(ModelBuilder _modelBuilder)
        {
            modelBuilder = _modelBuilder;
        }
        public void Seed()
        {
           // modelBuilder.Entity<Users>().HasData(
           //new Users { Id = 1, Username = "admin", PasswordHash = "admin123", Email = "admin@example.com", Role = 1 },
        
   //new Users { Id = 2, Username = "Shubham", PasswordHash = "user123", Email = "user@example.com", Role = 2 }
           //);

            modelBuilder.Entity<Roles>().HasData(
                new Roles { Id = 1, RoleName = "Admin", Description = "Admin Role" },
                new Roles { Id = 2, RoleName = "User", Description = "User Role" }
                );
        }
    }
}
