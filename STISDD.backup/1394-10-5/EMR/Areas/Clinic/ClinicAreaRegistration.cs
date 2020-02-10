﻿using System.Web.Mvc;

namespace EMR.Areas.Clinic
{
    public class ClinicAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Clinic";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Clinic_default",
                "Clinic/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "EMR.Areas.Clinic.Controllers" }
            );
        }
    }
}