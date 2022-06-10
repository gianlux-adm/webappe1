using Ica.SalesOrders.Models;
using Ica.SalesOrders.Models.Context;
using Ica.SalesOrders.Web.Models.DataTables;
using Ica.SalesOrders.Web.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Ica.SalesOrders.Web.Controllers;
using System.Configuration;
using System.Security.Claims;

namespace Ica.SalesOrders.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ViewEngines.Engines.Clear();
            IViewEngine razorEngine = new RazorViewEngine() { FileExtensions = new string[] { "cshtml" } };
            ViewEngines.Engines.Add(razorEngine);
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ControllerBuilder.Current.SetControllerFactory(new DefaultControllerFactory(new LocalizedControllerActivator()));
            ModelBinders.Binders.Add(typeof(DataTableOption),new DataTableModelBinder());

        }
        
        protected void Application_PostAuthenticateRequest()
        {
            if (Request.IsAuthenticated)
            {
                ClaimsIdentity currentIdentity = User.Identity as ClaimsIdentity;
                String currentRole = String.Empty;

                using (AppContext Context = new AppContext())
                {
                    String domainControllerName = ConfigurationManager.AppSettings["DomainControllerName"];
                    String accessGroupName = ConfigurationManager.AppSettings["AccessGroupName"];


                    if (CanAccessToSystem(domainControllerName, User.Identity.Name, accessGroupName))
                    {
                        
                            ApplicationUser u = Context.Users.Where(w => w.LoginName.Equals(User.Identity.Name, StringComparison.InvariantCultureIgnoreCase) && w.Enabled == true).FirstOrDefault();
                            if (u == null)
                            {
                                u = new ApplicationUser();
                                u.LoginName = User.Identity.Name;
                                u.Password = String.Empty;
                                u.CodiceE1 = String.Empty;
                                u.Name = currentIdentity.FindFirst("name").Value;
                                u.RoleId = 2;
                                u.Enabled = true;

                                Context.Users.Add(u);
                                Context.SaveChanges();
                            }
                            currentRole = Context.Roles.Where(w => w.RoleId == u.RoleId).FirstOrDefault().RoleDescription;
                 
                        
                    }
                    else
                    {
                        currentRole = "NoAccess";
                    }


                    //ApplicationUser u = Context.Users.Where(w => w.LoginName.Equals(User.Identity.Name, StringComparison.InvariantCultureIgnoreCase) && w.Enabled == true).FirstOrDefault();

                    GenericPrincipal principal = new GenericPrincipal(User.Identity, new string[] { currentRole });

                    Thread.CurrentPrincipal = HttpContext.Current.User = principal;
                }
            }
        }


        private bool CanAccessToSystem(String domainName, String username, String accessGroupName)
        {	
            bool r = false;
            return true;
            try
            {
                using (var searcher = new System.DirectoryServices.DirectorySearcher(new System.DirectoryServices.DirectoryEntry("LDAP://" + domainName)))
                {
                    searcher.Filter = "(userPrincipalName=" + username + ")";
                    System.DirectoryServices.SearchResultCollection result = searcher.FindAll();

                    foreach (System.DirectoryServices.SearchResult sr in result)
                    {
                        var memberOfArray = sr.Properties["memberof"];
                        if (memberOfArray != null)
                        {
                            foreach (var item in memberOfArray)
                            {
                                item.ToString();
                                if (item.ToString().ToLower().Contains(accessGroupName.ToLower()))
                                {
                                    r = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                r = false;
            }


            return r;
        }



    }
}
