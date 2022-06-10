using Ica.SalesOrders.Models;
using Ica.SalesOrders.Web.Models.DataTables;
using Ica.SalesOrders.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Ica.SalesOrders.Web.Models.DataTables;
using Ica.SalesOrders.Web.Extensions;
using Ica.SalesOrders.Web.Models;
using System.Data.Entity;
using System.Data;
namespace Ica.SalesOrders.Web.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UsersController : BaseController
    {
        public ActionResult Index()
        {
            return View(new ListUserViewModel());
        }

        public ActionResult List(DataTableOption option)
        {
            var query = Context.Users.AsQueryable();
            for (int i = 0; i < option.sSearchs.Count; i++)
            {
                String value = option.sSearchs[i];
                String field = option.mDataProp[i];
                if (!String.IsNullOrEmpty(value))
                {
                    if (field.Equals("id"))
                    {
                      
                    }
                }
            }
            

            Func<IQueryable<ApplicationUser>, IQueryable<ApplicationUser>> func =
                (x) =>
                {
                    return x.Where(w => w.Name.Contains(option.sSearch)
                   );
                };
            return GenericDataTableList(query, func, option, new List<String>() { "id" },null);
        }

        public ActionResult DownloadExcel()
        {

            String countries = String.Empty;
            /*
            DataTable list = Context.GetExcelUsers(countries);

            byte[] res = CsvService.ExportToCsv(Context, GetCurrentUserId(), list);

            
            return File(res, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Users.xlsx");*/
            return null;
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ApplicationUser currentUser = null;
           
                currentUser = Context.Users.Find(id);
            

            EditUserViewModel euvm = EditUserViewModel.Load(Context, currentUser);
            return PartialView(euvm);
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection values)
        {
            OperationResult<object> res = new OperationResult<object>();
            ApplicationUser u = new ApplicationUser();
            TryUpdateModel(u, "User", values);
            u.UserId = id;

            /*
            if (String.IsNullOrEmpty(u.Password))
            {
                ModelState.AddModelError("User.Password", Translate("UserManagement", "PasswordRequired"));
            }
            */
            if (String.IsNullOrEmpty(u.LoginName))
            {
                ModelState.AddModelError("User.LoginName", Translate("UserManagement","LoginNameRequired"));
            }
            else
            {
                ApplicationUser alreadyExist = Context.Users.Where(w => w.LoginName.Equals(u.LoginName) && w.UserId != u.UserId).FirstOrDefault();
                if (alreadyExist != null)
                {
                    ModelState.AddModelError("User.LoginName", Translate("UserManagement","LoginNameAlreadyExist")); 
                }
            }

            if (String.IsNullOrEmpty(u.Name))
            {
                ModelState.AddModelError("User.Name", Translate("UserManagement", "NameRequired"));
            }

            if (u.RoleId == 0)
            {
                ModelState.AddModelError("User.RoleId", Translate("UserManagement", "RoleRequired"));
            }

                       
            ApplicationUser oldUser = Context.Users.Find(id);

            if (ModelState.IsValid)
            {
                try
                {
                    if (oldUser != null)
                    {
                        oldUser.Name = u.Name;
                        oldUser.RoleId = u.RoleId;
                        oldUser.Password = String.Empty;// u.Password;
                        oldUser.LoginName = u.LoginName;
                        oldUser.Enabled = u.Enabled;
                        oldUser.CodiceE1 = u.CodiceE1;
                    }
                    else
                    {
                        u.Password = String.Empty;
                        Context.Users.Add(u);
                    }
                    Context.SaveChanges();

                    res.Message = Translate("Shared", "OperationComplete");
                    String role = Context.Roles.Where(w => w.RoleId.Equals(u.RoleId)).FirstOrDefault().RoleDescription;
                    String logMessage = String.Format("{0} User {1} - Assigned Role {2} - Status {3}", id == 0 ? "Created" : "Modified", u.UserId, role, u.Enabled ? "Enabled" : "Disabled");
                    Context.AddLog((short)LogActionEnum.EditUser, u.UserId.ToString(), null, GetCurrentUserId(), logMessage);
                }
                catch (Exception ex)
                {
                    res.Result = false;
                    res.Message = Translate("Shared", "UnexpectedError");
                    Context.AddLog((short)LogActionEnum.Exception, null, null, GetCurrentUserId(), "USers/Edit : " + ex.Message);
                }
            }
            else
            {
                res.Result = false;
            }

            u.Role = Context.Roles.Find(u.RoleId);
            EditUserViewModel euvm = EditUserViewModel.Load(Context, u);
            
            euvm.Result = res;
            return PartialView(euvm);
            
        }
    }
}