using System.ComponentModel.DataAnnotations;
namespace PRN232.LMS.Services.Models.Semesters
{
    /// <summary>
    ///     Mô hình/lớp xử lý cho SemesterRequest.
    /// </summary>
    public class SemesterRequest
    {
        /// <summary>
        ///     Giá trị SemesterName trong request/response.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string SemesterName { get; set; } = string.Empty;

        /// <summary>
        ///     Giá trị StartDate trong request/response.
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        ///     Giá trị EndDate trong request/response.
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }
    }
}
