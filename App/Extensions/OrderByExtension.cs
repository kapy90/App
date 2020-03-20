using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace App.Extensions.OrderByExtensions
{
    /// <summary>
    /// Linq sort
    /// </summary>
    public static partial class OrderByExtension
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, string propertyName)
        {
            return OrderBy(queryable, propertyName, false);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, string propertyName, bool desc)
        {
            var param = Expression.Parameter(typeof(T));
            var body = Expression.Property(param, propertyName);
            dynamic keySelector = Expression.Lambda(body, param);
            return desc ? Queryable.OrderByDescending(queryable, keySelector) : Queryable.OrderBy(queryable, keySelector);
        }

        public static IQueryable<TSource> OrderBy<TSource, Tkey>(this IQueryable<TSource> sources, Expression<Func<TSource, Tkey>> keySelector, bool desc)
        {
            var groupedOrderBy = desc ? Queryable.OrderByDescending(sources, keySelector) : Queryable.OrderBy(sources, keySelector);
            return groupedOrderBy.AsQueryable();
        }

    }
}
