using MediatR;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.Features.Users.Commands.ChangeUserRole;

public record ChangeUserRoleCommand(long UserId, UserRole Role) : IRequest;
