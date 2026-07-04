using System.Reflection;

namespace PRN232.LMS.Shared.Models
{
    public static class FieldSelector
    {
        public static List<object> SelectFields<T>(IEnumerable<T> items, string? fields)
        {
            var properties = GetSelectedProperties<T>(fields);
            return items.Select(item => SelectFields(item, properties)).ToList();
        }

        public static object SelectFields<T>(T item, string? fields)
        {
            var properties = GetSelectedProperties<T>(fields);
            return SelectFields(item, properties);
        }

        public static bool HasFields(string? fields)
        {
            return !string.IsNullOrWhiteSpace(fields);
        }

        private static object SelectFields<T>(T item, IEnumerable<PropertyInfo> properties)
        {
            var result = new Dictionary<string, object?>();
            foreach (var property in properties)
            {
                result[ToCamelCase(property.Name)] = property.GetValue(item);
            }
            return result;
        }

        private static IEnumerable<PropertyInfo> GetSelectedProperties<T>(string? fields)
        {
            var allProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (string.IsNullOrWhiteSpace(fields))
                return allProperties;

            var requestedFields = fields
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            return allProperties.Where(p => requestedFields.Contains(p.Name) || requestedFields.Contains(ToCamelCase(p.Name)));
        }

        private static string ToCamelCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || char.IsLower(value[0]))
                return value;
            return char.ToLowerInvariant(value[0]) + value[1..];
        }
    }
}
