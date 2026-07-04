using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Shared.Models;
using PRN232.LMS.StudentService.Data;
using PRN232.LMS.StudentService.Entities;
using PRN232.LMS.StudentService.Models;

namespace PRN232.LMS.StudentService.Services
{
    public class StudentService(StudentDbContext context) : IStudentService
    {
        public async Task<PaginatedResponse<StudentResponse>> GetStudentsAsync(QueryParameters query)
        {
            var students = context.Students.AsNoTracking().AsQueryable();

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

            var items = await students
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new StudentResponse
                {
                    StudentId = s.StudentId,
                    FullName = s.FullName,
                    Email = s.Email,
                    DateOfBirth = s.DateOfBirth
                })
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

        public async Task<PaginatedResponse<StudentV2Response>> GetStudentsV2Async(QueryParameters query)
        {
            var students = context.Students.AsNoTracking().AsQueryable();

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

        public async Task<StudentResponse?> GetStudentByIdAsync(int id)
        {
            var student = await context.Students.AsNoTracking()
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                return null;
            }

            return new StudentResponse
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth
            };
        }

        public async Task<StudentV2Response?> GetStudentByIdV2Async(int id)
        {
            var student = await context.Students.AsNoTracking()
                .FirstOrDefaultAsync(s => s.StudentId == id);

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
                StudentCode = student.StudentCode
            };
        }

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

            context.Students.Add(student);
            await context.SaveChangesAsync();

            return new StudentResponse
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth
            };
        }

        public async Task<StudentResponse?> UpdateStudentAsync(int id, StudentRequest request)
        {
            var student = await context.Students.FindAsync(id);
            if (student == null)
            {
                return null;
            }

            student.FullName = request.FullName.Trim();
            student.Email = request.Email.Trim();
            student.DateOfBirth = request.DateOfBirth;
            student.PhoneNumber = request.PhoneNumber?.Trim();
            student.StudentCode = request.StudentCode?.Trim();

            await context.SaveChangesAsync();

            return new StudentResponse
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth
            };
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await context.Students.FindAsync(id);
            if (student == null)
            {
                return false;
            }

            context.Students.Remove(student);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
