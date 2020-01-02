using SinavSistemi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SinavSistemi.Controllers
{
    public class TeacherController : Controller
    {
        SinavSistemiEntities db = new SinavSistemiEntities();
        // GET: Teacher
        public ActionResult Index()
        {
            return View();
        }

        public List<SelectListItem> DersGetir()
        {
            List<SelectListItem> dersler = new List<SelectListItem>();
            foreach (var item in db.Ders.ToList())
            {
                SelectListItem yeni = new SelectListItem();
                yeni.Text = item.dersAdi;
                yeni.Value = item.dersID.ToString();
                dersler.Add(yeni);
            }
            return dersler;
        }

        public List<SelectListItem> KonuGetir()
        {
            List<SelectListItem> konular = new List<SelectListItem>();
            var dersler = DersGetir();

            int secilenDers = Convert.ToInt32(dersler[0].Value);
            foreach (var item in db.Konu.Where(x => x.dersID.Equals(secilenDers)).ToList())
            {
                SelectListItem yeni = new SelectListItem();
                yeni.Text = item.konuAdi;
                yeni.Value = item.konuID.ToString();
                konular.Add(yeni);
            }
            return konular;
        }

        [HttpGet]
        public JsonResult GetMyData()
        {
            int id = Convert.ToInt32(Session["ogretmenID"]);
            var d = db.Ogretmen.FirstOrDefault(x => x.ogretmenID.Equals(id));
            if (d == null)
            {
                ToastrService.AddToUserQueue(new Toastr("Giriş Yapınız", "Kullanıcı Hatası", ToastrType.Info));
                RedirectToAction("Index", "Home");
            }
            var kullaniciDers = db.Ders.FirstOrDefault(x => x.dersID.Equals(d.ogretmenDersID)).dersAdi;
            var profileData = db.Kullanıcı.FirstOrDefault(x => x.kullaniciID.Equals(d.kullaniciID));
            var JsonModel = new
            {
                profilAd = profileData.kullaniciAd,
                profilSoyad = profileData.kullaniciSoyad,
                profilEmail = profileData.kullaniciEmail,
                profilTel = profileData.kullaniciTelefon,
                profilDers = kullaniciDers
            };

            return Json(JsonModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddQuestion()
        {

            ViewBag.dersler = DersGetir();
            ViewBag.konular = KonuGetir();

            return View();
        }

        [HttpPost]
        public ActionResult AddQuestion(FormCollection form)
        {
            SinavSistemiEntities db = new SinavSistemiEntities();

            int Id = Convert.ToInt32(Session["ogretmenID"]);
            Soru yeniSoru = new Soru()
            {
                soruMetin = form["soruMetin"],
                soruCevap = form["soruCevap"],
                soruYanlisCevap = form["soruYanlisCevap"],
                soruDersID = Convert.ToInt32(form["dersler"]),
                soruKonuID = Convert.ToInt32(form["konular"]),
                soruOgretmenID = Id

            };
            db.Soru.Add(yeniSoru);
            db.SaveChanges();

            ViewBag.dersler = DersGetir();
            ViewBag.konular = KonuGetir();
            return RedirectToAction("Index", "Teacher");
        }


        [HttpGet]
        public ActionResult GetStudents()
        {
            int id = Convert.ToInt32(Session["ogretmenID"]);
            if (id == 0 || id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<Ogrenci> ogrenciler = db.Ogrenci.Where(x => x.ogrenciOgrID.Equals(2)).ToList();
            List<Kullanıcı> kullanıcılar = new List<Kullanıcı>();
            foreach (var item in ogrenciler)
            {
                kullanıcılar.Add(db.Kullanıcı.FirstOrDefault(x => x.kullaniciID.Equals(item.kullaniciID)));
            }

            List<Profil> profiles = new List<Profil>();
            foreach (var item in kullanıcılar)
            {
                Profil p1 = new Profil();
                p1.ad = item.kullaniciAd;
                p1.soyad = item.kullaniciSoyad;
                p1.email = item.kullaniciEmail;

                foreach (var itemB in ogrenciler)
                {
                    p1.seviye = itemB.ogrenciSeviye;
                    p1.sinif = itemB.ogrenciSinif;
                }
                profiles.Add(p1);
            }

            return View(profiles);

        }





    }
}