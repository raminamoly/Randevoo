namespace Randevoo.Application.Interfaces.Notifications;

public interface IEmailSender
{
    Task SendEmailConfirmationAsync(string email, string confirmationLink, CancellationToken cancellationToken = default);
}
