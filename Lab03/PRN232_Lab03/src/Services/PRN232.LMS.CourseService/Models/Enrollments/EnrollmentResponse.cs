namespace PRN232.LMS.CourseService.Models.Enrollments
{
    public class EnrollmentResponse
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public EnrollmentStudentResponse? Student { get; set; }
        public EnrollmentCourseResponse? Course { get; set; }
    }

    public class EnrollmentStudentResponse
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class EnrollmentCourseResponse
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
    }
}
