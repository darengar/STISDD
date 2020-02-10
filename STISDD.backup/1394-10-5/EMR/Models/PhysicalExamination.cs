//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EMR.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PhysicalExamination
    {
        public int PhysicalExaminationId { get; set; }
        public Nullable<int> VisitId { get; set; }
        public Nullable<int> HospitalizationId { get; set; }
        public Nullable<System.DateTime> LeadTime { get; set; }
        public string Heart { get; set; }
        public string Lung { get; set; }
        public string Extremities { get; set; }
        public string ETC { get; set; }
    
        public virtual Hospitalization Hospitalization { get; set; }
        public virtual Visit Visit { get; set; }
    }
}
