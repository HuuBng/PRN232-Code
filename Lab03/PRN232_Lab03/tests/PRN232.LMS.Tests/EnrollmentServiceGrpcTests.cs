using Microsoft.EntityFrameworkCore;
using Moq;
using PRN232.LMS.CourseService.Data;
using PRN232.LMS.CourseService.Entities;
using PRN232.LMS.CourseService.Grpc;
using PRN232.LMS.CourseService.Models.Enrollments;
using PRN232.LMS.CourseService.Services;
using PRN232.LMS.Protos;
using Xunit;

namespace PRN232.LMS.Tests
{
    public class EnrollmentServiceGrpcTests
    {
        private readonly CourseDbContext _dbContext;
        private readonly Mock<IStudentGrpcClient> _grpcClientMock;
        private readonly EnrollmentService _service;

        public EnrollmentServiceGrpcTests()
        {
            var options = new DbContextOptionsBuilder<CourseDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new CourseDbContext(options);

            // Seed test data
            var semester = new Semester { SemesterName = "Test Semester", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(3) };
            var subject = new Subject { SubjectCode = "TST101", SubjectName = "Test Subject", Credit = 3 };
            _dbContext.Semesters.Add(semester);
            _dbContext.Subjects.Add(subject);
            _dbContext.SaveChanges();

            var course = new Course { CourseName = "Test Course", SemesterId = semester.SemesterId, SubjectId = subject.SubjectId };
            _dbContext.Courses.Add(course);
            _dbContext.SaveChanges();

            _grpcClientMock = new Mock<IStudentGrpcClient>();
            _service = new EnrollmentService(_dbContext, _grpcClientMock.Object);
        }

        [Fact]
        public async Task CreateEnrollment_WithValidStudentViaGrpc_CreatesSuccessfully()
        {
            // Arrange
            var course = _dbContext.Courses.First();
            _grpcClientMock.Setup(x => x.CheckStudentExistsAsync(1)).ReturnsAsync(true);

            var request = new EnrollmentRequest
            {
                StudentId = 1,
                CourseId = course.CourseId,
                EnrollDate = DateTime.Now,
                Status = "Active"
            };

            // Act
            var result = await _service.CreateEnrollmentAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.StudentId);
            Assert.Equal(course.CourseId, result.CourseId);
            _grpcClientMock.Verify(x => x.CheckStudentExistsAsync(1), Times.Once);
        }

        [Fact]
        public async Task CreateEnrollment_WithInvalidStudentViaGrpc_ThrowsException()
        {
            // Arrange
            var course = _dbContext.Courses.First();
            _grpcClientMock.Setup(x => x.CheckStudentExistsAsync(999)).ReturnsAsync(false);

            var request = new EnrollmentRequest
            {
                StudentId = 999,
                CourseId = course.CourseId,
                EnrollDate = DateTime.Now,
                Status = "Active"
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<EnrollmentValidationException>(
                () => _service.CreateEnrollmentAsync(request));
            Assert.Contains("Student with id 999 does not exist", ex.Message);
            _grpcClientMock.Verify(x => x.CheckStudentExistsAsync(999), Times.Once);
        }

        [Fact]
        public async Task GetEnrollmentById_WithExpandStudent_FetchesViaGrpc()
        {
            // Arrange
            var course = _dbContext.Courses.First();
            var enrollment = new Enrollment
            {
                StudentId = 1,
                CourseId = course.CourseId,
                EnrollDate = DateTime.Now,
                Status = "Active"
            };
            _dbContext.Enrollments.Add(enrollment);
            _dbContext.SaveChanges();

            _grpcClientMock.Setup(x => x.GetStudentByIdAsync(1))
                .ReturnsAsync(new StudentGrpcResponse
                {
                    StudentId = 1,
                    FullName = "Test Student",
                    Email = "test@lms.local",
                    Exists = true
                });

            // Act
            var result = await _service.GetEnrollmentByIdAsync(enrollment.EnrollmentId, "student");

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Student);
            Assert.Equal("Test Student", result.Student.FullName);
            Assert.Equal("test@lms.local", result.Student.Email);
            _grpcClientMock.Verify(x => x.GetStudentByIdAsync(1), Times.Once);
        }
    }
}
