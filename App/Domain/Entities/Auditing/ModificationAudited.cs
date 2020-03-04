using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.Auditing
{

	[Serializable]
	public abstract class ModificationAudited<TPrimaryKey, TUser> : Entity<TPrimaryKey>, IModificationAudited<TUser>, IModificationAudited, IModificationTime where TUser : IEntity<long>
	{
		public virtual DateTime ModificationTime
		{
			get;
			set;
		} = DateTime.Now;


		public virtual long? ModifierUserId
		{
			get;
			set;
		}

		[ForeignKey("ModifierUserId")]
		public virtual TUser ModifierUser
		{
			get;
			set;
		}
	}

}
