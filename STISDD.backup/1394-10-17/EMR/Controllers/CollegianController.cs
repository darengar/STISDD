using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EMR.Models;

namespace EMR.Controllers
{
    public class CollegianController : Controller
    {
        private Entities db = new Entities();

        public ActionResult Index()
        {
            return View(db.Collegians.ToList());
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Collegian collegian = db.Collegians.Find(id);
            if (collegian == null)
            {
                return HttpNotFound();
            }
            return View(collegian);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CollegianId,UniversityId,FirstName,LastName,StudentId,Phone,CellPhone,Address,LoginId,EMail")] Collegian collegian)
        {
            if (ModelState.IsValid)
            {
                db.Entry(collegian).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(collegian);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Collegian collegian = db.Collegians.Find(id);
            if (collegian == null)
            {
                return HttpNotFound();
            }
            return View(collegian);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Collegian collegian = db.Collegians.Find(id);
            db.Collegians.Remove(collegian);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
