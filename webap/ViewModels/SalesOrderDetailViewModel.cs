using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ica.SalesOrders.Models;
using Ica.SalesOrders.Web.Models;

namespace Ica.SalesOrders.Web.ViewModels
{
    public class SalesOrderDetailViewModel : ViewModelBase
    {
        public SalesOrderDetailViewModel()
        {
            Rows = new List<GetSalesV4MasterRow>();
        }
        public int OrderNumber { get; internal set; }
        public string OrderType { get; internal set; }
        public string OrderCompany { get; internal set; }
        public decimal OrderLineNumber { get; internal set; }
        public string Plant { get; internal set; }
        public int SoldTo { get; internal set; }
        public string ShipTo { get; internal set; }
        public DateTime OrderDate { get; internal set; }
        public string CustomerPO { get; internal set; }
        public string Currency { get; internal set; }
        public decimal ExchangeRate { get; internal set; }
        public List<WsMappingTable> Fields { get; internal set; }
        public List<GetSalesV4MasterRow> Rows { get; internal set; }
    }
}