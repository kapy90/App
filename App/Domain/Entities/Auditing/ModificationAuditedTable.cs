using System;

namespace App.Domain.Entities.Auditing
{
	[Serializable]
	public abstract class ModificationAuditedTable<TTable, TPrimaryKey, TUser> : ModificationAudited<TPrimaryKey, TUser> where TUser : IEntity<long>
	{
		public virtual long TableId
		{
			get;
			set;
		}

		public virtual TTable Table
		{
			get;
			set;
		}
	}
}
