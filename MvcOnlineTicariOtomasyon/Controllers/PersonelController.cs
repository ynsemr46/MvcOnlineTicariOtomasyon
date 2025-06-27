using MvcOnlineTicariOtomasyon.Models.Siniflar;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace MvcOnlineTicariOtomasyon.Controllers
{
    [Authorize]
    [Authorize(Roles = "A,P")]
    public class PersonelController : Controller
    {
        private readonly Context c = new Context();

        public ActionResult Index(string p = null)
        {
            var degerler = c.Personels.AsQueryable();
            if (!string.IsNullOrEmpty(p))
            {
                degerler = degerler.Where(x => x.PersonelAd.Contains(p) || x.PersonelSoyad.Contains(p));
            }
            return View(degerler.ToList());
        }

        [Authorize(Roles = "A")]
        [HttpGet]
        public ActionResult PersonelEkle(Departman dp)
        {
            List<SelectListItem> deger1 = (from x in c.Departmans.Where(x => x.Durum == true).ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.DepartmanAd,
                                               Value = x.DepartmanId.ToString()
                                           }).ToList();
            dp.Durum = true; // Varsayılan durum
            ViewBag.dgr1 = deger1;

            return View();
        }

        [HttpPost]
        public ActionResult PersonelEkle(Personel p, Departman dp)
        {
            if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
            {
                string dosyaadi = Path.GetFileName(Request.Files[0].FileName);
                string uzanti = Path.GetExtension(Request.Files[0].FileName);
                string yol = "~/Image/" + dosyaadi + uzanti;
                Request.Files[0].SaveAs(Server.MapPath(yol));
                p.PersonelGorsel = "/Image/" + dosyaadi + uzanti;
            }
            if (Request.Files["PersonelGorsel"]?.ContentLength > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("PersonelGorsel", "Dosya boyutu 2MB'dan büyük olamaz.");
                return View(p);
            }
            p.Yetki = "P"; // Varsayılan Personel
            dp.Durum = true; // Varsayılan durum
            c.Personels.Add(p);
            c.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult PersonelGetir(int id)
        {
            List<SelectListItem> deger1 = (from x in c.Departmans.Where(x => x.Durum == true).ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.DepartmanAd,
                                               Value = x.DepartmanId.ToString()
                                           }).ToList();
            ViewBag.dgr1 = deger1;
            var prs = c.Personels.Find(id);
            return View(prs);
        }

        [HttpPost]
        public ActionResult PersonelGuncelle(Personel p, Departman dp)
        {
            var prsn = c.Personels.Find(p.PersonelId);
            prsn.PersonelAd = p.PersonelAd;
            prsn.PersonelSoyad = p.PersonelSoyad;
            prsn.KullaniciAd = p.KullaniciAd;
            prsn.Sifre = p.Sifre; // Üretimde hash'le
            prsn.Yetki = p.Yetki; // Yetki formdan geliyor, varsayılan değer kaldırıldı
            prsn.Yetki = p.Yetki ?? "P";
            prsn.DepartmanId = p.DepartmanId;
            dp.Durum = true; // Varsayılan durum

            if (Request.Files["PersonelGorsel"]?.ContentLength > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("PersonelGorsel", "Dosya boyutu 2MB'dan büyük olamaz.");
                return View(p);
            }
            if (Request.Files["PersonelGorsel"]?.ContentLength > 0 && !new[] { "image/jpeg", "image/png", "image/gif" }.Contains(Request.Files["PersonelGorsel"].ContentType))
            {
                ModelState.AddModelError("PersonelGorsel", "Sadece JPEG, PNG veya GIF dosyaları yüklenebilir.");
                return View(p);
            }

            if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
            {
                string dosyaadi = Path.GetFileName(Request.Files[0].FileName);
                string uzanti = Path.GetExtension(Request.Files[0].FileName);
                string yol = "~/Image/" + dosyaadi + uzanti;
                Request.Files[0].SaveAs(Server.MapPath(yol));
                prsn.PersonelGorsel = "/Image/" + dosyaadi + uzanti;
            }
            else
            {
                prsn.PersonelGorsel = p.PersonelGorsel;
            }

            c.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult KendiProfilGuncelle()
        {
            var personelId = Session["PersonelId"]?.ToString();
            if (string.IsNullOrEmpty(personelId) || !int.TryParse(personelId, out int id))
            {
                return RedirectToAction("Index", "Login");
            }

            List<SelectListItem> deger1 = (from x in c.Departmans.Where(x => x.Durum == true).ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.DepartmanAd,
                                               Value = x.DepartmanId.ToString()
                                           }).ToList();
            ViewBag.dgr1 = deger1;

            var prs = c.Personels.Find(id);
            if (prs == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View(prs);
        }

        [HttpPost]
        public ActionResult KendiProfilGuncelle(Personel p)
        {
            var personelId = Session["PersonelId"]?.ToString();
            if (string.IsNullOrEmpty(personelId) || !int.TryParse(personelId, out int id) || p.PersonelId != id)
            {
                return RedirectToAction("Index", "Login");
            }

            var prsn = c.Personels.Find(p.PersonelId);
            if (prsn == null)
            {
                return RedirectToAction("Index", "Login");
            }

            prsn.PersonelAd = p.PersonelAd;
            prsn.PersonelSoyad = p.PersonelSoyad;
            prsn.KullaniciAd = p.KullaniciAd;
            if (!string.IsNullOrEmpty(p.Sifre))
            {
                prsn.Sifre = p.Sifre; // Üretimde hash'le
            }
            prsn.DepartmanId = p.DepartmanId;

            if (Request.Files["PersonelGorsel"]?.ContentLength > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("PersonelGorsel", "Dosya boyutu 2MB'dan büyük olamaz.");
                List<SelectListItem> deger1 = (from x in c.Departmans.Where(x => x.Durum == true).ToList()
                                               select new SelectListItem
                                               {
                                                   Text = x.DepartmanAd,
                                                   Value = x.DepartmanId.ToString()
                                               }).ToList();
                ViewBag.dgr1 = deger1;
                return View(p);
            }
            if (Request.Files["PersonelGorsel"]?.ContentLength > 0 && !new[] { "image/jpeg", "image/png", "image/gif" }.Contains(Request.Files["PersonelGorsel"].ContentType))
            {
                ModelState.AddModelError("PersonelGorsel", "Sadece JPEG, PNG veya GIF dosyaları yüklenebilir.");
                List<SelectListItem> deger1 = (from x in c.Departmans.Where(x => x.Durum == true).ToList()
                                               select new SelectListItem
                                               {
                                                   Text = x.DepartmanAd,
                                                   Value = x.DepartmanId.ToString()
                                               }).ToList();
                ViewBag.dgr1 = deger1;
                return View(p);
            }

            if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
            {
                string dosyaadi = Path.GetFileName(Request.Files[0].FileName);
                string uzanti = Path.GetExtension(Request.Files[0].FileName);
                string yol = "~/Image/" + dosyaadi + uzanti;
                Request.Files[0].SaveAs(Server.MapPath(yol));
                prsn.PersonelGorsel = "/Image/" + dosyaadi + uzanti;
            }

            c.SaveChanges();
            return RedirectToAction("Index", "Kategori");
        }

        public ActionResult PersonelListe()
        {
            var sorgu = c.Personels.ToList();
            return View(sorgu);
        }
    }
}