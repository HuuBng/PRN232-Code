namespace PRN232.LMS.Services.Models.Common
{
    /// <summary>
    ///     Metadata phân trang trả về cùng list API.
    /// </summary>
    public class PaginationMetadata
    {
        /// <summary>
        ///     Trang hiện tại.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        ///     Số phần tử mỗi trang.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        ///     Tổng số phần tử sau khi filter/search.
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        ///     Tổng số trang.
        /// </summary>
        public int TotalPages { get; set; }
    }

    /// <summary>
    ///     Response cho các API danh sách có phân trang.
    /// </summary>
    public class PaginatedResponse<T>
    {
        /// <summary>
        ///     Danh sách dữ liệu của trang hiện tại.
        /// </summary>
        public IEnumerable<T> Items { get; set; } = [];

        /// <summary>
        ///     Thông tin phân trang.
        /// </summary>
        public PaginationMetadata Pagination { get; set; } = new PaginationMetadata();
    }
}
