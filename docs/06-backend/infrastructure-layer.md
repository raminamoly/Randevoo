# Infrastructure Layer

## Purpose
Document infrastructure services, repositories, persistence, and external adapters.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.Infrastructure`

## Repositories
| Repository | Methods | Source |
| --- | --- | --- |
| IBalanceAccountRepository | See source | `src/Randevoo.Domain/Interfaces/Repositories/IBalanceAccountRepository.cs` |
| IDatingEventRepository | See source | `src/Randevoo.Domain/Interfaces/Repositories/IDatingEventRepository.cs` |
| IEventConversationRepository | See source | `src/Randevoo.Domain/Interfaces/Repositories/IEventConversationRepository.cs` |
| IEventDiscountCodeRepository | See source | `src/Randevoo.Domain/Interfaces/Repositories/IEventDiscountCodeRepository.cs` |
| IEventLikeRepository | See source | `src/Randevoo.Domain/Interfaces/Repositories/IEventLikeRepository.cs` |
| IEventParticipantSmsRequestRepository | See source | `src/Randevoo.Domain/Interfaces/Repositories/IEventParticipantSmsRequestRepository.cs` |
| IEventPlannerProfileRepository | See source | `src/Randevoo.Domain/Interfaces/Repositories/IEventPlannerProfileRepository.cs` |
| IEventSurveyRepository | See source | `src/Randevoo.Domain/Interfaces/Repositories/IEventSurveyRepository.cs` |
| IEventTicketRepository | See source | `src/Randevoo.Domain/Interfaces/Repositories/IEventTicketRepository.cs` |
| IEventTypeRepository | See source | `src/Randevoo.Domain/Interfaces/Repositories/IEventTypeRepository.cs` |
| IModerationReportRepository | See source | `src/Randevoo.Domain/Interfaces/Repositories/IModerationReportRepository.cs` |
| IRefreshTokenRepository | See source | `src/Randevoo.Domain/Interfaces/Repositories/IRefreshTokenRepository.cs` |
| ISmsQueueRepository | See source | `src/Randevoo.Domain/Interfaces/Repositories/ISmsQueueRepository.cs` |
| ISupportTicketRepository | See source | `src/Randevoo.Domain/Interfaces/Repositories/ISupportTicketRepository.cs` |
| IUserProfileRepository | See source | `src/Randevoo.Domain/Interfaces/Repositories/IUserProfileRepository.cs` |
| IUserRepository | See source | `src/Randevoo.Domain/Interfaces/Repositories/IUserRepository.cs` |
| BalanceAccountRepository | GetByUserIdAsync, AddAsync, UpdateAsync | `src/Randevoo.Infrastructure/Repositories/BalanceAccountRepository.cs` |
| DatingEventRepository | GetByIdAsync, GetByIdWithTicketsAsync, ListOpenAsync, CountByPlannerAsync, CountCancelledByPlannerAsync, CountCompletedByPlannerAsync, AddAsync, UpdateAsync | `src/Randevoo.Infrastructure/Repositories/DatingEventRepository.cs` |
| EventConversationRepository | GetByIdWithDetailsAsync, GetBetweenParticipantsAsync, CountActiveConnectionsForUserAsync, ListForUserAsync, ListForEventUserAsync, AddAsync, UpdateAsync | `src/Randevoo.Infrastructure/Repositories/EventConversationRepository.cs` |
| EventDiscountCodeRepository | GetApplicableByCodeAsync, UpdateAsync | `src/Randevoo.Infrastructure/Repositories/EventDiscountCodeRepository.cs` |
| EventLikeRepository | GetDirectedAsync, GetReverseAsync, AddAsync, UpdateAsync | `src/Randevoo.Infrastructure/Repositories/EventLikeRepository.cs` |
| EventParticipantSmsRequestRepository | GetByIdAsync, AddAsync, UpdateAsync | `src/Randevoo.Infrastructure/Repositories/EventParticipantSmsRequestRepository.cs` |
| EventPlannerProfileRepository | GetByUserIdAsync, AddAsync, UpdateAsync | `src/Randevoo.Infrastructure/Repositories/EventPlannerProfileRepository.cs` |
| EventSurveyRepository | GetByEventAndUserAsync, AddAsync, UpdateAsync | `src/Randevoo.Infrastructure/Repositories/EventSurveyRepository.cs` |
| EventTicketRepository | GetByIdAsync, GetByEventAndUserAsync, ListByUserIdAsync, ListByEventIdAsync, UpdateAsync | `src/Randevoo.Infrastructure/Repositories/EventTicketRepository.cs` |
| EventTypeRepository | GetByIdAsync, ListActiveAsync, AddAsync, UpdateAsync | `src/Randevoo.Infrastructure/Repositories/EventTypeRepository.cs` |
| ModerationReportRepository | GetByIdAsync, ListByReporterAsync, ListByStatusAsync, AddAsync, UpdateAsync | `src/Randevoo.Infrastructure/Repositories/ModerationReportRepository.cs` |
| RefreshTokenRepository | GetByTokenHashAsync, ListByUserIdAsync, AddAsync, UpdateAsync | `src/Randevoo.Infrastructure/Repositories/RefreshTokenRepository.cs` |
| SmsQueueRepository | AddRangeAsync | `src/Randevoo.Infrastructure/Repositories/SmsQueueRepository.cs` |
| SupportTicketRepository | GetByIdWithDetailsAsync, ListAsync, GetNextRoundRobinAssigneeAsync, IsTicketTypeActiveAsync, IsTicketStatusActiveAsync, IsTicketRecipientTypeActiveAsync, AddAsync, UpdateAsync | `src/Randevoo.Infrastructure/Repositories/SupportTicketRepository.cs` |
| UserProfileRepository | AddAsync, UpdateAsync, DeleteAsync, GetByIdAsync, GetByUserIdAsync, GetByIdWithDetailsAsync, GetByDisplayNameAsync, GetByEmailAsync | `src/Randevoo.Infrastructure/Repositories/UserProfileRepository.cs` |
| UserRepository | GetByIdAsync, GetByMobileNumberAsync, ExistsByEmailAsync, ListActiveSupportUsersAsync, AddAsync, UpdateAsync | `src/Randevoo.Infrastructure/Repositories/UserRepository.cs` |

## Services
- `src/Randevoo.Infrastructure/Services/AuditLogger.cs`
- `src/Randevoo.Infrastructure/Services/AuthTokenPolicy.cs`
- `src/Randevoo.Infrastructure/Services/ConsoleEmailSender.cs`
- `src/Randevoo.Infrastructure/Services/ConsoleSmsSender.cs`
- `src/Randevoo.Infrastructure/Services/CurrencyExchangeRateProvider.cs`
- `src/Randevoo.Infrastructure/Services/JwtTokenService.cs`
- `src/Randevoo.Infrastructure/Services/PrivacyDataReader.cs`
- `src/Randevoo.Infrastructure/Services/SecureCodeGenerator.cs`
- `src/Randevoo.Infrastructure/Services/Sha256CodeHasher.cs`

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
