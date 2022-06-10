using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Ica.SalesOrders.Models;
namespace Ica.SalesOrders.Web.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        [Required]
        public String Username { get; set; }
        [Required]
        public String Password { get; set; }
        [Required]
        public String Language { get; set; }

        public List<Language> Languages { get; set; }
        public String ReturnUrl { get; set; }
    }
}