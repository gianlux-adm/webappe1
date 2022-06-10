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
using Ica.SalesOrders.Web.WS;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Reflection;
using Ica.SalesOrders.Web.Extensions;
using System.Net;

namespace Ica.SalesOrders.Web.Controllers
{
    [Authorize]
    public class SalesOrdersController : BaseController
    {
        public SalesOrdersController()
        {

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
        }
        public ActionResult Index()
        {
            SearchSalesOrderViewModel vm = new SearchSalesOrderViewModel();
            vm.BusinessUnit = "01S01";
            vm.SoldTo = 706153;
            vm.DocumentType = "SO";
          
            return View(vm);
        }

        public ActionResult Results(String businessUnit, int? soldTo,string documentType)
        {
            if (String.IsNullOrEmpty(businessUnit))
            {
                ModelState.AddModelError("BusinessUnit", Translate("SalesOrders", "BusinessUnitMandatory"));
            }
            if (soldTo.HasValue==false)
            {
                ModelState.AddModelError("SoldTo", Translate("SalesOrders", "SoldToMandatory"));
            }
            if (String.IsNullOrEmpty(documentType))
            {
                ModelState.AddModelError("SoldTo", Translate("SalesOrders", "DocumentTypeMandatory"));
            }
            SalesOrderResultsViewModel vm = new SalesOrderResultsViewModel();
            if (ModelState.IsValid)
            {
                try
                {
                    vm.Result = new OperationResult<object>();
                    vm.Result.Result = true;
                    CustomBinding binding = new CustomBinding();

                    var security = TransportSecurityBindingElement.CreateUserNameOverTransportBindingElement();
                    security.IncludeTimestamp = false;
                    security.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256;
                    security.MessageSecurityVersion = MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10;

                    var encoding = new TextMessageEncodingBindingElement();
                    encoding.MessageVersion = MessageVersion.Soap11;

                    var transport = new HttpsTransportBindingElement();
                    transport.MaxReceivedMessageSize = 20000000; // 20 megs

                    binding.Elements.Add(security);
                    binding.Elements.Add(encoding);
                    binding.Elements.Add(transport);

                    SalesOrderManagerClient client = new SalesOrderManagerClient(binding, new EndpointAddress(
                        "https://ica-bssv1:9192/PD910/SalesOrderManager"
                        ));

                    // to use full client credential with Nonce uncomment this code:
                    // it looks like this might not be required - the service seems to work without it
                    client.ChannelFactory.Endpoint.Behaviors.Remove<System.ServiceModel.Description.ClientCredentials>();
                    client.ChannelFactory.Endpoint.Behaviors.Add(new IcaCredentials());

                    client.ClientCredentials.UserName.UserName = "TESTWS";
                    client.ClientCredentials.UserName.Password = "TESTWS";

                    getSalesOrderV3 input = new getSalesOrderV3();
                    input.header = new getSalesHeaderV3();
                    input.header.businessUnit = businessUnit;
                    input.header.soldTo = new entity();
                    input.header.soldTo.entityId = soldTo.Value;
                    input.header.soldTo.entityIdSpecified = true;
                    input.header.salesOrderKey = new salesOrderKey();
                    input.header.salesOrderKey.documentTypeCode = documentType;

                    client.Endpoint.Behaviors.Add(new MyBehavior());
                    showSalesOrderV4 results = client.getSalesOrderV4(input);

                    Session["SalesOrderV4"] = results;
                    var mappingList = Context.WsMappingTables.AsNoTracking().Where(w => w.Type == "GetSalesV4Master").OrderBy(o => o.Priority).ToList();

                    vm.Fields = mappingList;
                    foreach (var header in results.header)
                    {
                        GetSalesV4MasterRow row = new GetSalesV4MasterRow();
                        foreach (var mapping in mappingList)
                        {
                            GetSalesV4Field field = new GetSalesV4Field();
                            field.Name = mapping.Description;
                            field.Priority = mapping.Priority;
                            field.Value = GetMappingValue(header, mapping.Field);
                            field.Width = mapping.Width;
                            field.HtmlClass = mapping.HtmlClass;
                            row.Fields.Add(field);
                        }
                        vm.Rows.Add(row);
                    }
                }
                catch (Exception ex)
                {
                    vm.Result = new OperationResult<object>();
                    vm.Result.Result = false;
                    vm.Result.Message = Translate("SalesOrders", "SearchError");
                    vm.Result.Message += " " + ex.Message;
                }
                return PartialView("_Results", vm);
            }

            vm.Result = new OperationResult<object>();
            vm.Result.Result = false;
            vm.Result.Message = Translate("SalesOrders", "MandatoryFieldMissing");

            return PartialView("_NoResults",vm);
        }

        public ActionResult Details(String rowId)
        {
            String[] values = rowId.Split("|".ToCharArray());
            int orderNumber = int.Parse(values[0]);
            String orderTy = values[1];
            String orderCo = values[2];
            decimal lineNumber = decimal.Parse(values[3]);
            showSalesOrderV4 showSalesOrderV4 = (showSalesOrderV4)Session["SalesOrderV4"];

            showSalesHeaderV4 header = null;
            if (showSalesOrderV4 != null) {
                header  = showSalesOrderV4.header.Where(w => w.salesOrderKey.documentNumber.Equals(orderNumber)
            && w.salesOrderKey.documentTypeCode.Equals(orderTy)
            && w.salesOrderKey.documentCompany.Equals(orderCo)
            && w.detail[0].documentLineNumber.Equals(lineNumber)
            ).FirstOrDefault();
            }

            SalesOrderDetailViewModel vm = new SalesOrderDetailViewModel();
            vm.Result = new OperationResult<object>();
            if (header == null)
            {
                vm.Result.Result = false;
                vm.Result.Message = Translate("SalesOrders", "UnableToFoundDetailOrder");
            }
            else
            {
                vm.Result.Result = true;
                vm.OrderNumber = header.salesOrderKey.documentNumber;
                vm.OrderType = header.salesOrderKey.documentTypeCode;
                vm.OrderCompany = header.salesOrderKey.documentCompany;
                vm.OrderLineNumber = header.detail[0].documentLineNumber;
                vm.Plant = header.businessUnit;
                vm.SoldTo = header.detail[0].soldTo.entityId;
                vm.ShipTo = header.shipTo.shipTo.entityId + " " + header.shipTo.mailingName;
                vm.OrderDate = header.detail[0].dateOrdered;
                vm.CustomerPO = header.customerPO;
                vm.Currency = header.currencyCodeFrom;
                vm.ExchangeRate = header.rateExchangeOverride;


                var mappingList = Context.WsMappingTables.AsNoTracking().Where(w => w.Type == "GetSalesV4Detail").OrderBy(o => o.Priority).ToList();


                vm.Fields = mappingList;

                GetSalesV4MasterRow row = new GetSalesV4MasterRow();
                foreach (var mapping in mappingList)
                {
                    GetSalesV4Field field = new GetSalesV4Field();
                    field.Name = mapping.Description;
                    field.Priority = mapping.Priority;
                    field.Value = GetMappingValue(header, mapping.Field);
                    field.Width = mapping.Width;
                    field.HtmlClass = mapping.HtmlClass;
                    row.Fields.Add(field);
                }
                vm.Rows.Add(row);
            }

            return PartialView("_Details",vm);
        }

        private string GetMappingValue(showSalesHeaderV4 header, string field)
        {
            if (String.IsNullOrEmpty(field))
            {
                return String.Empty;
            }
            object ret = header.GetPropValue(field);
            Type returnType = ret.GetType();

            if (returnType == typeof(DateTime))
            {
                return String.Format("{0:dd/MM/yyyy}", ret);
            }
            

            return ret.ToString();
            
        }

        
    }
}