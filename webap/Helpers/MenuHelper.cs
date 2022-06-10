using Ica.SalesOrders.Models;
using Ica.SalesOrders.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ica.SalesOrders.Web.Helpers
{
    public static class MenuHelper
    {
        private static TagBuilder GenerateLi(Function mi,String currentAction, String currentController)
        {
            TagBuilder li = new TagBuilder("li");
            li.AddCssClass("treeview");
            if (String.IsNullOrEmpty(mi.Url))
            {
                mi.Url = String.Empty;
            }
            String[] urlValues = mi.Url.Replace(@"\", "/").TrimStart("/".ToCharArray()).Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (urlValues.Count() > 0)
            {
                String controller = urlValues[0];
                String action = "index";
                if (urlValues.Count() > 1)
                {
                    action = urlValues[1];
                }

                if ((controller.Equals(currentController, StringComparison.InvariantCultureIgnoreCase))
                    && (action.Equals(currentAction, StringComparison.InvariantCultureIgnoreCase)))
                {
                    li.AddCssClass("active");
                }
                else
                {
                    if ((controller.Equals(currentController, StringComparison.InvariantCultureIgnoreCase))
                    && (action.Equals(currentAction.Replace("Edit","Index"), StringComparison.InvariantCultureIgnoreCase)))
                    {
                        li.AddCssClass("active");
                    }
                }
            }

            TagBuilder a = new TagBuilder("a");
            a.MergeAttribute("href", mi.Url);
            TagBuilder i = new TagBuilder("i");
            i.AddCssClass(mi.Icon);
            a.InnerHtml += i;
            TagBuilder span = new TagBuilder("span");
            span.InnerHtml += mi.Description;
            a.InnerHtml += span;
            if (mi.Child.Count() > 0)
            {
                TagBuilder arrow = new TagBuilder("i");
                arrow.AddCssClass("fa fa-angle-left pull-right");
                a.InnerHtml += arrow;
            }
            li.InnerHtml += a;

            if (mi.Child.Count() > 0)
            {
                TagBuilder ul = new TagBuilder("ul");
                ul.AddCssClass("treeview-menu");
                foreach (Function child in mi.Child)
                {
                    TagBuilder toAddLi = GenerateLi(child, currentAction, currentController);
                    if (toAddLi.Attributes["class"].Contains("active"))
                    {
                        ul.AddCssClass("active");
                        li.AddCssClass("active");
                    }

                    ul.InnerHtml += toAddLi;
                }
                li.InnerHtml += ul;
            }

            return li;
        }

        public static MvcHtmlString Menu
            (this HtmlHelper helper
            , List<Function> menu)
        {
            ViewContext viewContext = helper.ViewContext;
            bool isChildAction = viewContext.Controller.ControllerContext.IsChildAction;
            if (isChildAction)
                viewContext = helper.ViewContext.ParentActionViewContext;
            RouteValueDictionary routeValues = viewContext.RouteData.Values;
            string currentAction = routeValues["action"].ToString();
            string currentController = routeValues["controller"].ToString();

            StringBuilder builder = new StringBuilder();
            foreach (Function mi in menu)
            {
                builder.Append(GenerateLi(mi, currentAction, currentController));
            }
            return MvcHtmlString.Create(builder.ToString());
        }

        /*

        public static String IsActive(this HtmlHelper html, List<MenuItem> records,String cssClass)
        {
              ViewContext viewContext = html.ViewContext;
              bool isChildAction = viewContext.Controller.ControllerContext.IsChildAction;
              if (isChildAction)
              viewContext = html.ViewContext.ParentActionViewContext;
              RouteValueDictionary routeValues = viewContext.RouteData.Values;
              string currentAction = routeValues["action"].ToString();
              string currentController = routeValues["controller"].ToString();

              MenuItem found = records.Where(w => w.Controller.Equals(currentController, StringComparison.InvariantCultureIgnoreCase) && w.Action.Equals(currentAction, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

              return (found == null) ? String.Empty : cssClass;
        }*/
    }
}