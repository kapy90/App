using System;

namespace App.Domain.Entities.Role
{
	[Serializable]
	public abstract class AccountRoleRef<TAccount, TRole> where TAccount : IEntity<long> where TRole : IEntity<long>
	{
		public virtual long AccountId
		{
			get;
			set;
		}

		public virtual long RoleId
		{
			get;
			set;
		}

		public virtual bool Active
		{
			get;
			set;
		}

		public virtual TAccount Account
		{
			get;
			set;
		}

		public virtual TRole Role
		{
			get;
			set;
		}
	}
}
