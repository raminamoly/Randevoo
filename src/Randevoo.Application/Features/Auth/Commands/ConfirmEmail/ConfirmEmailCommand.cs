using MediatR;

namespace Randevoo.Application.Features.Auth.Commands.ConfirmEmail;

public record ConfirmEmailCommand(long UserId, string Token) : IRequest;
