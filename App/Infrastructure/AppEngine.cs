﻿using App;
using App.Application.Services;
using App.Dependency;
using App.Domain.Services;
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

		private void RigistInternalService<TLifeTime, TInternalService>(IServiceCollection services, Type implementType, Func<Type, bool> derivedCondition)
		{
			var domainServiceInterFace = implementType.GetInterfaces()
							.Where(x => !x.Equals(typeof(TInternalService)))
							.Where(x => !x.Equals(typeof(TLifeTime)))
							.Where(derivedCondition).FirstOrDefault();

			if (domainServiceInterFace != null)
			{
				var dependencis = implementType.GetInterfaces()
						.Where(x => !x.Equals(typeof(TInternalService)))
						.Where(x => !x.Equals(typeof(TLifeTime)))
						.Where(x => !x.Equals(typeof(ITransientDependency)))
						.Where(x => x.Name.EndsWith("Dependency")).ToList();

				if (dependencis.Count > 0)
				{
					var appService = services.BuildServiceProvider().GetService(domainServiceInterFace);
					if (appService == null)
					{
						var dependency = dependencis.FirstOrDefault();
						if (dependency.Equals(typeof(IScopedDependency)))
						{
							services.AddScoped(domainServiceInterFace, implementType);
						}
						else if (dependency.Equals(typeof(ISingletonDependency)))
						{
							services.AddSingleton(domainServiceInterFace, implementType);
						}
					}
				}
				else
				{
					RigisterService(services, implementType, domainServiceInterFace);
				}
			}
		}

		private void RigisterFactory<TLifeTime>(IServiceCollection services, Type implementType)
		{
			if (implementType.GetInterfaces().Contains<Type>(typeof(IDomainService)))
			{
				this.RigistInternalService<TLifeTime,IDomainService>(services, implementType, (x) => x.Name.EndsWith("Manager") ||
					x.Name.EndsWith("Service") || x.Name.EndsWith("DomainService"));
			}
			else if (implementType.GetInterfaces().Contains<Type>(typeof(IApplicationService)))
			{
				this.RigistInternalService<TLifeTime, IApplicationService>(services, implementType, (x) => x.Name.EndsWith("AppService"));
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
				var serviceInterFace = implementType.GetInterfaces().Where(x=>!x.Equals(typeof(TLifeTime)))
							.Where(x=>x.Name.EndsWith(implementType.Name)).FirstOrDefault();
				if(serviceInterFace == null)
				{
					RigisterService(services, implementType);
				}
				else
				{
					RigisterService(services, implementType, serviceInterFace);
				}
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
			//find mapper configurations provided by other assemblies
			var mapperConfigurations = typeFinder.FindClassesOfType<IOrderedMapperProfile>();

			//create and sort instances of mapper configurations
			var instances = mapperConfigurations
				.Select(mapperConfiguration => (IOrderedMapperProfile)Activator.CreateInstance(mapperConfiguration))
				.OrderBy(mapperConfiguration => mapperConfiguration.Order);

			//create AutoMapper configuration
			var config = new MapperConfiguration(cfg =>
			{
				foreach (var instance in instances)
				{
					cfg.AddProfile(instance.GetType());
				}
			});

			//register
			AutoMapperConfiguration.Init(config);
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
