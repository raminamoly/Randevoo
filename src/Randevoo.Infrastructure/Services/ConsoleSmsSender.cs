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
        _logger.LogInformation("Login code SMS queued for mobile ending {MobileLast4}", Last4(mobileNumber));
        return Task.CompletedTask;
    }

    public Task SendMessageAsync(string mobileNumber, string message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("SMS queued for mobile ending {MobileLast4}", Last4(mobileNumber));
        return Task.CompletedTask;
    }

    private static string Last4(string value)
    {
        var trimmed = value.Trim();
        return trimmed.Length <= 4 ? trimmed : trimmed[^4..];
    }
}
