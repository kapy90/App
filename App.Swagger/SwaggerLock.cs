using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace App.Swagger
{
    public static class SwaggerLock
    {
        /// <summary>
        /// swagger 锁
        /// </summary>
        /// <param name="app"></param>
        public static void UseSwaggerLock(this IApplicationBuilder app, string lockToken)
        {
            app.Use(next =>
            {
                return async context =>
                {
                    if (context.Request.Path.StartsWithSegments("/swagger"))
                    {
                        var token = context.Request.Query["token"];
                        if (token != lockToken)
                        {
                            var swCo = context.Request.Cookies["swCookies"];
                            if (swCo != "pp1tac001@akd23")
                            {
                                await context.Response.WriteAsync("Swagger doc");
                            }
                            else
                            {
                                await next(context);
                            }
                        }
                        else
                        {
                            context.Response.Cookies.Append("swCookies", "pp1tac001@akd23");
                            context.Response.Redirect("/swagger");
                        }
                    }
                    else
                    {
                        await next(context);
                    }
                };
            });
        }
    }
}
