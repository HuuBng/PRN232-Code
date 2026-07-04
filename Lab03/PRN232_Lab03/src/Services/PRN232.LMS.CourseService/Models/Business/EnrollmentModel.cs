namespace PRN232.LMS.CourseService.Models.Business
{
    public class EnrollmentModel
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public EnrollmentStudentModel? Student { get; set; }
        public EnrollmentCourseModel? Course { get; set; }
    }

    public class EnrollmentStudentModel
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class EnrollmentCourseModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
    }
}
