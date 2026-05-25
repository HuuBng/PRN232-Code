namespace PRN232.LMS.Services.Models.Business
{
    /// <summary>
    ///     Business Model dùng cho xử lý nghiệp vụ sinh viên trong Service Layer.
    /// </summary>
    public class StudentModel
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

        /// <summary>
        ///     Ngày sinh sinh viên.
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        ///     Danh sách đăng ký học phần khi client yêu cầu expand.
        /// </summary>
        public IEnumerable<StudentEnrollmentModel>? Enrollments { get; set; }
    }

    /// <summary>
    ///     Business Model rút gọn cho đăng ký học phần của sinh viên.
    /// </summary>
    public class StudentEnrollmentModel
    {
        /// <summary>
        ///     Mã đăng ký.
        /// </summary>
        public int EnrollmentId { get; set; }

        /// <summary>
        ///     Mã khóa học.
        /// </summary>
        public int CourseId { get; set; }

        /// <summary>
        ///     Tên khóa học.
        /// </summary>
        public string CourseName { get; set; } = string.Empty;

        /// <summary>
        ///     Ngày đăng ký.
        /// </summary>
        public DateTime EnrollDate { get; set; }

        /// <summary>
        ///     Trạng thái đăng ký.
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}
