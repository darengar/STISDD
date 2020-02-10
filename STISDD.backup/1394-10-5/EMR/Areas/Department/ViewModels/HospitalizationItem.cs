using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EMR.Models;
using System.ComponentModel.DataAnnotations;

namespace EMR.Areas.Department.ViewModels
{
    public class CardiologyHospitalization
    {
        public CardiologyHospitalization()
        { 
        
        }
        public HistoryDiseas HistoryDiseasVmp { get; set; }
        public Hospitalization HospitalizationVmp { get; set; }
        public ChiefComplaint ChiefComplaintVmp { get; set; }
        public VitalSign VitalSignVmp { get; set; }
        public PhysicalExamination PhysicalExaminationVmp { get; set; }
        public Laboratory LaboratoryVmp { get; set; }
        public Cardiology CardiologyVmp { get; set; }      
        public FollowUp FollowUpVmp { get; set; }

    }

    public class WomanHospitalization
    {
        public WomanHospitalization()
        {

        }
        public HistoryDiseas HistoryDiseasVmp { get; set; }
        public Hospitalization HospitalizationVmp { get; set; }
        public ChiefComplaint ChiefComplaintVmp { get; set; }
        public VitalSign VitalSignVmp { get; set; }
        public PhysicalExamination PhysicalExaminationVmp { get; set; }
        public Laboratory LaboratoryVmp { get; set; }
        public Woman WomanVmp { get; set; }
        public FollowUp FollowUpVmp { get; set; }

    }

    public class InternalHospitalization
    {
        public InternalHospitalization()
        {

        }
        public HistoryDiseas HistoryDiseasVmp { get; set; }
        public Hospitalization HospitalizationVmp { get; set; }
        public ChiefComplaint ChiefComplaintVmp { get; set; }
        public VitalSign VitalSignVmp { get; set; }
        public PhysicalExamination PhysicalExaminationVmp { get; set; }
        public Laboratory LaboratoryVmp { get; set; }
        public Internal InternalVmp { get; set; }
        public FollowUp FollowUpVmp { get; set; }
    }

    public class LungHospitalization
    {
        public LungHospitalization()
        {

        }
        public HistoryDiseas HistoryDiseasVmp { get; set; }
        public Hospitalization HospitalizationVmp { get; set; }
        public ChiefComplaint ChiefComplaintVmp { get; set; }
        public VitalSign VitalSignVmp { get; set; }
        public PhysicalExamination PhysicalExaminationVmp { get; set; }
        public Laboratory LaboratoryVmp { get; set; }
        public Lung LungVmp { get; set; }
        public FollowUp FollowUpVmp { get; set; }
    }

    public class DigestionHospitalization
    {
        public DigestionHospitalization()
        {

        }
        public HistoryDiseas HistoryDiseasVmp { get; set; }
        public Hospitalization HospitalizationVmp { get; set; }
        public ChiefComplaint ChiefComplaintVmp { get; set; }
        public VitalSign VitalSignVmp { get; set; }
        public PhysicalExamination PhysicalExaminationVmp { get; set; }
        public Laboratory LaboratoryVmp { get; set; }
        public Digestion DigestionVmp { get; set; }
        public FollowUp FollowUpVmp { get; set; }
    }
}