using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ica.SalesOrders.Models;
using Ica.SalesOrders.Models.Context;
using Ica.SalesOrders.Web.Models;
namespace Ica.SalesOrders.Web.ViewModels
{
    public class EditUserViewModel : ViewModelBase
    {
        private EditUserViewModel()
        {

        }
        public ApplicationUser User { get; set; }
        public List<ApplicationRole> Roles { get; set; }
        public string Mode { get; set; }
        public Dictionary<bool,string> Status { get; set; }

        


        public static EditUserViewModel Load(AppContext ctx, ApplicationUser u)
        {
            EditUserViewModel euvm = new EditUserViewModel();
            if (u == null)
            {
                u = new ApplicationUser();
                u.Enabled = true;
            }
            euvm.User = u;
           
                euvm.Roles = ctx.Roles.OrderBy(o => o.RoleDescription).ToList();
            
            euvm.Status = new Dictionary<bool, string>();
            euvm.Status.Add(true, "Enabled");
            euvm.Status.Add(false, "Disabled");

            return euvm;
        }
    }
}