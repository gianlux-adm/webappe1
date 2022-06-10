using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ica.SalesOrders.Web.Models
{
    public class SaleResult
    {
        public DateTime ShippingDateExpected { get; internal set; }
        public string Recipient { get; internal set; }
        public string ArticleCode { get; internal set; }
        public decimal Quantity { get; internal set; }
        public string UnitOfMeasure { get; internal set; }
        public string SaleOrderStatusColor { get; internal set; }
        public string SaleOrderStatus { get; internal set; }
        public string ProductionOrderColor { get; internal set; }
        public string ProductionOrderStatus { get; internal set; }
        public string TransferOrderStatus { get; internal set; }
        public string TransferOrderColor { get; internal set; }
        public int RowIndex { get; internal set; }
    }
}