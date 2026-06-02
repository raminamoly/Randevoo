using MediatR;

namespace Randevoo.Application.Features.Privacy.Commands.DeleteMyAccount;

public record DeleteMyAccountCommand(long UserId) : IRequest;
