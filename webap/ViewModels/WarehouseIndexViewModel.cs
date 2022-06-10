using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ica.SalesOrders.Web.ViewModels
{
    public class WarehouseIndexViewModel : ViewModelBase
    {
        public WarehouseIndexViewModel()
        {
            ValoriCategoria1 = new Dictionary<string, string>();
            ValoriCategoria2 = new Dictionary<string, string>();
            ValoriCategoria3 = new Dictionary<string, string>();
            ValoriCategoria4 = new Dictionary<string, string>();
            ValoriMCU = new Dictionary<string, string>();
            ValoriMCU.Add("", "DEFAULT");
            ValoriMCU.Add("60", "TUTTI");
            ValoriAvailability = new Dictionary<string, string>();
            ValoriAvailability.Add("", "TUTTI");
            ValoriAvailability.Add("Y", "DISPONIBILI");
            ValoriAvailability.Add("N", "NON DISPONIBILI");
            MCU = String.Empty;
            Availability = "Y";
        }
        public String CodiceArticolo { get; set; }
        public String Descrizione { get; set; }
        public String Categoria1 { get; set; }
        public String Categoria2 { get; set; }
        public String Categoria3 { get; set; }
        public String Categoria4 { get; set; }
        public String MCU { get; set; }
        public String Availability { get; set; }

        public Dictionary<String,String> ValoriCategoria1 { get; set; }
        public Dictionary<String, String> ValoriCategoria2 { get; set; }
        public Dictionary<String, String> ValoriCategoria3 { get; set; }
        public Dictionary<String, String> ValoriCategoria4 { get; set; }
        public Dictionary<String, String> ValoriMCU { get; set; }
        public Dictionary<String, String> ValoriAvailability { get; set; }
        public string VersioneForm { get; internal set; }
    }
}