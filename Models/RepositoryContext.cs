using Microsoft.EntityFrameworkCore;

namespace StoreApp.Models
{
    public class RepositoryContext : DbContext
    {
        public DbSet<Product> Products { get; set; }  // Burada parantez eksikti
        public DbSet<CartItem> CartItems { get; set; } // Sepet ürünleri için yeni DbSet

        public RepositoryContext(DbContextOptions<RepositoryContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>()
            .HasData(
                new Product() { ProductID = 1, ProductName = "Computer", Price = 17_000 },
                new Product() { ProductID = 2, ProductName = "Keyboard", Price = 1_000 },
                new Product() { ProductID = 3, ProductName = "Mouse", Price = 500 },
                new Product() { ProductID = 4, ProductName = "Mointor", Price = 7_000 },
                new Product() { ProductID = 5, ProductName = "Deck", Price = 1_500 }
            );
        }
    }
}
