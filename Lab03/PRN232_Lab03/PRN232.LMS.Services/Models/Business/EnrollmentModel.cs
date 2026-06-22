namespace PRN232.LMS.Services.Models.Business
{
    /// <summary>
    ///     Business Model dùng cho xử lý nghiệp vụ đăng ký học phần trong Service Layer.
    /// </summary>
    public class EnrollmentModel
    {
        /// <summary>
        ///     Mã đăng ký.
        /// </summary>
        public int EnrollmentId { get; set; }

        /// <summary>
        ///     Mã sinh viên.
        /// </summary>
        public int StudentId { get; set; }

        /// <summary>
        ///     Mã khóa học.
        /// </summary>
        public int CourseId { get; set; }

        /// <summary>
        ///     Ngày đăng ký.
        /// </summary>
        public DateTime EnrollDate { get; set; }

        /// <summary>
        ///     Trạng thái đăng ký.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        ///     Thông tin sinh viên khi client yêu cầu expand.
        /// </summary>
        public EnrollmentStudentModel? Student { get; set; }

        /// <summary>
        ///     Thông tin khóa học khi client yêu cầu expand.
        /// </summary>
        public EnrollmentCourseModel? Course { get; set; }
    }

    /// <summary>
    ///     Business Model rút gọn cho sinh viên của đăng ký học phần.
    /// </summary>
    public class EnrollmentStudentModel
    {
        /// <summary>
        ///     Mã sinh viên.
        /// </summary>
        public int StudentId { get; set; }

        /// <summary>
        ///     Họ tên sinh viên.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        ///     Email sinh viên.
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    ///     Business Model rút gọn cho khóa học của đăng ký học phần.
    /// </summary>
    public class EnrollmentCourseModel
    {
        /// <summary>
        ///     Mã khóa học.
        /// </summary>
        public int CourseId { get; set; }

        /// <summary>
        ///     Tên khóa học.
        /// </summary>
        public string CourseName { get; set; } = string.Empty;
    }
}
