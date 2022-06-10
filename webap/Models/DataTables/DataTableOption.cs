using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Ica.SalesOrders.Web.Models.DataTables
{
    public class DataTableOption
    {
        public DataTableOption()
        {
            sSearchs = new List<String>();
            sColumns = new List<string>();
            iSortCols = new List<int>();
            sSortDirs = new List<DataTablesSortDirection>();
            mDataProp = new List<string>();
        }

        public String sEcho { get; set; }
        public int iDisplayStart { get; set; }
        public int iDisplayLength { get; set; }

        
        public int iColumns { get; set; }
        public string sSearch { get; set; }

        public List<String> sSearchs { get; set; }
        public List<String> mDataProp { get; set; }
        public List<String> sColumns { get; set; }
        public int iSortingCols { get; set; }

        public List<int> iSortCols { get; set; }


        public List<DataTablesSortDirection> sSortDirs { get; set; }

    }

    public class DataTableColumnFilterValues
    {
        
        public String Value { get; set; }
        public String Label { get; set; }
    }

    public enum DataTablesSortDirection
    {
        Ascending,
        Descending
    }

    public class DataTablesRow : Dictionary<string, object>
    {


        public DataTablesRow(string dtRowId = null, string dtRowClass = null)
            : this(dtRowId, dtRowClass, new Dictionary<string, object>())
        {
            this.DT_RowId = dtRowId;
            this.DT_RowClass = dtRowClass;
        }


        public DataTablesRow(string dtRowId, string dtRowClass, Dictionary<String, object> collection)
            : base(collection)
        {
            this.DT_RowId = dtRowId;
            this.DT_RowClass = dtRowClass;
        }




        public string DT_RowId
        {
            get;
            set;
        }

        public string DT_RowClass
        {
            get;
            set;
        }
    }

    public class DataTableResultExt : ActionResult
    {


        public Encoding ContentEncoding { get; set; }


        public string ContentType { get; set; }


        public JsonRequestBehavior JsonRequestBehavior { get; set; }


        public int iTotalDisplayRecords { get; set; }


        public int iTotalRecords { get; set; }


        public string sColumns { get; set; }


        public string sEcho { get; protected set; }

        public string WarningMessage { get; set; }


        public List<DataTablesRow> aaData { get; set; }
        public Dictionary<int, List<DataTableColumnFilterValues>> filters { get; set; }




        public DataTableResultExt(string sEcho = "", int iTotalRecords = 0, int iTotalDisplayRecords = 0, List<DataTablesRow> aaData = null,Dictionary<int,List<DataTableColumnFilterValues>> filters = null)
        {
            this.JsonRequestBehavior = JsonRequestBehavior.DenyGet;
            this.sEcho = sEcho;
            this.iTotalRecords = iTotalRecords;
            this.iTotalDisplayRecords = iTotalDisplayRecords;
            this.aaData = aaData;
            this.filters = filters;
        }


        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if ((this.JsonRequestBehavior == JsonRequestBehavior.DenyGet) &&
                string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Get not allowed");
            }

            HttpResponseBase response = context.HttpContext.Response;
            if (!string.IsNullOrEmpty(this.ContentType))
            {
                response.ContentType = this.ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }
            if (this.ContentEncoding != null)
            {
                response.ContentEncoding = this.ContentEncoding;
            }

            using (JsonWriter writer = new JsonTextWriter(response.Output))
            {
                writer.WriteStartObject();

                if (!String.IsNullOrEmpty(WarningMessage))
                {
                    writer.WritePropertyName("Message");
                    writer.WriteValue(this.WarningMessage);
                    writer.WritePropertyName("Class");
                    writer.WriteValue("alert-warning");
                }

                writer.WritePropertyName("sEcho");
                writer.WriteValue(this.sEcho);


                writer.WritePropertyName("iTotalRecords");
                writer.WriteValue(this.iTotalRecords);


                writer.WritePropertyName("iTotalDisplayRecords");
                writer.WriteValue(this.iTotalDisplayRecords);


                writer.WritePropertyName("aaData");
                writer.WriteStartArray();
                for (int i = 0; i < aaData.Count; i++)
                {
                    writer.WriteStartObject();
                    DataTablesRow row = aaData[i];
                    if (row.DT_RowId != null)
                    {
                        writer.WritePropertyName("DT_RowId");
                        writer.WriteValue(row.DT_RowId);
                    }


                    if (row.DT_RowClass != null)
                    {
                        writer.WritePropertyName("DT_RowClass");
                        writer.WriteValue(row.DT_RowClass);
                    }


                    foreach (String key in row.Keys)
                    {
                        writer.WritePropertyName(key);
                        writer.WriteValue(row[key]);
                    }
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();


                writer.WritePropertyName("filters");
                writer.WriteStartArray();
                if (filters != null)
                {
                    foreach (var key in filters.Keys)
                    {
                        writer.WriteStartObject();
                        List<DataTableColumnFilterValues> values =  filters[key];
                        writer.WritePropertyName("column");
                        writer.WriteValue(key.ToString());
                        writer.WritePropertyName("values");
                        writer.WriteStartArray();
                        foreach(var value in values)
                        {
                            writer.WriteStartObject();

                            writer.WritePropertyName("value");
                            writer.WriteValue(value.Value);

                            writer.WritePropertyName("label");
                            writer.WriteValue(value.Label);

                            writer.WriteEndObject();
                        }

                        writer.WriteEndArray();
                        writer.WriteEndObject();
                    }
                }
                writer.WriteEndArray();


                writer.WriteEndObject();
            }
        }
    }

    public class DataTableModelBinder : IModelBinder
    {
        #region IModelBinder Members


        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }




            if (bindingContext == null)
            {
                throw new ArgumentNullException("bindingContext");
            }


            DataTableOption dataTable = new DataTableOption();


            //see http://datatables.net/usage/server-side
            string sEcho = bindingContext.ValueProvider.GetValue("sEcho").AttemptedValue;
            if (string.IsNullOrEmpty(sEcho))
            {
                throw new ArgumentException("sEcho must always be provided");
            }
            dataTable.sEcho = sEcho;


            string iDisplayStartString = bindingContext.ValueProvider.GetValue("iDisplayStart").AttemptedValue;
            dataTable.iDisplayStart = int.Parse(iDisplayStartString);


            string iDisplayLengthString = bindingContext.ValueProvider.GetValue("iDisplayLength").AttemptedValue;
            dataTable.iDisplayLength = int.Parse(iDisplayLengthString);


            string iColumnsString = bindingContext.ValueProvider.GetValue("iColumns").AttemptedValue;
            dataTable.iColumns = int.Parse(iColumnsString);


            ValueProviderResult sSearchResult = bindingContext.ValueProvider.GetValue("sSearch");
            if (sSearchResult != null)
            {
                dataTable.sSearch = sSearchResult.AttemptedValue;
            }


            ValueProviderResult bEscapeRegexResult = bindingContext.ValueProvider.GetValue("bEscapeRegex");
            if (bEscapeRegexResult != null)
            {
                string bEscapeRegexString = bEscapeRegexResult.AttemptedValue;
                bool bEscapeRegex;
                if (!string.IsNullOrEmpty(bEscapeRegexString) &&
                     bool.TryParse(bEscapeRegexString, out bEscapeRegex))
                {
                    //dataTable.bEscapeRegex = bEscapeRegex;
                }
            }


            string sColumnsString = bindingContext.ValueProvider.GetValue("sColumns").AttemptedValue;
            if (!String.IsNullOrEmpty(sColumnsString))
            {
                dataTable.sColumns.AddRange(sColumnsString.Split(",".ToCharArray()));
            }

            for (int i = 0; i < dataTable.iColumns; i++)
            {
                bool bSortables = false;
                ValueProviderResult bSortableResult = bindingContext.ValueProvider.GetValue(string.Format("bSortable_{0}", i));
                if (bSortableResult != null)
                {
                    string bSortablesString = bSortableResult.AttemptedValue;
                    bool.TryParse(bSortablesString, out bSortables);
                }
                //dataTable.bSortables.Add(bSortables);
            }


            for (int i = 0; i < dataTable.iColumns; i++)
            {
                bool bSearchables = false;
                ValueProviderResult bSearchableResult = bindingContext.ValueProvider.GetValue(string.Format("bSearchable_{0}", i));
                if (bSearchableResult != null)
                {
                    string bSearchablesString = bSearchableResult.AttemptedValue;
                    bool.TryParse(bSearchablesString, out bSearchables);
                }
                //dataTable.bSearchables.Add(bSearchables);
            }


            for (int i = 0; i < dataTable.iColumns; i++)
            {
                string sSearchsString = string.Empty;
                ValueProviderResult sSearch_Result = bindingContext.ValueProvider.GetValue(string.Format("sSearch_{0}", i));
                if (sSearch_Result != null)
                {
                    sSearchsString = sSearch_Result.AttemptedValue;
                }
                dataTable.sSearchs.Add(sSearchsString);


                string mDataProp = string.Empty;
                ValueProviderResult mDataProp_Result = bindingContext.ValueProvider.GetValue(string.Format("mDataProp_{0}", i));
                if (mDataProp_Result != null)
                {
                    mDataProp = mDataProp_Result.AttemptedValue;
                }
                dataTable.mDataProp.Add(mDataProp);
            }


            for (int i = 0; i < dataTable.iColumns; i++)
            {
                bool bEscapeRegexs = false;
                ValueProviderResult bEscapeRegexsResult = bindingContext.ValueProvider.GetValue(string.Format("bEscapeRegex_{0}", i));
                if (bEscapeRegexsResult != null)
                {
                    string bEscapeRegexsString = bEscapeRegexsResult.AttemptedValue;
                    bool.TryParse(bEscapeRegexsString, out bEscapeRegexs);
                }
                //dataTable.bEscapeRegexs.Add(bEscapeRegexs);
            }



            string iSortingColsString = "0";
            if (bindingContext.ValueProvider.GetValue("iSortingCols") != null)
            {
                iSortingColsString = bindingContext.ValueProvider.GetValue("iSortingCols").AttemptedValue;
            }
            dataTable.iSortingCols = int.Parse(iSortingColsString);


            for (int i = 0; i < dataTable.iSortingCols; i++)
            {
                string iSortColsString = bindingContext.ValueProvider.GetValue(string.Format("iSortCol_{0}", i)).AttemptedValue;
                dataTable.iSortCols.Add(int.Parse(iSortColsString));
            }


            for (int i = 0; i < dataTable.iSortingCols; i++)
            {
                string sSortDirString = bindingContext.ValueProvider.GetValue(string.Format("sSortDir_{0}", i)).AttemptedValue;
                if (sSortDirString == "asc")
                {
                    dataTable.sSortDirs.Add(DataTablesSortDirection.Ascending);
                }
                else
                {
                    dataTable.sSortDirs.Add(DataTablesSortDirection.Descending);
                }
            }


            return dataTable;
        }


        #endregion
    }
}