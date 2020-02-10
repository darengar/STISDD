using System;
using System.Linq;
using System.Web.Mvc;
using EMR.Models;
using System.Data.Entity;
using Rotativa;
using PagedList;
using EMR.Areas.Clinic.ViewModels;


namespace EMR.Areas.Clinic.Controllers
{
    public class InternalController : Controller
    {
        private Entities db = new Entities();
        private ShafaEntities hisdb = new ShafaEntities();

        public ActionResult Index(int? clinicId, string SearchBy, string Search, int? page)
        {
            ViewBag.ClinicId = clinicId;

            var internalPatientAdm = hisdb.EMRInternalViews;
            if (SearchBy == "Family")
            {
                return View(internalPatientAdm.OrderByDescending(x => x.ID_Code).Where(x => x.Family == Search || Search == null).ToList().ToPagedList(page ?? 1, 20));
            }
            else if (SearchBy == "NationalCode")
            {
                return View(internalPatientAdm.OrderByDescending(x => x.ID_Code).Where(x => x.NationalCode == Search || Search == null).ToList().ToPagedList(page ?? 1, 20));
            }
            else
            {
                return View(internalPatientAdm.OrderByDescending(x => x.ID_Code).ToList().ToPagedList(page ?? 1, 20));
            }                                               
        }

        public ActionResult AjaxPage(int? page)
        {
            var patientlist = hisdb.EMRInternalViews.OrderByDescending(x => x.ID_Code).ToList().ToPagedList(page ?? 1, 20);
            return PartialView("_Internal", patientlist);
        }

        [HttpGet]
        public ActionResult Create(int? patientId, int? clinicId, int? admissionId)
        {
            ViewBag.PatientId = patientId;
            ViewBag.ClinicId = clinicId;
            ViewBag.AdmissionId = admissionId;

            InternalVisit internalVisit = new InternalVisit();

            HistoryDiseas historyDiseas = db.HistoryDiseases.FirstOrDefault(x => x.PatientId == patientId);
            if (historyDiseas != null)
                internalVisit.HistoryDiseasVmp = historyDiseas; // db.HistoryDiseases.Find(historyDiseas.HistoryDiseaseId);

            internalVisit.FollowUpVmp = new FollowUp();
            internalVisit.FollowUpVmp.Date = System.DateTime.Now;
            return View(internalVisit);
        }

        [HttpPost]
        public ActionResult Create([Bind()] InternalVisit internalVisit, FormCollection fCVisit)
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
                        internalVisit.HistoryDiseasVmp.PatientId = Convert.ToInt32(fCVisit["PatientId"]);
                        internalVisit.HistoryDiseasVmp.LeadTime = DateTime.Now;
                        db.HistoryDiseases.Add(internalVisit.HistoryDiseasVmp);
                        db.SaveChanges();
                    }
                    else
                    {
                        historyDiseas = db.HistoryDiseases.Find(historyDiseas.HistoryDiseaseId);
                        historyDiseas.Description = internalVisit.HistoryDiseasVmp.Description;
                        db.Entry(historyDiseas).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    var visit = new Visit();
                    visit.PatientId = Convert.ToInt32(fCVisit["PatientId"]);
                    visit.ClinicId = Convert.ToInt32(fCVisit["ClinicId"]);
                    visit.AdmissionId = Convert.ToInt32(fCVisit["AdmissionId"]);
                    if (Session["DoctorId"] != null)
                        visit.DoctorId = Convert.ToInt32(Session["DoctorId"]);//enter from session
                    if (Session["CollegianId"] != null)
                        visit.CollegianId = Convert.ToInt32(Session["CollegianId"]);//enter from session                               
                    visit.DateOfStay = Convert.ToDateTime(DateTime.Now.ToString());
                    db.Visits.Add(visit);
                    db.SaveChanges();
                    currentVisitId = db.Visits.Max(v => v.VisitId);

                    internalVisit.ChiefComplaintVmp.VisitId = currentVisitId;
                    internalVisit.ChiefComplaintVmp.LeadTime = DateTime.Now;
                    db.ChiefComplaints.Add(internalVisit.ChiefComplaintVmp);

                    internalVisit.PhysicalExaminationVmp.VisitId = currentVisitId;
                    internalVisit.PhysicalExaminationVmp.LeadTime = DateTime.Now;
                    db.PhysicalExaminations.Add(internalVisit.PhysicalExaminationVmp);

                    internalVisit.VitalSignVmp.VisitId = currentVisitId;
                    internalVisit.VitalSignVmp.LeadTime = DateTime.Now;
                    db.VitalSigns.Add(internalVisit.VitalSignVmp);

                    internalVisit.LaboratoryVmp.VisitId = currentVisitId;
                    internalVisit.LaboratoryVmp.LeadTime = DateTime.Now;
                    db.Laboratories.Add(internalVisit.LaboratoryVmp);

                    internalVisit.InternalVmp.VisitId = currentVisitId;
                    internalVisit.InternalVmp.LeadTime = DateTime.Now;
                    db.Internals.Add(internalVisit.InternalVmp);

                    internalVisit.FollowUpVmp.VisitId = currentVisitId;
                    if (fCVisit["FollowUpDate"] == string.Empty)
                        internalVisit.FollowUpVmp.Date = Convert.ToDateTime(fCVisit["DateFllowUp"].ToString());
                    db.FollowUps.Add(internalVisit.FollowUpVmp);

                    db.SaveChanges();

                    return RedirectToAction("VisitReport", new { patientId = fCVisit["PatientId"], clinicId = fCVisit["ClinicId"], admissionId = fCVisit["AdmissionId"], visitId = currentVisitId });
                }
                catch (Exception)
                {
                    Redirect("~/Views/Home/Index.cshtml");
                }
            }
            return View(internalVisit);
        }

        public ActionResult VisitReport(int? patientId, int? clinicId, int? admissionId, int? visitId)
        {
            ViewBag.PatientId = patientId;
            ViewBag.ClinicId = clinicId;
            ViewBag.AdmissionId = admissionId;
            ViewBag.VisitId = visitId;
            //در اینجا کدهای نمایش در صفحه نوشته شود
            ViewBag.ClinicName = "داخلی"; //db.Clinics.Where(c => c.ClinicId == clinicId).FirstOrDefault().Name;
            string patientName = hisdb.AdmIDs.Where(p => p.ID_Code == patientId).FirstOrDefault().Name; // db.Patients.Where(p => p.PatientId == patientId).FirstOrDefault().FullName;
            patientName += " ";
            patientName += patientName = hisdb.AdmIDs.Where(p => p.ID_Code == patientId).FirstOrDefault().Family;
            ViewBag.PatientName = patientName;
            InternalVisit internalVisit = new InternalVisit();
            internalVisit.HistoryDiseasVmp = db.HistoryDiseases.Where(p => p.PatientId == patientId).FirstOrDefault();
            internalVisit.VisitVmp = db.Visits.Where(v => v.VisitId == visitId).FirstOrDefault();
            internalVisit.ChiefComplaintVmp = internalVisit.VisitVmp.ChiefComplaints.FirstOrDefault(x => x.VisitId == visitId);
            internalVisit.PhysicalExaminationVmp = internalVisit.VisitVmp.PhysicalExaminations.FirstOrDefault(x => x.VisitId == visitId);
            internalVisit.VitalSignVmp = internalVisit.VisitVmp.VitalSigns.FirstOrDefault(x => x.VisitId == visitId);
            internalVisit.LaboratoryVmp = internalVisit.VisitVmp.Laboratories.FirstOrDefault(x => x.VisitId == visitId);
            internalVisit.InternalVmp = internalVisit.VisitVmp.Internals.FirstOrDefault(x => x.VisitId == visitId);
            internalVisit.FollowUpVmp = internalVisit.VisitVmp.FollowUps.FirstOrDefault(x => x.VisitId == visitId);
            return View(internalVisit);
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

            InternalVisit internalVisit = new InternalVisit();
            internalVisit.HistoryDiseasVmp = db.HistoryDiseases.FirstOrDefault(x => x.PatientId == patientId);
            internalVisit.VisitVmp = db.Visits.FirstOrDefault(v => v.VisitId == visitId);
            internalVisit.ChiefComplaintVmp = db.ChiefComplaints.FirstOrDefault(x => x.VisitId == visitId);
            internalVisit.PhysicalExaminationVmp = db.PhysicalExaminations.FirstOrDefault(x => x.VisitId == visitId);
            internalVisit.VitalSignVmp = db.VitalSigns.FirstOrDefault(x => x.VisitId == visitId);
            internalVisit.LaboratoryVmp = db.Laboratories.FirstOrDefault(x => x.VisitId == visitId);
            internalVisit.InternalVmp = db.Internals.FirstOrDefault(x => x.VisitId == visitId);
            internalVisit.FollowUpVmp = db.FollowUps.FirstOrDefault(x => x.VisitId == visitId);
            return View(internalVisit);
        }

        [HttpPost]
        public ActionResult Edit([Bind()] InternalVisit internalVisit, FormCollection fCVisit)
        {
            //در اینجا کدهای آپ دیت نوشته شود.
            if (ModelState.IsValid)
            {
                int patientId = Convert.ToInt32(fCVisit["PatientId"]);
                int visitId = Convert.ToInt32(fCVisit["VisitId"]);

                var historyDiseas = db.HistoryDiseases.FirstOrDefault(x => x.PatientId == patientId);
                historyDiseas.Description = internalVisit.HistoryDiseasVmp.Description;
                db.Entry(historyDiseas).State = EntityState.Modified;
                db.SaveChanges();

                var chiefComplaint = db.ChiefComplaints.FirstOrDefault(x => x.VisitId == visitId);
                chiefComplaint.ChestPain = internalVisit.ChiefComplaintVmp.ChestPain;
                chiefComplaint.Dyspnea = internalVisit.ChiefComplaintVmp.Dyspnea;
                chiefComplaint.Syncope = internalVisit.ChiefComplaintVmp.Syncope;
                chiefComplaint.Palpitation = internalVisit.ChiefComplaintVmp.Palpitation;
                chiefComplaint.Edema = internalVisit.ChiefComplaintVmp.Edema;
                chiefComplaint.ETC = internalVisit.ChiefComplaintVmp.ETC;
                db.Entry(chiefComplaint).State = EntityState.Modified;
                db.SaveChanges();

                var physicalExamination = db.PhysicalExaminations.FirstOrDefault(x => x.VisitId == visitId);
                physicalExamination.Heart = internalVisit.PhysicalExaminationVmp.Heart;
                physicalExamination.Lung = internalVisit.PhysicalExaminationVmp.Lung;
                physicalExamination.Extremities = internalVisit.PhysicalExaminationVmp.Extremities;
                physicalExamination.ETC = internalVisit.PhysicalExaminationVmp.ETC;
                db.Entry(physicalExamination).State = EntityState.Modified;
                db.SaveChanges();

                var vitalSign = db.VitalSigns.FirstOrDefault(x => x.VisitId == visitId);
                vitalSign.BP = internalVisit.VitalSignVmp.BP;
                vitalSign.HR = internalVisit.VitalSignVmp.HR;
                vitalSign.RR = internalVisit.VitalSignVmp.RR;
                db.Entry(vitalSign).State = EntityState.Modified;
                db.SaveChanges();

                var laboratory = db.Laboratories.FirstOrDefault(x => x.VisitId == visitId);
                laboratory.BUN = internalVisit.LaboratoryVmp.BUN;
                laboratory.Cr = internalVisit.LaboratoryVmp.Cr;
                laboratory.Na = internalVisit.LaboratoryVmp.Na;
                laboratory.K = internalVisit.LaboratoryVmp.K;
                laboratory.Ca = internalVisit.LaboratoryVmp.Ca;
                laboratory.Mg = internalVisit.LaboratoryVmp.Mg;
                laboratory.FBS = internalVisit.LaboratoryVmp.FBS;
                laboratory.Hb1AC = internalVisit.LaboratoryVmp.Hb1AC;
                laboratory.Chol = internalVisit.LaboratoryVmp.Chol;
                laboratory.LDL = internalVisit.LaboratoryVmp.LDL;
                laboratory.HDL = internalVisit.LaboratoryVmp.HDL;
                laboratory.TG = internalVisit.LaboratoryVmp.TG;
                laboratory.VitOHD3 = internalVisit.LaboratoryVmp.VitOHD3;
                laboratory.CTnI = internalVisit.LaboratoryVmp.CTnI;
                laboratory.CKMB = internalVisit.LaboratoryVmp.CKMB;
                laboratory.AST = internalVisit.LaboratoryVmp.AST;
                laboratory.ALT = internalVisit.LaboratoryVmp.ALT;
                laboratory.ALP = internalVisit.LaboratoryVmp.ALP;
                laboratory.BiL = internalVisit.LaboratoryVmp.BiL;
                laboratory.PSA = internalVisit.LaboratoryVmp.PSA;
                laboratory.TSH = internalVisit.LaboratoryVmp.TSH;
                laboratory.T3 = internalVisit.LaboratoryVmp.T3;
                laboratory.T4 = internalVisit.LaboratoryVmp.T4;
                laboratory.T3RU = internalVisit.LaboratoryVmp.T3RU;
                db.Entry(laboratory).State = EntityState.Modified;
                db.SaveChanges();

                var internalEnt = db.Internals.FirstOrDefault(x => x.VisitId == visitId);
                internalEnt.Description = internalVisit.InternalVmp.Description;
                db.Entry(internalEnt).State = EntityState.Modified;
                db.SaveChanges();

                var followUp = db.FollowUps.FirstOrDefault(x => x.VisitId == visitId);
                followUp.Description = internalVisit.FollowUpVmp.Description;
                followUp.Date = internalVisit.FollowUpVmp.Date;
                db.Entry(followUp).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("VisitReport", new { visitId = fCVisit["VisitId"], patientId = fCVisit["PatientId"], clinicId = fCVisit["ClinicId"] });
            }
            return View(internalVisit);
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