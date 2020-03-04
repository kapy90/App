using App.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace App.Infrastructure
{
	public static class AppServiceCollectionExtensions
	{
		public static void AddApp(this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment, string webRootPath)
		{
			string path = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? Path.Combine(hostEnvironment.ContentRootPath, "Logs") : Path.Combine(AppContext.BaseDirectory, "Logs");
			services.AddSingleton(new Log(path));
			AppEngine appEngine = new AppEngine(hostEnvironment);
			((IEngine)appEngine).ConfigureServices(services, configuration, webRootPath);
			EngineContext.Current = appEngine;
		}

		public static IConfigurationRoot GetConfiguration(this IHostEnvironment hostEnvironment)
		{
			string contentRootPath = hostEnvironment.ContentRootPath;
			string environmentName = hostEnvironment.EnvironmentName;
			return BuildConfiguration(contentRootPath, environmentName);
		}

		public static TConfig ConfigureStartupConfig<TConfig>(this IServiceCollection services, IConfiguration configuration) where TConfig : class, new()
		{
			if (services == null)
			{
				throw new ArgumentNullException("services");
			}
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			TConfig config = new TConfig();
			configuration.Bind(config);
			services.AddSingleton(config);
			return config;
		}

		private static IConfigurationRoot BuildConfiguration(string path, string environmentName = null)
		{
			IConfigurationBuilder builder3 = new ConfigurationBuilder().SetBasePath(path);
			builder3 = (string.IsNullOrWhiteSpace(environmentName) ? builder3.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) : builder3.AddJsonFile("appsettings." + environmentName + ".json", optional: true));
			builder3 = builder3.AddEnvironmentVariables();
			return builder3.Build();
		}
	}
}
