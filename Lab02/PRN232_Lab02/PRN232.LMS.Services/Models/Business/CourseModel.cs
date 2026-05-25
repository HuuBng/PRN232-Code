namespace PRN232.LMS.Services.Models.Business
{
    /// <summary>
    ///     Business Model dùng cho xử lý nghiệp vụ khóa học trong Service Layer.
    /// </summary>
    public class CourseModel
    {
        /// <summary>
        ///     Mã khóa học.
        /// </summary>
        public int CourseId { get; set; }

        /// <summary>
        ///     Tên khóa học.
        /// </summary>
        public string CourseName { get; set; } = string.Empty;

        /// <summary>
        ///     Mã học kỳ.
        /// </summary>
        public int SemesterId { get; set; }

        /// <summary>
        ///     Mã môn học.
        /// </summary>
        public int SubjectId { get; set; }

        /// <summary>
        ///     Thông tin học kỳ khi client yêu cầu expand.
        /// </summary>
        public CourseSemesterModel? Semester { get; set; }

        /// <summary>
        ///     Thông tin môn học khi client yêu cầu expand.
        /// </summary>
        public CourseSubjectModel? Subject { get; set; }
    }

    /// <summary>
    ///     Business Model rút gọn cho học kỳ của khóa học.
    /// </summary>
    public class CourseSemesterModel
    {
        /// <summary>
        ///     Mã học kỳ.
        /// </summary>
        public int SemesterId { get; set; }

        /// <summary>
        ///     Tên học kỳ.
        /// </summary>
        public string SemesterName { get; set; } = string.Empty;
    }

    /// <summary>
    ///     Business Model rút gọn cho môn học của khóa học.
    /// </summary>
    public class CourseSubjectModel
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
