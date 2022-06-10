using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ica.SalesOrders.Models;
namespace Ica.SalesOrders.Web.ViewModels
{
    public class ListLanguageViewModel : ViewModelBase
    {
        public List<Language> Languages { get; set; }
    }
}