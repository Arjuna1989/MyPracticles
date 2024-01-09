
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{
    public class APIExceptionMiddleWare
    {
        private readonly ILogger<APIExceptionMiddleWare> _logger;
        private readonly IHostEnvironment _env;
        private readonly RequestDelegate _next;
        public APIExceptionMiddleWare(RequestDelegate next,ILogger<APIExceptionMiddleWare> logger, IHostEnvironment env)
        {
            _next = next;
            _env = env;
            _logger = logger;

        }


        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, ex.Message.ToString());
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var Response = _env.IsDevelopment()
                ? new APIExceptions(context.Response.StatusCode, ex.Message.ToString(), ex.StackTrace?.ToString())
                : new APIExceptions(context.Response.StatusCode, ex.Message.ToString(), ex.StackTrace?.ToString());

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(Response, options);
                 await context.Response.WriteAsJsonAsync(json);


            }
        }
    }
}