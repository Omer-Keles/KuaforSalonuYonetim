using KuaforSalonuYonetim.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace KuaforSalonuYonetim.Controllers;

public class AuthController : Controller
    {
        private readonly KuaforContext _context;

        public AuthController(KuaforContext context)
        {
            _context = context;
        }
        
        // GET: /Auth/SignUp
        public IActionResult SignUp()
        {
            return View();
        }

        // POST: /Auth/SignUp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(Kullanici kullanici)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcı e-posta adresiyle kayıtlı mı kontrol et
                var mevcutKullanici = await _context.Kullanicilar
                    .FirstOrDefaultAsync(u => u.Kullanici_Email == kullanici.Kullanici_Email);
                if (mevcutKullanici != null)
                {
                    ModelState.AddModelError("Kullanici_Email", "Bu e-posta adresi zaten kayıtlı.");
                    return View(kullanici);
                }

                // PasswordHasher ile şifreyi hash'leyin
                var passwordHasher = new PasswordHasher<Kullanici>();
                kullanici.Kullanici_Parola = passwordHasher.HashPassword(kullanici, kullanici.Kullanici_Parola);
                
                // Yeni kullanıcıyı veritabanına ekleyin
                _context.Kullanicilar.Add(kullanici);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }

            return View(kullanici);
        }

        // GET: /Auth/Login
        // Giriş sayfası
        public IActionResult Login()
        {
            // Eğer kullanıcı zaten giriş yaptıysa, tekrar login sayfasına gitmesin
            if (HttpContext.Session.GetString("Kullanici_Email") != null)
            {
                return RedirectToAction("Index", "Home"); // Ana sayfaya yönlendir
            }
            return View();
        }

        // Giriş işlemi
        [HttpPost]
        public async Task<IActionResult> Login(string email, string parola)
        {
            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(u => u.Kullanici_Email == email);

            if (kullanici == null)
            {
                ModelState.AddModelError("", "Geçersiz e-posta adresi.");
                return View();
            }

            var passwordHasher = new PasswordHasher<Kullanici>();
            var result = passwordHasher.VerifyHashedPassword(kullanici, kullanici.Kullanici_Parola, parola);

            if (result == PasswordVerificationResult.Success)
            {
                // Başarılı giriş, kullanıcı bilgilerini session'a kaydediyoruz
                HttpContext.Session.SetString("Kullanici_Email", kullanici.Kullanici_Email);
                HttpContext.Session.SetString("Kullanici_Adi", kullanici.Kullanici_Adi);

                return RedirectToAction("Index", "Home"); // Ana sayfaya yönlendir
            }
            else
            {
                ModelState.AddModelError("", "Geçersiz şifre.");
                return View();
            }
        }
        
        // Profil sayfası
        public IActionResult Profil()
        {
            ViewData["KullaniciEmail"] = HttpContext.Session.GetString("Kullanici_Email");
            var kullaniciEmail = HttpContext.Session.GetString("Kullanici_Email");

            if (string.IsNullOrEmpty(kullaniciEmail))
            {
                return RedirectToAction("Login"); // Giriş yapmamışsa login sayfasına yönlendir
            }

            var kullanici = _context.Kullanicilar.FirstOrDefault(u => u.Kullanici_Email == kullaniciEmail);

            if (kullanici == null)
            {
                return RedirectToAction("Login"); // Kullanıcı bulunamazsa login sayfasına yönlendir
            }

            return View(kullanici); // Kullanıcı bilgilerini Profil sayfasına gönder
        }
        
        // Çıkış yapma işlemi
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Kullanici_Email");
            HttpContext.Session.Remove("Kullanici_Adi");

            return RedirectToAction("Index", "Home"); // Ana sayfaya yönlendir
        }

    }