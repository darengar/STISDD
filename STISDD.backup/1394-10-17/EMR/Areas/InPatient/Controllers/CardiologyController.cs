using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EMR.Models;
using PagedList;
using System.Threading.Tasks;

namespace EMR.Areas.InPatient.Controllers
{
    public class CardiologyController : Controller
    {
        private Entities db = new Entities();
        private ShafaEntities hisdb = new ShafaEntities();

        public ActionResult DHPatient(int? clinicId, string SearchBy, string Search, int? page)
        {
            ViewBag.ClinicId = clinicId;

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

        public async Task<ActionResult> Hospitalization(int? patientId, int? clinicId, int? admissionId)
        {
            ViewBag.PatientId = patientId;
            ViewBag.ClinicId = clinicId;
            ViewBag.AdmissionId = admissionId;

            ViewBag.ClinicName = "قلب"; //db.Clinics.Where(c => c.ClinicId == clinicId).FirstOrDefault().Name;
            string patientName = hisdb.AdmIDs.Where(p => p.ID_Code == patientId).FirstOrDefault().Name; // db.Patients.Where(p => p.PatientId == patientId).FirstOrDefault().FullName;
            patientName += " ";
            patientName += patientName = hisdb.AdmIDs.Where(p => p.ID_Code == patientId).FirstOrDefault().Family;
            ViewBag.PatientName = patientName;             

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

            Visit visit = null;// db.Visits.Where(x => x.AdmissionId == admissionId).ToList();
            if (Session["DoctorId"] != null)
            {
                int docId = Convert.ToInt32(Session["DoctorId"]);
                visit = db.Visits.Where(x => (x.AdmissionId == admissionId && x.KindOfTherapyId == 3 && x.DoctorId == docId)).FirstOrDefault();
            }  
            if (Session["CollegianId"] != null)
            {
                int collId = Convert.ToInt32(Session["CollegianId"]);
                visit = db.Visits.Where(x => (x.AdmissionId == admissionId && x.KindOfTherapyId == 3 && x.CollegianId == collId)).FirstOrDefault();
            }        
      
            if (visit != null)
            {
                ViewBag.VisitId = visit.VisitId;
                ViewBag.ChiefComplaint = visit.ChiefComplaints.ToList();
                ViewBag.VitalSign = visit.VitalSigns.ToList();
                ViewBag.PhysicalExamination = visit.PhysicalExaminations.ToList();
                ViewBag.Laboratory = visit.Laboratories.ToList();
                ViewBag.Cardiology = visit.Cardiologys.ToList();

                return View(visit);                
            }
            else
            {
                visit = new Visit();
                visit.PatientId = patientId;
                visit.ClinicId = clinicId;
                visit.KindOfTherapyId = 3;//inpatient
                visit.AdmissionId = admissionId;
                visit.DateOfStay = DateTime.Now;
                if (Session["DoctorId"] != null)
                    visit.DoctorId = Convert.ToInt32(Session["DoctorId"]);
                if (Session["CollegianId"] != null)
                    visit.CollegianId = Convert.ToInt32(Session["CollegianId"]);
                db.Visits.Add(visit);
                db.SaveChanges();

                ViewBag.VisitId = visit.VisitId;

                var chiefComplaint = new ChiefComplaint();
                chiefComplaint.VisitId = visit.VisitId;
                chiefComplaint.LeadTime = DateTime.Now;
                db.ChiefComplaints.Add(chiefComplaint);
                await db.SaveChangesAsync();
               
                ViewBag.ChiefComplaint = visit.ChiefComplaints.ToList();           

                ViewBag.PhysicalExamination = visit.PhysicalExaminations.ToList();

                ViewBag.VitalSign = visit.VitalSigns.ToList();

                ViewBag.Laboratory = visit.Laboratories.ToList();

                ViewBag.Cardiology = visit.Cardiologys.ToList();

                return View(visit);
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

        public ActionResult Create(int? visitId)
        {
            ViewBag.VisitId = visitId;
            return PartialView("_Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CardiologyId,VisitId,ECG,Echocardiography,Angiography,PCI,EST,SPECTMPI,Diagnosis,Plan,Assistant,Intern,Trainee")] Cardiology cardiology, FormCollection fc)
        {            
            if (ModelState.IsValid)
            {
                cardiology.VisitId = Convert.ToInt32(fc["VisitId"].ToString());
                cardiology.LeadTime = DateTime.Now;
                db.Cardiologys.Add(cardiology);
                await db.SaveChangesAsync();
                return Json(new { success = true });            
            }
            ViewBag.VisitId = fc["VisitId"].ToString();
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
        public async Task<ActionResult> Edit([Bind(Include = "CardiologyId,VisitId,ECG,Echocardiography,Angiography,PCI,EST,SPECTMPI,Diagnosis,Plan,Assistant,Intern,Trainee")] Cardiology cardiology)
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
