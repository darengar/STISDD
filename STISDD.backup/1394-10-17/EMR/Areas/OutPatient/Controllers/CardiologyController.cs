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
    public class CardiologyController : Controller
    {
        private Entities db = new Entities();
        private ShafaEntities hisdb = new ShafaEntities();

        public ActionResult Index(int? clinicId, string SearchBy, string Search, int? page)
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
            return PartialView("_Cardiology", patientlist);
        }

        [HttpGet]
        public ActionResult Create(int? patientId, int? clinicId, int? admissionId)
        {
            ViewBag.PatientId = patientId;
            ViewBag.ClinicId = clinicId;
            ViewBag.AdmissionId = admissionId;

            CardiologyVisit cardiologyVisit = new CardiologyVisit();

            HistoryDiseas historyDiseas = db.HistoryDiseases.FirstOrDefault(x => x.PatientId == patientId);
            if (historyDiseas != null)
                cardiologyVisit.HistoryDiseasVmp = historyDiseas; 
                                                   
            cardiologyVisit.FollowUpVmp = new FollowUp();
            cardiologyVisit.FollowUpVmp.Date =  System.DateTime.Now.Date;
            return View(cardiologyVisit);
        }
        
        [HttpPost]
        public ActionResult Create([Bind()] CardiologyVisit cardiologyVisit, FormCollection fc)
        {
            int currentVisitId = 0;
            if (ModelState.IsValid)
            {
                try
                {
                    int patientId = Convert.ToInt32(fc["PatientId"]);
                    var historyDiseas = db.HistoryDiseases.FirstOrDefault(x => x.PatientId == patientId);
                    if (historyDiseas == null)
                    {
                        cardiologyVisit.HistoryDiseasVmp.PatientId = Convert.ToInt32(fc["PatientId"]);
                        cardiologyVisit.HistoryDiseasVmp.LeadTime = DateTime.Now;
                        db.HistoryDiseases.Add(cardiologyVisit.HistoryDiseasVmp);
                        db.SaveChanges();
                    }
                    else
                    {
                        historyDiseas = db.HistoryDiseases.Find(historyDiseas.HistoryDiseaseId);
                        historyDiseas.Description = cardiologyVisit.HistoryDiseasVmp.Description;
                        db.Entry(historyDiseas).State = EntityState.Modified;                       
                        db.SaveChanges();
                    }                    

                    var visit = new Visit();
                    visit.PatientId = Convert.ToInt32(fc["PatientId"]);
                    visit.ClinicId = Convert.ToInt32(fc["ClinicId"]);
                    visit.KindOfTherapyId = 1;//outpatient
                    visit.AdmissionId = Convert.ToInt32(fc["AdmissionId"]);
                    if (Session["DoctorId"] != null)
                        visit.DoctorId = Convert.ToInt32(Session["DoctorId"]);//enter from session
                    if (Session["CollegianId"] != null)
                        visit.CollegianId = Convert.ToInt32(Session["CollegianId"]);//enter from session                               
                    visit.DateOfStay = Convert.ToDateTime(DateTime.Now.ToString());
                    db.Visits.Add(visit);
                    db.SaveChanges();
                    currentVisitId = db.Visits.Max(v => v.VisitId);

                    cardiologyVisit.ChiefComplaintVmp.VisitId = currentVisitId;
                    cardiologyVisit.ChiefComplaintVmp.LeadTime = DateTime.Now;
                    db.ChiefComplaints.Add(cardiologyVisit.ChiefComplaintVmp);

                    cardiologyVisit.PhysicalExaminationVmp.VisitId = currentVisitId;
                    cardiologyVisit.PhysicalExaminationVmp.LeadTime = DateTime.Now;
                    db.PhysicalExaminations.Add(cardiologyVisit.PhysicalExaminationVmp);

                    cardiologyVisit.VitalSignVmp.VisitId = currentVisitId;
                    cardiologyVisit.VitalSignVmp.LeadTime = DateTime.Now;
                    db.VitalSigns.Add(cardiologyVisit.VitalSignVmp);

                    cardiologyVisit.LaboratoryVmp.VisitId = currentVisitId;
                    cardiologyVisit.LaboratoryVmp.LeadTime = DateTime.Now;
                    db.Laboratories.Add(cardiologyVisit.LaboratoryVmp);

                    cardiologyVisit.CardiologyVmp.VisitId = currentVisitId;
                    cardiologyVisit.CardiologyVmp.LeadTime = DateTime.Now;
                    db.Cardiologys.Add(cardiologyVisit.CardiologyVmp);

                    cardiologyVisit.FollowUpVmp.VisitId = currentVisitId;
                    if (fc["FollowUpDate"] == string.Empty)
                        cardiologyVisit.FollowUpVmp.Date = Convert.ToDateTime(fc["DateFllowUp"].ToString());
                    db.FollowUps.Add(cardiologyVisit.FollowUpVmp);

                    db.SaveChanges();

                    return RedirectToAction("VisitReport", new { patientId = fc["PatientId"], clinicId = fc["ClinicId"], admissionId = fc["AdmissionId"], visitId = currentVisitId });
                 }
                catch (Exception)
                {
                    Redirect("~/Views/Home/Index.cshtml");                    
                }               
            }
            return View(cardiologyVisit);                                                                                             
        }

        public ActionResult VisitReport(int? patientId, int? clinicId, int? admissionId, int? visitId)
        {            
            ViewBag.PatientId = patientId;
            ViewBag.ClinicId = clinicId;
            ViewBag.AdmissionId = admissionId;
            ViewBag.VisitId = visitId;
            //در اینجا کدهای نمایش در صفحه نوشته شود
            ViewBag.ClinicName = "قلب"; //db.Clinics.Where(c => c.ClinicId == clinicId).FirstOrDefault().Name;
            string patientName  = hisdb.AdmIDs.Where(p => p.ID_Code == patientId).FirstOrDefault().Name; // db.Patients.Where(p => p.PatientId == patientId).FirstOrDefault().FullName;
            patientName += " ";
            patientName += patientName = hisdb.AdmIDs.Where(p => p.ID_Code == patientId).FirstOrDefault().Family;
            ViewBag.PatientName = patientName; 
            CardiologyVisit cardiologyVisit = new CardiologyVisit();
            cardiologyVisit.HistoryDiseasVmp = db.HistoryDiseases.Where(p => p.PatientId == patientId).FirstOrDefault();
            cardiologyVisit.VisitVmp = db.Visits.Where(v => v.VisitId == visitId).FirstOrDefault();
            cardiologyVisit.ChiefComplaintVmp = cardiologyVisit.VisitVmp.ChiefComplaints.FirstOrDefault(x => x.VisitId == visitId);
            cardiologyVisit.PhysicalExaminationVmp = cardiologyVisit.VisitVmp.PhysicalExaminations.FirstOrDefault(x => x.VisitId == visitId);
            cardiologyVisit.VitalSignVmp = cardiologyVisit.VisitVmp.VitalSigns.FirstOrDefault(x => x.VisitId == visitId);
            cardiologyVisit.LaboratoryVmp = cardiologyVisit.VisitVmp.Laboratories.FirstOrDefault(x => x.VisitId == visitId);
            cardiologyVisit.CardiologyVmp = cardiologyVisit.VisitVmp.Cardiologys.FirstOrDefault(x => x.VisitId == visitId);
            cardiologyVisit.FollowUpVmp = cardiologyVisit.VisitVmp.FollowUps.FirstOrDefault(x => x.VisitId == visitId);
            return View(cardiologyVisit);
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
            if(Session["DoctorId"] != null)
            {
                int iDoctorId = Convert.ToInt32(Session["DoctorId"].ToString());
                patientClinicVisit = patientClinicVisit.Where(x => x.DoctorId == iDoctorId);
            }                
            if(Session["CollegianId"] != null)
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

            CardiologyVisit cardiologyVisit = new CardiologyVisit();
            cardiologyVisit.HistoryDiseasVmp = db.HistoryDiseases.FirstOrDefault(x => x.PatientId == patientId); 
            cardiologyVisit.VisitVmp = db.Visits.FirstOrDefault(v => v.VisitId == visitId);
            cardiologyVisit.ChiefComplaintVmp = db.ChiefComplaints.FirstOrDefault(x => x.VisitId == visitId);
            cardiologyVisit.PhysicalExaminationVmp = db.PhysicalExaminations.FirstOrDefault(x => x.VisitId == visitId);
            cardiologyVisit.VitalSignVmp = db.VitalSigns.FirstOrDefault(x => x.VisitId == visitId);
            cardiologyVisit.LaboratoryVmp = db.Laboratories.FirstOrDefault(x => x.VisitId == visitId);
            cardiologyVisit.CardiologyVmp = db.Cardiologys.FirstOrDefault(x => x.VisitId == visitId);
            cardiologyVisit.FollowUpVmp = db.FollowUps.FirstOrDefault(x => x.VisitId == visitId);
            return View(cardiologyVisit);
        }
        
        [HttpPost]
        public ActionResult Edit([Bind()] CardiologyVisit cardiologyVisit, FormCollection fCVisit)
        {            
            //در اینجا کدهای آپ دیت نوشته شود.
            if (ModelState.IsValid)
            {
                int patientId = Convert.ToInt32(fCVisit["PatientId"]);
                int visitId = Convert.ToInt32(fCVisit["VisitId"]);

                var historyDiseas = db.HistoryDiseases.FirstOrDefault(x => x.PatientId == patientId);
                historyDiseas.Description = cardiologyVisit.HistoryDiseasVmp.Description;
                db.Entry(historyDiseas).State = EntityState.Modified;
                db.SaveChanges();

                var chiefComplaint = db.ChiefComplaints.FirstOrDefault(x => x.VisitId == visitId);
                chiefComplaint.ChestPain = cardiologyVisit.ChiefComplaintVmp.ChestPain;
                chiefComplaint.Dyspnea = cardiologyVisit.ChiefComplaintVmp.Dyspnea;
                chiefComplaint.Syncope = cardiologyVisit.ChiefComplaintVmp.Syncope;
                chiefComplaint.Palpitation = cardiologyVisit.ChiefComplaintVmp.Palpitation;
                chiefComplaint.Edema = cardiologyVisit.ChiefComplaintVmp.Edema;
                chiefComplaint.ETC = cardiologyVisit.ChiefComplaintVmp.ETC;
                db.Entry(chiefComplaint).State = EntityState.Modified;
                db.SaveChanges();

                var physicalExamination = db.PhysicalExaminations.FirstOrDefault(x=>x.VisitId == visitId);
                physicalExamination.Heart = cardiologyVisit.PhysicalExaminationVmp.Heart;
                physicalExamination.Lung = cardiologyVisit.PhysicalExaminationVmp.Lung;
                physicalExamination.Extremities = cardiologyVisit.PhysicalExaminationVmp.Extremities;
                physicalExamination.ETC = cardiologyVisit.PhysicalExaminationVmp.ETC;
                db.Entry(physicalExamination).State = EntityState.Modified;
                db.SaveChanges();

                var vitalSign = db.VitalSigns.FirstOrDefault(x=>x.VisitId == visitId);
                vitalSign.BP = cardiologyVisit.VitalSignVmp.BP;
                vitalSign.HR = cardiologyVisit.VitalSignVmp.HR;
                vitalSign.RR = cardiologyVisit.VitalSignVmp.RR;
                db.Entry(vitalSign).State = EntityState.Modified;
                db.SaveChanges();

                var laboratory = db.Laboratories.FirstOrDefault(x=>x.VisitId == visitId);
                laboratory.BUN = cardiologyVisit.LaboratoryVmp.BUN;
                laboratory.Cr = cardiologyVisit.LaboratoryVmp.Cr;
                laboratory.Na = cardiologyVisit.LaboratoryVmp.Na;
                laboratory.K = cardiologyVisit.LaboratoryVmp.K;
                laboratory.Ca = cardiologyVisit.LaboratoryVmp.Ca;
                laboratory.Mg = cardiologyVisit.LaboratoryVmp.Mg;
                laboratory.FBS = cardiologyVisit.LaboratoryVmp.FBS;
                laboratory.Hb1AC = cardiologyVisit.LaboratoryVmp.Hb1AC;
                laboratory.Chol = cardiologyVisit.LaboratoryVmp.Chol;
                laboratory.LDL = cardiologyVisit.LaboratoryVmp.LDL;
                laboratory.HDL = cardiologyVisit.LaboratoryVmp.HDL;
                laboratory.TG = cardiologyVisit.LaboratoryVmp.TG;
                laboratory.VitOHD3 = cardiologyVisit.LaboratoryVmp.VitOHD3;
                laboratory.CTnI = cardiologyVisit.LaboratoryVmp.CTnI;
                laboratory.CKMB = cardiologyVisit.LaboratoryVmp.CKMB;
                laboratory.AST = cardiologyVisit.LaboratoryVmp.AST;
                laboratory.ALT = cardiologyVisit.LaboratoryVmp.ALT;
                laboratory.ALP = cardiologyVisit.LaboratoryVmp.ALP;
                laboratory.BiL = cardiologyVisit.LaboratoryVmp.BiL;
                laboratory.PSA = cardiologyVisit.LaboratoryVmp.PSA;
                laboratory.TSH = cardiologyVisit.LaboratoryVmp.TSH;
                laboratory.T3 = cardiologyVisit.LaboratoryVmp.T3;
                laboratory.T4 = cardiologyVisit.LaboratoryVmp.T4;
                laboratory.T3RU = cardiologyVisit.LaboratoryVmp.T3RU;
                db.Entry(laboratory).State = EntityState.Modified;
                db.SaveChanges();

                var cardiology = db.Cardiologys.FirstOrDefault(x => x.VisitId == visitId);
                cardiology.ECG = cardiologyVisit.CardiologyVmp.ECG;
                cardiology.Echocardiography = cardiologyVisit.CardiologyVmp.Echocardiography;
                cardiology.Angiography = cardiologyVisit.CardiologyVmp.Angiography;
                cardiology.PCI = cardiologyVisit.CardiologyVmp.PCI;
                cardiology.EST = cardiologyVisit.CardiologyVmp.EST;
                cardiology.SPECTMPI = cardiologyVisit.CardiologyVmp.SPECTMPI;
                cardiology.Diagnosis = cardiologyVisit.CardiologyVmp.Diagnosis;
                cardiology.Plan = cardiologyVisit.CardiologyVmp.Plan;
                db.Entry(cardiology).State = EntityState.Modified;
                db.SaveChanges();

                var followUp = db.FollowUps.FirstOrDefault(x => x.VisitId == visitId);
                followUp.Description = cardiologyVisit.FollowUpVmp.Description;
                followUp.Date = cardiologyVisit.FollowUpVmp.Date;
                db.Entry(followUp).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("VisitReport", new { visitId = fCVisit["VisitId"], patientId = fCVisit["PatientId"], clinicId = fCVisit["ClinicId"] });
            }
            return View(cardiologyVisit);
        }

        public ActionResult ReportPDF(string patientName, int? patientId, int? clinicId, int? admissionId, int? visitId)
        {
            ViewBag.PatientId = patientId;
            ViewBag.ClinicId = clinicId;
            ViewBag.AdmissionId = admissionId;
            ViewBag.VisitId = visitId;

            return new ActionAsPdf("VisitReport", new { patientId = ViewBag.PatientId, clinicId = ViewBag.ClinicId, admissionId = ViewBag.AdmissionId, visitId = ViewBag.VisitId })
            {
                FileName = string.Format("{0}.pdf" , patientName)                
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
