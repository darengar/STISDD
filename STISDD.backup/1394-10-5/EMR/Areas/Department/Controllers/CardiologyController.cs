using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EMR.Models;
using PagedList;
using System.Threading.Tasks;

namespace EMR.Areas.Department.Controllers
{
    public class CardiologyController : Controller
    {
        private Entities db = new Entities();
        private ShafaEntities hisdb = new ShafaEntities();

        public ActionResult DHPatient(int? departmentId, string SearchBy, string Search, int? page)
        {
            ViewBag.DepartmentId = departmentId;

            var cardiologyPatientAdm = hisdb.EMRCardiologyViews;
            if (SearchBy == "Family")
            {
                return View(cardiologyPatientAdm.OrderByDescending(x => x.ID_Code).Where(x => x.Family == Search || Search == null).ToList().ToPagedList(page ?? 1, 20));
            }
            else if (SearchBy == "NationalCode")
            {
                return View(cardiologyPatientAdm.OrderByDescending(x => x.ID_Code).Where(x => x.NationalCode == Search || Search == null).ToList().ToPagedList(page ?? 1, 20));
            }
            else
            {
                return View(cardiologyPatientAdm.OrderByDescending(x => x.ID_Code).ToList().ToPagedList(page ?? 1, 20));
            }
        }

        public ActionResult AjaxPage(int? page)
        {
            var patientlist = hisdb.EMRCardiologyViews.OrderByDescending(x => x.ID_Code).ToList().ToPagedList(page ?? 1, 20);
            return PartialView("_DHPatient", patientlist);
        }

        public async Task<ActionResult> Hospitalization(int? patientId, int? departmentId, int? admissionId)
        {
            ViewBag.PatientId = patientId;
            ViewBag.DepartmentId = departmentId;
            ViewBag.AdmissionId = admissionId;

            var historyDiseas = db.HistoryDiseases.FirstOrDefault(x => x.PatientId == patientId);
            if (historyDiseas != null)
            {
                ViewBag.HistoryDiseas = historyDiseas;
            }                
            else
            {
                historyDiseas = new HistoryDiseas();
                historyDiseas.PatientId = patientId;
                historyDiseas.LeadTime = DateTime.Now;
                historyDiseas.Description = "";
                db.HistoryDiseases.Add(historyDiseas);
                await db.SaveChangesAsync();
                ViewBag.HistoryDiseas = historyDiseas;
            }
            
            var hospitalization = db.Hospitalizations.Where(x => x.AdmissionId == admissionId).FirstOrDefault();
            if (hospitalization != null)
            {
                ViewBag.HospitalizationId = hospitalization.HospitalizationId;
                ViewBag.ChiefComplaint = hospitalization.ChiefComplaints.ToList();
                ViewBag.VitalSign = hospitalization.VitalSigns.ToList();
                ViewBag.PhysicalExamination = hospitalization.PhysicalExaminations.ToList();
                ViewBag.Laboratory = hospitalization.Laboratories.ToList();
                ViewBag.Cardiology = hospitalization.Cardiologys.ToList();

                return View(hospitalization);                
            }
            else
            {
                var newHospitalization = new Hospitalization();
                newHospitalization.PatientId = patientId;
                newHospitalization.DepartmentId = departmentId;
                newHospitalization.AdmissionId = admissionId;
                newHospitalization.DateOfStay = DateTime.Now;
                if (Session["DoctorId"] != null)
                    newHospitalization.DoctorId = Convert.ToInt32(Session["DoctorId"]);
                if (Session["CollegianId"] != null)
                    newHospitalization.CollegianId = Convert.ToInt32(Session["CollegianId"]);
                db.Hospitalizations.Add(newHospitalization);
                db.SaveChanges();

                ViewBag.HospitalizationId = newHospitalization.HospitalizationId;

                var chiefComplaint = new ChiefComplaint();
                chiefComplaint.HospitalizationId = newHospitalization.HospitalizationId;
                chiefComplaint.LeadTime = DateTime.Now;
                db.ChiefComplaints.Add(chiefComplaint);
                await db.SaveChangesAsync();
               
                ViewBag.ChiefComplaint = newHospitalization.ChiefComplaints.ToList();           

                ViewBag.PhysicalExamination = newHospitalization.PhysicalExaminations.ToList();

                ViewBag.VitalSign = newHospitalization.VitalSigns.ToList();

                ViewBag.Laboratory = newHospitalization.Laboratories.ToList();

                ViewBag.Cardiology = newHospitalization.Cardiologys.ToList();

                return View(newHospitalization);
            }            
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cardiology cardiology = await db.Cardiologys.FindAsync(id);
            if (cardiology == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Details", cardiology);
        }

        public ActionResult Create(int? hospitalizationId)
        {
            ViewBag.HospitalizationId = hospitalizationId;
            return PartialView("_Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CardiologyId,HospitalizationId,ECG,Echocardiography,Angiography,PCI,EST,SPECTMPI,Diagnosis,Plan,Assistant,Intern,Trainee")] Cardiology cardiology, FormCollection fc)
        {            
            if (ModelState.IsValid)
            {
                cardiology.HospitalizationId = Convert.ToInt32(fc["HospitalizationId"].ToString());
                cardiology.LeadTime = DateTime.Now;
                db.Cardiologys.Add(cardiology);
                await db.SaveChangesAsync();
                return Json(new { success = true });            
            }
            ViewBag.HospitalizationId = fc["HospitalizationId"].ToString();
            return PartialView("_Create", cardiology);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cardiology cardiology = await db.Cardiologys.FindAsync(id);
            if (cardiology == null)
            {
                return HttpNotFound();
            }            
            return PartialView("_Edit", cardiology);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CardiologyId,HospitalizationId,ECG,Echocardiography,Angiography,PCI,EST,SPECTMPI,Diagnosis,Plan,Assistant,Intern,Trainee")] Cardiology cardiology)
        {                       
            if (ModelState.IsValid)
            {
                db.Entry(cardiology).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }            
            return PartialView("_Edit", cardiology);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cardiology cardiology = await db.Cardiologys.FindAsync(id);
            if (cardiology == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Delete", cardiology);
        }  

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Cardiology cardiology = await db.Cardiologys.FindAsync(id);
            db.Cardiologys.Remove(cardiology);
            await db.SaveChangesAsync();
            return Json(new { success = true });
        }        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                hisdb.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
