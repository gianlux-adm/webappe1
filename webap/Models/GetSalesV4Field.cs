using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ica.SalesOrders.Web.Models
{
    public class GetSalesV4Field
    {
        public string Name { get; internal set; }
        public int Priority { get; internal set; }
        public string Value { get; internal set; }
        public int Width { get; internal set; }
        public string HtmlClass { get; internal set; }
    }
}