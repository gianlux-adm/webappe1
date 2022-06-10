using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ica.SalesOrders.Web.Auth
{
    public class SystemWebCookieManager : ICookieManager
    {
        public string GetRequestCookie(IOwinContext context, string key)
        {
            System.IO.File.AppendAllText(@"C:\tmp\log.log", "Reading Cookie with key : " + key + System.Environment.NewLine);

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var webContext = context.Get<HttpContextBase>(typeof(HttpContextBase).FullName);
            var cookie = webContext.Request.Cookies[key];
            string cookieValue = null;
            if (cookie == null)
            {
                System.IO.File.AppendAllText(@"C:\tmp\log.log", "Cookie with key : " + key + " not found" +  System.Environment.NewLine);

                System.IO.File.AppendAllText(@"C:\tmp\log.log", "Level 2 : Attemping to read cookie key : " + key.Substring(0, key.Length - 1) + System.Environment.NewLine);

                cookie = webContext.Request.Cookies[key.Substring(0, key.Length - 1)];

                if (cookie != null)
                {
                    cookieValue = cookie.Value.Substring(1,cookie.Value.Length-1);
                }
                else
                {
                    System.IO.File.AppendAllText(@"C:\tmp\log.log", "Level 2 : Cookie with key : " + key.Substring(0, key.Length - 1) + " not found" + System.Environment.NewLine);
                }
            }
            else
            {
                cookieValue = cookie.Value;
            }
            System.IO.File.AppendAllText(@"C:\tmp\log.log", "Value of Cookie " + key + " - " + cookieValue + System.Environment.NewLine);
            return cookieValue;
        }

        public void AppendResponseCookie(IOwinContext context, string key, string value, CookieOptions options)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            var webContext = context.Get<HttpContextBase>(typeof(HttpContextBase).FullName);

            bool domainHasValue = !string.IsNullOrEmpty(options.Domain);
            bool pathHasValue = !string.IsNullOrEmpty(options.Path);
            bool expiresHasValue = options.Expires.HasValue;

            var cookie = new HttpCookie(key, value);
            if (domainHasValue)
            {
                cookie.Domain = options.Domain;
            }
            if (pathHasValue)
            {
                cookie.Path = options.Path;
            }
            if (expiresHasValue)
            {
                cookie.Expires = options.Expires.Value;
            }
            if (options.Secure)
            {
                cookie.Secure = true;
            }
            if (options.HttpOnly)
            {
                cookie.HttpOnly = true;
            }

            webContext.Response.AppendCookie(cookie);
        }

        public void DeleteCookie(IOwinContext context, string key, CookieOptions options)
        {
            System.IO.File.AppendAllText(@"C:\tmp\log.log", "Attemping to delete Cookie with key " + key + System.Environment.NewLine);

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            AppendResponseCookie(
                context,
                key,
                string.Empty,
                new CookieOptions
                {
                    Path = options.Path,
                    Domain = options.Domain,
                    Expires = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                });
        }
    }
}