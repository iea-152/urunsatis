using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using urunsatis.Models;
using urunsatis.Utility;

namespace urunsatis.Controllers
{
    public class LoginController : Controller
    {
        private readonly UygulamaDbContext _context;

        public LoginController(UygulamaDbContext context)
        {
            _context = context;
        }

        // Giriş Sayfası (GET)
        [HttpGet]
        public IActionResult Index()
        {
            // Zaten giriş yapmışsa ana sayfaya at
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // Giriş İşlemi (POST)
        [HttpPost]
        public async Task<IActionResult> Index(User p)
        {
            // Veritabanında kullanıcıyı ara (Email ve Şifre ile)
            var user = _context.Users.FirstOrDefault(x => x.Email == p.Email && x.Password == p.Password);

            if (user != null)
            {
                // Kullanıcı bulundu, Kimlik Kartı (Claims) oluşturuluyor
                var claims = new List<Claim>
                {
                    // Profil sayfası için ID'yi mutlaka ekliyoruz
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName ?? ""),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var userIdentity = new ClaimsIdentity(claims, "CookieAuth");
                var principal = new ClaimsPrincipal(userIdentity);

                // Cookie oluşturup giriş yaptır
                await HttpContext.SignInAsync("CookieAuth", principal);

                return RedirectToAction("Index", "Home");
            }

            // Hatalı giriş
            ViewBag.Hata = "Email adresi veya şifre hatalı!";
            return View();
        }

        // Çıkış Yap (Logout)
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Index", "Login");
        }
        // SADECE İLK KULLANICIYI OLUŞTURMAK İÇİN (Sonra Silebilirsin)
        [HttpGet]
        public IActionResult AdminOlustur()
        {
            if (!_context.Users.Any())
            {
                var admin = new User
                {
                    FullName = "Sistem Yöneticisi",
                    Email = "admin@admin.com",
                    Password = "123", // Şimdilik şifresiz/hashsiz
                    Role = "Admin",
                    ProfileImage = "no-image.png"
                };
                _context.Users.Add(admin);
                _context.SaveChanges();
                return Content("Admin kullanıcısı oluşturuldu! Email: admin@admin.com Şifre: 123");
            }
            return Content("Zaten kullanıcı var, oluşturulmadı.");
        }
        // --- KAYIT OL (REGISTER) EKLENEN KISIM ---

        // Kayıt Sayfasını Aç (GET)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Kayıt İşlemini Yap (POST)
        [HttpPost]
        public IActionResult Register(User p)
        {
            // 1. Aynı e-posta var mı kontrol et
            var varMi = _context.Users.Any(x => x.Email == p.Email);
            if (varMi)
            {
                ViewBag.Hata = "Bu e-posta adresi zaten kayıtlı!";
                return View();
            }

            // 2. Yeni kullanıcıyı ayarla
            p.Role = "User"; // Yeni gelenler standart kullanıcı olsun
            p.ProfileImage = "no-image.png"; // Varsayılan resim

            // 3. Veritabanına kaydet
            _context.Users.Add(p);
            _context.SaveChanges();

            // 4. Başarılıysa Giriş sayfasına yönlendir
            TempData["basarili"] = "Kayıt başarılı! Şimdi giriş yapabilirsiniz.";
            return RedirectToAction("Index");
        }
    }
}
