using MediatR;

namespace Randevoo.Application.Features.Auth.Commands.RequestMobileLoginCode;

public record RequestMobileLoginCodeCommand(string MobileNumber) : IRequest;
