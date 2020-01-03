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
            if (ogrID.Equals(0))
            {
                RedirectToAction("Index", "Home");
            }
            //Giris Yapmıs Ogrencinin Ogretmen ID'si
            var ogr = db.Ogrenci.FirstOrDefault(x => x.ogrenciID.Equals(ogrID));
            var id = ogr.ogrenciOgrID;

            //Sorulari en son girlilen teste göre dengeli bir şekilde hazırlar.
            List<Denge> dengeler = dengele();

            //18 soru öğrencinin sevyesine göre getiriliyor. yanlış yaptığı konulara ağırlık veriliyor.
            List<Soru> sorular = new List<Soru>();
            for (int i = 2; i < 8; i++)
            {
                int konuID = dengeler[i - 2].knID;
                int sayi = dengeler[i - 2].soruSayisi;

                IEnumerable<Soru> kn = db.Soru.Where(
                    x => x.soruKonuID.Equals(konuID)
                    && x.soruSeviye < ogr.ogrenciSeviye)
                    .Take(sayi).ToList();

                sorular.AddRange(kn);
            }

            //18 soru rasgele öğrenci sevyesine göre getiriliyor. yanlış yaptığı konulara ağırlık veriliyor.
            int soruSeviye = 0;
            for (int i = 2; i < 8; i++)
            {
                soruSeviye = new Random().Next(0, 0);
                IEnumerable<Soru> kn = db.Soru.Where(x => x.soruKonuID.Equals(i) && x.soruSeviye.Equals(soruSeviye)).Take(3).ToList();
                sorular.AddRange(kn);
            }

            ToastrService.AddToUserQueue(new Toastr("Sorular Yüklendi.", "Sınav Sistemi", ToastrType.Info));

            //Sınav Sayfası Açılırken Soruların Goruntulenmesi icin View'e model olarak yolluyoruz.
            return View(sorular);
        }

        private List<Denge> dengele()
        {

            List<Denge> dengeler = new List<Denge>();
            for (int i = 2; i < 8; i++)
            {
                Denge denge = new Denge();
                sonCevaplar sonCevaplar = db.sonCevaplar.FirstOrDefault(x => x.id.Equals(i));
                denge.knID = sonCevaplar.id;
                denge.yanlissayisi = (int)sonCevaplar.yanlisAdeti;
                dengeler.Add(denge);
            }

            dengeler = dengeler.OrderByDescending(o => o.yanlissayisi).ToList();

            if (dengeler[0].yanlissayisi == 0) //ilk değer sıfırsa tüm değerler 0 demektir. Yani eşit dağıtılıyor.
            {
                for (int j = 0; j < 6; j++)
                {
                    dengeler[j].soruSayisi = 3;

                }
                return dengeler;
            }

            int kalanHak = 12;

            for (int i = 0; i < 6; ++i)
            {

                if (dengeler[i].yanlissayisi != 0 && kalanHak > 0)
                {
                    dengeler[i].soruSayisi++;
                    kalanHak--;
                }
            }

            for (int i = 0; i < 6; ++i)
            {
                if (dengeler[i].yanlissayisi != 0 && kalanHak > 0)
                {
                    if (i < 2)
                    {
                        dengeler[i].soruSayisi += 2;
                        kalanHak -= 2;
                    }
                    else
                    {
                        dengeler[i].soruSayisi += 1;
                        kalanHak -= 1;
                    }


                }
            }

            for (int i = 0; i < 6; ++i)
            {
                if (dengeler[i].yanlissayisi != 0 && kalanHak > 0)
                {
                    if (i < 1)
                    {
                        dengeler[i].soruSayisi += 3;
                        kalanHak -= 2;
                    }
                    else
                    {
                        dengeler[i].soruSayisi += 2;
                        kalanHak -= 1;
                    }


                }
            }

            return dengeler;

        }


        [HttpPost]
        public ActionResult Exam(CevapModel cevapModel)
        {
            //Giris Yapmıs Ogrenci ID'si
            int ogrID = Convert.ToInt32(Session["ogrenciID"]);

            //Giris Yapmıs Ogrencinin Ogretmen ID'si
            var ogr = db.Ogrenci.FirstOrDefault(x => x.ogrenciID.Equals(ogrID));
            var id = ogr.ogrenciOgrID;

            //Ogrencinin Ogretmeninin ekledigi sorulardan 10 tane getirme
            // Ogrencinin Ogretmeninin ekledigi sorulardan 10 tane getirme
            //var sorular = db.Soru.Where(x => x.soruOgretmenID.Equals(id)).Take(10).ToList();

            for (int i = 2; i < 8; i++)
            {
                db.sonCevaplar.FirstOrDefault(x => x.id.Equals(i)).yanlisAdeti = 0;
            }

            Sinav sinav = new Sinav();
            sinav.sinavTarih = DateTime.Now;
            sinav.ogrenciID = ogrID;
            sinav.sinavID = 0;
            sinav.sinavSonuc = 0;
            sinav.dersID = 1;
            sinav.yanlisSayisi = 0;
            sinav.dogruSayisi = 0;

            //Kullanıcının Sectigi Cevapları Alma

            foreach (var cevap in cevapModel.cevaplar)
            {
                Soru soru = db.Soru.FirstOrDefault(x => x.soruID.Equals(cevap.soruId));

                if (soru != null && soru.soruID == cevap.soruId)
                {
                    if (soru.soruCevap.Equals(cevap.verilenCevap))
                    {
                        sinav.dogruSayisi++;
                    }
                    else if (cevap.verilenCevap != null)
                    {
                        sinav.yanlisSayisi++;
                        db.sonCevaplar.FirstOrDefault(x => x.id.Equals(soru.soruKonuID)).yanlisAdeti++;
                    }

                }
            }

            db.Sinav.Add(sinav);
            db.SaveChanges();

            ToastrService.AddToUserQueue(new Toastr("Sınav Tamamlandı.", "Sınav Bitti.", ToastrType.Info));
            return RedirectToAction("Index", "Student");
        }

        public ActionResult LogOut()
        {
            Session.Abandon();
            ToastrService.AddToUserQueue(new Toastr("Başarıyla Çıkış Yapıldı.", "Çıkış İşlemi", ToastrType.Info));
            return RedirectToAction("Index", "Home");
        }
    }
}