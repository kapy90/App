using System;

namespace App
{

	[Serializable]
	public class UserFriendlyException : Exception
	{
		public UserFriendlyException()
		{
		}

		public UserFriendlyException(string message)
			: base(message)
		{
		}
	}
}
