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
            catch (OutOfRangeError)
            {
                Console.WriteLine("Mems");
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Out of range!");
                return;
            }
            catch (System.Exception)
            {

            }
        }
    }
}