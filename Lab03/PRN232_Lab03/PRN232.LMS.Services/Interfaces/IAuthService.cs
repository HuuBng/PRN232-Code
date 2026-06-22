using PRN232.LMS.Services.Models.Auth;
namespace PRN232.LMS.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthTokenResponse?> LoginAsync(LoginRequest request);

        Task<AuthTokenResponse?> RefreshTokenAsync(RefreshTokenRequest request);
    }
}
