using MvcOnlineTicariOtomasyon.Models.Siniflar;
using System;
using System.Linq;
using System.Web.Mvc;

namespace MvcOnlineTicariOtomasyon.Controllers
{
    [Authorize]
    [Authorize(Roles = "A,P")]
    public class CariController : Controller
    {
        Context c = new Context();

        public ActionResult Index(string p)
        {
            var degerler = from x in c.Carilers select x;

            if (!string.IsNullOrEmpty(p))
            {
                degerler = degerler.Where(y => y.CariAd.Contains(p) || y.CariSoyad.Contains(p));
            }

            var sonuc = degerler.Where(x => x.Durum == true).OrderBy(x => x.CariAd).ToList();
            ViewBag.AramaTerimi = p;
            return View(sonuc);
        }

        [HttpGet]
        public ActionResult YeniCari()
        {
            return View();
        }

        [HttpPost]
        public ActionResult YeniCari(Cariler ca)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Email kontrolü
                    var mevcutEmail = c.Carilers.Any(x => x.CariMail == ca.CariMail && x.Durum == true);
                    if (mevcutEmail)
                    {
                        ModelState.AddModelError("CariMail", "Bu email adresi zaten kayıtlı.");
                        return View(ca);
                    }

                    ca.Durum = true;
                    ca.ToplamBorc = 0;
                    c.Carilers.Add(ca);
                    c.SaveChanges();

                    TempData["Mesaj"] = "Cari başarıyla eklendi.";
                    TempData["MesajTipi"] = "success";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Cari ekleme sırasında bir hata oluştu: " + ex.Message);
                }
            }
            return View(ca);
        }

        public ActionResult CariSil(int id)
        {
            try
            {
                var cari = c.Carilers.Find(id);
                if (cari == null)
                {
                    TempData["Mesaj"] = "Cari bulunamadı.";
                    TempData["MesajTipi"] = "error";
                    return RedirectToAction("Index");
                }

                // Cari ile ilişkili satış var mı kontrol et
                var satisVar = c.SatisHarekets.Any(x => x.CariId == id);
                if (satisVar)
                {
                    TempData["Mesaj"] = "Bu cari ile ilişkili satış kayıtları bulunduğu için silinemez.";
                    TempData["MesajTipi"] = "warning";
                    return RedirectToAction("Index");
                }

                cari.Durum = false;
                c.SaveChanges();

                TempData["Mesaj"] = "Cari başarıyla silindi.";
                TempData["MesajTipi"] = "success";
            }
            catch (Exception ex)
            {
                TempData["Mesaj"] = "Cari silme sırasında bir hata oluştu: " + ex.Message;
                TempData["MesajTipi"] = "error";
            }

            return RedirectToAction("Index");
        }

        public ActionResult CariGetir(int id)
        {
            var cari = c.Carilers.Find(id);
            if (cari == null || cari.Durum == false)
            {
                TempData["Mesaj"] = "Cari bulunamadı.";
                TempData["MesajTipi"] = "error";
                return RedirectToAction("Index");
            }
            return View("CariGetir", cari);
        }

        [HttpPost]
        public ActionResult CariGuncelle(Cariler car)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var cari = c.Carilers.Find(car.CariId);
                    if (cari == null)
                    {
                        TempData["Mesaj"] = "Cari bulunamadı.";
                        TempData["MesajTipi"] = "error";
                        return RedirectToAction("Index");
                    }

                    // Email kontrolü (kendi ID'si hariç)
                    var mevcutEmail = c.Carilers.Any(x => x.CariMail == car.CariMail && x.CariId != car.CariId && x.Durum == true);
                    if (mevcutEmail)
                    {
                        ModelState.AddModelError("CariMail", "Bu email adresi zaten kayıtlı.");
                        return View("CariGetir", car);
                    }

                    cari.CariAd = car.CariAd;
                    cari.CariSoyad = car.CariSoyad;
                    cari.CariSehir = car.CariSehir;
                    cari.CariMail = car.CariMail;
                    c.SaveChanges();

                    TempData["Mesaj"] = "Cari başarıyla güncellendi.";
                    TempData["MesajTipi"] = "success";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Cari güncelleme sırasında bir hata oluştu: " + ex.Message);
                }
            }
            return View("CariGetir", car);
        }

        public ActionResult MusteriSatis(int id)
        {
            var cari = c.Carilers.Find(id);
            if (cari == null || cari.Durum == false)
            {
                TempData["Mesaj"] = "Cari bulunamadı.";
                TempData["MesajTipi"] = "error";
                return RedirectToAction("Index");
            }

            var degerler = c.SatisHarekets.Where(x => x.CariId == id)
                            .OrderByDescending(x => x.Tarih)
                            .ToList();

            ViewBag.CariAd = cari.CariAd + " " + cari.CariSoyad;
            ViewBag.CariId = id;
            ViewBag.ToplamSatis = degerler.Sum(x => x.ToplamTutar);
            ViewBag.SatisSayisi = degerler.Count();

            return View(degerler);
        }

        public ActionResult CariDetay(int id)
        {
            var cari = c.Carilers.Find(id);
            if (cari == null || cari.Durum == false)
            {
                TempData["Mesaj"] = "Cari bulunamadı.";
                TempData["MesajTipi"] = "error";
                return RedirectToAction("Index");
            }

            var satislar = c.SatisHarekets.Where(x => x.CariId == id).ToList();
            ViewBag.ToplamSatis = satislar.Sum(x => x.ToplamTutar);
            ViewBag.SatisSayisi = satislar.Count();
            ViewBag.SonSatis = satislar.OrderByDescending(x => x.Tarih).FirstOrDefault()?.Tarih;

            return View(cari);
        }


    }
}