using System.Web;
using System.Web.Optimization;

namespace STISDD
{
    public class BundleConfig
    {        
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/bootstrap-hover-dropdown.js"));

            bundles.Add(new ScriptBundle("~/bundles/site").Include(
                        "~/Scripts/Site.js"));

            bundles.Add(new ScriptBundle("~/bundles/gmap").Include(
                       "~/Scripts/jquery.gmap.js",
                       "~/Scripts/jquery.gmap_init.js"));

            bundles.Add(new ScriptBundle("~/bundles/modalform").Include(
                        "~/Scripts/modalform.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                       "~/Content/bootstrap.css",
                       "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/menu").Include(
                       "~/Content/Menu/styles.css"));

            bundles.Add(new ScriptBundle("~/bundles/menu").Include(
                       "~/Scripts/jquery-latest.min.js"));

            bundles.Add(new StyleBundle("~/Content/Home_HoverEffect").Include(
                       "~/Content/Home_HoverEffect/demo.css",
                       "~/Content/Home_HoverEffect/normalize.css",
                       "~/Content/Home_HoverEffect/set1.css"));

            //bundles.Add(new StyleBundle("~/Content/OrginalHoverEffect").Include(
            //           "~/Content/OrginalHoverEffect/demo.css",
            //           "~/Content/OrginalHoverEffect/style_common.css",
            //           "~/Content/OrginalHoverEffect/style1.css"));

            BundleTable.EnableOptimizations = false;
        }
    }
}
