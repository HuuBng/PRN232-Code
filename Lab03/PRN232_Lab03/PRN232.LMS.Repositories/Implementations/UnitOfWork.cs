using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
namespace PRN232.LMS.Repositories.Implementations
{
    /// <summary>
    ///     Cài đặt Unit of Work để dùng chung một DbContext cho nhiều repository.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        /// <summary>
        ///     Khởi tạo Unit of Work với DbContext hiện tại.
        /// </summary>
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Students = new GenericRepository<Student>(_context);
            Subjects = new GenericRepository<Subject>(_context);
            Semesters = new GenericRepository<Semester>(_context);
            Courses = new GenericRepository<Course>(_context);
            Enrollments = new GenericRepository<Enrollment>(_context);
            Users = new GenericRepository<User>(_context);
            RefreshTokens = new GenericRepository<RefreshToken>(_context);
        }

        /// <inheritdoc />
        public IGenericRepository<Student> Students { get; }

        /// <inheritdoc />
        public IGenericRepository<Subject> Subjects { get; }

        /// <inheritdoc />
        public IGenericRepository<Semester> Semesters { get; }

        /// <inheritdoc />
        public IGenericRepository<Course> Courses { get; }

        /// <inheritdoc />
        public IGenericRepository<Enrollment> Enrollments { get; }

        /// <inheritdoc />
        public IGenericRepository<User> Users { get; }

        /// <inheritdoc />
        public IGenericRepository<RefreshToken> RefreshTokens { get; }

        /// <inheritdoc />
        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
