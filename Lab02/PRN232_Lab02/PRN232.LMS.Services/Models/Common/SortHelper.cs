using System.Linq.Expressions;
using System.Reflection;
namespace PRN232.LMS.Services.Models.Common
{
    /// <summary>
    ///     Helper dùng chung cho các service: kiểm tra expand và áp dụng sort đa field.
    /// </summary>
    public static class SortHelper
    {

        private static readonly MethodInfo OrderByMethod = GetSortMethod("OrderBy");
        private static readonly MethodInfo OrderByDescendingMethod = GetSortMethod("OrderByDescending");
        private static readonly MethodInfo ThenByMethod = GetSortMethod("ThenBy");
        private static readonly MethodInfo ThenByDescendingMethod = GetSortMethod("ThenByDescending");
        /// <summary>
        ///     Kiểm tra client có yêu cầu expand một navigation property cụ thể hay không.
        /// </summary>
        /// <param name="expand">Chuỗi expand từ query parameter, các tên phân cách bằng dấu phẩy.</param>
        /// <param name="name">Tên navigation property cần kiểm tra.</param>
        /// <returns>True nếu property được yêu cầu expand.</returns>
        public static bool ShouldExpand(string? expand, string name)
        {
            return !string.IsNullOrWhiteSpace(expand) &&
                   expand.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                       .Any(x => x.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        ///     Áp dụng sắp xếp đa field lên IQueryable dựa trên chuỗi sort.
        /// </summary>
        /// <typeparam name="T">Entity type cần sắp xếp.</typeparam>
        /// <param name="query">IQueryable gốc.</param>
        /// <param name="sort">Chuỗi sort từ client, phân cách bằng dấu phẩy; tiền tố '-' cho descending.</param>
        /// <param name="defaultField">Tên property sắp xếp mặc định khi không có sort hoặc field không khớp.</param>
        /// <param name="mappings">Danh sách ánh xạ giữa tên sort field và tên property thực của entity.</param>
        /// <returns>IQueryable đã được sắp xếp.</returns>
        public static IQueryable<T> ApplySort<T>(
            IQueryable<T> query,
            string? sort,
            string defaultField,
            params (string sortField, string propertyName)[] mappings)
        {
            var fieldMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var (sortField, propertyName) in mappings)
            {
                fieldMap[sortField] = propertyName;
            }

            if (string.IsNullOrWhiteSpace(sort))
            {
                return ApplyOrder(query, defaultField, false);
            }

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

        /// <summary>
        ///     Áp dụng OrderBy hoặc OrderByDescending bằng reflection, tương thích EF Core.
        /// </summary>
        private static IOrderedQueryable<T> ApplyOrder<T>(
            IQueryable<T> source,
            string propertyName,
            bool descending)
        {
            var lambda = BuildLambda<T>(propertyName);
            var method = descending ? OrderByDescendingMethod : OrderByMethod;
            var genericMethod = method.MakeGenericMethod(typeof(T), lambda.Body.Type);
            return (IOrderedQueryable<T>)genericMethod.Invoke(null, [source, lambda])!;
        }

        /// <summary>
        ///     Áp dụng ThenBy hoặc ThenByDescending bằng reflection, tương thích EF Core.
        /// </summary>
        private static IOrderedQueryable<T> ApplyThenBy<T>(
            IOrderedQueryable<T> source,
            string propertyName,
            bool descending)
        {
            var lambda = BuildLambda<T>(propertyName);
            var method = descending ? ThenByDescendingMethod : ThenByMethod;
            var genericMethod = method.MakeGenericMethod(typeof(T), lambda.Body.Type);
            return (IOrderedQueryable<T>)genericMethod.Invoke(null, [source, lambda])!;
        }

        /// <summary>
        ///     Dựng biểu thức lambda x => x.PropertyName cho LINQ OrderBy/ThenBy.
        /// </summary>
        private static LambdaExpression BuildLambda<T>(string propertyName)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var member = Expression.Property(param, propertyName);
            return Expression.Lambda(member, param);
        }

        /// <summary>
        ///     Tìm MethodInfo của Queryable.OrderBy/OrderByDescending/ThenBy/ThenByDescending (overload 2 tham số).
        /// </summary>
        private static MethodInfo GetSortMethod(string name)
        {
            return typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == name && m.GetParameters().Length == 2);
        }
    }
}
