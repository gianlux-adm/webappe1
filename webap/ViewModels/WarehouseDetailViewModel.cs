using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ica.SalesOrders.Web.Models;

namespace Ica.SalesOrders.Web.ViewModels
{
    public class WarehouseDetailViewModel : ViewModelBase
    {
        public WarehouseDetailViewModel()
        {
            Articoli = new List<Articolo>();
            MCU = "60";
            ValoriMCU = new Dictionary<string, string>();
            ValoriMCU.Add("", "DEFAULT");
            ValoriMCU.Add("60", "TUTTI");
            ValoriAvailability = new Dictionary<string, string>();
            ValoriAvailability.Add("", "TUTTI");
            ValoriAvailability.Add("Y", "DISPONIBILI");
            ValoriAvailability.Add("N", "NON DISPONIBILI");
            DetailAvailability = "Y";
        }
        public List<Articolo> Articoli { get; set; }
        public String MCU { get; set; }
        public string DetailAvailability { get; set; }
        public Dictionary<string, string> ValoriMCU { get; set; }
        public Dictionary<string, string> ValoriAvailability { get; set; }
    }
}