using System.Reflection;
namespace PRN232.LMS.Services.Models.Common
{
    /// <summary>
    ///     Hỗ trợ chọn động các field trả về theo query parameter fields.
    /// </summary>
    public static class FieldSelector
    {
        /// <summary>
        ///     Chọn một số field từ danh sách object response.
        /// </summary>
        public static IEnumerable<object> SelectFields<T>(IEnumerable<T> items, string? fields)
        {
            var properties = GetSelectedProperties<T>(fields);
            return items.Select(item => SelectFields(item, properties));
        }

        /// <summary>
        ///     Chọn một số field từ một object response.
        /// </summary>
        public static object SelectFields<T>(T item, string? fields)
        {
            var properties = GetSelectedProperties<T>(fields);
            return SelectFields(item, properties);
        }

        /// <summary>
        ///     Kiểm tra client có truyền fields hay không.
        /// </summary>
        public static bool HasFields(string? fields)
        {
            return !string.IsNullOrWhiteSpace(fields);
        }

        /// <summary>
        ///     Tạo dictionary chỉ chứa các property được chọn.
        /// </summary>
        private static object SelectFields<T>(T item, IEnumerable<PropertyInfo> properties)
        {
            var result = new Dictionary<string, object?>();

            foreach (var property in properties)
            {
                result[ToCamelCase(property.Name)] = property.GetValue(item);
            }

            return result;
        }

        /// <summary>
        ///     Lấy danh sách property hợp lệ từ chuỗi fields.
        /// </summary>
        private static IEnumerable<PropertyInfo> GetSelectedProperties<T>(string? fields)
        {
            var allProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (string.IsNullOrWhiteSpace(fields))
            {
                return allProperties;
            }

            var requestedFields = fields
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            return allProperties.Where(p => requestedFields.Contains(p.Name) || requestedFields.Contains(ToCamelCase(p.Name)));
        }

        /// <summary>
        ///     Chuyển tên property PascalCase sang camelCase để JSON key thân thiện hơn.
        /// </summary>
        private static string ToCamelCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || char.IsLower(value[0]))
            {
                return value;
            }

            return char.ToLowerInvariant(value[0]) + value[1..];
        }
    }
}
