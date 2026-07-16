using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PRN232.LMS.CourseService.Consumers;
using PRN232.LMS.CourseService.Data;
using PRN232.LMS.Shared.Events;
using Xunit;

namespace PRN232.LMS.Tests.Consumers
{
    public class StudentCreatedConsumerTests
    {
        [Fact]
        public async Task Consume_PersistsReceivedStudentEventRow()
        {
            var dbContext = NewInMemoryDbContext();
            var consumer = new StudentCreatedConsumer(NullLogger<StudentCreatedConsumer>.Instance, dbContext);

            var messageId = Guid.NewGuid();
            var occurredAt = new DateTime(2026, 7, 16, 12, 0, 0, DateTimeKind.Utc);
            var @event = new StudentCreatedIntegrationEvent(
                StudentId: 42,
                StudentCode: "SE12345",
                FullName: "Nguyen Van A",
                Email: "a@lms.local",
                OccurredAt: occurredAt);

            var contextMock = new Mock<ConsumeContext<StudentCreatedIntegrationEvent>>();
            contextMock.SetupGet(c => c.Message).Returns(@event);
            contextMock.SetupGet(c => c.MessageId).Returns(messageId);

            await consumer.Consume(contextMock.Object);

            var saved = await dbContext.ReceivedStudentEvents.AsNoTracking().ToListAsync();
            Assert.Single(saved);
            var row = saved[0];
            Assert.Equal(42, row.StudentId);
            Assert.Equal("SE12345", row.StudentCode);
            Assert.Equal("Nguyen Van A", row.FullName);
            Assert.Equal("a@lms.local", row.Email);
            Assert.Equal(messageId, row.MessageId);
        }

        [Fact]
        public async Task Consume_WithNullMessageId_PersistsNullMessageId()
        {
            var dbContext = NewInMemoryDbContext();
            var consumer = new StudentCreatedConsumer(NullLogger<StudentCreatedConsumer>.Instance, dbContext);

            var @event = new StudentCreatedIntegrationEvent(
                StudentId: 7,
                StudentCode: "SE7777",
                FullName: "Tran Thi B",
                Email: "b@lms.local",
                OccurredAt: DateTime.UtcNow);

            var contextMock = new Mock<ConsumeContext<StudentCreatedIntegrationEvent>>();
            contextMock.SetupGet(c => c.Message).Returns(@event);
            contextMock.SetupGet(c => c.MessageId).Returns((Guid?)null);

            await consumer.Consume(contextMock.Object);

            var saved = await dbContext.ReceivedStudentEvents.AsNoTracking().ToListAsync();
            Assert.Single(saved);
            Assert.Equal(7, saved[0].StudentId);
            Assert.Null(saved[0].MessageId);
        }

        private static CourseDbContext NewInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<CourseDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new CourseDbContext(options);
        }
    }
}
