using App;
using App.Application.Services;
using App.Dependency;
using App.Infrastructure.Mapper;
using App.Logging;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace App.Infrastructure
{
	public class AppEngine : IEngine
	{
		private IServiceProvider _serviceProvider
		{
			get;
			set;
		}

		private IHostEnvironment _hostingEnvironment
		{
			get;
		}

		public virtual IServiceProvider ServiceProvider => _serviceProvider;

		public AppEngine(IHostEnvironment hostEnvironment)
		{
			_hostingEnvironment = hostEnvironment;
		}

		protected IServiceProvider GetServiceProvider()
		{
			return ServiceProvider.GetService<IHttpContextAccessor>().HttpContext?.RequestServices ?? ServiceProvider;
		}

		protected virtual void RegisterDependencies(IServiceCollection services, ITypeFinder typeFinder)
		{
			services.AddSingleton((IEngine)this);
			services.AddSingleton(typeFinder);
			typeFinder.FindClassesOfType<ITransientDependency>().ToList().ForEach(delegate (Type x)
			{
				RigisterFactory<ITransientDependency>(services, x);
			});
			typeFinder.FindClassesOfType<IScopedDependency>().ToList().ForEach(delegate (Type x)
			{
				RigisterFactory<IScopedDependency>(services, x);
			});
			typeFinder.FindClassesOfType<ISingletonDependency>().ToList().ForEach(delegate (Type x)
			{
				RigisterFactory<ISingletonDependency>(services, x);
			});
			_serviceProvider = services.BuildServiceProvider();
		}

		private void RigisterFactory<TLifeTime>(IServiceCollection services, Type implementType)
		{
			if (implementType.GetInterfaces().Contains<Type>(typeof(IApplicationService)))
			{
				Type appServiceInterFace = (from f in implementType.GetInterfaces()
											where !f.Equals(typeof(IApplicationService))
											where !f.Equals(typeof(TLifeTime))
											select f).FirstOrDefault();
				if (appServiceInterFace != null && appServiceInterFace.Name.EndsWith("AppService"))
				{
					RigisterService(services, implementType, appServiceInterFace);
				}
			}
			else if (implementType.GetInterfaces().Contains<Type>(typeof(IExceptionLog)))
			{
				if (!typeof(ExceptionLog).Equals(implementType))
				{
					RigisterService(services, implementType, typeof(IExceptionLog));
				}
			}
			else
			{
				RigisterService(services, implementType);
			}
		}

		private void RigisterService(IServiceCollection services, Type implementType, Type interfaceType = null)
		{
			if (implementType.GetInterfaces().Contains<Type>(typeof(ITransientDependency)))
			{
				if (interfaceType == null)
				{
					services.AddTransient(implementType);
				}
				else
				{
					services.AddTransient(interfaceType, implementType);
				}
			}
			else if (implementType.GetInterfaces().Contains<Type>(typeof(IScopedDependency)))
			{
				if (interfaceType == null)
				{
					services.AddScoped(implementType);
				}
				else
				{
					services.AddScoped(interfaceType, implementType);
				}
			}
			else if (implementType.GetInterfaces().Contains<Type>(typeof(ISingletonDependency)))
			{
				if (interfaceType == null)
				{
					services.AddSingleton(implementType);
				}
				else
				{
					services.AddSingleton(interfaceType, implementType);
				}
			}
		}

		protected virtual void AddAutoMapper(IServiceCollection services, ITypeFinder typeFinder)
		{
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Expected O, but got Unknown
			IEnumerable<Type> mapperConfigurations = typeFinder.FindClassesOfType<IOrderedMapperProfile>();
			IOrderedEnumerable<IOrderedMapperProfile> instances = from mapperConfiguration in mapperConfigurations
																  select (IOrderedMapperProfile)Activator.CreateInstance(mapperConfiguration) into mapperConfiguration
																  orderby mapperConfiguration.Order
																  select mapperConfiguration;
			AutoMapperConfiguration.Init((MapperConfiguration)(object)new MapperConfiguration((Action<IMapperConfigurationExpression>)delegate (IMapperConfigurationExpression cfg)
			{
				foreach (IOrderedMapperProfile current in instances)
				{
					cfg.AddProfile(current.GetType());
				}
			}));
		}

		private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly a) => a.FullName == args.Name);
			if (assembly != null)
			{
				return assembly;
			}
			return Resolve<ITypeFinder>().GetAssemblies().FirstOrDefault((Assembly a) => a.FullName == args.Name);
		}

		public void ConfigureServices(IServiceCollection services, IConfiguration configuration, string webRootPath)
		{
			AppFileProvider appFileProvider = new AppFileProvider(_hostingEnvironment, webRootPath);
			services.AddSingleton((IAppFileProvider)appFileProvider);
			WebAppTypeFinder typeFinder = new WebAppTypeFinder(appFileProvider);
			foreach (IAppStartup item in from startup in typeFinder.FindClassesOfType<IAppStartup>()
										 select (IAppStartup)Activator.CreateInstance(startup) into startup
										 orderby startup.Order
										 select startup)
			{
				item.ConfigureServices(services, configuration);
			}
			AddAutoMapper(services, typeFinder);
			RegisterDependencies(services, typeFinder);
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
		}

		public void ConfigureRequestPipeline(IApplicationBuilder application)
		{
			foreach (IAppStartup item in from startup in Resolve<ITypeFinder>().FindClassesOfType<IAppStartup>()
										 select (IAppStartup)Activator.CreateInstance(startup) into startup
										 orderby startup.Order
										 select startup)
			{
				item.Configure(application);
			}
			application.UseExceptionHandler(delegate (IApplicationBuilder builder)
			{
				builder.Run(ExceptionHandler.Invoke);
			});
		}

		public T Resolve<T>() where T : class
		{
			return (T)Resolve(typeof(T));
		}

		public object Resolve(Type type)
		{
			return GetServiceProvider().GetService(type);
		}

		public virtual IEnumerable<T> ResolveAll<T>()
		{
			return (IEnumerable<T>)GetServiceProvider().GetServices(typeof(T));
		}

		public virtual object ResolveUnregistered(Type type)
		{
			Exception innerException = null;
			ConstructorInfo[] constructors = type.GetConstructors();
			foreach (ConstructorInfo constructor in constructors)
			{
				try
				{
					IEnumerable<object> parameters = from parameter in constructor.GetParameters()
													 select Resolve(parameter.ParameterType) ?? throw new AppException("Unknown dependency");
					return Activator.CreateInstance(type, parameters.ToArray());
				}
				catch (Exception ex)
				{
					innerException = ex;
				}
			}
			throw new AppException("No constructor was found that had all the dependencies satisfied.", innerException);
		}
	}
}
