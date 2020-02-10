using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EMR.Models;
using System.ComponentModel.DataAnnotations;

namespace EMR.Areas.Clinic.ViewModels
{
    public class CardiologyVisit 
    {
        public CardiologyVisit()
        { 
        
        }
        public HistoryDiseas HistoryDiseasVmp { get; set; }
        public Visit VisitVmp { get; set; }
        public ChiefComplaint ChiefComplaintVmp { get; set; }
        public VitalSign VitalSignVmp { get; set; }
        public PhysicalExamination PhysicalExaminationVmp { get; set; }
        public Laboratory LaboratoryVmp { get; set; }
        public Cardiology CardiologyVmp { get; set; }      
        public FollowUp FollowUpVmp { get; set; }

    }

    public class WomanVisit
    {
        public WomanVisit()
        {

        }
        public HistoryDiseas HistoryDiseasVmp { get; set; }
        public Visit VisitVmp { get; set; }
        public ChiefComplaint ChiefComplaintVmp { get; set; }
        public VitalSign VitalSignVmp { get; set; }
        public PhysicalExamination PhysicalExaminationVmp { get; set; }
        public Laboratory LaboratoryVmp { get; set; }
        public Woman WomanVmp { get; set; }
        public FollowUp FollowUpVmp { get; set; }

    }

    public class InternalVisit
    {
        public InternalVisit()
        {

        }
        public HistoryDiseas HistoryDiseasVmp { get; set; }
        public Visit VisitVmp { get; set; }
        public ChiefComplaint ChiefComplaintVmp { get; set; }
        public VitalSign VitalSignVmp { get; set; }
        public PhysicalExamination PhysicalExaminationVmp { get; set; }
        public Laboratory LaboratoryVmp { get; set; }
        public Internal InternalVmp { get; set; }
        public FollowUp FollowUpVmp { get; set; }
    }

    public class LungVisit
    {
        public LungVisit()
        {

        }
        public HistoryDiseas HistoryDiseasVmp { get; set; }
        public Visit VisitVmp { get; set; }
        public ChiefComplaint ChiefComplaintVmp { get; set; }
        public VitalSign VitalSignVmp { get; set; }
        public PhysicalExamination PhysicalExaminationVmp { get; set; }
        public Laboratory LaboratoryVmp { get; set; }
        public Lung LungVmp { get; set; }
        public FollowUp FollowUpVmp { get; set; }
    }

    public class DigestionVisit
    {
        public DigestionVisit()
        {

        }
        public HistoryDiseas HistoryDiseasVmp { get; set; }
        public Visit VisitVmp { get; set; }
        public ChiefComplaint ChiefComplaintVmp { get; set; }
        public VitalSign VitalSignVmp { get; set; }
        public PhysicalExamination PhysicalExaminationVmp { get; set; }
        public Laboratory LaboratoryVmp { get; set; }
        public Digestion DigestionVmp { get; set; }
        public FollowUp FollowUpVmp { get; set; }
    }
}