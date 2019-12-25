using System.Linq;
using System.Web.Mvc;


namespace SinavSistemi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GirisYap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GirisYap(FormCollection form)
        {
            string email = form["email"];
            string password = form["password"];

            SinavSistemiEntities db = new SinavSistemiEntities();
            var kullanicilar = db.Kullanıcı.ToList();
            var sonuc = kullanicilar.Where(x => x.kullaniciEmail.Equals(email) && x.kullaniciSifre.Equals(password)).FirstOrDefault();

            if (sonuc != null)
            {
                switch (sonuc.kullaniciTur)
                {
                    case true:
                        var ogretmen = db.Ogretmen.FirstOrDefault(x => x.kullaniciID.Equals(sonuc.kullaniciID));
                        Session.Add("ogretmenID", ogretmen.ogretmenID);
                        ToastrService.AddToUserQueue(new Toastr(message: "Başarılı Bir şekilde Giriş Yapıldı.", type: ToastrType.Success));
                        return RedirectToAction("Index", "Teacher");
                    case false:
                        var ogrenci = db.Ogrenci.FirstOrDefault(x => x.kullaniciID.Equals(sonuc.kullaniciID));
                        Session.Add("ogrenciID", ogrenci.ogrenciID);
                        ToastrService.AddToUserQueue(new Toastr(message: "Başarılı Bir şekilde Giriş Yapıldı.", type: ToastrType.Success));
                        return RedirectToAction("Index", "Student");
                    default:
                        ToastrService.AddToUserQueue(new Toastr(message: "Giriş Yapılamadı!", type: ToastrType.Error));
                        return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ToastrService.AddToUserQueue(new Toastr(message: "Giriş Yapılamadı!", type: ToastrType.Error));
                return RedirectToAction("Index", "Home");
            }
        }
    }
}