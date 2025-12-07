using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using urunsatis.Models;
using urunsatis.Utility;
using Microsoft.AspNetCore.Authorization;

namespace urunsatis.Controllers


{
    [Authorize]
    public class HomeController : Controller
    {   
        private readonly UygulamaDbContext _context;

        public HomeController(UygulamaDbContext context)
        {
            _context = context;
        }

        // Bu metot hem ana sayfayı açar, hem de kategori seçilince filtreler
        public IActionResult Index(int? id) // Buradaki 'id' kategori ID'sidir
        {
            var urunler = _context.Products.AsQueryable();

            if (id != null) // Eğer bir kategoriye tıklanmışsa filtrele
            {
                urunler = urunler.Where(x => x.CategoryId == id);
            }

            return View(urunler.ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }
        // "search" parametresi silindi
        public IActionResult UrunleriGetir(int? id)
        {
            var urunler = _context.Products.Include(p => p.Category).AsQueryable();

            if (id != null)
            {
                urunler = urunler.Where(x => x.CategoryId == id);
            }

            // Arama filtreleme kodları silindi

            return PartialView("_UrunListesi", urunler.ToList());
        }
    }
}