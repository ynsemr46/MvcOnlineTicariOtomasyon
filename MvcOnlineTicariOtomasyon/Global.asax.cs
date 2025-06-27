using System;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MvcOnlineTicariOtomasyon
{
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            GlobalFilters.Filters.Add(new AuthorizeAttribute());
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Kültür ayarlarýný InvariantCulture olarak ayarla
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;


        }
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var context = new Models.Siniflar.Context();
                var userName = HttpContext.Current.User.Identity.Name;
                string[] roles = new string[0];

                // Önce Admins tablosunu kontrol et (yüksek öncelik)
                var admin = context.Admins.FirstOrDefault(x => x.KullaniciAd == userName);
                if (admin != null && admin.Yetki == "A")
                {
                    roles = new[] { "A" };
                }
                else
                {
                    // Admins'de yoksa Personels tablosunu kontrol et
                    var personel = context.Personels.FirstOrDefault(x => x.KullaniciAd == userName);
                    if (personel != null)
                    {
                        roles = new[] { personel.Yetki }; // Yetki "P" veya "A" olabilir
                    }
                }

                // Yeni bir GenericPrincipal ile kullanýcýyý güncelle
                if (roles.Length > 0)
                {
                    HttpContext.Current.User = new GenericPrincipal(
                        new GenericIdentity(userName), roles);
                }
            }
        }
    }
}
