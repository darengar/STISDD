using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EMR.Models;

namespace EMR.Controllers
{
    public class DoctorController : Controller
    {
        private Entities db = new Entities();

        public ActionResult Index()
        {
            return View(db.Doctors.ToList());
        }                

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }            
            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
         public ActionResult Edit([Bind(Include = "DoctorId,HospitalId,FirstName,LastName,Phone,CellPhone,EMail,LoginId,Address")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(doctor).State = EntityState.Modified;                
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(doctor);
        }
        
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Doctor doctor = db.Doctors.Find(id);
            db.Doctors.Remove(doctor);
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
