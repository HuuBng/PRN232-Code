using Grpc.Core;
using PRN232.LMS.Protos;

namespace PRN232.LMS.CourseService.Grpc
{
    public class StudentGrpcClient(StudentGrpc.StudentGrpcClient client, ILogger<StudentGrpcClient> logger) : IStudentGrpcClient
    {
        public async Task<StudentGrpcResponse?> GetStudentByIdAsync(int studentId)
        {
            try
            {
                var response = await client.GetStudentByIdAsync(new GetStudentByIdRequest { StudentId = studentId });
                return response.Exists ? response : null;
            }
            catch (RpcException ex)
            {
                logger.LogWarning(ex, "gRPC error calling GetStudentById for student {StudentId}", studentId);
                return null;
            }
        }

        public async Task<bool> CheckStudentExistsAsync(int studentId)
        {
            try
            {
                var response = await client.CheckStudentExistsAsync(new CheckStudentExistsRequest { StudentId = studentId });
                return response.Exists;
            }
            catch (RpcException ex)
            {
                logger.LogWarning(ex, "gRPC error calling CheckStudentExists for student {StudentId}", studentId);
                return false;
            }
        }
    }
}
