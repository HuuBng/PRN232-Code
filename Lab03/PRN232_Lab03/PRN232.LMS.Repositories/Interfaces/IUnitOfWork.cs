using PRN232.LMS.Repositories.Entities;
namespace PRN232.LMS.Repositories.Interfaces
{
    /// <summary>
    ///     Quản lý các repository và gom việc lưu thay đổi vào một giao dịch làm việc.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        ///     Repository thao tác với sinh viên.
        /// </summary>
        IGenericRepository<Student> Students { get; }

        /// <summary>
        ///     Repository thao tác với môn học.
        /// </summary>
        IGenericRepository<Subject> Subjects { get; }

        /// <summary>
        ///     Repository thao tác với học kỳ.
        /// </summary>
        IGenericRepository<Semester> Semesters { get; }

        /// <summary>
        ///     Repository thao tác với khóa học.
        /// </summary>
        IGenericRepository<Course> Courses { get; }

        /// <summary>
        ///     Repository thao tác với đăng ký học phần.
        /// </summary>
        IGenericRepository<Enrollment> Enrollments { get; }

        /// <summary>
        ///     Repository thao tác với người dùng xác thực.
        /// </summary>
        IGenericRepository<User> Users { get; }

        /// <summary>
        ///     Repository thao tác với refresh token.
        /// </summary>
        IGenericRepository<RefreshToken> RefreshTokens { get; }

        /// <summary>
        ///     Lưu toàn bộ thay đổi của các repository xuống database.
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}
