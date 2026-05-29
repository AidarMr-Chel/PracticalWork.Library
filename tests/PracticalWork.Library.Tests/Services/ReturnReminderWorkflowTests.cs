using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Models;
using PracticalWork.Library.Services;

namespace PracticalWork.Library.Tests.Services;

public class ReturnReminderWorkflowTests
{
    [Fact]
    public async Task ExecuteAsync_WhenNoReminders_DoesNotSendEmail()
    {
        var fixedTime = new DateTimeOffset(2026, 5, 15, 12, 0, 0, TimeSpan.Zero);
        var timeProvider = new FakeTimeProvider(fixedTime);

        var reminderRepo = new Mock<IReminderRepository>();
        reminderRepo.Setup(r => r.GetRemindersForDueDateAsync(It.IsAny<DateOnly>(), default))
            .ReturnsAsync(Array.Empty<ReminderData>());

        var email = new Mock<IEmailService>();
        var templates = new Mock<IEmailTemplateService>();

        var sut = CreateSut(reminderRepo.Object, email.Object, templates.Object, timeProvider);

        var result = await sut.ExecuteAsync();

        result.IsSuccess.Should().BeTrue();
        result.ProcessedCount.Should().Be(0);
        email.Verify(e => e.SendAsync(It.IsAny<EmailMessage>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRemindersExist_SendsWithoutRealDelay()
    {
        var fixedTime = new DateTimeOffset(2026, 5, 15, 12, 0, 0, TimeSpan.Zero);
        var timeProvider = new FakeTimeProvider(fixedTime);

        var reminders = new[]
        {
            new ReminderData(
                Guid.NewGuid(),
                "Reader",
                "reader@test.com",
                new[] { "Author" },
                "Book",
                DateOnly.FromDateTime(fixedTime.Date).AddDays(3),
                3)
        };

        var reminderRepo = new Mock<IReminderRepository>();
        reminderRepo.Setup(r => r.GetRemindersForDueDateAsync(It.IsAny<DateOnly>(), default))
            .ReturnsAsync(reminders);

        var email = new Mock<IEmailService>();
        email.Setup(e => e.SendAsync(It.IsAny<EmailMessage>()))
            .ReturnsAsync(EmailSendResult.Success());

        var templates = new Mock<IEmailTemplateService>();
        templates.Setup(t => t.RenderAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync("body");

        var sut = CreateSut(reminderRepo.Object, email.Object, templates.Object, timeProvider);

        var result = await sut.ExecuteAsync();

        result.ProcessedCount.Should().Be(1);
        email.Verify(e => e.SendAsync(It.Is<EmailMessage>(m => m.To == "reader@test.com")), Times.Once);
        templates.Verify(t => t.RenderAsync("ReturnReminder.html", It.IsAny<Dictionary<string, string>>()), Times.Once);
    }

    private static ReturnReminderWorkflow CreateSut(
        IReminderRepository reminderRepository,
        IEmailService emailService,
        IEmailTemplateService templateService,
        TimeProvider timeProvider)
    {
        var settings = Options.Create(new EmailTemplateSettings
        {
            ReturnReminder = new ReturnReminderTemplate
            {
                DaysBeforeDueDate = 3,
                SubjectTemplate = "Return {BookTitle}",
                LibraryName = "Lib",
                LibraryAddress = "Addr",
                LibraryPhone = "Phone",
                WorkingHours = "9-18"
            }
        });

        return new ReturnReminderWorkflow(
            reminderRepository,
            emailService,
            templateService,
            settings,
            timeProvider,
            NullLogger<ReturnReminderWorkflow>.Instance);
    }

    private sealed class FakeTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}
