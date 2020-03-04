using System;
using System.Collections.Generic;

namespace App.Collections.Extensions
{
	public static class CollectionExtensions
	{
		public static bool IsNullOrEmpty<T>(this ICollection<T> source)
		{
			if (source != null)
			{
				return source.Count <= 0;
			}
			return true;
		}

		public static bool AddIfNotContains<T>(this ICollection<T> source, T item)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (source.Contains(item))
			{
				return false;
			}
			source.Add(item);
			return true;
		}
	}
}
