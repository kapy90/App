using App.Dependency;
using System;

namespace App.Logging
{
	public interface IExceptionLog : IScopedDependency
	{
		void Invoke(Exception exception, int statusCode);
	}
}
