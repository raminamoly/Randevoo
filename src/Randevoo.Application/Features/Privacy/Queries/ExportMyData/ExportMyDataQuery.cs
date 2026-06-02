using MediatR;
using Randevoo.Application.Features.Privacy.Common;

namespace Randevoo.Application.Features.Privacy.Queries.ExportMyData;

public record ExportMyDataQuery(long UserId) : IRequest<PrivacyExportDto>;
