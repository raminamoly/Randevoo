namespace Randevoo.ControlCenter.Models.Moderation;

public sealed record ModerationQueueItem(Guid Id, string Subject, string Reason, DateTimeOffset CreatedAt);
