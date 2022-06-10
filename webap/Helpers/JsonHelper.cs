using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Ica.SalesOrders.Web.Helpers
{
        public static class JsonHelper
        {
        public static MvcHtmlString ToDataTableFilter<T>(this HtmlHelper helper, List<T> list, Func<T,String> func)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("[");

            int i = 0;
            foreach (var item in list)
            {
                if (i > 0)
                {
                    builder.Append(",");
                }
                builder.AppendFormat("{{ \"{0}\"}}", func(item));
                i++;
            }

            builder.Append("]");

            return MvcHtmlString.Create(builder.ToString());
        }

        public static MvcHtmlString ToDataTableFilter<T>(this HtmlHelper helper, List<T> list, Func<T, String> value, Func<T, String> label)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("[");

            int i = 0;
            foreach (var item in list)
            {
                if (i > 0)
                {
                    builder.Append(",");
                }
                builder.AppendFormat("{{ value : \"{0}\", label : \"{1}\"  }}", value(item), label(item));
                i++;
            }

            builder.Append("]");

            return MvcHtmlString.Create(builder.ToString());
        }

        public static MvcHtmlString ToDataTableFilter(this HtmlHelper helper, Dictionary<String, String> dict)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("[");

            int i = 0;
            foreach (String key in dict.Keys)
            {
                if (i > 0)
                {
                    builder.Append(",");
                }
                builder.AppendFormat("{{ value : \"{0}\", label : \"{1}\"  }}", key, dict[key]);
                i++;
            }

            builder.Append("]");

            return MvcHtmlString.Create(builder.ToString());
        }

        public static MvcHtmlString ToJson(this HtmlHelper helper, Dictionary<String, String> dict)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("[");

            int i = 0;
            foreach(String key in dict.Keys) {
                if (i > 0)
                {
                    builder.Append(",");
                }
                builder.AppendFormat("{{ key : \"{0}\", value : \"{1}\"  }}",key, dict[key]);
                i++;
            }

            builder.Append("]");

            return MvcHtmlString.Create(builder.ToString());
        }
    }
}