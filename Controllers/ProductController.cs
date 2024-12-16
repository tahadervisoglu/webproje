using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreApp.Models;

namespace StoreApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly RepositoryContext _context;

        public ProductController(RepositoryContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = _context.Products.ToList();
            ViewBag.ProductCount = _context.Products.Count();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            // Kullanıcının sepetine yeni ürün ekleyin ya da mevcut ürünün miktarını arttırın
            var cartItem = await _context.CartItems
                                          .FirstOrDefaultAsync(c => c.ProductID == productId);

            if (cartItem != null)
            {
                // Eğer ürün zaten sepette varsa miktarını artır
                cartItem.Quantity++;
            }
            else
            {
                // Yeni ürün ekle
                cartItem = new CartItem
                {
                    ProductID = productId,
                    Price = product.Price,
                    Quantity = 1
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        // Sepetten ürün çıkar
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var cartItem = await _context.CartItems
                                          .FirstOrDefaultAsync(c => c.ProductID == productId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            // Sepet sayfasına yönlendirme
            return RedirectToAction("ViewCart");
        }


        // Sepet sayfası için ürünleri görüntüle
        public IActionResult ViewCart()
        {
            var cartItems = _context.CartItems.Include(c => c.Product).ToList();
            return View("Cart", cartItems);
        }

        public IActionResult Get(int id)
        {
            Product product = _context.Products.First(p => p.ProductID.Equals(id));
            return View(product);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(product);
        }
    }

}