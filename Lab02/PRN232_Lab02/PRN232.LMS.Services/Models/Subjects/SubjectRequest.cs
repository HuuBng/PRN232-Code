using System.ComponentModel.DataAnnotations;
namespace PRN232.LMS.Services.Models.Subjects
{
    /// <summary>
    ///     Mô hình/lớp xử lý cho SubjectRequest.
    /// </summary>
    public class SubjectRequest
    {
        /// <summary>
        ///     Giá trị SubjectCode trong request/response.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string SubjectCode { get; set; } = string.Empty;

        /// <summary>
        ///     Giá trị SubjectName trong request/response.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string SubjectName { get; set; } = string.Empty;

        /// <summary>
        ///     Giá trị Credit trong request/response.
        /// </summary>
        [Range(1, 10)]
        public int Credit { get; set; }
    }
}
