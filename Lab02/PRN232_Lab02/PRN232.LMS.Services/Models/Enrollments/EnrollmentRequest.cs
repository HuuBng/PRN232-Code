using System.ComponentModel.DataAnnotations;
namespace PRN232.LMS.Services.Models.Enrollments
{
    /// <summary>
    ///     Mô hình/lớp xử lý cho EnrollmentRequest.
    /// </summary>
    public class EnrollmentRequest
    {
        /// <summary>
        ///     Giá trị StudentId trong request/response.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int StudentId { get; set; }

        /// <summary>
        ///     Giá trị CourseId trong request/response.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int CourseId { get; set; }

        /// <summary>
        ///     Giá trị EnrollDate trong request/response.
        /// </summary>
        [Required]
        public DateTime EnrollDate { get; set; }

        /// <summary>
        ///     Giá trị Status trong request/response.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;
    }
}
