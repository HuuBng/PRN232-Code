namespace PRN232.LMS.CourseService.Models.Courses
{
    public class CourseResponse
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int SemesterId { get; set; }
        public int SubjectId { get; set; }
        public CourseSemesterResponse? Semester { get; set; }
        public CourseSubjectResponse? Subject { get; set; }
    }

    public class CourseSemesterResponse
    {
        public int SemesterId { get; set; }
        public string SemesterName { get; set; } = string.Empty;
    }

    public class CourseSubjectResponse
    {
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int Credit { get; set; }
    }
}
