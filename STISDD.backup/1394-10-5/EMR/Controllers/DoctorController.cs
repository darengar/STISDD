using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EMR.Models;

namespace EMR.Controllers
{
    public class DoctorController : Controller
    {
        private Entities db = new Entities();

        // GET: /Doctor/
        public ActionResult Index()
        {
            var doctors = db.Doctors.Include(d => d.Hospital);
            return View(doctors.ToList());
        }                

        // GET: /Doctor/Edit/5
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
            //ViewBag.HospitalId = new SelectList(db.Hospitals, "HospitalId", "Name", doctor.HospitalId);
            return View(doctor);
        }

        // POST: /Doctor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
         public ActionResult Edit([Bind(Include = "DoctorId,HospitalId,FirstName,LastName,FullName,Phone,CellPhone,EMail,LoginId,Address")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(doctor).State = EntityState.Modified;                
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            //ViewBag.HospitalId = new SelectList(db.Hospitals, "HospitalId", "Name", doctor.HospitalId);
            return View(doctor);
        }

        // GET: /Doctor/Delete/5
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

        // POST: /Doctor/Delete/5
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
