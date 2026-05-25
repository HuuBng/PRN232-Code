namespace PRN232.LMS.Services.Models.Courses
{
    /// <summary>
    ///     Mô hình/lớp xử lý cho CourseResponse.
    /// </summary>
    public class CourseResponse
    {
        /// <summary>
        ///     Giá trị CourseId trong request/response.
        /// </summary>
        public int CourseId { get; set; }
        /// <summary>
        ///     Giá trị CourseName trong request/response.
        /// </summary>
        public string CourseName { get; set; } = string.Empty;
        /// <summary>
        ///     Giá trị SemesterId trong request/response.
        /// </summary>
        public int SemesterId { get; set; }
        /// <summary>
        ///     Giá trị SubjectId trong request/response.
        /// </summary>
        public int SubjectId { get; set; }
        /// <summary>
        ///     Giá trị Semester trong request/response.
        /// </summary>
        public CourseSemesterResponse? Semester { get; set; }
        /// <summary>
        ///     Giá trị Subject trong request/response.
        /// </summary>
        public CourseSubjectResponse? Subject { get; set; }
    }

    /// <summary>
    ///     Mô hình/lớp xử lý cho CourseSemesterResponse.
    /// </summary>
    public class CourseSemesterResponse
    {
        /// <summary>
        ///     Giá trị SemesterId trong request/response.
        /// </summary>
        public int SemesterId { get; set; }
        /// <summary>
        ///     Giá trị SemesterName trong request/response.
        /// </summary>
        public string SemesterName { get; set; } = string.Empty;
    }

    /// <summary>
    ///     Mô hình/lớp xử lý cho CourseSubjectResponse.
    /// </summary>
    public class CourseSubjectResponse
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
