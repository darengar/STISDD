using System;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using EMR.Models;
using System.Threading.Tasks;

namespace EMR.Areas.Department.Controllers
{
    public class VitalSignController : Controller
    {
        private Entities db = new Entities();

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VitalSign vitalSign = await db.VitalSigns.FindAsync(id);
            if (vitalSign == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Details", vitalSign);
        }

        public ActionResult Create(int? hospitalizationId)
        {
            ViewBag.HospitalizationId = hospitalizationId;
            return PartialView("_Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "VitalSignId,HospitalizationId,LeadTime,BP,HR,RR")] VitalSign vitalsign, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                vitalsign.HospitalizationId = Convert.ToInt32(fc["HospitalizationId"].ToString());
                vitalsign.LeadTime = DateTime.Now;
                db.VitalSigns.Add(vitalsign);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            ViewBag.HospitalizationId = fc["HospitalizationId"].ToString();
            return PartialView("_Create", vitalsign);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VitalSign vitalsign = await db.VitalSigns.FindAsync(id);
            if (vitalsign == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Edit", vitalsign);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "VitalSignId,HospitalizationId,LeadTime,BP,HR,RR")] VitalSign vitalsign)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vitalsign).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("_Edit", vitalsign);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VitalSign vitalsign = await db.VitalSigns.FindAsync(id);
            if (vitalsign == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Delete", vitalsign);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            VitalSign vitalSign = db.VitalSigns.Find(id);
            db.VitalSigns.Remove(vitalSign);
            await db.SaveChangesAsync();
            return Json(new { success = true });
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
