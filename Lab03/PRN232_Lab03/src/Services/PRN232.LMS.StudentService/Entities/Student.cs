namespace PRN232.LMS.StudentService.Entities
{
    public class Student
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? StudentCode { get; set; }
    }
}
