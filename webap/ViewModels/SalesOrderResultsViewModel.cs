using Ica.SalesOrders.Models;
using Ica.SalesOrders.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ica.SalesOrders.Web.ViewModels
{
    public class SalesOrderResultsViewModel : ViewModelBase
    {
        public SalesOrderResultsViewModel()
        {
            Rows = new List<GetSalesV4MasterRow>();
        }
        public List<GetSalesV4MasterRow> Rows { get; set; }
        public List<WsMappingTable> Fields { get; internal set; }
    }
}