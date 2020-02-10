using System;
using System.Linq;
using System.Web.Mvc;
using EMR.Models;
using EMR.Areas.OutPatient.ViewModels;
using System.Data.Entity;
using Rotativa;
using PagedList;


namespace EMR.Areas.OutPatient.Controllers
{
    public class GynocologyController : Controller
    {
        private Entities db = new Entities();
        private ShafaEntities hisdb = new ShafaEntities();

        public ActionResult Index(int? clinicId, string SearchBy, string Search, int? page)
        {
            ViewBag.ClinicId = clinicId;

            var gynocologyPatientAdm = hisdb.EMRGynocologyViews;
            if (SearchBy == "Family")
            {
                return View(gynocologyPatientAdm.OrderByDescending(x => x.ID_Code).Where(x => x.Family == Search || Search == null).ToList().ToPagedList(page ?? 1, 20));
            }
            else if (SearchBy == "NationalCode")
            {
                return View(gynocologyPatientAdm.OrderByDescending(x => x.ID_Code).Where(x => x.NationalCode == Search || Search == null).ToList().ToPagedList(page ?? 1, 20));
            }
            else
            {
                return View(gynocologyPatientAdm.OrderByDescending(x => x.ID_Code).ToList().ToPagedList(page ?? 1, 20));
            }                                               
        }

        public ActionResult AjaxPage(int? page)
        {
            var patientlist = hisdb.EMRGynocologyViews.OrderByDescending(x => x.ID_Code).ToList().ToPagedList(page ?? 1, 20);
            return PartialView("_Gynocology", patientlist);
        }

        [HttpGet]
        public ActionResult Create(int? patientId, int? clinicId, int? admissionId)
        {
            ViewBag.PatientId = patientId;
            ViewBag.ClinicId = clinicId;
            ViewBag.AdmissionId = admissionId;

            GynocologyVisit gynocologyVisit = new GynocologyVisit();

            HistoryDiseas historyDiseas = db.HistoryDiseases.FirstOrDefault(x => x.PatientId == patientId);
            if (historyDiseas != null)
                gynocologyVisit.HistoryDiseasVmp = historyDiseas; // db.HistoryDiseases.Find(historyDiseas.HistoryDiseaseId);

            gynocologyVisit.FollowUpVmp = new FollowUp();
            gynocologyVisit.FollowUpVmp.Date = System.DateTime.Now;
            return View(gynocologyVisit);
        }

        [HttpPost]
        public ActionResult Create([Bind()] GynocologyVisit gynocologyVisit, FormCollection fCVisit)
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
                        gynocologyVisit.HistoryDiseasVmp.PatientId = Convert.ToInt32(fCVisit["PatientId"]);
                        gynocologyVisit.HistoryDiseasVmp.LeadTime = DateTime.Now;
                        db.HistoryDiseases.Add(gynocologyVisit.HistoryDiseasVmp);
                        db.SaveChanges();
                    }
                    else
                    {
                        historyDiseas = db.HistoryDiseases.Find(historyDiseas.HistoryDiseaseId);
                        historyDiseas.Description = gynocologyVisit.HistoryDiseasVmp.Description;
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

                    gynocologyVisit.ChiefComplaintVmp.VisitId = currentVisitId;
                    gynocologyVisit.ChiefComplaintVmp.LeadTime = DateTime.Now;
                    db.ChiefComplaints.Add(gynocologyVisit.ChiefComplaintVmp);

                    gynocologyVisit.PhysicalExaminationVmp.VisitId = currentVisitId;
                    gynocologyVisit.PhysicalExaminationVmp.LeadTime = DateTime.Now;
                    db.PhysicalExaminations.Add(gynocologyVisit.PhysicalExaminationVmp);

                    gynocologyVisit.VitalSignVmp.VisitId = currentVisitId;
                    gynocologyVisit.VitalSignVmp.LeadTime = DateTime.Now;
                    db.VitalSigns.Add(gynocologyVisit.VitalSignVmp);

                    gynocologyVisit.LaboratoryVmp.VisitId = currentVisitId;
                    gynocologyVisit.LaboratoryVmp.LeadTime = DateTime.Now;
                    db.Laboratories.Add(gynocologyVisit.LaboratoryVmp);

                    gynocologyVisit.GynocologyVmp.VisitId = currentVisitId;
                    gynocologyVisit.GynocologyVmp.LeadTime = DateTime.Now;
                    db.Gynocologys.Add(gynocologyVisit.GynocologyVmp);

                    gynocologyVisit.FollowUpVmp.VisitId = currentVisitId;
                    if (fCVisit["FollowUpDate"] == string.Empty)
                        gynocologyVisit.FollowUpVmp.Date = Convert.ToDateTime(fCVisit["DateFllowUp"].ToString());
                    db.FollowUps.Add(gynocologyVisit.FollowUpVmp);

                    db.SaveChanges();

                    return RedirectToAction("VisitReport", new { patientId = fCVisit["PatientId"], clinicId = fCVisit["ClinicId"], admissionId = fCVisit["AdmissionId"], visitId = currentVisitId });
                }
                catch (Exception)
                {
                    Redirect("~/Views/Home/Index.cshtml");
                }
            }
            return View(gynocologyVisit);
        }

        public ActionResult VisitReport(int? patientId, int? clinicId, int? admissionId, int? visitId)
        {
            ViewBag.PatientId = patientId;
            ViewBag.ClinicId = clinicId;
            ViewBag.AdmissionId = admissionId;
            ViewBag.VisitId = visitId;
            //در اینجا کدهای نمایش در صفحه نوشته شود
            ViewBag.ClinicName = "زنان و زایمان"; //db.Clinics.Where(c => c.ClinicId == clinicId).FirstOrDefault().Name;
            string patientName = hisdb.AdmIDs.Where(p => p.ID_Code == patientId).FirstOrDefault().Name; // db.Patients.Where(p => p.PatientId == patientId).FirstOrDefault().FullName;
            patientName += " ";
            patientName += patientName = hisdb.AdmIDs.Where(p => p.ID_Code == patientId).FirstOrDefault().Family;
            ViewBag.PatientName = patientName;
            GynocologyVisit gynocologyVisit = new GynocologyVisit();
            gynocologyVisit.HistoryDiseasVmp = db.HistoryDiseases.Where(p => p.PatientId == patientId).FirstOrDefault();
            gynocologyVisit.VisitVmp = db.Visits.Where(v => v.VisitId == visitId).FirstOrDefault();
            gynocologyVisit.ChiefComplaintVmp = gynocologyVisit.VisitVmp.ChiefComplaints.FirstOrDefault(x => x.VisitId == visitId);
            gynocologyVisit.PhysicalExaminationVmp = gynocologyVisit.VisitVmp.PhysicalExaminations.FirstOrDefault(x => x.VisitId == visitId);
            gynocologyVisit.VitalSignVmp = gynocologyVisit.VisitVmp.VitalSigns.FirstOrDefault(x => x.VisitId == visitId);
            gynocologyVisit.LaboratoryVmp = gynocologyVisit.VisitVmp.Laboratories.FirstOrDefault(x => x.VisitId == visitId);
            gynocologyVisit.GynocologyVmp = gynocologyVisit.VisitVmp.Gynocologys.FirstOrDefault(x => x.VisitId == visitId);
            gynocologyVisit.FollowUpVmp = gynocologyVisit.VisitVmp.FollowUps.FirstOrDefault(x => x.VisitId == visitId);
            return View(gynocologyVisit);
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
            var patientClinicVisit = db.Visits.Where(x => (x.PatientId == patientId && x.ClinicId == clinicId && x.KindOfTherapyId == 1));
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

            GynocologyVisit gynocologyVisit = new GynocologyVisit();
            gynocologyVisit.HistoryDiseasVmp = db.HistoryDiseases.FirstOrDefault(x => x.PatientId == patientId);
            gynocologyVisit.VisitVmp = db.Visits.FirstOrDefault(v => v.VisitId == visitId);
            gynocologyVisit.ChiefComplaintVmp = db.ChiefComplaints.FirstOrDefault(x => x.VisitId == visitId);
            gynocologyVisit.PhysicalExaminationVmp = db.PhysicalExaminations.FirstOrDefault(x => x.VisitId == visitId);
            gynocologyVisit.VitalSignVmp = db.VitalSigns.FirstOrDefault(x => x.VisitId == visitId);
            gynocologyVisit.LaboratoryVmp = db.Laboratories.FirstOrDefault(x => x.VisitId == visitId);
            gynocologyVisit.GynocologyVmp = db.Gynocologys.FirstOrDefault(x => x.VisitId == visitId);
            gynocologyVisit.FollowUpVmp = db.FollowUps.FirstOrDefault(x => x.VisitId == visitId);
            return View(gynocologyVisit);
        }

        [HttpPost]
        public ActionResult Edit([Bind()] GynocologyVisit gynocologyVisit, FormCollection fCVisit)
        {
            //در اینجا کدهای آپ دیت نوشته شود.
            if (ModelState.IsValid)
            {
                int patientId = Convert.ToInt32(fCVisit["PatientId"]);
                int visitId = Convert.ToInt32(fCVisit["VisitId"]);

                var historyDiseas = db.HistoryDiseases.FirstOrDefault(x => x.PatientId == patientId);
                historyDiseas.Description = gynocologyVisit.HistoryDiseasVmp.Description;
                db.Entry(historyDiseas).State = EntityState.Modified;
                db.SaveChanges();

                var chiefComplaint = db.ChiefComplaints.FirstOrDefault(x => x.VisitId == visitId);
                chiefComplaint.ChestPain = gynocologyVisit.ChiefComplaintVmp.ChestPain;
                chiefComplaint.Dyspnea = gynocologyVisit.ChiefComplaintVmp.Dyspnea;
                chiefComplaint.Syncope = gynocologyVisit.ChiefComplaintVmp.Syncope;
                chiefComplaint.Palpitation = gynocologyVisit.ChiefComplaintVmp.Palpitation;
                chiefComplaint.Edema = gynocologyVisit.ChiefComplaintVmp.Edema;
                chiefComplaint.ETC = gynocologyVisit.ChiefComplaintVmp.ETC;
                db.Entry(chiefComplaint).State = EntityState.Modified;
                db.SaveChanges();

                var physicalExamination = db.PhysicalExaminations.FirstOrDefault(x => x.VisitId == visitId);
                physicalExamination.Heart = gynocologyVisit.PhysicalExaminationVmp.Heart;
                physicalExamination.Lung = gynocologyVisit.PhysicalExaminationVmp.Lung;
                physicalExamination.Extremities = gynocologyVisit.PhysicalExaminationVmp.Extremities;
                physicalExamination.ETC = gynocologyVisit.PhysicalExaminationVmp.ETC;
                db.Entry(physicalExamination).State = EntityState.Modified;
                db.SaveChanges();

                var vitalSign = db.VitalSigns.FirstOrDefault(x => x.VisitId == visitId);
                vitalSign.BP = gynocologyVisit.VitalSignVmp.BP;
                vitalSign.HR = gynocologyVisit.VitalSignVmp.HR;
                vitalSign.RR = gynocologyVisit.VitalSignVmp.RR;
                db.Entry(vitalSign).State = EntityState.Modified;
                db.SaveChanges();

                var laboratory = db.Laboratories.FirstOrDefault(x => x.VisitId == visitId);
                laboratory.BUN = gynocologyVisit.LaboratoryVmp.BUN;
                laboratory.Cr = gynocologyVisit.LaboratoryVmp.Cr;
                laboratory.Na = gynocologyVisit.LaboratoryVmp.Na;
                laboratory.K = gynocologyVisit.LaboratoryVmp.K;
                laboratory.Ca = gynocologyVisit.LaboratoryVmp.Ca;
                laboratory.Mg = gynocologyVisit.LaboratoryVmp.Mg;
                laboratory.FBS = gynocologyVisit.LaboratoryVmp.FBS;
                laboratory.Hb1AC = gynocologyVisit.LaboratoryVmp.Hb1AC;
                laboratory.Chol = gynocologyVisit.LaboratoryVmp.Chol;
                laboratory.LDL = gynocologyVisit.LaboratoryVmp.LDL;
                laboratory.HDL = gynocologyVisit.LaboratoryVmp.HDL;
                laboratory.TG = gynocologyVisit.LaboratoryVmp.TG;
                laboratory.VitOHD3 = gynocologyVisit.LaboratoryVmp.VitOHD3;
                laboratory.CTnI = gynocologyVisit.LaboratoryVmp.CTnI;
                laboratory.CKMB = gynocologyVisit.LaboratoryVmp.CKMB;
                laboratory.AST = gynocologyVisit.LaboratoryVmp.AST;
                laboratory.ALT = gynocologyVisit.LaboratoryVmp.ALT;
                laboratory.ALP = gynocologyVisit.LaboratoryVmp.ALP;
                laboratory.BiL = gynocologyVisit.LaboratoryVmp.BiL;
                laboratory.PSA = gynocologyVisit.LaboratoryVmp.PSA;
                laboratory.TSH = gynocologyVisit.LaboratoryVmp.TSH;
                laboratory.T3 = gynocologyVisit.LaboratoryVmp.T3;
                laboratory.T4 = gynocologyVisit.LaboratoryVmp.T4;
                laboratory.T3RU = gynocologyVisit.LaboratoryVmp.T3RU;
                db.Entry(laboratory).State = EntityState.Modified;
                db.SaveChanges();

                var gynocology = db.Gynocologys.FirstOrDefault(x => x.VisitId == visitId);
                gynocology.Dch = gynocologyVisit.GynocologyVmp.Dch;
                gynocology.Lch = gynocologyVisit.GynocologyVmp.Lch;
                gynocology.Ab = gynocologyVisit.GynocologyVmp.Ab;
                gynocology.P = gynocologyVisit.GynocologyVmp.P;
                gynocology.G = gynocologyVisit.GynocologyVmp.G;
                gynocology.BloodGroup = gynocologyVisit.GynocologyVmp.BloodGroup;
                gynocology.Rh = gynocologyVisit.GynocologyVmp.Rh;
                gynocology.CaesareanNo = gynocologyVisit.GynocologyVmp.CaesareanNo;

                gynocology.VantvnzNo = gynocologyVisit.GynocologyVmp.VantvnzNo;
                gynocology.PreviousBirthWeight = gynocologyVisit.GynocologyVmp.PreviousBirthWeight;
                gynocology.LastPregnancy = gynocologyVisit.GynocologyVmp.LastPregnancy;
                gynocology.NaturalChildbirthNo = gynocologyVisit.GynocologyVmp.NaturalChildbirthNo;
                gynocology.EDCAccordingLMP = gynocologyVisit.GynocologyVmp.EDCAccordingLMP;
                gynocology.EDCAccordingSonography = gynocologyVisit.GynocologyVmp.EDCAccordingSonography;
                gynocology.FirstSonographyResult = gynocologyVisit.GynocologyVmp.FirstSonographyResult;
                gynocology.SecondSonographyResult = gynocologyVisit.GynocologyVmp.SecondSonographyResult;

                gynocology.FirstLabResult = gynocologyVisit.GynocologyVmp.FirstLabResult;
                gynocology.SecondLabResult = gynocologyVisit.GynocologyVmp.SecondLabResult;
                gynocology.PreviousPregnancyWeight = gynocologyVisit.GynocologyVmp.PreviousPregnancyWeight;
                gynocology.Stature = gynocologyVisit.GynocologyVmp.Stature;
                gynocology.BMI = gynocologyVisit.GynocologyVmp.BMI;
                gynocology.HistoryBreastDiseases = gynocologyVisit.GynocologyVmp.HistoryBreastDiseases;
                gynocology.HistoryTakingCertainMedications = gynocologyVisit.GynocologyVmp.HistoryTakingCertainMedications;
                gynocology.HistoryVaccination = gynocologyVisit.GynocologyVmp.HistoryVaccination;

                db.Entry(gynocology).State = EntityState.Modified;
                db.SaveChanges();

                var followUp = db.FollowUps.FirstOrDefault(x => x.VisitId == visitId);
                followUp.Description = gynocologyVisit.FollowUpVmp.Description;
                followUp.Date = gynocologyVisit.FollowUpVmp.Date;
                db.Entry(followUp).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("VisitReport", new { visitId = fCVisit["VisitId"], patientId = fCVisit["PatientId"], clinicId = fCVisit["ClinicId"] });
            }
            return View(gynocologyVisit);
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