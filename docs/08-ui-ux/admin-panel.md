# Admin Panel

## Purpose
Document AdminPanel backend integration and policies.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.AdminPanel/Program.cs`
- `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseAdminAnalyticsApiClient.cs`
- `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseAdminUserProfilesApiClient.cs`
- `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseDashboardApiClient.cs`
- `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseEventDiscountCodesApiClient.cs`
- `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseEventTagsApiClient.cs`
- `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseEventTypesApiClient.cs`
- `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseEventsApiClient.cs`
- `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseFinanceApiClient.cs`
- `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseLocationsApiClient.cs`
- `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseModelMapper.cs`
- `src/Randevoo.AdminPanel/Services/ApiClients/DatabasePlannerProfilesApiClient.cs`
- `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseSupportTicketsApiClient.cs`
- `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseUserProfilesApiClient.cs`
- `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseUsersApiClient.cs`
- `src/Randevoo.AdminPanel/Services/ApiClients/IAdminAnalyticsApiClient.cs`
- `src/Randevoo.AdminPanel/Services/ApiClients/IAdminUserProfilesApiClient.cs`
- `src/Randevoo.AdminPanel/Services/ApiClients/IDashboardApiClient.cs`
- `src/Randevoo.AdminPanel/Services/ApiClients/IEventDiscountCodesApiClient.cs`
- `src/Randevoo.AdminPanel/Services/ApiClients/IEventTagsApiClient.cs`
- `src/Randevoo.AdminPanel/Services/ApiClients/IEventTypesApiClient.cs`

## API clients
| Client | Calls | Source |
| --- | --- | --- |
| DatabaseAdminAnalyticsApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseAdminAnalyticsApiClient.cs` |
| DatabaseAdminUserProfilesApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseAdminUserProfilesApiClient.cs` |
| DatabaseDashboardApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseDashboardApiClient.cs` |
| DatabaseEventDiscountCodesApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseEventDiscountCodesApiClient.cs` |
| DatabaseEventTagsApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseEventTagsApiClient.cs` |
| DatabaseEventTypesApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseEventTypesApiClient.cs` |
| DatabaseEventsApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseEventsApiClient.cs` |
| DatabaseFinanceApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseFinanceApiClient.cs` |
| DatabaseLocationsApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseLocationsApiClient.cs` |
| DatabaseModelMapper | See source | `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseModelMapper.cs` |
| DatabasePlannerProfilesApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/DatabasePlannerProfilesApiClient.cs` |
| DatabaseSupportTicketsApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseSupportTicketsApiClient.cs` |
| DatabaseUserProfilesApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseUserProfilesApiClient.cs` |
| DatabaseUsersApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/DatabaseUsersApiClient.cs` |
| IAdminAnalyticsApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/IAdminAnalyticsApiClient.cs` |
| IAdminUserProfilesApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/IAdminUserProfilesApiClient.cs` |
| IDashboardApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/IDashboardApiClient.cs` |
| IEventDiscountCodesApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/IEventDiscountCodesApiClient.cs` |
| IEventTagsApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/IEventTagsApiClient.cs` |
| IEventTypesApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/IEventTypesApiClient.cs` |
| IEventsApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/IEventsApiClient.cs` |
| IFinanceApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/IFinanceApiClient.cs` |
| ILocationsApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/ILocationsApiClient.cs` |
| IPlannerProfilesApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/IPlannerProfilesApiClient.cs` |
| ISupportTicketsApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/ISupportTicketsApiClient.cs` |
| IUserProfilesApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/IUserProfilesApiClient.cs` |
| IUsersApiClient | See source | `src/Randevoo.AdminPanel/Services/ApiClients/IUsersApiClient.cs` |

## Pages
| Page | Route | Policy | Elements | Source |
| --- | --- | --- | --- | --- |
| Account/Forbidden.cshtml | `/Account/Forbidden` | Anonymous | Forms: 0, tables: 0, fields: none detected | `src/Randevoo.AdminPanel/Pages/Account/Forbidden.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Account/Forbidden.cshtml.cs` |
| Account/Login.cshtml | `/Account/Login` | Anonymous | Forms: 2, tables: 0, fields: QuickLoginKey, Step, Input.Mobile, Input.Role, Input.VerificationCode | `src/Randevoo.AdminPanel/Pages/Account/Login.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Account/Login.cshtml.cs` |
| Account/Logout.cshtml | `/Account/Logout` | Anonymous | Forms: 0, tables: 0, fields: none detected | `src/Randevoo.AdminPanel/Pages/Account/Logout.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Account/Logout.cshtml.cs` |
| Buyers/Index.cshtml | `/Buyers` | AdminPlannerOrSupport | Forms: 1, tables: 1, fields: PageNumber, BuyerUserId, TicketOrderId, Search, EventId, PaymentStatus | `src/Randevoo.AdminPanel/Pages/Buyers/Index.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Buyers/Index.cshtml.cs` |
| Dashboard/Events.cshtml | `/Dashboard/Events` | AdminOnly | Forms: 0, tables: 0, fields: none detected | `src/Randevoo.AdminPanel/Pages/Dashboard/Events.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Dashboard/Events.cshtml.cs` |
| Dashboard/Index.cshtml | `/Dashboard` | AdminOnly | Forms: 0, tables: 1, fields: none detected | `src/Randevoo.AdminPanel/Pages/Dashboard/Index.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Dashboard/Index.cshtml.cs` |
| Dashboard/Money.cshtml | `/Dashboard/Money` | AdminOnly | Forms: 0, tables: 0, fields: none detected | `src/Randevoo.AdminPanel/Pages/Dashboard/Money.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Dashboard/Money.cshtml.cs` |
| Dashboard/My.cshtml | `/Dashboard/My` | AdminOrPlanner | Forms: 0, tables: 1, fields: none detected | `src/Randevoo.AdminPanel/Pages/Dashboard/My.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Dashboard/My.cshtml.cs` |
| Dashboard/Sales.cshtml | `/Dashboard/Sales` | AdminOnly | Forms: 0, tables: 0, fields: none detected | `src/Randevoo.AdminPanel/Pages/Dashboard/Sales.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Dashboard/Sales.cshtml.cs` |
| Dashboard/Users.cshtml | `/Dashboard/Users` | AdminOnly | Forms: 0, tables: 0, fields: none detected | `src/Randevoo.AdminPanel/Pages/Dashboard/Users.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Dashboard/Users.cshtml.cs` |
| DiscountCodes/Index.cshtml | `/DiscountCodes` | AdminOnly | Forms: 3, tables: 2, fields: Id, Input.DatingEventId, Input.Code, Input.Title, Input.GenderScope, Input.DiscountType | `src/Randevoo.AdminPanel/Pages/DiscountCodes/Index.cshtml`<br>`src/Randevoo.AdminPanel/Pages/DiscountCodes/Index.cshtml.cs` |
| Error.cshtml | `/Error` | See folder convention | Forms: 0, tables: 0, fields: none detected | `src/Randevoo.AdminPanel/Pages/Error.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Error.cshtml.cs` |
| EventTypes/Index.cshtml | `/EventTypes` | AdminOnly | Forms: 2, tables: 1, fields: Id, Input.Name, Input.Description, Input.IsActive, id | `src/Randevoo.AdminPanel/Pages/EventTypes/Index.cshtml`<br>`src/Randevoo.AdminPanel/Pages/EventTypes/Index.cshtml.cs` |
| Events/Buyers.cshtml | `/Events/Buyers` | AdminOrPlanner | Forms: 0, tables: 0, fields: none detected | `src/Randevoo.AdminPanel/Pages/Events/Buyers.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Events/Buyers.cshtml.cs` |
| Events/Conversation.cshtml | `/Events/Conversation` | AdminOnly | Forms: 0, tables: 0, fields: none detected | `src/Randevoo.AdminPanel/Pages/Events/Conversation.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Events/Conversation.cshtml.cs` |
| Events/Conversations.cshtml | `/Events/Conversations` | AdminOnly | Forms: 0, tables: 0, fields: none detected | `src/Randevoo.AdminPanel/Pages/Events/Conversations.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Events/Conversations.cshtml.cs` |
| Events/Details.cshtml | `/Events/Details` | AdminOrPlanner | Forms: 5, tables: 0, fields: id, commissionPercent, note | `src/Randevoo.AdminPanel/Pages/Events/Details.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Events/Details.cshtml.cs` |
| Events/Edit.cshtml | `/Events/Edit` | AdminOrPlanner | Forms: 1, tables: 0, fields: ExistingEventId, Input.Image1, Input.Image2, Input.Image3, Input.Latitude, Input.Longitude | `src/Randevoo.AdminPanel/Pages/Events/Edit.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Events/Edit.cshtml.cs` |
| Events/Faqs.cshtml | `/Events/Faqs` | AdminOrPlanner | Forms: 1, tables: 0, fields: EventId, Faqs[@index].Question, Faqs[@index].Answer | `src/Randevoo.AdminPanel/Pages/Events/Faqs.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Events/Faqs.cshtml.cs` |
| Events/Index.cshtml | `/Events` | AdminOnly | Forms: 1, tables: 1, fields: Scope, PageNumber, Search, TagId, City, EventModeId | `src/Randevoo.AdminPanel/Pages/Events/Index.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Events/Index.cshtml.cs` |
| Events/My.cshtml | `/Events/My` | AdminOrPlanner | Forms: 3, tables: 1, fields: id | `src/Randevoo.AdminPanel/Pages/Events/My.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Events/My.cshtml.cs` |
| Events/Sms.cshtml | `/Events/Sms` | AdminOrPlanner | Forms: 4, tables: 0, fields: eventId, SearchText, StatusFilter, ScheduleFilter, RequesterFilter, NewMessage | `src/Randevoo.AdminPanel/Pages/Events/Sms.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Events/Sms.cshtml.cs` |
| Events/SurveyRatings.cshtml | `/Events/SurveyRatings` | AdminOrPlanner | Forms: 0, tables: 0, fields: none detected | `src/Randevoo.AdminPanel/Pages/Events/SurveyRatings.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Events/SurveyRatings.cshtml.cs` |
| Finance/Index.cshtml | `/Finance` | AdminOnly | Forms: 2, tables: 2, fields: ReviewInput.RequestId, ReviewInput.ReviewNote | `src/Randevoo.AdminPanel/Pages/Finance/Index.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Finance/Index.cshtml.cs` |
| Finance/My.cshtml | `/Finance/My` | AdminOrPlanner | Forms: 1, tables: 1, fields: Input.Amount | `src/Randevoo.AdminPanel/Pages/Finance/My.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Finance/My.cshtml.cs` |
| Finance/PaymentReceipts.cshtml | `/Finance/PaymentReceipts` | SupportOrAdmin | Forms: 2, tables: 1, fields: ReviewInput.ReceiptId, ReviewInput.RejectReason | `src/Randevoo.AdminPanel/Pages/Finance/PaymentReceipts.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Finance/PaymentReceipts.cshtml.cs` |
| Finance/ReceivedReceipts.cshtml | `/Finance/ReceivedReceipts` | AdminOrPlanner | Forms: 2, tables: 1, fields: ReviewInput.ReceiptId, ReviewInput.RejectReason | `src/Randevoo.AdminPanel/Pages/Finance/ReceivedReceipts.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Finance/ReceivedReceipts.cshtml.cs` |
| Finance/TicketTransactions.cshtml | `/Finance/TicketTransactions` | AdminOnly | Forms: 1, tables: 1, fields: PageNumber, Search, FromDate, ToDate, Sort | `src/Randevoo.AdminPanel/Pages/Finance/TicketTransactions.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Finance/TicketTransactions.cshtml.cs` |
| Finance/User.cshtml | `/Finance/User` | AdminOnly | Forms: 0, tables: 2, fields: none detected | `src/Randevoo.AdminPanel/Pages/Finance/User.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Finance/User.cshtml.cs` |
| Finance/Withdrawals.cshtml | `/Finance/Withdrawals` | AdminOnly | Forms: 3, tables: 1, fields: PageNumber, Search, Status, ReviewInput.RequestId, ReviewInput.ReviewNote | `src/Randevoo.AdminPanel/Pages/Finance/Withdrawals.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Finance/Withdrawals.cshtml.cs` |
| Index.cshtml | `/` | See folder convention | Forms: 0, tables: 0, fields: none detected | `src/Randevoo.AdminPanel/Pages/Index.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Index.cshtml.cs` |
| Logs/Index.cshtml | `/Logs` | AdminOnly | Forms: 1, tables: 1, fields: PageNumber, Search, RangeKey, Role, Status, Sort | `src/Randevoo.AdminPanel/Pages/Logs/Index.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Logs/Index.cshtml.cs` |
| Logs/SmsQueue.cshtml | `/Logs/SmsQueue` | AdminOnly | Forms: 1, tables: 1, fields: PageNumber, Search, Status, Sort | `src/Randevoo.AdminPanel/Pages/Logs/SmsQueue.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Logs/SmsQueue.cshtml.cs` |
| Participants/Index.cshtml | `/Participants` | AdminPlannerOrSupport | Forms: 2, tables: 1, fields: EventId, BuyerUserId, TicketOrderId, PageNumber, View, Search | `src/Randevoo.AdminPanel/Pages/Participants/Index.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Participants/Index.cshtml.cs` |
| Planner/Approvals.cshtml | `/Planner/Approvals` | AdminOnly | Forms: 0, tables: 1, fields: none detected | `src/Randevoo.AdminPanel/Pages/Planner/Approvals.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Planner/Approvals.cshtml.cs` |
| Planner/BankAccounts.cshtml | `/Planner/BankAccounts` | AdminOrPlanner | Forms: 2, tables: 1, fields: Input.Id, Input.CurrencyCode, Input.AccountHolderName, Input.CardNumber, Input.Iban, Input.BankName | `src/Randevoo.AdminPanel/Pages/Planner/BankAccounts.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Planner/BankAccounts.cshtml.cs` |
| Planner/Details.cshtml | `/Planner/Details` | Anonymous | Forms: 0, tables: 1, fields: none detected | `src/Randevoo.AdminPanel/Pages/Planner/Details.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Planner/Details.cshtml.cs` |
| Planner/Index.cshtml | `/Planner` | AdminOnly | Forms: 0, tables: 0, fields: none detected | `src/Randevoo.AdminPanel/Pages/Planner/Index.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Planner/Index.cshtml.cs` |
| Planner/Profile.cshtml | `/Planner/Profile` | AdminOrPlanner | Forms: 1, tables: 1, fields: Input.FullName, Input.City, Input.Title, ProfileImageFile, Input.PictureUrl, Input.Resume | `src/Randevoo.AdminPanel/Pages/Planner/Profile.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Planner/Profile.cshtml.cs` |
| Planner/Review.cshtml | `/Planner/Review` | AdminOnly | Forms: 1, tables: 0, fields: Input.FullName, Input.City, Input.Title, Input.PictureUrl, Input.Resume, Input.ReviewNote | `src/Randevoo.AdminPanel/Pages/Planner/Review.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Planner/Review.cshtml.cs` |
| Privacy.cshtml | `/Privacy` | See folder convention | Forms: 0, tables: 0, fields: none detected | `src/Randevoo.AdminPanel/Pages/Privacy.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Privacy.cshtml.cs` |
| Public/Event.cshtml | `/Public/Event` | Anonymous | Forms: 0, tables: 0, fields: none detected | `src/Randevoo.AdminPanel/Pages/Public/Event.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Public/Event.cshtml.cs` |
| Settings/Index.cshtml | `/Settings` | AdminOnly | Forms: 1, tables: 2, fields: RateInput.CurrencyCode, RateInput.Rate | `src/Randevoo.AdminPanel/Pages/Settings/Index.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Settings/Index.cshtml.cs` |
| Settings/Locations.cshtml | `/Settings/Locations` | AdminOnly | Forms: filters, country editor, city editor. Tables: country list and city list. Fields: Search, CountryId, CountryInput.Name, CountryInput.Code, CountryInput.DisplayOrder, CountryInput.IsActive, CityInput.CountryId, CityInput.Name, CityInput.Latitude, CityInput.Longitude, CityInput.DisplayOrder, CityInput.IsActive | `src/Randevoo.AdminPanel/Pages/Settings/Locations.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Settings/Locations.cshtml.cs` |
| Settings/OperationPermissions.cshtml | `/Settings/OperationPermissions` | AdminOnly | Forms: catalog sync, filters, role matrix, user override. Tables: operation role matrix and override list. Fields: Search, GroupKey, Entity, RiskLevel, Surface, IncludeInactive, UserSearch, RolePermissions[].Role, RolePermissions[].Entity, RolePermissions[].Action, RolePermissions[].Allowed, OverrideInput.UserId, OverrideInput.ActionKey, OverrideInput.Allowed, OverrideInput.ExpiresAtUtc, OverrideInput.Note | `src/Randevoo.AdminPanel/Pages/Settings/OperationPermissions.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Settings/OperationPermissions.cshtml.cs` |
| Shared/_DashboardRangeFilter.cshtml | `/Shared/_DashboardRangeFilter` | See folder convention | Forms: 1, tables: 0, fields: rangeKey | `src/Randevoo.AdminPanel/Pages/Shared/_DashboardRangeFilter.cshtml` |
| Shared/_EventImageSlider.cshtml | `/Shared/_EventImageSlider` | See folder convention | Forms: 0, tables: 0, fields: none detected | `src/Randevoo.AdminPanel/Pages/Shared/_EventImageSlider.cshtml` |
| Shared/_Layout.cshtml | `/Shared/_Layout` | See folder convention | Forms: 0, tables: 0, fields: none detected | `src/Randevoo.AdminPanel/Pages/Shared/_Layout.cshtml` |
| Shared/_SidebarNav.cshtml | `/Shared/_SidebarNav` | See folder convention | Forms: 0, tables: 0, fields: none detected | `src/Randevoo.AdminPanel/Pages/Shared/_SidebarNav.cshtml` |
| Shared/_Topbar.cshtml | `/Shared/_Topbar` | See folder convention | Forms: 0, tables: 0, fields: none detected | `src/Randevoo.AdminPanel/Pages/Shared/_Topbar.cshtml` |
| Shared/_ValidationScriptsPartial.cshtml | `/Shared/_ValidationScriptsPartial` | See folder convention | Forms: 0, tables: 0, fields: none detected | `src/Randevoo.AdminPanel/Pages/Shared/_ValidationScriptsPartial.cshtml` |
| Support/Create.cshtml | `/Support/Create` | See folder convention | Forms: 1, tables: 0, fields: Input.Title, Input.TicketTypeId, Input.TicketRecipientTypeId, Input.EventId, Input.Body, Attachments | `src/Randevoo.AdminPanel/Pages/Support/Create.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Support/Create.cshtml.cs` |
| Support/Details.cshtml | `/Support/Details` | See folder convention | Forms: 3, tables: 3, fields: ReplyInput.TicketId, ReplyInput.RepresentedUserId, ReplyInput.Body, Attachments, StatusInput.TicketId, StatusInput.TicketStatusId | `src/Randevoo.AdminPanel/Pages/Support/Details.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Support/Details.cshtml.cs` |
| Support/Index.cshtml | `/Support` | SupportOrAdmin | Forms: 1, tables: 0, fields: CreatedFromJalali, CreatedToJalali, TicketStatusId, TicketTypeId, TicketRecipientTypeId, SubmitterRole | `src/Randevoo.AdminPanel/Pages/Support/Index.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Support/Index.cshtml.cs` |
| Support/My.cshtml | `/Support/My` | See folder convention | Forms: 0, tables: 1, fields: none detected | `src/Randevoo.AdminPanel/Pages/Support/My.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Support/My.cshtml.cs` |
| Support/Received.cshtml | `/Support/Received` | AdminOrPlanner | Forms: 0, tables: 1, fields: none detected | `src/Randevoo.AdminPanel/Pages/Support/Received.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Support/Received.cshtml.cs` |
| Support/Tickets.cshtml | `/Support/Tickets` | SupportOrAdmin | Forms: 1, tables: 1, fields: CreatedFromJalali, CreatedToJalali, TicketTypeId, TicketRecipientTypeId, SubmitterRole, AssigneeUserId | `src/Randevoo.AdminPanel/Pages/Support/Tickets.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Support/Tickets.cshtml.cs` |
| Tags/Index.cshtml | `/Tags` | AdminOnly | Forms: 2, tables: 1, fields: Id, Input.Name, Input.IsActive, id | `src/Randevoo.AdminPanel/Pages/Tags/Index.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Tags/Index.cshtml.cs` |
| UserProfiles/AdminEdit.cshtml | `/UserProfiles/AdminEdit` | AdminOnly | Forms: 6, tables: 0, fields: ProfileInput.DisplayName, ProfileInput.MobileNumber, DateOfBirthText, ProfileInput.Gender, ProfileInput.HeightCentimeters, ProfileInput.CountryId | `src/Randevoo.AdminPanel/Pages/UserProfiles/AdminEdit.cshtml`<br>`src/Randevoo.AdminPanel/Pages/UserProfiles/AdminEdit.cshtml.cs` |
| UserProfiles/Details.cshtml | `/UserProfiles/Details` | AdminOrPlanner | Forms: 0, tables: 1, fields: none detected | `src/Randevoo.AdminPanel/Pages/UserProfiles/Details.cshtml`<br>`src/Randevoo.AdminPanel/Pages/UserProfiles/Details.cshtml.cs` |
| UserProfiles/Index.cshtml | `/UserProfiles` | AdminOnly | Forms: 1, tables: 1, fields: PageNumber, Search, CityId, GenderId, ZodiacSignId, IsActive | `src/Randevoo.AdminPanel/Pages/UserProfiles/Index.cshtml`<br>`src/Randevoo.AdminPanel/Pages/UserProfiles/Index.cshtml.cs` |
| Users/Index.cshtml | `/Users` | AdminOnly | Forms: 1, tables: 1, fields: UserId, Input.FullName, Input.Mobile, Input.Role, Input.IsActive | `src/Randevoo.AdminPanel/Pages/Users/Index.cshtml`<br>`src/Randevoo.AdminPanel/Pages/Users/Index.cshtml.cs` |
| _ViewImports.cshtml | `/_ViewImports` | See folder convention | Forms: 0, tables: 0, fields: none detected | `src/Randevoo.AdminPanel/Pages/_ViewImports.cshtml` |
| _ViewStart.cshtml | `/_ViewStart` | See folder convention | Forms: 0, tables: 0, fields: none detected | `src/Randevoo.AdminPanel/Pages/_ViewStart.cshtml` |

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
