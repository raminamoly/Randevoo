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
        _logger.LogInformation("Email confirmation message queued for {EmailDomain}", GetDomain(email));
        return Task.CompletedTask;
    }

    private static string GetDomain(string email)
    {
        var atIndex = email.LastIndexOf('@');
        return atIndex >= 0 && atIndex < email.Length - 1 ? email[(atIndex + 1)..] : "unknown";
    }
}
