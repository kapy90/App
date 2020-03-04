using App;
using App.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using System.Threading.Tasks;

namespace App.Logging
{
	public class ExceptionHandler
	{
		public static async Task Invoke(HttpContext context)
		{
			Exception exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
			if (exception == null)
			{
				await context.Response.WriteAsync(null);
				context.Response.Clear();
				return;
			}
			exception.ToString();
			int statusCode = context.Response.StatusCode;
			if (exception is UserFriendlyException)
			{
				statusCode = 400;
			}
			context.RequestServices.GetService<IExceptionLog>().Invoke(exception, statusCode);
			if (exception is UserFriendlyException)
			{
				UserFriendlyException userFriendlyException = exception as UserFriendlyException;
				context.Response.StatusCode = 400;
				context.Response.ContentType = "text/plain; charset=utf-8";
				await context.Response.WriteAsync(userFriendlyException.Message);
			}
			else
			{
				context.Response.StatusCode = 500;
				await context.Response.WriteAsync("服务器内部错误", Encoding.GetEncoding("GB2312"));
			}
		}
	}
}
