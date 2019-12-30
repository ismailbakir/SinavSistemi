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
        [HttpGet]
        public ActionResult AddQuestion()
        {
            return View();
        }
    }
}