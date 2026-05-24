using System.ComponentModel.DataAnnotations;
namespace PRN232.LMS.Services.Models.Courses
{
    /// <summary>
    ///     Mô hình/lớp xử lý cho CourseRequest.
    /// </summary>
    public class CourseRequest
    {
        /// <summary>
        ///     Giá trị CourseName trong request/response.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string CourseName { get; set; } = string.Empty;

        /// <summary>
        ///     Giá trị SemesterId trong request/response.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int SemesterId { get; set; }

        /// <summary>
        ///     Giá trị SubjectId trong request/response.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int SubjectId { get; set; }
    }
}
