using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ica.SalesOrders.Web.SchedeService;

namespace Ica.SalesOrders.Web.ViewModels.Sales
{
    public class DettaglioSchedeViewModel : ViewModelBase
    {
        public String CodiceArticolo { get; set; }
        public List<Scheda> Schede { get; set; }
    }
}