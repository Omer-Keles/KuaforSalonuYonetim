using Microsoft.AspNetCore.Mvc;
using KuaforSalonuYonetim.Models;

namespace KuaforSalonuYonetim.Controllers
{
    public class IslemController : Controller
    {
        private readonly KuaforContext _context;

        public IslemController(KuaforContext context)
        {
            _context = context;
        }

        // İşlem Listesi
        public IActionResult Index()
        {
            ViewData["KullaniciEmail"] = HttpContext.Session.GetString("Kullanici_Email");
            ViewData["KullaniciAdi"] = HttpContext.Session.GetString("Kullanici_Adi");
            ViewData["KullaniciRol"] = HttpContext.Session.GetString("Kullanici_Rol");
        
            var kullaniciEmail = HttpContext.Session.GetString("Kullanici_Email");
            var kullaniciRol = HttpContext.Session.GetString("Kullanici_Rol");
        
            if (string.IsNullOrEmpty(kullaniciEmail))
            {
                return RedirectToAction("Login","Kullanici"); // Giriş yapmamışsa login sayfasına yönlendir
            }

            if (kullaniciRol != "Admin")
            {
                return RedirectToAction("Index","Home");
            }
            var islemler = _context.Islemler.ToList();
            return View(islemler);
        }

        // Yeni İşlem Ekle Sayfası
        public IActionResult IslemEkle()
        {
            ViewData["KullaniciEmail"] = HttpContext.Session.GetString("Kullanici_Email");
            ViewData["KullaniciAdi"] = HttpContext.Session.GetString("Kullanici_Adi");
            ViewData["KullaniciRol"] = HttpContext.Session.GetString("Kullanici_Rol");
        
            var kullaniciEmail = HttpContext.Session.GetString("Kullanici_Email");
            var kullaniciRol = HttpContext.Session.GetString("Kullanici_Rol");
        
            if (string.IsNullOrEmpty(kullaniciEmail))
            {
                return RedirectToAction("Login","Kullanici"); // Giriş yapmamışsa login sayfasına yönlendir
            }

            if (kullaniciRol != "Admin")
            {
                return RedirectToAction("Index","Home");
            }
            return View();
        }

        // Yeni İşlem Ekleme İşlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IslemEkle(Islem islem)
        {
            ViewData["KullaniciEmail"] = HttpContext.Session.GetString("Kullanici_Email");
            ViewData["KullaniciAdi"] = HttpContext.Session.GetString("Kullanici_Adi");
            ViewData["KullaniciRol"] = HttpContext.Session.GetString("Kullanici_Rol");
            var kullaniciEmail = HttpContext.Session.GetString("Kullanici_Email");
            var kullaniciRol = HttpContext.Session.GetString("Kullanici_Rol");
        
            if (string.IsNullOrEmpty(kullaniciEmail))
            {
                return RedirectToAction("Login","Kullanici"); // Giriş yapmamışsa login sayfasına yönlendir
            }

            if (kullaniciRol != "Admin")
            {
                return RedirectToAction("Index","Home");
            }
            if (!ModelState.IsValid)
            {
                TempData["HataMesaji"] = "İşlem eklenirken bir hata oluştu.";
                return View(islem);
            }

            _context.Islemler.Add(islem);
            _context.SaveChanges();

            TempData["BasariMesaji"] = "Yeni işlem başarıyla eklendi.";
            return RedirectToAction("Index");
        }

        // İşlem Düzenleme Sayfası
        public IActionResult Duzenle(int id)
        {
            
            ViewData["KullaniciEmail"] = HttpContext.Session.GetString("Kullanici_Email");
            ViewData["KullaniciAdi"] = HttpContext.Session.GetString("Kullanici_Adi");
            ViewData["KullaniciRol"] = HttpContext.Session.GetString("Kullanici_Rol");
        
            var kullaniciEmail = HttpContext.Session.GetString("Kullanici_Email");
            var kullaniciRol = HttpContext.Session.GetString("Kullanici_Rol");
        
            if (string.IsNullOrEmpty(kullaniciEmail))
            {
                return RedirectToAction("Login","Kullanici"); // Giriş yapmamışsa login sayfasına yönlendir
            }

            if (kullaniciRol != "Admin")
            {
                return RedirectToAction("Index","Home");
            }
            var islem = _context.Islemler.Find(id);
            if (islem == null)
            {
                TempData["HataMesaji"] = "İlgili işlem bulunamadı.";
                return RedirectToAction("Index");
            }

            return View(islem);
        }

        // İşlem Düzenleme İşlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Duzenle(Islem islem)
        {
            ViewData["KullaniciEmail"] = HttpContext.Session.GetString("Kullanici_Email");
            ViewData["KullaniciAdi"] = HttpContext.Session.GetString("Kullanici_Adi");
            ViewData["KullaniciRol"] = HttpContext.Session.GetString("Kullanici_Rol");
            var kullaniciEmail = HttpContext.Session.GetString("Kullanici_Email");
            var kullaniciRol = HttpContext.Session.GetString("Kullanici_Rol");
        
            if (string.IsNullOrEmpty(kullaniciEmail))
            {
                return RedirectToAction("Login","Kullanici"); // Giriş yapmamışsa login sayfasına yönlendir
            }

            if (kullaniciRol != "Admin")
            {
                return RedirectToAction("Index","Home");
            }
            if (!ModelState.IsValid)
            {
                TempData["HataMesaji"] = "İşlem düzenlenirken bir hata oluştu.";
                return View(islem);
            }

            _context.Islemler.Update(islem);
            _context.SaveChanges();

            TempData["BasariMesaji"] = "İşlem başarıyla güncellendi.";
            return RedirectToAction("Index");
        }

        // İşlem Silme İşlemi
        [HttpPost]
        public IActionResult Sil(int id)
        {
            ViewData["KullaniciEmail"] = HttpContext.Session.GetString("Kullanici_Email");
            ViewData["KullaniciAdi"] = HttpContext.Session.GetString("Kullanici_Adi");
            ViewData["KullaniciRol"] = HttpContext.Session.GetString("Kullanici_Rol");
            var kullaniciEmail = HttpContext.Session.GetString("Kullanici_Email");
            var kullaniciRol = HttpContext.Session.GetString("Kullanici_Rol");
        
            if (string.IsNullOrEmpty(kullaniciEmail))
            {
                return RedirectToAction("Login","Kullanici"); // Giriş yapmamışsa login sayfasına yönlendir
            }

            if (kullaniciRol != "Admin")
            {
                return RedirectToAction("Index","Home");
            }
            var islem = _context.Islemler.Find(id);
            if (islem == null)
            {
                TempData["HataMesaji"] = "İlgili işlem bulunamadı.";
                return RedirectToAction("Index");
            }

            _context.Islemler.Remove(islem);
            _context.SaveChanges();

            TempData["BasariMesaji"] = "İşlem başarıyla silindi.";
            return RedirectToAction("Index");
        }
    }
}
