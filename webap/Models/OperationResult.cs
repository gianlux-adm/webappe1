using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ica.SalesOrders.Web.Models
{
    public class OperationResult<T>
    {
        public OperationResult()
        {
            Result = true;
            Warning = false;
            Message = String.Empty;
        }
        public bool Result { get; set; }
        public String Message { get; set; }
        public T Data { get; set; }

        public bool Warning { get; set; }
    }
}