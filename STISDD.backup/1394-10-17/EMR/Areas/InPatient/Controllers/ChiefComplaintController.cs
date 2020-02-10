using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using EMR.Models;

namespace EMR.Areas.InPatient.Controllers
{
    public class ChiefComplaintController : Controller
    {
        private Entities db = new Entities();      
        
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiefComplaint chiefComplaint = await db.ChiefComplaints.FindAsync(id);
            if (chiefComplaint == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Details", chiefComplaint);
        }        

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiefComplaint chiefcomplaint = await db.ChiefComplaints.FindAsync(id);
            if (chiefcomplaint == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Edit", chiefcomplaint);            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ChiefComplaintId,VisitId,LeadTime,ChestPain,Dyspnea,Syncope,Palpitation,Edema,ETC")] ChiefComplaint chiefcomplaint)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chiefcomplaint).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("_Edit", chiefcomplaint);
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
