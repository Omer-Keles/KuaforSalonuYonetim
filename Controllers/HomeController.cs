using KuaforSalonuYonetim.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KuaforSalonuYonetim.Controllers
{
    public class HomeController : Controller
    {
        private readonly KuaforContext _context;

        public HomeController(KuaforContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Kullanıcı oturumu açmış mı kontrol et
            ViewData["KullaniciEmail"] = HttpContext.Session.GetString("Kullanici_Email");
            ViewData["KullaniciAdi"] = HttpContext.Session.GetString("Kullanici_Adi");
            ViewData["KullaniciRol"] = HttpContext.Session.GetString("Kullanici_Rol");
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
            ViewData["KullaniciRol"] = HttpContext.Session.GetString("Kullanici_Rol");
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
            // Veritabanından işlemleri al
            var islemler = _context.Islemler.ToList();

            // View'a gönder

            // Salon ve çalışma saatlerini View'a gönderiyoruz
            ViewData["Salon"] = salon;
            ViewData["CalismaSaatleri"] = calismaSaatleri;
            return View(islemler);
        }

        public IActionResult CalismaSaatlerimiz()
        {
            ViewData["KullaniciEmail"] = HttpContext.Session.GetString("Kullanici_Email");
            ViewData["KullaniciAdi"] = HttpContext.Session.GetString("Kullanici_Adi");
            ViewData["KullaniciRol"] = HttpContext.Session.GetString("Kullanici_Rol");
            
            // Salon bilgisini veritabanından alıyoruz
            var salon = _context.Salonlar.FirstOrDefault();
            
            // Çalışma saatlerini veritabanından çekiyoruz
            var calismaSaatleri = _context.CalismaSaatleri
                .Where(c => c.SalonId == salon.SalonId)
                .OrderBy(c => c.CalismaSaatleriId) // ID'ye göre sıralama
                .ToList();
            
            ViewData["Salon"] = salon;
            ViewData["CalismaSaatleri"] = calismaSaatleri;
            return View(calismaSaatleri);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
