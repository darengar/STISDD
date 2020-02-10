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
    public class DigestionController : Controller
    {
        private Entities db = new Entities();
        private ShafaEntities hisdb = new ShafaEntities();

        public ActionResult Index(int? clinicId, string SearchBy, string Search, int? page)
        {
            ViewBag.ClinicId = clinicId;

            var digestionPatientAdm = hisdb.EMRDigestionViews;
            if (SearchBy == "Family")
            {
                return View(digestionPatientAdm.OrderByDescending(x => x.ID_Code).Where(x => x.Family == Search || Search == null).ToList().ToPagedList(page ?? 1, 20));
            }
            else if (SearchBy == "NationalCode")
            {
                return View(digestionPatientAdm.OrderByDescending(x => x.ID_Code).Where(x => x.NationalCode == Search || Search == null).ToList().ToPagedList(page ?? 1, 20));
            }
            else
            {
                return View(digestionPatientAdm.OrderByDescending(x => x.ID_Code).ToList().ToPagedList(page ?? 1, 20));
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

            DigestionVisit digestionVisit = new DigestionVisit();

            HistoryDiseas historyDiseas = db.HistoryDiseases.FirstOrDefault(x => x.PatientId == patientId);
            if (historyDiseas != null)
                digestionVisit.HistoryDiseasVmp = historyDiseas; // db.HistoryDiseases.Find(historyDiseas.HistoryDiseaseId);

            digestionVisit.FollowUpVmp = new FollowUp();
            digestionVisit.FollowUpVmp.Date = System.DateTime.Now;
            return View(digestionVisit);
        }

        [HttpPost]
        public ActionResult Create([Bind()] DigestionVisit digestionVisit, FormCollection fCVisit)
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
                        digestionVisit.HistoryDiseasVmp.PatientId = Convert.ToInt32(fCVisit["PatientId"]);
                        digestionVisit.HistoryDiseasVmp.LeadTime = DateTime.Now;
                        db.HistoryDiseases.Add(digestionVisit.HistoryDiseasVmp);
                        db.SaveChanges();
                    }
                    else
                    {
                        historyDiseas = db.HistoryDiseases.Find(historyDiseas.HistoryDiseaseId);
                        historyDiseas.Description = digestionVisit.HistoryDiseasVmp.Description;
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

                    digestionVisit.ChiefComplaintVmp.VisitId = currentVisitId;
                    digestionVisit.ChiefComplaintVmp.LeadTime = DateTime.Now;
                    db.ChiefComplaints.Add(digestionVisit.ChiefComplaintVmp);

                    digestionVisit.PhysicalExaminationVmp.VisitId = currentVisitId;
                    digestionVisit.PhysicalExaminationVmp.LeadTime = DateTime.Now;
                    db.PhysicalExaminations.Add(digestionVisit.PhysicalExaminationVmp);

                    digestionVisit.VitalSignVmp.VisitId = currentVisitId;
                    digestionVisit.VitalSignVmp.LeadTime = DateTime.Now;
                    db.VitalSigns.Add(digestionVisit.VitalSignVmp);

                    digestionVisit.LaboratoryVmp.VisitId = currentVisitId;
                    digestionVisit.LaboratoryVmp.LeadTime = DateTime.Now;
                    db.Laboratories.Add(digestionVisit.LaboratoryVmp);

                    digestionVisit.DigestionVmp.VisitId = currentVisitId;
                    digestionVisit.DigestionVmp.LeadTime = DateTime.Now;
                    db.Digestions.Add(digestionVisit.DigestionVmp);

                    digestionVisit.FollowUpVmp.VisitId = currentVisitId;
                    if (fCVisit["FollowUpDate"] == string.Empty)
                        digestionVisit.FollowUpVmp.Date = Convert.ToDateTime(fCVisit["DateFllowUp"].ToString());
                    db.FollowUps.Add(digestionVisit.FollowUpVmp);

                    db.SaveChanges();

                    return RedirectToAction("VisitReport", new { patientId = fCVisit["PatientId"], clinicId = fCVisit["ClinicId"], admissionId = fCVisit["AdmissionId"], visitId = currentVisitId });
                }
                catch (Exception)
                {
                    Redirect("~/Views/Home/Index.cshtml");
                }
            }
            return View(digestionVisit);
        }

        public ActionResult VisitReport(int? patientId, int? clinicId, int? admissionId, int? visitId)
        {
            ViewBag.PatientId = patientId;
            ViewBag.ClinicId = clinicId;
            ViewBag.AdmissionId = admissionId;
            ViewBag.VisitId = visitId;
            //در اینجا کدهای نمایش در صفحه نوشته شود
            ViewBag.ClinicName = "گوارش"; //db.Clinics.Where(c => c.ClinicId == clinicId).FirstOrDefault().Name;
            string patientName = hisdb.AdmIDs.Where(p => p.ID_Code == patientId).FirstOrDefault().Name; // db.Patients.Where(p => p.PatientId == patientId).FirstOrDefault().FullName;
            patientName += " ";
            patientName += patientName = hisdb.AdmIDs.Where(p => p.ID_Code == patientId).FirstOrDefault().Family;
            ViewBag.PatientName = patientName;
            DigestionVisit digestionVisit = new DigestionVisit();
            digestionVisit.HistoryDiseasVmp = db.HistoryDiseases.Where(p => p.PatientId == patientId).FirstOrDefault();
            digestionVisit.VisitVmp = db.Visits.Where(v => v.VisitId == visitId).FirstOrDefault();
            digestionVisit.ChiefComplaintVmp = digestionVisit.VisitVmp.ChiefComplaints.FirstOrDefault(x => x.VisitId == visitId);
            digestionVisit.PhysicalExaminationVmp = digestionVisit.VisitVmp.PhysicalExaminations.FirstOrDefault(x => x.VisitId == visitId);
            digestionVisit.VitalSignVmp = digestionVisit.VisitVmp.VitalSigns.FirstOrDefault(x => x.VisitId == visitId);
            digestionVisit.LaboratoryVmp = digestionVisit.VisitVmp.Laboratories.FirstOrDefault(x => x.VisitId == visitId);
            digestionVisit.DigestionVmp = digestionVisit.VisitVmp.Digestions.FirstOrDefault(x => x.VisitId == visitId);
            digestionVisit.FollowUpVmp = digestionVisit.VisitVmp.FollowUps.FirstOrDefault(x => x.VisitId == visitId);
            return View(digestionVisit);
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

            DigestionVisit digestionVisit = new DigestionVisit();
            digestionVisit.HistoryDiseasVmp = db.HistoryDiseases.FirstOrDefault(x => x.PatientId == patientId);
            digestionVisit.VisitVmp = db.Visits.FirstOrDefault(v => v.VisitId == visitId);
            digestionVisit.ChiefComplaintVmp = db.ChiefComplaints.FirstOrDefault(x => x.VisitId == visitId);
            digestionVisit.PhysicalExaminationVmp = db.PhysicalExaminations.FirstOrDefault(x => x.VisitId == visitId);
            digestionVisit.VitalSignVmp = db.VitalSigns.FirstOrDefault(x => x.VisitId == visitId);
            digestionVisit.LaboratoryVmp = db.Laboratories.FirstOrDefault(x => x.VisitId == visitId);
            digestionVisit.DigestionVmp = db.Digestions.FirstOrDefault(x => x.VisitId == visitId);
            digestionVisit.FollowUpVmp = db.FollowUps.FirstOrDefault(x => x.VisitId == visitId);
            return View(digestionVisit);
        }

        [HttpPost]
        public ActionResult Edit([Bind()] DigestionVisit digestionVisit, FormCollection fCVisit)
        {
            //در اینجا کدهای آپ دیت نوشته شود.
            if (ModelState.IsValid)
            {
                int patientId = Convert.ToInt32(fCVisit["PatientId"]);
                int visitId = Convert.ToInt32(fCVisit["VisitId"]);

                var historyDiseas = db.HistoryDiseases.FirstOrDefault(x => x.PatientId == patientId);
                historyDiseas.Description = digestionVisit.HistoryDiseasVmp.Description;
                db.Entry(historyDiseas).State = EntityState.Modified;
                db.SaveChanges();

                var chiefComplaint = db.ChiefComplaints.FirstOrDefault(x => x.VisitId == visitId);
                chiefComplaint.ChestPain = digestionVisit.ChiefComplaintVmp.ChestPain;
                chiefComplaint.Dyspnea = digestionVisit.ChiefComplaintVmp.Dyspnea;
                chiefComplaint.Syncope = digestionVisit.ChiefComplaintVmp.Syncope;
                chiefComplaint.Palpitation = digestionVisit.ChiefComplaintVmp.Palpitation;
                chiefComplaint.Edema = digestionVisit.ChiefComplaintVmp.Edema;
                chiefComplaint.ETC = digestionVisit.ChiefComplaintVmp.ETC;
                db.Entry(chiefComplaint).State = EntityState.Modified;
                db.SaveChanges();

                var physicalExamination = db.PhysicalExaminations.FirstOrDefault(x => x.VisitId == visitId);
                physicalExamination.Heart = digestionVisit.PhysicalExaminationVmp.Heart;
                physicalExamination.Lung = digestionVisit.PhysicalExaminationVmp.Lung;
                physicalExamination.Extremities = digestionVisit.PhysicalExaminationVmp.Extremities;
                physicalExamination.ETC = digestionVisit.PhysicalExaminationVmp.ETC;
                db.Entry(physicalExamination).State = EntityState.Modified;
                db.SaveChanges();

                var vitalSign = db.VitalSigns.FirstOrDefault(x => x.VisitId == visitId);
                vitalSign.BP = digestionVisit.VitalSignVmp.BP;
                vitalSign.HR = digestionVisit.VitalSignVmp.HR;
                vitalSign.RR = digestionVisit.VitalSignVmp.RR;
                db.Entry(vitalSign).State = EntityState.Modified;
                db.SaveChanges();

                var laboratory = db.Laboratories.FirstOrDefault(x => x.VisitId == visitId);
                laboratory.BUN = digestionVisit.LaboratoryVmp.BUN;
                laboratory.Cr = digestionVisit.LaboratoryVmp.Cr;
                laboratory.Na = digestionVisit.LaboratoryVmp.Na;
                laboratory.K = digestionVisit.LaboratoryVmp.K;
                laboratory.Ca = digestionVisit.LaboratoryVmp.Ca;
                laboratory.Mg = digestionVisit.LaboratoryVmp.Mg;
                laboratory.FBS = digestionVisit.LaboratoryVmp.FBS;
                laboratory.Hb1AC = digestionVisit.LaboratoryVmp.Hb1AC;
                laboratory.Chol = digestionVisit.LaboratoryVmp.Chol;
                laboratory.LDL = digestionVisit.LaboratoryVmp.LDL;
                laboratory.HDL = digestionVisit.LaboratoryVmp.HDL;
                laboratory.TG = digestionVisit.LaboratoryVmp.TG;
                laboratory.VitOHD3 = digestionVisit.LaboratoryVmp.VitOHD3;
                laboratory.CTnI = digestionVisit.LaboratoryVmp.CTnI;
                laboratory.CKMB = digestionVisit.LaboratoryVmp.CKMB;
                laboratory.AST = digestionVisit.LaboratoryVmp.AST;
                laboratory.ALT = digestionVisit.LaboratoryVmp.ALT;
                laboratory.ALP = digestionVisit.LaboratoryVmp.ALP;
                laboratory.BiL = digestionVisit.LaboratoryVmp.BiL;
                laboratory.PSA = digestionVisit.LaboratoryVmp.PSA;
                laboratory.TSH = digestionVisit.LaboratoryVmp.TSH;
                laboratory.T3 = digestionVisit.LaboratoryVmp.T3;
                laboratory.T4 = digestionVisit.LaboratoryVmp.T4;
                laboratory.T3RU = digestionVisit.LaboratoryVmp.T3RU;
                db.Entry(laboratory).State = EntityState.Modified;
                db.SaveChanges();

                var digestion = db.Digestions.FirstOrDefault(x => x.VisitId == visitId);                
                digestion.Description = digestionVisit.DigestionVmp.Description;
                db.Entry(digestion).State = EntityState.Modified;
                db.SaveChanges();

                var followUp = db.FollowUps.FirstOrDefault(x => x.VisitId == visitId);
                followUp.Description = digestionVisit.FollowUpVmp.Description;
                followUp.Date = digestionVisit.FollowUpVmp.Date;
                db.Entry(followUp).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("VisitReport", new { visitId = fCVisit["VisitId"], patientId = fCVisit["PatientId"], clinicId = fCVisit["ClinicId"] });
            }
            return View(digestionVisit);
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