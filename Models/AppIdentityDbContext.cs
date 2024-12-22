using Microsoft.AspNetCore.Identity; // Identity sistemiyle ilgili sınıflar
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Identity için Entity Framework entegrasyonu
using Microsoft.EntityFrameworkCore; // Entity Framework Core kütüphanesi

namespace SportsApp.Models {
    // Identity sistemi için özel bir DbContext sınıfı tanımlanıyor
    public class AppIdentityDbContext : IdentityDbContext<IdentityUser> {
        // Yapılandırıcı metot: DbContextOptions ile yapılandırma ayarları alınıyor
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options) {
            // Base sınıfın (IdentityDbContext) constructor'ına options parametresi gönderiliyor
        }
    }
}
