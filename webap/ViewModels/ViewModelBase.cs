
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ica.SalesOrders.Models;
using Ica.SalesOrders.Web.Models;

namespace Ica.SalesOrders.Web.ViewModels
{
    public class ViewModelBase
    {
        private bool _showLeftMenu = true;
        public OperationResult<object> Result { get; set; }
        public ModelStateDictionary ModelState { get; set; }
        public List<Translation> Translations { get; set; }
        public List<Translation> TranslationEN { get; set; }
        
        public void ChangeShowLeftMenu(bool value)
        {
            _showLeftMenu = value;
        }
        public virtual bool ShowLeftMenu
        {
            get
            {
                return _showLeftMenu;
            }
        }
        public List<string> Errors { get; set; }
    }
}