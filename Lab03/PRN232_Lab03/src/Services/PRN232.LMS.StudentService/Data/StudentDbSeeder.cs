using Microsoft.EntityFrameworkCore;
using PRN232.LMS.StudentService.Entities;

namespace PRN232.LMS.StudentService.Data
{
    public static class StudentDbSeeder
    {
        public static async Task SeedAsync(StudentDbContext context)
        {
            await context.Database.MigrateAsync();

            if (!await context.Students.AnyAsync())
            {
                var students = new List<Student>();
                for (int i = 1; i <= 50; i++)
                {
                    students.Add(new Student
                    {
                        FullName = $"Student {i:D3}",
                        Email = $"student{i:D3}@lms.local",
                        DateOfBirth = new DateTime(2000 + (i % 5), 1 + (i % 12), 1 + (i % 28)),
                        PhoneNumber = $"090{i:D7}",
                        StudentCode = $"SE{i:D6}"
                    });
                }
                context.Students.AddRange(students);
                await context.SaveChangesAsync();
            }
        }
    }
}
