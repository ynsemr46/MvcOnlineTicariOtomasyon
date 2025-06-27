using MvcOnlineTicariOtomasyon.Models.Siniflar;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace MvcOnlineTicariOtomasyon.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly Context c = new Context();

        // GET: Giriş Sayfası
        public ActionResult Index()
        {
            return View();
        }

        // POST: Giriş
        [HttpPost]
        public ActionResult Index(string KullaniciAd, string Sifre)
        {
            // Admin kontrolü
            var admin = c.Admins.FirstOrDefault(x => x.KullaniciAd == KullaniciAd && x.Sifre == Sifre);
            if (admin != null)
            {
                FormsAuthentication.SetAuthCookie(admin.KullaniciAd, false);
                Session["UserType"] = "Admin";
                Session["KullaniciAd"] = admin.KullaniciAd;
                return RedirectToAction("Index", "Kategori");
            }

            // Personel kontrolü
            var personel = c.Personels.FirstOrDefault(x => x.KullaniciAd == KullaniciAd && x.Sifre == Sifre);
            if (personel != null)
            {
                FormsAuthentication.SetAuthCookie(personel.KullaniciAd, false);
                Session["UserType"] = "Personel";
                Session["KullaniciAd"] = personel.KullaniciAd;
                Session["PersonelId"] = personel.PersonelId;
                return RedirectToAction("Index", "Kategori");
            }

            ViewBag.Error = "Geçersiz kullanıcı adı veya şifre.";
            return View();
        }

        // Çıkış
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index");
        }
    }
}