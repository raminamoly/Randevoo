# Indexes And Constraints

## Purpose
List index/constraint declarations visible in DbContext.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

- `b.HasIndex(u => u.MobileNumber).IsUnique();`
- `b.HasIndex(u => u.Email).IsUnique() .HasFilter("[Email] IS NOT NULL");`
- `b.HasIndex(token => token.TokenHash).IsUnique();`
- `b.HasIndex(token => token.UserId);`
- `b.HasIndex(log => log.ActorUserId);`
- `b.HasIndex(log => new { log.LogType, log.CreatedAt });`
- `b.HasIndex(log => new { log.Module, log.CreatedAt });`
- `b.HasIndex(log => new { log.Status, log.CreatedAt });`
- `b.HasIndex(log => new { log.TargetType, log.TargetId });`
- `b.HasIndex(log => log.CreatedAt);`
- `b.HasIndex(role => role.Name).IsUnique();`
- `b.HasIndex(action => new { action.Entity, action.Action }).IsUnique();`
- `b.HasIndex(permission => new { permission.Role, permission.Entity, permission.Action }).IsUnique();`
- `b.HasIndex(permission => new { permission.Entity, permission.Action });`
- `b.HasIndex(permission => new { permission.UserId, permission.Entity, permission.Action }).IsUnique();`
- `b.HasIndex(permission => new { permission.Entity, permission.Action });`
- `b.HasIndex(permission => permission.ExpiresAtUtc);`
- `b.HasIndex(status => status.Name).IsUnique();`
- `b.HasIndex(type => type.Name).IsUnique();`
- `b.HasIndex(type => type.Name).IsUnique();`
- `b.HasIndex(currency => currency.Code).IsUnique();`
- `b.HasIndex(rate => new { rate.FromCurrencyCode, rate.ToCurrencyCode, rate.EffectiveFromUtc }).IsUnique();`
- `b.HasIndex(rate => new { rate.FromCurrencyCode, rate.ToCurrencyCode, rate.EffectiveToUtc });`
- `b.HasIndex(country => country.Name).IsUnique();`
- `b.HasIndex(country => country.Code).IsUnique();`
- `b.HasIndex(city => new { city.CountryId, city.Name }).IsUnique();`
- `b.HasIndex(level => level.Title).IsUnique();`
- `b.HasIndex(gender => gender.Title).IsUnique();`
- `b.HasIndex(sign => sign.Code).IsUnique();`
- `b.HasIndex(sign => sign.Title).IsUnique();`
- `b.HasIndex(p => p.UserId).IsUnique();`
- `b.HasIndex(mode => mode.Name).IsUnique();`
- `b.HasIndex(platform => platform.Name).IsUnique();`
- `b.HasIndex(a => a.UserId).IsUnique();`
- `b.HasIndex(t => t.UserId);`
- `b.HasIndex(t => t.CurrencyCode);`
- `b.HasIndex(t => t.ExchangeRateId);`
- `b.HasIndex(t => t.TicketOrderId);`
- `b.HasIndex(payment => payment.UserId);`
- `b.HasIndex(payment => payment.DatingEventId);`
- `b.HasIndex(payment => payment.EventTicketId);`
- `b.HasIndex(payment => payment.TicketOrderId);`
- `b.HasIndex(payment => payment.BalanceTransactionId);`
- `b.HasIndex(payment => payment.CurrencyCode);`
- `b.HasIndex(payment => payment.ExchangeRateId);`
- `b.HasIndex(payment => payment.TrackingCode).IsUnique();`
- `b.HasIndex(receipt => new { receipt.DestinationType, receipt.Status, receipt.SubmittedAtUtc });`
- `b.HasIndex(receipt => receipt.DatingEventId);`
- `b.HasIndex(receipt => receipt.ParticipantUserId);`
- `b.HasIndex(receipt => receipt.PlannerUserId);`
- `b.HasIndex(receipt => receipt.EventTicketId);`
- `b.HasIndex(receipt => receipt.TicketOrderId);`
- `b.HasIndex(receipt => receipt.EventDiscountCodeId);`
- `b.HasIndex(receipt => receipt.CurrencyCode);`
- `b.HasIndex(receipt => receipt.ExchangeRateId);`
- `b.HasIndex(request => request.CurrencyCode);`
- `b.HasIndex(request => request.ExchangeRateId);`
- `b.HasIndex(request => request.UserId);`
- `b.HasIndex(request => new { request.Status, request.RequestedAtUtc });`
- `b.HasIndex(account => account.UserId);`
- `b.HasIndex(account => account.CurrencyCode);`
- `b.HasIndex(account => account.Iban).IsUnique().HasFilter("[Iban] IS NOT NULL");`
- `b.HasIndex(p => p.DisplayName).IsUnique();`
- `b.HasIndex(p => p.UserId).IsUnique();`
- `b.HasIndex(p => p.CountryId);`
- `b.HasIndex(p => p.CityId);`
- `b.HasIndex(p => p.EducationLevelId);`
- `b.HasIndex(p => p.GenderId);`
- `b.HasIndex(p => p.ZodiacSignId);`
- `b.HasIndex(image => new { image.UserProfileId, image.DisplayOrder }).IsUnique();`
- `b.HasIndex(i => i.Name).IsUnique();`
- `b.HasIndex(e => e.EventTypeId);`
- `b.HasIndex(e => e.EventModeId);`
- `b.HasIndex(e => e.OnlineEventPlatformId);`
- `b.HasIndex(e => e.CountryId);`
- `b.HasIndex(e => e.CityId);`
- `b.HasIndex(e => e.MinimumEducationLevelId);`
- `b.HasIndex(e => new { e.IsCancelled, e.DateTimeEnd });`
- `b.HasIndex(e => new { e.IsCancelled, e.IsOpenForSell, e.DateTimeEnd });`
- `b.HasIndex(e => new { e.ReviewStatus, e.DateTimeStart });`
- `b.HasIndex(e => new { e.DateTimeStart, e.Id });`
- `b.HasIndex(e => new { e.UpdatedAt, e.Id });`
- `b.HasIndex(e => e.EventPlannerUserId);`
- `b.HasIndex(e => e.CurrencyCode);`
- `b.HasIndex(e => e.MaleTicketCurrencyCode);`
- `b.HasIndex(e => e.FemaleTicketCurrencyCode);`
- `b.HasIndex(faq => new { faq.DatingEventId, faq.DisplayOrder }).IsUnique();`
- `b.HasIndex(discountCode => new { discountCode.DatingEventId, discountCode.Code }).IsUnique();`
- `b.HasIndex(discountCode => discountCode.Code) .IsUnique() .HasDatabaseName("IX_EventDiscountCodes_Global_Code") .HasFilter("[DatingEventId] IS NULL");`
- `b.HasIndex(discountCode => new { discountCode.IsActive, discountCode.StartsAtUtc, discountCode.EndsAtUtc });`
- `b.HasIndex(like => new { like.DatingEventId, like.FromUserId, like.ToUserId }).IsUnique();`
- `b.HasIndex(like => new { like.DatingEventId, like.ToUserId, like.Status });`
- `b.HasIndex(tag => tag.Name).IsUnique();`
- `b.HasIndex(eventTag => new { eventTag.DatingEventId, eventTag.TagId }).IsUnique();`
- `b.HasIndex(eventTag => eventTag.TagId);`
- `b.HasIndex(order => order.DatingEventId);`
- `b.HasIndex(order => order.BuyerUserId);`
- `b.HasIndex(order => new { order.PaymentStatus, order.CreatedAt });`
- `b.HasIndex(order => new { order.OrderStatus, order.CreatedAt });`
- `b.HasIndex(order => order.CurrencyCode);`
- `b.HasIndex(order => order.ExchangeRateId);`
- `b.HasIndex(order => order.EventDiscountCodeId);`
- `b.HasIndex(order => order.ApprovedByUserId);`
- `b.HasIndex(t => new { t.DatingEventId, t.UserId }).IsUnique();`
- `b.HasIndex(t => t.TicketOrderId);`
- `b.HasIndex(t => t.CurrencyCode);`
- `b.HasIndex(t => t.ExchangeRateId);`
- `b.HasIndex(t => t.EventDiscountCodeId);`
- `b.HasIndex(c => new { c.DatingEventId, c.StarterUserId, c.ParticipantUserId }).IsUnique();`
- `b.HasIndex(m => m.EventConversationId);`
- `b.HasIndex(block => new { block.EventConversationId, block.BlockerUserId, block.BlockedUserId }).IsUnique();`
- `b.HasIndex(response => new { response.DatingEventId, response.UserId }).IsUnique();`
- `b.HasIndex(rating => new { rating.EventSurveyResponseId, rating.Factor }).IsUnique();`
- `b.HasIndex(type => type.Name).IsUnique();`
- `b.HasIndex(report => report.Status);`
- `b.HasIndex(report => report.ReporterUserId);`
- `b.HasIndex(report => report.ReportedUserId);`
- `b.HasIndex(ticket => new { ticket.Status, ticket.Category, ticket.CreatedAt });`
- `b.HasIndex(ticket => new { ticket.TicketStatusId, ticket.TicketTypeId, ticket.TicketRecipientTypeId, ticket.CreatedAt });`
- `b.HasIndex(ticket => ticket.SubmitterUserId);`
- `b.HasIndex(ticket => ticket.AssignedSupportUserId);`
- `b.HasIndex(ticket => ticket.RecipientPlannerUserId);`
- `b.HasIndex(ticket => ticket.DatingEventId);`
- `b.HasIndex(status => status.Name).IsUnique();`
- `b.HasIndex(category => category.Name).IsUnique();`
- `b.HasIndex(recipient => recipient.Name).IsUnique();`
- `b.HasIndex(message => message.SupportTicketId);`
- `b.HasIndex(history => history.SupportTicketId);`
- `b.HasIndex(cursor => cursor.QueueName).IsUnique();`
- `b.HasIndex(request => new { request.DatingEventId, request.Status, request.CreatedAt });`
- `b.HasIndex(request => request.PlannedSendAtUtc);`
- `b.HasIndex(item => new { item.Status, item.CreatedAt });`
- `b.HasIndex(item => item.EventParticipantSmsRequestId);`
- `b.HasIndex(item => item.PlannedSendAtUtc);`

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
