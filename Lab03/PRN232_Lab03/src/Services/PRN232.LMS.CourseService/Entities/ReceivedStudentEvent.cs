namespace PRN232.LMS.CourseService.Entities;

/// <summary>
/// Observable evidence row written by <c>StudentCreatedConsumer</c> every time
/// CourseService receives a <c>StudentCreatedIntegrationEvent</c> from the broker.
/// Lives in the CourseService database and is independent of the Student entity.
/// </summary>
public class ReceivedStudentEvent
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public string StudentCode { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime ReceivedAt { get; set; }

    /// <summary>
    /// Optional MassTransit message identifier for traceability.
    /// </summary>
    public Guid? MessageId { get; set; }
}
