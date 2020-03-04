using App.Dependency;
using System;

namespace App.Logging
{
	public class ExceptionLog : IExceptionLog, IScopedDependency
	{
		public virtual void Invoke(Exception exception, int statusCode)
		{
		}
	}
}
