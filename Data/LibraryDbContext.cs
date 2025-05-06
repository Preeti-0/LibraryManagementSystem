using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Entities;

namespace LibraryManagementSystem.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        // Add other DbSets as needed (e.g., for Books, etc.)
        // public DbSet<Book> Books { get; set; }
    }
}