using System;
using System.Linq;
using System.Web.Mvc;
using EMR.Models;
using System.Data.Entity;
using Rotativa;
using PagedList;
using EMR.Areas.OutPatient.ViewModels;

namespace EMR.Areas.OutPatient.Controllers
{
    public class LungController : Controller
    {
        private Entities db = new Entities();
        private ShafaEntities hisdb = new ShafaEntities();

        public ActionResult Index(int? clinicId, string SearchBy, string Search, int? page)
        {
            ViewBag.ClinicId = clinicId;

            var LungPatientAdm = hisdb.EMRLungViews;
            if (SearchBy == "Family")
            {
                return View(LungPatientAdm.OrderByDescending(x => x.ID_Code).Where(x => x.Family == Search || Search == null).ToList().ToPagedList(page ?? 1, 20));
            }
            else if (SearchBy == "NationalCode")
            {
                return View(LungPatientAdm.OrderByDescending(x => x.ID_Code).Where(x => x.NationalCode == Search || Search == null).ToList().ToPagedList(page ?? 1, 20));
            }
            else
            {
                return View(LungPatientAdm.OrderByDescending(x => x.ID_Code).ToList().ToPagedList(page ?? 1, 20));
            }                                               
        }

        public ActionResult AjaxPage(int? page)
        {
            var patientlist = hisdb.EMRLungViews.OrderByDescending(x => x.ID_Code).ToList().ToPagedList(page ?? 1, 20);
            return PartialView("_Lung", patientlist);
        }

        [HttpGet]
        public ActionResult Create(int? patientId, int? clinicId, int? admissionId)
        {
            ViewBag.PatientId = patientId;
            ViewBag.ClinicId = clinicId;
            ViewBag.AdmissionId = admissionId;

            LungVisit lungVisit = new LungVisit();

            HistoryDiseas historyDiseas = db.HistoryDiseases.FirstOrDefault(x => x.PatientId == patientId);
            if (historyDiseas != null)
                lungVisit.HistoryDiseasVmp = historyDiseas; // db.HistoryDiseases.Find(historyDiseas.HistoryDiseaseId);

            lungVisit.FollowUpVmp = new FollowUp();
            lungVisit.FollowUpVmp.Date = System.DateTime.Now;
            return View(lungVisit);
        }

        [HttpPost]
        public ActionResult Create([Bind()] LungVisit lungVisit, FormCollection fCVisit)
        {
            int currentVisitId = 0;
            if (ModelState.IsValid)
            {
                try
                {
                    int patientId = Convert.ToInt32(fCVisit["PatientId"]);
                    var historyDiseas = db.HistoryDiseases.FirstOrDefault(x => x.PatientId == patientId);
                    if (historyDiseas == null)
                    {
                        lungVisit.HistoryDiseasVmp.PatientId = Convert.ToInt32(fCVisit["PatientId"]);
                        lungVisit.HistoryDiseasVmp.LeadTime = DateTime.Now;
                        db.HistoryDiseases.Add(lungVisit.HistoryDiseasVmp);
                        db.SaveChanges();
                    }
                    else
                    {
                        historyDiseas = db.HistoryDiseases.Find(historyDiseas.HistoryDiseaseId);
                        historyDiseas.Description = lungVisit.HistoryDiseasVmp.Description;
                        db.Entry(historyDiseas).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    var visit = new Visit();
                    visit.PatientId = Convert.ToInt32(fCVisit["PatientId"]);
                    visit.ClinicId = Convert.ToInt32(fCVisit["ClinicId"]);
                    visit.KindOfTherapyId = 1;//outpatient
                    visit.AdmissionId = Convert.ToInt32(fCVisit["AdmissionId"]);
                    if (Session["DoctorId"] != null)
                        visit.DoctorId = Convert.ToInt32(Session["DoctorId"]);//enter from session
                    if (Session["CollegianId"] != null)
                        visit.CollegianId = Convert.ToInt32(Session["CollegianId"]);//enter from session                               
                    visit.DateOfStay = Convert.ToDateTime(DateTime.Now.ToString());
                    db.Visits.Add(visit);
                    db.SaveChanges();
                    currentVisitId = db.Visits.Max(v => v.VisitId);

                    lungVisit.ChiefComplaintVmp.VisitId = currentVisitId;
                    lungVisit.ChiefComplaintVmp.LeadTime = DateTime.Now;
                    db.ChiefComplaints.Add(lungVisit.ChiefComplaintVmp);

                    lungVisit.PhysicalExaminationVmp.VisitId = currentVisitId;
                    lungVisit.PhysicalExaminationVmp.LeadTime = DateTime.Now;
                    db.PhysicalExaminations.Add(lungVisit.PhysicalExaminationVmp);

                    lungVisit.VitalSignVmp.VisitId = currentVisitId;
                    lungVisit.VitalSignVmp.LeadTime = DateTime.Now;
                    db.VitalSigns.Add(lungVisit.VitalSignVmp);

                    lungVisit.LaboratoryVmp.VisitId = currentVisitId;
                    lungVisit.LaboratoryVmp.LeadTime = DateTime.Now;
                    db.Laboratories.Add(lungVisit.LaboratoryVmp);

                    lungVisit.LungVmp.VisitId = currentVisitId;
                    lungVisit.LungVmp.LeadTime = DateTime.Now;
                    db.Lungs.Add(lungVisit.LungVmp);

                    lungVisit.FollowUpVmp.VisitId = currentVisitId;
                    if (fCVisit["FollowUpDate"] == string.Empty)
                        lungVisit.FollowUpVmp.Date = Convert.ToDateTime(fCVisit["DateFllowUp"].ToString());
                    db.FollowUps.Add(lungVisit.FollowUpVmp);

                    db.SaveChanges();

                    return RedirectToAction("VisitReport", new { patientId = fCVisit["PatientId"], clinicId = fCVisit["ClinicId"], admissionId = fCVisit["AdmissionId"], visitId = currentVisitId });
                }
                catch (Exception)
                {
                    Redirect("~/Views/Home/Index.cshtml");
                }
            }
            return View(lungVisit);
        }

        public ActionResult VisitReport(int? patientId, int? clinicId, int? admissionId, int? visitId)
        {
            ViewBag.PatientId = patientId;
            ViewBag.ClinicId = clinicId;
            ViewBag.AdmissionId = admissionId;
            ViewBag.VisitId = visitId;
            //در اینجا کدهای نمایش در صفحه نوشته شود
            ViewBag.ClinicName = "ریه"; //db.Clinics.Where(c => c.ClinicId == clinicId).FirstOrDefault().Name;
            string patientName = hisdb.AdmIDs.Where(p => p.ID_Code == patientId).FirstOrDefault().Name; // db.Patients.Where(p => p.PatientId == patientId).FirstOrDefault().FullName;
            patientName += " ";
            patientName += patientName = hisdb.AdmIDs.Where(p => p.ID_Code == patientId).FirstOrDefault().Family;
            ViewBag.PatientName = patientName;
            LungVisit lungVisit = new LungVisit();
            lungVisit.HistoryDiseasVmp = db.HistoryDiseases.Where(p => p.PatientId == patientId).FirstOrDefault();
            lungVisit.VisitVmp = db.Visits.Where(v => v.VisitId == visitId).FirstOrDefault();
            lungVisit.ChiefComplaintVmp = lungVisit.VisitVmp.ChiefComplaints.FirstOrDefault(x => x.VisitId == visitId);
            lungVisit.PhysicalExaminationVmp = lungVisit.VisitVmp.PhysicalExaminations.FirstOrDefault(x => x.VisitId == visitId);
            lungVisit.VitalSignVmp = lungVisit.VisitVmp.VitalSigns.FirstOrDefault(x => x.VisitId == visitId);
            lungVisit.LaboratoryVmp = lungVisit.VisitVmp.Laboratories.FirstOrDefault(x => x.VisitId == visitId);
            lungVisit.LungVmp = lungVisit.VisitVmp.Lungs.FirstOrDefault(x => x.VisitId == visitId);
            lungVisit.FollowUpVmp = lungVisit.VisitVmp.FollowUps.FirstOrDefault(x => x.VisitId == visitId);
            return View(lungVisit);
        }

        [HttpGet]
        public ActionResult PastVisit(int? patientId, int? clinicId, int? admissionId, int? visitId)
        {
            ViewBag.PatientId = patientId;
            string patientName = hisdb.AdmIDs.Where(p => p.ID_Code == patientId).FirstOrDefault().Name; // db.Patients.Where(p => p.PatientId == patientId).FirstOrDefault().FullName;
            patientName += " ";
            patientName += patientName = hisdb.AdmIDs.Where(p => p.ID_Code == patientId).FirstOrDefault().Family;
            ViewBag.PatientName = patientName;
            ViewBag.ClinicId = clinicId;
            ViewBag.AdmissionId = admissionId;
            ViewBag.VisitId = visitId;
            var patientClinicVisit = db.Visits.Where(x => (x.PatientId == patientId && x.ClinicId == clinicId));
            if (Session["DoctorId"] != null)
            {
                int iDoctorId = Convert.ToInt32(Session["DoctorId"].ToString());
                patientClinicVisit = patientClinicVisit.Where(x => x.DoctorId == iDoctorId);
            }
            if (Session["CollegianId"] != null)
            {
                int iCollegianId = Convert.ToInt32(Session["CollegianId"].ToString());
                patientClinicVisit = patientClinicVisit.Where(x => x.CollegianId == iCollegianId);
            }
            return View(patientClinicVisit.ToList());
        }

        public ActionResult Edit(int? patientId, int? clinicId, int? admissionId, int? visitId)
        {
            ViewBag.PatientId = patientId;
            ViewBag.ClinicId = clinicId;
            ViewBag.AdmissionId = admissionId;
            ViewBag.VisitId = visitId;

            LungVisit lungVisit = new LungVisit();
            lungVisit.HistoryDiseasVmp = db.HistoryDiseases.FirstOrDefault(x => x.PatientId == patientId);
            lungVisit.VisitVmp = db.Visits.FirstOrDefault(v => v.VisitId == visitId);
            lungVisit.ChiefComplaintVmp = db.ChiefComplaints.FirstOrDefault(x => x.VisitId == visitId);
            lungVisit.PhysicalExaminationVmp = db.PhysicalExaminations.FirstOrDefault(x => x.VisitId == visitId);
            lungVisit.VitalSignVmp = db.VitalSigns.FirstOrDefault(x => x.VisitId == visitId);
            lungVisit.LaboratoryVmp = db.Laboratories.FirstOrDefault(x => x.VisitId == visitId);
            lungVisit.LungVmp = db.Lungs.FirstOrDefault(x => x.VisitId == visitId);
            lungVisit.FollowUpVmp = db.FollowUps.FirstOrDefault(x => x.VisitId == visitId);
            return View(lungVisit);
        }

        [HttpPost]
        public ActionResult Edit([Bind()] LungVisit lungVisit, FormCollection fCVisit)
        {
            //در اینجا کدهای آپ دیت نوشته شود.
            if (ModelState.IsValid)
            {
                int patientId = Convert.ToInt32(fCVisit["PatientId"]);
                int visitId = Convert.ToInt32(fCVisit["VisitId"]);

                var historyDiseas = db.HistoryDiseases.FirstOrDefault(x => x.PatientId == patientId);
                historyDiseas.Description = lungVisit.HistoryDiseasVmp.Description;
                db.Entry(historyDiseas).State = EntityState.Modified;
                db.SaveChanges();

                var chiefComplaint = db.ChiefComplaints.FirstOrDefault(x => x.VisitId == visitId);
                chiefComplaint.ChestPain = lungVisit.ChiefComplaintVmp.ChestPain;
                chiefComplaint.Dyspnea = lungVisit.ChiefComplaintVmp.Dyspnea;
                chiefComplaint.Syncope = lungVisit.ChiefComplaintVmp.Syncope;
                chiefComplaint.Palpitation = lungVisit.ChiefComplaintVmp.Palpitation;
                chiefComplaint.Edema = lungVisit.ChiefComplaintVmp.Edema;
                chiefComplaint.ETC = lungVisit.ChiefComplaintVmp.ETC;
                db.Entry(chiefComplaint).State = EntityState.Modified;
                db.SaveChanges();

                var physicalExamination = db.PhysicalExaminations.FirstOrDefault(x => x.VisitId == visitId);
                physicalExamination.Heart = lungVisit.PhysicalExaminationVmp.Heart;
                physicalExamination.Lung = lungVisit.PhysicalExaminationVmp.Lung;
                physicalExamination.Extremities = lungVisit.PhysicalExaminationVmp.Extremities;
                physicalExamination.ETC = lungVisit.PhysicalExaminationVmp.ETC;
                db.Entry(physicalExamination).State = EntityState.Modified;
                db.SaveChanges();

                var vitalSign = db.VitalSigns.FirstOrDefault(x => x.VisitId == visitId);
                vitalSign.BP = lungVisit.VitalSignVmp.BP;
                vitalSign.HR = lungVisit.VitalSignVmp.HR;
                vitalSign.RR = lungVisit.VitalSignVmp.RR;
                db.Entry(vitalSign).State = EntityState.Modified;
                db.SaveChanges();

                var laboratory = db.Laboratories.FirstOrDefault(x => x.VisitId == visitId);
                laboratory.BUN = lungVisit.LaboratoryVmp.BUN;
                laboratory.Cr = lungVisit.LaboratoryVmp.Cr;
                laboratory.Na = lungVisit.LaboratoryVmp.Na;
                laboratory.K = lungVisit.LaboratoryVmp.K;
                laboratory.Ca = lungVisit.LaboratoryVmp.Ca;
                laboratory.Mg = lungVisit.LaboratoryVmp.Mg;
                laboratory.FBS = lungVisit.LaboratoryVmp.FBS;
                laboratory.Hb1AC = lungVisit.LaboratoryVmp.Hb1AC;
                laboratory.Chol = lungVisit.LaboratoryVmp.Chol;
                laboratory.LDL = lungVisit.LaboratoryVmp.LDL;
                laboratory.HDL = lungVisit.LaboratoryVmp.HDL;
                laboratory.TG = lungVisit.LaboratoryVmp.TG;
                laboratory.VitOHD3 = lungVisit.LaboratoryVmp.VitOHD3;
                laboratory.CTnI = lungVisit.LaboratoryVmp.CTnI;
                laboratory.CKMB = lungVisit.LaboratoryVmp.CKMB;
                laboratory.AST = lungVisit.LaboratoryVmp.AST;
                laboratory.ALT = lungVisit.LaboratoryVmp.ALT;
                laboratory.ALP = lungVisit.LaboratoryVmp.ALP;
                laboratory.BiL = lungVisit.LaboratoryVmp.BiL;
                laboratory.PSA = lungVisit.LaboratoryVmp.PSA;
                laboratory.TSH = lungVisit.LaboratoryVmp.TSH;
                laboratory.T3 = lungVisit.LaboratoryVmp.T3;
                laboratory.T4 = lungVisit.LaboratoryVmp.T4;
                laboratory.T3RU = lungVisit.LaboratoryVmp.T3RU;
                db.Entry(laboratory).State = EntityState.Modified;
                db.SaveChanges();

                var lung = db.Lungs.FirstOrDefault(x => x.VisitId == visitId);                
                lung.Description = lungVisit.LungVmp.Description;
                db.Entry(lung).State = EntityState.Modified;
                db.SaveChanges();

                var followUp = db.FollowUps.FirstOrDefault(x => x.VisitId == visitId);
                followUp.Description = lungVisit.FollowUpVmp.Description;
                followUp.Date = lungVisit.FollowUpVmp.Date;
                db.Entry(followUp).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("VisitReport", new { visitId = fCVisit["VisitId"], patientId = fCVisit["PatientId"], clinicId = fCVisit["ClinicId"] });
            }
            return View(lungVisit);
        }

        public ActionResult ReportPDF(string patientName, int? patientId, int? clinicId, int? admissionId, int? visitId)
        {
            ViewBag.PatientId = patientId;
            ViewBag.ClinicId = clinicId;
            ViewBag.AdmissionId = admissionId;
            ViewBag.VisitId = visitId;

            return new ActionAsPdf("VisitReport", new { patientId = ViewBag.PatientId, clinicId = ViewBag.ClinicId, admissionId = ViewBag.AdmissionId, visitId = ViewBag.VisitId })
            {
                FileName = string.Format("{0}.pdf", patientName)                
            };
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