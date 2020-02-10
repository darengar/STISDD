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
    public class CollegianController : Controller
    {
        private Entities db = new Entities();

        // GET: /Collegian/
        public ActionResult Index()
        {
            return View(db.Collegians.ToList());
        }

        // GET: /Collegian/Details/5
        public ActionResult Details(int? id)
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

        // GET: /Collegian/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Collegian/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="CollegianId,FirstName,LastName,FullName,StudentId,Phone,CellPhone,Address,LoginId,EMail")] Collegian collegian)
        {
            if (ModelState.IsValid)
            {
                db.Collegians.Add(collegian);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(collegian);
        }

        // GET: /Collegian/Edit/5
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

        // POST: /Collegian/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="CollegianId,FirstName,LastName,FullName,StudentId,Phone,CellPhone,Address,LoginId,EMail")] Collegian collegian)
        {
            if (ModelState.IsValid)
            {
                db.Entry(collegian).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(collegian);
        }

        // GET: /Collegian/Delete/5
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

        // POST: /Collegian/Delete/5
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
