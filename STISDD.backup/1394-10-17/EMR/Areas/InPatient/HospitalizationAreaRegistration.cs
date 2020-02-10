using System.Web.Mvc;

namespace EMR.Areas.InPatient
{
    public class InPatientAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "InPatient";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Department_default",
                "InPatient/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "EMR.Areas.InPatient.Controllers" }
            );
        }
    }
}