using Microsoft.AspNetCore.Mvc;
using urunsatis.Models;
using urunsatis.Utility;
using System.Linq;

namespace urunsatis.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly UygulamaDbContext _db;

        public CategoryMenuViewComponent(UygulamaDbContext db)
        {
            _db = db;
        }

        public IViewComponentResult Invoke()
        {
            // Veritabanından kategorileri çek
            var kategoriler = _db.Categories.ToList();
            return View(kategoriler);
        }
    }
}