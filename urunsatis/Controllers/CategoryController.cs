using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using urunsatis.Models;
using urunsatis.Utility;

namespace urunsatis.Controllers
{
    public class CategoryController : Controller
    {
        private readonly UygulamaDbContext _db;

        public CategoryController(UygulamaDbContext db)
        {
            _db = db;
        }

        // LİSTELEME SAYFASI
        public IActionResult Index()
        {
            // ESKİ KODUN: List<Category> testListesi = new ... (YANLIŞTI)
            // YENİ KOD: Veritabanından çekiyoruz
            var categoryListesi = _db.Categories.ToList();

            return View(categoryListesi);
        }

        // EKLEME SAYFASI (GET)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        // EKLEME İŞLEMİ (POST)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Add(Category p)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Add(p);
                _db.SaveChanges();
                TempData["basarili"] = "Kategori başarıyla eklendi."; // Mesaj gösterelim
                return RedirectToAction("Index");
            }

            // Eğer buraya düşüyorsa hata var demektir.
            return View(p);
        }

        // --- GET ---
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _db.Categories.Find(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // --- POST ---
        [HttpPost]
        public IActionResult Edit(Category p)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Update(p);
                _db.SaveChanges();
                TempData["basarili"] = "Kategori güncellendi.";
                return RedirectToAction("Index");
            }
            return View(p);
        }
        // Silme İşlemi (Lazım olur)
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var category = _db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            _db.Categories.Remove(category);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}