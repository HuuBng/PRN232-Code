using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Business;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Enrollments;
namespace PRN232.LMS.Services.Implementations;

/// <summary>
///     Mô hình/lớp xử lý cho EnrollmentService.
/// </summary>
public class EnrollmentService(IUnitOfWork unitOfWork) : IEnrollmentService
{
    /// <summary>
    ///     Xử lý request/nghiệp vụ GetEnrollmentsAsync.
    /// </summary>
    public async Task<PaginatedResponse<EnrollmentResponse>> GetEnrollmentsAsync(QueryParameters query)
    {
        var includeStudent = SortHelper.ShouldExpand(query.Expand, "student");
        var includeCourse = SortHelper.ShouldExpand(query.Expand, "course");
        var enrollments = unitOfWork.Enrollments.GetAll();

        if (includeStudent) enrollments = enrollments.Include(e => e.Student);
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
            .Select(e => ToResponse(ToModel(e, includeStudent, includeCourse)))
            .ToListAsync();

        return new PaginatedResponse<EnrollmentResponse>
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

    /// <summary>
    ///     Xử lý request/nghiệp vụ GetEnrollmentByIdAsync.
    /// </summary>
    public async Task<EnrollmentResponse?> GetEnrollmentByIdAsync(int id, string? expand = null)
    {
        var includeStudent = SortHelper.ShouldExpand(expand, "student");
        var includeCourse = SortHelper.ShouldExpand(expand, "course");
        var enrollments = unitOfWork.Enrollments.GetAll();

        if (includeStudent) enrollments = enrollments.Include(e => e.Student);
        if (includeCourse) enrollments = enrollments.Include(e => e.Course);

        var enrollment = await enrollments.FirstOrDefaultAsync(e => e.EnrollmentId == id);
        return enrollment == null ? null : ToResponse(ToModel(enrollment, includeStudent, includeCourse));
    }

    /// <summary>
    ///     Xử lý request/nghiệp vụ CreateEnrollmentAsync.
    /// </summary>
    public async Task<EnrollmentResponse> CreateEnrollmentAsync(EnrollmentRequest request)
    {
        var enrollment = new Enrollment
        {
            StudentId = request.StudentId,
            CourseId = request.CourseId,
            EnrollDate = request.EnrollDate,
            Status = request.Status.Trim()
        };

        await unitOfWork.Enrollments.AddAsync(enrollment);
        await unitOfWork.SaveChangesAsync();
        return ToResponse(ToModel(enrollment, includeStudent: false, includeCourse: false));
    }

    /// <summary>
    ///     Xử lý request/nghiệp vụ UpdateEnrollmentAsync.
    /// </summary>
    public async Task<EnrollmentResponse?> UpdateEnrollmentAsync(int id, EnrollmentRequest request)
    {
        var enrollment = await unitOfWork.Enrollments.GetByIdAsync(id);
        if (enrollment == null) return null;

        enrollment.StudentId = request.StudentId;
        enrollment.CourseId = request.CourseId;
        enrollment.EnrollDate = request.EnrollDate;
        enrollment.Status = request.Status.Trim();

        unitOfWork.Enrollments.Update(enrollment);
        await unitOfWork.SaveChangesAsync();
        return ToResponse(ToModel(enrollment, includeStudent: false, includeCourse: false));
    }

    /// <summary>
    ///     Xử lý request/nghiệp vụ DeleteEnrollmentAsync.
    /// </summary>
    public async Task<bool> DeleteEnrollmentAsync(int id)
    {
        var enrollment = await unitOfWork.Enrollments.GetByIdAsync(id);
        if (enrollment == null) return false;

        unitOfWork.Enrollments.Delete(enrollment);
        await unitOfWork.SaveChangesAsync();
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
            Student = includeStudent && enrollment.Student != null
                ? new EnrollmentStudentModel
                {
                    StudentId = enrollment.Student.StudentId,
                    FullName = enrollment.Student.FullName,
                    Email = enrollment.Student.Email
                }
                : null,
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
}
