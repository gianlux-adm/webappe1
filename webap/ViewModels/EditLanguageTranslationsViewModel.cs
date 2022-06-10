using Ica.SalesOrders.Models;
using Ica.SalesOrders.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ica.SalesOrders.Web.ViewModels
{
    public class EditLanguageTranslationsViewModel : ViewModelBase
    {
        private EditLanguageTranslationsViewModel()
        {

        }
        public Language Language { get; set; }
        public List<String> Pages { get; set; }
        public List<Translation> Entries { get; set; }
        public List<Translation> EntriesEn { get; set; }
        public String SelectedPage { get; set; }
        public static EditLanguageTranslationsViewModel Create(Language language,
            List<String> pages,
            List<Translation> entries,
            List<Translation> entriesEn,
            OperationResult<object> res)
        {
            EditLanguageTranslationsViewModel eltvm = new EditLanguageTranslationsViewModel();
            eltvm.Language = language;
            eltvm.Pages = pages;
            eltvm.Entries = entries;
            eltvm.EntriesEn = entriesEn;
            eltvm.Result = res;
            return eltvm;
        }
    }
}