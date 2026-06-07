namespace PRN232.LMS.Services.Models.Auth
{
    public class AuthTokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public int ExpiresIn { get; set; }
    }
}
