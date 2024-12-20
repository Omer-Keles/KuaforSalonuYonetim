using KuaforSalonuYonetim.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KuaforSalonuYonetim.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Kullanıcı oturumu açmış mı kontrol et
            ViewData["KullaniciEmail"] = HttpContext.Session.GetString("Kullanici_Email");
            ViewData["KullaniciAdi"] = HttpContext.Session.GetString("Kullanici_Adi");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
