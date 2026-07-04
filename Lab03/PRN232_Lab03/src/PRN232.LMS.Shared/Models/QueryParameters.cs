namespace PRN232.LMS.Shared.Models
{
    public class QueryParameters
    {
        public string? Search { get; set; }
        public string? Sort { get; set; }
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public string? Fields { get; set; }
        public string? Expand { get; set; }

        public int ValidPage
        {
            get => Page < 1 ? 1 : Page;
        }

        public int ValidSize
        {
            get => Size < 1 ? 10 : Size > 100 ? 100 : Size;
        }
    }
}
