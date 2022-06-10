using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Optimization;

namespace Ica.SalesOrders.Web.App_Start
{
    public class BundleConfig
    {
        
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/AdminLteBundle").Include(
                      "~/Content/AdminLte/bootstrap/css/bootstrap.min.css",
                      "~/Content/AdminLte/css/font-awesome.min.css",
                      "~/Content/AdminLte/css/ionicons.min.css",
                      "~/Content/AdminLte/css/AdminLTE.min.css",
                      "~/Content/AdminLte/css/skins/_all-skins.min.css",
                      "~/Content/Site.css",
                      "~/Scripts/AdminLte/plugins/datatables/dataTables.bootstrap.css",
                      "~/Content/AdminLte/jQueryUi/jquery-ui.css",
                      "~/Content/bootstrap-switch/bootstrap-switch.css",
                      "~/Content/boostrap-duallistbox/bootstrap-duallistbox.css",
                      "~/Scripts/AdminLte/plugins/timepicker/bootstrap-timepicker.css",
                      "~/Scripts/bootstrap-datetimepicker.css",
                      "~/Scripts/AdminLte/plugins/select2/select2.css"
                      ).WithLastModifiedToken());

            bundles.Add(new ScriptBundle("~/Scripts/AdminLteScripts").Include(
                "~/Scripts/AdminLte/plugins/jQuery/jQuery-2.1.4.min.js",
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js",
                "~/Scripts/jquery.unobtrusive-ajax.min.js",
                "~/Scripts/AdminLte/plugins/jQueryUI/jquery-ui.min.js",
                "~/Scripts/AdminLte/noconflict.js",
                "~/Scripts/AdminLte/bootstrap/bootstrap.min.js",
                "~/Scripts/AdminLte/app.min.js",
                "~/Scripts/AdminLte/plugins/fullcalendar/moment.min.js",
                "~/Scripts/AdminLte/plugins/fullcalendar/fullcalendar.js",
                "~/Scripts/bootstrap.datetimepicker.js",
                "~/Scripts/bootstrap-switch/bootstrap-switch.js",
                "~/Scripts/boostrap-duallistbox/jquery.bootstrap-duallistbox.js",
                "~/Scripts/AdminLte/plugins/timepicker/bootstrap-timepicker.js",
                "~/Scripts/jquery.typeahead.js",
                "~/Scripts/AdminLte/plugins/datatables/jquery.dataTables.js",
                "~/Scripts/AdminLte/plugins/datatables/dataTables.bootstrap.js",
                "~/Scripts/AdminLte/plugins/datatables/extensions/ColumnFilter/jquery.dataTables.columnFilter.js",
                //"~/Scripts/AdminLte/plugins/datatables/extensions/ColVis/js/dataTables.colVis.js",
                "~/Scripts/custom.js",
                "~/Scripts/bootstrap-fileinput/js/fileinput.js",
                "~/Scripts/AdminLte/plugins/select2/select2.js"
            ).WithLastModifiedToken());
        }
         
    }


    public static class BundleExtensions
    {
        public static Bundle WithLastModifiedToken(this Bundle sb)
        {
            sb.Transforms.Add(new LastModifiedBundleTransform());
            return sb;
        }
    }
    public class LastModifiedBundleTransform : IBundleTransform
    {
        public void Process(BundleContext context, BundleResponse response)
        {
            foreach (var file in response.Files)
            {
                var lastWrite = File.GetLastWriteTime(HostingEnvironment.MapPath(file.IncludedVirtualPath)).Ticks.ToString();
                file.IncludedVirtualPath = string.Concat(file.IncludedVirtualPath, "?v=", lastWrite);
            }
        }
    }

}