using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.Auditing
{
	public interface IModificationTime
	{
		[Column("ModificationTime")]
		DateTime ModificationTime
		{
			get;
			set;
		}
	}
}
