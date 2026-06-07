using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
namespace PRN232.LMS.Repositories.Data
{
    public static class DbSeeder
    {
        private const int RequiredSemesterCount = 5;
        private const int RequiredStudentCount = 50;
        private const int RequiredSubjectCount = 10;
        private const int RequiredCourseCount = 20;
        private const int RequiredEnrollmentCount = 500;

        public static async Task SeedAsync(AppDbContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            await SeedSemestersAsync(context);
            await SeedStudentsAsync(context);
            await SeedSubjectsAsync(context);
            await SeedCoursesAsync(context);
            await SeedEnrollmentsAsync(context);
            await SeedUsersAsync(context);
        }

        private static async Task SeedUsersAsync(AppDbContext context)
        {
            if (await context.Users.AnyAsync())
            {
                return;
            }

            var hasher = new PasswordHasher<User>();
            var admin = new User
            {
                Username = "admin",
                Role = "Admin"
            };
            admin.PasswordHash = hasher.HashPassword(admin, "12345");

            var user = new User
            {
                Username = "student",
                Role = "User"
            };
            user.PasswordHash = hasher.HashPassword(user, "12345");

            context.Users.AddRange(admin, user);
            await context.SaveChangesAsync();
        }

        private static async Task SeedSemestersAsync(AppDbContext context)
        {
            var existingCount = await context.Semesters.CountAsync();
            if (existingCount >= RequiredSemesterCount)
            {
                return;
            }

            var existingNames = await context.Semesters
                .Select(s => s.SemesterName)
                .ToListAsync();

            var existingNameSet = existingNames.ToHashSet(StringComparer.OrdinalIgnoreCase);
            var semesters = new List<Semester>
            {
                new Semester
                {
                    SemesterName = "Spring 2024",
                    StartDate = new DateTime(2024, 1, 8),
                    EndDate = new DateTime(2024, 5, 5)
                },
                new Semester
                {
                    SemesterName = "Summer 2024",
                    StartDate = new DateTime(2024, 5, 20),
                    EndDate = new DateTime(2024, 8, 18)
                },
                new Semester
                {
                    SemesterName = "Fall 2024",
                    StartDate = new DateTime(2024, 9, 2),
                    EndDate = new DateTime(2024, 12, 22)
                },
                new Semester
                {
                    SemesterName = "Spring 2025",
                    StartDate = new DateTime(2025, 1, 6),
                    EndDate = new DateTime(2025, 5, 4)
                },
                new Semester
                {
                    SemesterName = "Summer 2025",
                    StartDate = new DateTime(2025, 5, 19),
                    EndDate = new DateTime(2025, 8, 17)
                }
            };

            var missing = RequiredSemesterCount - existingCount;
            context.Semesters.AddRange(semesters
                .Where(s => !existingNameSet.Contains(s.SemesterName))
                .Take(missing));

            await context.SaveChangesAsync();
        }

        private static async Task SeedStudentsAsync(AppDbContext context)
        {
            var existingCount = await context.Students.CountAsync();
            if (existingCount >= RequiredStudentCount)
            {
                return;
            }

            var existingEmails = await context.Students
                .Select(s => s.Email)
                .ToListAsync();

            var existingEmailSet = existingEmails.ToHashSet(StringComparer.OrdinalIgnoreCase);
            var students = Enumerable.Range(1, RequiredStudentCount)
                .Select(index => new Student
                {
                    FullName = $"Student {index:000}",
                    Email = $"student{index:000}@lms.local",
                    DateOfBirth = new DateTime(2000 + index % 6, index % 12 + 1, index % 27 + 1)
                })
                .Where(s => !existingEmailSet.Contains(s.Email))
                .Take(RequiredStudentCount - existingCount)
                .ToList();

            context.Students.AddRange(students);
            await context.SaveChangesAsync();
        }

        private static async Task SeedSubjectsAsync(AppDbContext context)
        {
            var existingCount = await context.Subjects.CountAsync();
            if (existingCount >= RequiredSubjectCount)
            {
                return;
            }

            var existingCodes = await context.Subjects
                .Select(s => s.SubjectCode)
                .ToListAsync();

            var existingCodeSet = existingCodes.ToHashSet(StringComparer.OrdinalIgnoreCase);
            var subjects = new List<Subject>
            {
                new Subject { SubjectCode = "PRN232", SubjectName = "Advanced Cross-Platform Application Programming With .NET", Credit = 3 },
                new Subject { SubjectCode = "PRO192", SubjectName = "Object-Oriented Programming", Credit = 3 },
                new Subject { SubjectCode = "CSD201", SubjectName = "Data Structures and Algorithms", Credit = 3 },
                new Subject { SubjectCode = "DBI202", SubjectName = "Database Systems", Credit = 3 },
                new Subject { SubjectCode = "SWE201", SubjectName = "Software Engineering", Credit = 3 },
                new Subject { SubjectCode = "PRJ301", SubjectName = "Java Web Application Development", Credit = 3 },
                new Subject { SubjectCode = "SWT301", SubjectName = "Software Testing", Credit = 3 },
                new Subject { SubjectCode = "MAS291", SubjectName = "Statistics and Probability", Credit = 3 },
                new Subject { SubjectCode = "NWC203", SubjectName = "Computer Networking", Credit = 3 },
                new Subject { SubjectCode = "SWP391", SubjectName = "Application Development Project", Credit = 4 }
            };

            context.Subjects.AddRange(subjects
                .Where(s => !existingCodeSet.Contains(s.SubjectCode))
                .Take(RequiredSubjectCount - existingCount));

            await context.SaveChangesAsync();
        }

        private static async Task SeedCoursesAsync(AppDbContext context)
        {
            var existingCount = await context.Courses.CountAsync();
            if (existingCount >= RequiredCourseCount)
            {
                return;
            }

            var semesters = await context.Semesters
                .OrderBy(s => s.StartDate)
                .Take(RequiredSemesterCount)
                .ToListAsync();

            var subjects = await context.Subjects
                .OrderBy(s => s.SubjectCode)
                .Take(RequiredSubjectCount)
                .ToListAsync();

            if (semesters.Count == 0 || subjects.Count == 0)
            {
                return;
            }

            var courses = new List<Course>();
            var missing = RequiredCourseCount - existingCount;

            for (var index = 0; index < missing; index++)
            {
                var courseNumber = existingCount + index + 1;
                var subject = subjects[index % subjects.Count];
                var semester = semesters[index % semesters.Count];

                courses.Add(new Course
                {
                    CourseName = $"{subject.SubjectCode} - {semester.SemesterName} - Group {courseNumber:00}",
                    SubjectId = subject.SubjectId,
                    SemesterId = semester.SemesterId
                });
            }

            context.Courses.AddRange(courses);
            await context.SaveChangesAsync();
        }

        private static async Task SeedEnrollmentsAsync(AppDbContext context)
        {
            var existingCount = await context.Enrollments.CountAsync();
            if (existingCount >= RequiredEnrollmentCount)
            {
                return;
            }

            var students = await context.Students
                .OrderBy(s => s.StudentId)
                .Take(RequiredStudentCount)
                .ToListAsync();

            var courses = await context.Courses
                .Include(c => c.Semester)
                .OrderBy(c => c.CourseId)
                .Take(RequiredCourseCount)
                .ToListAsync();

            if (students.Count == 0 || courses.Count == 0)
            {
                return;
            }

            var existingPairs = await context.Enrollments
                .Select(e => new { e.StudentId, e.CourseId })
                .ToListAsync();

            var existingPairSet = existingPairs
                .Select(e => (e.StudentId, e.CourseId))
                .ToHashSet();

            var statuses = new[] { "Active", "Completed", "Dropped" };
            var enrollments = new List<Enrollment>();
            var missing = RequiredEnrollmentCount - existingCount;

            foreach (var student in students)
            {
                foreach (var course in courses)
                {
                    if (enrollments.Count >= missing)
                    {
                        break;
                    }

                    if (!existingPairSet.Add((student.StudentId, course.CourseId)))
                    {
                        continue;
                    }

                    var dayOffset = (student.StudentId + course.CourseId) % 14;
                    enrollments.Add(new Enrollment
                    {
                        StudentId = student.StudentId,
                        CourseId = course.CourseId,
                        EnrollDate = course.Semester.StartDate.AddDays(dayOffset),
                        Status = statuses[(student.StudentId + course.CourseId) % statuses.Length]
                    });
                }

                if (enrollments.Count >= missing)
                {
                    break;
                }
            }

            context.Enrollments.AddRange(enrollments);
            await context.SaveChangesAsync();
        }
    }
}
