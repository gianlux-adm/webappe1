using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ica.SalesOrders.Web.Helpers
{
    public enum AspettoBox
    {
        Default,
        Primary
    }

    public static class IDisposableHelpers
    {
        public static IDisposable Box(this HtmlHelper htmlHelper, string titolo = null, string footer = null, AspettoBox aspetto = AspettoBox.Default, bool comprimibile = false, bool chiudibile = false, bool overlayCaricamento = false)
        {
            var haHeader = !string.IsNullOrEmpty(titolo) || aspetto != AspettoBox.Default || comprimibile || chiudibile;
            var classeBox = "box-" + aspetto.ToString().ToLower() + (aspetto != AspettoBox.Default ? " box-solid" : "");
            var inizio = string.Format("<div class=\"box {0}\">\r\n", classeBox) +
                (haHeader ? "<div class=\"box-header with-border\">\r\n" : string.Empty) +
                (titolo != null ? string.Format("<h3 class=\"box-title\">{0}</h3>", titolo) : string.Empty) +
                (comprimibile || chiudibile ? "<div class=\"box-tools pull-right\">\r\n" : string.Empty) +
                (comprimibile ? "<button class=\"btn btn-box-tool\" data-widget=\"collapse\"><i class=\"fa fa-minus\"></i></button>\r\n" : string.Empty) +
                (chiudibile ? "<button class=\"btn btn-box-tool\" data-dismiss=\"modal\"><i class=\"fa fa-remove\"></i></button>\r\n" : string.Empty) +
                (comprimibile || chiudibile ? "</div>" : string.Empty) +
                (haHeader ? "</div><!-- /.box-header -->\r\n" : string.Empty) +
                "<div class=\"box-body\">";
            var fine = "</div><!-- /.box-body -->\r\n" +
                (footer != null ? string.Format("<div class=\"box-footer\"{1}>\r\n{0}\r\n</div>\r\n", footer, footer == string.Empty ? " style=\"display:none\"" : string.Empty) : string.Empty) +
                (overlayCaricamento ? "<div class=\"overlay\" style=\"display:none;\"><i class=\"fa fa-spinner fa-pulse\"></i></div>" : string.Empty) +
                "</div>";
            return new DisposableHelper(
                htmlHelper.ViewContext.Writer,
                inizio,
                fine
            );
        }


    }

    class DisposableHelper : IDisposable
    {
        private Action end;


        // When the object is created, write "begin" function
        public DisposableHelper(TextWriter writer, string begin, string end) : this(() => writer.Write(begin), () => writer.Write(end)) { }
        public DisposableHelper(Action begin, Action end)
        {
            this.end = end;
            begin();
        }

        // When the object is disposed (end of using block), write "end" function
        public void Dispose()
        {
            end();
        }
    }
}