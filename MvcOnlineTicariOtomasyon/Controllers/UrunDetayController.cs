using MvcOnlineTicariOtomasyon.Models.Siniflar;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcOnlineTicariOtomasyon.Controllers
{
    [Authorize]
    [Authorize(Roles = "A,P")]
    public class UrunDetayController : Controller
    {
        private Context c = new Context();

        // GET: UrunDetay
        public ActionResult Index(int id)
        {
            Class1 cs = new Class1();
            var urun = c.Uruns.FirstOrDefault(x => x.UrunId == id);
            if (urun == null)
            {
                return HttpNotFound();
            }

            cs.Deger1 = new List<Urun> { urun };

            cs.Deger5 = c.UrunGorsels.Where(y => y.UrunId == id).ToList();


            ViewBag.Ozellikler = c.UrunOzelliks.Where(o => o.UrunId == id).ToList();

            return View(cs);
        }
    }
}