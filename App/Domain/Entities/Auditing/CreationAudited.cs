using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.Auditing
{

	[Serializable]
	public abstract class CreationAudited<TPrimaryKey, TUser> : Entity<TPrimaryKey>, ICreationAudited<TUser>, ICreationAudited, ICreationTime where TUser : IEntity<long>
	{
		public virtual DateTime CreationTime
		{
			get;
			set;
		} = DateTime.Now;


		public virtual long? CreatorUserId
		{
			get;
			set;
		}

		public virtual TUser CreatorUser
		{
			get;
			set;
		}
	}

}
