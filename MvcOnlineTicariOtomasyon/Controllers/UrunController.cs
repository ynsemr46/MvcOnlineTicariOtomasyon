using MvcOnlineTicariOtomasyon.Models.Siniflar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace MvcOnlineTicariOtomasyon.Controllers
{
    [Authorize]
    [Authorize(Roles = "A,P")]
    public class UrunController : Controller
    {
        private Context db = new Context();

        // GET: Urun
        [Authorize]
        public ActionResult Index(string p)
        {
            var urunler = from x in db.Uruns where x.Durum == true select x;
            if (!string.IsNullOrEmpty(p))
            {
                urunler = urunler.Where(m => m.UrunAd.Contains(p));
            }
            return View(urunler.ToList());
        }

        // Yeni Ürün Ekleme (GET)
        [HttpGet]
        public ActionResult YenıUrun()
        {
            ViewBag.dgr1 = db.Kategoris.Select(k => new SelectListItem
            {
                Value = k.KategoriId.ToString(),
                Text = k.KategoriAd
            }).ToList();
            return View(new Urun());
        }

        // Yeni Ürün Ekleme (POST)
        [HttpPost]
        public ActionResult YenıUrun(Urun urun, string[] OzellikAdi, string[] OzellikDegeri)
        {
            if (ModelState.IsValid && urun.Kategoriid > 0)
            {
                try
                {
                    // Ana görsel dosya yükleme
                    if (Request.Files["UrunGorsel"] != null && Request.Files["UrunGorsel"].ContentLength > 0)
                    {
                        string dosyaadi = Path.GetFileName(Request.Files["UrunGorsel"].FileName);
                        string uzanti = Path.GetExtension(Request.Files["UrunGorsel"].FileName);
                        string yol = "~/Image/" + dosyaadi + uzanti;
                        Request.Files["UrunGorsel"].SaveAs(Server.MapPath(yol));
                        urun.UrunGorsel = "/Image/" + dosyaadi + uzanti;
                    }

                    urun.Durum = true;
                    db.Uruns.Add(urun);
                    db.SaveChanges();



                    // Özellikleri kaydet
                    if (OzellikAdi != null && OzellikDegeri != null && OzellikAdi.Length == OzellikDegeri.Length)
                    {
                        for (int i = 0; i < OzellikAdi.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(OzellikAdi[i]) && !string.IsNullOrEmpty(OzellikDegeri[i]))
                            {
                                db.UrunOzelliks.Add(new UrunOzellik
                                {
                                    UrunId = urun.UrunId,
                                    OzellikAdi = OzellikAdi[i],
                                    OzellikDegeri = OzellikDegeri[i]
                                });
                            }
                        }
                    }

                    // Ek görselleri kaydet (dosya yükleme)
                    if (Request.Files.Count > 1) // Ana görsel hariç diğer dosyalar
                    {
                        for (int i = 1; i < Request.Files.Count; i++)
                        {
                            var file = Request.Files[i];
                            if (file != null && file.ContentLength > 0)
                            {
                                string dosyaadi = Path.GetFileName(file.FileName);
                                string uzanti = Path.GetExtension(file.FileName);
                                string yol = "~/Image/" + dosyaadi + uzanti;
                                file.SaveAs(Server.MapPath(yol));
                                db.UrunGorsels.Add(new UrunGorsel
                                {
                                    UrunId = urun.UrunId,
                                    GorselUrl = "/Image/" + dosyaadi + uzanti
                                });
                            }
                        }
                    }
                    if (urun.Stok > 0 && urun.AlisFiyat > 0)
                    {
                        var gider = new Gider
                        {
                            GiderAciklama = urun.UrunAd + " alımı",
                            Tarih = DateTime.Now,
                            Tutar = urun.Stok * urun.AlisFiyat
                        };
                        db.Giders.Add(gider);
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Kaydetme işlemi sırasında bir hata oluştu: " + ex.Message;
                }
            }
            else
            {
                ViewBag.Error = "Lütfen tüm zorunlu alanları doldurun ve bir kategori seçin.";
            }

            ViewBag.dgr1 = db.Kategoris.Select(k => new SelectListItem
            {
                Value = k.KategoriId.ToString(),
                Text = k.KategoriAd
            }).ToList();
            return View(urun);
        }

        // Ürün Silme
        public ActionResult UrunSil(int id)
        {
            var deger = db.Uruns.Find(id);
            if (deger != null)
            {
                deger.Durum = false;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // Ürün Güncelleme (GET)
        public ActionResult UrunGetir(int id)
        {
            var urun = db.Uruns.Find(id);
            if (urun == null) return HttpNotFound();

            ViewBag.dgr1 = db.Kategoris.Select(k => new SelectListItem
            {
                Value = k.KategoriId.ToString(),
                Text = k.KategoriAd,
                Selected = k.KategoriId == urun.Kategoriid
            }).ToList();
            ViewBag.Ozellikler = db.UrunOzelliks.Where(o => o.UrunId == id).ToList();
            ViewBag.Gorseller = db.UrunGorsels.Where(g => g.UrunId == id).ToList();

            return View(urun);
        }

        // Ürün Güncelleme (POST)
        [HttpPost]
        public ActionResult UrunGuncelle(Urun urun, string[] OzellikAdi, string[] OzellikDegeri)
        {
            if (ModelState.IsValid)
            {
                var mevcutUrun = db.Uruns.Find(urun.UrunId);
                if (mevcutUrun == null) return HttpNotFound();

                mevcutUrun.UrunAd = urun.UrunAd;
                mevcutUrun.Marka = urun.Marka;
                mevcutUrun.Stok = urun.Stok;
                mevcutUrun.AlisFiyat = urun.AlisFiyat;
                mevcutUrun.SatisFiyat = urun.SatisFiyat;
                mevcutUrun.Durum = urun.Durum;
                mevcutUrun.Aciklama = urun.Aciklama;
                mevcutUrun.Kategoriid = urun.Kategoriid;
                mevcutUrun.Birim = urun.Birim;

                short eskiStok = urun.Stok;
                short fark = (short)(mevcutUrun.Stok - eskiStok);
                if (fark > 0 && mevcutUrun.AlisFiyat > 0)
                {
                    var gider = new Gider
                    {
                        GiderAciklama = mevcutUrun.UrunAd + " stok artırımı",
                        Tarih = DateTime.Now,
                        Tutar = fark * mevcutUrun.AlisFiyat
                    };
                    db.Giders.Add(gider);
                }

                // Ana görsel dosya yükleme
                if (Request.Files["UrunGorsel"] != null && Request.Files["UrunGorsel"].ContentLength > 0)
                {
                    string dosyaadi = Path.GetFileName(Request.Files["UrunGorsel"].FileName);
                    string uzanti = Path.GetExtension(Request.Files["UrunGorsel"].FileName);
                    string yol = "~/Image/" + dosyaadi + uzanti;
                    Request.Files["UrunGorsel"].SaveAs(Server.MapPath(yol));
                    mevcutUrun.UrunGorsel = "/Image/" + dosyaadi + uzanti;
                }

                // Mevcut özellikleri ve görselleri sil
                var mevcutOzellikler = db.UrunOzelliks.Where(o => o.UrunId == urun.UrunId).ToList();
                db.UrunOzelliks.RemoveRange(mevcutOzellikler);

                var mevcutGorseller = db.UrunGorsels.Where(g => g.UrunId == urun.UrunId).ToList();
                db.UrunGorsels.RemoveRange(mevcutGorseller);

                // Yeni özellikleri ekle
                if (OzellikAdi != null && OzellikDegeri != null)
                {
                    for (int i = 0; i < OzellikAdi.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(OzellikAdi[i]) && !string.IsNullOrEmpty(OzellikDegeri[i]))
                        {
                            db.UrunOzelliks.Add(new UrunOzellik
                            {
                                UrunId = urun.UrunId,
                                OzellikAdi = OzellikAdi[i],
                                OzellikDegeri = OzellikDegeri[i]
                            });
                        }
                    }
                }

                // Yeni ek görselleri ekle (dosya yükleme)
                if (Request.Files.Count > 1) // Ana görsel hariç diğer dosyalar
                {
                    for (int i = 1; i < Request.Files.Count; i++)
                    {
                        var file = Request.Files[i];
                        if (file != null && file.ContentLength > 0)
                        {
                            string dosyaadi = Path.GetFileName(file.FileName);
                            string uzanti = Path.GetExtension(file.FileName);
                            string yol = "~/Image/" + dosyaadi + uzanti;
                            file.SaveAs(Server.MapPath(yol));
                            db.UrunGorsels.Add(new UrunGorsel
                            {
                                UrunId = urun.UrunId,
                                GorselUrl = "/Image/" + dosyaadi + uzanti
                            });
                        }
                    }
                }


                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.dgr1 = db.Kategoris.Select(k => new SelectListItem
            {
                Value = k.KategoriId.ToString(),
                Text = k.KategoriAd,
                Selected = k.KategoriId == urun.Kategoriid
            }).ToList();
            ViewBag.Ozellikler = db.UrunOzelliks.Where(o => o.UrunId == urun.UrunId).ToList();
            ViewBag.Gorseller = db.UrunGorsels.Where(g => g.UrunId == urun.UrunId).ToList();
            return View("UrunGetir", urun);
        }

        public ActionResult UrunListesi(Urun urn)
        {
            var degerler = db.Uruns.Where(x => x.Durum == true).ToList();
            return View(degerler);
        }

        [HttpGet]
        public ActionResult SatisYap(int id)
        {
            List<SelectListItem> deger3 = (from x in db.Personels.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.PersonelAd + " " + x.PersonelSoyad,
                                               Value = x.PersonelId.ToString()
                                           }).ToList();

            ViewBag.dgr3 = deger3;
            var deger1 = db.Uruns.Find(id);
            ViewBag.dgr1 = deger1.UrunId;
            ViewBag.dgr2 = deger1.SatisFiyat;
            return View();
        }

        [HttpPost]
        public ActionResult SatisYap(SatisHareket p)
        {
            p.Tarih = DateTime.Parse(DateTime.Now.ToShortDateString());
            db.SatisHarekets.Add(p);
            db.SaveChanges();
            return RedirectToAction("Index", "Satis");
        }
    }
}