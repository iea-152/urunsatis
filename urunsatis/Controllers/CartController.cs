using Microsoft.AspNetCore.Mvc;
using urunsatis.Extensions;
using urunsatis.Models;
using urunsatis.Utility;

namespace urunsatis.Controllers
{
    public class CartController : Controller
    {
        private readonly UygulamaDbContext _context;

        // Constructor SADELEŞTİ: Artık HubContext istemiyor
        public CartController(UygulamaDbContext context)
        {
            _context = context;
        }

        // 1. SEPET LİSTELEME
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Sepet") ?? new List<CartItem>();
            return View(cart);
        }

        // 2. SEPETE EKLEME
        [HttpPost]
        public IActionResult AddToCart(int id, int quantity)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                // Stok kontrolü
                if (quantity > product.Stock)
                {
                    TempData["hata"] = $"Stok yetersiz! En fazla {product.Stock} adet alabilirsiniz.";
                    return RedirectToAction("Index", "Home");
                }

                var cart = HttpContext.Session.GetObject<List<CartItem>>("Sepet") ?? new List<CartItem>();
                var existingItem = cart.FirstOrDefault(x => x.ProductId == id);

                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    cart.Add(new CartItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Price = product.Price,
                        ImageUrl = product.ImageUrl,
                        Quantity = quantity
                    });
                }
                HttpContext.Session.SetObject("Sepet", cart);
                TempData["basarili"] = "Ürün sepete eklendi.";
            }
            return RedirectToAction("Index");
        }

        // 3. SEPETTEN SİLME
        public IActionResult RemoveFromCart(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Sepet");
            if (cart != null)
            {
                var item = cart.FirstOrDefault(x => x.ProductId == id);
                if (item != null)
                {
                    cart.Remove(item);
                    HttpContext.Session.SetObject("Sepet", cart);
                }
            }
            return RedirectToAction("Index");
        }

        // 4. ÖDEME SAYFASINI AÇ (GET)
        [HttpGet]
        public IActionResult Payment()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Sepet");

            if (cart == null || !cart.Any())
            {
                TempData["hata"] = "Sepetiniz boş.";
                return RedirectToAction("Index");
            }

            ViewBag.ToplamTutar = cart.Sum(x => x.Price * x.Quantity);
            return View();
        }

        // 5. ÖDEMEYİ İŞLE VE BİTİR (POST)
        [HttpPost]
        public IActionResult ProcessPayment(string paymentMethod)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Sepet");
            if (cart == null || !cart.Any()) return RedirectToAction("Index");

            // Stoktan Düşme İşlemi
            foreach (var item in cart)
            {
                var product = _context.Products.Find(item.ProductId);
                if (product != null)
                {
                    if (product.Stock >= item.Quantity)
                    {
                        product.Stock -= item.Quantity;
                    }
                    else
                    {
                        TempData["hata"] = $"Üzgünüz, {product.Name} ürünü tükenmiş.";
                        return RedirectToAction("Index");
                    }
                }
            }
            _context.SaveChanges();

            // Sinyal Gönderme (SignalR) Kısmı SİLİNDİ

            // Sepeti Temizle
            HttpContext.Session.Remove("Sepet");

            TempData["basarili"] = "Siparişiniz alındı! Teşekkürler.";
            return RedirectToAction("Index", "Home");
        }
    }
}