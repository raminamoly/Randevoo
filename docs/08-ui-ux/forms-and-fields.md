# Forms And Fields

## Purpose
Inventory form fields detected from Razor markup.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.AdminPanel/Pages/Account/Forbidden.cshtml`
- `src/Randevoo.AdminPanel/Pages/Account/Login.cshtml`
- `src/Randevoo.AdminPanel/Pages/Account/Logout.cshtml`
- `src/Randevoo.AdminPanel/Pages/Buyers/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Dashboard/Events.cshtml`
- `src/Randevoo.AdminPanel/Pages/Dashboard/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Dashboard/Money.cshtml`
- `src/Randevoo.AdminPanel/Pages/Dashboard/My.cshtml`
- `src/Randevoo.AdminPanel/Pages/Dashboard/Sales.cshtml`
- `src/Randevoo.AdminPanel/Pages/Dashboard/Users.cshtml`
- `src/Randevoo.AdminPanel/Pages/DiscountCodes/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Error.cshtml`
- `src/Randevoo.AdminPanel/Pages/EventTypes/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Buyers.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Conversation.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Conversations.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Details.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Edit.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Faqs.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/My.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Sms.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/SurveyRatings.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/My.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/PaymentReceipts.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/ReceivedReceipts.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/TicketTransactions.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/User.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/Withdrawals.cshtml`
- `src/Randevoo.AdminPanel/Pages/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Logs/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Logs/SmsQueue.cshtml`
- `src/Randevoo.AdminPanel/Pages/Participants/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Planner/Approvals.cshtml`
- `src/Randevoo.AdminPanel/Pages/Planner/BankAccounts.cshtml`
- `src/Randevoo.AdminPanel/Pages/Planner/Details.cshtml`
- `src/Randevoo.AdminPanel/Pages/Planner/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Planner/Profile.cshtml`
- `src/Randevoo.AdminPanel/Pages/Planner/Review.cshtml`

| Page | Route | Fields | Source |
| --- | --- | --- | --- |
| Account/Forbidden.cshtml | `/Account/Forbidden` | None detected | `src/Randevoo.AdminPanel/Pages/Account/Forbidden.cshtml` |
| Account/Login.cshtml | `/Account/Login` | QuickLoginKey, Step, Input.Mobile, Input.Role, Input.VerificationCode | `src/Randevoo.AdminPanel/Pages/Account/Login.cshtml` |
| Account/Logout.cshtml | `/Account/Logout` | None detected | `src/Randevoo.AdminPanel/Pages/Account/Logout.cshtml` |
| Buyers/Index.cshtml | `/Buyers` | PageNumber, BuyerUserId, TicketOrderId, Search, EventId, PaymentStatus, Sort, PageSize | `src/Randevoo.AdminPanel/Pages/Buyers/Index.cshtml` |
| Dashboard/Events.cshtml | `/Dashboard/Events` | None detected | `src/Randevoo.AdminPanel/Pages/Dashboard/Events.cshtml` |
| Dashboard/Index.cshtml | `/Dashboard` | None detected | `src/Randevoo.AdminPanel/Pages/Dashboard/Index.cshtml` |
| Dashboard/Money.cshtml | `/Dashboard/Money` | None detected | `src/Randevoo.AdminPanel/Pages/Dashboard/Money.cshtml` |
| Dashboard/My.cshtml | `/Dashboard/My` | None detected | `src/Randevoo.AdminPanel/Pages/Dashboard/My.cshtml` |
| Dashboard/Sales.cshtml | `/Dashboard/Sales` | None detected | `src/Randevoo.AdminPanel/Pages/Dashboard/Sales.cshtml` |
| Dashboard/Users.cshtml | `/Dashboard/Users` | None detected | `src/Randevoo.AdminPanel/Pages/Dashboard/Users.cshtml` |
| DiscountCodes/Index.cshtml | `/DiscountCodes` | Id, Input.DatingEventId, Input.Code, Input.Title, Input.GenderScope, Input.DiscountType, Input.Value, Input.MaxUsageCount, StartsAtText, EndsAtText, Input.Description, Input.IsActive, Search, EventId, id, isActive | `src/Randevoo.AdminPanel/Pages/DiscountCodes/Index.cshtml` |
| Error.cshtml | `/Error` | None detected | `src/Randevoo.AdminPanel/Pages/Error.cshtml` |
| EventTypes/Index.cshtml | `/EventTypes` | Id, Input.Name, Input.Description, Input.IsActive, id | `src/Randevoo.AdminPanel/Pages/EventTypes/Index.cshtml` |
| Events/Buyers.cshtml | `/Events/Buyers` | None detected | `src/Randevoo.AdminPanel/Pages/Events/Buyers.cshtml` |
| Events/Conversation.cshtml | `/Events/Conversation` | None detected | `src/Randevoo.AdminPanel/Pages/Events/Conversation.cshtml` |
| Events/Conversations.cshtml | `/Events/Conversations` | None detected | `src/Randevoo.AdminPanel/Pages/Events/Conversations.cshtml` |
| Events/Details.cshtml | `/Events/Details` | id, commissionPercent, note | `src/Randevoo.AdminPanel/Pages/Events/Details.cshtml` |
| Events/Edit.cshtml | `/Events/Edit` | ExistingEventId, Input.Image1, Input.Image2, Input.Image3, Input.Latitude, Input.Longitude, Input.OrganizerCommissionPercent, Input.Title, AssignedPlannerId, Input.EventTypeId, Input.EventModeId, Input.OnlineEventPlatformId, Input.OnlineJoinUrl, Input.OnlineAccessInstructions, Input.VenueName, Input.Country, Input.City, Input.Region, Input.Address, StartDateText, StartTimeText, EndDateText, EndTimeText, Input.MinimumEducationLevelId, Input.LikeLimit, Input.MaleTicketPrice, Input.MaleTicketCurrencyCode, Input.CapacityMale, Input.AgeRangeForMale, Input.FemaleTicketPrice, Input.FemaleTicketCurrencyCode, Input.CapacityFemale, Input.AgeRangeForFemale, Input.TagIds, Input.DescriptionHtml, Image1File, Image2File, Image3File, Input.PaymentCollectionMethod, Input.OrganizerPaymentInstructions | `src/Randevoo.AdminPanel/Pages/Events/Edit.cshtml` |
| Events/Faqs.cshtml | `/Events/Faqs` | EventId, Faqs[@index].Question, Faqs[@index].Answer | `src/Randevoo.AdminPanel/Pages/Events/Faqs.cshtml` |
| Events/Index.cshtml | `/Events` | Scope, PageNumber, Search, TagId, City, EventModeId, OperationalStatus, ReviewStatus, FromDate, ToDate, Sort | `src/Randevoo.AdminPanel/Pages/Events/Index.cshtml` |
| Events/My.cshtml | `/Events/My` | id | `src/Randevoo.AdminPanel/Pages/Events/My.cshtml` |
| Events/Sms.cshtml | `/Events/Sms` | eventId, SearchText, StatusFilter, ScheduleFilter, RequesterFilter, NewMessage, NewPlannedSendAtLocal, ReviewRequestId, ApprovedMessage, ReviewPlannedSendAtLocal, ReviewNote, RejectNote | `src/Randevoo.AdminPanel/Pages/Events/Sms.cshtml` |
| Events/SurveyRatings.cshtml | `/Events/SurveyRatings` | None detected | `src/Randevoo.AdminPanel/Pages/Events/SurveyRatings.cshtml` |
| Finance/Index.cshtml | `/Finance` | ReviewInput.RequestId, ReviewInput.ReviewNote | `src/Randevoo.AdminPanel/Pages/Finance/Index.cshtml` |
| Finance/My.cshtml | `/Finance/My` | Input.Amount | `src/Randevoo.AdminPanel/Pages/Finance/My.cshtml` |
| Finance/PaymentReceipts.cshtml | `/Finance/PaymentReceipts` | ReviewInput.ReceiptId, ReviewInput.RejectReason | `src/Randevoo.AdminPanel/Pages/Finance/PaymentReceipts.cshtml` |
| Finance/ReceivedReceipts.cshtml | `/Finance/ReceivedReceipts` | ReviewInput.ReceiptId, ReviewInput.RejectReason | `src/Randevoo.AdminPanel/Pages/Finance/ReceivedReceipts.cshtml` |
| Finance/TicketTransactions.cshtml | `/Finance/TicketTransactions` | PageNumber, Search, FromDate, ToDate, Sort | `src/Randevoo.AdminPanel/Pages/Finance/TicketTransactions.cshtml` |
| Finance/User.cshtml | `/Finance/User` | None detected | `src/Randevoo.AdminPanel/Pages/Finance/User.cshtml` |
| Finance/Withdrawals.cshtml | `/Finance/Withdrawals` | PageNumber, Search, Status, ReviewInput.RequestId, ReviewInput.ReviewNote | `src/Randevoo.AdminPanel/Pages/Finance/Withdrawals.cshtml` |
| Index.cshtml | `/` | None detected | `src/Randevoo.AdminPanel/Pages/Index.cshtml` |
| Logs/Index.cshtml | `/Logs` | PageNumber, Search, RangeKey, Role, Status, Sort, Action, LogType, Module, IpAddress | `src/Randevoo.AdminPanel/Pages/Logs/Index.cshtml` |
| Logs/SmsQueue.cshtml | `/Logs/SmsQueue` | PageNumber, Search, Status, Sort | `src/Randevoo.AdminPanel/Pages/Logs/SmsQueue.cshtml` |
| Participants/Index.cshtml | `/Participants` | EventId, BuyerUserId, TicketOrderId, PageNumber, View, Search, FilterEventId, ProfileStatus, TicketStatus, Gender, Sort, FromDate, ToDate, PageSize, Input.TicketId, Input.Reason | `src/Randevoo.AdminPanel/Pages/Participants/Index.cshtml` |
| Planner/Approvals.cshtml | `/Planner/Approvals` | None detected | `src/Randevoo.AdminPanel/Pages/Planner/Approvals.cshtml` |
| Planner/BankAccounts.cshtml | `/Planner/BankAccounts` | Input.Id, Input.CurrencyCode, Input.AccountHolderName, Input.CardNumber, Input.Iban, Input.BankName, Input.AccountNumber, Input.PayoutMethod, Input.Country, Input.SwiftCode, Input.AccountIdentifier, Input.PublicPaymentInstructions, Input.IsActive, bankAccountId, isActive | `src/Randevoo.AdminPanel/Pages/Planner/BankAccounts.cshtml` |
| Planner/Details.cshtml | `/Planner/Details` | None detected | `src/Randevoo.AdminPanel/Pages/Planner/Details.cshtml` |
| Planner/Index.cshtml | `/Planner` | None detected | `src/Randevoo.AdminPanel/Pages/Planner/Index.cshtml` |
| Planner/Profile.cshtml | `/Planner/Profile` | Input.FullName, Input.City, Input.Title, ProfileImageFile, Input.PictureUrl, Input.Resume | `src/Randevoo.AdminPanel/Pages/Planner/Profile.cshtml` |
| Planner/Review.cshtml | `/Planner/Review` | Input.FullName, Input.City, Input.Title, Input.PictureUrl, Input.Resume, Input.ReviewNote | `src/Randevoo.AdminPanel/Pages/Planner/Review.cshtml` |
| Privacy.cshtml | `/Privacy` | None detected | `src/Randevoo.AdminPanel/Pages/Privacy.cshtml` |
| Public/Event.cshtml | `/Public/Event` | None detected | `src/Randevoo.AdminPanel/Pages/Public/Event.cshtml` |
| Settings/Index.cshtml | `/Settings` | RateInput.CurrencyCode, RateInput.Rate | `src/Randevoo.AdminPanel/Pages/Settings/Index.cshtml` |
| Settings/OperationPermissions.cshtml | `/Settings/OperationPermissions` | Entity, UserSearch, RolePermissions[inputIndex].Role, RolePermissions[inputIndex].Action, RolePermissions[inputIndex].Allowed, OverrideInput.UserId, OverrideInput.Action, OverrideInput.Allowed, OverrideInput.ExpiresAtUtc, OverrideInput.Note | `src/Randevoo.AdminPanel/Pages/Settings/OperationPermissions.cshtml` |
| Shared/_DashboardRangeFilter.cshtml | `/Shared/_DashboardRangeFilter` | rangeKey | `src/Randevoo.AdminPanel/Pages/Shared/_DashboardRangeFilter.cshtml` |
| Shared/_EventImageSlider.cshtml | `/Shared/_EventImageSlider` | None detected | `src/Randevoo.AdminPanel/Pages/Shared/_EventImageSlider.cshtml` |
| Shared/_Layout.cshtml | `/Shared/_Layout` | None detected | `src/Randevoo.AdminPanel/Pages/Shared/_Layout.cshtml` |
| Shared/_SidebarNav.cshtml | `/Shared/_SidebarNav` | None detected | `src/Randevoo.AdminPanel/Pages/Shared/_SidebarNav.cshtml` |
| Shared/_Topbar.cshtml | `/Shared/_Topbar` | None detected | `src/Randevoo.AdminPanel/Pages/Shared/_Topbar.cshtml` |
| Shared/_ValidationScriptsPartial.cshtml | `/Shared/_ValidationScriptsPartial` | None detected | `src/Randevoo.AdminPanel/Pages/Shared/_ValidationScriptsPartial.cshtml` |
| Support/Create.cshtml | `/Support/Create` | Input.Title, Input.TicketTypeId, Input.TicketRecipientTypeId, Input.EventId, Input.Body, Attachments | `src/Randevoo.AdminPanel/Pages/Support/Create.cshtml` |
| Support/Details.cshtml | `/Support/Details` | ReplyInput.TicketId, ReplyInput.RepresentedUserId, ReplyInput.Body, Attachments, StatusInput.TicketId, StatusInput.TicketStatusId, StatusInput.Note, ReassignInput.TicketId, ReassignInput.AssigneeUserId, ReassignInput.Note | `src/Randevoo.AdminPanel/Pages/Support/Details.cshtml` |
| Support/Index.cshtml | `/Support` | CreatedFromJalali, CreatedToJalali, TicketStatusId, TicketTypeId, TicketRecipientTypeId, SubmitterRole, AssigneeUserId | `src/Randevoo.AdminPanel/Pages/Support/Index.cshtml` |
| Support/My.cshtml | `/Support/My` | None detected | `src/Randevoo.AdminPanel/Pages/Support/My.cshtml` |
| Support/Received.cshtml | `/Support/Received` | None detected | `src/Randevoo.AdminPanel/Pages/Support/Received.cshtml` |
| Support/Tickets.cshtml | `/Support/Tickets` | CreatedFromJalali, CreatedToJalali, TicketTypeId, TicketRecipientTypeId, SubmitterRole, AssigneeUserId | `src/Randevoo.AdminPanel/Pages/Support/Tickets.cshtml` |
| Tags/Index.cshtml | `/Tags` | Id, Input.Name, Input.IsActive, id | `src/Randevoo.AdminPanel/Pages/Tags/Index.cshtml` |
| UserProfiles/AdminEdit.cshtml | `/UserProfiles/AdminEdit` | ProfileInput.DisplayName, ProfileInput.MobileNumber, DateOfBirthText, ProfileInput.Gender, ProfileInput.HeightCentimeters, ProfileInput.CountryId, ProfileInput.CityId, ProfileInput.EducationLevelId, ProfileInput.ZodiacSignId, ProfileInput.Smoking, ProfileInput.IsActive, imageUrl, imageInput.ImageUrl, interestName, interestInput.InterestName, smsInput.Message | `src/Randevoo.AdminPanel/Pages/UserProfiles/AdminEdit.cshtml` |
| UserProfiles/Details.cshtml | `/UserProfiles/Details` | None detected | `src/Randevoo.AdminPanel/Pages/UserProfiles/Details.cshtml` |
| UserProfiles/Index.cshtml | `/UserProfiles` | PageNumber, Search, CityId, GenderId, ZodiacSignId, IsActive, IsProfileComplete, Sort | `src/Randevoo.AdminPanel/Pages/UserProfiles/Index.cshtml` |
| Users/Index.cshtml | `/Users` | UserId, Input.FullName, Input.Mobile, Input.Role, Input.IsActive | `src/Randevoo.AdminPanel/Pages/Users/Index.cshtml` |
| _ViewImports.cshtml | `/_ViewImports` | None detected | `src/Randevoo.AdminPanel/Pages/_ViewImports.cshtml` |
| _ViewStart.cshtml | `/_ViewStart` | None detected | `src/Randevoo.AdminPanel/Pages/_ViewStart.cshtml` |

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
