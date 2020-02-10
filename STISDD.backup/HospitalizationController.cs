using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EMR.Models;

namespace EMR.Controllers
{
    public class HospitalizationController : Controller
    {
        private EMRDBEntities db = new EMRDBEntities();
        
        public ActionResult Insert()
        {
            ViewBag.Sex = new SelectList(new[] { new { Id = 0, Sex = "زن" }, new { Id = 1, Sex = "مرد" } }, "Id", "Sex", 1);
            ViewBag.HospitalId = new SelectList(db.Hospitals, "HospitalId", "Name");
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name");
            ViewBag.DoctorId = new SelectList(db.Doctors, "DoctorId", "FullName");
            return View();
        }

        [HttpPost]
        public ActionResult Insert(FormCollection fcPatient)
        {
            Cardiology(fcPatient);
            return RedirectToAction("Insert");
        }

        private void Cardiology(FormCollection fcPatient)
        {
            try
            {
                var patient = new Patient();
                patient.FirstName = fcPatient["FirstName"].ToString();
                patient.LastName = fcPatient["LastName"].ToString();
                patient.Sex = (Convert.ToByte(fcPatient["Sex"]) == 0) ? false : true;
                patient.BirthDate = Convert.ToDateTime(fcPatient["BirthDate"].ToString());
                patient.BloodGroup = fcPatient["BloodGroup"];
                patient.NationalId = fcPatient["NationalId"].ToString();
                patient.Phone = fcPatient["Phone"].ToString();
                patient.CellPhone = fcPatient["CellPhone"].ToString();
                patient.Address = fcPatient["Address"].ToString();
                db.Patients.Add(patient);
                db.SaveChanges();

                var historyDiseas = new HistoryDiseas();
                historyDiseas.PatientId = db.Patients.Max(x => x.PatientId);
                historyDiseas.Description = fcPatient["PMHDescription"];
                db.HistoryDiseases.Add(historyDiseas);

                var hospitalization = new Hospitalization();
                hospitalization.DepartmentId = Convert.ToInt32(fcPatient["DepartmentId"]);
                hospitalization.PatientId = db.Patients.Max(x => x.PatientId);
                //hospitalization.DateOfStay = Convert.ToDateTime(fcPatient["DateOfStay"].ToString());
                hospitalization.DateOfStay = Convert.ToDateTime(DateTime.Now.ToString());

                int doctorId, hospitalId;
                doctorId = Convert.ToInt32(fcPatient["DoctorId"]);
                hospitalId = Convert.ToInt32(fcPatient["HospitalId"]);
                var existHospDoc = db.HospDocs.Where(x => (x.HospitalId == hospitalId) && (x.DoctorId == doctorId));
                if (0 == db.HospDocs.Where(x => (x.HospitalId == hospitalId) && (x.DoctorId == doctorId)).Count())
                {
                    var newHosDoc = new HospDoc();
                    newHosDoc.HospitalId = hospitalId;
                    newHosDoc.DoctorId = doctorId;
                    db.HospDocs.Add(newHosDoc);
                    db.SaveChanges();
                    hospitalization.HospDocId = db.HospDocs.Max(item => item.HospDocId);
                }
                else
                {
                    hospitalization.HospDocId = existHospDoc.First().HospDocId;
                }
                db.Hospitalizations.Add(hospitalization);
                db.SaveChanges();
                int hospitalizationId = db.Hospitalizations.Max(x => x.HospitalizationId);                

                var chiefComplaint = new ChiefComplaint();
                chiefComplaint.HospitalizationId = hospitalizationId;
                chiefComplaint.ChestPain = fcPatient["ChestPain"];
                chiefComplaint.Dyspnea = fcPatient["Dyspnea"];
                chiefComplaint.Syncope = fcPatient["Syncope"];
                chiefComplaint.Palpitation = fcPatient["Palpitation"];
                chiefComplaint.Edema = fcPatient["Edema"];
                chiefComplaint.ETC = fcPatient["ETC"];
                db.ChiefComplaints.Add(chiefComplaint);

                var physicalExamination = new PhysicalExamination();
                physicalExamination.HospitalizationId = hospitalizationId;
                physicalExamination.Heart = fcPatient["Heart"];
                physicalExamination.Lung = fcPatient["Lung"];
                physicalExamination.Extremities = fcPatient["Ext"];
                physicalExamination.ETC = fcPatient["ETC"];
                db.PhysicalExaminations.Add(physicalExamination);

                var vitalSign = new VitalSign();
                vitalSign.HospitalizationId = hospitalizationId;
                vitalSign.BP = fcPatient["BP"];
                vitalSign.HR = fcPatient["HR"];
                vitalSign.RR = fcPatient["RR"];
                db.VitalSigns.Add(vitalSign);

                var laboratory = new Laboratory();
                laboratory.HospitalizationId = hospitalizationId;
                laboratory.Mg = fcPatient["Mg"];
                laboratory.FBS = fcPatient["FBS"];
                laboratory.Hb1AC = fcPatient["Hb1AC"];
                laboratory.BUN = fcPatient["BUN"];
                laboratory.Cr = fcPatient["Cr"];
                laboratory.Na = fcPatient["Na"];
                laboratory.K = fcPatient["K"];
                laboratory.Ca = fcPatient["Ca"];
                laboratory.Chol = fcPatient["Chol"];
                laboratory.LDL = fcPatient["LDL"];
                laboratory.HDL = fcPatient["HDL"];
                laboratory.TG = fcPatient["TG"];
                laboratory.VitOHD3 = fcPatient["VitOHD3"];
                laboratory.CTnI = fcPatient["CTnI"];
                laboratory.CKMB = fcPatient["CKMB"];
                laboratory.AST = fcPatient["AST"];
                laboratory.ALT = fcPatient["ALT"];
                laboratory.ALP = fcPatient["ALP"];
                laboratory.BiL = fcPatient["BiL"];
                laboratory.PSA = fcPatient["PSA"];
                laboratory.TSH = fcPatient["TSH"];
                laboratory.T3 = fcPatient["T3"];
                laboratory.T4 = fcPatient["T4"];
                laboratory.T3RU = fcPatient["T3RU"];
                db.Laboratories.Add(laboratory);                                                
                
                var cardiology = new Cardiology();
                cardiology.HospitalizationId = hospitalizationId;
                cardiology.ECG = fcPatient["ECG"];
                cardiology.EchoCardiography = fcPatient["EchoCardiography"];
                cardiology.Angiography = fcPatient["Angiography"];
                cardiology.EST1 = fcPatient["EST1"];
                cardiology.SPECTMPi = fcPatient["SPECTMPi"];
                cardiology.Diagnosis = fcPatient["Diagnosis"];
                cardiology.Plan = fcPatient["Plan"];
                cardiology.Assistant = fcPatient["Assistant"];
                cardiology.Intern = fcPatient["Intern"];
                cardiology.Trainee = fcPatient["trainee"];
                db.Cardiologys.Add(cardiology);     

                var followUp = new FollowUp();
                followUp.HospitalizationId = hospitalizationId;
                followUp.Description = fcPatient["Description"];
                followUp.Date = Convert.ToDateTime(fcPatient["DateFllowUp"].ToString());
                db.FollowUps.Add(followUp);

                db.SaveChanges();
            }
            catch (Exception)
            {

            }
        }

        // GET: /References/
        public ActionResult Index(int? departmentId, int? patientId)
        {
            ViewBag.PatientId = patientId;
            //definetion
            Session["PatientId"] = patientId;
            var hospitalization = db.Hospitalizations.Include(h => h.Department).Include(h => h.HospDoc).Include(h => h.Patient);
            return View(hospitalization.Where(x => (x.DepartmentId == departmentId) && (x.PatientId == patientId)).ToList());
        }        

        // GET: /References/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hospitalization hospitalization = db.Hospitalizations.Find(id);
            if (hospitalization == null)
            {
                return HttpNotFound();
            }
            return View(hospitalization);
        }

        // GET: /Hospitalization/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name");
            ViewBag.HospDocId = new SelectList(db.HospDocs, "HospDocId", "HospDocId");
            ViewBag.PatientId = new SelectList(db.Patients, "PatientId", "FullName");
            return View();
        }

        // POST: /Hospitalization/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HospitalizationId,DepartmentId,PatientId,DateOfStay,HospDocId")] Hospitalization hospitalization)
        {
            if (ModelState.IsValid)
            {
                db.Hospitalizations.Add(hospitalization);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", hospitalization.DepartmentId);
            ViewBag.HospDocId = new SelectList(db.HospDocs, "HospDocId", "HospDocId", hospitalization.HospDocId);
            ViewBag.PatientId = new SelectList(db.Patients, "PatientId", "FullName", hospitalization.PatientId);
            return View(hospitalization);
        }

        // GET: /Hospitalization/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hospitalization hospitalization = db.Hospitalizations.Find(id);
            if (hospitalization == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepId = hospitalization.DepartmentId;
            ViewBag.PatId = hospitalization.PatientId;

            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", hospitalization.DepartmentId);
            ViewBag.HospDocId = new SelectList(db.HospDocs, "HospDocId", "HospDocId", hospitalization.HospDocId);
            ViewBag.PatientId = new SelectList(db.Patients, "PatientId", "FullName", hospitalization.PatientId);
            return View(hospitalization);
        }

        // POST: /Hospitalization/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HospitalizationId,DepartmentId,PatientId,DateOfStay,HospDocId")] Hospitalization hospitalization)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hospitalization).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new {departmentId = hospitalization.DepartmentId, patientId = hospitalization.PatientId });
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", hospitalization.DepartmentId);
            ViewBag.HospDocId = new SelectList(db.HospDocs, "HospDocId", "HospDocId", hospitalization.HospDocId);
            ViewBag.PatientId = new SelectList(db.Patients, "PatientId", "FullName", hospitalization.PatientId);
            return View(hospitalization);
        }

        // GET: /Hospitalization/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hospitalization hospitalization = db.Hospitalizations.Find(id);
            if (hospitalization == null)
            {
                return HttpNotFound();
            }
            return View(hospitalization);
        }

        // POST: /Hospitalization/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Hospitalization hospitalization = db.Hospitalizations.Find(id);
            db.Hospitalizations.Remove(hospitalization);
            db.SaveChanges();
            return RedirectToAction("Index");
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
