using System.Xml.Serialization;

namespace PRN232.LMS.Shared.Models
{
    public class PaginationMetadata
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }

    [XmlType("PaginatedResponse")]
    public class PaginatedResponse<T>
    {
        [XmlArray("Items")]
        [XmlArrayItem("Item")]
        public List<T> Items { get; set; } = [];
        public PaginationMetadata Pagination { get; set; } = new PaginationMetadata();
    }
}
