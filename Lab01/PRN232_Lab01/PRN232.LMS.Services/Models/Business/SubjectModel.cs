namespace PRN232.LMS.Services.Models.Business
{
    /// <summary>
    ///     Business Model dùng cho xử lý nghiệp vụ môn học trong Service Layer.
    /// </summary>
    public class SubjectModel
    {
        /// <summary>
        ///     Mã môn học.
        /// </summary>
        public int SubjectId { get; set; }

        /// <summary>
        ///     Mã code môn học.
        /// </summary>
        public string SubjectCode { get; set; } = string.Empty;

        /// <summary>
        ///     Tên môn học.
        /// </summary>
        public string SubjectName { get; set; } = string.Empty;

        /// <summary>
        ///     Số tín chỉ.
        /// </summary>
        public int Credit { get; set; }
    }
}
