namespace PRN232.LMS.Services.Models.Semesters
{
    /// <summary>
    ///     Mô hình/lớp xử lý cho SemesterResponse.
    /// </summary>
    public class SemesterResponse
    {
        /// <summary>
        ///     Giá trị SemesterId trong request/response.
        /// </summary>
        public int SemesterId { get; set; }
        /// <summary>
        ///     Giá trị SemesterName trong request/response.
        /// </summary>
        public string SemesterName { get; set; } = string.Empty;
        /// <summary>
        ///     Giá trị StartDate trong request/response.
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        ///     Giá trị EndDate trong request/response.
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}
