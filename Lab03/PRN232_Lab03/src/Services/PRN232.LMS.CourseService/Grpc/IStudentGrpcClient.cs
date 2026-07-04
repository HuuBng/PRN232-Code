using PRN232.LMS.Protos;

namespace PRN232.LMS.CourseService.Grpc
{
    public interface IStudentGrpcClient
    {
        Task<StudentGrpcResponse?> GetStudentByIdAsync(int studentId);
        Task<bool> CheckStudentExistsAsync(int studentId);
    }
}
