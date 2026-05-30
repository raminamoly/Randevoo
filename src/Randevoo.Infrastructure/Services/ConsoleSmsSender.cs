using Microsoft.Extensions.Logging;
using Randevoo.Application.Interfaces.Notifications;

namespace Randevoo.Infrastructure.Services;

public class ConsoleSmsSender : ISmsSender
{
    private readonly ILogger<ConsoleSmsSender> _logger;

    public ConsoleSmsSender(ILogger<ConsoleSmsSender> logger)
    {
        _logger = logger;
    }

    public Task SendLoginCodeAsync(string mobileNumber, string code, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Login code for {MobileNumber}: {Code}", mobileNumber, code);
        return Task.CompletedTask;
    }

    public Task SendMessageAsync(string mobileNumber, string message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("SMS to {MobileNumber}: {Message}", mobileNumber, message);
        return Task.CompletedTask;
    }
}
