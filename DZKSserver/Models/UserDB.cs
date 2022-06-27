namespace DZKSserver.Models
{
    public class UserDB : DbContext
    {
        public UserDB(DbContextOptions<UserDB> options) : base(options) { } // берем базовые опции при создании
        public DbSet<User> users => Set<User>();
        public DbSet<Letter> letters => Set<Letter>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { userId = 1, login = "Administrator", password = "12345" }
                ) ;
            base.OnModelCreating(modelBuilder);
        }
    }
}
