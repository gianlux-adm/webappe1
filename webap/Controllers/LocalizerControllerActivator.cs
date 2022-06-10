using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ica.SalesOrders.Web.Controllers
{
    public class LocalizedControllerActivator : IControllerActivator
    {
        private string _DefaultLanguage = "en";

        public IController Create(RequestContext requestContext, Type controllerType)
        {
            //Get the {language} parameter in the RouteData
            string lang = "en"; // requestContext.RouteData.Values["lang"] ?? _DefaultLanguage;

            HttpCookie cookie = requestContext.HttpContext.Request.Cookies["_culture"];
            if (cookie != null)
            {
                lang = cookie.Value;
            }
            else
            {

                cookie = new HttpCookie("_culture");
                cookie.Value = lang;
            }
            cookie.Expires = DateTime.Now.AddYears(1);
            requestContext.HttpContext.Response.Cookies.Add(cookie);


            try
            {
                Thread.CurrentThread.CurrentCulture =
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            }
            catch (Exception e)
            {
                throw new NotSupportedException(String.Format("ERROR: Invalid language code '{0}'.", lang));
            }

            return DependencyResolver.Current.GetService(controllerType) as IController;
        }
    }
}