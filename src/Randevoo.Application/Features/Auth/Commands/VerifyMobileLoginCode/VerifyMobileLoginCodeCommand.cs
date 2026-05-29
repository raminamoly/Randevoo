using MediatR;
using Randevoo.Application.Features.Auth.Common;

namespace Randevoo.Application.Features.Auth.Commands.VerifyMobileLoginCode;

public record VerifyMobileLoginCodeCommand(string MobileNumber, string Code) : IRequest<AuthResult>;
