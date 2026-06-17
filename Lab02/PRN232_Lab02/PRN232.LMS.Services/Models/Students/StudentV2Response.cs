namespace PRN232.LMS.Services.Models.Students
{
    /// <summary>
    ///     v2 response: superset of <see cref="StudentResponse"/> that exposes
    ///     the optional <c>PhoneNumber</c> and <c>StudentCode</c> fields.
    /// </summary>
    public class StudentV2Response
    {
        public int StudentId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string? PhoneNumber { get; set; }

        public string? StudentCode { get; set; }

        public IEnumerable<StudentEnrollmentResponse>? Enrollments { get; set; }
    }
}
