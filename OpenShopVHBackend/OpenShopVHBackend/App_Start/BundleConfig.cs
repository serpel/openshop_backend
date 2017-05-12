#region Copyright Syncfusion Inc. 2001 - 2017
// Copyright Syncfusion Inc. 2001 - 2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Web;
using System.Web.Optimization;

namespace OpenShopVHBackend
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();

            bundles.Add(new ScriptBundle("~/bundles/javascripts").Include(
              "~/Content/assets/plugins/jquery-1.8.3.min.js",
              "~/Content/assets/plugins/jquery-ui/jquery-ui-1.10.1.custom.min.js",
              "~/Content/assets/plugins/boostrapv3/js/bootstrap.min.js",
              "~/Content/assets/plugins/breakpoints.js",
              "~/Content/assets/plugins/jquery-unveil/jquery.unveil.min.js",
              "~/Content/assets/plugins/jquery-block-ui/jqueryblockui.min.js",
              "~/Content/assets/plugins/jquery-scrollbar/jquery.scrollbar.min.js",
              "~/Content/assets/plugins/jquery-numberAnimate/jquery.animateNumbers.js",
              "~/Content/assets/plugins/jquery-notifications/js/messenger.min.js",
              "~/Content/assets/plugins/jquery-notifications/js/messenger-theme-future.min.js",
              "~/Content/assets/plugins/bootstrap-tag/bootstrap-tagsinput.min.js",
              "~/Content/assets/js/core.js",
              "~/Content/assets/js/demo.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
               "~/Content/assets/plugins/bootstrap-tag/bootstrap-tagsinput.css",
               "~/Content/assets/plugins/bootstrap-select2/select2.css",
               "~/Content/assets/plugins/boostrapv3/css/bootstrap.min.css",
               "~/Content/assets/plugins/boostrapv3/css/bootstrap-theme.min.css",
               "~/Content/assets/plugins/font-awesome/css/font-awesome.css",
               "~/Content/assets/css/animate.min.css",
               "~/Content/assets/plugins/jquery-scrollbar/jquery.scrollbar.css",
               "~/Content/assets/plugins/jquery-notifications/css/messenger.min.css",
               "~/Content/assets/plugins/jquery-notifications/css/messenger-theme-future.min.css",
               "~/Content/assets/plugins/jquery-notifications/css/location-sel.min.css",
               "~/Content/assets/css/style.css",
               "~/Content/assets/css/responsive.css",
               "~/Content/assets/css/custom-icon-set.css"));

            BundleTable.EnableOptimizations = false;

            bundles.Add(new ScriptBundle("~/bundles/ejscripts").Include(
                           "~/Scripts/jsrender.min.js",
                           "~/Scripts/jquery.easing-1.3.min.js",
                            "~/Scripts/ej/ej.web.all.min.js",
                            "~/Scripts/ej/ej.unobtrusive.min.js"));
            bundles.Add(new StyleBundle("~/bundles/ejstyles").Include(
                      "~/ejThemes/flat-saffron/ej.web.all.min.css"));
        }
    }
}
