using PRN232.LMS.StudentService.Validation;
using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.StudentService.Models
{
    public class StudentRequest
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        [FptStudentCode]
        public string? StudentCode { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }
    }
}
