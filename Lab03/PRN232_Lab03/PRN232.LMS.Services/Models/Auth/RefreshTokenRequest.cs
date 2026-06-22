using System.ComponentModel.DataAnnotations;
namespace PRN232.LMS.Services.Models.Auth
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
