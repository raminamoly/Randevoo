using MediatR;

namespace Randevoo.Application.Features.DatingEvents.Commands.SetDatingEventCommission;

public record SetDatingEventCommissionCommand(long EventId, decimal CommissionPercent) : IRequest;
