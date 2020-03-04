using App.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;

namespace App.Domain.Entities.Role
{

	[Serializable]
	public abstract class RoleTable<TPrimarykey, TUser> : FullAudited<TPrimarykey, TUser> where TUser : IEntity<long>
	{
		public const int MaxNameLength = 50;

		[StringLength(50)]
		public virtual string Name
		{
			get;
			set;
		}

		public bool Enable
		{
			get;
			set;
		}
	}
}
