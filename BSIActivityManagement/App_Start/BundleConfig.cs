using System.Web;
using System.Web.Optimization;

namespace BSIActivityManagement
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new Bundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new Bundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new Bundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new Bundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/agency.min.css",
                      "~/Content/site.css","~/Content/font-awesome.min.css"));
            bundles.Add(new Bundle("~/Content/persiancss").Include(
                "~/Content/bootstrap.css",
                "~/Content/agency.min.css",
                "~/Content/site.css",
                "~/Content/font-awesome.min.css",
                "~/Content/bootstrap-flipped.min.css", 
                "~/Content/bootstrap-rtl.min.css", 
                "~/Content/PersianLanguage.css"));
            bundles.Add(new Bundle("~/Content/agencycss").Include(
                "~/Content/agencysite.css",
                "~/Content/simple-sidebar.css"
                ));
            bundles.Add(new Bundle("~/Content/SideBar").Include(
                "~/Content/reset.css",
                "~/Content/style.css"
                ));
            bundles.Add(new Bundle("~/Scripts/SideBar").Include(
                "~/Scripts/jquery.menu-aim.js",
                "~/Scripts/main.js"
                ));
            //System.Web.Optimization.BundleTable.EnableOptimizations = false;
        }
    }
}
