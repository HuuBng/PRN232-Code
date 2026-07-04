using System.ComponentModel.DataAnnotations;
namespace PRN232.LMS.CourseService.Models.Semesters
{
    public class SemesterRequest
    {
        [Required]
        [StringLength(100)]
        public string SemesterName { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
