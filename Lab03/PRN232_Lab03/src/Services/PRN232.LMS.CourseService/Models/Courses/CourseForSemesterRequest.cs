using System.ComponentModel.DataAnnotations;
namespace PRN232.LMS.CourseService.Models.Courses
{
    public class CourseForSemesterRequest
    {
        [Required]
        [StringLength(100)]
        public string CourseName { get; set; } = string.Empty;

        public int? SubjectId { get; set; }
    }
}
