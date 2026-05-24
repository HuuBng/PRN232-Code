namespace PRN232.LMS.Services.Models.Business
{
    /// <summary>
    ///     Business Model dùng cho xử lý nghiệp vụ học kỳ trong Service Layer.
    /// </summary>
    public class SemesterModel
    {
        /// <summary>
        ///     Mã học kỳ.
        /// </summary>
        public int SemesterId { get; set; }

        /// <summary>
        ///     Tên học kỳ.
        /// </summary>
        public string SemesterName { get; set; } = string.Empty;

        /// <summary>
        ///     Ngày bắt đầu.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        ///     Ngày kết thúc.
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}
