

namespace App.Domain.Entities.Auditing
{
	public interface ICreationAudited : ICreationTime
	{
		long? CreatorUserId
		{
			get;
			set;
		}
	}


	public interface ICreationAudited<TUser> : ICreationAudited, ICreationTime where TUser : IEntity<long>
	{
		TUser CreatorUser
		{
			get;
			set;
		}
	}

}
