using System;

namespace App.Domain.Entities.Role
{
	[Serializable]
	public abstract class RoleMemuRef<TRole, TMenu> where TRole : IEntity<long> where TMenu : IEntity<long>
	{
		public virtual long RoleId
		{
			get;
			set;
		}

		public virtual long MenuId
		{
			get;
			set;
		}

		public virtual int Rights
		{
			get;
			set;
		}

		public virtual TRole Role
		{
			get;
			set;
		}

		public virtual TMenu Menu
		{
			get;
			set;
		}
	}
}
