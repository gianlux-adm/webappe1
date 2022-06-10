using Ica.SalesOrders.Models;
using Ica.SalesOrders.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ica.SalesOrders.Web.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public HomeViewModel()
        {
        }

        public override bool ShowLeftMenu
        {
            get
            {
                return false;
            }
        }


        internal static HomeViewModel Load(AppContext ctx,int userId)
        {
            HomeViewModel vm = new HomeViewModel();
            ApplicationUser currentUser = ctx.Users.Find(userId);
            return vm;
        }
    }
}