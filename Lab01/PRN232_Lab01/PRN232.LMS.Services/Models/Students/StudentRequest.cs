using System.ComponentModel.DataAnnotations;
namespace PRN232.LMS.Services.Models.Students
{
    /// <summary>
    ///     Mô hình dữ liệu client gửi lên khi tạo/cập nhật sinh viên.
    /// </summary>
    public class StudentRequest
    {
        /// <summary>
        ///     Họ tên sinh viên.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        ///     Email sinh viên.
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        ///     Ngày sinh sinh viên.
        /// </summary>
        [Required]
        public DateTime DateOfBirth { get; set; }
    }
}
