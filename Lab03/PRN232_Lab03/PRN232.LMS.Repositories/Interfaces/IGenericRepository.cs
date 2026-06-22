using System.Linq.Expressions;
namespace PRN232.LMS.Repositories.Interfaces
{
    /// <summary>
    ///     Cung cấp các thao tác truy cập dữ liệu dùng chung cho mọi entity.
    ///     Repository chỉ làm việc với Entity Model, không chứa business logic hoặc response model.
    /// </summary>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        ///     Lấy nguồn truy vấn để service layer áp dụng search, sort, paging, include.
        /// </summary>
        IQueryable<T> GetAll();

        /// <summary>
        ///     Tìm entity theo khóa chính kiểu int.
        /// </summary>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        ///     Tìm entity đầu tiên thỏa điều kiện truyền vào.
        /// </summary>
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        ///     Thêm entity mới vào DbContext.
        /// </summary>
        Task AddAsync(T entity);

        /// <summary>
        ///     Đánh dấu entity đã thay đổi để cập nhật xuống database.
        /// </summary>
        void Update(T entity);

        /// <summary>
        ///     Xóa entity khỏi DbContext.
        /// </summary>
        void Delete(T entity);

        /// <summary>
        ///     Kiểm tra entity có tồn tại theo khóa chính hay không.
        /// </summary>
        Task<bool> ExistsAsync(int id);
    }
}
