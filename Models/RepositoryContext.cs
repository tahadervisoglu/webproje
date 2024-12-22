using Microsoft.EntityFrameworkCore;

namespace StoreApp.Models
{
    // Bu sınıf, Entity Framework Core kullanarak veritabanı bağlamını temsil eder.
    public class RepositoryContext : DbContext
    {
        // Products tablosunu temsil eden DbSet. Veritabanında ürün verilerini tutar.
        public DbSet<Product> Products { get; set; } 

        // CartItems tablosunu temsil eden DbSet. Kullanıcıların sepetlerinde tuttuğu ürünleri yönetir.
        public DbSet<CartItem> CartItems { get; set; } 

        // Yapılandırıcı metot: RepositoryContext için DbContextOptions ayarlarını alır.
        public RepositoryContext(DbContextOptions<RepositoryContext> options)
            : base(options)
        {
            // Base sınıf (DbContext) yapılandırmasını üstlenir.
        }
    }
}
