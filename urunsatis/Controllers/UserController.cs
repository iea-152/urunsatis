using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using urunsatis.Models;
using urunsatis.Utility;

namespace urunsatis.Controllers
{
    [Authorize] // Sadece giriş yapanlar girebilir
    public class UserController : Controller
    {
        private readonly UygulamaDbContext _context;

        public UserController(UygulamaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Profile()
        {
            // Giriş yapan kullanıcının ID'sini Cookie'den bul (LoginController'da Claim olarak eklenmeli)
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Index", "Login");

            int userId = int.Parse(userIdString);
            var user = _context.Users.Find(userId);

            return View(user);
        }

        [HttpPost]
        public IActionResult Profile(User model)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.Parse(userIdString);

            var user = _context.Users.Find(userId);
            if (user == null) return NotFound();

            // Bilgileri güncelle
            user.FullName = model.FullName;
            user.Email = model.Email;

            // Şifre alanı boş değilse güncelle
            if (!string.IsNullOrEmpty(model.Password))
            {
                user.Password = model.Password;
            }

            _context.SaveChanges();
            ViewBag.Mesaj = "Bilgiler başarıyla güncellendi!";

            return View(user);
        }
    }
}