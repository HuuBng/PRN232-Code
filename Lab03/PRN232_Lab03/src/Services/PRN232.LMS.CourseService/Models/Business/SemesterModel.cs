namespace PRN232.LMS.CourseService.Models.Business
{
    public class SemesterModel
    {
        public int SemesterId { get; set; }
        public string SemesterName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
