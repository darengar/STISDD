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
    
    public partial class Clinic
    {
        public Clinic()
        {
            this.Admissions = new HashSet<Admission>();
        }
    
        public int ClinicId { get; set; }
        public Nullable<int> HospitalId { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<Admission> Admissions { get; set; }
        public virtual Hospital Hospital { get; set; }
    }
}
