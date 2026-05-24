namespace PRN232.LMS.Services.Models.Subjects
{
    /// <summary>
    ///     Mô hình/lớp xử lý cho SubjectResponse.
    /// </summary>
    public class SubjectResponse
    {
        /// <summary>
        ///     Giá trị SubjectId trong request/response.
        /// </summary>
        public int SubjectId { get; set; }
        /// <summary>
        ///     Giá trị SubjectCode trong request/response.
        /// </summary>
        public string SubjectCode { get; set; } = string.Empty;
        /// <summary>
        ///     Giá trị SubjectName trong request/response.
        /// </summary>
        public string SubjectName { get; set; } = string.Empty;
        /// <summary>
        ///     Giá trị Credit trong request/response.
        /// </summary>
        public int Credit { get; set; }
    }
}
