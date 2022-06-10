using Ica.SalesOrders.Models;
using Ica.SalesOrders.Web.Models;
using Ica.SalesOrders.Web.Models.DataTables;
using Ica.SalesOrders.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Ica.SalesOrders.Web.Controllers
{
    
    public class HomeController : BaseController
    {
        
        // GET: /Home/
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                
                if (User.IsInRole("NoAccess"))
                {
                    return RedirectToAction("Index", "NoAccess");
                }
                return RedirectToAction("Index", "Sales");
            }
            else
            {
                return RedirectToAction("LoginAdfs", "Account");
            }
        }



    }
}