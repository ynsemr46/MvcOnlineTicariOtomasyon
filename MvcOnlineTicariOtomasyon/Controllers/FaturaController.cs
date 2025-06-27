using MvcOnlineTicariOtomasyon.Models.Siniflar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcOnlineTicariOtomasyon.Controllers
{
    [Authorize]
    [Authorize(Roles = "A,P")]
    public class FaturaController : Controller
    {
        private readonly Context c = new Context();

        public ActionResult Index(string p)
        {
            var liste = c.Faturalars.AsQueryable();
            if (!string.IsNullOrEmpty(p))
            {
                liste = liste.Where(x => x.FaturaSeriNo.Contains(p) || x.FaturaSıraNo.Contains(p) || x.TeslimEden.Contains(p));
            }
            return View(liste.ToList());
        }

        public ActionResult FaturaDetay(int id)
        {
            var degerler = c.FaturaKalems.Where(x => x.Faturaid == id).ToList();
            ViewBag.FaturaId = id;
            return View(degerler);
        }

        public JsonResult GetProductPrice(int id)
        {
            var urun = c.Uruns.Find(id);
            if (urun == null)
                return Json(new { error = "Ürün bulunamadı." }, JsonRequestBehavior.AllowGet);
            return Json(new { price = urun.SatisFiyat }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Dinamik()
        {
            Class4 cs = new Class4();
            cs.deger1 = c.Faturalars.ToList();
            cs.deger2 = c.FaturaKalems.ToList();
            cs.deger3 = c.Tahsilats.ToList(); // Yeni: Tahsilatlar
            return View(cs);
        }

        [HttpGet]
        public ActionResult GetFaturaForUpdate(int id)
        {
            var fatura = c.Faturalars.Find(id);
            if (fatura == null) return Json(new { error = "Fatura bulunamadı." }, JsonRequestBehavior.AllowGet);

            var cariId = c.Carilers.FirstOrDefault(c => (c.CariAd + " " + c.CariSoyad) == fatura.TeslimAlan)?.CariId;
            var personelId = c.Personels.FirstOrDefault(p => (p.PersonelAd + " " + p.PersonelSoyad) == fatura.TeslimEden)?.PersonelId;

            var kalemler = c.FaturaKalems.Where(fk => fk.Faturaid == id).Select(fk => new
            {
                Aciklama = fk.Aciklama,
                Birim = fk.Birim,
                Miktar = fk.Miktar,
                BirimFiyat = fk.BirimFiyat,
                Tutar = fk.Tutar,
                UrunId = fk.UrunId
            }).ToList();

            return Json(new
            {
                fatura = new
                {
                    FaturaId = fatura.FaturaId,
                    FaturaSeriNo = fatura.FaturaSeriNo,
                    FaturaSıraNo = fatura.FaturaSıraNo,
                    Tarih = fatura.Tarih,
                    VergiDairesi = fatura.VergiDairesi,
                    Saat = fatura.Saat,
                    CariId = cariId,
                    PersonelId = personelId,
                    Toplam = fatura.Toplam
                },
                kalemler
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FaturaKaydet(string FaturaId, string FaturaSeriNo, string FaturaSıraNo, DateTime Tarih, string VergiDairesi, string Saat, int CariId, int PersonelId, string Toplam, List<KalemViewModel> kalemler, bool isUpdate)
        {
            using (var transaction = c.Database.BeginTransaction())
            {
                try
                {
                    var cari = c.Carilers.Find(CariId);
                    if (cari == null) throw new Exception("Cari bulunamadı.");

                    decimal yeniToplam = decimal.Parse(Toplam);
                    decimal eskiToplam = 0;

                    if (isUpdate && !string.IsNullOrEmpty(FaturaId))
                    {
                        int id = int.Parse(FaturaId);
                        var fatura = c.Faturalars.Find(id);
                        if (fatura == null) throw new Exception("Fatura bulunamadı.");

                        eskiToplam = fatura.Toplam;
                        var existingKalems = c.FaturaKalems.Where(fk => fk.Faturaid == id).ToList();
                        var existingSatislar = c.SatisHarekets.Where(sh => sh.FaturaId == id).ToList();
                        foreach (var kalem in existingKalems)
                        {
                            var urun = c.Uruns.Find(kalem.UrunId);
                            if (urun != null) urun.Stok += (short)kalem.Miktar;
                            c.FaturaKalems.Remove(kalem);
                        }
                        foreach (var satis in existingSatislar)
                        {
                            c.SatisHarekets.Remove(satis);
                        }
                        c.SaveChanges();

                        fatura.FaturaSeriNo = FaturaSeriNo;
                        fatura.FaturaSıraNo = FaturaSıraNo;
                        fatura.Tarih = Tarih;
                        fatura.VergiDairesi = VergiDairesi;
                        fatura.Saat = Saat;
                        fatura.TeslimEden = c.Personels.Find(PersonelId)?.PersonelAd + " " + c.Personels.Find(PersonelId)?.PersonelSoyad;
                        fatura.TeslimAlan = cari.CariAd + " " + cari.CariSoyad;
                        fatura.Toplam = yeniToplam;
                        fatura.OdendiMi = fatura.OdenenMiktar >= yeniToplam;
                    }
                    else
                    {
                        var fatura = new Faturalar
                        {
                            FaturaSeriNo = FaturaSeriNo,
                            FaturaSıraNo = FaturaSıraNo,
                            Tarih = Tarih,
                            VergiDairesi = VergiDairesi,
                            Saat = Saat,
                            TeslimEden = c.Personels.Find(PersonelId)?.PersonelAd + " " + c.Personels.Find(PersonelId)?.PersonelSoyad,
                            TeslimAlan = cari.CariAd + " " + cari.CariSoyad,
                            Toplam = yeniToplam,
                            OdenenMiktar = 0,
                            OdendiMi = false
                        };
                        c.Faturalars.Add(fatura);
                        c.SaveChanges();
                    }

                    // Cari borcunu güncelle
                    cari.ToplamBorc = cari.ToplamBorc - eskiToplam + yeniToplam;

                    foreach (var x in kalemler)
                    {
                        var urun = c.Uruns.Find(x.UrunId);
                        if (urun == null || urun.Stok < x.Miktar)
                        {
                            throw new Exception($"Yetersiz stok: {urun?.UrunAd} için stok miktarı {urun?.Stok}, istenen miktar {x.Miktar}");
                        }

                        var faturaId = isUpdate ? int.Parse(FaturaId) : c.Faturalars.OrderByDescending(f => f.FaturaId).First().FaturaId;
                        var satis = new SatisHareket
                        {
                            Tarih = Tarih,
                            Miktar = x.Miktar,
                            Birim = urun.Birim, // Birim bilgisini Urun tablosundan al
                            Fiyat = x.BirimFiyat,
                            ToplamTutar = x.Tutar,
                            UrunId = x.UrunId,
                            CariId = CariId,
                            PersonelId = PersonelId,
                            FaturaId = faturaId
                        };
                        c.SatisHarekets.Add(satis);
                        c.SaveChanges();

                        var fk = new FaturaKalem
                        {
                            Aciklama = x.Aciklama,
                            Birim = urun.Birim, // Birim bilgisini Urun tablosundan al
                            BirimFiyat = x.BirimFiyat,
                            Miktar = x.Miktar,
                            Tutar = x.Tutar,
                            Faturaid = faturaId,
                            UrunId = x.UrunId,
                            SatisId = satis.SatisId
                        };
                        c.FaturaKalems.Add(fk);

                        urun.Stok -= (short)x.Miktar;
                    }
                    c.SaveChanges();
                    transaction.Commit();
                    return Json(isUpdate ? "Fatura güncellendi!" : "Satış ve fatura başarıyla kaydedildi!", JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json($"Hata: {ex.Message}", JsonRequestBehavior.AllowGet);
                }
            }
        }

        public JsonResult GetProductUnit(int id)
        {
            var urun = c.Uruns.Find(id);
            if (urun == null)
                return Json(new { error = "Ürün bulunamadı." }, JsonRequestBehavior.AllowGet);
            return Json(new { unit = urun.Birim }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetFaturaTahsilatBilgileri(int faturaId)
        {
            var fatura = c.Faturalars.Find(faturaId);
            if (fatura == null)
                return Json(new { error = "Fatura bulunamadı." }, JsonRequestBehavior.AllowGet);

            var cari = c.Carilers.FirstOrDefault(x => (x.CariAd + " " + x.CariSoyad) == fatura.TeslimAlan);
            if (cari == null)
                return Json(new { error = "Cari bulunamadı." }, JsonRequestBehavior.AllowGet);

            return Json(new
            {
                faturaId = fatura.FaturaId,
                faturaSeriNo = fatura.FaturaSeriNo,
                faturaSiraNo = fatura.FaturaSıraNo,
                cariId = cari.CariId,
                cariAd = cari.CariAd + " " + cari.CariSoyad,
                toplam = fatura.Toplam,
                odenenMiktar = fatura.OdenenMiktar,
                kalanBorc = fatura.Toplam - fatura.OdenenMiktar
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult TahsilatKaydet(int faturaId, int cariId, string odemeTuru, decimal tahsilatMiktari, string aciklama)
        {
            using (var transaction = c.Database.BeginTransaction())
            {
                try
                {
                    // Fatura kontrolü
                    var fatura = c.Faturalars.Find(faturaId);
                    if (fatura == null)
                        return Json(new { error = "Fatura bulunamadı." }, JsonRequestBehavior.AllowGet);

                    // Cari kontrolü
                    var cari = c.Carilers.Find(cariId);
                    if (cari == null)
                        return Json(new { error = "Cari bulunamadı." }, JsonRequestBehavior.AllowGet);

                    // Tahsilat miktarı kontrolü
                    if (tahsilatMiktari <= 0)
                        return Json(new { error = "Tahsilat miktarı sıfırdan büyük olmalıdır." }, JsonRequestBehavior.AllowGet);

                    // Fatura kalan borç kontrolü
                    decimal kalanBorc = fatura.Toplam - fatura.OdenenMiktar;
                    if (tahsilatMiktari > kalanBorc)
                        return Json(new { error = $"Tahsilat miktarı ({tahsilatMiktari:C}) kalan borcu ({kalanBorc:C}) aşamaz." }, JsonRequestBehavior.AllowGet);

                    // Cari borç kontrolü (negatife düşmeyi engelle, ama sıfıra izin ver)
                    const decimal tolerance = 0.0001m; // Hassasiyet için tolerans
                    if (cari.ToplamBorc - tahsilatMiktari < -tolerance)
                        return Json(new { error = $"Tahsilat, cari borcunu negatife düşüremez. Cari borç: {cari.ToplamBorc:C}, Tahsilat miktarı: {tahsilatMiktari:C}" }, JsonRequestBehavior.AllowGet);

                    // Tahsilat kaydı oluştur
                    var tahsilat = new Tahsilat
                    {
                        FaturaId = faturaId,
                        CariId = cariId,
                        OdemeTuru = odemeTuru,
                        TahsilatMiktari = tahsilatMiktari,
                        TahsilatTarihi = DateTime.Now,
                        Aciklama = aciklama
                    };
                    c.Tahsilats.Add(tahsilat);

                    // Fatura ve cari güncellemeleri
                    fatura.OdenenMiktar += tahsilatMiktari;
                    fatura.OdendiMi = fatura.OdenenMiktar >= fatura.Toplam - tolerance; // Hassasiyet için tolerans

                    cari.ToplamBorc -= tahsilatMiktari;

                    // Değişiklikleri kaydet
                    c.SaveChanges();
                    transaction.Commit();

                    return Json(new { success = "Tahsilat başarıyla kaydedildi!" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { error = $"Hata: {ex.Message}" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult pdfListesi(Urun urn)
        {
            var degerler = c.Tahsilats.ToList();
            return View(degerler);
        }

        public ActionResult TahsilatListesi()
        {
            var tahsilatlar = c.Tahsilats.ToList();
            return View(tahsilatlar);
        }

        public class KalemViewModel
        {
            public int UrunId { get; set; }
            public string Aciklama { get; set; }
            public string Birim { get; set; }
            public decimal Miktar { get; set; }
            public decimal BirimFiyat { get; set; }
            public decimal Tutar { get; set; }
        }
    }
}