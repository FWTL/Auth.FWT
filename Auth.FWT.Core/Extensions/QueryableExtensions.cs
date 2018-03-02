using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Auth.FWT.Core.Data;

namespace Auth.FWT.Core.Extensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class QueryableExtensions
    {
        public static IQueryable<TSource> OrderByIndex<TSource, TKey>(this IQueryable<TSource> source, int column, OrderBy direction = Auth.FWT.Core.Data.OrderBy.Ascending, params OrderableColumn<TSource, TKey>[] @params)
        {
            var orderable = @params.FirstOrDefault(x => x.Id == column);
            if (orderable != null)
            {
                return source.OrderWithDirection(orderable.Fn, direction);
            }

            return source;
        }

        public static IOrderedQueryable<TSource> OrderWithDirection<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, OrderBy direction = Auth.FWT.Core.Data.OrderBy.Ascending)
        {
            if (direction == Auth.FWT.Core.Data.OrderBy.Ascending)
            {
                return source.OrderBy(keySelector);
            }

            return source.OrderByDescending(keySelector);
        }

        public static IQueryable<T> Paginate<T>(this IOrderedQueryable<T> query, int recordStart, int pageSize)
        {
            return query.Skip(recordStart).Take(pageSize);
        }

        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int recordStart, int pageSize)
        {
            return query.Skip(recordStart).Take(pageSize);
        }

        public static IEnumerable<TResult> Random<TSource, TResult>(this IQueryable<TSource> source, Func<TSource, TResult> selector, int rows = 1)
        {
            return source.OrderBy(x => Guid.NewGuid()).Take(rows).Select(selector);
        }

        public static IEnumerable<TResult> GetAllOfType<TResult>(this IEnumerable<TResult> source)
        {
            return source.Where(s => s.GetType().FullName == typeof(TResult).FullName);
        }

        public static List<Dictionary<string, object>> GetListOfValuesOf<TResult>(this IEnumerable<TResult> source, params string[] @params)
        {
            var results = new List<Dictionary<string, object>>();

            foreach (var item in source)
            {
                results.Add(GetValuesOf(item, @params));
            }

            return results;
        }

        public static Dictionary<string, object> GetValuesOf<TResult>(this TResult source, params string[] @params)
        {
            var type = source.GetType();
            var result = new Dictionary<string, object>();
            foreach (var param in @params)
            {
                result.Add(param, type.GetProperty(param)?.GetValue(source));
            }

            return result;
        }

        public static TResult GetRefValuesOf<TResult>(this object source, string param) where TResult : class
        {
            var type = source.GetType();
            var result = type.GetProperty(param)?.GetValue(source);

            return result as TResult;
        }

        public static TResult GetStructValuesOf<TResult>(this object source, string param) where TResult : struct
        {
            var type = source.GetType();
            var result = type.GetProperty(param)?.GetValue(source);

            return (TResult)result;
        }
    }
}