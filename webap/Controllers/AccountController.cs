using Ica.SalesOrders.Core.Http;
using Ica.SalesOrders.Jde.Dto;
using Ica.SalesOrders.Jde.Services;
using Ica.SalesOrders.Models;
using Ica.SalesOrders.Web.Models;
using Ica.SalesOrders.Web.ViewModels;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Owin;
using System.Web;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security.Cookies;
//using Ica.SalesOrders.Web.Auth;

namespace Ica.SalesOrders.Web.Controllers
{
    public class AccountController : BaseController
    {
        /*
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(bool? expired)
        {
            LoginViewModel lvm = new LoginViewModel();
            lvm.Languages = Context.Languages.OrderBy(o => o.LanguageName).ToList();

            String lang= System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            HttpCookie cookie = HttpContext.Request.Cookies["_culture"];
            if (cookie != null)
            {
                lang = cookie.Value;
            }

            lvm.Language = lang;
            if ((expired.HasValue) && (expired.Value==true)) {
                lvm.Result = new OperationResult<object>();
                lvm.Result.Result = false;
                lvm.Result.Message = Translate("Shared", "SessionExpired");
            }
            
            return View(lvm);
        } // End LoginView.

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel lvm)
        {
            if (ModelState.IsValid)
            {
                
                ApplicationUser u = Context.Users.Where(w=>w.LoginName.Equals(lvm.Username) && w.Password.Equals(lvm.Password)).FirstOrDefault();
                
                    if (u != null)
                    {
                        if (u.Enabled)
                        {
                            FormsAuthentication.SetAuthCookie(lvm.Username, true);

                            HttpCookie langCookie = new HttpCookie("_culture", lvm.Language);
                            langCookie.Expires = DateTime.Now.AddYears(1);
                            Response.Cookies.Add(langCookie);

                            return Redirect(lvm.ReturnUrl ?? "/");
                        }
                        else
                        {
                            ModelState.AddModelError("UtenteNonTrovato", Translate("Login", "UserNotEnabled"));
                            lvm.Languages = Context.Languages.OrderBy(o => o.LanguageName).ToList();
                            lvm.Language = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                            return View(lvm);
                        }
                    }
                    u = Context.Users.Where(w => w.LoginName.Equals(lvm.Username)).FirstOrDefault();
                    if (u == null)
                    {
                        ModelState.AddModelError("UtenteNonTrovato", Translate("Login", "UserNotFound"));
                    }
                    else
                    {
                        ModelState.AddModelError("UtenteNonTrovato", Translate("Login", "PasswordNotValid"));
                    }
                
            }
            lvm.Languages = Context.Languages.OrderBy(o => o.LanguageName).ToList();
            lvm.Language = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            return View(lvm);
        } // End LoginView.
        */

        


        [HttpPost]
        public ActionResult JdeLogin(JdeLoginViewModel vm)
        {
            vm.Function = Context.Functions.Where(w => w.FunctionId == vm.IdFunction).FirstOrDefault();
            AuthService authService = new AuthService();
            int currentUserId = GetCurrentUserId();
            bool autenticato = EffettuaAutententicazioneJde(currentUserId,vm.Username,vm.Password, vm.Function.Data1,vm.Function.Data2, authService);
            if (autenticato)
            {
                return Redirect(vm.ReturnUrl);
            }

            vm.Result = new OperationResult<object>();
            vm.Result.Result = false;
            vm.Result.Message = "ERRORE TODO";
            return View("LoginJde", vm);
        }


        /*
        [HttpGet]
        public ActionResult Logout()
        {
            Session["Version"] = null;
            FormsAuthentication.SignOut();
            return Redirect("/");
        } // End Logout.
        */

        public ActionResult LoginAdfs()
        {
            LoginViewModel lvm = new LoginViewModel();
            return View("Login",lvm);
        }


        
        public async Task<ActionResult> Login(string redirectUrl,int force =0)
        {
            if (Request.IsAuthenticated && force == 1)
            {
                //await MsalAppBuilder.ClearUserTokenCache();
                IEnumerable<AuthenticationDescription> authTypes = HttpContext.GetOwinContext().Authentication.GetAuthenticationTypes();
                HttpContext.GetOwinContext().Authentication.SignOut(authTypes.Select(t => t.AuthenticationType).ToArray());
                Request.GetOwinContext().Authentication.GetAuthenticationTypes();
                return null;
            }
            if (Request.IsAuthenticated == false)
            {
                redirectUrl = redirectUrl ?? "/";

                HttpContext.Session.Clear();
                // Use the default policy to process the sign up / sign in flow
                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties
                { RedirectUri = redirectUrl }, 
                OpenIdConnectAuthenticationDefaults.AuthenticationType);
                return null;
            }

            return RedirectToAction("Index", "SalesOrders");
        }

        /*
         *  Called when requesting to edit a profile
         */
        public void EditProfile()
        {
            if (Request.IsAuthenticated)
            {
                // Let the middleware know you are trying to use the edit profile policy (see OnRedirectToIdentityProvider in Startup.Auth.cs)
                HttpContext.GetOwinContext().Set("Policy", Globals.EditProfilePolicyId);

                // Set the page to redirect to after editing the profile
                var authenticationProperties = new AuthenticationProperties { RedirectUri = "/" };
                HttpContext.GetOwinContext().Authentication.Challenge(authenticationProperties);

                return;
            }

            Response.Redirect("/");
        }

        /*
         *  Called when requesting to reset a password
         */
        public void ResetPassword()
        {
            // Let the middleware know you are trying to use the reset password policy (see OnRedirectToIdentityProvider in Startup.Auth.cs)
            HttpContext.GetOwinContext().Set("Policy", Globals.ResetPasswordPolicyId);

            // Set the page to redirect to after changing passwords
            var authenticationProperties = new AuthenticationProperties { RedirectUri = "/" };
            HttpContext.GetOwinContext().Authentication.Challenge(authenticationProperties);

            return;
        }

        /*
         *  Called when requesting to sign out
         */
        public async Task Logout()
        {
            // To sign out the user, you should issue an OpenIDConnect sign out request.
            if (Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(
        OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);

                /*
                IEnumerable<AuthenticationDescription> authTypes = HttpContext.GetOwinContext().Authentication.GetAuthenticationTypes();
                HttpContext.GetOwinContext().Authentication.SignOut(authTypes.Select(t => t.AuthenticationType).ToArray());
                Request.GetOwinContext().Authentication.GetAuthenticationTypes();*/
            }
        }



    }
}