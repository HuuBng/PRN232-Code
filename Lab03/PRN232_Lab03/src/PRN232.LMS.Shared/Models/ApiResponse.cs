using System.Xml.Serialization;

namespace PRN232.LMS.Shared.Models
{
    [XmlType("ApiResponse")]
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        [XmlIgnore]
        public object? Errors { get; set; }

        public static ApiResponse<T> Ok(T? data, string message = "Request processed successfully")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = null
            };
        }

        public static ApiResponse<T> Fail(string message, object? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = errors
            };
        }
    }
}
