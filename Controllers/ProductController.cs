using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreApp.Models;

namespace StoreApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly RepositoryContext _context;

        // ProductController constructor, RepositoryContext'i Dependency Injection ile alıyoruz
        public ProductController(RepositoryContext context)
        {
            _context = context;
        }

        // Index action methodu, ürünlerin listelendiği sayfayı render eder
        public IActionResult Index()
        {
            // Tüm ürünleri alıyoruz ve View'a gönderiyoruz
            var model = _context.Products.ToList();
            // Ürün sayısını ViewBag'e ekliyoruz
            ViewBag.ProductCount = _context.Products.Count();
            return View(model);
        }

        // Sepete ürün ekleme işlemi
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            // Ürünü veritabanından buluyoruz
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            // Oturumdaki kullanıcıyı alıyoruz
            var userId = User.Identity.Name;
            // Kullanıcının sepette bu üründen olup olmadığını kontrol ediyoruz
            var cartItem = await _context.CartItems
                                          .FirstOrDefaultAsync(c => c.ProductID == productId && c.UserID == userId);

            if (cartItem != null)
            {
                // Eğer ürün zaten sepette varsa, miktarını artırıyoruz
                cartItem.Quantity++;
            }
            else
            {
                // Yeni ürün ekliyoruz
                cartItem = new CartItem
                {
                    ProductID = productId,
                    Price = product.Price,
                    Quantity = 1,
                    UserID = userId  // Sepet öğesini kullanıcıya bağlıyoruz
                };
                _context.CartItems.Add(cartItem);
            }
            // Başarılı işlem mesajı gösteriyoruz
            TempData["SuccessMessage"] = "Ürün sepete eklendi!";
            // Değişiklikleri kaydediyoruz
            await _context.SaveChangesAsync();
            // Ürün listesine yönlendiriyoruz
            return RedirectToAction("Index", "Product");
        }

        // Ürün silme işlemi
        [HttpPost]
        public async Task<IActionResult> Delete(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            // Ürünü veritabanından siliyoruz
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // Sepetten ürün çıkarma işlemi
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = User.Identity.Name; // Oturumdaki kullanıcıyı alıyoruz
            var cartItem = await _context.CartItems
                                         .FirstOrDefaultAsync(c => c.UserID == userId && c.ProductID == productId);

            if (cartItem != null)
            {
                // Sepet öğesini siliyoruz
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            // Sepetteki yeni ürünleri ve toplam fiyatı alıyoruz
            var cartItems = _context.CartItems.Include(c => c.Product)
                                              .Where(c => c.UserID == userId)
                                              .ToList();

            var totalPrice = cartItems.Sum(c => c.Product.Price * c.Quantity);
            ViewBag.TotalPrice = totalPrice;

            // Cart sayfasına yönlendiriyoruz
            return View("Cart", cartItems);
        }

        // Sepet sayfası için ürünleri görüntüle
        public IActionResult ViewCart()
        {
            var userId = User.Identity.Name; // Oturumdaki kullanıcıyı alıyoruz
            var cartItems = _context.CartItems
                                     .Include(c => c.Product)
                                     .Where(c => c.UserID == userId) // Kullanıcıya özel sepeti alıyoruz
                                     .ToList();

            ViewBag.TotalPrice = cartItems.Sum(c => c.Product.Price * c.Quantity);

            // Cart sayfasını render ediyoruz
            return View("Cart", cartItems);
        }

        // Yeni ürün ekleme sayfasını render et
        [HttpGet]
        public IActionResult Create()
        {
            // Yetkisiz erişim mesajı ekliyoruz
            ViewBag.ErrorMessage = "You are not authorized to access this page.";
            return View();
        }

        // Ürün ekleme işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                // Yeni ürünü ekliyoruz
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            // Model geçerli değilse, aynı sayfayı tekrar gösteriyoruz
            return View(product);
        }
    }
}
