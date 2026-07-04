using System.ComponentModel.DataAnnotations;
namespace PRN232.LMS.CourseService.Models.Enrollments
{
    public class EnrollmentRequest
    {
        [Range(1, int.MaxValue)]
        public int StudentId { get; set; }

        [Range(1, int.MaxValue)]
        public int CourseId { get; set; }

        [Required]
        public DateTime EnrollDate { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;
    }
}
