namespace PRN232.LMS.Services.Models.Common
{
    /// <summary>
    ///     Định dạng response thống nhất cho toàn bộ API.
    /// </summary>
    public class ApiResponse<T>
    {
        /// <summary>
        ///     Cho biết request xử lý thành công hay thất bại.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        ///     Thông báo mô tả kết quả xử lý.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        ///     Dữ liệu trả về cho client.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        ///     Thông tin lỗi nếu request thất bại.
        /// </summary>
        public object? Errors { get; set; }

        /// <summary>
        ///     Tạo response thành công.
        /// </summary>
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

        /// <summary>
        ///     Tạo response thất bại.
        /// </summary>
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
