using System.Web.Mvc;
using EMR.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;

namespace EMR.Controllers
{
    public class HomeController : Controller
    {
        private Entities db = new Entities();
        private ShafaEntities hisdb = new ShafaEntities();

        [HttpGet]
        public ActionResult Index()
        {            
            return View();
        }

        [HttpGet]
        public ActionResult InPatient()
        {
            return View();
        }

        [HttpGet]
        public ActionResult OutPatient()
        {
            return View();
        }
         
        [HttpGet]
        public ActionResult About()
        {
            ViewBag.Message = "برنامه ثبت اطلاعات بیماران دانشگاه علوم پزشگی استان گلستان";
            return View();
        }

        [HttpGet]
        [Authorize(Roles="Admin")]
        public ActionResult Administrator()
        {            
            return View();
        }

        [HttpGet]
        public ActionResult Help()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddRole()
        {
            ApplicationDbContext ac = new ApplicationDbContext();
            IdentityRole r = new IdentityRole();
            r.Name = "Admin";
            ac.Roles.Add(r);
            r = null;
            r = new IdentityRole();
            r.Name = "Doctor";
            ac.Roles.Add(r);
            r = null;
            r = new IdentityRole();
            r.Name = "Collegian";
            ac.Roles.Add(r);
            ac.SaveChanges();
            return RedirectToAction("Index");
        }        
    }
}