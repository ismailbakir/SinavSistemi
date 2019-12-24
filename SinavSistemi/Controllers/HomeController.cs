using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
                return RedirectToAction("Index","Teacher");
            }
            else
                return RedirectToAction("Index","Home");
        }


    }
}