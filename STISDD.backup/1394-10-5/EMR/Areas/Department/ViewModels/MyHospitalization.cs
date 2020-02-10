using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EMR.Models;

namespace EMR.Areas.Department.ViewModels
{
    public class MyHospitalization
    {
        public int VisitId { get; set; }

        public string FullName { get; set; }

        public int DoctorId { get; set; }

        public int CollegianId { get; set; }

        //public DateTime DateOfStay { get; set; }

        //public string NationalId { get; set; }
    }
}