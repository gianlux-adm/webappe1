using Ica.SalesOrders.Core.Http;
using Ica.SalesOrders.Jde.Dto;
using Ica.SalesOrders.Models;
using Ica.SalesOrders.Web.Models;
using Ica.SalesOrders.Web.Models.DataTables;
using Ica.SalesOrders.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Ica.SalesOrders.Web.Extensions;

namespace Ica.SalesOrders.Web.Controllers
{
    [Authorize]
    public class WarehouseController : JdeController
    {
        public const string WAREHOUSE_SEARCH_FORM = "P55ICAP1_W55ICAP1A";

        private List<ArticoloDto> ParseArticoli(SendRequestResponse<SearchFormResponseDto> res)
        {
            List<ArticoloDto> list = new List<ArticoloDto>();
            foreach (var row in res.Payload.FormData.Data.GridData.Righe)
            {
                var codiceArticoloValue = row.Values.Where(w => w.ColumnName == "z_LITM_19").FirstOrDefault();
                var descrizioneValue = row.Values.Where(w => w.ColumnName == "z_DSC1_26").FirstOrDefault();
                var descrizione2Vlue = row.Values.Where(w => w.ColumnName == "z_DSC2_27").FirstOrDefault();

                ArticoloDto art = new ArticoloDto();
                art.CodiceArticolo = codiceArticoloValue.Value;
                art.Descrizione = descrizioneValue.Value + descrizione2Vlue.Value.Trim();
                list.Add(art);
            }
            return list;
        }

        public ActionResult GetArticoli(String query, int type)
        {
            List<ArticoloDto> list = new List<ArticoloDto>();
            try
            {
                if (1 == 1)
                {
                list = GetArticoliFromForm(query, type);
            }
           
            }
            catch (Exception ex)
            {

            }

            return Json(list, JsonRequestBehavior.AllowGet);
}

        private List<ArticoloDto> GetArticoliFromForm(string query, int type)
        {
            String version = GetVersion(WAREHOUSE_SEARCH_FORM);
            List<ArticoloDto> list = new List<ArticoloDto>();
            SendRequestResponse<SearchFormResponseDto> res = null;
            FormServiceDto req = new FormServiceDto();

            if (!String.IsNullOrEmpty(JdeToken))
            {
                req.Token = JdeToken;
            }

            req.FormName = "P55ICAP1_W55ICAP1A";
//            req.Version = version;
            req.OutputType = "VERSION2";
            req.ReturnControlIDs = "1[19,26,27]";

            if (type == 0)
            {
             
                

                req.Query = new FormServiceQueryDto();
                req.Query.AutoFind = true;
                req.Query.Conditions = new List<FormServiceQueryConditionDto>();
                
                    req.Query.Conditions.Add(new FormServiceQueryConditionDto() { QueryOperator = "STR_CONTAIN", QueryField = "1[19]", Value = new List<FormServiceQueryConditionValueDto>() { new FormServiceQueryConditionValueDto() { SpecialValueId = "LITERAL", Content = query.ToUpper() } } });
                
                //req.MaxPageSize = 10;

                req.FormServiceAction = "R";
                req.BypassFormServiceEREvent = false;

                String serverUrl = System.Configuration.ConfigurationManager.AppSettings["ServerUrl"];
                res = SalesOrders.Core.Http.SendRecordService.SendRequest<SearchFormResponseDto>(Context,"POST",  serverUrl + "/jderest/v2/formservice", req, "fs_P55ICAP1_W55ICAP1A",GetCurrentUserId(),(short)LogActionEnum.WarehouseAutocomplete);

                if (res.Unauthorized)
                {
                    throw new UnauthorizedException();
                }

                if (res.Result)
                {
                    list.AddRange(ParseArticoli(res));
                }

            }
            else
            {
                req.Query.Conditions.Add(new FormServiceQueryConditionDto() { QueryOperator = "STR_CONTAIN", QueryField = "1[26]", Value = new List<FormServiceQueryConditionValueDto>() { new FormServiceQueryConditionValueDto() { SpecialValueId = "LITERAL", Content = query } } });


                
                    String serverUrl = System.Configuration.ConfigurationManager.AppSettings["ServerUrl"];
                    res = SalesOrders.Core.Http.SendRecordService.SendRequest<SearchFormResponseDto>(Context,"POST",  serverUrl + "/jderest/v2/formservice", req, "fs_P55ICAP1_W55ICAP1A", GetCurrentUserId(), (short)LogActionEnum.WarehouseAutocomplete);

                    if (res.Unauthorized)
                    {
                        throw new UnauthorizedException();
                    }
                    if (res.Result)
                    {
                        list.AddRange(ParseArticoli(res));
                    }
            }

            return list;

        }

        private List<ArticoloDto> GetArticoliFromFormOld(string query, int type)
        {
            String version = GetVersion(WAREHOUSE_SEARCH_FORM);
            List<ArticoloDto> list = new List<ArticoloDto>();
            SendRequestResponse<SearchFormResponseDto> res = null;
            SearchFormRequestDto req = null;
            if (type == 0)
            {
                req = new SearchFormRequestDto();

                if (!String.IsNullOrEmpty(JdeToken))
                {
                    req.Token = JdeToken;
                }
                else
                {
                    req.Username = CurrentUserJde.Username;
                    req.Password = CurrentUserJde.Password;
                }
                req.FormName = "P55ICAP1_W55ICAP1A";
                req.Version = version;
                req.Action = "open";
                req.OutputType = "VERSION2";
                req.EnableNextPageProcessing = false;
                req.FormRequest = new FormRequestDto();
                req.FormRequest.MaxPageSize = 10;
                req.FormRequest.ReturnControlIds = "1[19,26,27]";

                bool useQuery = true;
                if (useQuery)
                {
                    req.FormRequest.Query = new FormServiceQueryDto();
                    req.FormRequest.Query.AutoFind = true;
                    req.FormRequest.Query.Conditions = new List<FormServiceQueryConditionDto>();
                    req.FormRequest.Query.Conditions.Add(new FormServiceQueryConditionDto() { QueryField = "1[19]", QueryOperator = "STR_CONTAIN", Value = new List<FormServiceQueryConditionValueDto>() { new FormServiceQueryConditionValueDto() { Content = query } } });
                }
                else
                {
                    req.FormRequest.FormActions = new List<FormActionDto>();
                    req.FormRequest.FormActions.Add(new FormActionDto() { Command = "SetQBEValue", ControlID = "1[19]", Value = "%" + query + "%" });
                    req.FormRequest.FormActions.Add(new FormActionDto() { Command = "DoAction", ControlID = "15" });

                }

                req.FormRequest.FormName = "P55ICAP1_W55ICAP1A";
                req.FormRequest.Version = version;
                req.FormRequest.FormServiceAction = "R";
                req.FormRequest.BypassFormServiceErEvent = true;
                req.StackId = 0;
                req.StateId = 0;
                String serverUrl = System.Configuration.ConfigurationManager.AppSettings["ServerUrl"];
                res = SalesOrders.Core.Http.SendRecordService.SendRequest<SearchFormResponseDto>(Context, "POST", serverUrl + "/jderest/v2/appstack", req, "fs_P55ICAP1_W55ICAP1A", GetCurrentUserId(), (short)LogActionEnum.WarehouseAutocomplete);

                if (res.Unauthorized)
                {
                    throw new UnauthorizedException();
                }

                if (res.Result)
                {
                    list.AddRange(ParseArticoli(res));
                }

            }
            else
            {

                req = new SearchFormRequestDto();

                if (!String.IsNullOrEmpty(JdeToken))
                {
                    req.Token = JdeToken;
                }
                else
                {
                    req.Username = CurrentUserJde.Username;
                    req.Password = CurrentUserJde.Password;
                }
                req.FormName = "P55ICAP1_W55ICAP1A";
                req.Version = version;
                req.Action = "open";
                req.OutputType = "VERSION2";
                req.EnableNextPageProcessing = false;
                req.FormRequest = new FormRequestDto();
                req.FormRequest.MaxPageSize = 10;
                req.FormRequest.ReturnControlIds = "1[19,26,27]";
                req.FormRequest.FormActions = new List<FormActionDto>();
                bool useQuery = true;
                if (useQuery)
                {
                    req.FormRequest.Query = new FormServiceQueryDto();
                    req.FormRequest.Query.AutoFind = true;
                    req.FormRequest.Query.Conditions = new List<FormServiceQueryConditionDto>();
                    req.FormRequest.Query.Conditions.Add(new FormServiceQueryConditionDto() { QueryField = "1[26]", QueryOperator = "STR_CONTAIN", Value = new List<FormServiceQueryConditionValueDto>() { new FormServiceQueryConditionValueDto() { Content = query } } });
                }
                else
                {
                    req.FormRequest.FormActions.Add(new FormActionDto() { Command = "SetQBEValue", ControlID = "1[26]", Value = "%" + query + "%" });
                    req.FormRequest.FormActions.Add(new FormActionDto() { Command = "DoAction", ControlID = "15" });
                }
                req.FormRequest.FormName = "P55ICAP1_W55ICAP1A";
                req.FormRequest.Version = version;
                req.FormRequest.FormServiceAction = "R";
                req.FormRequest.BypassFormServiceErEvent = true;
                req.StackId = 0;
                req.StateId = 0;
                String serverUrl = System.Configuration.ConfigurationManager.AppSettings["ServerUrl"];
                res = SalesOrders.Core.Http.SendRecordService.SendRequest<SearchFormResponseDto>(Context, "POST", serverUrl + "/jderest/v2/appstack", req, "fs_P55ICAP1_W55ICAP1A", GetCurrentUserId(), (short)LogActionEnum.WarehouseAutocomplete);

                if (res.Unauthorized)
                {
                    throw new UnauthorizedException();
                }
                if (res.Result)
                {
                    list.AddRange(ParseArticoli(res));
                }
            }

            return list;

        }


        private DataServiceRequestDto GetDefaultDdlRequest(string token,string level)
        {
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

            request.TableName = "F0005";
            request.Query = new DataServiceQueryDto();
            request.Query.Conditions = new List<DataServiceQueryConditionDto>();
            request.Query.Conditions.Add(new DataServiceQueryConditionDto()
            {
                Value = new DataServiceQueryConditionValueDto() { Content = "41" },
                QueryField = "F0005.SY",
                QueryOperator = "EQUAL"
            });
            request.Query.Conditions.Add(new DataServiceQueryConditionDto()
            {
                Value = new DataServiceQueryConditionValueDto() { Content = level },
                QueryField = "F0005.RT",
                QueryOperator = "EQUAL"
            });

            return request;
        }

        public ActionResult Index()
        {
            string version = GetVersion(WAREHOUSE_SEARCH_FORM);

            WarehouseIndexViewModel vm = new WarehouseIndexViewModel();
            DataServiceRequestDto request = GetDefaultDdlRequest(CurrentUserJde.Token,"S1");
            SendRequestResponse<DataServiceResponseDto>  response = GetDataService(request, LogActionEnum.WarehouseDDL);

            if (response.Result)
            {
                vm.ValoriCategoria1 = GetValoriCategoria(response);
            }
            vm.VersioneForm = version;

            return View(vm);
        }


        public ActionResult GetResults(string searchId,String mcu,String availability,String codiceArticolo,string descrizione,
            string categoria1,string categoria2,string categoria3,string categoria4,DataTableOption option)
        {
            try
            {
                String version = GetVersion(WAREHOUSE_SEARCH_FORM);
                ApplicationUser currentUser = GetCurrentUser();
                String currentCacheSearchId = String.Format("Warehouse|{0}|{1}", currentUser.UserId, searchId);

                List<Articolo> list = null;
                var cacheData = HttpContext.Cache[currentCacheSearchId];
                if (cacheData == null)
                {
                    //Effettuo la chiamata

                    WarehouseSearchViewModel vm = new WarehouseSearchViewModel();
                    //Effettuo la richiesta
                    SearchFormRequestDto req = new SearchFormRequestDto();

                    if (!String.IsNullOrEmpty(JdeToken))
                    {
                        req.Token = JdeToken;
                    }
                    else
                    {
                        req.Username = CurrentUserJde.Username;
                        req.Password = CurrentUserJde.Password;
                    }

                    
                    req.Version = version;
                    req.FormName = "P55ICAP1_W55ICAP1A";
                    req.Action = "open";
                    req.OutputType = "VERSION2";
                    req.EnableNextPageProcessing = false;
                    req.FormRequest = new FormRequestDto();

                    //req.FormRequest.ReturnControlIds = "1[19,26,27,39]";
                    req.FormRequest.ReturnControlIds = null;
                    req.FormRequest.FormActions = new List<FormActionDto>();
                    bool filter = false;
                    /*
                    if (!String.IsNullOrEmpty(mcu))
                    {
                        req.FormRequest.FormActions.Add(new FormActionDto() { Command = "SetControlValue", ControlID = "60", Value = "" });

                    }*/
                    if (!String.IsNullOrEmpty(codiceArticolo))
                    {
                        filter = true;
                        req.FormRequest.FormActions.Add(new FormActionDto() { Command = "SetQBEValue", ControlID = "1[19]", Value = codiceArticolo });
                    }

                    if (!String.IsNullOrEmpty(descrizione))
                    {
                        filter = true;
                        req.FormRequest.FormActions.Add(new FormActionDto() { Command = "SetQBEValue", ControlID = "1[20]", Value = descrizione });
                    }

                    if (!String.IsNullOrEmpty(categoria1))
                    {
                        req.FormRequest.FormActions.Add(new FormActionDto() { Command = "SetQBEValue", ControlID = "1[28]", Value = categoria1 });
                    }
                    if (!String.IsNullOrEmpty(categoria2))
                    {
                        req.FormRequest.FormActions.Add(new FormActionDto() { Command = "SetQBEValue", ControlID = "1[29]", Value = categoria2 });
                    }
                    if (!String.IsNullOrEmpty(categoria3))
                    {
                        filter = true;
                        req.FormRequest.FormActions.Add(new FormActionDto() { Command = "SetQBEValue", ControlID = "1[30]", Value = categoria3 });
                    }
                    if (!String.IsNullOrEmpty(categoria4))
                    {
                        filter = true;
                        req.FormRequest.FormActions.Add(new FormActionDto() { Command = "SetQBEValue", ControlID = "1[31]", Value = categoria4 });
                    }


                    if (filter)
                    {
                        req.FormRequest.MaxPageSize = int.MaxValue;
                    }
                    else
                    {
                        req.FormRequest.MaxPageSize = 10;
                    }

                    req.FormRequest.FormActions.Add(new FormActionDto() { Command = "DoAction", ControlID = "15" });
                    req.FormRequest.FormName = "P55ICAP1_W55ICAP1A";
                    req.FormRequest.Version = version;
                    req.FormRequest.FormServiceAction = "R";
                    req.FormRequest.BypassFormServiceErEvent = true;
                    req.StackId = 0;
                    req.StateId = 0;

                    list = new List<Articolo>();
                    if (filter)
                    {
                        String serverUrl = System.Configuration.ConfigurationManager.AppSettings["ServerUrl"];
                        var response = SalesOrders.Core.Http.SendRecordService.SendRequest<SearchFormResponseDto>(Context, "POST", serverUrl + "/jderest/v2/appstack", req, "fs_P55ICAP1_W55ICAP1A", GetCurrentUserId(), (short)LogActionEnum.WarehouseSearch);

                        if (response.Unauthorized)
                        {
                            throw new UnauthorizedException();
                        }

                        if (response.Result)
                        {
                            if (!String.IsNullOrEmpty(availability))
                            {
                                foreach (var row in response.Payload.FormData.Data.GridData.Righe)
                                {
                                    var rowAvailability = row.Values.Where(w => w.ColumnName == "z_EV01_39").FirstOrDefault();

                                    string rowAvailabilityValue = rowAvailability.Value;
                                    if (availability.Equals(rowAvailabilityValue, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        var rowArticolo = row.Values.Where(w => w.ColumnName == "z_LITM_19").FirstOrDefault();
                                        var rowDescrizione = row.Values.Where(w => w.ColumnName == "z_DSC1_26").FirstOrDefault();
                                        Articolo art = new Articolo();
                                        art.CodiceArticolo = rowArticolo.Value;
                                        art.Descrizione = rowDescrizione.Value;
                                        art.Disponibilita = rowAvailabilityValue;
                                        art.RowIndex = row.RowIndex;
                                        list.Add(art);
                                    }
                                }
                            }
                            else
                            {
                                foreach (var item in response.Payload.FormData.Data.GridData.Righe)
                                {
                                    var rowAvailability = item.Values.Where(w => w.ColumnName == "z_EV01_39").FirstOrDefault();
                                    var rowArticolo = item.Values.Where(w => w.ColumnName == "z_LITM_19").FirstOrDefault();
                                    var rowDescrizione = item.Values.Where(w => w.ColumnName == "z_DSC1_26").FirstOrDefault();
                                    Articolo art = new Articolo();
                                    art.CodiceArticolo = rowArticolo.Value;
                                    art.Descrizione = rowDescrizione.Value;
                                    art.Disponibilita = rowAvailability.Value;
                                    art.RowIndex = item.RowIndex;
                                    list.Add(art);
                                }
                            }

                            //Aggiungo la cache 
                            HttpContext.Cache.Add(String.Format("Warehouse|{0}|{1}", currentUser.UserId, searchId), list, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(10), CacheItemPriority.Normal, null);
                        }
                        else
                        {
                            return Json(new { Success = false, Message = "Errore nella chiamata a JDE verificare nei log"  }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    list = cacheData as List<Articolo>;
                }


                int sortCol = option.iSortCols[0];
                DataTablesSortDirection direction = option.sSortDirs[0];
                String orderColumn = String.Empty;
                String sort = direction == DataTablesSortDirection.Ascending ? "asc" : "desc";
                switch (sortCol)
                {
                    case 1:
                        orderColumn = "CodiceArticolo";
                        break;
                    case 2:
                        orderColumn = "Descrizione";
                        break;
                    case 3:
                        orderColumn = "Disponibilita";
                        break;
                }


                var results = list.AsQueryable().OrderBy(orderColumn, sort).Skip(option.iDisplayStart).Take(option.iDisplayLength);
                var result = new DataTableResultExt("",
                    results.Count(), list.Count(),
                    results.Select(s => s.ToDataTablesRow()).ToList()
                    );

                result.WarningMessage = String.Empty;
                result.ContentEncoding = Encoding.UTF8;
                return result;
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdateDetail(string availability,string articolo, string mcu)
        {
            String version = GetVersion(WAREHOUSE_SEARCH_FORM);
            var currentUser = GetCurrentUser();

            SendRequestResponse<SearchFormResponseDto> response = GetWarehouseDetail(version, mcu, articolo);

            if (response.Unauthorized)
            {
                throw new UnauthorizedException();
            }
            WarehouseDetailViewModel vm = new WarehouseDetailViewModel();

            if (response.Result)
            {
                List<Articolo> articoli = new List<Articolo>();
                foreach (var riga in response.Payload.FormData.Data.GridData.Righe)
                {
                    Articolo art = new Articolo();
                    art.Parse(riga);
                    switch (availability)
                    {
                        case "Y":
                            if (art.QtaDisponibile > 0)
                            {
                                articoli.Add(art);
                            }
                            break;
                        case "N":
                            if (art.QtaDisponibile == 0)
                            {
                                articoli.Add(art);
                            }
                            break;
                        default:
                            articoli.Add(art);
                            break;
                    }
                    
                    
                    
                }
                if (articoli.Count == 0)
                {
                    if (response.Payload.FormData.Data.GridData.Righe.Count() > 0)
                    {
                        Articolo art = new Articolo();
                        art.Parse(response.Payload.FormData.Data.GridData.Righe[0]);
                        articoli.Add(art);
                    }
                    else
                    {
                        Articolo art = new Articolo();
                        art.CodiceArticolo = articolo;
                        articoli.Add(art);
                    }
                }
                vm.Articoli = articoli;
                vm.MCU = mcu;
                vm.DetailAvailability = availability;
                vm.Result = new OperationResult<object>();
                vm.Result.Result = true;
            }
            else
            {
                vm.Result = new OperationResult<object>();
                vm.Result.Result = false;
                vm.Result.Message = "TODO ERRORE";
            }
            return Json(new { Success = vm.Result.Result, Message = vm.Result.Message,  Articoli = vm.Articoli }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int id,string searchId)
        {
            String version = GetVersion(WAREHOUSE_SEARCH_FORM);
            ApplicationUser currentUser = GetCurrentUser();
            String currentCacheSearchId = String.Format("Warehouse|{0}|{1}", currentUser.UserId, searchId);

            List<Articolo> list = null;
            var cacheData = HttpContext.Cache[currentCacheSearchId];

            if (cacheData != null)
            {
                list = (List<Articolo>)cacheData;
            }
            else
            {
                //restituire errore
            }

            Articolo currentRow = list.Where(w => w.RowIndex == id).FirstOrDefault();

            //Effettuo la richiesta
            SendRequestResponse<SearchFormResponseDto> response = GetWarehouseDetail(version,"60", currentRow.CodiceArticolo);

            if (response.Unauthorized)
            {
                throw new UnauthorizedException();
            }
            WarehouseDetailViewModel vm = new WarehouseDetailViewModel();

            if (response.Result)
            {
                List<Articolo> articoli = new List<Articolo>();
                foreach (var riga in response.Payload.FormData.Data.GridData.Righe)
                {
                    Articolo art = new Articolo();
                    art.Parse(riga);
                            if (art.QtaDisponibile > 0)
                            {
                                articoli.Add(art);
                            }
                }
                if (response.Payload.FormData.Data.GridData.Righe.Count() == 0)
                {
                    articoli.Add(currentRow);
                }

                vm.Articoli = articoli;

            }
            else
            {
                vm.Result = new OperationResult<object>();
                vm.Result.Result = false;
                vm.Result.Message = "TODO ERRORE";
            }
            return PartialView("_Details", vm);


        }

        private SendRequestResponse<SearchFormResponseDto> GetWarehouseDetail(string version,string mcu, String codiceArticolo)
        {
            SearchFormRequestDto req = new SearchFormRequestDto();

            if (!String.IsNullOrEmpty(JdeToken))
            {
                req.Token = JdeToken;
            }
            else
            {
                req.Username = CurrentUserJde.Username;
                req.Password = CurrentUserJde.Password;
            }


            req.FormName = "P55ICAP1_W55ICAP1B";
            req.Version = version;
            req.Action = "open";
            req.OutputType = "VERSION2";
            req.EnableNextPageProcessing = false;
            req.FormRequest = new FormRequestDto();
            req.FormRequest.MaxPageSize = int.MaxValue;
            //req.FormRequest.ReturnControlIds = "54|1[45,46,47,49,306]";
            req.FormRequest.FormActions = new List<FormActionDto>();
            
            if (!String.IsNullOrEmpty(mcu))
            {
                req.FormRequest.FormActions.Add(new FormActionDto() { Command = "SetControlValue", ControlID = "60", Value = "" });

            }
            if (!String.IsNullOrEmpty(codiceArticolo))
            {
                req.FormRequest.FormActions.Add(new FormActionDto() { Command = "SetQBEValue", ControlID = "1[23]", Value = codiceArticolo });
            }

            req.FormRequest.FormActions.Add(new FormActionDto() { Command = "DoAction", ControlID = "15" });
            req.FormRequest.FormName = "P55ICAP1_W55ICAP1B";
            req.FormRequest.Version = version;
            req.FormRequest.FormServiceAction = "R";
            req.FormRequest.BypassFormServiceErEvent = true;
            req.StackId = 0;
            req.StateId = 0;
            String serverUrl = System.Configuration.ConfigurationManager.AppSettings["ServerUrl"];
            var response = SalesOrders.Core.Http.SendRecordService.SendRequest<SearchFormResponseDto>(Context, "POST",  serverUrl + "/jderest/v2/appstack", req, "fs_P55ICAP1_W55ICAP1B", GetCurrentUserId(), (short)LogActionEnum.WarehouseSearch);
            return response;
        }


      

        private Dictionary<string, string> GetValoriCategoria(SendRequestResponse<DataServiceResponseDto> response)
        {
            Dictionary<String, String> dict = new Dictionary<string, string>();
            foreach (var row in response.Payload.Content.Data.GridData.Righe)
            {
                var key = row.Values.Where(w => w.ColumnName == "F0005_KY").FirstOrDefault();
                var description = row.Values.Where(w => w.ColumnName == "F0005_DL01").FirstOrDefault();
                if (!String.IsNullOrEmpty(key.Value.Trim()))
                {
                    dict.Add(key.Value.Trim(), description.Value);
                }
            }
            return dict;
        }

       

        public ActionResult GetDDL(string cat1,string cat2,string cat3, string mode)
        {
            DataServiceRequestDto request = null;
            
            switch (mode)
            {
                case "Categoria2":
                    request = GetDefaultDdlRequest(CurrentUserJde.Token,"S2");
                    request.Query.Conditions.Add(new DataServiceQueryConditionDto()
                    {
                        QueryField = "F0005.DL02",
                        Value = new DataServiceQueryConditionValueDto() { Content = cat1 },
                        QueryOperator = "STR_CONTAIN"
                    });
                    break;
                case "Categoria3":
                    request = GetDefaultDdlRequest(CurrentUserJde.Token, "S3");
                    request.Query.Conditions.Add(new DataServiceQueryConditionDto()
                    {
                        QueryField = "F0005.DL02",
                        Value = new DataServiceQueryConditionValueDto() { Content = cat2 },
                        QueryOperator = "STR_CONTAIN"
                    });
                    break;
                case "Categoria4":
                    request = GetDefaultDdlRequest(CurrentUserJde.Token, "S4");
                    request.Query.Conditions.Add(new DataServiceQueryConditionDto()
                    {
                        QueryField = "F0005.DL02",
                        Value = new DataServiceQueryConditionValueDto() { Content = cat3 },
                        QueryOperator = "STR_CONTAIN"
                    });
                    break;
            }
            SendRequestResponse<DataServiceResponseDto> response = GetDataService(request, LogActionEnum.WarehouseDDL);

            Dictionary<String, String> dict = new Dictionary<string, string>();
            if (response.Result)
            {
                dict = GetValoriCategoria(response);
            }


            return Json(dict.Select(s=>new { Key = s.Key, Value = s.Value }).ToList(), JsonRequestBehavior.AllowGet);
        }

    }
}