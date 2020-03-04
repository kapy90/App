using App.Domain.Entities;
using App.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities
{
	[Serializable]
	public abstract class SystemErrorTable : Entity<long>, ICreationTime
	{
		public const int MaxExceptionTypeLength = 255;

		[StringLength(255)]
		public virtual string ExceptionType
		{
			get;
			set;
		}

		public virtual int HttpCode
		{
			get;
			set;
		}

		[Column("Exception", TypeName = "mediumtext")]
		public virtual string Exception
		{
			get;
			set;
		}

		public virtual DateTime CreationTime
		{
			get;
			set;
		} = DateTime.Now;

	}
}
