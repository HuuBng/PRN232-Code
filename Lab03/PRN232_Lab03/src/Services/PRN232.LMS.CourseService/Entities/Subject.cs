namespace PRN232.LMS.CourseService.Entities
{
    public class Subject
    {
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int Credit { get; set; }
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
