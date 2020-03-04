

namespace App.Domain.Entities.Auditing
{
	public interface IDeletionAudited : IDeletionTime
	{
		long? DeleterUserId
		{
			get;
			set;
		}

		bool IsDeleted
		{
			get;
			set;
		}
	}

	public interface IDeletionAudited<TUser> : IDeletionAudited, IDeletionTime where TUser : IEntity<long>
	{
		TUser DeleterUser
		{
			get;
			set;
		}
	}


}
