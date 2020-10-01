using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ASG_Leaderboard_Project
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync("Not found Exception: " + ex.Message);
                return;
            }
            catch (OutOfRangeError ex)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Not found Exception: " + ex.Message);
                return;
            }
            catch (System.Exception ex)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("System Exception: " + ex.Message);
                return;
            }
        }
    }
}