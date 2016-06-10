using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Core.Infrastructure;
using WebApi.Core.DTO;
using WebApi.Core.Requests;

namespace WebApi.Controllers
{

    public class ApiControllerBase : System.Web.Http.ApiController
    {
        
        protected PageResponseDto<TEntity> CreatePageResponse<TEntity>(PagedRequestBase request,
            IEnumerable<TEntity> records, int numberTotalOfItems)
        {

            var totalPages = numberTotalOfItems / request.NumItems;
            if (numberTotalOfItems % request.NumItems != 0) totalPages++;

            int numPage = 1;
            if (request.Start > 0)
            {
                numPage = request.Start / request.NumItems + 1;
                if (request.Start % request.NumItems != 0) numPage++;
            }



            var pageResponseDto = new PageResponseDto<TEntity>
            {
                Records = records,
                ItemsPerPage = request.NumItems,
                Page = numPage,
                TotalItems = numberTotalOfItems,
                TotalPages = totalPages,
                StartIndex = request.Start,
                FirstPageUrl = CreateFirstPageUrl(request),
                LastPageUrl = CreateLastPageUrl(request, numberTotalOfItems),
                NextPageUrl = CreateNextPageUrl(request, numberTotalOfItems),
                PreviousPageUrl = CreatePreviousPageUrl(request)
            };
            return pageResponseDto;
        }

        private string CreatePreviousPageUrl(PagedRequestBase request)
        {
            if (request.Start == 0) return null;
            var startIndex = Math.Max(request.Start - request.NumItems, 0);
            return CreateUriForSearch(startIndex, request);
        }

        private string CreateNextPageUrl(PagedRequestBase request, int numberTotalOfItems)
        {
            if (request.Start + request.NumItems > numberTotalOfItems - 1) return null;
            int startIndex = request.Start + request.NumItems;
            return CreateUriForSearch(startIndex, request);
        }

        private string CreateLastPageUrl(PagedRequestBase request, int numberTotalOfItems)
        {
            if (request.Start + request.NumItems > numberTotalOfItems - 1) return null;
            int startIndex;
            if (numberTotalOfItems % request.NumItems == 0)
                startIndex = (numberTotalOfItems / request.NumItems - 1) * request.NumItems;
            else
                startIndex = (numberTotalOfItems / request.NumItems) * request.NumItems;
            return CreateUriForSearch(startIndex, request);
        }

        private string CreateFirstPageUrl(PagedRequestBase request)
        {
            if (request.Start == 0) return null;
            return CreateUriForSearch(0, request);
        }

        private string CreateUriForSearch(int startIndex, PagedRequestBase request)
        {
            var queryValues = this.Request.GetQueryNameValuePairs();
            var strBuild = new StringBuilder();
            strBuild.AppendFormat("?start={0}&numItems={1}", startIndex, request.NumItems);
            if (request.OrderingFields!=null && request.OrderingFields.Any())
            {
                strBuild.AppendFormat("&orderby={0}", request.OrderBy);
            }
            for (int i = 0; i < queryValues.Count(); i++)
            {
                var item = queryValues.ElementAt(i);
                if (item.Key.ToLower() != "start" && item.Key.ToLower() != "numitems")
                    strBuild.AppendFormat("&{0}={1}", item.Key, item.Value);
            }

            string uri = this.Url.Link("DefaultApi", new
            {
                controller = this.ControllerContext.ControllerDescriptor.ControllerName
            });
            uri += strBuild.ToString();
            return uri;
        }

        protected void ThrowHttpResponseException(HttpStatusCode statusCode, string message)
        {
            var response = new HttpResponseMessage(statusCode);
            response.ReasonPhrase = message;
            throw new HttpResponseException(response);
        }
    }
}
