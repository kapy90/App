﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.Auditing
{
	[Serializable]
	public abstract class DeletionAudited<TPrimaryKey, TUser> : Entity<TPrimaryKey>, IDeletionAudited<TUser>, IDeletionAudited, IDeletionTime where TUser : IEntity<long>
	{
		public virtual DateTime DeletionTime
		{
			get;
			set;
		} = DateTime.Now;


		public bool IsDeleted
		{
			get;
			set;
		}

		public virtual long? DeleterUserId
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
