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