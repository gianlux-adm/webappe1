using Ica.SalesOrders.Models;
using Ica.SalesOrders.Models.Context;
using Ica.SalesOrders.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Ica.SalesOrders.Web.Helpers
{
    public static class TranslationsHelper
    {
        public static MvcHtmlString InfoToolTip(this HtmlHelper helper, String page, String key) 
        {
            String text = helper.Translate(page,key);
            if (String.IsNullOrEmpty(text)) {
                return MvcHtmlString.Create(String.Empty);
            }
            TagBuilder builder = new TagBuilder("i");
            builder.AddCssClass("fa fa-info-circle");
            builder.Attributes.Add("style","color:red;");
            builder.Attributes.Add("data-toggle","tooltip");
            builder.Attributes.Add("title",text);

            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }
        public static String Translate(this HtmlHelper helper,  String page, String key)
        {
            ViewModelBase model = helper.ViewData.Model as ViewModelBase;
            Translation t =  model.Translations.Where(w => w.Page.Equals(page) && w.Key.Equals(key)).FirstOrDefault();
            if ((t == null) || (String.IsNullOrEmpty(t.Value)))
            {
                t = model.TranslationEN.Where(w => w.Page.Equals(page) && w.Key.Equals(key)).FirstOrDefault();
            }
            if (t == null)
            {
                try
                {
                    AppContext ctx = new AppContext();
                    Translation newTranslation = new Translation();
                    newTranslation.Key = key;
                    newTranslation.LanguageID = "EN";
                    newTranslation.Page = page;
                    newTranslation.Value = String.Format("TO CHANGE {0}.{1}", page, key);

                    ctx.Translations.Add(newTranslation);
                    ctx.SaveChanges();
                    return newTranslation.Value;
                }
                catch (Exception ex)
                {
                }
                return "EXCEPTION INSERT";
            }
            return t.Value;
        }

        public static String DataTablesUrl(this HtmlHelper helper) {
            String result = "//cdn.datatables.net/plug-ins/9dcbecd42ad/i18n/English.json";
            String ln = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            switch (ln.ToUpper())
            {
                case "DE":
                    result = "//cdn.datatables.net/plug-ins/9dcbecd42ad/i18n/German.json";
                    break;
                case "FR":
                    result = "//cdn.datatables.net/plug-ins/9dcbecd42ad/i18n/English.json";
                    break;
                case "IT":
                    result = "//cdn.datatables.net/plug-ins/9dcbecd42ad/i18n/Italian.json";
                    break;
            }
            return result;
        }


        public static String DataTableColumns(this HtmlHelper helper, List<WsMappingTable> fields, bool addCommand)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[");
            if (addCommand)
            {
                builder.Append("{ \"sWidth\": \"100px\", \"bSortable\": false, \"sClass\": \"center\" },");
            }
            foreach(WsMappingTable field in fields)
            {
                builder.AppendFormat("{{ \"sType\": \"{0}\", \"sWidth\": \"{1}px\", \"bSortable\": {2}, \"sClass\": \"{3}\" }},", field.ColumnType,field.Width,field.Sortable ? "true" : "false",field.HtmlClass);
            }
            
            return builder.ToString().TrimEnd(',')+"]";
            /*
                
                { "sType": "string", "sName": "DescrizioneMateriale", "mData": "DescrizioneMateriale" }, //DescrizioneMateriale
                { "sType": "mynumber", "type": "mynumber" },//ImportoCondizioni
                { "sType": "string", "sName": "Magazzino", "mData": "Magazzino" }, //CodiceMagazzino
                { "sType": "string", "sName": "ValorizzazioneUtile", "mData": "ValorizzazioneUtile" }, //ValorizzazioneUtile
                { "sType": "string", "sName": "DefinizioneMagazzino", "mData": "DefinizioneMagazzino" }, //Descrizione
                { "sType": "string", "sName": "Telefono", "mData": "Telefono" }//Telefono

            String result = "//cdn.datatables.net/plug-ins/9dcbecd42ad/i18n/English.json";
            String ln = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            switch (ln.ToUpper())
            {
                case "DE":
                    result = "//cdn.datatables.net/plug-ins/9dcbecd42ad/i18n/German.json";
                    break;
                case "FR":
                    result = "//cdn.datatables.net/plug-ins/9dcbecd42ad/i18n/English.json";
                    break;
                case "IT":
                    result = "//cdn.datatables.net/plug-ins/9dcbecd42ad/i18n/Italian.json";
                    break;
            }
            return result;*/
        }
    }
}