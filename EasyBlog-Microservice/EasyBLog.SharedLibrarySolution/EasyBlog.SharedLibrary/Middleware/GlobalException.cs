
using System.Net;
using System.Text.Json;
using Azure.Core;
using EasyBlog.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EasyBlog.SharedLibrary.Middleware
{
    internal class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            string message = "sorry! Internal server error occurred. Kindy try agian";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "error ";
            
            try
            {
                await next(context);
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "Warning";
                    message = "Too many request made.";
                    statusCode = StatusCodes.Status429TooManyRequests;
                    await ModifyHeader(context, title, message, statusCode);
                }

                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "Alert";
                    message = "You are not authorized to access";
                    await ModifyHeader(context, title, message, statusCode);
                }

                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Out of access";
                    message = "You are not allowed/required to access";
                    statusCode = StatusCodes.Status403Forbidden;
                    await ModifyHeader(context, title, message, statusCode);
                }
            }
            catch (Exception ex) 
            { 
                LogException.LogExceptions(ex);
                if(ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Out of time";
                    message = "Request timeout ... try again!";
                    statusCode = StatusCodes.Status408RequestTimeout;
                }
                await ModifyHeader(context, title, message, statusCode);
            }
        }

        private async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Detail = message,
                Status = statusCode,
                Title = title,

            }), CancellationToken.None);
            return;
        }
    }
}
