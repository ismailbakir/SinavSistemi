using System.Linq;
using System.Web.Mvc;


namespace SinavSistemi.Controllers
{
    public class HomeController : Controller
    {
        SinavSistemiEntities db = new SinavSistemiEntities();

        public ActionResult Index()
        {
            return View();
        }

        public Kullanıcı KullaniciGetir(FormCollection form)
        {
            string email = form["email"];
            string password = form["password"];

            var kullanicilar = db.Kullanıcı.ToList();
            var sonuc = kullanicilar.Where(x => x.kullaniciEmail.Equals(email) && x.kullaniciSifre.Equals(password)).FirstOrDefault();
            return sonuc;
        }
        [HttpGet]
        public ActionResult OgrenciGiris()
        {
            return View();
        }

        [HttpPost]
        public ActionResult OgrenciGiris(FormCollection form)
        {
            var kullanici = KullaniciGetir(form);

            if (kullanici != null && kullanici.kullaniciTur.Equals(false))
            {
                var ogrenci = db.Ogrenci.FirstOrDefault(x => x.kullaniciID.Equals(kullanici.kullaniciID));
                Session.Add("ogrenciID", ogrenci.ogrenciID);
                ToastrService.AddToUserQueue(new Toastr("Başarılı Bir şekilde Giriş Yapıldı.","Giriş Durumu",ToastrType.Success));
                return RedirectToAction("Index", "Student");
            }
            else
            {
                ToastrService.AddToUserQueue(new Toastr("Giriş Yapılamadı!", "Giriş Durumu", ToastrType.Error));
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult OgretmenGiris()
        {
            return View();
        }

        [HttpPost]
        public ActionResult OgretmenGiris(FormCollection form)
        {
            var kullanici = KullaniciGetir(form);

            if (kullanici != null && kullanici.kullaniciTur.Equals(true))
            {
                var ogretmen = db.Ogretmen.FirstOrDefault(x => x.kullaniciID.Equals(kullanici.kullaniciID));
                Session.Add("ogretmenID", ogretmen.ogretmenID);
                ToastrService.AddToUserQueue(new Toastr("Başarılı Bir şekilde Giriş Yapıldı.", "Giriş Durumu", ToastrType.Success));
                return RedirectToAction("Index", "Teacher");
            }
            else
            {
                ToastrService.AddToUserQueue(new Toastr("Giriş Yapılamadı!", "Giriş Durumu", ToastrType.Error));
                return RedirectToAction("Index", "Home");
            }
        }
    }
}