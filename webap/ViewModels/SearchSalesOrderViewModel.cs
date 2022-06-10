using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ica.SalesOrders.Web.ViewModels
{
    public class SearchSalesOrderViewModel : ViewModelBase
    {
        public String BusinessUnit { get; set; }
        public int? SoldTo { get; set; }
        public string DocumentType { get; set; }
    }
}