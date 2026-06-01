using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Business;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Courses;
namespace PRN232.LMS.Services.Implementations;

/// <summary>
///     Mô hình/lớp xử lý cho CourseService.
/// </summary>
public class CourseService(IUnitOfWork unitOfWork) : ICourseService
{
    /// <summary>
    ///     Xử lý request/nghiệp vụ GetCoursesAsync.
    /// </summary>
    public async Task<PaginatedResponse<CourseResponse>> GetCoursesAsync(QueryParameters query)
    {
        var includeSemester = SortHelper.ShouldExpand(query.Expand, "semester");
        var includeSubject = SortHelper.ShouldExpand(query.Expand, "subject");
        var courses = unitOfWork.Courses.GetAll();

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

    /// <summary>
    ///     Xử lý request/nghiệp vụ GetCourseByIdAsync.
    /// </summary>
    public async Task<CourseResponse?> GetCourseByIdAsync(int id, string? expand = null)
    {
        var includeSemester = SortHelper.ShouldExpand(expand, "semester");
        var includeSubject = SortHelper.ShouldExpand(expand, "subject");
        var courses = unitOfWork.Courses.GetAll();

        if (includeSemester) courses = courses.Include(c => c.Semester);
        if (includeSubject) courses = courses.Include(c => c.Subject);

        var course = await courses.FirstOrDefaultAsync(c => c.CourseId == id);
        return course == null ? null : ToResponse(ToModel(course, includeSemester, includeSubject));
    }

    /// <summary>
    ///     Xử lý request/nghiệp vụ CreateCourseAsync.
    /// </summary>
    public async Task<CourseResponse> CreateCourseAsync(CourseRequest request)
    {
        await ValidateReferencesAsync(request);

        var course = new Course
        {
            CourseName = request.CourseName.Trim(),
            SemesterId = request.SemesterId,
            SubjectId = request.SubjectId
        };

        await unitOfWork.Courses.AddAsync(course);
        await unitOfWork.SaveChangesAsync();
        return ToResponse(ToModel(course, includeSemester: false, includeSubject: false));
    }

    /// <summary>
    ///     Xử lý request/nghiệp vụ UpdateCourseAsync.
    /// </summary>
    public async Task<CourseResponse?> UpdateCourseAsync(int id, CourseRequest request)
    {
        var course = await unitOfWork.Courses.GetByIdAsync(id);
        if (course == null) return null;

        await ValidateReferencesAsync(request);

        course.CourseName = request.CourseName.Trim();
        course.SemesterId = request.SemesterId;
        course.SubjectId = request.SubjectId;

        unitOfWork.Courses.Update(course);
        await unitOfWork.SaveChangesAsync();
        return ToResponse(ToModel(course, includeSemester: false, includeSubject: false));
    }

    /// <summary>
    ///     Xử lý request/nghiệp vụ DeleteCourseAsync.
    /// </summary>
    public async Task<bool> DeleteCourseAsync(int id)
    {
        var course = await unitOfWork.Courses.GetByIdAsync(id);
        if (course == null) return false;

        unitOfWork.Courses.Delete(course);
        await unitOfWork.SaveChangesAsync();
        return true;
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
        if (!await unitOfWork.Semesters.ExistsAsync(request.SemesterId))
        {
            throw new CourseValidationException($"Semester with id {request.SemesterId} does not exist");
        }

        if (!await unitOfWork.Subjects.ExistsAsync(request.SubjectId))
        {
            throw new CourseValidationException($"Subject with id {request.SubjectId} does not exist");
        }
    }
}
