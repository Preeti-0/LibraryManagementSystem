using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Entities;

namespace LibraryManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Publisher> Publishers { get; set; }

        public DbSet<Member> Members { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Bookmark> Bookmarks { get; set; }

        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public DbSet<Banner> Banners { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Optional: Unique email for User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Optional: Review constraint (1 review per book per member)
            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.BookId, r.MemberId })
                .IsUnique();
        }

        public void SeedAdminUser()
        {
            var admin = Members.FirstOrDefault(u => u.Email == "versebook050@gmail.com");
            if (admin == null)
            {
                Members.Add(new Member
                {
                    Name = "Admin",
                    Email = "versebook050@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Role = "Admin",
                    MembershipId = Guid.NewGuid().ToString(),
                    IsVerified = true
                });
            }
            else if (!admin.IsVerified || admin.Role != "Admin")
            {
                admin.IsVerified = true;
                admin.Role = "Admin";
            }

            SaveChanges();
        }


    }
}
