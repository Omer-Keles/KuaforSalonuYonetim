using Microsoft.AspNetCore.Mvc;
using KuaforSalonuYonetim.Models;
using Microsoft.EntityFrameworkCore;

public class CalisanController : Controller
{
    private readonly KuaforContext _context;

    public CalisanController(KuaforContext context)
    {
        _context = context;
    }

    // GET: /Calisan/Index
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
        return View();
    }
    
    public IActionResult CalisanRandevular(int id)
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
        var calisan = _context.Calisanlar
            .FirstOrDefault(c => c.CalisanId == id);

        if (calisan == null)
            return NotFound();

        var randevular = _context.Randevular
            .Include(r => r.Islem)
            .Include(r => r.Kullanici)
            .Where(r => r.CalisanId == id && r.Durum != "Reddedildi" && r.Durum != "Beklemede") // "Reddedildi" hariç
            .OrderBy(r => r.RandevuTarihi)
            .ThenBy(r => r.BaslangicSaati)
            .ToList();

        ViewData["CalisanAdi"] = $"{calisan.CalisanAdi} {calisan.CalisanSoyadi}";

        return View(randevular);
    }



    // GET: /Calisan/IslemlerSaatler/1
    public IActionResult IslemlerSaatler(int id)
    {
        ViewData["KullaniciEmail"] = HttpContext.Session.GetString("Kullanici_Email");
        ViewData["KullaniciAdi"] = HttpContext.Session.GetString("Kullanici_Adi");
        ViewData["KullaniciRol"] = HttpContext.Session.GetString("Kullanici_Rol");

        if (string.IsNullOrEmpty(ViewData["KullaniciEmail"]?.ToString()))
        {
            return RedirectToAction("Login", "Kullanici");
        }

        if (ViewData["KullaniciRol"]?.ToString() != "Admin")
        {
            return RedirectToAction("Index", "Home");
        }

        var calisan = _context.Calisanlar
            .Include(c => c.CalisanIslemler)
            .ThenInclude(ci => ci.Islem)
            .FirstOrDefault(c => c.CalisanId == id);

        if (calisan == null)
            return NotFound();

        var uygunSaatler = _context.CalisanUygunSaatler
            .Where(us => us.CalisanId == id)
            .ToList();

        var salonSaatleri = _context.CalismaSaatleri.ToList();
        var mevcutIslemIds = calisan.CalisanIslemler.Select(ci => ci.IslemId).ToList();
        var kalanIslemler = _context.Islemler.Where(i => !mevcutIslemIds.Contains(i.IslemId)).ToList();

        // Çalışanın uygun olmayan günlerini filtrele
        var mevcutGunler = uygunSaatler.Select(us => us.Gun).ToList();
        var kalanGunler = salonSaatleri.Where(s => !mevcutGunler.Contains(s.Gun)).ToList();

        ViewData["UygunSaatler"] = uygunSaatler;
        ViewData["SalonSaatleri"] = kalanGunler;
        ViewData["Islemler"] = kalanIslemler;

        return View(calisan);
    }
    
    // POST: Çalışana yeni işlem ekle
    [HttpPost]
    public IActionResult IslemEkle(int calisanId, int islemId)
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
        if (!_context.CalisanIslemler.Any(ci => ci.CalisanId == calisanId && ci.IslemId == islemId))
        {
            _context.CalisanIslemler.Add(new CalisanIslem { CalisanId = calisanId, IslemId = islemId });
            _context.SaveChanges();
        }
        return RedirectToAction("IslemlerSaatler", new { id = calisanId });
    }
    
    // POST: Çalışanın işlemini sil
    [HttpPost]
    public IActionResult IslemSil(int id)
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
        var calisanIslem = _context.CalisanIslemler.Find(id);
        if (calisanIslem == null)
        {
            TempData["HataMesaji"] = "Silinecek işlem bulunamadı.";
            return RedirectToAction("IslemlerSaatler", new { id = id });
        }

        _context.CalisanIslemler.Remove(calisanIslem);
        _context.SaveChanges();

        TempData["BasariMesaji"] = "İşlem başarıyla silindi.";
        return RedirectToAction("IslemlerSaatler", new { id = calisanIslem.CalisanId });
    }

    // POST: Çalışanın uygun saatini ekle
    // POST: Çalışanın uygun saatini ekle
    [HttpPost]
    public IActionResult SaatEkle(int calisanId, string gun, TimeSpan baslangic, TimeSpan bitis)
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
        var salonSaati = _context.CalismaSaatleri.FirstOrDefault(cs => cs.Gun == gun);
        if (salonSaati == null || TimeSpan.Parse(salonSaati.BaslangicSaati) > baslangic || TimeSpan.Parse(salonSaati.BitisSaati) < bitis)
        {
            TempData["HataMesaji"] = "Girilen saatler salonun çalışma saatlerine uygun değil.";
            return RedirectToAction("IslemlerSaatler", new { id = calisanId });
        }

        if (!_context.CalisanUygunSaatler.Any(cu => cu.CalisanId == calisanId && cu.Gun == gun))
        {
            _context.CalisanUygunSaatler.Add(new CalisanUygunSaat
            {
                CalisanId = calisanId,
                Gun = gun,
                BaslangicSaati = baslangic,
                BitisSaati = bitis
            });
            _context.SaveChanges();
        }
        TempData["BasariMesaji"] = "Saat başarıyla eklendi.";

        return RedirectToAction("IslemlerSaatler", new { id = calisanId });
    }

    [HttpPost]
    public IActionResult SaatGuncelle(int id, TimeSpan baslangic, TimeSpan bitis)
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
        var uygunSaat = _context.CalisanUygunSaatler.Find(id);
        if (uygunSaat == null)
        {
            TempData["HataMesaji"] = "Güncellenecek saat bulunamadı.";
            return RedirectToAction("IslemlerSaatler", new { id = id });
        }
            

        var salonSaati = _context.CalismaSaatleri.FirstOrDefault(cs => cs.Gun == uygunSaat.Gun);
        if (salonSaati == null || TimeSpan.Parse(salonSaati.BaslangicSaati) > baslangic || TimeSpan.Parse(salonSaati.BitisSaati) < bitis)
        {
            TempData["HataMesaji"] = "Girilen saatler salonun çalışma saatlerine uygun değil.";
            return RedirectToAction("IslemlerSaatler", new { id = uygunSaat.CalisanId });
        }

        uygunSaat.BaslangicSaati = baslangic;
        uygunSaat.BitisSaati = bitis;
        _context.SaveChanges();

        TempData["BasariMesaji"] = "Saatler başarıyla güncellendi.";
        return RedirectToAction("IslemlerSaatler", new { id = uygunSaat.CalisanId });
    }

    
    // POST: Çalışanın uygun saatini sil
    [HttpPost]
    public IActionResult SaatSil(int id)
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
        var uygunSaat = _context.CalisanUygunSaatler.Find(id);
        if (uygunSaat == null)
        {
            TempData["HataMesaji"] = "Silinecek uygun saat bulunamadı.";
            return RedirectToAction("IslemlerSaatler", new { id = id });
        }

        _context.CalisanUygunSaatler.Remove(uygunSaat);
        _context.SaveChanges();

        TempData["BasariMesaji"] = "Saat başarıyla silindi.";
        return RedirectToAction("IslemlerSaatler", new { id = uygunSaat.CalisanId });
    }

    // GET: /Calisan/Duzenle/1
    [HttpGet]
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
        var calisan = _context.Calisanlar
            .Include(c => c.CalisanIslemler)
            .Include(c => c.UygunSaatler)
            .FirstOrDefault(c => c.CalisanId == id);

        if (calisan == null)
            return NotFound();

        return View(calisan);
    }

    // POST: /Calisan/Duzenle/1
    [HttpPost]
    public IActionResult Duzenle(Calisan model)
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
            return View(model);

        var calisan = _context.Calisanlar.Find(model.CalisanId);

        if (calisan != null)
        {
            calisan.CalisanAdi = model.CalisanAdi;
            calisan.CalisanSoyadi = model.CalisanSoyadi;
            calisan.Telefon = model.Telefon;

            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }
    
    // GET: /Calisan/Randevular
    public IActionResult Randevular()
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
        var beklemede = _context.Randevular
            .Include(r => r.Kullanici)
            .Include(r => r.Calisan)
            .Include(r => r.Islem)
            .Where(r => r.Durum == "Beklemede")
            .ToList();

        var onaylanan = _context.Randevular
            .Include(r => r.Kullanici)
            .Include(r => r.Calisan)
            .Include(r => r.Islem)
            .Where(r => r.Durum == "Onaylandı")
            .ToList();

        var reddedilen = _context.Randevular
            .Include(r => r.Kullanici)
            .Include(r => r.Calisan)
            .Include(r => r.Islem)
            .Where(r => r.Durum == "Reddedildi")
            .ToList();

        ViewData["Beklemede"] = beklemede;
        ViewData["Onaylanan"] = onaylanan;
        ViewData["Reddedilen"] = reddedilen;

        return View();
    }

    // POST: /Calisan/Onayla
    [HttpPost]
    public IActionResult Onayla(int id)
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
        var randevu = _context.Randevular.Find(id);
        if (randevu != null)
        {
            randevu.Durum = "Onaylandı";
            _context.SaveChanges();
        }
        return RedirectToAction("Randevular");
    }

    // POST: /Calisan/Reddet
    [HttpPost]
    public IActionResult Reddet(int id)
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
        var randevu = _context.Randevular.Find(id);
        if (randevu != null)
        {
            randevu.Durum = "Reddedildi";
            _context.SaveChanges();
        }
        return RedirectToAction("Randevular");
    }
    
    [HttpGet]
    public IActionResult CalisanEkle()
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CalisanEkle(Calisan model)
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
            TempData["HataMesaji"] = "Geçersiz çalışan bilgileri.";
            return View(model);
        }

        var yeniCalisan = new Calisan
        {
            CalisanAdi = model.CalisanAdi,
            CalisanSoyadi = model.CalisanSoyadi,
            Telefon = model.Telefon
        };

        _context.Calisanlar.Add(yeniCalisan);
        _context.SaveChanges();

        TempData["BasariMesaji"] = "Yeni çalışan başarıyla eklendi.";
        return RedirectToAction("Index"); // Çalışanların listelendiği sayfaya yönlendirme
    }
    [HttpPost]
    public IActionResult CalisanSil(int id)
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
        // Çalışanı ve ilişkili verileri kontrol et
        var calisan = _context.Calisanlar
            .Include(c => c.CalisanIslemler)
            .Include(c => c.UygunSaatler)
            .FirstOrDefault(c => c.CalisanId == id);

        if (calisan == null)
        {
            TempData["HataMesaji"] = "Silinecek çalışan bulunamadı.";
            return RedirectToAction("Index");
        }

        // Çalışanın ilişkili verilerini sil
        _context.CalisanIslemler.RemoveRange(calisan.CalisanIslemler);
        _context.CalisanUygunSaatler.RemoveRange(calisan.UygunSaatler);

        // Çalışanı sil
        _context.Calisanlar.Remove(calisan);
        _context.SaveChanges();

        TempData["BasariMesaji"] = "Çalışan başarıyla silindi.";
        return RedirectToAction("Index");
    }


}