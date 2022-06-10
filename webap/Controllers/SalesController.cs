using Ica.SalesOrders.Core.Http;
using Ica.SalesOrders.Jde.Dto;
using Ica.SalesOrders.Models;
using Ica.SalesOrders.Web.Models;
using Ica.SalesOrders.Web.Models.DataTables;
using Ica.SalesOrders.Web.ViewModels;
using Ica.SalesOrders.Web.ViewModels.Sales;
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
    public class SalesController : JdeController
    {
        public const string CUSTOMER_SEARCH_FORM = "P0101S_W0101SA";
        public const string COLOR_SERACH_FORM = "P0004A_W0004AA";
        public const string SALES_SEARCH_FORM = "P55IC42I_W55IC42IA";
        // GET: Sales
        public ActionResult Index()
        {
            List<MappingColor> colorList = GetColorsList();
            SalesIndexViewModel vm = new SalesIndexViewModel();
            vm.VersioneForm = GetVersion(SALES_SEARCH_FORM);
            vm.NumeroOrdineCliente = "19002232";
            return View(vm);
        }

        private List<MappingColor> GetColorsList()
        {
            List<MappingColor> colorList = HttpContext.Cache["COLORS_MAPPINGS"] as List<MappingColor>;
            if (colorList == null)
            {
                //Aggiungo la cache 
                //Sincronizzo la cache con il server
                SendRequestResponse<SearchFormResponseDto> res = null;
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
                req.FormName = COLOR_SERACH_FORM;
                req.Version = String.Empty;
                req.Action = "open";
                req.OutputType = "VERSION2";
                req.EnableNextPageProcessing = false;
                req.FormRequest = new FormRequestDto();
                req.FormRequest.MaxPageSize = 2000;
                //req.FormRequest.ReturnControlIds = "1[28,7]";
                req.FormRequest.FormActions = new List<FormActionDto>();
                req.FormRequest.FormActions.Add(new FormActionDto() { Command = "SetControlValue", ControlID = "16", Value = "55IC" });
                req.FormRequest.FormActions.Add(new FormActionDto() { Command = "SetControlValue", ControlID = "18", Value = "I0" });
                req.FormRequest.FormActions.Add(new FormActionDto() { Command = "DoAction", ControlID = "22" });
                req.FormRequest.FormName = COLOR_SERACH_FORM;
                req.FormRequest.Version = "ZJDE0001";// 
                req.FormRequest.FormServiceAction = "R";
                req.FormRequest.BypassFormServiceErEvent = true;
                req.StackId = 0;
                req.StateId = 0;
                String serverUrl = System.Configuration.ConfigurationManager.AppSettings["ServerUrl"];
                res = SalesOrders.Core.Http.SendRecordService.SendRequest<SearchFormResponseDto>(Context, "POST", serverUrl + "/jderest/v2/appstack", req, "fs_P0004A_W0004AA", GetCurrentUserId(), (short)LogActionEnum.WarehouseAutocomplete);

                if (res.Result)
                {
                    foreach(var riga in res.Payload.FormData.Data.GridData.Righe)
                    {
                        var name =  riga.Values.Where(w => w.ColumnName == "z_DL01_11").FirstOrDefault().Value;
                        var codiceColore = riga.Values.Where(w => w.ColumnName == "z_KY_10").FirstOrDefault().Value;
                        var exMap = Context.MappingColors.Where(w => w.ColorCode == codiceColore).FirstOrDefault();
                        if (exMap == null)
                        {
                            MappingColor mc = new MappingColor();
                            mc.ColorCode = codiceColore;
                            mc.ColorDescription = name;
                            mc.ColorHex = "#000000";
                            Context.MappingColors.Add(mc);
                            Context.SaveChanges();
                        }
                    }
                }
                
                colorList = Context.MappingColors.ToList();
                HttpContext.Cache.Add("COLORS_MAPPINGS", colorList, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(60), CacheItemPriority.Normal, null);
            }
            return colorList;
        }

        public ActionResult GetClienti(String query)
        {
            List<ClienteDto> list = new List<ClienteDto>();
            try
            {
                list = GetCustomersQuery(query);
            }
            catch (Exception ex)
            {

            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }


        private List<ClienteDto> ParseClienti(SendRequestResponse<SearchFormResponseDto> res)
        {
            List<ClienteDto> list = new List<ClienteDto>();
            foreach (var row in res.Payload.FormData.Data.GridData.Righe)
            {
                var codiceIndirizzo = row.Values.Where(w => w.ColumnName == "z_AN8_7").FirstOrDefault();
                var descrizioneValue = row.Values.Where(w => w.ColumnName == "z_ALPH_8").FirstOrDefault();

                ClienteDto art = new ClienteDto();
                art.CodiceCliente = codiceIndirizzo.Value;
                art.DescrizioneCliente = descrizioneValue.Value;
                list.Add(art);
            }
            return list;
        }


        private List<ClienteDto> GetCustomersQuery(string query)
        {
            String version = GetVersion(CUSTOMER_SEARCH_FORM).ToString();
            List<ClienteDto> list = new List<ClienteDto>();
            SendRequestResponse<SearchFormResponseDto> res = null;
            SearchFormRequestDto req = null;

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
            req.FormName = CUSTOMER_SEARCH_FORM;
            req.Version = String.Empty;
            req.Action = "open";
            req.OutputType = "VERSION2";
            req.EnableNextPageProcessing = false;
            req.FormRequest = new FormRequestDto();
            req.FormRequest.MaxPageSize = 10;
            bool useQuery = true;
            if (useQuery)
            {
                req.FormRequest.Query = new FormServiceQueryDto();
                req.FormRequest.Query.AutoFind = true;
                req.FormRequest.Query.Conditions = new List<FormServiceQueryConditionDto>();
                int numberCustomer = 0;
                if (int.TryParse(query, out numberCustomer))
                {
                    req.FormRequest.Query.Conditions.Add(new FormServiceQueryConditionDto() { QueryOperator = "EQUAL", QueryField = "1[7]", Value = new List<FormServiceQueryConditionValueDto>() { new FormServiceQueryConditionValueDto() { SpecialValueId = "LITERAL", Content = query } } });
                }
                else
                {
                    req.FormRequest.Query.Conditions.Add(new FormServiceQueryConditionDto() { QueryOperator = "STR_CONTAIN", QueryField = "1[8]", Value = new List<FormServiceQueryConditionValueDto>() { new FormServiceQueryConditionValueDto() { SpecialValueId = "LITERAL", Content = query.ToUpper() } } });
                }
            }
            else
            {
                //req.FormRequest.ReturnControlIds = "1[28,7]";
                req.FormRequest.FormActions = new List<FormActionDto>();
                req.FormRequest.FormActions.Add(new FormActionDto() { Command = "SetQBEValue", ControlID = "1[8]", Value = "%" + query + "%" });
                req.FormRequest.FormActions.Add(new FormActionDto() { Command = "DoAction", ControlID = "13" });
            }
                req.FormRequest.FormName = CUSTOMER_SEARCH_FORM;
            req.FormRequest.Version = String.Empty;// "ZJDE0002";// GetVersion(CUSTOMER_SEARCH_FORM);
            req.FormRequest.FormServiceAction = "R";
            req.FormRequest.BypassFormServiceErEvent = true;
            req.StackId = 0;
            req.StateId = 0;
            String serverUrl = System.Configuration.ConfigurationManager.AppSettings["ServerUrl"];
            res = SalesOrders.Core.Http.SendRecordService.SendRequest<SearchFormResponseDto>(Context, "POST", serverUrl + "/jderest/v2/appstack", req, "fs_P0101S_W0101SA", GetCurrentUserId(), (short)LogActionEnum.WarehouseAutocomplete);

            if (res.Unauthorized)
            {
                throw new UnauthorizedException();
            }

            if (res.Result)
            {

                list.AddRange(ParseClienti(res));
            }
            return list;

        }

        public ActionResult DownloadScheda(int id, string type)
        {
            string error = String.Empty;
            try
            {
                SchedeService.SchedeClient clientWs = new SchedeService.SchedeClient();
                var res = clientWs.GetScheda(id, type);
                return new FileStreamResult(res, "application/pdf");
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return View("NoFile", new ViewModelBase() { Result = new OperationResult<object>() { Result= false, Message = error } });
        }

        public ActionResult GetSchede(string id)
        {
            String articleWithoutRevision = id;
            if (id.LastIndexOf("-") > -1)
            {
                articleWithoutRevision = id.Substring(0, id.LastIndexOf("-"));
            }
             
            DettaglioSchedeViewModel vm = new DettaglioSchedeViewModel();
            SchedeService.SchedeClient clientWs = new SchedeService.SchedeClient();
            vm.Schede =  clientWs.GetSchede(articleWithoutRevision).ToList();
            vm.CodiceArticolo = id;
            return PartialView("_Schede",vm);
        }

        public ActionResult GetResultsOld(string searchId, int searchType, String codiceArticolo, string codiceCliente, string NumeroOrdineCliente, DataTableOption option)
        {
            try
            {
                var colorList = GetColorsList();
                String version = GetVersion(SALES_SEARCH_FORM);
                ApplicationUser currentUser = GetCurrentUser();
                String currentCacheSearchId = String.Format("Sales|{0}|{1}", currentUser.UserId, searchId);

                List<SaleResult> list = null;
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
                    req.FormName = SALES_SEARCH_FORM;
                    req.Action = "open";
                    req.OutputType = "VERSION2";
                    req.EnableNextPageProcessing = false;
                    req.FormRequest = new FormRequestDto();
                    req.FormRequest.ReturnControlIds = "1[120,187,101,146,142,174,183,181,185,177,184,151]";
                    //req.FormRequest.ReturnControlIds = null;
//                    req.FormRequest.MaxPageSize = 1;// option.iDisplayLength;
  //                  req.EnableNextPageProcessing = true;
                    bool useQuery = true;
                    if (useQuery)
                    {
                        req.FormRequest.Query = new FormServiceQueryDto();
                        req.FormRequest.Query.AutoFind = true;
                        req.FormRequest.Query.Conditions = new List<FormServiceQueryConditionDto>();
                        if (!String.IsNullOrEmpty(NumeroOrdineCliente))
                        {
                            req.FormRequest.Query.Conditions.Add(new FormServiceQueryConditionDto() { QueryOperator = "EQUAL", QueryField = "1[81]", Value = new List<FormServiceQueryConditionValueDto>() { new FormServiceQueryConditionValueDto() { SpecialValueId = "LITERAL", Content = NumeroOrdineCliente } } });
                        }
                        if (!String.IsNullOrEmpty(codiceArticolo))
                        {
                            req.FormRequest.Query.Conditions.Add(new FormServiceQueryConditionDto() { QueryOperator = "EQUAL", QueryField = "1[101]", Value = new List<FormServiceQueryConditionValueDto>() { new FormServiceQueryConditionValueDto() { SpecialValueId = "LITERAL", Content = codiceArticolo } } });
                        }
                        if (!String.IsNullOrEmpty(codiceCliente))
                        {
                            req.FormRequest.Query.Conditions.Add(new FormServiceQueryConditionDto() { QueryOperator = "EQUAL", QueryField = "1[71]", Value = new List<FormServiceQueryConditionValueDto>() { new FormServiceQueryConditionValueDto() { SpecialValueId = "LITERAL", Content = codiceCliente } } });
                        }

                        
                        switch (searchType)
                        {
                            case 0: //Data Oggi
                                req.FormRequest.Query.Conditions.Add(new FormServiceQueryConditionDto() { QueryOperator = "EQUAL", QueryField = "1[120]", Value = new List<FormServiceQueryConditionValueDto>() { new FormServiceQueryConditionValueDto() { SpecialValueId = "TODAY", Content = "0" } } });
                                break;
                            case 1: //Data spedizione promessa prossimi 7 giorni
                                    req.FormRequest.Query.Conditions.Add(new FormServiceQueryConditionDto() { QueryOperator = "BETWEEN", QueryField = "1[120]", Value = new List<FormServiceQueryConditionValueDto>() { new FormServiceQueryConditionValueDto() { SpecialValueId = "TODAY", Content = "0" }, new FormServiceQueryConditionValueDto() { SpecialValueId = "TODAY_PLUS_DAY", Content = "7" } } });
                                break;
                            case 2: //Spedito ultimi 7 giorni
                                req.FormRequest.Query.Conditions.Add(new FormServiceQueryConditionDto() { QueryOperator = "BETWEEN", QueryField = "1[120]", Value = new List<FormServiceQueryConditionValueDto>() { new FormServiceQueryConditionValueDto() { SpecialValueId = "TODAY_MINUS_DAY", Content = "7" }, new FormServiceQueryConditionValueDto() { SpecialValueId = "TODAY", Content = "0" } } });
                                break;
                        }
                        
                    }
                    else
                    {
                        req.FormRequest.FormActions = new List<FormActionDto>();
                        if (!String.IsNullOrEmpty(NumeroOrdineCliente))
                        {
                            req.FormRequest.FormActions.Add(new FormActionDto() { Command = "SetQBEValue", ControlID = "1[81]", Value = NumeroOrdineCliente });
                        }
                        if (!String.IsNullOrEmpty(codiceArticolo))
                        {
                            req.FormRequest.FormActions.Add(new FormActionDto() { Command = "SetQBEValue", ControlID = "1[101]", Value = codiceArticolo });
                        }
                        /*
                        switch (searchType)
                        {
                            case 0: //Data Oggi
                                req.FormRequest.FormActions.Add(new FormActionDto() { Command = "SetQBEValue", ControlID = "1[120]", Value = String.Format("{0:dd/MM/yyyy}", DateTime.Now.Date) });
                                break;
                            case 1: //Data spedizione promessa prossimi 7 giorni
                                req.FormRequest.FormActions.Add(new FormActionDto() { Command = "SetQBEValue", ControlID = "1[120]", Value = ">=" + String.Format("{0:dd/MM/yyyy}", DateTime.Now.Date) });
                                req.FormRequest.FormActions.Add(new FormActionDto() { Command = "SetQBEValue", ControlID = "1[120]", Value = "<=" + String.Format("{0:dd/MM/yyyy}", DateTime.Now.Date.AddDays(7)) });
                                break;
                            case 2: //Spedito ultimi 7 giorni
                                req.FormRequest.FormActions.Add(new FormActionDto() { Command = "SetQBEValue", ControlID = "1[151]", Value = ">=" + String.Format("{0:dd/MM/yyyy}", DateTime.Now.Date.AddDays(-7)) });
                                break;
                        }*/
                        req.FormRequest.FormActions.Add(new FormActionDto() { Command = "DoAction", ControlID = "15" });
                    }

                    req.FormRequest.MaxPageSize = 2000;

                    
                    req.FormRequest.FormName = SALES_SEARCH_FORM;
                    req.FormRequest.Version = version;
                    req.FormRequest.FormServiceAction = "R";
                    req.FormRequest.BypassFormServiceErEvent = true;
                    req.StackId = 0;
                    req.StateId = 0;

                    list = new List<SaleResult>();
                    //if (filter)
                    //{
                    String serverUrl = System.Configuration.ConfigurationManager.AppSettings["ServerUrl"];
                    //" 
                    var response = SalesOrders.Core.Http.SendRecordService.SendRequest<SearchFormResponseDto>(Context, "POST", serverUrl + "/jderest/v2/appstack", req, "fs_" + SALES_SEARCH_FORM, GetCurrentUserId(), (short)LogActionEnum.WarehouseSearch);

                    if (response.Unauthorized)
                    {
                        throw new UnauthorizedException();
                    }

                    if (response.Result)
                    {
                        foreach (var row in response.Payload.FormData.Data.GridData.Righe)
                        {
                            SaleResult sr = new SaleResult();
                            var expecetedDateShipping = row.Values.Where(w => w.ColumnName == "z_PPDJ_120").FirstOrDefault();
                            sr.ShippingDateExpected = JavaTimeStampToDateTime(double.Parse(expecetedDateShipping.InternalValue));
                            var recipient = row.Values.Where(w => w.ColumnName == "z_DL01_187").FirstOrDefault();
                            sr.Recipient = recipient.Value;
                            var articleCode = row.Values.Where(w => w.ColumnName == "z_LITM_101").FirstOrDefault();
                            sr.ArticleCode = articleCode.Value;
                            var quantity = row.Values.Where(w => w.ColumnName == "z_UORG_146").FirstOrDefault();
                            sr.Quantity = decimal.Parse(quantity.Value);
                            var unitOfMeasure = row.Values.Where(w => w.ColumnName == "z_UOM_142").FirstOrDefault();
                            sr.UnitOfMeasure = unitOfMeasure.Value;
                            var saleOrderStatusColor = row.Values.Where(w => w.ColumnName == "z_EV01_174").FirstOrDefault();
                            var currentSaleOrderStatusColor = colorList.Where(w => w.ColorCode == saleOrderStatusColor.Value).FirstOrDefault();
                            sr.SaleOrderStatusColor = currentSaleOrderStatusColor == null ? string.Empty : currentSaleOrderStatusColor.ColorHex;

                            
                            var saleOrderStatus = row.Values.Where(w => w.ColumnName == "z_DL01_183").FirstOrDefault();
                            sr.SaleOrderStatus = saleOrderStatus.Value;
                            var productionOrderColor = row.Values.Where(w => w.ColumnName == "z_SPHD_181").FirstOrDefault();
                            
                            var currentProductionOrderColor = colorList.Where(w => w.ColorCode == productionOrderColor.Value).FirstOrDefault();
                            sr.ProductionOrderColor = currentProductionOrderColor == null ? string.Empty : currentProductionOrderColor.ColorHex;

                            var productionOrderStatus = row.Values.Where(w => w.ColumnName == "z_DL01_185").FirstOrDefault();
                            sr.ProductionOrderStatus = productionOrderStatus.Value;

                            var transferOrderColor = row.Values.Where(w => w.ColumnName == "z_EV01_177").FirstOrDefault();
                            var currentTransferOrderColor = colorList.Where(w => w.ColorCode == transferOrderColor.Value).FirstOrDefault();
                            sr.TransferOrderColor = currentTransferOrderColor==null ? string.Empty : currentTransferOrderColor.ColorHex;

                            var transferOrderStatus = row.Values.Where(w => w.ColumnName == "z_DL01_184").FirstOrDefault();
                            sr.TransferOrderStatus = transferOrderStatus.Value;


                            

                            sr.RowIndex = row.RowIndex;
                            list.Add(sr);

                        }


                        //Aggiungo la cache 
                        HttpContext.Cache.Add(String.Format("Sales|{0}|{1}", currentUser.UserId, searchId), list, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(10), CacheItemPriority.Normal, null);
                    }
                    else
                    {
                        return Json(new { Success = false, Message = "Errore nella chiamata a JDE verificare nei log" }, JsonRequestBehavior.AllowGet);
                    }
                    //}
                }
                else
                {
                    list = cacheData as List<SaleResult>;
                }


                int sortCol = option.iSortCols[0];
                DataTablesSortDirection direction = option.sSortDirs[0];
                String orderColumn = String.Empty;
                String sort = direction == DataTablesSortDirection.Ascending ? "asc" : "desc";
                switch (sortCol)
                {
                    case 1:
                        orderColumn = "ShippingDateExpected";
                        break;
                    case 2:
                        orderColumn = "Recipient";
                        break;
                    case 3:
                        orderColumn = "ArticleCode";
                        break;
                    case 4:
                        orderColumn = "Quantity";
                        break;
                    case 5:
                        orderColumn = "UnitOfMeasure";
                        break;
                    case 6:
                        orderColumn = "SaleOrderStatus";
                        break;
                    case 7:
                        orderColumn = "ProductionOrderStatus";
                        break;
                    case 8:
                        orderColumn = "TransferOrderStatus";
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


        public ActionResult GetResults(string searchId, int searchType, String codiceArticolo, string codiceCliente, string NumeroOrdineCliente, DataTableOption option)
        {
            try
            {
                var colorList = GetColorsList();
                String version = GetVersion(SALES_SEARCH_FORM);
                ApplicationUser currentUser = GetCurrentUser();
                String currentCacheSearchId = String.Format("Sales|{0}|{1}", currentUser.UserId, searchId);

                List<SaleResult> list = null;
                var cacheData = HttpContext.Cache[currentCacheSearchId];
                if (cacheData == null)
                {
                    //Effettuo la chiamata
                    WarehouseSearchViewModel vm = new WarehouseSearchViewModel();
                    //Effettuo la richiesta

                    FormServiceDto req = new FormServiceDto();

                    if (!String.IsNullOrEmpty(JdeToken))
                    {
                        req.Token = JdeToken;
                    }
                    else
                    {
                      //  req.Username = CurrentUserJde.Username;
                       // req.Password = CurrentUserJde.Password;
                    }

                    req.Version = version;
                    req.FormName = SALES_SEARCH_FORM;
                    //req.Action = "open";
                    req.OutputType = "VERSION2";
                    //req.EnableNextPageProcessing = false;
                    //req.FormRequest = new FormRequestDto();
                    req.ReturnControlIDs = "1[120,187,101,146,142,174,183,181,185,177,184,151]";
                    //req.FormRequest.ReturnControlIds = null;
                    //                    req.FormRequest.MaxPageSize = 1;// option.iDisplayLength;
                    //                  req.EnableNextPageProcessing = true;
                    
                        req.Query = new FormServiceQueryDto();
                        req.Query.AutoFind = true;
                        req.Query.Conditions = new List<FormServiceQueryConditionDto>();
                        if (!String.IsNullOrEmpty(NumeroOrdineCliente))
                        {
                            req.Query.Conditions.Add(new FormServiceQueryConditionDto() { QueryOperator = "EQUAL", QueryField = "1[81]", Value = new List<FormServiceQueryConditionValueDto>() { new FormServiceQueryConditionValueDto() { SpecialValueId = "LITERAL", Content = NumeroOrdineCliente } } });
                        }
                        if (!String.IsNullOrEmpty(codiceArticolo))
                        {
                            req.Query.Conditions.Add(new FormServiceQueryConditionDto() { QueryOperator = "EQUAL", QueryField = "1[101]", Value = new List<FormServiceQueryConditionValueDto>() { new FormServiceQueryConditionValueDto() { SpecialValueId = "LITERAL", Content = codiceArticolo } } });
                        }
                        if (!String.IsNullOrEmpty(codiceCliente))
                        {
                            req.Query.Conditions.Add(new FormServiceQueryConditionDto() { QueryOperator = "EQUAL", QueryField = "1[71]", Value = new List<FormServiceQueryConditionValueDto>() { new FormServiceQueryConditionValueDto() { SpecialValueId = "LITERAL", Content = codiceCliente } } });
                        }


                        switch (searchType)
                        {
                            case 0: //Data Oggi
                                req.Query.Conditions.Add(new FormServiceQueryConditionDto() { QueryOperator = "EQUAL", QueryField = "1[120]", Value = new List<FormServiceQueryConditionValueDto>() { new FormServiceQueryConditionValueDto() { SpecialValueId = "TODAY", Content = "0" } } });
                                break;
                            case 1: //Data spedizione promessa prossimi 7 giorni
                                req.Query.Conditions.Add(new FormServiceQueryConditionDto() { QueryOperator = "BETWEEN", QueryField = "1[120]", Value = new List<FormServiceQueryConditionValueDto>() { new FormServiceQueryConditionValueDto() { SpecialValueId = "TODAY", Content = "0" }, new FormServiceQueryConditionValueDto() { SpecialValueId = "TODAY_PLUS_DAY", Content = "7" } } });
                                break;
                            case 2: //Spedito ultimi 7 giorni
                                req.Query.Conditions.Add(new FormServiceQueryConditionDto() { QueryOperator = "BETWEEN", QueryField = "1[120]", Value = new List<FormServiceQueryConditionValueDto>() { new FormServiceQueryConditionValueDto() { SpecialValueId = "TODAY_MINUS_DAY", Content = "7" }, new FormServiceQueryConditionValueDto() { SpecialValueId = "TODAY", Content = "0" } } });
                                break;
                        }

                                       req.MaxPageSize = 2000;


                    req.FormServiceAction = "R";
                    req.BypassFormServiceEREvent = false;
                    
                    list = new List<SaleResult>();
                    //if (filter)
                    //{
                    String serverUrl = System.Configuration.ConfigurationManager.AppSettings["ServerUrl"];
                    //" / jderest/v2/appstack"
                    var response = SalesOrders.Core.Http.SendRecordService.SendRequest<SearchFormResponseDto>(Context, "POST", serverUrl + "/jderest/v2/formservice", req, "fs_" + SALES_SEARCH_FORM, GetCurrentUserId(), (short)LogActionEnum.WarehouseSearch);

                    if (response.Unauthorized)
                    {
                        throw new UnauthorizedException();
                    }

                    if (response.Result)
                    {
                        foreach (var row in response.Payload.FormData.Data.GridData.Righe)
                        {
                            SaleResult sr = new SaleResult();
                            var expecetedDateShipping = row.Values.Where(w => w.ColumnName == "z_PPDJ_120").FirstOrDefault();
                            sr.ShippingDateExpected = JavaTimeStampToDateTime(double.Parse(expecetedDateShipping.InternalValue));
                            var recipient = row.Values.Where(w => w.ColumnName == "z_DL01_187").FirstOrDefault();
                            sr.Recipient = recipient.Value;
                            var articleCode = row.Values.Where(w => w.ColumnName == "z_LITM_101").FirstOrDefault();
                            sr.ArticleCode = articleCode.Value;
                            var quantity = row.Values.Where(w => w.ColumnName == "z_UORG_146").FirstOrDefault();
                            sr.Quantity = decimal.Parse(quantity.Value);
                            var unitOfMeasure = row.Values.Where(w => w.ColumnName == "z_UOM_142").FirstOrDefault();
                            sr.UnitOfMeasure = unitOfMeasure.Value;
                            var saleOrderStatusColor = row.Values.Where(w => w.ColumnName == "z_EV01_174").FirstOrDefault();
                            var currentSaleOrderStatusColor = colorList.Where(w => w.ColorCode == saleOrderStatusColor.Value).FirstOrDefault();
                            sr.SaleOrderStatusColor = currentSaleOrderStatusColor == null ? string.Empty : currentSaleOrderStatusColor.ColorHex;


                            var saleOrderStatus = row.Values.Where(w => w.ColumnName == "z_DL01_183").FirstOrDefault();
                            sr.SaleOrderStatus = saleOrderStatus.Value;
                            var productionOrderColor = row.Values.Where(w => w.ColumnName == "z_SPHD_181").FirstOrDefault();

                            var currentProductionOrderColor = colorList.Where(w => w.ColorCode == productionOrderColor.Value).FirstOrDefault();
                            sr.ProductionOrderColor = currentProductionOrderColor == null ? string.Empty : currentProductionOrderColor.ColorHex;

                            var productionOrderStatus = row.Values.Where(w => w.ColumnName == "z_DL01_185").FirstOrDefault();
                            sr.ProductionOrderStatus = productionOrderStatus.Value;

                            var transferOrderColor = row.Values.Where(w => w.ColumnName == "z_EV01_177").FirstOrDefault();
                            var currentTransferOrderColor = colorList.Where(w => w.ColorCode == transferOrderColor.Value).FirstOrDefault();
                            sr.TransferOrderColor = currentTransferOrderColor == null ? string.Empty : currentTransferOrderColor.ColorHex;

                            var transferOrderStatus = row.Values.Where(w => w.ColumnName == "z_DL01_184").FirstOrDefault();
                            sr.TransferOrderStatus = transferOrderStatus.Value;




                            sr.RowIndex = row.RowIndex;
                            list.Add(sr);

                        }


                        //Aggiungo la cache 
                        HttpContext.Cache.Add(String.Format("Sales|{0}|{1}", currentUser.UserId, searchId), list, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(10), CacheItemPriority.Normal, null);
                    }
                    else
                    {
                        return Json(new { Success = false, Message = "Errore nella chiamata a JDE verificare nei log" }, JsonRequestBehavior.AllowGet);
                    }
                    //}
                }
                else
                {
                    list = cacheData as List<SaleResult>;
                }

                int colIdx = 0;
                foreach(var columnFilterData in option.sSearchs)
                {
                    if (!String.IsNullOrEmpty(columnFilterData))
                    {
                        
                    
                    switch (colIdx)
                    {
                        case 1:
                            DateTime currentDateShipping = DateTime.ParseExact(columnFilterData, "dd/MM/yy", null);
                            list = list.Where(w => w.ShippingDateExpected.Date == currentDateShipping.Date).ToList();
                            break;
                        case 2:
                                list = list.Where(w => w.Recipient == columnFilterData).ToList();
                        break;
                            case 3:
                                list = list.Where(w => w.ArticleCode == columnFilterData).ToList();
                                break;
                            case 4:
                                decimal currentQty = decimal.Parse(columnFilterData);
                                list = list.Where(w => w.Quantity == currentQty).ToList();
                                break;
                            case 5:
                                list = list.Where(w => w.UnitOfMeasure == columnFilterData).ToList();
                                break;
                            case 6:
                                list = list.Where(w => w.SaleOrderStatus == columnFilterData).ToList();
                                break;
                            case 7:
                                list = list.Where(w => w.ProductionOrderStatus == columnFilterData).ToList();
                                break;
                            case 8:
                                list = list.Where(w => w.TransferOrderStatus == columnFilterData).ToList();
                                break;
                        }
                    }

                    colIdx++;
                }


                int sortCol = option.iSortCols[0];
                DataTablesSortDirection direction = option.sSortDirs[0];
                String orderColumn = String.Empty;
                String sort = direction == DataTablesSortDirection.Ascending ? "asc" : "desc";
                switch (sortCol)
                {
                    case 1:
                        orderColumn = "ShippingDateExpected";
                        break;
                    case 2:
                        orderColumn = "Recipient";
                        break;
                    case 3:
                        orderColumn = "ArticleCode";
                        break;
                    case 4:
                        orderColumn = "Quantity";
                        break;
                    case 5:
                        orderColumn = "UnitOfMeasure";
                        break;
                    case 6:
                        orderColumn = "SaleOrderStatus";
                        break;
                    case 7:
                        orderColumn = "ProductionOrderStatus";
                        break;
                    case 8:
                        orderColumn = "TransferOrderStatus";
                        break;
                }


                //Aggiungo i filtri
                Dictionary<int, List<DataTableColumnFilterValues>> filters = new Dictionary<int, List<DataTableColumnFilterValues>>();
                filters.Add(1, list.Select(s => s.ShippingDateExpected).Distinct().OrderBy(o=>o).Select(s => new DataTableColumnFilterValues() { Value = String.Format("{0:dd/MM/yy}", s), Label = String.Format("{0:dd/MM/yy}", s), }).ToList());
                filters.Add(2, list.Select(s => s.Recipient).Distinct().OrderBy(o => o).Select(s => new DataTableColumnFilterValues() { Value = s, Label = s }).ToList());
                filters.Add(3, list.Select(s => s.ArticleCode).Distinct().OrderBy(o => o).Select(s => new DataTableColumnFilterValues() { Value = s, Label = s }).ToList());
                filters.Add(4, list.Select(s => s.Quantity).Distinct().OrderBy(o => o).Select(s => new DataTableColumnFilterValues() { Value = String.Format("{0:0.##}", s), Label = String.Format("{0:0.##}", s) }).ToList());
                filters.Add(5, list.Select(s => s.UnitOfMeasure).Distinct().OrderBy(o => o).Select(s => new DataTableColumnFilterValues() { Value = s, Label = s }).ToList());
                filters.Add(6, list.Select(s => s.SaleOrderStatus).Distinct().OrderBy(o => o).Select(s => new DataTableColumnFilterValues() { Value = s, Label = s }).ToList());
                filters.Add(7, list.Select(s => s.ProductionOrderStatus).Distinct().OrderBy(o => o).Select(s => new DataTableColumnFilterValues() { Value = s, Label = s }).ToList());
                filters.Add(8, list.Select(s => s.TransferOrderStatus).Distinct().OrderBy(o => o).Select(s => new DataTableColumnFilterValues() { Value = s, Label = s }).ToList());



                var results = list.AsQueryable().OrderBy(orderColumn, sort).Skip(option.iDisplayStart).Take(option.iDisplayLength);
                var result = new DataTableResultExt("",
                    results.Count(), list.Count(),
                    results.Select(s => s.ToDataTablesRow()).ToList()
                    ,filters
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

    }
}