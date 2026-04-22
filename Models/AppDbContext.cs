using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
namespace webProgramming.Models
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> option) : base(option)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Type).HasConversion<int>();
                entity.Property(e => e.EstateType).HasConversion<int>();
                entity.Property(e => e.CategoryType).HasConversion<int>();

                // Configure the relationship between Product and User
                entity.HasOne(p => p.Seller)
                    .WithMany()
                    .HasForeignKey(p => p.SellerId)
                    .HasPrincipalKey(u => u.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.UserId).IsRequired();
                entity.HasIndex(u => u.UserId).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Neighb> Neighs { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Favor> Favors { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<AboutUs> AboutUs { get; set; }
        public DbSet<Settings> Settings { get; set; }

    }
}
