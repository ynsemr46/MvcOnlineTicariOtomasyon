using MvcOnlineTicariOtomasyon.Models.Siniflar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcOnlineTicariOtomasyon.Controllers
{
    [Authorize]
    [Authorize(Roles = "A,P")]
    public class SatisController : Controller
    {
        private readonly Context c = new Context();

        public ActionResult Index()
        {
            var degerler = c.SatisHarekets.ToList();
            return View(degerler);
        }

        public ActionResult SatisListesi()
        {
            var degerler = c.SatisHarekets.ToList();
            return View(degerler);
        }

        [HttpGet]
        public ActionResult YeniSatis()
        {
            List<SelectListItem> deger1 = (from x in c.Uruns.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.UrunAd,
                                               Value = x.UrunId.ToString()
                                           }).ToList();
            List<SelectListItem> deger2 = (from x in c.Carilers.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.CariAd + " " + x.CariSoyad,
                                               Value = x.CariId.ToString()
                                           }).ToList();
            List<SelectListItem> deger3 = (from x in c.Personels.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.PersonelAd + " " + x.PersonelSoyad,
                                               Value = x.PersonelId.ToString()
                                           }).ToList();

            ViewBag.dgr1 = deger1;
            ViewBag.dgr2 = deger2;
            ViewBag.dgr3 = deger3;
            return View();
        }

        [HttpPost]
        public ActionResult YeniSatis(SatisViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var transaction = c.Database.BeginTransaction())
            {
                try
                {
                    // Fatura oluştur
                    var fatura = new Faturalar
                    {
                        FaturaSeriNo = model.FaturaSeriNo,
                        FaturaSıraNo = model.FaturaSıraNo,
                        Tarih = model.Tarih,
                        Saat = model.Saat,
                        VergiDairesi = model.VergiDairesi,
                        TeslimEden = c.Personels.Find(model.PersonelId)?.PersonelAd + " " + c.Personels.Find(model.PersonelId)?.PersonelSoyad,
                        TeslimAlan = c.Carilers.Find(model.CariId)?.CariAd + " " + c.Carilers.Find(model.CariId)?.CariSoyad,
                        Toplam = model.Kalemler.Sum(x => x.Tutar)
                    };
                    c.Faturalars.Add(fatura);
                    c.SaveChanges();

                    // Satış hareketleri ve fatura kalemleri oluştur
                    foreach (var kalem in model.Kalemler)
                    {
                        var urun = c.Uruns.Find(kalem.UrunId);
                        if (urun == null || urun.Stok < kalem.Miktar)
                        {
                            throw new Exception($"Yetersiz stok: {urun?.UrunAd} için stok miktarı {urun?.Stok}, istenen miktar {kalem.Miktar}");
                        }

                        var satis = new SatisHareket
                        {
                            Tarih = model.Tarih,
                            Miktar = kalem.Miktar,
                            Birim = urun?.Birim,
                            Fiyat = kalem.BirimFiyat,
                            ToplamTutar = kalem.Tutar,
                            UrunId = kalem.UrunId,
                            CariId = model.CariId,
                            PersonelId = model.PersonelId,
                            FaturaId = fatura.FaturaId
                        };
                        c.SatisHarekets.Add(satis);

                        // Stok güncelle
                        urun.Stok -= (short)kalem.Miktar;

                        var faturaKalem = new FaturaKalem
                        {
                            Aciklama = urun?.UrunAd,
                            Miktar = kalem.Miktar,
                            Birim = urun?.Birim,
                            BirimFiyat = kalem.BirimFiyat,
                            Tutar = kalem.Tutar,
                            Faturaid = fatura.FaturaId,
                            UrunId = kalem.UrunId,
                            SatisId = satis.SatisId // Will be set after saving satis
                        };
                        c.FaturaKalems.Add(faturaKalem);
                    }
                    c.SaveChanges();
                    transaction.Commit();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "Satış işlemi sırasında bir hata oluştu: " + ex.Message);
                    return View(model);
                }
            }
        }

        public ActionResult SatisGetir(int id)
        {
            List<SelectListItem> deger1 = (from x in c.Uruns.Where(x => x.Durum == true).ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.UrunAd,
                                               Value = x.UrunId.ToString()
                                           }).ToList();
            List<SelectListItem> deger2 = (from x in c.Carilers.Where(x => x.Durum == true).ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.CariAd + " " + x.CariSoyad,
                                               Value = x.CariId.ToString()
                                           }).ToList();
            List<SelectListItem> deger3 = (from x in c.Personels.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.PersonelAd + " " + x.PersonelSoyad,
                                               Value = x.PersonelId.ToString()
                                           }).ToList();

            ViewBag.dgr1 = deger1;
            ViewBag.dgr2 = deger2;
            ViewBag.dgr3 = deger3;
            var deger = c.SatisHarekets.Find(id);
            return View("SatisGetir", deger);
        }

        [HttpPost]
        public ActionResult SatisGuncelle(SatisHareket p)
        {
            using (var transaction = c.Database.BeginTransaction())
            {
                try
                {
                    var deger = c.SatisHarekets.Find(p.SatisId);
                    if (deger == null)
                    {
                        return HttpNotFound();
                    }

                    // Eski miktarı stoğa geri ekle
                    var eskiUrun = c.Uruns.Find(deger.UrunId);
                    if (eskiUrun != null)
                    {
                        eskiUrun.Stok += (short)deger.Miktar;
                    }

                    // Yeni miktarı stoktan düş
                    var yeniUrun = c.Uruns.Find(p.UrunId);
                    if (yeniUrun == null || yeniUrun.Stok < p.Miktar)
                    {
                        throw new Exception($"Yetersiz stok: {yeniUrun?.UrunAd} için stok miktarı {yeniUrun?.Stok}, istenen miktar {p.Miktar}");
                    }
                    yeniUrun.Stok -= (short)p.Miktar;

                    deger.CariId = p.CariId;
                    deger.Miktar = p.Miktar;
                    deger.Birim = c.Uruns.Find(p.UrunId)?.Birim;
                    deger.Fiyat = p.Fiyat;
                    deger.PersonelId = p.PersonelId;
                    deger.Tarih = p.Tarih;
                    deger.ToplamTutar = p.ToplamTutar;
                    deger.UrunId = p.UrunId;

                    // Update corresponding FaturaKalem
                    var faturaKalem = c.FaturaKalems.FirstOrDefault(fk => fk.SatisId == p.SatisId);
                    if (faturaKalem != null)
                    {
                        faturaKalem.Miktar = p.Miktar;
                        faturaKalem.Birim = deger.Birim;
                        faturaKalem.BirimFiyat = p.Fiyat;
                        faturaKalem.Tutar = p.ToplamTutar;
                        faturaKalem.UrunId = p.UrunId;
                        faturaKalem.Aciklama = c.Uruns.Find(p.UrunId)?.UrunAd;
                    }

                    // Update Fatura total
                    var fatura = c.Faturalars.Find(deger.FaturaId);
                    if (fatura != null)
                    {
                        fatura.Toplam = c.FaturaKalems.Where(fk => fk.Faturaid == fatura.FaturaId).Sum(fk => fk.Tutar);
                    }

                    c.SaveChanges();
                    transaction.Commit();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "Güncelleme sırasında bir hata oluştu: " + ex.Message);
                    return View("SatisGetir", p);
                }
            }
        }

        public ActionResult SatisDetay(int id)
        {
            var degerler = c.SatisHarekets.Where(x => x.SatisId == id).ToList();
            return View(degerler);
        }

        public ActionResult SatisSil(int id)
        {
            using (var transaction = c.Database.BeginTransaction())
            {
                try
                {
                    var satis = c.SatisHarekets.Find(id);
                    if (satis == null)
                    {
                        return HttpNotFound();
                    }

                    // Stoğu geri yükle
                    var urun = c.Uruns.Find(satis.UrunId);
                    if (urun != null)
                    {
                        urun.Stok += (short)satis.Miktar;
                    }

                    // Fatura kalemini sil
                    var faturaKalem = c.FaturaKalems.FirstOrDefault(fk => fk.SatisId == id);
                    if (faturaKalem != null)
                    {
                        c.FaturaKalems.Remove(faturaKalem);
                    }

                    // Fatura toplamını güncelle
                    var fatura = c.Faturalars.Find(satis.FaturaId);
                    if (fatura != null)
                    {
                        fatura.Toplam = c.FaturaKalems
                            .Where(fk => fk.Faturaid == fatura.FaturaId)
                            .Select(fk => (decimal?)fk.Tutar)
                            .Sum() ?? 0m; // Use decimal literal '0m'
                        if (fatura.Toplam == 0)
                        {
                            c.Faturalars.Remove(fatura);
                        }
                    }

                    // Satışı sil
                    c.SatisHarekets.Remove(satis);
                    c.SaveChanges();
                    transaction.Commit();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "Silme işlemi sırasında bir hata oluştu: " + ex.Message);
                    return RedirectToAction("Index");
                }
            }
        }

        public JsonResult GetProductUnit(int id)
        {
            var urun = c.Uruns.Find(id);
            return Json(new { unit = urun?.Birim }, JsonRequestBehavior.AllowGet);
        }
    }

    public class SatisViewModel
    {
        public int CariId { get; set; }
        public int PersonelId { get; set; }
        public string FaturaSeriNo { get; set; }
        public string FaturaSıraNo { get; set; }
        public DateTime Tarih { get; set; }
        public string Saat { get; set; }
        public string VergiDairesi { get; set; }
        public List<KalemViewModel> Kalemler { get; set; }
    }

    public class KalemViewModel
    {
        public int UrunId { get; set; }
        public decimal Miktar { get; set; }
        public string Birim { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal Tutar { get; set; }
    }
}