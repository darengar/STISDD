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
    
    public partial class Collegian
    {
        public Collegian()
        {
            this.Hospitalizations = new HashSet<Hospitalization>();
            this.Visits = new HashSet<Visit>();
        }
    
        public int CollegianId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string StudentId { get; set; }
        public string Phone { get; set; }
        public string CellPhone { get; set; }
        public string Address { get; set; }
        public string LoginId { get; set; }
        public string EMail { get; set; }
    
        public virtual ICollection<Hospitalization> Hospitalizations { get; set; }
        public virtual ICollection<Visit> Visits { get; set; }
    }
}
