namespace PRN232.LMS.Services.Models.Enrollments
{
    /// <summary>
    ///     Mô hình/lớp xử lý cho EnrollmentResponse.
    /// </summary>
    public class EnrollmentResponse
    {
        /// <summary>
        ///     Giá trị EnrollmentId trong request/response.
        /// </summary>
        public int EnrollmentId { get; set; }
        /// <summary>
        ///     Giá trị StudentId trong request/response.
        /// </summary>
        public int StudentId { get; set; }
        /// <summary>
        ///     Giá trị CourseId trong request/response.
        /// </summary>
        public int CourseId { get; set; }
        /// <summary>
        ///     Giá trị EnrollDate trong request/response.
        /// </summary>
        public DateTime EnrollDate { get; set; }
        /// <summary>
        ///     Giá trị Status trong request/response.
        /// </summary>
        public string Status { get; set; } = string.Empty;
        /// <summary>
        ///     Giá trị Student trong request/response.
        /// </summary>
        public EnrollmentStudentResponse? Student { get; set; }
        /// <summary>
        ///     Giá trị Course trong request/response.
        /// </summary>
        public EnrollmentCourseResponse? Course { get; set; }
    }

    /// <summary>
    ///     Mô hình/lớp xử lý cho EnrollmentStudentResponse.
    /// </summary>
    public class EnrollmentStudentResponse
    {
        /// <summary>
        ///     Giá trị StudentId trong request/response.
        /// </summary>
        public int StudentId { get; set; }
        /// <summary>
        ///     Giá trị FullName trong request/response.
        /// </summary>
        public string FullName { get; set; } = string.Empty;
        /// <summary>
        ///     Giá trị Email trong request/response.
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    ///     Mô hình/lớp xử lý cho EnrollmentCourseResponse.
    /// </summary>
    public class EnrollmentCourseResponse
    {
        /// <summary>
        ///     Giá trị CourseId trong request/response.
        /// </summary>
        public int CourseId { get; set; }
        /// <summary>
        ///     Giá trị CourseName trong request/response.
        /// </summary>
        public string CourseName { get; set; } = string.Empty;
    }
}
