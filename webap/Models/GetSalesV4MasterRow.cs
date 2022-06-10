using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ica.SalesOrders.Web.Models
{
    public class GetSalesV4MasterRow
    {
        public GetSalesV4MasterRow()
        {
            Fields = new List<GetSalesV4Field>();
        }
        public List<GetSalesV4Field> Fields { get; set; }

        public string RowId
        {
            get {
                return Fields[0].Value + "|" + Fields[1].Value + "|" + Fields[2].Value + "|" + Fields[3].Value;
            }
        }
    }
}