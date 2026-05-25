namespace PRN232.LMS.Services.Models.Students
{
    /// <summary>
    ///     Mô hình/lớp xử lý cho StudentResponse.
    /// </summary>
    public class StudentResponse
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
        /// <summary>
        ///     Giá trị DateOfBirth trong request/response.
        /// </summary>
        public DateTime DateOfBirth { get; set; }
        /// <summary>
        ///     Giá trị Enrollments trong request/response.
        /// </summary>
        public IEnumerable<StudentEnrollmentResponse>? Enrollments { get; set; }
    }

    /// <summary>
    ///     Mô hình/lớp xử lý cho StudentEnrollmentResponse.
    /// </summary>
    public class StudentEnrollmentResponse
    {
        /// <summary>
        ///     Giá trị EnrollmentId trong request/response.
        /// </summary>
        public int EnrollmentId { get; set; }
        /// <summary>
        ///     Giá trị CourseId trong request/response.
        /// </summary>
        public int CourseId { get; set; }
        /// <summary>
        ///     Giá trị CourseName trong request/response.
        /// </summary>
        public string CourseName { get; set; } = string.Empty;
        /// <summary>
        ///     Giá trị EnrollDate trong request/response.
        /// </summary>
        public DateTime EnrollDate { get; set; }
        /// <summary>
        ///     Giá trị Status trong request/response.
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}
