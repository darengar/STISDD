using System.Web.Mvc;

namespace EMR.Areas.OutPatient
{
    public class OutPatientAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "OutPatient";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "OutPatient_default",
                "OutPatient/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "EMR.Areas.OutPatient.Controllers" }
            );
        }
    }
}