using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ica.SalesOrders.Web.Models
{
    public enum LogActionEnum
    {
        Exception = 1,
        
        WarehouseAutocomplete = 4,
        WarehouseDDL = 5,
        WarehouseSearch = 6,
        EditLanguage = 7,
        EditTranslation = 8,
        EditUser = 9,
        WarehouseGetVersion = 10,
        JdeAuthentication = 11
    }
}