using System;
using System.Linq;
using System.Web.Mvc;
using EMR.Models;
using EMR.Areas.Clinic.ViewModels;
using System.Data.Entity;
using Rotativa;
using PagedList;


namespace EMR.Areas.Clinic.Controllers
{
    public class WomanController : Controller
    {
        private Entities db = new Entities();
        private ShafaEntities hisdb = new ShafaEntities();

        public ActionResult Index(int? clinicId, string SearchBy, string Search, int? page)
        {
            ViewBag.ClinicId = clinicId;

            var womanPatientAdm = hisdb.EMRWomanViews;
            if (SearchBy == "Family")
            {
                return View(womanPatientAdm.OrderByDescending(x => x.ID_Code).Where(x => x.Family == Search || Search == null).ToList().ToPagedList(page ?? 1, 20));
            }
            else if (SearchBy == "NationalCode")
            {
                return View(womanPatientAdm.OrderByDescending(x => x.ID_Code).Where(x => x.NationalCode == Search || Search == null).ToList().ToPagedList(page ?? 1, 20));
            }
            else
            {
                return View(womanPatientAdm.OrderByDescending(x => x.ID_Code).ToList().ToPagedList(page ?? 1, 20));
            }                                               
        }

        public ActionResult AjaxPage(int? page)
        {
            var patientlist = hisdb.EMRWomanViews.OrderByDescending(x => x.ID_Code).ToList().ToPagedList(page ?? 1, 20);
            return PartialView("_Woman", patientlist);
        }

        [HttpGet]
        public ActionResult Create(int? patientId, int? clinicId, int? admissionId)
        {
            ViewBag.PatientId = patientId;
            ViewBag.ClinicId = clinicId;
            ViewBag.AdmissionId = admissionId;

            WomanVisit womanVisit = new WomanVisit();

            HistoryDiseas historyDiseas = db.HistoryDiseases.FirstOrDefault(x => x.PatientId == patientId);
            if (historyDiseas != null)
                womanVisit.HistoryDiseasVmp = historyDiseas; // db.HistoryDiseases.Find(historyDiseas.HistoryDiseaseId);

            womanVisit.FollowUpVmp = new FollowUp();
            womanVisit.FollowUpVmp.Date = System.DateTime.Now;
            return View(womanVisit);
        }

        [HttpPost]
        public ActionResult Create([Bind()] WomanVisit womanVisit, FormCollection fCVisit)
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
                        womanVisit.HistoryDiseasVmp.PatientId = Convert.ToInt32(fCVisit["PatientId"]);
                        womanVisit.HistoryDiseasVmp.LeadTime = DateTime.Now;
                        db.HistoryDiseases.Add(womanVisit.HistoryDiseasVmp);
                        db.SaveChanges();
                    }
                    else
                    {
                        historyDiseas = db.HistoryDiseases.Find(historyDiseas.HistoryDiseaseId);
                        historyDiseas.Description = womanVisit.HistoryDiseasVmp.Description;
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

                    womanVisit.ChiefComplaintVmp.VisitId = currentVisitId;
                    womanVisit.ChiefComplaintVmp.LeadTime = DateTime.Now;
                    db.ChiefComplaints.Add(womanVisit.ChiefComplaintVmp);

                    womanVisit.PhysicalExaminationVmp.VisitId = currentVisitId;
                    womanVisit.PhysicalExaminationVmp.LeadTime = DateTime.Now;
                    db.PhysicalExaminations.Add(womanVisit.PhysicalExaminationVmp);

                    womanVisit.VitalSignVmp.VisitId = currentVisitId;
                    womanVisit.VitalSignVmp.LeadTime = DateTime.Now;
                    db.VitalSigns.Add(womanVisit.VitalSignVmp);

                    womanVisit.LaboratoryVmp.VisitId = currentVisitId;
                    womanVisit.LaboratoryVmp.LeadTime = DateTime.Now;
                    db.Laboratories.Add(womanVisit.LaboratoryVmp);

                    womanVisit.WomanVmp.VisitId = currentVisitId;
                    womanVisit.WomanVmp.LeadTime = DateTime.Now;
                    db.Womans.Add(womanVisit.WomanVmp);

                    womanVisit.FollowUpVmp.VisitId = currentVisitId;
                    if (fCVisit["FollowUpDate"] == string.Empty)
                        womanVisit.FollowUpVmp.Date = Convert.ToDateTime(fCVisit["DateFllowUp"].ToString());
                    db.FollowUps.Add(womanVisit.FollowUpVmp);

                    db.SaveChanges();

                    return RedirectToAction("VisitReport", new { patientId = fCVisit["PatientId"], clinicId = fCVisit["ClinicId"], admissionId = fCVisit["AdmissionId"], visitId = currentVisitId });
                }
                catch (Exception)
                {
                    Redirect("~/Views/Home/Index.cshtml");
                }
            }
            return View(womanVisit);
        }

        public ActionResult VisitReport(int? patientId, int? clinicId, int? admissionId, int? visitId)
        {
            ViewBag.PatientId = patientId;
            ViewBag.ClinicId = clinicId;
            ViewBag.AdmissionId = admissionId;
            ViewBag.VisitId = visitId;
            //در اینجا کدهای نمایش در صفحه نوشته شود
            ViewBag.ClinicName = "زنان"; //db.Clinics.Where(c => c.ClinicId == clinicId).FirstOrDefault().Name;
            string patientName = hisdb.AdmIDs.Where(p => p.ID_Code == patientId).FirstOrDefault().Name; // db.Patients.Where(p => p.PatientId == patientId).FirstOrDefault().FullName;
            patientName += " ";
            patientName += patientName = hisdb.AdmIDs.Where(p => p.ID_Code == patientId).FirstOrDefault().Family;
            ViewBag.PatientName = patientName;
            WomanVisit womanVisit = new WomanVisit();
            womanVisit.HistoryDiseasVmp = db.HistoryDiseases.Where(p => p.PatientId == patientId).FirstOrDefault();
            womanVisit.VisitVmp = db.Visits.Where(v => v.VisitId == visitId).FirstOrDefault();
            womanVisit.ChiefComplaintVmp = womanVisit.VisitVmp.ChiefComplaints.FirstOrDefault(x => x.VisitId == visitId);
            womanVisit.PhysicalExaminationVmp = womanVisit.VisitVmp.PhysicalExaminations.FirstOrDefault(x => x.VisitId == visitId);
            womanVisit.VitalSignVmp = womanVisit.VisitVmp.VitalSigns.FirstOrDefault(x => x.VisitId == visitId);
            womanVisit.LaboratoryVmp = womanVisit.VisitVmp.Laboratories.FirstOrDefault(x => x.VisitId == visitId);
            womanVisit.WomanVmp = womanVisit.VisitVmp.Womans.FirstOrDefault(x => x.VisitId == visitId);
            womanVisit.FollowUpVmp = womanVisit.VisitVmp.FollowUps.FirstOrDefault(x => x.VisitId == visitId);
            return View(womanVisit);
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

            WomanVisit womanVisit = new WomanVisit();
            womanVisit.HistoryDiseasVmp = db.HistoryDiseases.FirstOrDefault(x => x.PatientId == patientId);
            womanVisit.VisitVmp = db.Visits.FirstOrDefault(v => v.VisitId == visitId);
            womanVisit.ChiefComplaintVmp = db.ChiefComplaints.FirstOrDefault(x => x.VisitId == visitId);
            womanVisit.PhysicalExaminationVmp = db.PhysicalExaminations.FirstOrDefault(x => x.VisitId == visitId);
            womanVisit.VitalSignVmp = db.VitalSigns.FirstOrDefault(x => x.VisitId == visitId);
            womanVisit.LaboratoryVmp = db.Laboratories.FirstOrDefault(x => x.VisitId == visitId);
            womanVisit.WomanVmp = db.Womans.FirstOrDefault(x => x.VisitId == visitId);
            womanVisit.FollowUpVmp = db.FollowUps.FirstOrDefault(x => x.VisitId == visitId);
            return View(womanVisit);
        }

        [HttpPost]
        public ActionResult Edit([Bind()] WomanVisit womanVisit, FormCollection fCVisit)
        {
            //در اینجا کدهای آپ دیت نوشته شود.
            if (ModelState.IsValid)
            {
                int patientId = Convert.ToInt32(fCVisit["PatientId"]);
                int visitId = Convert.ToInt32(fCVisit["VisitId"]);

                var historyDiseas = db.HistoryDiseases.FirstOrDefault(x => x.PatientId == patientId);
                historyDiseas.Description = womanVisit.HistoryDiseasVmp.Description;
                db.Entry(historyDiseas).State = EntityState.Modified;
                db.SaveChanges();

                var chiefComplaint = db.ChiefComplaints.FirstOrDefault(x => x.VisitId == visitId);
                chiefComplaint.ChestPain = womanVisit.ChiefComplaintVmp.ChestPain;
                chiefComplaint.Dyspnea = womanVisit.ChiefComplaintVmp.Dyspnea;
                chiefComplaint.Syncope = womanVisit.ChiefComplaintVmp.Syncope;
                chiefComplaint.Palpitation = womanVisit.ChiefComplaintVmp.Palpitation;
                chiefComplaint.Edema = womanVisit.ChiefComplaintVmp.Edema;
                chiefComplaint.ETC = womanVisit.ChiefComplaintVmp.ETC;
                db.Entry(chiefComplaint).State = EntityState.Modified;
                db.SaveChanges();

                var physicalExamination = db.PhysicalExaminations.FirstOrDefault(x => x.VisitId == visitId);
                physicalExamination.Heart = womanVisit.PhysicalExaminationVmp.Heart;
                physicalExamination.Lung = womanVisit.PhysicalExaminationVmp.Lung;
                physicalExamination.Extremities = womanVisit.PhysicalExaminationVmp.Extremities;
                physicalExamination.ETC = womanVisit.PhysicalExaminationVmp.ETC;
                db.Entry(physicalExamination).State = EntityState.Modified;
                db.SaveChanges();

                var vitalSign = db.VitalSigns.FirstOrDefault(x => x.VisitId == visitId);
                vitalSign.BP = womanVisit.VitalSignVmp.BP;
                vitalSign.HR = womanVisit.VitalSignVmp.HR;
                vitalSign.RR = womanVisit.VitalSignVmp.RR;
                db.Entry(vitalSign).State = EntityState.Modified;
                db.SaveChanges();

                var laboratory = db.Laboratories.FirstOrDefault(x => x.VisitId == visitId);
                laboratory.BUN = womanVisit.LaboratoryVmp.BUN;
                laboratory.Cr = womanVisit.LaboratoryVmp.Cr;
                laboratory.Na = womanVisit.LaboratoryVmp.Na;
                laboratory.K = womanVisit.LaboratoryVmp.K;
                laboratory.Ca = womanVisit.LaboratoryVmp.Ca;
                laboratory.Mg = womanVisit.LaboratoryVmp.Mg;
                laboratory.FBS = womanVisit.LaboratoryVmp.FBS;
                laboratory.Hb1AC = womanVisit.LaboratoryVmp.Hb1AC;
                laboratory.Chol = womanVisit.LaboratoryVmp.Chol;
                laboratory.LDL = womanVisit.LaboratoryVmp.LDL;
                laboratory.HDL = womanVisit.LaboratoryVmp.HDL;
                laboratory.TG = womanVisit.LaboratoryVmp.TG;
                laboratory.VitOHD3 = womanVisit.LaboratoryVmp.VitOHD3;
                laboratory.CTnI = womanVisit.LaboratoryVmp.CTnI;
                laboratory.CKMB = womanVisit.LaboratoryVmp.CKMB;
                laboratory.AST = womanVisit.LaboratoryVmp.AST;
                laboratory.ALT = womanVisit.LaboratoryVmp.ALT;
                laboratory.ALP = womanVisit.LaboratoryVmp.ALP;
                laboratory.BiL = womanVisit.LaboratoryVmp.BiL;
                laboratory.PSA = womanVisit.LaboratoryVmp.PSA;
                laboratory.TSH = womanVisit.LaboratoryVmp.TSH;
                laboratory.T3 = womanVisit.LaboratoryVmp.T3;
                laboratory.T4 = womanVisit.LaboratoryVmp.T4;
                laboratory.T3RU = womanVisit.LaboratoryVmp.T3RU;
                db.Entry(laboratory).State = EntityState.Modified;
                db.SaveChanges();

                var woman = db.Womans.FirstOrDefault(x => x.VisitId == visitId);
                woman.Dch = womanVisit.WomanVmp.Dch;
                woman.Lch = womanVisit.WomanVmp.Lch;
                woman.Ab = womanVisit.WomanVmp.Ab;
                woman.P = womanVisit.WomanVmp.P;
                woman.G = womanVisit.WomanVmp.G;
                woman.BloodGroup = womanVisit.WomanVmp.BloodGroup;
                woman.Rh = womanVisit.WomanVmp.Rh;
                woman.CaesareanNo = womanVisit.WomanVmp.CaesareanNo;

                woman.VantvnzNo = womanVisit.WomanVmp.VantvnzNo;
                woman.PreviousBirthWeight = womanVisit.WomanVmp.PreviousBirthWeight;
                woman.LastPregnancy = womanVisit.WomanVmp.LastPregnancy;
                woman.NaturalChildbirthNo = womanVisit.WomanVmp.NaturalChildbirthNo;
                woman.EDCAccordingLMP = womanVisit.WomanVmp.EDCAccordingLMP;
                woman.EDCAccordingSonography = womanVisit.WomanVmp.EDCAccordingSonography;
                woman.FirstSonographyResult = womanVisit.WomanVmp.FirstSonographyResult;
                woman.SecondSonographyResult = womanVisit.WomanVmp.SecondSonographyResult;

                woman.FirstLabResult = womanVisit.WomanVmp.FirstLabResult;
                woman.SecondLabResult = womanVisit.WomanVmp.SecondLabResult;
                woman.PreviousPregnancyWeight = womanVisit.WomanVmp.PreviousPregnancyWeight;
                woman.Stature = womanVisit.WomanVmp.Stature;
                woman.BMI = womanVisit.WomanVmp.BMI;
                woman.HistoryBreastDiseases = womanVisit.WomanVmp.HistoryBreastDiseases;
                woman.HistoryTakingCertainMedications = womanVisit.WomanVmp.HistoryTakingCertainMedications;
                woman.HistoryVaccination = womanVisit.WomanVmp.HistoryVaccination;

                db.Entry(woman).State = EntityState.Modified;
                db.SaveChanges();

                var followUp = db.FollowUps.FirstOrDefault(x => x.VisitId == visitId);
                followUp.Description = womanVisit.FollowUpVmp.Description;
                followUp.Date = womanVisit.FollowUpVmp.Date;
                db.Entry(followUp).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("VisitReport", new { visitId = fCVisit["VisitId"], patientId = fCVisit["PatientId"], clinicId = fCVisit["ClinicId"] });
            }
            return View(womanVisit);
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