using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ica.SalesOrders.Models.Context;
using Ica.SalesOrders.Web.ViewModels;
using System.Threading;
using Ica.SalesOrders.Web.Models.DataTables;
using Ica.SalesOrders.Web.Extensions;
using System.Text;
using Ica.SalesOrders.Models;
using Ica.SalesOrders.Web.Models;
using System.Data.SqlClient;
using System.Security.Principal;
using System.IO;
using Ica.SalesOrders.Models.Context;
using Ica.SalesOrders.Core.Http;
using Ica.SalesOrders.Jde.Dto;
using Ica.SalesOrders.Jde.Services;
using System.Security.Claims;
using System.Web.Routing;

namespace Ica.SalesOrders.Web.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
            Context = new AppContext();
        }

        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                                                                         viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public void CurrentUserIsRole(String role) {
            GenericPrincipal current = User as GenericPrincipal;
            
        }

        public AppContext Context { get; set; }

        protected string Translate(String page, String key)
        {
            String languageId = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            return Translate(page, key, languageId);
        }

        protected string Translate(String page, String key, string language)
        {
            List<Translation> translations = ViewBag.Translations as List<Translation>;

            Translation t = translations.Where(w => w.LanguageID.Equals(language,StringComparison.InvariantCultureIgnoreCase) && w.Page.Equals(page) && w.Key.Equals(key)).FirstOrDefault();
            if ((t == null) || (String.IsNullOrEmpty(t.Value)))
            {
                t = translations.Where(w => w.LanguageID.Equals("en", StringComparison.InvariantCultureIgnoreCase) && w.Page.Equals(page) && w.Key.Equals(key)).FirstOrDefault();
            }
            if ((t == null) || (String.IsNullOrEmpty(t.Value)))
            {
                Translation newTranslation = new Translation();
                newTranslation.Key = key;
                newTranslation.LanguageID = "EN";
                newTranslation.Page = page;
                newTranslation.Value = String.Format("TO CHANGE {0}.{1}", page, key);

                Context.Translations.Add(newTranslation);
                Context.SaveChanges();
                return newTranslation.Value;
            }
            return t.Value;
        }

    
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                if (filterContext.HttpContext.User.Identity.IsAuthenticated == false)
                {
                    filterContext.Result = new HttpStatusCodeResult(403, "SessionExpired");
                    return;
                }
            }

            bool checkNoAccess = true;
            string currentController = filterContext.RouteData.Values["Controller"].ToString();

            
            if (currentController.Equals("NoAccess", StringComparison.InvariantCultureIgnoreCase) || currentController.Equals("Account",StringComparison.InvariantCultureIgnoreCase))
            {
                checkNoAccess = false;
            }
            if (checkNoAccess)
            {

            
            if (Request.IsAuthenticated)
            {
                ClaimsIdentity currentIdentity = User.Identity as ClaimsIdentity;
                    if (User.IsInRole("NoAccess"))
                    {
                        filterContext.Result = new RedirectToRouteResult(
   new RouteValueDictionary
   {
        {"controller", "NoAccess"},
        {"action", "Index"}
   }
);
                    }
                
            }
            }


            ViewBag.Languages = Context.Languages.OrderBy(o => o.LanguageName).ToList();
            ViewBag.Translations = Context.Translations.ToList();
            String languageId = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            ViewBag.Menu = Context.GetMenu(GetCurrentUserId(), languageId);
            base.OnActionExecuting(filterContext);
        }

        protected void AssignTranslations(ViewModelBase edvm)
        {
            String languageId = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            List<Translation> list = ViewBag.Translations as List<Translation>;
            edvm.Translations = list.Where(w => w.LanguageID.Equals(languageId, StringComparison.InvariantCultureIgnoreCase)).ToList();
            edvm.TranslationEN = list.Where(w => w.LanguageID.Equals("en", StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Result is ViewResultBase)
            {
                //ViewResultBase res = filterContext.Result as ViewResultBase;
                ViewResultBase res = (ViewResultBase)filterContext.Result;
                if (res.Model is ViewModelBase)
                {
                    ViewModelBase model = res.Model as ViewModelBase;
                    AssignTranslations(model);
                }
            }
            base.OnActionExecuted(filterContext);
        }

        public ActionResult SetLanguage(string langaugeId)
        {
            HttpCookie cookie = new HttpCookie("_culture", langaugeId);
            cookie.Expires = DateTime.Now.AddYears(1);
            Response.Cookies.Add(cookie);
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Context.Dispose();
        }

        protected ApplicationUser GetCurrentUser()
        {
            return Context.Users.Where(w => w.LoginName.Equals(User.Identity.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }

        protected int GetCurrentUserId()
        {
            ApplicationUser u = GetCurrentUser();

            if (u == null)
            {
                return 0;
            }
            return u.UserId;
        }

        protected ActionResult GenericDataTableList<T>(IQueryable<T> query, Func<IQueryable<T>, IQueryable<T>> filter, DataTableOption option,List<String> excludedFilter
            ,String thenOrderBy)
        {
            int totalRecords = query.Count();

            if (!String.IsNullOrEmpty(option.sSearch))
            {
                if (filter != null)
                {
                    query = filter(query);
                }
            }
            for (int i = 0; i < option.sSearchs.Count; i++)
            {
                String value = option.sSearchs[i];
                String field = option.mDataProp[i];
                if (!String.IsNullOrEmpty(value))
                {
                    if (excludedFilter.Contains(field)) 
                    {
                    }
                    else
                    {
                        query = query.Where(field, value);
                    }
                }
            }

            int totalFilterNumber = query.Count();

            String orderByField = option.sColumns[option.iSortCols[0]];
            DataTablesSortDirection direction = option.sSortDirs[0];
            

            if (String.IsNullOrEmpty(thenOrderBy))
            {
                query = query.OrderBy(orderByField, direction == DataTablesSortDirection.Ascending ? "asc" : "desc");
            }
            else {
                //query = query.OrderBy(orderByField, direction == DataTablesSortDirection.Ascending ? "asc" : "desc", thenOrderBy);
                query = query.OrderingHelper(orderByField, direction == DataTablesSortDirection.Ascending ? false : true, false);
                query = query.OrderingHelper(thenOrderBy, false, true);

            }
            
            query = query.Skip(option.iDisplayStart).Take(option.iDisplayLength);

            var results = query.ToList();

            var result = new DataTableResultExt(option.sEcho,
                totalRecords, totalFilterNumber,

                results.Select(s => s.ToDataTablesRow<T>()).ToList()
                );

            result.ContentEncoding = Encoding.UTF8;
            return result;
        }



        public bool EffettuaAutententicazioneJde(int userId, String username,String password, string environment,string role, AuthService authService)
        {
            bool r = false;
            
            //SendRequestResponse<AuthResponseDto> response = authService.Authenticate(Context,"http://ica-e1web08.ica.dom:9097/jderest/tokenrequest", role, environment, username, password,GetCurrentUserId());

            //if (response.Result)
            //{
                var userOrigin = Context.UserOrigins.Where(w => w.IdUser == userId && w.IdOrigin == 1 ).FirstOrDefault();
                if (userOrigin == null)
                {
                    userOrigin = new UserOrigin();
                    userOrigin.IdOrigin = 1;
                    userOrigin.IdUser = userId;
                    userOrigin.Password = password;
                    userOrigin.Username = username;
                    userOrigin.Token = String.Empty;
                    userOrigin.TokenCreationDate = DateTime.Now;
                    Context.UserOrigins.Add(userOrigin);
                }
                else
                {
                    userOrigin.Token = String.Empty;
                    userOrigin.TokenCreationDate = DateTime.Now;
                    userOrigin.Password = password;
                    userOrigin.Username = username;
                }
                Context.SaveChanges();
                r = true;
            //}

            Session["Version"] = null;
            return r;
        }
    }
}