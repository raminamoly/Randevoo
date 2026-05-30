namespace Randevoo.Application.Interfaces.Notifications;

public interface ISmsSender
{
    Task SendLoginCodeAsync(string mobileNumber, string code, CancellationToken cancellationToken = default);
    Task SendMessageAsync(string mobileNumber, string message, CancellationToken cancellationToken = default);
}
