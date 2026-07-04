using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.IdentityService.Models
{
    public class LoginRequest
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Password { get; set; } = string.Empty;
    }
}
