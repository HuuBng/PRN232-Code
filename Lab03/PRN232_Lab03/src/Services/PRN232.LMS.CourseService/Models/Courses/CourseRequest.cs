using System.ComponentModel.DataAnnotations;
namespace PRN232.LMS.CourseService.Models.Courses
{
    public class CourseRequest
    {
        [Required]
        [StringLength(100)]
        public string CourseName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int SemesterId { get; set; }

        [Range(1, int.MaxValue)]
        public int SubjectId { get; set; }
    }
}
