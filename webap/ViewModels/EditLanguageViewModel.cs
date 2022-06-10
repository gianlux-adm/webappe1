using Ica.SalesOrders.Models;
using Ica.SalesOrders.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ica.SalesOrders.Web.ViewModels
{
    public class EditLanguageViewModel : ViewModelBase
    {
        private EditLanguageViewModel()
        {

        }
        public Language Language { get; set; }
        public string Mode { get; set; }

        public static EditLanguageViewModel Create(Language language, OperationResult<object> result,string mode)
        {
            EditLanguageViewModel eltvm = new EditLanguageViewModel();
            eltvm.Result = result;
            eltvm.Language = language;
            eltvm.Mode = mode;
            return eltvm;
        }


        
    }
}