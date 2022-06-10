using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ica.SalesOrders.Web.ViewModels.Sales
{
    public class SalesIndexViewModel : ViewModelBase
    {
        public SalesIndexViewModel()
        {
          
        }
        public String CodiceArticolo { get; set; }
        public String CodiceCliente { get; set; }
        public String NumeroOrdineCliente { get; set; }
        public String Descrizione { get; set; }
        public string VersioneForm { get; set; }
    }
}