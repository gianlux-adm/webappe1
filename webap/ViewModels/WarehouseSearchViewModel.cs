using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ica.SalesOrders.Core.Http;
using Ica.SalesOrders.Jde.Dto;

namespace Ica.SalesOrders.Web.ViewModels
{
    public class WarehouseSearchViewModel : ViewModelBase
    {
        public WarehouseSearchViewModel()
        {
            Rows = new List<GridRowDto>();
        }
        public List<GridRowDto> Rows { get; set; }
        public int Page { get; set; }
        public bool HasNext { get; set; }
        public string NextUrl { get; set; }

        

        internal void ParseResponse(SendRequestResponse<SearchFormResponseDto> res, int page, int pageSize)
        {
            Rows = res.Payload.FormData.Data.GridData.Righe;
            Page = page;
            HasNext = false;
            var nextLink = res.Payload.Links.Where(w => w.Rel == "next").FirstOrDefault();
            if (nextLink != null)
            {
                HasNext = res.Payload.FormData.Data.GridData.Summary.MoreRecords;
                NextUrl = nextLink.Href;
            }
        }
    }
}