namespace PRN232.LMS.Shared.Events;

/// <summary>
/// Integration event published by StudentService whenever a new student is created.
/// Consumed asynchronously by other services (e.g. CourseService) via MassTransit / RabbitMQ.
/// </summary>
public record StudentCreatedIntegrationEvent(
    int StudentId,
    string StudentCode,
    string FullName,
    string Email,
    DateTime OccurredAt);

/// <summary>
/// Integration event published by StudentService whenever an existing student is updated.
/// </summary>
public record StudentUpdatedIntegrationEvent(
    int StudentId,
    string StudentCode,
    string FullName,
    string Email,
    DateTime OccurredAt);

/// <summary>
/// Integration event published by StudentService whenever a student is deleted.
/// </summary>
public record StudentDeletedIntegrationEvent(
    int StudentId,
    DateTime OccurredAt);
