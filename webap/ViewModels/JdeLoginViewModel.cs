using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ica.SalesOrders.Models;
namespace Ica.SalesOrders.Web.ViewModels
{
    public class JdeLoginViewModel : ViewModelBase
    {
        public String ReturnUrl { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public int IdFunction { get; set; }
        public Function Function { get; set; }
    }
}