using Microsoft.EntityFrameworkCore;
using PRN232.LMS.CourseService.Entities;

namespace PRN232.LMS.CourseService.Data
{
    public static class CourseDbSeeder
    {
        public static async Task SeedAsync(CourseDbContext context)
        {
            await context.Database.MigrateAsync();

            if (!await context.Semesters.AnyAsync())
            {
                var semesters = new List<Semester>
                {
                    new() { SemesterName = "Spring 2024", StartDate = new DateTime(2024, 1, 15), EndDate = new DateTime(2024, 5, 15) },
                    new() { SemesterName = "Summer 2024", StartDate = new DateTime(2024, 5, 20), EndDate = new DateTime(2024, 8, 20) },
                    new() { SemesterName = "Fall 2024", StartDate = new DateTime(2024, 9, 1), EndDate = new DateTime(2024, 12, 20) },
                    new() { SemesterName = "Spring 2025", StartDate = new DateTime(2025, 1, 15), EndDate = new DateTime(2025, 5, 15) },
                    new() { SemesterName = "Fall 2025", StartDate = new DateTime(2025, 9, 1), EndDate = new DateTime(2025, 12, 20) }
                };
                context.Semesters.AddRange(semesters);
                await context.SaveChangesAsync();
            }

            if (!await context.Subjects.AnyAsync())
            {
                var subjects = new List<Subject>
                {
                    new() { SubjectCode = "PRN211", SubjectName = "Object-Oriented Programming", Credit = 3 },
                    new() { SubjectCode = "PRN221", SubjectName = "Software Design Patterns", Credit = 3 },
                    new() { SubjectCode = "PRN231", SubjectName = "Web Development", Credit = 3 },
                    new() { SubjectCode = "PRN232", SubjectName = "Mobile Development", Credit = 3 },
                    new() { SubjectCode = "SWD392", SubjectName = "Software Architecture", Credit = 4 },
                    new() { SubjectCode = "SWP391", SubjectName = "Software Project Management", Credit = 4 },
                    new() { SubjectCode = "EXE101", SubjectName = "Experiential Learning", Credit = 2 },
                    new() { SubjectCode = "JPD121", SubjectName = "Japanese Elementary", Credit = 3 },
                    new() { SubjectCode = "MLN111", SubjectName = "Philosophy of Marxism", Credit = 3 },
                    new() { SubjectCode = "ENG302", SubjectName = "Advanced English", Credit = 3 }
                };
                context.Subjects.AddRange(subjects);
                await context.SaveChangesAsync();
            }

            if (!await context.Courses.AnyAsync())
            {
                var semesters = await context.Semesters.ToListAsync();
                var subjects = await context.Subjects.ToListAsync();
                var courses = new List<Course>();
                int courseNum = 1;
                foreach (var semester in semesters)
                {
                    foreach (var subject in subjects.Take(4))
                    {
                        courses.Add(new Course
                        {
                            CourseName = $"{subject.SubjectName} - {semester.SemesterName}",
                            SemesterId = semester.SemesterId,
                            SubjectId = subject.SubjectId
                        });
                        courseNum++;
                    }
                }
                context.Courses.AddRange(courses);
                await context.SaveChangesAsync();
            }

            if (!await context.Enrollments.AnyAsync())
            {
                var courses = await context.Courses.ToListAsync();
                var enrollments = new List<Enrollment>();
                var random = new Random(42);
                int enrollmentId = 1;
                foreach (var course in courses)
                {
                    int numStudents = random.Next(5, 15);
                    var studentIds = Enumerable.Range(1, 50).OrderBy(_ => random.Next()).Take(numStudents).ToList();
                    foreach (var studentId in studentIds)
                    {
                        enrollments.Add(new Enrollment
                        {
                            StudentId = studentId,
                            CourseId = course.CourseId,
                            EnrollDate = new DateTime(2024, 1, 15).AddDays(random.Next(0, 120)),
                            Status = random.Next(0, 3) switch { 0 => "Active", 1 => "Completed", _ => "Dropped" }
                        });
                        enrollmentId++;
                    }
                }
                context.Enrollments.AddRange(enrollments);
                await context.SaveChangesAsync();
            }
        }
    }
}
