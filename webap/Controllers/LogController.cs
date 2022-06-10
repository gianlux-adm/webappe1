using Ica.SalesOrders.Models;
using Ica.SalesOrders.Web.Models.DataTables;
using Ica.SalesOrders.Web.ViewModels;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Ica.SalesOrders.Web.Controllers
{
    public class LogController : BaseController
    {
        
        public LogController()
        {
        
        }


        public ActionResult User(String query)
        {
            var list = Context.Users.Where(w => w.LoginName.StartsWith(query)
            || w.Name.Contains(query)
            ).OrderBy(o => o.LoginName).Take(10).ToList();
            return Json(list.Select(s => new { s.UserId, s.LoginName, s.Role.RoleDescription }), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Index()
        {
            LogViewModel lvm = new LogViewModel();
            lvm.LogAreas = Context.LogArea.OrderBy(o => o.LogAreaName).ToList();
            lvm.LogActions = new List<LogAction>();
            return View(lvm);
        }


        [HttpPost]
        public ActionResult Download(String dateStart, String dateEnd, short? logArea, int? logAction, string itemIdTypeValue, string secondItemIdTypeValue, int? author)
        {
            String countries = String.Empty;
            DateTime? dateStartValue = null;
            DateTime? dateEndValue = null;

            String format = "dd/MM/yyyy HH':'mm";
            if (!String.IsNullOrEmpty(dateStart))
            {
                dateStartValue = DateTime.ParseExact(dateStart, format, CultureInfo.InvariantCulture);
            }
            else
            {
                dateStartValue = new DateTime(2001, 1, 1);
            }
            if (!String.IsNullOrEmpty(dateEnd))
            {
                dateEndValue = DateTime.ParseExact(dateEnd, format, CultureInfo.InvariantCulture);
            }
            else
            {
                dateEndValue = DateTime.Now;
            }

            if (dateStartValue > dateEndValue)
            {
                return new HttpStatusCodeResult(500, Translate("Logs", "DateEndMustBeGreaterThanStart"));
            }


            int from = 0;
            int to = int.MaxValue;
            ApplicationUser currentUser = GetCurrentUser();
            int totalRecords = 0;
            List<LogEntry> logs = Context.GetLogs(dateStartValue, dateEndValue, logAction, logArea, itemIdTypeValue, secondItemIdTypeValue, from, to, countries, out totalRecords, GetCurrentUserId(), author,1);

            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/Logs.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource("LogsDS", logs);
            localReport.DataSources.Add(reportDataSource);
            /*
           List<ReportParameter> repParameters = new List<ReportParameter>() {                
           new ReportParameter("DataInizio", from.Value.ToString("dd/MM/yyyy")),
           new ReportParameter("DataFine", to.Value.ToString("dd/MM/yyyy")) 
       };
           localReport.SetParameters(repParameters);
             */
            string tipo = "excel";
            string reportType = "";
            string deviceInfo = "";
            switch (tipo.ToUpper())
            {
                case "PDF":
                    reportType = "PDF";
                    deviceInfo =
 "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>11.69in</PageWidth>" +
                "  <PageHeight>8.77in</PageHeight>" +
                "  <MarginTop>0.25in</MarginTop>" +
                "  <MarginLeft>1in</MarginLeft>" +
                "  <MarginRight>1in</MarginRight>" +
                "  <MarginBottom>0.25in</MarginBottom>" +
                "</DeviceInfo>";
                    break;

                default:
                    reportType = "EXCEL";
                    deviceInfo =
                    "<DeviceInfo>" +
                    " <SimplePageHeaders>False</SimplePageHeaders>" +
                    "</DeviceInfo>";

                    //contentType = "application/vnd.ms-excel";
                    break;
            }

            string mimeType;
            string encoding;
            string fileNameExtension;

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;
            //Render the report            
            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("Logs{0:yyyyMMdd}.{1}", DateTime.Now, fileNameExtension));
            return File(renderedBytes, mimeType);
        }

        [HttpPost]
        public ActionResult List(String dateStart, String dateEnd, short? logArea, int? logAction, string itemIdTypeValue, string secondItemIdTypeValue, DataTableOption option, int? author)
        {
            int sortCol = option.iSortCols[0];
            DataTablesSortDirection sortDirection = option.sSortDirs[0];
            if (sortDirection == Models.DataTables.DataTablesSortDirection.Descending)
            {
                sortCol += 1000;
            }

            String countries = String.Empty;
            DateTime? dateStartValue = null;
            DateTime? dateEndValue = null;

            String format = "dd/MM/yyyy HH':'mm";
            if (!String.IsNullOrEmpty(dateStart))
            {
                dateStartValue = DateTime.ParseExact(dateStart, format, CultureInfo.InvariantCulture);
            }
            else
            {
                dateStartValue = new DateTime(2001, 1, 1);
            }
            if (!String.IsNullOrEmpty(dateEnd))
            {
                dateEndValue = DateTime.ParseExact(dateEnd, format, CultureInfo.InvariantCulture);
            }
            else
            {
                dateEndValue = DateTime.Now;
            }

            if (dateStartValue > dateEndValue)
            {
                return new HttpStatusCodeResult(500, Translate("Logs","DateEndMustBeGreaterThanStart"));
            }


            int from = option.iDisplayStart + 1;
            int to = option.iDisplayStart + option.iDisplayLength;
            ApplicationUser currentUser = GetCurrentUser();
            int totalRecords = 0;
            List<LogEntry> logs = Context.GetLogs(dateStartValue, dateEndValue, logAction, logArea, itemIdTypeValue, secondItemIdTypeValue, from, to, countries, out totalRecords, GetCurrentUserId(), author,sortCol);
            
            var result = new DataTableResultExt(option.sEcho,
                totalRecords, totalRecords,
                logs.Select(s => s.ToDataTablesRow()).ToList()
                );

            result.ContentEncoding = Encoding.UTF8;
            return result;
        }

        [HttpGet]
        public ActionResult Actions(short areaID)
        {
            List<LogAction> actions = Context.LogActions.Where(w => w.LogAreaID.Equals(areaID)).ToList();

            
            return Json(actions.Select(s=> new {
                s.ItemIdType,
                s.LogActionID,
                s.LogActionName,
                s.LogAreaID,
                s.SecondItemIdType,
                Label = String.IsNullOrEmpty(s.ItemIdType) ? String.Empty : Translate("Logs",s.ItemIdType),
                SecondLabel = String.IsNullOrEmpty(s.SecondItemIdType) ? String.Empty : Translate("Logs", s.SecondItemIdType)
            }), JsonRequestBehavior.AllowGet);
        }

    }
}