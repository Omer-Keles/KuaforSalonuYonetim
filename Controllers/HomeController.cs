using KuaforSalonuYonetim.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KuaforSalonuYonetim.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly KuaforContext _context;

        public HomeController(KuaforContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Kullanıcı oturumu açmış mı kontrol et
            ViewData["KullaniciEmail"] = HttpContext.Session.GetString("Kullanici_Email");
            ViewData["KullaniciAdi"] = HttpContext.Session.GetString("Kullanici_Adi");
            
            // Salon bilgisini veritabanından alıyoruz
            var salon = _context.Salonlar.FirstOrDefault();

            if (salon == null)
            {
                return NotFound(); // Eğer salon yoksa 404 döner
            }
            
            // Salon ve çalışma saatlerini View'a gönderiyoruz
            ViewData["Salon"] = salon;
            return View();
        }

        public IActionResult Islemler()
        {
            // Kullanıcı oturumu açmış mı kontrol et
            ViewData["KullaniciEmail"] = HttpContext.Session.GetString("Kullanici_Email");
            ViewData["KullaniciAdi"] = HttpContext.Session.GetString("Kullanici_Adi");
            
            // Salon bilgisini veritabanından alıyoruz
            var salon = _context.Salonlar.FirstOrDefault();

            if (salon == null)
            {
                return NotFound(); // Eğer salon yoksa 404 döner
            }

            // Çalışma saatlerini veritabanından çekiyoruz
            var calismaSaatleri = _context.CalismaSaatleri
                .Where(c => c.SalonId == salon.SalonId)
                .OrderBy(c => c.CalismaSaatleriId) // ID'ye göre sıralama
                .ToList();

            // Salon ve çalışma saatlerini View'a gönderiyoruz
            ViewData["Salon"] = salon;
            ViewData["CalismaSaatleri"] = calismaSaatleri;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
