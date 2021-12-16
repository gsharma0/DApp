using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.Helpers;
using Microsoft.AspNetCore.Http;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeaders(this HttpResponse response, int currentPage 
        , int totalPages, int itemsPerPage, int totalItems)
        {
            var paginatioHeaders = new PaginationHeaders(currentPage,totalPages,itemsPerPage, totalItems);
            var options = new JsonSerializerOptions(){
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
                
            response.Headers.Add("Pagination",JsonSerializer.Serialize(paginatioHeaders,options));
            response.Headers.Add("Access-Control-Expose-Headers","Pagination");
        }
    }
}