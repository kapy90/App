using App.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;

namespace App.Domain.Entities.Role
{
	[Serializable]
	public abstract class MenuTable<TPrimarykey, TUser> : FullAudited<TPrimarykey, TUser> where TUser : IEntity<long>
	{
		public const int MaxNameLength = 50;

		public const int MaxIconLength = 50;

		[StringLength(50)]
		public virtual string Name
		{
			get;
			set;
		}

		[StringLength(50)]
		public virtual string Icon
		{
			get;
			set;
		}

		public virtual long ParentId
		{
			get;
			set;
		}

		public virtual long SortId
		{
			get;
			set;
		}
	}

}
