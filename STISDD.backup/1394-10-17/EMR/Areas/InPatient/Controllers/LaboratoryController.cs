using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using EMR.Models;
using System;

namespace EMR.Areas.InPatient.Controllers
{
    public class LaboratoryController : Controller
    {
        private Entities db = new Entities();

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Laboratory laboratory = await db.Laboratories.FindAsync(id);
            if (laboratory == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Details",laboratory);
        }

        public ActionResult Create(int? visitId)
        {
            ViewBag.HospitalizationId = visitId;
            return PartialView("_Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "LaboratoryId,VisitId,LeadTime,Mg,FBS,Hb1AC,BUN,Cr,Na,K,Ca,Chol,LDL,HDL,TG,VitOHD3,CTnI,CKMB,AST,ALT,ALP,BiL,PSA,TSH,T3,T4,T3RU")] Laboratory laboratory, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                laboratory.VisitId = Convert.ToInt32(fc["VisitId"].ToString());
                laboratory.LeadTime = DateTime.Now;
                db.Laboratories.Add(laboratory);
                await db.SaveChangesAsync();
                return Json(new { success = true });  
            }

            ViewBag.HospitalizationId = fc["VisitId"]; 
            return PartialView("_Create", laboratory);
        }
        
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Laboratory laboratory = await db.Laboratories.FindAsync(id);
            if (laboratory == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Edit", laboratory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="LaboratoryId,VisitId,LeadTime,Mg,FBS,Hb1AC,BUN,Cr,Na,K,Ca,Chol,LDL,HDL,TG,VitOHD3,CTnI,CKMB,AST,ALT,ALP,BiL,PSA,TSH,T3,T4,T3RU")] Laboratory laboratory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(laboratory).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("_Edit", laboratory);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Laboratory laboratory = await db.Laboratories.FindAsync(id);
            if (laboratory == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Delete", laboratory);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Laboratory laboratory = await db.Laboratories.FindAsync(id);
            db.Laboratories.Remove(laboratory);
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