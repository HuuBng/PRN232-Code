namespace PRN232.LMS.Shared.Models
{
    public class PaginationMetadata
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }

    public class PaginatedResponse<T>
    {
        public IEnumerable<T> Items { get; set; } = [];
        public PaginationMetadata Pagination { get; set; } = new PaginationMetadata();
    }
}
