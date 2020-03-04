using System;
using System.Runtime.Serialization;

namespace App
{

	[Serializable]
	public class AppException : Exception
	{
		public AppException()
		{
		}

		public AppException(string message)
			: base(message)
		{
		}

		public AppException(string messageFormat, params object[] args)
			: base(string.Format(messageFormat, args))
		{
		}

		protected AppException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public AppException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
