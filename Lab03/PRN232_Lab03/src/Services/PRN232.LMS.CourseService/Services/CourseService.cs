using Microsoft.EntityFrameworkCore;
using PRN232.LMS.CourseService.Data;
using PRN232.LMS.CourseService.Entities;
using PRN232.LMS.CourseService.Models.Business;
using PRN232.LMS.CourseService.Models.Courses;
using PRN232.LMS.Shared.Models;

namespace PRN232.LMS.CourseService.Services
{
    public class CourseService(CourseDbContext context) : ICourseService
    {
        public async Task<PaginatedResponse<CourseResponse>> GetCoursesAsync(QueryParameters query)
        {
            var includeSemester = SortHelper.ShouldExpand(query.Expand, "semester");
            var includeSubject = SortHelper.ShouldExpand(query.Expand, "subject");
            var courses = context.Courses.AsQueryable();

            if (includeSemester) courses = courses.Include(c => c.Semester);
            if (includeSubject) courses = courses.Include(c => c.Subject);

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var keyword = query.Search.Trim().ToLower();
                courses = courses.Where(c => c.CourseName.ToLower().Contains(keyword));
            }

            courses = SortHelper.ApplySort(courses, query.Sort, "CourseId",
                ("coursename", "CourseName"),
                ("semesterid", "SemesterId"),
                ("subjectid", "SubjectId"));

            var totalItems = await courses.CountAsync();
            var page = query.ValidPage;
            var pageSize = query.ValidSize;

            var items = await courses
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => ToResponse(ToModel(c, includeSemester, includeSubject)))
                .ToListAsync();

            return new PaginatedResponse<CourseResponse>
            {
                Items = items,
                Pagination = new PaginationMetadata
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
                }
            };
        }

        public async Task<CourseResponse?> GetCourseByIdAsync(int id, string? expand = null)
        {
            var includeSemester = SortHelper.ShouldExpand(expand, "semester");
            var includeSubject = SortHelper.ShouldExpand(expand, "subject");
            var courses = context.Courses.AsQueryable();

            if (includeSemester) courses = courses.Include(c => c.Semester);
            if (includeSubject) courses = courses.Include(c => c.Subject);

            var course = await courses.FirstOrDefaultAsync(c => c.CourseId == id);
            return course == null ? null : ToResponse(ToModel(course, includeSemester, includeSubject));
        }

        public async Task<CourseResponse> CreateCourseAsync(CourseRequest request)
        {
            await ValidateReferencesAsync(request);

            var course = new Course
            {
                CourseName = request.CourseName.Trim(),
                SemesterId = request.SemesterId,
                SubjectId = request.SubjectId
            };

            context.Courses.Add(course);
            await context.SaveChangesAsync();
            return ToResponse(ToModel(course, false, false));
        }

        public async Task<CourseResponse?> UpdateCourseAsync(int id, CourseRequest request)
        {
            var course = await context.Courses.FindAsync(id);
            if (course == null) return null;

            await ValidateReferencesAsync(request);

            course.CourseName = request.CourseName.Trim();
            course.SemesterId = request.SemesterId;
            course.SubjectId = request.SubjectId;

            context.Courses.Update(course);
            await context.SaveChangesAsync();
            return ToResponse(ToModel(course, false, false));
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            var course = await context.Courses.FindAsync(id);
            if (course == null) return false;

            context.Courses.Remove(course);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetDefaultSubjectIdAsync()
        {
            var subject = await context.Subjects
                .OrderBy(s => s.SubjectId)
                .FirstOrDefaultAsync();

            if (subject == null)
            {
                throw new CourseValidationException("Subject does not exist");
            }

            return subject.SubjectId;
        }

        private static CourseModel ToModel(Course course, bool includeSemester, bool includeSubject)
        {
            return new CourseModel
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                SemesterId = course.SemesterId,
                SubjectId = course.SubjectId,
                Semester = includeSemester && course.Semester != null
                    ? new CourseSemesterModel
                    {
                        SemesterId = course.Semester.SemesterId,
                        SemesterName = course.Semester.SemesterName
                    }
                    : null,
                Subject = includeSubject && course.Subject != null
                    ? new CourseSubjectModel
                    {
                        SubjectId = course.Subject.SubjectId,
                        SubjectCode = course.Subject.SubjectCode,
                        SubjectName = course.Subject.SubjectName,
                        Credit = course.Subject.Credit
                    }
                    : null
            };
        }

        private static CourseResponse ToResponse(CourseModel course)
        {
            return new CourseResponse
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                SemesterId = course.SemesterId,
                SubjectId = course.SubjectId,
                Semester = course.Semester == null
                    ? null
                    : new CourseSemesterResponse
                    {
                        SemesterId = course.Semester.SemesterId,
                        SemesterName = course.Semester.SemesterName
                    },
                Subject = course.Subject == null
                    ? null
                    : new CourseSubjectResponse
                    {
                        SubjectId = course.Subject.SubjectId,
                        SubjectCode = course.Subject.SubjectCode,
                        SubjectName = course.Subject.SubjectName,
                        Credit = course.Subject.Credit
                    }
            };
        }

        private async Task ValidateReferencesAsync(CourseRequest request)
        {
            if (!await context.Semesters.AnyAsync(s => s.SemesterId == request.SemesterId))
            {
                throw new CourseValidationException($"Semester with id {request.SemesterId} does not exist");
            }

            if (!await context.Subjects.AnyAsync(s => s.SubjectId == request.SubjectId))
            {
                throw new CourseValidationException($"Subject with id {request.SubjectId} does not exist");
            }
        }
    }
}
