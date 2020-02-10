using System.Web.Mvc;
using STISDD.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;

namespace STISDD.Controllers
{
    public class HomeController : Controller
    {
        //private Entities db = new Entities();       

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
    }
}