using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SinavSistemi.Controllers
{
    public class StudentController : Controller
    {
        SinavSistemiEntities db = new SinavSistemiEntities();

        // GET: Student
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetMyData()
        {
            int id = Convert.ToInt32(Session["ogrenciID"]);
            var d = db.Ogrenci.FirstOrDefault(x => x.ogrenciID.Equals(id));
            if (d == null)
            {
                ToastrService.AddToUserQueue(new Toastr("Lütfen Giriş Yapınız", "Kullanıcı Hatası", ToastrType.Info));
                RedirectToAction("Index", "Home");
            }
            var profileData = db.Kullanıcı.FirstOrDefault(x => x.kullaniciID.Equals(d.kullaniciID));
            var JsonModel = new
            {
                profilAd = profileData.kullaniciAd,
                profilSoyad = profileData.kullaniciSoyad,
                profilEmail = profileData.kullaniciEmail,
                profilTel = profileData.kullaniciTelefon,
                profilSinif = d.ogrenciSinif,
                profilSeviye = d.ogrenciSeviye
            };

            return Json(JsonModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Exam()
        {
            //Giris Yapmıs Ogrenci ID'si
            int ogrID = Convert.ToInt32(Session["ogrenciID"]);
            
            //Giris Yapmıs Ogrencinin Ogretmen ID'si
            var ogr = db.Ogrenci.FirstOrDefault(x => x.ogrenciID.Equals(ogrID));
            var id = ogr.ogrenciOgrID;
            
            //Ogrencinin Ogretmeninin ekledigi sorulardan 10 tane getirme
            var sorular = db.Soru.Where(x => x.soruOgretmenID.Equals(id)).Take(10).ToList();
            ToastrService.AddToUserQueue(new Toastr("Sorular Yüklendi.", "Sınav Sistemi", ToastrType.Info));

            //Sınav Sayfası Açılırken Soruların Goruntulenmesi icin View'e model olarak yolluyoruz.
            return View(sorular);
        }

        [HttpPost]
        public ActionResult Exam(FormCollection form)
        {
            //Giris Yapmıs Ogrenci ID'si
            int ogrID = Convert.ToInt32(Session["ogrenciID"]);

            //Giris Yapmıs Ogrencinin Ogretmen ID'si
            var ogr = db.Ogrenci.FirstOrDefault(x => x.ogrenciID.Equals(ogrID));
            var id = ogr.ogrenciOgrID;

            //Ogrencinin Ogretmeninin ekledigi sorulardan 10 tane getirme
            var sorular = db.Soru.Where(x => x.soruOgretmenID.Equals(id)).Take(10).ToList();

            //Kullanıcının Sectigi Cevapları Alma
            List<string> cevaplar = new List<string>();
            foreach (var item in sorular)
            {
                if (form[item.soruID.ToString()] == null)
                {
                    cevaplar.Add("NaN");
                    continue;
                }
                cevaplar.Add(form[item.soruID.ToString()]);
            }

            /*
             * Veritabanındaki dogru cevaplar ile kullanıcının verdigi cevaplar karsılastirilacak
             * Sinav olusturulup dogru yanlıs sayısı tutulacak
             */

            ToastrService.AddToUserQueue(new Toastr("Sınav Tamamlandı.", "Sınav Bitti.", ToastrType.Info));
            return RedirectToAction("Index", "Student");
        }
    }
}