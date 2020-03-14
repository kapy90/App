using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.Auditing
{
	[Serializable]
	public abstract class FullAudited<TPrimaryKey, TUser> : CreationAudited<TPrimaryKey, TUser>, IFullAudited<TUser>, ICreationAudited<TUser>, ICreationAudited, ICreationTime, IDeletionAudited<TUser>, IDeletionAudited, IDeletionTime where TUser : IEntity<long>
	{
		public virtual DateTime? DeletionTime
		{
			get;
			set;
		}


		public virtual long? DeleterUserId
		{
			get;
			set;
		}

		public bool IsDeleted
		{
			get;
			set;
		}

		[ForeignKey("DeleterUserId")]
		public virtual TUser DeleterUser
		{
			get;
			set;
		}
	}
}
