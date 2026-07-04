using PRN232.LMS.IdentityService.Models;

namespace PRN232.LMS.IdentityService.Services
{
    public interface IAuthService
    {
        Task<AuthTokenResponse?> LoginAsync(LoginRequest request);
        Task<AuthTokenResponse?> RefreshTokenAsync(RefreshTokenRequest request);
    }
}
