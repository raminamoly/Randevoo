using MediatR;

namespace Randevoo.Application.Features.Auth.Commands.RequestEmailConfirmation;

public record RequestEmailConfirmationCommand(long UserId, string Email, string ConfirmationBaseUrl) : IRequest;
