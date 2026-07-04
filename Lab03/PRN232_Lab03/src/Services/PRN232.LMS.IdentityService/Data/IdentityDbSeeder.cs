using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.IdentityService.Entities;

namespace PRN232.LMS.IdentityService.Data
{
    public static class IdentityDbSeeder
    {
        private static readonly PasswordHasher<User> PasswordHasher = new();

        public static async Task SeedAsync(IdentityDbContext context)
        {
            await context.Database.MigrateAsync();

            if (!await context.Users.AnyAsync())
            {
                var admin = new User
                {
                    Username = "admin",
                    PasswordHash = PasswordHasher.HashPassword(new User(), "123456"),
                    Role = "Admin"
                };

                var student = new User
                {
                    Username = "student",
                    PasswordHash = PasswordHasher.HashPassword(new User(), "123456"),
                    Role = "User"
                };

                context.Users.AddRange(admin, student);
                await context.SaveChangesAsync();
            }
        }
    }
}
