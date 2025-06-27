using MvcOnlineTicariOtomasyon.Models.Siniflar;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace MvcOnlineTicariOtomasyon.Controllers
{
    [Authorize]
    [Authorize(Roles = "A,P")]
    public class istatistikController : Controller
    {
        private readonly Context _context = new Context();

        public ActionResult Index()
        {
            DateTime bugun = DateTime.Today;
            DateTime son30Gun = bugun.AddDays(-30);
            DateTime son7Gun = bugun.AddDays(-7);

            // Durumu aktif olanlar filtreleniyor
            var aktifUrunler = _context.Uruns.Where(x => x.Durum == true).ToList();
            var aktifCariler = _context.Carilers.Where(x => x.Durum == true).ToList();

            // Müşteri ve Personel Metrikleri
            ViewBag.ToplamCari = aktifCariler.Count;
            ViewBag.ToplamUrun = aktifUrunler.Count;
            ViewBag.ToplamPersonel = _context.Personels.Count();
            ViewBag.ToplamKategori = _context.Kategoris.Count();

            // Envanter Metrikleri
            ViewBag.ToplamStok = aktifUrunler.Sum(x => (int)x.Stok);
            ViewBag.ToplamMarka = aktifUrunler.Select(x => x.Marka).Distinct().Count();
            ViewBag.KritikStok = aktifUrunler.Count(x => x.Stok <= 20);
            ViewBag.EnPahaliUrun = aktifUrunler.OrderByDescending(x => x.SatisFiyat).Select(x => x.UrunAd).FirstOrDefault() ?? "Yok";
            ViewBag.EnUcuzUrun = aktifUrunler.OrderBy(x => x.SatisFiyat).Select(x => x.UrunAd).FirstOrDefault() ?? "Yok";

            // Finansal Metrikler
            ViewBag.ToplamGelir = (_context.SatisHarekets.Where(x => x.Urun.Durum == true).Sum(x => (decimal?)x.ToplamTutar) ?? 0) +
                                  (_context.Tahsilats.Sum(x => (decimal?)x.TahsilatMiktari) ?? 0);
            ViewBag.ToplamGider = _context.Giders.Sum(x => (decimal?)x.Tutar) ?? 0;
            ViewBag.KasaBakiyesi = ViewBag.ToplamGelir - ViewBag.ToplamGider;

            // Aylık
            ViewBag.AylikSatis = _context.SatisHarekets.Where(x => x.Tarih >= son30Gun && x.Urun.Durum == true).Sum(x => (decimal?)x.ToplamTutar) ?? 0;
            ViewBag.AylikTahsilat = _context.Tahsilats.Where(x => x.TahsilatTarihi >= son30Gun).Sum(x => (decimal?)x.TahsilatMiktari) ?? 0;
            ViewBag.AylikGider = _context.Giders.Where(x => x.Tarih >= son30Gun).Sum(x => (decimal?)x.Tutar) ?? 0;

            // En çok satan
            ViewBag.EnCokSatanUrun = _context.SatisHarekets
                .Where(x => x.Urun.Durum == true)
                .GroupBy(x => x.UrunId)
                .OrderByDescending(g => g.Sum(x => x.Miktar))
                .Select(g => g.FirstOrDefault().Urun.UrunAd)
                .FirstOrDefault() ?? "Yok";

            // Bugünlük
            ViewBag.BugunSatisAdet = _context.SatisHarekets.Count(x => DbFunctions.TruncateTime(x.Tarih) == bugun && x.Urun.Durum == true);
            ViewBag.BugunSatisTutar = _context.SatisHarekets.Where(x => DbFunctions.TruncateTime(x.Tarih) == bugun && x.Urun.Durum == true)
                .Sum(x => (decimal?)x.ToplamTutar) ?? 0;
            ViewBag.BugunTahsilat = _context.Tahsilats.Where(x => DbFunctions.TruncateTime(x.TahsilatTarihi) == bugun)
                .Sum(x => (decimal?)x.TahsilatMiktari) ?? 0;

            // Grafik verileri
            ViewBag.KategoriDagilim = aktifUrunler
                .GroupBy(x => x.Kategori.KategoriAd)
                .Select(g => new { Kategori = g.Key, Sayi = g.Count() })
                .ToList();

            ViewBag.HaftalikSatis = _context.SatisHarekets
                .Where(x => x.Tarih >= son7Gun && x.Urun.Durum == true)
                .GroupBy(x => DbFunctions.TruncateTime(x.Tarih))
                .Select(g => new { Tarih = g.Key, Toplam = g.Sum(x => x.ToplamTutar) })
                .OrderBy(x => x.Tarih)
                .ToList();

            ViewBag.AylikFinansal = new
            {
                Gelir = _context.SatisHarekets
                    .Where(x => x.Tarih >= son30Gun && x.Urun.Durum == true)
                    .GroupBy(x => DbFunctions.TruncateTime(x.Tarih))
                    .Select(g => new { Tarih = g.Key, Toplam = g.Sum(x => x.ToplamTutar) })
                    .Union(_context.Tahsilats
                        .Where(x => x.TahsilatTarihi >= son30Gun)
                        .GroupBy(x => DbFunctions.TruncateTime(x.TahsilatTarihi))
                        .Select(g => new { Tarih = g.Key, Toplam = g.Sum(x => x.TahsilatMiktari) }))
                    .GroupBy(x => x.Tarih)
                    .Select(g => new { Tarih = g.Key, Toplam = g.Sum(x => x.Toplam) })
                    .OrderBy(x => x.Tarih)
                    .ToList(),

                Gider = _context.Giders
                    .Where(x => x.Tarih >= son30Gun)
                    .GroupBy(x => DbFunctions.TruncateTime(x.Tarih))
                    .Select(g => new { Tarih = g.Key, Toplam = g.Sum(x => x.Tutar) })
                    .OrderBy(x => x.Tarih)
                    .ToList()
            };

            return View();
        }

        public ActionResult KolayTablolar()
        {
            var viewModel = new HizliBakisViewModel
            {
                SehirDagilim = _context.Carilers.Where(x => x.Durum == true)
                    .GroupBy(x => x.CariSehir)
                    .Select(g => new SinifGrup
                    {
                        Sehir = g.Key,
                        Sayi = g.Count()
                    })
                    .OrderByDescending(x => x.Sayi)
                    .ToList(),

                KategoriDagilim = _context.Uruns.Where(x => x.Durum == true)
                    .GroupBy(x => x.Kategori.KategoriAd)
                    .Select(g => new SinifGrup
                    {
                        Kategori = g.Key,
                        Sayi = g.Count()
                    })
                    .OrderByDescending(x => x.Sayi)
                    .ToList(),

                EnBorcluCariler = _context.Carilers.Where(x => x.Durum == true)
                    .Where(x => x.ToplamBorc > 0)
                    .OrderByDescending(x => x.ToplamBorc)
                    .Take(5)
                    .Select(x => new CariBorcluViewModel
                    {
                        CariId = x.CariId,
                        CariAd = x.CariAd,
                        CariSoyad = x.CariSoyad,
                        ToplamBorc = x.ToplamBorc
                    })
                    .ToList(),

                SonGiderler = _context.Giders
                    .OrderByDescending(x => x.Tarih)
                    .Take(5)
                    .Select(x => new GiderViewModel
                    {
                        GiderId = x.GiderId,
                        GiderAciklama = x.GiderAciklama,
                        Tutar = x.Tutar,
                        Tarih = x.Tarih
                    })
                    .ToList(),

                SonTahsilatlar = _context.Tahsilats
                    .OrderByDescending(x => x.TahsilatTarihi)
                    .Take(5)
                    .Select(x => new TahsilatViewModel
                    {
                        TahsilatId = x.TahsilatId,
                        TahsilatMiktari = x.TahsilatMiktari,
                        TahsilatTarihi = x.TahsilatTarihi,
                        OdemeTuru = x.OdemeTuru,
                        CariAd = x.Cariler.CariAd + (string.IsNullOrEmpty(x.Cariler.CariSoyad) ? "" : " " + x.Cariler.CariSoyad)
                    })
                    .ToList()
            };

            return View(viewModel);
        }

        public PartialViewResult Partial1()
        {
            var departmanDagilim = _context.Personels
                .GroupBy(x => x.Departman.DepartmanAd)
                .Select(g => new SinifGrup2
                {
                    Departman = g.Key,
                    Sayi = g.Count()
                })
                .OrderByDescending(x => x.Sayi)
                .ToList();
            return PartialView(departmanDagilim);
        }

        public PartialViewResult Partial2()
        {
            var cariler = _context.Carilers.Where(x => x.Durum == true)
                .Select(x => new
                {
                    x.CariId,
                    x.CariAd,
                    x.CariSoyad,
                    x.CariSehir,
                    x.CariMail,
                    x.ToplamBorc
                })
                .ToList();
            return PartialView(cariler);
        }

        public PartialViewResult Partial3()
        {
            var urunler = _context.Uruns.Where(x => x.Durum == true)
                .Include(x => x.Kategori)
                .Select(x => new
                {
                    x.UrunId,
                    x.UrunAd,
                    x.Marka,
                    x.Stok,
                    x.AlisFiyat,
                    x.SatisFiyat,
                    KategoriAd = x.Kategori.KategoriAd
                })
                .ToList();
            return PartialView(urunler);
        }

        public PartialViewResult Partial4()
        {
            var markaDagilim = _context.Uruns.Where(x => x.Durum == true)
                .GroupBy(x => x.Marka)
                .Select(g => new SinifGrup3
                {
                    marka = g.Key,
                    sayi = g.Count(),
                    toplamStok = g.Sum(x => (int)x.Stok)
                })
                .OrderByDescending(x => x.sayi)
                .ToList();
            return PartialView(markaDagilim);
        }
    }
}