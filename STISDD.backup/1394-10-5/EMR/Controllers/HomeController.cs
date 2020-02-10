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

        public ActionResult Index()
        {            
            return View();
        }

        public ActionResult Clinic()
        {
            return View();
        }

        public ActionResult Department()
        {
            return View();
        }
               
        public ActionResult About()
        {
            ViewBag.Message = "برنامه ثبت اطلاعات بیماران دانشگاه علوم پزشگی استان گلستان";
            return View();
        }

        [Authorize(Roles="Admin")]
        public ActionResult Administrator()
        {            
            return View();
        }

        public ActionResult Help()
        {
            return View();
        }

        //Add Role
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