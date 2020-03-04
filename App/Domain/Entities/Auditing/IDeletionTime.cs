using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.Auditing
{
	public interface IDeletionTime
	{
		[Column("DeletionTime")]
		DateTime DeletionTime
		{
			get;
			set;
		}
	}
}
