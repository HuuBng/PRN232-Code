using System.ComponentModel.DataAnnotations;
namespace PRN232.LMS.CourseService.Models.Subjects
{
    public class SubjectRequest
    {
        [Required]
        [StringLength(20)]
        public string SubjectCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string SubjectName { get; set; } = string.Empty;

        [Range(1, 10)]
        public int Credit { get; set; }
    }
}
