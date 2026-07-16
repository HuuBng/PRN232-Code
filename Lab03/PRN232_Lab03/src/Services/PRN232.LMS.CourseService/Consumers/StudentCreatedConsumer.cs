using MassTransit;
using PRN232.LMS.CourseService.Data;
using PRN232.LMS.CourseService.Entities;
using PRN232.LMS.Shared.Events;

namespace PRN232.LMS.CourseService.Consumers;

/// <summary>
/// MassTransit consumer that persists a <see cref="ReceivedStudentEvent"/> row
/// every time a <see cref="StudentCreatedIntegrationEvent"/> is delivered
/// via RabbitMQ. This provides observable evidence in the CourseService DB
/// that the asynchronous message was received and processed.
/// </summary>
public class StudentCreatedConsumer : IConsumer<StudentCreatedIntegrationEvent>
{
    private readonly ILogger<StudentCreatedConsumer> _logger;
    private readonly CourseDbContext _dbContext;

    public StudentCreatedConsumer(
        ILogger<StudentCreatedConsumer> logger,
        CourseDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<StudentCreatedIntegrationEvent> context)
    {
        var message = context.Message;
        var messageId = context.MessageId ?? Guid.Empty;

        _logger.LogInformation(
            "Received {EventType} for StudentId={StudentId}, StudentCode={StudentCode}, FullName={FullName}, Email={Email}, MessageId={MessageId}, OccurredAt={OccurredAt:o}",
            nameof(StudentCreatedIntegrationEvent),
            message.StudentId,
            message.StudentCode,
            message.FullName,
            message.Email,
            messageId,
            message.OccurredAt);

        var row = new ReceivedStudentEvent
        {
            StudentId = message.StudentId,
            StudentCode = message.StudentCode ?? string.Empty,
            FullName = message.FullName ?? string.Empty,
            Email = message.Email ?? string.Empty,
            ReceivedAt = DateTime.UtcNow,
            MessageId = messageId == Guid.Empty ? null : messageId
        };

        _dbContext.ReceivedStudentEvents.Add(row);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Persisted ReceivedStudentEvent row Id={RowId} for StudentId={StudentId}",
            row.Id,
            row.StudentId);
    }
}
