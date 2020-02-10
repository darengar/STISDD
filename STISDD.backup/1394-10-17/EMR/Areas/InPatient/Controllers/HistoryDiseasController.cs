using System;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using EMR.Models;
using System.Threading.Tasks;

namespace EMR.Areas.InPatient.Controllers
{
    public class HistoryDiseasController : Controller
    {
        private Entities db = new Entities();

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {                
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           
            HistoryDiseas historyDiseas = await db.HistoryDiseases.FindAsync(id);
            if (historyDiseas == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Details", historyDiseas);
        }       
        
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HistoryDiseas historyDiseas = await db.HistoryDiseases.FindAsync(id);
            if (historyDiseas == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Edit", historyDiseas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "HistoryDiseaseId,LeadTime,PatientId,Description")] HistoryDiseas historyDiseas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(historyDiseas).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("_Edit", historyDiseas);
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
