using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.Owin.Security;
//using Ica.SalesOrders.Web.Auth;
using Ica.SalesOrders.Models.Context;
using Microsoft.AspNet.Identity;
using Ica.SalesOrders.Web.Auth;
using System.Globalization;
using System.Configuration;
using Ica.SalesOrders.Models;

[assembly: OwinStartup(typeof(Ica.SalesOrders.Web.Startup1))]

namespace Ica.SalesOrders.Web
{
    public class Startup1
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());


            var options = new OpenIdConnectAuthenticationOptions
            {
                MetadataAddress = String.Format(Globals.WellKnownMetadata, Globals.Tenant, Globals.DefaultPolicy),

                ClientId = Globals.ClientId,
                //Authority = Globals.B2CAuthority,
                RedirectUri = Globals.RedirectUri,
                PostLogoutRedirectUri = Globals.RedirectUri,
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "emails",
                    ValidateIssuer = false
                },
                //CookieManager = new SystemWebCookieManager(),
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthenticationFailed = (context) =>
                    {
                        System.IO.File.AppendAllText(@"C:\tmp\log.log", "Errore autenticazione !");
                        context.HandleResponse();
                        context.OwinContext.Response.Redirect("/Home/Index");
                        return Task.FromResult(0);
                    }
                                    ,
                    SecurityTokenValidated = Test
                }
            };

            app.Use(typeof(MyOpenIDConnectAuthenticationMiddleware), app, options);

        }
        private Task Test(SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> arg)
        {
            System.IO.File.AppendAllText(@"C:\tmp\log.log", "Arrived inside Security Token Validated" + System.Environment.NewLine);

            ClaimsIdentity myClaims = arg.AuthenticationTicket.Identity as ClaimsIdentity;
            String name = String.Empty;
            if (myClaims != null)
            {
                try
                {
                    name = myClaims.FindFirstValue("name");
                }
                catch (Exception ex)
                {

                }
            }

            String currentEmail = arg.AuthenticationTicket.Identity.Name;
            System.IO.File.AppendAllText(@"C:\tmp\log.log", "Identity : " + currentEmail + System.Environment.NewLine);

            System.IO.File.AppendAllText(@"C:\tmp\log.log", "Execute Owin Authentication Sign " + System.Environment.NewLine);

            /*
            ClaimsIdentity newClaim = new ClaimsIdentity(
                arg.AuthenticationTicket.Identity.Claims,
                CookieAuthenticationDefaults.AuthenticationType,"emails","role");
            System.Web.HttpContext.Current.GetOwinContext().Authentication.SignIn(new AuthenticationProperties
            {
                IsPersistent = true
            }, newClaim);
            */
            

            ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationType, ClaimTypes.GivenName, ClaimTypes.Role);
            arg.AuthenticationTicket = new AuthenticationTicket(identity, arg.AuthenticationTicket.Properties);
            arg.AuthenticationTicket.Identity.AddClaim(new Claim(ClaimTypes.Name, currentEmail));
            arg.AuthenticationTicket.Identity.AddClaim(new Claim(ClaimTypes.GivenName, currentEmail));
            arg.AuthenticationTicket.Identity.AddClaim(new Claim(ClaimTypes.Email, currentEmail));
            arg.AuthenticationTicket.Identity.AddClaim(new Claim("name", name));
            arg.AuthenticationTicket.Identity.AddClaim(new Claim("emails", currentEmail));

            arg.AuthenticationTicket.Properties.RedirectUri = "/Sales";

            return Task.FromResult(0);

        }


        
        public void ConfigureAuthOld(IAppBuilder app)
        {
            

          // Required for Azure webapps, as by default they force TLS 1.2 and this project attempts 1.0
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            app.SetDefaultSignInAsAuthenticationType("OpenIdConnect");// CookieAuthenticationDefaults.AuthenticationType);

            CookieAuthenticationOptions opt = new CookieAuthenticationOptions();
            opt.AuthenticationType = "OpenIdConnect";// DefaultAuthenticationTypes.ApplicationCookie;
            app.UseCookieAuthentication(opt);

            var idConnectOpt = new OpenIdConnectAuthenticationOptions
            {
                // Generate the metadata address using the tenant and policy information
                MetadataAddress = String.Format(Globals.WellKnownMetadata, Globals.Tenant, Globals.DefaultPolicy),

                // These are standard OpenID Connect parameters, with values pulled from web.config
                ClientId = Globals.ClientId,
                RedirectUri = Globals.RedirectUri,
                PostLogoutRedirectUri = Globals.RedirectUri,
                //SignInAsAuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                // Specify the callbacks for each type of notifications
                
                Notifications = new OpenIdConnectAuthenticationNotifications
                {

                    RedirectToIdentityProvider = ctx =>
                    {
                       bool isAjaxRequest = (ctx.Request.Headers != null && ctx.Request.Headers["X-Requested-With"] == "XMLHttpRequest");

                        if (isAjaxRequest)
                        {
                            ctx.Response.Headers.Remove("Set-Cookie");
                            ctx.State = NotificationResultState.HandledResponse;
                        }

                        return Task.FromResult(0);
                    },
                    //AuthorizationCodeReceived = OnAuthorizationCodeReceived,
                    //AuthenticationFailed = OnAuthenticationFailed,
                    //SecurityTokenValidated = SecurityTokenValidated
                },

                
                // Specify the claim type that specifies the Name property.
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "emails",
                    ValidateIssuer = false
                }
                /*
                // Specify the scope by appending all of the scopes requested into one string (separated by a blank space)
                Scope = $"openid profile offline_access {Globals.ReadTasksScope} {Globals.WriteTasksScope}"
        */
            };
            app.UseOpenIdConnectAuthentication(
                idConnectOpt
            );
        }
/*
        private Task SecurityTokenValidated(SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> n)
        {
            
            string currentUserEmail = n.AuthenticationTicket.Identity.FindFirst("emails").Value;
            using (AppContext ctx = new AppContext())
            {
                var user = ctx.Users.Where(w => w.LoginName == currentUserEmail).FirstOrDefault();
                if (user == null || user.Enabled == false)
                {
                    n.AuthenticationTicket.Identity.AddClaim(new Claim(ClaimTypes.Role, "NoAccess"));
                }
                else
                {
                    n.AuthenticationTicket.Identity.AddClaim(new Claim(ClaimTypes.Role, user.Role.RoleDescription));
                }
            }

            return Task.FromResult(0);
        }
        */
        /*
         *  On each call to Azure AD B2C, check if a policy (e.g. the profile edit or password reset policy) has been specified in the OWIN context.
         *  If so, use that policy when making the call. Also, don't request a code (since it won't be needed).
         */
        private Task OnRedirectToIdentityProvider(RedirectToIdentityProviderNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification)
        {
            var policy = notification.OwinContext.Get<string>("Policy");

            if (!string.IsNullOrEmpty(policy) && !policy.Equals(Globals.DefaultPolicy))
            {
                notification.ProtocolMessage.Scope = OpenIdConnectScope.OpenId;
                notification.ProtocolMessage.ResponseType = OpenIdConnectResponseType.IdToken;
                notification.ProtocolMessage.IssuerAddress = notification.ProtocolMessage.IssuerAddress.ToLower().Replace(Globals.DefaultPolicy.ToLower(), policy.ToLower());
            }

            return Task.FromResult(0);
        }

        /*
         * Catch any failures received by the authentication middleware and handle appropriately
         */
        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification)
        {
            notification.HandleResponse();

            // Handle the error code that Azure AD B2C throws when trying to reset a password from the login page
            // because password reset is not supported by a "sign-up or sign-in policy"
            if (notification.ProtocolMessage.ErrorDescription != null && notification.ProtocolMessage.ErrorDescription.Contains("AADB2C90118"))
            {
                // If the user clicked the reset password link, redirect to the reset password route
                notification.Response.Redirect("/Account/ResetPassword");
            }
            else if (notification.Exception.Message == "access_denied")
            {
                notification.Response.Redirect("/");
            }
            else
            {
                notification.Response.Redirect("/Home/Error?message=" + notification.Exception.Message);
            }

            return Task.FromResult(0);
        }

        /*
         * Callback function when an authorization code is received
         */
        private async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedNotification notification)
        {
            try
            {
                System.IO.File.AppendAllText(@"C:\tmp\log.log", "OnAuthorizationCodeReceived" + System.Environment.NewLine);
                /*
				 The `MSALPerUserMemoryTokenCache` is created and hooked in the `UserTokenCache` used by `IConfidentialClientApplication`.
				 At this point, if you inspect `ClaimsPrinciple.Current` you will notice that the Identity is still unauthenticated and it has no claims,
				 but `MSALPerUserMemoryTokenCache` needs the claims to work properly. Because of this sync problem, we are using the constructor that
				 receives `ClaimsPrincipal` as argument and we are getting the claims from the object `AuthorizationCodeReceivedNotification context`.
				 This object contains the property `AuthenticationTicket.Identity`, which is a `ClaimsIdentity`, created from the token received from
				 Azure AD and has a full set of claims.
				 */

                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(notification.AuthenticationTicket.Identity);
                System.IO.File.AppendAllText(@"C:\tmp\log.log", "claimsPrincipal" + System.Environment.NewLine);
                ClaimsIdentity claimsPrincipalIdentity = claimsPrincipal.Identity as ClaimsIdentity;
                System.IO.File.AppendAllText(@"C:\tmp\log.log", "claimsPrincipalIdentity" + System.Environment.NewLine);
                String currentUserEmail = String.Empty;
                var emailsClaims = claimsPrincipal.FindFirst("emails");
                var existingClaim = claimsPrincipal.FindFirst("name");
                if (existingClaim != null && emailsClaims != null)
                {
                    claimsPrincipalIdentity.RemoveClaim(existingClaim);

                    claimsPrincipalIdentity.AddClaim(new Claim("name", emailsClaims.Value));
                    currentUserEmail = emailsClaims.Value;
                }

                System.IO.File.AppendAllText(@"C:\tmp\log.log", "CurrentEmail : " + currentUserEmail + System.Environment.NewLine);

                System.IO.File.AppendAllText(@"C:\tmp\log.log", "Open App Context" + System.Environment.NewLine);
                //TODO : Verificare se l'utente è abilitato o meno al servizio
                //Se l'utente non è abilitato al servizio mostrare avviso
                using (AppContext ctx = new AppContext())
                {
                    var user = ctx.Users.Where(w => w.LoginName == currentUserEmail).FirstOrDefault();
                    if (user == null || user.Enabled==false)
                    {                        
                        claimsPrincipalIdentity.AddClaim(new Claim(ClaimTypes.Role, "NoAccess"));
                    }
                    else
                    {
                        claimsPrincipalIdentity.AddClaim(new Claim(ClaimTypes.Role, user.Role.RoleDescription));
                    }
                }

                
                IConfidentialClientApplication confidentialClient = MsalAppBuilder.BuildConfidentialClientApplication(claimsPrincipal);
                
                
                System.IO.File.AppendAllText(@"C:\tmp\log.log", "Acquire Token by Authorization Code "  + System.Environment.NewLine);

                // Upon successful sign in, get & cache a token using MSAL
                AuthenticationResult result = await confidentialClient.AcquireTokenByAuthorizationCode(Globals.Scopes, notification.Code).ExecuteAsync();
                //AuthenticationResult result = await confidentialClient.AcquireTokenByAuthorizationCode(null,notification.Code).ExecuteAsync();
                

                //var authContext = new AuthenticationContext(notification.Options.Authority);

                //System.IO.File.AppendAllText(@"C:\tmp\log.log", "Auth Result Expiration : " + result.ExpiresOn + System.Environment.NewLine);

            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(@"C:\tmp\log.log", "Eccezione " + ex.Message + System.Environment.NewLine);

                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = $"Unable to get authorization code {ex.Message}."
                });
            }
        }

    }
}
