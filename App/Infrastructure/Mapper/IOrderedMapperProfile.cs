using System;
using System.Collections.Generic;
using System.Text;

namespace App.Infrastructure.Mapper
{
	public interface IOrderedMapperProfile
	{
		int Order
		{
			get;
		}
	}
}
