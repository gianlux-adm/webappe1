using Ica.SalesOrders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ica.SalesOrders.Web.ViewModels
{
    public class LogViewModel : ViewModelBase
    {
        public int LogArea { get; set; }
        public int? LogAction { get; set; }
        public string itemIdTypeValue { get; set; }
        public string secondItemIdTypeValue { get; set; }
        public String Filter { get; set; }
        public List<LogArea> LogAreas { get; set; }
        public List<LogAction> LogActions { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public List<LogEntry> Logs { get; set; }
    }
}