using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Business;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Students;
namespace PRN232.LMS.Services.Implementations
{
    /// <summary>
    ///     Mô hình/lớp xử lý cho StudentService.
    /// </summary>
    public class StudentService(IUnitOfWork unitOfWork) : IStudentService
    {
        /// <summary>
        ///     Xử lý request/nghiệp vụ GetStudentsAsync.
        /// </summary>
        public async Task<PaginatedResponse<StudentResponse>> GetStudentsAsync(QueryParameters query)
        {
            var students = unitOfWork.Students.GetAll();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var keyword = query.Search.Trim().ToLower();
                students = students.Where(s =>
                    s.FullName.ToLower().Contains(keyword) ||
                    s.Email.ToLower().Contains(keyword));
            }

            students = SortHelper.ApplySort(students, query.Sort, "StudentId",
                ("fullname", "FullName"),
                ("email", "Email"),
                ("dateofbirth", "DateOfBirth"));

            var totalItems = await students.CountAsync();
            var page = query.ValidPage;
            var pageSize = query.ValidSize;

            if (SortHelper.ShouldExpand(query.Expand, "enrollments"))
            {
                students = students.Include(s => s.Enrollments).ThenInclude(e => e.Course);
            }

            var items = await students
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => ToResponse(ToModel(s, SortHelper.ShouldExpand(query.Expand, "enrollments"))))
                .ToListAsync();

            return new PaginatedResponse<StudentResponse>
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
        ///     v2: paginated list with <c>PhoneNumber</c> and <c>StudentCode</c>
        ///     surfaced in the response.
        /// </summary>
        public async Task<PaginatedResponse<StudentV2Response>> GetStudentsV2Async(QueryParameters query)
        {
            var students = unitOfWork.Students.GetAll();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var keyword = query.Search.Trim().ToLower();
                students = students.Where(s =>
                    s.FullName.ToLower().Contains(keyword) ||
                    s.Email.ToLower().Contains(keyword));
            }

            students = SortHelper.ApplySort(students, query.Sort, "StudentId",
                ("fullname", "FullName"),
                ("email", "Email"),
                ("dateofbirth", "DateOfBirth"));

            var totalItems = await students.CountAsync();
            var page = query.ValidPage;
            var pageSize = query.ValidSize;

            var includeEnrollments = SortHelper.ShouldExpand(query.Expand, "enrollments");
            if (includeEnrollments)
            {
                students = students.Include(s => s.Enrollments).ThenInclude(e => e.Course);
            }

            var items = await students
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new StudentV2Response
                {
                    StudentId = s.StudentId,
                    FullName = s.FullName,
                    Email = s.Email,
                    DateOfBirth = s.DateOfBirth,
                    PhoneNumber = s.PhoneNumber,
                    StudentCode = s.StudentCode
                })
                .ToListAsync();

            return new PaginatedResponse<StudentV2Response>
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
        ///     Xử lý request/nghiệp vụ GetStudentByIdAsync.
        /// </summary>
        public async Task<StudentResponse?> GetStudentByIdAsync(int id, string? expand = null)
        {
            var students = unitOfWork.Students.GetAll();
            var includeEnrollments = SortHelper.ShouldExpand(expand, "enrollments");

            if (includeEnrollments)
            {
                students = students.Include(s => s.Enrollments).ThenInclude(e => e.Course);
            }

            var student = await students.FirstOrDefaultAsync(s => s.StudentId == id);
            return student == null ? null : ToResponse(ToModel(student, includeEnrollments));
        }

        /// <summary>
        ///     v2: returns the same shape as GetStudentByIdAsync but also
        ///     surfaces <c>PhoneNumber</c> and <c>StudentCode</c>.
        /// </summary>
        public async Task<StudentV2Response?> GetStudentByIdV2Async(int id, string? expand = null)
        {
            var students = unitOfWork.Students.GetAll();
            var includeEnrollments = SortHelper.ShouldExpand(expand, "enrollments");

            if (includeEnrollments)
            {
                students = students.Include(s => s.Enrollments).ThenInclude(e => e.Course);
            }

            var student = await students.FirstOrDefaultAsync(s => s.StudentId == id);
            if (student == null)
            {
                return null;
            }

            return new StudentV2Response
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth,
                PhoneNumber = student.PhoneNumber,
                StudentCode = student.StudentCode,
                Enrollments = includeEnrollments
                    ? student.Enrollments.Select(e => new StudentEnrollmentResponse
                    {
                        EnrollmentId = e.EnrollmentId,
                        CourseId = e.CourseId,
                        CourseName = e.Course?.CourseName ?? string.Empty,
                        EnrollDate = e.EnrollDate,
                        Status = e.Status
                    })
                    : null
            };
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ CreateStudentAsync.
        /// </summary>
        public async Task<StudentResponse> CreateStudentAsync(StudentRequest request)
        {
            var student = new Student
            {
                FullName = request.FullName.Trim(),
                Email = request.Email.Trim(),
                DateOfBirth = request.DateOfBirth,
                PhoneNumber = request.PhoneNumber?.Trim(),
                StudentCode = request.StudentCode?.Trim()
            };

            await unitOfWork.Students.AddAsync(student);
            await unitOfWork.SaveChangesAsync();

            return ToResponse(ToModel(student, false));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ UpdateStudentAsync.
        /// </summary>
        public async Task<StudentResponse?> UpdateStudentAsync(int id, StudentRequest request)
        {
            var student = await unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
            {
                return null;
            }

            student.FullName = request.FullName.Trim();
            student.Email = request.Email.Trim();
            student.DateOfBirth = request.DateOfBirth;
            student.PhoneNumber = request.PhoneNumber?.Trim();
            student.StudentCode = request.StudentCode?.Trim();

            unitOfWork.Students.Update(student);
            await unitOfWork.SaveChangesAsync();

            return ToResponse(ToModel(student, false));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ DeleteStudentAsync.
        /// </summary>
        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
            {
                return false;
            }

            unitOfWork.Students.Delete(student);
            await unitOfWork.SaveChangesAsync();
            return true;
        }

        private static StudentModel ToModel(Student student, bool includeEnrollments)
        {
            return new StudentModel
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth,
                Enrollments = includeEnrollments
                    ? student.Enrollments.Select(e => new StudentEnrollmentModel
                    {
                        EnrollmentId = e.EnrollmentId,
                        CourseId = e.CourseId,
                        CourseName = e.Course?.CourseName ?? string.Empty,
                        EnrollDate = e.EnrollDate,
                        Status = e.Status
                    })
                    : null
            };
        }

        private static StudentResponse ToResponse(StudentModel student)
        {
            return new StudentResponse
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth,
                Enrollments = student.Enrollments?.Select(e => new StudentEnrollmentResponse
                {
                    EnrollmentId = e.EnrollmentId,
                    CourseId = e.CourseId,
                    CourseName = e.CourseName,
                    EnrollDate = e.EnrollDate,
                    Status = e.Status
                })
            };
        }
    }
}
