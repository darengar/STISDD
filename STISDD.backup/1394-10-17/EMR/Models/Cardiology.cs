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
    
    public partial class Cardiology
    {
        public int CardiologyId { get; set; }
        public Nullable<int> VisitId { get; set; }
        public Nullable<System.DateTime> LeadTime { get; set; }
        public string ECG { get; set; }
        public string Echocardiography { get; set; }
        public string Angiography { get; set; }
        public string PCI { get; set; }
        public string EST { get; set; }
        public string SPECTMPI { get; set; }
        public string Diagnosis { get; set; }
        public string Plan { get; set; }
        public string Assistant { get; set; }
        public string Intern { get; set; }
        public string Trainee { get; set; }
    
        public virtual Visit Visit { get; set; }
    }
}
