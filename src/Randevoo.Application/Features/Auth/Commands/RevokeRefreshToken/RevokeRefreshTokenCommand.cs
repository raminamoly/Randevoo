using MediatR;

namespace Randevoo.Application.Features.Auth.Commands.RevokeRefreshToken;

public record RevokeRefreshTokenCommand(string RefreshToken) : IRequest;
