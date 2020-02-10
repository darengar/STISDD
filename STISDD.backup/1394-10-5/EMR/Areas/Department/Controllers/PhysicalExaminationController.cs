using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using EMR.Models;

namespace EMR.Areas.Department.Controllers
{
    public class PhysicalExaminationController : Controller
    {
        private Entities db = new Entities();        
       
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhysicalExamination physicalExamination = await db.PhysicalExaminations.FindAsync(id);
            if (physicalExamination == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Details", physicalExamination);
        }

        public ActionResult Create(int? hospitalizationId)
        {
            ViewBag.HospitalizationId = hospitalizationId;
            return PartialView("_Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="PhysicalExaminationId,HospitalizationId,LeadTime,Heart,Lung,Extremities,ETC")] PhysicalExamination physicalExamination, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                physicalExamination.HospitalizationId = Convert.ToInt32(fc["HospitalizationId"].ToString());
                physicalExamination.LeadTime = DateTime.Now;        
                db.PhysicalExaminations.Add(physicalExamination);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            ViewBag.HospitalizationId = fc["HospitalizationId"].ToString();            
            return PartialView("_Create", physicalExamination);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhysicalExamination physicalExamination = await db.PhysicalExaminations.FindAsync(id);
            if (physicalExamination == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Edit", physicalExamination);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="PhysicalExaminationId,HospitalizationId,LeadTime,Heart,Lung,Extremities,ETC")] PhysicalExamination physicalExamination)
        {
            if (ModelState.IsValid)
            {
                db.Entry(physicalExamination).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("_Edit", physicalExamination);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhysicalExamination physicalExamination = await db.PhysicalExaminations.FindAsync(id);
            if (physicalExamination == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Delete", physicalExamination);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PhysicalExamination physicalExamination = await db.PhysicalExaminations.FindAsync(id);
            db.PhysicalExaminations.Remove(physicalExamination);
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
