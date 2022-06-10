using Ica.SalesOrders.Core.Http;
using Ica.SalesOrders.Jde.Dto;
using Ica.SalesOrders.Jde.Services;
using Ica.SalesOrders.Models;
using Ica.SalesOrders.Web.Models;
using Ica.SalesOrders.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ica.SalesOrders.Web.Controllers
{
    public class JdeController : BaseController
    {
     
        public JdeController()
        {
            AuthService = new AuthService();
            AuthMode = 1; //Todo prendere da DB
        }

        public DateTime JavaTimeStampToDateTime(double javaTimeStamp)
        {
            // Java timestamp is milliseconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public double DateTimeToJavaTimeStamp(DateTime date)
        {
            return date.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalMilliseconds; 
        }


        public SendRequestResponse<DataServiceResponseDto> GetDataService(DataServiceRequestDto req, LogActionEnum logAction)
        {
            String serverUrl = System.Configuration.ConfigurationManager.AppSettings["ServerUrl"];
            SendRequestResponse<DataServiceResponseDto> res = SalesOrders.Core.Http.SendRecordService.SendRequest<DataServiceResponseDto>(Context, "POST", serverUrl + "/jderest/v2/dataservice", req, String.Format("fs_DATABROWSE_{0}", req.TableName), GetCurrentUserId(), (short)logAction);
            if (res.Unauthorized)
            {
                throw new UnauthorizedException();
            }
            return res;
        }

        public string GetVersion(string fullnameForm)
        {
            if (Session[fullnameForm] == null)
            {
                string formName = fullnameForm.Split("_".ToCharArray())[1];
                string formPrefix = fullnameForm.Split("_".ToCharArray())[0];
                String version = GetVersion(formName, formPrefix, CurrentUserJde.Username);
                if (String.IsNullOrEmpty(version))
                {
                    version = GetVersion(formName, formPrefix, "*PUBLIC"); ;
                }
                if (String.IsNullOrEmpty(version))
                {
                    version = "ZJDE0001";
                }
                Session[fullnameForm] = version;
            }
            return Session[fullnameForm].ToString();
        }

     


        private string GetVersion(string formName, string formPrefix, string username)
        {
            String version = String.Empty;
            DataServiceRequestDto request = new DataServiceRequestDto();

            if (!String.IsNullOrEmpty(JdeToken))
            {
                request.Token = JdeToken;
            }
            else
            {
                request.Username = CurrentUserJde.Username;
                request.Password = CurrentUserJde.Password;
            }

            request.ReturnControlIDs = "F55ICAPP.FMNM|F55ICAPP.OBNM|F55ICAPP.USER|F55ICAPP.VER";

            request.TableName = "F55ICAPP";
            request.Query = new DataServiceQueryDto();
            request.Query.Conditions = new List<DataServiceQueryConditionDto>();

            request.Query.Conditions.Add(new DataServiceQueryConditionDto()
            {
                Value = new DataServiceQueryConditionValueDto() { Content = formName },
                QueryField = "F55ICAPP.FMNM",
                QueryOperator = "EQUAL"
            });
            request.Query.Conditions.Add(new DataServiceQueryConditionDto()
            {
                Value = new DataServiceQueryConditionValueDto() { Content = formPrefix },
                QueryField = "F55ICAPP.OBNM",
                QueryOperator = "EQUAL"
            });
            request.Query.Conditions.Add(new DataServiceQueryConditionDto()
            {
                Value = new DataServiceQueryConditionValueDto() { Content = username },
                QueryField = "F55ICAPP.USER",
                QueryOperator = "EQUAL"
            });

            var res2 = GetDataService(request, LogActionEnum.WarehouseGetVersion);

            if (res2.Result)
            {
                if (res2.Payload.Content != null)
                {
                    foreach (var row in res2.Payload.Content.Data.GridData.Righe)
                    {
                        var key = row.Values.Where(w => w.ColumnName == "F55ICAPP_VER").FirstOrDefault();

                        if (key != null)
                        {
                            version = key.Value.ToString();
                        }
                    }
                }
            }
            return version;
        }

        public String JdeToken { get; set; }
        public int AuthMode { get; set; }

        public AuthService AuthService { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (filterContext.Result == null)
            {

                string actionName = filterContext.ActionDescriptor.ActionName;
                string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

                String urlAction = String.Format("/{0}/{1}", controllerName, actionName);

                Function currentFunction = Context.Functions.Where(w => w.Url == urlAction).FirstOrDefault();

                if (currentFunction == null)
                {
                    currentFunction = Context.Functions.FirstOrDefault();
                }
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    ApplicationUser currentUser = GetCurrentUser();
                    CurrentUserJde = Context.UserOrigins.Where(w => w.IdUser == currentUser.UserId && w.IdOrigin == 1).FirstOrDefault();
                    ViewBag.CurrentUserJde = CurrentUserJde;

                    if (AuthMode == 1)
                    {
                        bool redirectToJdeLogin = false;
                        if (CurrentUserJde == null)
                        {
                            //Devo effettuare l'autenticazione
                            redirectToJdeLogin = true;
                        }
                        else
                        {
                            //Genero il token
                            String serverUrl = System.Configuration.ConfigurationManager.AppSettings["ServerUrl"];
                            SendRequestResponse<AuthResponseDto> response = AuthService.Authenticate(Context, serverUrl + "/jderest/tokenrequest", currentFunction.Data2, currentFunction.Data1, CurrentUserJde.Username, CurrentUserJde.Password, GetCurrentUserId());
                            if (response.Unauthorized)
                            {
                                throw new UnauthorizedException();
                            }
                            if (response.Result == false)
                            {
                                throw new UnauthorizedException();
                            }
                            JdeToken = response.Payload.UserInfo.Token;
                        }


                        if (redirectToJdeLogin)
                        {
                            var vm = new JdeLoginViewModel() { ReturnUrl = urlAction, Function = currentFunction, IdFunction = currentFunction.FunctionId };
                            ViewModelBase model = vm as ViewModelBase;
                            AssignTranslations(model);

                            filterContext.Result = new ViewResult
                            {
                                ViewName = "LoginJde",
                                ViewData = new ViewDataDictionary(filterContext.Controller.ViewData)
                                {
                                    Model = vm
                                }
                            };
                        }
                    }
                    else
                    {
                        //Autenticazione con Token
                        JdeToken = String.Empty;
                    }
                }

            }
            
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            if (CurrentUserJde != null)
            {
                if (AuthMode == 1)
                {
                    if (!String.IsNullOrEmpty(JdeToken))
                    {
                        String serverUrl = System.Configuration.ConfigurationManager.AppSettings["ServerUrl"];
                        SendRequestResponse<ValidateTokenResponseDto> authResponse = AuthService.Logout(Context,  serverUrl + "/jderest/v2/tokenrequest/logout", JdeToken, GetCurrentUserId());
                    }
                }
            }
        }

        public ActionResult LoginJde(String returnUrl)
        {
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            
            String urlAction = String.Format("/{0}/{1}", controllerName, "Index");

            Function currentFunction = Context.Functions.Where(w => w.Url == urlAction).FirstOrDefault();


            var vm = new JdeLoginViewModel() { ReturnUrl = "/" + controllerName + "/Index", Function = currentFunction, IdFunction = currentFunction.FunctionId };
            ViewModelBase model = vm as ViewModelBase;
            AssignTranslations(model);

            return View("LoginJde", vm);
            
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            
            if (filterContext.Exception is UnauthorizedException)
            {
                filterContext.ExceptionHandled = true;
                var controllerName = filterContext.RouteData.Values["controller"];
                var actionName = "Index";

                

                String urlAction = String.Format("/{0}/{1}", controllerName, actionName);

                Function currentFunction = Context.Functions.Where(w => w.Url == urlAction).FirstOrDefault();

                var vm = new JdeLoginViewModel() { ReturnUrl = urlAction, Function = currentFunction, IdFunction = currentFunction.FunctionId };
                ViewModelBase model = vm as ViewModelBase;
                AssignTranslations(model);

                filterContext.Result = new ViewResult
                {
                    ViewName = "LoginJde",
                    ViewData = new ViewDataDictionary(filterContext.Controller.ViewData)
                    {
                        Model = vm
                    }
                };
            }
        }

        public UserOrigin CurrentUserJde { get; set; }
    }
}