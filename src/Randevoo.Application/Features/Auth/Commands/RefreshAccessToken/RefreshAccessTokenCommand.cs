using MediatR;
using Randevoo.Application.Features.Auth.Common;

namespace Randevoo.Application.Features.Auth.Commands.RefreshAccessToken;

public record RefreshAccessTokenCommand(string RefreshToken) : IRequest<AuthResult>;
