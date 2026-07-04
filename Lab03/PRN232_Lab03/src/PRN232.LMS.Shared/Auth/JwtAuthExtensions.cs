using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace PRN232.LMS.Shared.Auth
{
    public static class JwtAuthExtensions
    {
        public static IServiceCollection AddLmsJwtAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSecret = configuration["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret must be configured.");
            var jwtIssuer = configuration["Jwt:Issuer"] ?? "PRN232.LMS";
            var jwtAudience = configuration["Jwt:Audience"] ?? "PRN232.LMS.Client";

            if (jwtSecret.Length < 32)
                throw new InvalidOperationException("JWT secret must be at least 32 characters.");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ReadOrAdmin", policy => policy.RequireRole("User", "Admin"));
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            });

            return services;
        }
    }
}
