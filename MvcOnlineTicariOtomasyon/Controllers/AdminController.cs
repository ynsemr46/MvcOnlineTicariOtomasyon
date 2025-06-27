using MvcOnlineTicariOtomasyon.Models.Siniflar;
using System.Linq;
using System.Web.Mvc;

namespace MvcOnlineTicariOtomasyon.Controllers
{
    [Authorize]
    [Authorize(Roles = "A")] // Sadece Admin rolü erişebilir
    public class AdminController : Controller
    {
        private readonly Context c = new Context();

        // Admin Listesi
        public ActionResult Index(string p = null)
        {
            var degerler = c.Admins.AsQueryable();
            if (!string.IsNullOrEmpty(p))
            {
                degerler = degerler.Where(x => x.KullaniciAd.Contains(p));
            }
            return View(degerler.ToList());
        }

        // Yeni Admin Ekleme (GET)
        [HttpGet]
        public ActionResult AdminEkle()
        {
            return View();
        }

        // Yeni Admin Ekleme (POST)
        [HttpPost]
        public ActionResult AdminEkle(Admin admin)
        {
            if (!ModelState.IsValid)
            {
                return View(admin);
            }

            admin.Yetki = "A"; // Varsayılan olarak Admin yetkisi
            c.Admins.Add(admin);
            c.SaveChanges();
            return RedirectToAction("Index");
        }

        // Admin Güncelleme (GET)
        [HttpGet]
        public ActionResult AdminGetir(int id)
        {
            var admin = c.Admins.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // Admin Güncelleme (POST)
        [HttpPost]
        public ActionResult AdminGuncelle(Admin admin)
        {
            if (!ModelState.IsValid)
            {
                return View("AdminGetir", admin);
            }

            var mevcutAdmin = c.Admins.Find(admin.AdminId);
            if (mevcutAdmin == null)
            {
                return HttpNotFound();
            }

            mevcutAdmin.KullaniciAd = admin.KullaniciAd;
            mevcutAdmin.Sifre = admin.Sifre; // Üretimde şifreyi hash'le
            mevcutAdmin.Yetki = "A"; // Yetki sabit olarak "A"
            c.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}