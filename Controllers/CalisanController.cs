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
        return View();
    }
    
    public IActionResult CalisanRandevular(int id)
    {
        ViewData["KullaniciEmail"] = HttpContext.Session.GetString("Kullanici_Email");
        ViewData["KullaniciAdi"] = HttpContext.Session.GetString("Kullanici_Adi");
        ViewData["KullaniciRol"] = HttpContext.Session.GetString("Kullanici_Rol");
        var calisan = _context.Calisanlar
            .FirstOrDefault(c => c.CalisanId == id);

        if (calisan == null)
            return NotFound();

        var randevular = _context.Randevular
            .Include(r => r.Islem)
            .Include(r => r.Kullanici)
            .Where(r => r.CalisanId == id && r.Durum != "Reddedildi") // "Reddedildi" hariç
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
        // Çalışanın işlemleri ve uygun saatlerini ayrı sorgularla çekiyoruz
        var calisan = _context.Calisanlar
            .Include(c => c.CalisanIslemler)
            .ThenInclude(ci => ci.Islem)
            .FirstOrDefault(c => c.CalisanId == id);

        if (calisan == null)
            return NotFound();

        var uygunSaatler = _context.CalisanUygunSaatler
            .Where(us => us.CalisanId == id)
            .ToList();

        // Çalışan bilgileri ve uygun saatleri ViewData ile gönderiyoruz
        ViewData["UygunSaatler"] = uygunSaatler;

        return View(calisan);
    }

    // GET: /Calisan/Duzenle/1
    [HttpGet]
    public IActionResult Duzenle(int id)
    {
        ViewData["KullaniciEmail"] = HttpContext.Session.GetString("Kullanici_Email");
        ViewData["KullaniciAdi"] = HttpContext.Session.GetString("Kullanici_Adi");
        ViewData["KullaniciRol"] = HttpContext.Session.GetString("Kullanici_Rol");
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
        var randevu = _context.Randevular.Find(id);
        if (randevu != null)
        {
            randevu.Durum = "Reddedildi";
            _context.SaveChanges();
        }
        return RedirectToAction("Randevular");
    }
}