using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportsApp.ViewModels;

public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    // Controller constructor, SignInManager ve UserManager'ı Dependency Injection ile alıyoruz
    public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    // GET methodu, Login sayfasını render etmek için kullanılıyor
    [HttpGet]
    public IActionResult Login(string ReturnUrl)
    {
        // ReturnUrl, kullanıcının giriş yaptıktan sonra dönmek istediği URL'yi saklar
        // Bu değeri ViewData'ya ekleyerek, View'da erişilebilir kılıyoruz
        ViewData["ReturnUrl"] = ReturnUrl;
        return View();
    }

    // POST methodu, Login işlemi için kullanılıyor
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, string ReturnUrl)
    {
        // Kullanıcı adını modelden alıp, veritabanında arıyoruz
        var user = await _userManager.FindByNameAsync(model.UserName);

        if (user != null)
        {
            // Şifre doğrulaması yapıyoruz
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (result.Succeeded)
            {
                // Başarılı giriş sonrası, kullanıcıyı Product sayfasına yönlendiriyoruz
                return RedirectToAction("Index", "Product");
            }
            else
            {
                // Giriş başarısızsa, TempData ile hata mesajı gönderiyoruz
                TempData["ErrorMessage"] = "Giriş başarısız! Kullanıcı adı veya şifre yanlış.";
            }
        }
        else
        {
            // Kullanıcı bulunamadıysa, TempData ile hata mesajı gönderiyoruz
            TempData["ErrorMessage"] = "Giriş başarısız! Kullanıcı adı veya şifre yanlış.";
        }

        // Hata mesajını göstermek için tekrar login formunu döndürüyoruz
        return View(model);
    }

    // Logout işlemi, kullanıcıyı çıkış yaparak Login sayfasına yönlendiriyor
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }

    // GET methodu, Register sayfasını render etmek için kullanılıyor
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // POST methodu, Register işlemi için kullanılıyor
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Yeni bir kullanıcı nesnesi oluşturuyoruz
            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            // Kullanıcıyı oluşturuyoruz
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Kullanıcı başarıyla oluşturulduysa, ona 'User' rolünü ekliyoruz
                await _userManager.AddToRoleAsync(user, "User");
                // Başarılı kayıt sonrası, Home sayfasına yönlendiriyoruz
                return RedirectToAction("Index", "Home");
            }

            // Hata durumunda, ModelState'e hata mesajlarını ekliyoruz
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // Model geçerli değilse, tekrar kayıt formunu gösteriyoruz
        return View(model);
    }
}
