using Microsoft.AspNetCore.Mvc;
using KuaforSalonuYonetim.Models;
using Microsoft.EntityFrameworkCore;

namespace KuaforSalonuYonetim.Controllers
{
    public class SalonController : Controller
    {
        private readonly KuaforContext _context;

        public SalonController(KuaforContext context)
        {
            _context = context;
        }

        // Salon bilgilerini görüntüle
        public async Task<IActionResult> Index()
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
            var salonlar = await _context.Salonlar.ToListAsync();
            return View(salonlar);
        }

        // Salon bilgilerini düzenle
        public async Task<IActionResult> Edit(int id)
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
            var salon = await _context.Salonlar.FindAsync(id);
            if (salon == null)
            {
                return NotFound();
            }
            return View(salon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Salon salon)
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
            if (id != salon.SalonId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalonExists(salon.SalonId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(salon);
        }

        // Çalışma saatlerini görüntüle
        public async Task<IActionResult> CalismaSaatleri(int salonId)
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
            var calismaSaatleri = await _context.CalismaSaatleri
                .Where(c => c.SalonId == salonId)
                .ToListAsync();

            ViewBag.SalonId = salonId;
            return View(calismaSaatleri);
        }
        
        // Çalışma saatlerini düzenle
        public async Task<IActionResult> DuzenleSaat(int id)
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
            var saat = await _context.CalismaSaatleri.FindAsync(id);
            if (saat == null)
            {
                return NotFound();
            }

            ViewBag.Saatler = GetSaatListesi();
            return View(saat);
        }
        // Çalışma saatlerini düzenle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DuzenleSaat(int id, CalismaSaatleri model)
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
            if (id != model.CalismaSaatleriId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(CalismaSaatleri), new { salonId = model.SalonId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CalismaSaatleriExists(model.CalismaSaatleriId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Saatler = GetSaatListesi();
            return View(model);
        }

        
// Yeni gün ekle
        public async Task<IActionResult> YeniGunEkle(int salonId)
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
            var mevcutGünler = await _context.CalismaSaatleri
                .Where(c => c.SalonId == salonId)
                .Select(c => c.Gun)
                .ToListAsync();

            var uygunGünler = GetGunListesi().Where(g => !mevcutGünler.Contains(g)).ToList();

            ViewBag.SalonId = salonId;
            ViewBag.UygunGünler = uygunGünler;
            ViewBag.Saatler = GetSaatListesi();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> YeniGunEkle(CalismaSaatleri model)
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
            if (ModelState.IsValid)
            {
                _context.CalismaSaatleri.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CalismaSaatleri), new { salonId = model.SalonId });
            }

            var mevcutGünler = await _context.CalismaSaatleri
                .Where(c => c.SalonId == model.SalonId)
                .Select(c => c.Gun)
                .ToListAsync();

            ViewBag.UygunGünler = GetGunListesi().Where(g => !mevcutGünler.Contains(g)).ToList();
            ViewBag.Saatler = GetSaatListesi();
            return View(model);
        }
        private List<string> GetGunListesi()
        {
            return new List<string> { "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi" };
        }

        private List<string> GetSaatListesi()
        {
            return new List<string>
            {
                "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00"
            };
        }

        private bool CalismaSaatleriExists(int id)
        {
            return _context.CalismaSaatleri.Any(e => e.CalismaSaatleriId == id);
        }

        private bool SalonExists(int id)
        {
            return _context.Salonlar.Any(e => e.SalonId == id);
        }
        // Çalışma saatlerini sil
        [HttpPost]
        public async Task<IActionResult> SilAjax(int id)
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
            var saat = await _context.CalismaSaatleri.FindAsync(id);
            if (saat != null)
            {
                _context.CalismaSaatleri.Remove(saat);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Çalışma saati başarıyla silindi." });
            }

            return Json(new { success = false, message = "Çalışma saati bulunamadı." });
        }

    }
}
