using System.Web;
using System.Web.Optimization;

namespace SinavSistemi
{
    public class BundleConfig
    {
        // Paketleme hakkında daha fazla bilgi için lütfen https://go.microsoft.com/fwlink/?LinkId=301862 adresini ziyaret edin
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-3.4.1.js",
                        //"~/Scripts/jquery-3.4.1.min.js",
                        //"~/Scripts/jquery-3.4.1.slim.js",
                        "~/Scripts/jquery.countdown.js",
                        "~/Scripts/jquery.countdown.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Geliştirme yapmak ve öğrenmek için Modernizr'ın geliştirme sürümünü kullanın. Daha sonra,
            // üretim için hazır. https://modernizr.com adresinde derleme aracını kullanarak yalnızca ihtiyacınız olan testleri seçin.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/toastr.js",
                      "~/Scripts/toastr.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap.min.css",
                      "~/Content/style.css",
                      "~/Content/toastr.css",
                      "~/Content/toastr.min.css",
                      "~/Content/site.css"));


            BundleTable.EnableOptimizations = true;
        }
    }
}
