using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ica.SalesOrders.Web.ViewModels
{
    public class GenericDeleteModalViewModel : ViewModelBase
    {
        public bool CanDelete { get; set; }
        public String DeleteMessage { get; set; }
        public object Key { get; set; }
        public string Title { get; set; }
        public string PostAction { get; set; }

        public string PostController { get; set; }

        public string AdditionalView { get; set; }

        public object AdditionaViewModel { get; set; }
    }
}