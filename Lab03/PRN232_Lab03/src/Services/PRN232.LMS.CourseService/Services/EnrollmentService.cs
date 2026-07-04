using Microsoft.EntityFrameworkCore;
using PRN232.LMS.CourseService.Data;
using PRN232.LMS.CourseService.Entities;
using PRN232.LMS.CourseService.Grpc;
using PRN232.LMS.CourseService.Models.Business;
using PRN232.LMS.CourseService.Models.Enrollments;
using PRN232.LMS.Shared.Models;

namespace PRN232.LMS.CourseService.Services
{
    public class EnrollmentService(CourseDbContext context, IStudentGrpcClient studentGrpcClient) : IEnrollmentService
    {
        public async Task<PaginatedResponse<EnrollmentResponse>> GetEnrollmentsAsync(QueryParameters query)
        {
            var includeStudent = SortHelper.ShouldExpand(query.Expand, "student");
            var includeCourse = SortHelper.ShouldExpand(query.Expand, "course");
            var enrollments = context.Enrollments.AsQueryable();

            if (includeCourse) enrollments = enrollments.Include(e => e.Course);

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var keyword = query.Search.Trim().ToLower();
                enrollments = enrollments.Where(e => e.Status.ToLower().Contains(keyword));
            }

            enrollments = SortHelper.ApplySort(enrollments, query.Sort, "EnrollmentId",
                ("enrolldate", "EnrollDate"),
                ("status", "Status"),
                ("studentid", "StudentId"),
                ("courseid", "CourseId"));

            var totalItems = await enrollments.CountAsync();
            var page = query.ValidPage;
            var pageSize = query.ValidSize;

            var items = await enrollments
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var models = items.Select(e => ToModel(e, includeStudent, includeCourse)).ToList();

            if (includeStudent)
            {
                var uniqueStudentIds = models.Where(m => m.Student == null).Select(m => m.StudentId).Distinct().ToList();
                var studentMap = new Dictionary<int, EnrollmentStudentModel>();
                foreach (var studentId in uniqueStudentIds)
                {
                    var student = await studentGrpcClient.GetStudentByIdAsync(studentId);
                    if (student != null)
                    {
                        studentMap[studentId] = new EnrollmentStudentModel
                        {
                            StudentId = student.StudentId,
                            FullName = student.FullName,
                            Email = student.Email
                        };
                    }
                }

                foreach (var model in models)
                {
                    if (model.Student == null && studentMap.TryGetValue(model.StudentId, out var student))
                    {
                        model.Student = student;
                    }
                }
            }

            return new PaginatedResponse<EnrollmentResponse>
            {
                Items = models.Select(ToResponse).ToList(),
                Pagination = new PaginationMetadata
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
                }
            };
        }

        public async Task<EnrollmentResponse?> GetEnrollmentByIdAsync(int id, string? expand = null)
        {
            var includeStudent = SortHelper.ShouldExpand(expand, "student");
            var includeCourse = SortHelper.ShouldExpand(expand, "course");
            var enrollments = context.Enrollments.AsQueryable();

            if (includeCourse) enrollments = enrollments.Include(e => e.Course);

            var enrollment = await enrollments.FirstOrDefaultAsync(e => e.EnrollmentId == id);
            if (enrollment == null) return null;

            var model = ToModel(enrollment, includeStudent, includeCourse);

            if (includeStudent && model.Student == null)
            {
                var student = await studentGrpcClient.GetStudentByIdAsync(model.StudentId);
                if (student != null)
                {
                    model.Student = new EnrollmentStudentModel
                    {
                        StudentId = student.StudentId,
                        FullName = student.FullName,
                        Email = student.Email
                    };
                }
            }

            return ToResponse(model);
        }

        public async Task<EnrollmentResponse> CreateEnrollmentAsync(EnrollmentRequest request)
        {
            var studentExists = await studentGrpcClient.CheckStudentExistsAsync(request.StudentId);
            if (!studentExists)
            {
                throw new EnrollmentValidationException($"Student with id {request.StudentId} does not exist");
            }

            if (!await context.Courses.AnyAsync(c => c.CourseId == request.CourseId))
            {
                throw new EnrollmentValidationException($"Course with id {request.CourseId} does not exist");
            }

            var enrollment = new Enrollment
            {
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                EnrollDate = NormalizeDate(request.EnrollDate),
                Status = request.Status.Trim()
            };

            context.Enrollments.Add(enrollment);
            await context.SaveChangesAsync();
            return ToResponse(ToModel(enrollment, false, false));
        }

        public async Task<EnrollmentResponse?> UpdateEnrollmentAsync(int id, EnrollmentRequest request)
        {
            var enrollment = await context.Enrollments.FindAsync(id);
            if (enrollment == null) return null;

            var studentExists = await studentGrpcClient.CheckStudentExistsAsync(request.StudentId);
            if (!studentExists)
            {
                throw new EnrollmentValidationException($"Student with id {request.StudentId} does not exist");
            }

            if (!await context.Courses.AnyAsync(c => c.CourseId == request.CourseId))
            {
                throw new EnrollmentValidationException($"Course with id {request.CourseId} does not exist");
            }

            enrollment.StudentId = request.StudentId;
            enrollment.CourseId = request.CourseId;
            enrollment.EnrollDate = NormalizeDate(request.EnrollDate);
            enrollment.Status = request.Status.Trim();

            context.Enrollments.Update(enrollment);
            await context.SaveChangesAsync();
            return ToResponse(ToModel(enrollment, false, false));
        }

        public async Task<bool> DeleteEnrollmentAsync(int id)
        {
            var enrollment = await context.Enrollments.FindAsync(id);
            if (enrollment == null) return false;

            context.Enrollments.Remove(enrollment);
            await context.SaveChangesAsync();
            return true;
        }

        private static EnrollmentModel ToModel(Enrollment enrollment, bool includeStudent, bool includeCourse)
        {
            return new EnrollmentModel
            {
                EnrollmentId = enrollment.EnrollmentId,
                StudentId = enrollment.StudentId,
                CourseId = enrollment.CourseId,
                EnrollDate = enrollment.EnrollDate,
                Status = enrollment.Status,
                Course = includeCourse && enrollment.Course != null
                    ? new EnrollmentCourseModel
                    {
                        CourseId = enrollment.Course.CourseId,
                        CourseName = enrollment.Course.CourseName
                    }
                    : null
            };
        }

        private static EnrollmentResponse ToResponse(EnrollmentModel enrollment)
        {
            return new EnrollmentResponse
            {
                EnrollmentId = enrollment.EnrollmentId,
                StudentId = enrollment.StudentId,
                CourseId = enrollment.CourseId,
                EnrollDate = enrollment.EnrollDate,
                Status = enrollment.Status,
                Student = enrollment.Student == null
                    ? null
                    : new EnrollmentStudentResponse
                    {
                        StudentId = enrollment.Student.StudentId,
                        FullName = enrollment.Student.FullName,
                        Email = enrollment.Student.Email
                    },
                Course = enrollment.Course == null
                    ? null
                    : new EnrollmentCourseResponse
                    {
                        CourseId = enrollment.Course.CourseId,
                        CourseName = enrollment.Course.CourseName
                    }
            };
        }

        private static DateTime NormalizeDate(DateTime value)
        {
            return value.Kind switch
            {
                DateTimeKind.Utc => DateTime.SpecifyKind(value.ToLocalTime(), DateTimeKind.Unspecified),
                DateTimeKind.Local => DateTime.SpecifyKind(value, DateTimeKind.Unspecified),
                _ => value
            };
        }
    }
}
