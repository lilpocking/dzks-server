namespace DZKSserver.Models
{
    public class ArchiveDB : DbContext
    {
        public ArchiveDB(DbContextOptions<ArchiveDB> options) : base(options) { } // берем базовые опции при создании
        public DbSet<Letter> letters => Set<Letter>();
    }
}
