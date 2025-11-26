using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using urunsatis.Models;
using urunsatis.Utility;

public class CategoryController : Controller
{
    // 1. Veritabanı değişkenini tanımlıyoruz (Adı _db olsun)
    private readonly UygulamaDbContext _db;

    // 2. Kurucu Metot (Constructor) ile bağlantıyı alıyoruz
    public CategoryController(UygulamaDbContext db)
    {
        _db = db;
    }

    // ... Diğer metodların (Index vs.) burada duruyor ...
    // Veri tabanı bağlantınızın (DbContext) burada olduğunu varsayıyorum.
    // Örneğin: private readonly UygulamaDbContext _db;

    // Category Listeleme (Index)
    public IActionResult Index()
    {
        // 1. Veri tabanından tüm Categoryleri çekme
        // Örnek: var CategoryListesi = _db.Categoryler.ToList(); 

        // Şimdilik test için manuel bir liste oluşturalım:
        List<Category> testListesi = new List<Category>
        {
        };

        // 2. Listeyi View'a gönderme
        return View(testListesi);
    }

    // Category Ekleme Formunu Gösteren Metot (GET)
    public IActionResult Add ()
    {
        return View();
    }




    // ---------------------------------------------------------

    // Ekle butonuna basınca ÇALIŞACAK olan kısım (Bunu eklemelisin)
    [HttpPost]
    public IActionResult Add(Category p)
    {
        if (ModelState.IsValid)
        {
            // Artık DbContext değil, yukarıda tanımladığımız "_db"yi kullanıyoruz
            _db.Categories.Add(p);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        return View();
    }
}
