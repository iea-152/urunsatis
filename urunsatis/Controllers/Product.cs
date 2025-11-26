using Microsoft.AspNetCore.Mvc;

namespace urunsatis.Controllers
{
    public class Product : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
