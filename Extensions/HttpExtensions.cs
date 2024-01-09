using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddHttpExtension(this HttpResponse response, PaginationHeader paginationHeader)
        {
            var jsonOption = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonResponseHeader = JsonSerializer.Serialize(paginationHeader, jsonOption);
            response.Headers.Add("pagination", jsonResponseHeader);
            response.Headers.Add("Access-Control-Expose-Headers", "pagination");

        }
    }
}