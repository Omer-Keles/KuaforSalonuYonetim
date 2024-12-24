using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using KuaforSalonuYonetim.Models;
using KuaforSalonuYonetim.Models.ViewModels;

namespace KuaforSalonuYonetim.Controllers
{
    public class RandevuController : Controller
    {
        private readonly KuaforContext _context;

        public RandevuController(KuaforContext context)
        {
            _context = context;
        }

        // GET: RandevuAl
        [HttpGet]
        public IActionResult RandevuAl()
        {
            ViewData["KullaniciEmail"] = HttpContext.Session.GetString("Kullanici_Email");
            ViewData["KullaniciAdi"] = HttpContext.Session.GetString("Kullanici_Adi");
            // Session'dan kullanıcı maili (örnek)
            var kullaniciEmail = HttpContext.Session.GetString("Kullanici_Email");
            if (string.IsNullOrEmpty(kullaniciEmail))
            {
                return RedirectToAction("Login", "Kullanici");
            }

            // Formun ilk görüntülenişi -> Tüm işlemler, tüm çalışanlar
            var vm = new RandevuAlViewModel
            {
                Islemler = _context.Islemler.ToList(),
                Calisanlar = _context.Calisanlar.ToList()
            };
            return View(vm);
        }

        // POST: RandevuAl
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RandevuAl(RandevuAlViewModel model)
        {
            ViewData["KullaniciEmail"] = HttpContext.Session.GetString("Kullanici_Email");
            ViewData["KullaniciAdi"] = HttpContext.Session.GetString("Kullanici_Adi");

            if (!ModelState.IsValid)
            {
                model.Islemler = _context.Islemler.ToList();
                model.Calisanlar = _context.Calisanlar.ToList();
                return View(model);
            }

            var kullaniciEmail = HttpContext.Session.GetString("Kullanici_Email");
            if (string.IsNullOrEmpty(kullaniciEmail))
            {
                return RedirectToAction("Login", "Kullanici");
            }

            var kullanici = _context.Kullanicilar.FirstOrDefault(u => u.Kullanici_Email == kullaniciEmail);
            if (kullanici == null)
            {
                ModelState.AddModelError("", "Geçerli kullanıcı bulunamadı.");
                model.Islemler = _context.Islemler.ToList();
                model.Calisanlar = _context.Calisanlar.ToList();
                return View(model);
            }

            var secilenIslem = _context.Islemler.FirstOrDefault(i => i.IslemId == model.IslemId);
            var secilenCalisan = _context.Calisanlar.FirstOrDefault(c => c.CalisanId == model.CalisanId);

            if (secilenIslem == null || secilenCalisan == null)
            {
                ModelState.AddModelError("", "Seçilen işlem veya çalışan geçersiz.");
                model.Islemler = _context.Islemler.ToList();
                model.Calisanlar = _context.Calisanlar.ToList();
                return View(model);
            }

            var baslangic = model.BaslangicSaati ?? TimeSpan.Zero;
            var islemSuresi = TimeSpan.FromMinutes(secilenIslem.Sure);
            var bitis = (model.BitisSaati == null || model.BitisSaati == TimeSpan.Zero)
                ? baslangic.Add(islemSuresi)
                : model.BitisSaati.Value;

            var randevuTarihi = model.RandevuTarihi.Value.Date;

            // Güncellenmiş çakışma kontrolü
            bool cakismaVar = _context.Randevular.Any(r =>
                r.CalisanId == secilenCalisan.CalisanId &&
                r.RandevuTarihi.Date == randevuTarihi &&
                r.Durum != "Reddedildi" && // Reddedilen randevuları hariç tut
                (r.BaslangicSaati < bitis && r.BitisSaati > baslangic)
            );

            if (cakismaVar)
            {
                ModelState.AddModelError("", "Seçilen tarih ve saatte bu çalışanın başka randevusu var.");
                model.Islemler = _context.Islemler.ToList();
                model.Calisanlar = _context.Calisanlar.ToList();
                return View(model);
            }

            // Yeni randevu oluştur
            var randevu = new Randevu
            {
                CalisanId = secilenCalisan.CalisanId,
                IslemId = secilenIslem.IslemId,
                RandevuTarihi = randevuTarihi,
                BaslangicSaati = baslangic,
                BitisSaati = bitis,
                Ucret = secilenIslem.Ucret,
                Durum = "Beklemede",
                KullaniciId = kullanici.Kullanici_Id
            };

            _context.Randevular.Add(randevu);
            _context.SaveChanges();

            return RedirectToAction("Randevularim");
        }


        [HttpGet]
        public IActionResult Randevularim()
        {
            ViewData["KullaniciEmail"] = HttpContext.Session.GetString("Kullanici_Email");
            ViewData["KullaniciAdi"] = HttpContext.Session.GetString("Kullanici_Adi");
            // 1) Session'dan kullanıcı e-mailini al
            var kullaniciEmail = HttpContext.Session.GetString("Kullanici_Email");
            if (string.IsNullOrEmpty(kullaniciEmail))
            {
                // Eğer giriş yapılmamışsa ya da session yoksa, login sayfasına yönlendirebilirsiniz
                return RedirectToAction("Login", "Kullanici");
            }

            // 2) Kullanıcıyı veritabanında bul
            var kullanici = _context.Kullanicilar
                .FirstOrDefault(k => k.Kullanici_Email == kullaniciEmail);
            if (kullanici == null)
            {
                // Kullanıcı geçersizse
                return RedirectToAction("Login", "Kullanici");
            }

            // 3) İlgili kullanıcının randevularını çek
            //    - Randevular tablosunu include ederek Calisan, Islem navigasyonlarını da doldururuz
            var randevular = _context.Randevular
                .Include(r => r.Calisan)
                .Include(r => r.Islem)
                .Where(r => r.KullaniciId == kullanici.Kullanici_Id)
                .OrderByDescending(r => r.RandevuTarihi) // tarih sıralaması (isteğe bağlı)
                .ToList();

            return View("Randevularim", randevular);
            // "Randevularim.cshtml" adında bir view döndürür
        }


        // ============================
        //       AJAX ENDPOINT'LER
        // ============================

        // Tüm işlemler
        [HttpGet]
        public IActionResult GetAllIslemler()
        {
            var all = _context.Islemler
                .Select(i => new
                {
                    i.IslemId,
                    i.IslemAdi,
                    i.Ucret,
                    i.Sure
                })
                .ToList();
            return Json(all);
        }

        // Tüm çalışanlar
        [HttpGet]
        public IActionResult GetAllCalisanlar()
        {
            var all = _context.Calisanlar
                .Select(c => new
                {
                    c.CalisanId,
                    AdSoyad = c.CalisanAdi + " " + c.CalisanSoyadi
                })
                .ToList();
            return Json(all);
        }

        // Bir çalışanın yapabildiği işlemler
        [HttpGet]
        public IActionResult GetIslemlerByCalisan(int calisanId)
        {
            var islemIdList = _context.CalisanIslemler
                .Where(ci => ci.CalisanId == calisanId)
                .Select(ci => ci.IslemId)
                .Distinct()
                .ToList();
            var islemler = _context.Islemler
                .Where(i => islemIdList.Contains(i.IslemId))
                .Select(i => new
                {
                    i.IslemId,
                    i.IslemAdi,
                    i.Ucret,
                    i.Sure
                })
                .ToList();
            return Json(islemler);
        }

        // Bir işlemi yapabilen çalışanlar
        [HttpGet]
        public IActionResult GetCalisanlarByIslem(int islemId)
        {
            var calisanIdList = _context.CalisanIslemler
                .Where(ci => ci.IslemId == islemId)
                .Select(ci => ci.CalisanId)
                .Distinct()
                .ToList();
            var calisanlar = _context.Calisanlar
                .Where(c => calisanIdList.Contains(c.CalisanId))
                .Select(c => new
                {
                    c.CalisanId,
                    AdSoyad = c.CalisanAdi + " " + c.CalisanSoyadi
                })
                .ToList();
            return Json(calisanlar);
        }

        // Seçilen işlemin ücret/süresini getir
        [HttpGet]
        public IActionResult GetIslemUcret(int islemId)
        {
            var islem = _context.Islemler.FirstOrDefault(i => i.IslemId == islemId);
            if (islem == null) return NotFound();
            return Json(new { ucret = islem.Ucret, sure = islem.Sure });
        }

        // Müsait saatler (tarih + çalışan)
        [HttpGet]
        public IActionResult GetMusaitSaatler(int? calisanId, string tarih)
        {
            if (!DateTime.TryParseExact(tarih, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out DateTime seciliTarih))
                return BadRequest("Geçersiz tarih formatı.");

            string gunAdi = seciliTarih.ToString("dddd", new CultureInfo("tr-TR"));

            TimeSpan baslangic;
            TimeSpan bitis;

            // Çalışan seçilmediyse -> salonun çalıştığı saat
            if (calisanId == null || calisanId == 0)
            {
                var salonSaati = _context.CalismaSaatleri.FirstOrDefault(cs => cs.Gun == gunAdi);
                if (salonSaati == null) return Json(new List<string>());
                baslangic = TimeSpan.Parse(salonSaati.BaslangicSaati);
                bitis = TimeSpan.Parse(salonSaati.BitisSaati);
            }
            else
            {
                // Çalışanın uygun saati
                var calisanSaati = _context.CalisanUygunSaatler
                    .FirstOrDefault(cu => cu.CalisanId == calisanId && cu.Gun == gunAdi);
                if (calisanSaati == null) return Json(new List<string>());
                baslangic = calisanSaati.BaslangicSaati;
                bitis = calisanSaati.BitisSaati;
            }

            // Yarım saat slot
            var slotlar = new List<TimeSpan>();
            var curr = baslangic;
            while (curr < bitis)
            {
                slotlar.Add(curr);
                curr = curr.Add(new TimeSpan(0, 30, 0));
            }

            // O günkü randevuları al ve "Reddedildi" olanları hariç tut
            var randevular = _context.Randevular
                .Where(r => r.RandevuTarihi.Date == seciliTarih.Date && r.Durum != "Reddedildi");
            if (calisanId != null && calisanId != 0)
                randevular = randevular.Where(r => r.CalisanId == calisanId);

            var rList = randevular.ToList();
            var doluSlotlar = new List<TimeSpan>();

            foreach (var r in rList)
            {
                foreach (var s in slotlar)
                {
                    if (s >= r.BaslangicSaati && s < r.BitisSaati)
                        doluSlotlar.Add(s);
                }
            }

            var musaitSlotlar = slotlar.Except(doluSlotlar).ToList();
            var sonuc = musaitSlotlar.Select(ts => ts.ToString(@"hh\:mm")).ToList();
            return Json(sonuc);
        }
    }
}