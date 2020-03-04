using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Infrastructure
{
	public interface IAppStartup
	{
		int Order
		{
			get;
		}

		void ConfigureServices(IServiceCollection services, IConfiguration configuration);

		void Configure(IApplicationBuilder application);
	}
}
