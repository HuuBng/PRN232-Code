using System.ComponentModel.DataAnnotations;
namespace PRN232.LMS.Services.Models.Courses
{
    /// <summary>
    ///     Request body for creating a course under a semester route.
    /// </summary>
    public class CourseForSemesterRequest
    {
        /// <summary>
        ///     Giá trị CourseName trong request/response.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string CourseName { get; set; } = string.Empty;

        /// <summary>
        ///     Giá trị SubjectId trong request/response.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int SubjectId { get; set; }
    }
}
