using Microsoft.Extensions.Logging;
using Randevoo.Application.Interfaces.Notifications;

namespace Randevoo.Infrastructure.Services;

public class ConsoleEmailSender : IEmailSender
{
    private readonly ILogger<ConsoleEmailSender> _logger;

    public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendEmailConfirmationAsync(string email, string confirmationLink, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Email confirmation for {Email}: {ConfirmationLink}", email, confirmationLink);
        return Task.CompletedTask;
    }
}
