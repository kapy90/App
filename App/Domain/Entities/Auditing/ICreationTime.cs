using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.Auditing
{
	public interface ICreationTime
	{
		[Column("CreationTime")]
		DateTime CreationTime
		{
			get;
			set;
		}
	}
}
