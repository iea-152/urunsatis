using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using urunsatis.Utility;
using urunsatis.Models; // User ve Product modelleri için

namespace urunsatis.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UygulamaDbContext _context;

        public AdminController(UygulamaDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // --- 1. SAYAÇLAR (Kutular İçin) ---
            ViewBag.UrunSayisi = _context.Products.Count();
            ViewBag.KategoriSayisi = _context.Categories.Count();
            ViewBag.KullaniciSayisi = _context.Users.Count();

            // --- 2. LİSTELER (Tablolar İçin) ---

            // A) Kritik Stok Listesi (Stoğu 5'ten az olanlar)
            var kritikStokListesi = _context.Products.Where(x => x.Stock < 5).ToList();

            // B) Üye Listesi (Son 10 üyeyi çekelim)
            // ViewBag ile gönderiyoruz çünkü Model olarak zaten ürünleri gönderdik
            ViewBag.SonUyeler = _context.Users.OrderByDescending(x => x.Id).Take(10).ToList();

            // Sayfaya ürün listesini model olarak gönderiyoruz
            return View(kritikStokListesi);
        }
        // --- ÜYE LİSTESİ SAYFASI ---
        public IActionResult UserList()
        {
            var users = _context.Users.ToList();
            return View(users);
        }
    }

}