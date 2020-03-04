using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace App.Domain.Entities
{
    [Serializable]
    public abstract class Entity : Entity<int>, IEntity, IEntity<int>
    {
    }

	[Serializable]
	public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey>
	{
		[Column(Order = 1)]
		public virtual TPrimaryKey Id
		{
			get;
			set;
		}
	}
}
