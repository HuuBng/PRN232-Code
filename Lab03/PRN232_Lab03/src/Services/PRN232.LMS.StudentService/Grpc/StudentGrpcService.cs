using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Protos;
using PRN232.LMS.StudentService.Data;

namespace PRN232.LMS.StudentService.Grpc
{
    public class StudentGrpcService(StudentDbContext context) : StudentGrpc.StudentGrpcBase
    {
        public override async Task<StudentGrpcResponse> GetStudentById(GetStudentByIdRequest request, ServerCallContext contextGrpc)
        {
            var student = await context.Students.AsNoTracking()
                .FirstOrDefaultAsync(s => s.StudentId == request.StudentId);

            if (student == null)
            {
                return new StudentGrpcResponse
                {
                    StudentId = request.StudentId,
                    Exists = false
                };
            }

            return new StudentGrpcResponse
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth.ToString("yyyy-MM-dd"),
                PhoneNumber = student.PhoneNumber ?? string.Empty,
                StudentCode = student.StudentCode ?? string.Empty,
                Exists = true
            };
        }

        public override async Task<StudentExistsResponse> CheckStudentExists(CheckStudentExistsRequest request, ServerCallContext contextGrpc)
        {
            var exists = await context.Students.AsNoTracking()
                .AnyAsync(s => s.StudentId == request.StudentId);

            return new StudentExistsResponse
            {
                Exists = exists,
                StudentId = request.StudentId
            };
        }
    }
}
