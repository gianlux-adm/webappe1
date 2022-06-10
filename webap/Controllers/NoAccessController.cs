//using Ica.SalesOrders.Web.Auth;
using Ica.SalesOrders.Web.ViewModels;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Ica.SalesOrders.Web.Controllers
{
    public class NoAccessController : BaseController
    {
        // GET: NoAccess
        public ActionResult Index()
        {
            NoAccessViewModel vm = new NoAccessViewModel();
            vm.Username = String.Empty;
            if (Request.IsAuthenticated)
            {
                vm.Username = User.Identity.Name;
            }
            
            return View(vm);
        }
    }
}