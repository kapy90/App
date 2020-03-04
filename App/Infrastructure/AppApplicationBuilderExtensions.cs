using System;
using App.Infrastructure;
using Microsoft.AspNetCore.Builder;

namespace App.Infrastructure
{
	public static class AppApplicationBuilderExtensions
	{
		public static void UseApp(this IApplicationBuilder application)
		{
			EngineContext.Current.ConfigureRequestPipeline(application);
		}
	}
}
