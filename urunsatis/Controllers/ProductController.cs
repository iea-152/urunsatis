using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using urunsatis.Models;
using urunsatis.Utility;

namespace urunsatis.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly UygulamaDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment; // <-- Dosya yolu bulucu

        // Constructor'ı güncelle (hostEnvironment ekledik)
        public ProductController(UygulamaDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            // Include(u => u.Category) ekledik.
            // Bu sayede ürünleri çekerken "Kategorisini de yanında getir" demiş olduk.
            var urunler = _context.Products.Include(u => u.Category).ToList();

            return View(urunler);
        }
        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.CategoryList = new SelectList(_context.Categories.ToList(), "Id", "Name");
            return View();
        }

        // --- RESİM YÜKLEMELİ EKLEME ---
        [HttpPost]
        public async Task<IActionResult> Add(Product p)
        {
            if (p.ImageUpload != null)
            {
                // 1. Resim için benzersiz isim uydur (Aynı isimli resimler çakışmasın)
                string dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(p.ImageUpload.FileName);

                // 2. Kaydedilecek yer: wwwroot/img/
                string kayitYolu = Path.Combine(_hostEnvironment.WebRootPath, "img", dosyaAdi);

                // 3. Dosyayı oraya kopyala
                using (var fileStream = new FileStream(kayitYolu, FileMode.Create))
                {
                    await p.ImageUpload.CopyToAsync(fileStream);
                }

                // 4. Veritabanına sadece yolunu yaz
                p.ImageUrl = "/img/" + dosyaAdi;
            }
            else
            {
                // Resim yüklemezse varsayılan resim olsun
                p.ImageUrl = "/img/no-image.png";
            }

            _context.Products.Add(p);
            _context.SaveChanges();
            TempData["basarili"] = "Ürün başarıyla eklendi.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var urun = _context.Products.Find(id);
            if (urun == null) return NotFound();
            ViewBag.CategoryList = new SelectList(_context.Categories.ToList(), "Id", "Name");
            return View(urun);
        }

        // --- RESİM YÜKLEMELİ GÜNCELLEME ---
        [HttpPost]
        public async Task<IActionResult> Edit(Product p)
        {
            // Eski ürün bilgisini çek (Resim değişmediyse eskisini korumak için)
            var eskiUrun = _context.Products.AsNoTracking().FirstOrDefault(x => x.Id == p.Id);

            if (p.ImageUpload != null)
            {
                // Yeni resim varsa kaydet
                string dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(p.ImageUpload.FileName);
                string kayitYolu = Path.Combine(_hostEnvironment.WebRootPath, "img", dosyaAdi);

                using (var fileStream = new FileStream(kayitYolu, FileMode.Create))
                {
                    await p.ImageUpload.CopyToAsync(fileStream);
                }
                p.ImageUrl = "/img/" + dosyaAdi;
            }
            else
            {
                // Yeni resim seçilmediyse eskisini koru
                p.ImageUrl = eskiUrun.ImageUrl;
            }

            _context.Products.Update(p);
            _context.SaveChanges();
            TempData["basarili"] = "Ürün güncellendi.";
            return RedirectToAction("Index");
        }

        // Delete metodu aynen kalabilir...
        public IActionResult Delete(int id)
        {
            var urun = _context.Products.Find(id);
            if (urun != null)
            {
                _context.Products.Remove(urun);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        // --- AJAX İLE SİLME İŞLEMİ (Sayfa Yenilenmeden) ---
        [HttpPost]
        public IActionResult DeleteAjax(int id)
        {
            var urun = _context.Products.Find(id);
            if (urun != null)
            {
                // Varsa sil
                _context.Products.Remove(urun);
                _context.SaveChanges();
                // Başarılı döndür
                return Json(new { success = true });
            }
            // Bulamazsa hata döndür
            return Json(new { success = false });
        }
    }
}