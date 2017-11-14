using System.Web;
using System.Web.Optimization;
using System.Web.Optimization.HashCache;

namespace DIProject
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Content/Scripts/jquery-{version}.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval")
                .Include("~/Content/Scripts/jquery.validate*"));

            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/site.css", "~/Content/loading.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/script").Include(
                "~/Content/Scripts/app.js", "~/Content/Scripts/loading.min.js"));

            BundleTable.EnableOptimizations = true;
            BundleTable.Bundles.ApplyHashCache();
        }
    }
}
