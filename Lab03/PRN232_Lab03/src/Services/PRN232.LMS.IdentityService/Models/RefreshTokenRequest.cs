using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.IdentityService.Models
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
