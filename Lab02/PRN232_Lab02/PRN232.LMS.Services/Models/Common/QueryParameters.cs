namespace PRN232.LMS.Services.Models.Common
{
    /// <summary>
    ///     Các query parameter dùng chung cho list API: search, sort, paging, field selection và expansion.
    /// </summary>
    public class QueryParameters
    {
        /// <summary>
        ///     Từ khóa tìm kiếm.
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        ///     Chuỗi sắp xếp. Ví dụ: fullName hoặc -dateOfBirth.
        /// </summary>
        public string? Sort { get; set; }

        /// <summary>
        ///     Số trang client yêu cầu.
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        ///     Số phần tử mỗi trang client yêu cầu.
        /// </summary>
        public int Size { get; set; } = 10;

        /// <summary>
        ///     Danh sách field client muốn lấy. Ví dụ: studentId,fullName,email.
        /// </summary>
        public string? Fields { get; set; }

        /// <summary>
        ///     Danh sách navigation property cần mở rộng. Ví dụ: student,course.
        /// </summary>
        public string? Expand { get; set; }

        /// <summary>
        ///     Page hợp lệ, không nhỏ hơn 1.
        /// </summary>
        public int ValidPage
        {
            get => Page < 1 ? 1 : Page;
        }

        /// <summary>
        ///     Size hợp lệ, giới hạn tối đa 100 để tránh query quá lớn.
        /// </summary>
        public int ValidSize
        {
            get => Size < 1 ? 10 : Size > 100 ? 100 : Size;
        }
    }
}
