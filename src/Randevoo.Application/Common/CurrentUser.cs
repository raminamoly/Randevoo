using Randevoo.Domain.Enums;

namespace Randevoo.Application.Common;

public record CurrentUser(long UserId, UserRole Role);
