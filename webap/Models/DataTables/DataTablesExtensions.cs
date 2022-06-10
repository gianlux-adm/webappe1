using Ica.SalesOrders.Jde.Dto;
using Ica.SalesOrders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Ica.SalesOrders.Web.Models.DataTables
{
    public static class DataTablesExtensions
    {
        public static DataTablesRow ToDataTablesRow<T>(this T item)
        {
            Type currentType = item.GetType();
            if (currentType.ToString().Contains("System.Data.Entity.DynamicProxies")) {
                currentType = currentType.BaseType;
            }
            List<Type> invokePars = new List<Type>() { currentType };
            MethodInfo[] methods = typeof(DataTablesExtensions).GetMethods();
            MethodInfo selected = null;
            foreach (MethodInfo info in methods)
            {
                if (info.Name.Equals("ToDataTablesRow"))
                {
                    var parameters = info.GetParameters();
                    if ((parameters != null) && (parameters.Count() > 0))
                    {
                        if (parameters.FirstOrDefault().ParameterType.Equals(currentType))
                        {
                            selected = info;
                            break;
                        }
                    }
                }

            }
            if (selected == null)
            {
                return null;
            }
            //MethodInfo methodInfo = currentType.GetMethod("ToDataTablesRow");
            object[] pars = new object[1];
            pars[0] = item;
            return selected.Invoke(null, pars) as DataTablesRow;
        }

        public static DataTablesRow ToDataTablesRow(this GridRowDto item)
        {
            DataTablesRow row = new DataTablesRow();
            row.Add("id", item.RowIndex);
            foreach(var column in item.Values)
            {
                row.Add(column.ColumnName,column.Value);
            }
            
            return row;
        }

        public static DataTablesRow ToDataTablesRow(this Articolo item)
        {
            DataTablesRow row = new DataTablesRow();
            row.Add("id", item.RowIndex);
            row.Add("CodiceArticolo", item.CodiceArticolo);
            row.Add("Descrizione", item.Descrizione);
            row.Add("Disponibilita", item.Disponibilita);


            return row;
        }

        public static DataTablesRow ToDataTablesRow(this SaleResult item)
        {
            DataTablesRow row = new DataTablesRow();
            row.Add("id", item.RowIndex);
            row.Add("ArticleCode", item.ArticleCode);
            row.Add("ProductionOrderColor", item.ProductionOrderColor);
            row.Add("ProductionOrderStatus", item.ProductionOrderStatus);

            row.Add("Quantity", item.Quantity);
            row.Add("Recipient", item.Recipient);
            row.Add("SaleOrderStatus", item.SaleOrderStatus);
            row.Add("SaleOrderStatusColor", item.SaleOrderStatusColor);
            row.Add("ShippingDateExpected", String.Format("{0:dd/MM/yy}",item.ShippingDateExpected));
            row.Add("TransferOrderColor", item.TransferOrderColor);
            row.Add("TransferOrderStatus", item.TransferOrderStatus);
            row.Add("UnitOfMeasure", item.UnitOfMeasure);

            return row;
        }


        public static DataTablesRow ToDataTablesRow(this ApplicationUser item)
        {
            DataTablesRow row = new DataTablesRow();
            row.Add("id", item.UserId);
            row.Add("LoginName", item.LoginName);
            row.Add("Name", item.Name);
            row.Add("CodiceE1", item.CodiceE1);
            row.Add("Enabled", item.Enabled ? "Enabled" : "Disabled");
            row.Add("RoleId", item.Role.RoleDescription);
            return row;
        }

        public static DataTablesRow ToDataTablesRow(this LogEntry item)
        {
            DataTablesRow row = new DataTablesRow();
            row.Add("Data", String.Format("{0:dd/MM/yyyy HH':'mm':'ss}", item.Data));
            row.Add("LogActionName", item.LogActionName);
            row.Add("Message", item.Message);
            row.Add("Author", item.Author);

            return row;
        }

       

       
    }
}