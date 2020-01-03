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
            int id = Convert.ToInt32(Session["ogretmenID"]);
            if (id.Equals(0))
            {
                RedirectToAction("Index", "Home");
            }
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
            int id = Convert.ToInt32(Session["ogretmenID"]);
            if (id == null)
            {
                RedirectToAction("Index", "Home");
            }

            ViewBag.dersler = DersGetir();
            ViewBag.konular = KonuGetir();

            var sorular = db.Soru.Where(x => x.soruOgretmenID.Equals(id)).Take(5).OrderByDescending(x => x.soruID).ToList();
            return View(sorular);
        }

        [HttpPost]
        public ActionResult AddQuestion(FormCollection form)
        {
            SinavSistemiEntities db = new SinavSistemiEntities();

            int id = Convert.ToInt32(Session["ogretmenID"]);
            Soru yeniSoru = new Soru()
            {
                soruMetin = form["soruMetin"],
                soruCevap = form["soruCevap"],
                soruYanlisCevap = form["soruYanlisCevap"],
                soruDersID = Convert.ToInt32(form["dersler"]),
                soruKonuID = Convert.ToInt32(form["konular"]),
                soruOgretmenID = id
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
            if (id.Equals(0))
            {
                RedirectToAction("Index", "Home");
            }
            var query = from o in db.Ogrenci
                             join k in db.Kullanıcı
                                on o.kullaniciID equals k.kullaniciID
                             where o.ogrenciOgrID == id
                             select new OgrenciKullanici
                             {
                                 ad = k.kullaniciAd,
                                 soyad = k.kullaniciSoyad,
                                 email = k.kullaniciEmail,
                                 tel = k.kullaniciTelefon,
                                 sinif = o.ogrenciSinif,
                                 seviye = o.ogrenciSeviye
                             };
            List<OgrenciKullanici> profiller = query.ToList();
            return View(profiller);
        }



        public ActionResult LogOut()
        {
            Session.Abandon();
            ToastrService.AddToUserQueue(new Toastr("Başarıyla Çıkış Yapıldı.", "Çıkış İşlemi", ToastrType.Info));
            return RedirectToAction("Index", "Home");
        }

    }
}