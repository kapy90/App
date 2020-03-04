using System;

namespace App.Domain.Entities.Auditing
{
	public interface IModificationAudited : IModificationTime
	{
		long? ModifierUserId
		{
			get;
			set;
		}
	}

	public interface IModificationAudited<TUser> : IModificationAudited, IModificationTime where TUser : IEntity<long>
	{
		TUser ModifierUser
		{
			get;
			set;
		}
	}

}
