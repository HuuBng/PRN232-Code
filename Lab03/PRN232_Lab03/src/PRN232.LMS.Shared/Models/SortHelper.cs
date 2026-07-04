using System.Linq.Expressions;
using System.Reflection;

namespace PRN232.LMS.Shared.Models
{
    public static class SortHelper
    {
        private static readonly MethodInfo OrderByMethod = GetSortMethod("OrderBy");
        private static readonly MethodInfo OrderByDescendingMethod = GetSortMethod("OrderByDescending");
        private static readonly MethodInfo ThenByMethod = GetSortMethod("ThenBy");
        private static readonly MethodInfo ThenByDescendingMethod = GetSortMethod("ThenByDescending");

        public static bool ShouldExpand(string? expand, string name)
        {
            return !string.IsNullOrWhiteSpace(expand) &&
                   expand.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                       .Any(x => x.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public static IQueryable<T> ApplySort<T>(
            IQueryable<T> query,
            string? sort,
            string defaultField,
            params (string sortField, string propertyName)[] mappings)
        {
            var fieldMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var (sortField, propertyName) in mappings)
                fieldMap[sortField] = propertyName;

            if (string.IsNullOrWhiteSpace(sort))
                return ApplyOrder(query, defaultField, false);

            var fields = sort.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            IOrderedQueryable<T>? ordered = null;

            foreach (var rawField in fields)
            {
                var descending = rawField.StartsWith('-');
                var fieldName = descending ? rawField[1..].Trim() : rawField.Trim();
                var propertyName = fieldMap.TryGetValue(fieldName, out var mapped) ? mapped : defaultField;

                ordered = ordered == null
                    ? ApplyOrder(query, propertyName, descending)
                    : ApplyThenBy(ordered, propertyName, descending);
            }

            return ordered ?? ApplyOrder(query, defaultField, false);
        }

        private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string propertyName, bool descending)
        {
            var lambda = BuildLambda<T>(propertyName);
            var method = descending ? OrderByDescendingMethod : OrderByMethod;
            var genericMethod = method.MakeGenericMethod(typeof(T), lambda.Body.Type);
            return (IOrderedQueryable<T>)genericMethod.Invoke(null, [source, lambda])!;
        }

        private static IOrderedQueryable<T> ApplyThenBy<T>(IOrderedQueryable<T> source, string propertyName, bool descending)
        {
            var lambda = BuildLambda<T>(propertyName);
            var method = descending ? ThenByDescendingMethod : ThenByMethod;
            var genericMethod = method.MakeGenericMethod(typeof(T), lambda.Body.Type);
            return (IOrderedQueryable<T>)genericMethod.Invoke(null, [source, lambda])!;
        }

        private static LambdaExpression BuildLambda<T>(string propertyName)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var member = Expression.Property(param, propertyName);
            return Expression.Lambda(member, param);
        }

        private static MethodInfo GetSortMethod(string name)
        {
            return typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == name && m.GetParameters().Length == 2);
        }
    }
}
