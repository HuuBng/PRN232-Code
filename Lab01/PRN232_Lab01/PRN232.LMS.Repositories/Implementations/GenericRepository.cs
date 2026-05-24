using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Interfaces;
using System.Linq.Expressions;
namespace PRN232.LMS.Repositories.Implementations
{
    /// <summary>
    ///     Cài đặt repository dùng chung bằng Entity Framework Core DbSet.
    ///     Lớp này chỉ chịu trách nhiệm truy cập dữ liệu.
    /// </summary>
    public class GenericRepository<T>(AppDbContext context) : IGenericRepository<T> where T : class
    {
        /// <summary>
        ///     DbSet tương ứng với entity T.
        /// </summary>
        private readonly DbSet<T> _dbSet = context.Set<T>();

        /// <summary>
        ///     Trả về IQueryable để service có thể xây dựng truy vấn trước khi execute.
        /// </summary>
        public IQueryable<T> GetAll()
        {
            return _dbSet.AsQueryable();
        }

        /// <summary>
        ///     Tìm entity bằng primary key.
        /// </summary>
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        ///     Tìm entity đầu tiên theo điều kiện cụ thể.
        /// </summary>
        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        ///     Thêm entity mới vào DbSet.
        /// </summary>
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        /// <summary>
        ///     Cập nhật entity đang được theo dõi hoặc attach entity mới vào trạng thái Modified.
        /// </summary>
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        /// <summary>
        ///     Xóa entity khỏi DbSet.
        /// </summary>
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        /// <summary>
        ///     Kiểm tra entity tồn tại theo primary key.
        /// </summary>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.FindAsync(id) != null;
        }
    }
}
