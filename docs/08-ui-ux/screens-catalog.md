# Screens Catalog

## Purpose
Detailed catalog of detected Razor Pages screens.

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
- `src/Randevoo.AdminPanel/Pages/Privacy.cshtml`
- `src/Randevoo.AdminPanel/Pages/Public/Event.cshtml`
- `src/Randevoo.AdminPanel/Pages/Settings/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Settings/Locations.cshtml`
- `src/Randevoo.AdminPanel/Pages/Settings/OperationPermissions.cshtml`
- `src/Randevoo.AdminPanel/Pages/Shared/_DashboardRangeFilter.cshtml`
- `src/Randevoo.AdminPanel/Pages/Shared/_EventImageSlider.cshtml`
- `src/Randevoo.AdminPanel/Pages/Shared/_Layout.cshtml`
- `src/Randevoo.AdminPanel/Pages/Shared/_SidebarNav.cshtml`
- `src/Randevoo.AdminPanel/Pages/Shared/_Topbar.cshtml`
- `src/Randevoo.AdminPanel/Pages/Shared/_ValidationScriptsPartial.cshtml`


## Screen/Page: Account/Forbidden.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Account/Forbidden.cshtml`
- `src/Randevoo.AdminPanel/Pages/Account/Forbidden.cshtml.cs`

Route:
`/Account/Forbidden`

Accessible by:
- Anonymous

Purpose:
Account/Forbidden screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 0
- Fields: None detected
- Buttons: None detected

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Account/Login.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Account/Login.cshtml`
- `src/Randevoo.AdminPanel/Pages/Account/Login.cshtml.cs`

Route:
`/Account/Login`

Accessible by:
- Anonymous

Purpose:
Account/Login screen in the AdminPanel.

Main UI elements:
- Forms: 2
- Tables: 0
- Fields: QuickLoginKey, Step, Input.Mobile, Input.Role, Input.VerificationCode
- Buttons: ورود سریع, دریافت کد تایید, ورود به پنل, بازگشت و ویرایش اطلاعات

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Account/Logout.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Account/Logout.cshtml`
- `src/Randevoo.AdminPanel/Pages/Account/Logout.cshtml.cs`

Route:
`/Account/Logout`

Accessible by:
- Anonymous

Purpose:
Account/Logout screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 0
- Fields: None detected
- Buttons: None detected

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Buyers/Index.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Buyers/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Buyers/Index.cshtml.cs`

Route:
`/Buyers`

Accessible by:
- AdminPlannerOrSupport

Purpose:
Buyers/Index screen in the AdminPanel.

Main UI elements:
- Forms: 1
- Tables: 1
- Fields: PageNumber, BuyerUserId, TicketOrderId, Search, EventId, PaymentStatus, Sort, PageSize
- Buttons: فیلترها, اعمال فیلتر

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Dashboard/Events.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Dashboard/Events.cshtml`
- `src/Randevoo.AdminPanel/Pages/Dashboard/Events.cshtml.cs`

Route:
`/Dashboard/Events`

Accessible by:
- AdminOnly

Purpose:
Dashboard/Events screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 0
- Fields: None detected
- Buttons: None detected

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Dashboard/Index.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Dashboard/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Dashboard/Index.cshtml.cs`

Route:
`/Dashboard`

Accessible by:
- AdminOnly

Purpose:
Dashboard/Index screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 1
- Fields: None detected
- Buttons: @DisplayFormatter.Count(point.EventCount)

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Dashboard/Money.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Dashboard/Money.cshtml`
- `src/Randevoo.AdminPanel/Pages/Dashboard/Money.cshtml.cs`

Route:
`/Dashboard/Money`

Accessible by:
- AdminOnly

Purpose:
Dashboard/Money screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 0
- Fields: None detected
- Buttons: None detected

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Dashboard/My.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Dashboard/My.cshtml`
- `src/Randevoo.AdminPanel/Pages/Dashboard/My.cshtml.cs`

Route:
`/Dashboard/My`

Accessible by:
- AdminOrPlanner

Purpose:
Dashboard/My screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 1
- Fields: None detected
- Buttons: None detected

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Dashboard/Sales.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Dashboard/Sales.cshtml`
- `src/Randevoo.AdminPanel/Pages/Dashboard/Sales.cshtml.cs`

Route:
`/Dashboard/Sales`

Accessible by:
- AdminOnly

Purpose:
Dashboard/Sales screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 0
- Fields: None detected
- Buttons: None detected

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Dashboard/Users.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Dashboard/Users.cshtml`
- `src/Randevoo.AdminPanel/Pages/Dashboard/Users.cshtml.cs`

Route:
`/Dashboard/Users`

Accessible by:
- AdminOnly

Purpose:
Dashboard/Users screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 0
- Fields: None detected
- Buttons: None detected

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: DiscountCodes/Index.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/DiscountCodes/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/DiscountCodes/Index.cshtml.cs`

Route:
`/DiscountCodes`

Accessible by:
- AdminOnly

Purpose:
Discount Codes/Index screen in the AdminPanel.

Main UI elements:
- Forms: 3
- Tables: 2
- Fields: Id, Input.DatingEventId, Input.Code, Input.Title, Input.GenderScope, Input.DiscountType, Input.Value, Input.MaxUsageCount, StartsAtText, EndsAtText, Input.Description, Input.IsActive, Search, EventId, id, isActive
- Buttons: @(Model.IsEditing ? "نمایش فرم ویرایش" : "افزودن کد تخفیف"), ذخیره کد تخفیف, اعمال فیلتر, عملیات, @(item.IsActive ? "غیرفعال سازی" : "فعال سازی")

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Error.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Error.cshtml`
- `src/Randevoo.AdminPanel/Pages/Error.cshtml.cs`

Route:
`/Error`

Accessible by:
- See folder convention

Purpose:
Error screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 0
- Fields: None detected
- Buttons: None detected

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: EventTypes/Index.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/EventTypes/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/EventTypes/Index.cshtml.cs`

Route:
`/EventTypes`

Accessible by:
- AdminOnly

Purpose:
Event Types/Index screen in the AdminPanel.

Main UI elements:
- Forms: 2
- Tables: 1
- Fields: Id, Input.Name, Input.Description, Input.IsActive, id
- Buttons: @(Model.IsEditing ? "نمایش فرم ویرایش" : "افزودن نوع رویداد"), ذخیره نوع رویداد, عملیات, حذف غیرفعال است, حذف نوع رویداد

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Events/Buyers.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Events/Buyers.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Buyers.cshtml.cs`

Route:
`/Events/Buyers`

Accessible by:
- AdminOrPlanner

Purpose:
Events/Buyers screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 0
- Fields: None detected
- Buttons: None detected

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Events/Conversation.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Events/Conversation.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Conversation.cshtml.cs`

Route:
`/Events/Conversation`

Accessible by:
- AdminOnly

Purpose:
Events/Conversation screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 0
- Fields: None detected
- Buttons: None detected

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Events/Conversations.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Events/Conversations.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Conversations.cshtml.cs`

Route:
`/Events/Conversations`

Accessible by:
- AdminOnly

Purpose:
Events/Conversations screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 0
- Fields: None detected
- Buttons: None detected

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Events/Details.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Events/Details.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Details.cshtml.cs`

Route:
`/Events/Details`

Accessible by:
- AdminOrPlanner

Purpose:
Events/Details screen in the AdminPanel.

Main UI elements:
- Forms: 5
- Tables: 0
- Fields: id, commissionPercent, note
- Buttons: عملیات صفحه, باز کردن فروش, بستن فروش, لغو رویداد, تایید بررسی, تایید و باز کردن فروش, ثبت بازخورد و بستن فروش

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Events/Edit.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Events/Edit.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Edit.cshtml.cs`

Route:
`/Events/Edit`

Accessible by:
- AdminOrPlanner

Purpose:
Events/Edit screen in the AdminPanel.

Main UI elements:
- Forms: 1
- Tables: 0
- Fields: ExistingEventId, Input.Image1, Input.Image2, Input.Image3, Input.Latitude, Input.Longitude, Input.OrganizerCommissionPercent, Input.Title, AssignedPlannerId, Input.EventTypeId, Input.EventModeId, Input.OnlineEventPlatformId, Input.OnlineJoinUrl, Input.OnlineAccessInstructions, Input.VenueName, Input.Country, Input.City, Input.Region, Input.Address, StartDateText, StartTimeText, EndDateText, EndTimeText, Input.MinimumEducationLevelId, Input.LikeLimit, Input.MaleTicketPrice, Input.MaleTicketCurrencyCode, Input.CapacityMale, Input.AgeRangeForMale, Input.FemaleTicketPrice, Input.FemaleTicketCurrencyCode, Input.CapacityFemale, Input.AgeRangeForFemale, Input.TagIds, Input.DescriptionHtml, Image1File, Image2File, Image3File, Input.PaymentCollectionMethod, Input.OrganizerPaymentInstructions
- Buttons: 1 اطلاعات پایه, 2 زمان‌بندی, 3 مخاطب و بلیت, 4 محتوا, 5 تصاویر, 6 محاسبات مالی, 7 مرور و ثبت, 1 کاور

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Events/Faqs.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Events/Faqs.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Faqs.cshtml.cs`

Route:
`/Events/Faqs`

Accessible by:
- AdminOrPlanner

Purpose:
Events/Faqs screen in the AdminPanel.

Main UI elements:
- Forms: 1
- Tables: 0
- Fields: EventId, Faqs[@index].Question, Faqs[@index].Answer
- Buttons: ذخیره سوالات متداول

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Events/Index.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Events/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Index.cshtml.cs`

Route:
`/Events`

Accessible by:
- AdminOnly

Purpose:
Events/Index screen in the AdminPanel.

Main UI elements:
- Forms: 1
- Tables: 1
- Fields: Scope, PageNumber, Search, TagId, City, EventModeId, OperationalStatus, ReviewStatus, FromDate, ToDate, Sort
- Buttons: فیلترها, اعمال فیلتر, عملیات

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Events/My.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Events/My.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/My.cshtml.cs`

Route:
`/Events/My`

Accessible by:
- AdminOrPlanner

Purpose:
Events/My screen in the AdminPanel.

Main UI elements:
- Forms: 3
- Tables: 1
- Fields: id
- Buttons: عملیات, درخواست فروش, درخواست بستن فروش, درخواست لغو

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Events/Sms.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Events/Sms.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Sms.cshtml.cs`

Route:
`/Events/Sms`

Accessible by:
- AdminOrPlanner

Purpose:
Events/Sms screen in the AdminPanel.

Main UI elements:
- Forms: 4
- Tables: 0
- Fields: eventId, SearchText, StatusFilter, ScheduleFilter, RequesterFilter, NewMessage, NewPlannedSendAtLocal, ReviewRequestId, ApprovedMessage, ReviewPlannedSendAtLocal, ReviewNote, RejectNote
- Buttons: فیلتر, جزئیات, ثبت درخواست پیام, تایید و ورود به صف, رد درخواست

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Events/SurveyRatings.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Events/SurveyRatings.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/SurveyRatings.cshtml.cs`

Route:
`/Events/SurveyRatings`

Accessible by:
- AdminOrPlanner

Purpose:
Events/Survey Ratings screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 0
- Fields: None detected
- Buttons: None detected

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Finance/Index.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Finance/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/Index.cshtml.cs`

Route:
`/Finance`

Accessible by:
- AdminOnly

Purpose:
Finance/Index screen in the AdminPanel.

Main UI elements:
- Forms: 2
- Tables: 2
- Fields: ReviewInput.RequestId, ReviewInput.ReviewNote
- Buttons: عملیات, تایید پرداخت, رد درخواست

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Finance/My.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Finance/My.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/My.cshtml.cs`

Route:
`/Finance/My`

Accessible by:
- AdminOrPlanner

Purpose:
Finance/My screen in the AdminPanel.

Main UI elements:
- Forms: 1
- Tables: 1
- Fields: Input.Amount
- Buttons: ثبت درخواست تسویه

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Finance/PaymentReceipts.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Finance/PaymentReceipts.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/PaymentReceipts.cshtml.cs`

Route:
`/Finance/PaymentReceipts`

Accessible by:
- SupportOrAdmin

Purpose:
Finance/Payment Receipts screen in the AdminPanel.

Main UI elements:
- Forms: 2
- Tables: 1
- Fields: ReviewInput.ReceiptId, ReviewInput.RejectReason
- Buttons: تایید, رد

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Finance/ReceivedReceipts.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Finance/ReceivedReceipts.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/ReceivedReceipts.cshtml.cs`

Route:
`/Finance/ReceivedReceipts`

Accessible by:
- AdminOrPlanner

Purpose:
Finance/Received Receipts screen in the AdminPanel.

Main UI elements:
- Forms: 2
- Tables: 1
- Fields: ReviewInput.ReceiptId, ReviewInput.RejectReason
- Buttons: تایید دریافت, رد

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Finance/TicketTransactions.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Finance/TicketTransactions.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/TicketTransactions.cshtml.cs`

Route:
`/Finance/TicketTransactions`

Accessible by:
- AdminOnly

Purpose:
Finance/Ticket Transactions screen in the AdminPanel.

Main UI elements:
- Forms: 1
- Tables: 1
- Fields: PageNumber, Search, FromDate, ToDate, Sort
- Buttons: فیلترها, اعمال

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Finance/User.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Finance/User.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/User.cshtml.cs`

Route:
`/Finance/User`

Accessible by:
- AdminOnly

Purpose:
Finance/User screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 2
- Fields: None detected
- Buttons: None detected

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Finance/Withdrawals.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Finance/Withdrawals.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/Withdrawals.cshtml.cs`

Route:
`/Finance/Withdrawals`

Accessible by:
- AdminOnly

Purpose:
Finance/Withdrawals screen in the AdminPanel.

Main UI elements:
- Forms: 3
- Tables: 1
- Fields: PageNumber, Search, Status, ReviewInput.RequestId, ReviewInput.ReviewNote
- Buttons: فیلترها, اعمال, عملیات, تایید پرداخت, رد درخواست

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Index.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Index.cshtml.cs`

Route:
`/`

Accessible by:
- See folder convention

Purpose:
Index screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 0
- Fields: None detected
- Buttons: None detected

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Logs/Index.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Logs/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Logs/Index.cshtml.cs`

Route:
`/Logs`

Accessible by:
- AdminOnly

Purpose:
Logs/Index screen in the AdminPanel.

Main UI elements:
- Forms: 1
- Tables: 1
- Fields: PageNumber, Search, RangeKey, Role, Status, Sort, Action, LogType, Module, IpAddress
- Buttons: فیلترها, اعمال فیلتر

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Logs/SmsQueue.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Logs/SmsQueue.cshtml`
- `src/Randevoo.AdminPanel/Pages/Logs/SmsQueue.cshtml.cs`

Route:
`/Logs/SmsQueue`

Accessible by:
- AdminOnly

Purpose:
Logs/Sms Queue screen in the AdminPanel.

Main UI elements:
- Forms: 1
- Tables: 1
- Fields: PageNumber, Search, Status, Sort
- Buttons: فیلترها

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Participants/Index.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Participants/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Participants/Index.cshtml.cs`

Route:
`/Participants`

Accessible by:
- AdminPlannerOrSupport

Purpose:
Participants/Index screen in the AdminPanel.

Main UI elements:
- Forms: 2
- Tables: 1
- Fields: EventId, BuyerUserId, TicketOrderId, PageNumber, View, Search, FilterEventId, ProfileStatus, TicketStatus, Gender, Sort, FromDate, ToDate, PageSize, Input.TicketId, Input.Reason
- Buttons: فیلترها, اعمال فیلتر, عملیات, بازگشت اضطراری, ثبت بازگشت وجه

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Planner/Approvals.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Planner/Approvals.cshtml`
- `src/Randevoo.AdminPanel/Pages/Planner/Approvals.cshtml.cs`

Route:
`/Planner/Approvals`

Accessible by:
- AdminOnly

Purpose:
Planner/Approvals screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 1
- Fields: None detected
- Buttons: عملیات

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Planner/BankAccounts.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Planner/BankAccounts.cshtml`
- `src/Randevoo.AdminPanel/Pages/Planner/BankAccounts.cshtml.cs`

Route:
`/Planner/BankAccounts`

Accessible by:
- AdminOrPlanner

Purpose:
Planner/Bank Accounts screen in the AdminPanel.

Main UI elements:
- Forms: 2
- Tables: 1
- Fields: Input.Id, Input.CurrencyCode, Input.AccountHolderName, Input.CardNumber, Input.Iban, Input.BankName, Input.AccountNumber, Input.PayoutMethod, Input.Country, Input.SwiftCode, Input.AccountIdentifier, Input.PublicPaymentInstructions, Input.IsActive, bankAccountId, isActive
- Buttons: @(Model.Input.Id is null ? "ذخیره حساب" : "ذخیره ویرایش"), عملیات, @(account.IsActive ? "غیرفعال کردن" : "فعال کردن")

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Planner/Details.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Planner/Details.cshtml`
- `src/Randevoo.AdminPanel/Pages/Planner/Details.cshtml.cs`

Route:
`/Planner/Details`

Accessible by:
- Anonymous

Purpose:
Planner/Details screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 1
- Fields: None detected
- Buttons: None detected

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Planner/Index.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Planner/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Planner/Index.cshtml.cs`

Route:
`/Planner`

Accessible by:
- AdminOnly

Purpose:
Planner/Index screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 0
- Fields: None detected
- Buttons: بیشتر

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Planner/Profile.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Planner/Profile.cshtml`
- `src/Randevoo.AdminPanel/Pages/Planner/Profile.cshtml.cs`

Route:
`/Planner/Profile`

Accessible by:
- AdminOrPlanner

Purpose:
Planner/Profile screen in the AdminPanel.

Main UI elements:
- Forms: 1
- Tables: 1
- Fields: Input.FullName, Input.City, Input.Title, ProfileImageFile, Input.PictureUrl, Input.Resume
- Buttons: ثبت برای تایید مدیر

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Planner/Review.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Planner/Review.cshtml`
- `src/Randevoo.AdminPanel/Pages/Planner/Review.cshtml.cs`

Route:
`/Planner/Review`

Accessible by:
- AdminOnly

Purpose:
Planner/Review screen in the AdminPanel.

Main UI elements:
- Forms: 1
- Tables: 0
- Fields: Input.FullName, Input.City, Input.Title, Input.PictureUrl, Input.Resume, Input.ReviewNote
- Buttons: تایید و انتشار, رد درخواست

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Privacy.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Privacy.cshtml`
- `src/Randevoo.AdminPanel/Pages/Privacy.cshtml.cs`

Route:
`/Privacy`

Accessible by:
- See folder convention

Purpose:
Privacy screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 0
- Fields: None detected
- Buttons: None detected

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Public/Event.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Public/Event.cshtml`
- `src/Randevoo.AdminPanel/Pages/Public/Event.cshtml.cs`

Route:
`/Public/Event`

Accessible by:
- Anonymous

Purpose:
Public/Event screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 0
- Fields: None detected
- Buttons: None detected

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Settings/Index.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Settings/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Settings/Index.cshtml.cs`

Route:
`/Settings`

Accessible by:
- AdminOnly

Purpose:
Settings/Index screen in the AdminPanel.

Main UI elements:
- Forms: 1
- Tables: 2
- Fields: RateInput.CurrencyCode, RateInput.Rate
- Buttons: ثبت نرخ جدید

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Settings/OperationPermissions.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Settings/OperationPermissions.cshtml`
- `src/Randevoo.AdminPanel/Pages/Settings/OperationPermissions.cshtml.cs`

Route:
`/Settings/OperationPermissions`

Accessible by:
- AdminOnly

Purpose:
Settings/Operation Permissions screen in the AdminPanel.

Current behavior:
- Admin-only permission center for the operation catalog, grouped entity tree, role matrix, and user-specific overrides.
- Filters include Search, GroupKey, Entity, RiskLevel, Surface, IncludeInactive, and UserSearch.
- Role matrix fields include RolePermissions[].Role, RolePermissions[].Entity, RolePermissions[].Action, and RolePermissions[].Allowed.
- User override fields include OverrideInput.UserId, OverrideInput.ActionKey, OverrideInput.Allowed, OverrideInput.ExpiresAtUtc, and OverrideInput.Note.
- EndUser is intentionally excluded because this screen manages admin-panel roles only.

Main UI elements:
- Forms: 4
- Tables: 2
- Fields: Entity, UserSearch, RolePermissions[inputIndex].Role, RolePermissions[inputIndex].Action, RolePermissions[inputIndex].Allowed, OverrideInput.UserId, OverrideInput.Action, OverrideInput.Allowed, OverrideInput.ExpiresAtUtc, OverrideInput.Note
- Buttons: نمایش, ذخیره دسترسی نقش‌ها, ثبت override, حذف

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Shared/_DashboardRangeFilter.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Shared/_DashboardRangeFilter.cshtml`

Route:
`/Shared/_DashboardRangeFilter`

Accessible by:
- See folder convention

Purpose:
Shared/ Dashboard Range Filter screen in the AdminPanel.

Main UI elements:
- Forms: 1
- Tables: 0
- Fields: rangeKey
- Buttons: None detected

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Shared/_EventImageSlider.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Shared/_EventImageSlider.cshtml`

Route:
`/Shared/_EventImageSlider`

Accessible by:
- See folder convention

Purpose:
Shared/ Event Image Slider screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 0
- Fields: None detected
- Buttons: قبلی, بعدی

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Shared/_Layout.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Shared/_Layout.cshtml`

Route:
`/Shared/_Layout`

Accessible by:
- See folder convention

Purpose:
Shared/ Layout screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 0
- Fields: None detected
- Buttons: منو

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Shared/_SidebarNav.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Shared/_SidebarNav.cshtml`

Route:
`/Shared/_SidebarNav`

Accessible by:
- See folder convention

Purpose:
Shared/ Sidebar Nav screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 0
- Fields: None detected
- Buttons: پشتیبانی, عملیات من, برگزارکننده, رویدادها, شرکت‌کنندگان, مالی, تحلیل, اطلاعات پایه

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Shared/_Topbar.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Shared/_Topbar.cshtml`

Route:
`/Shared/_Topbar`

Accessible by:
- See folder convention

Purpose:
Shared/ Topbar screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 0
- Fields: None detected
- Buttons: @(user?.FullName ?? "میهمان") @role

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Shared/_ValidationScriptsPartial.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Shared/_ValidationScriptsPartial.cshtml`

Route:
`/Shared/_ValidationScriptsPartial`

Accessible by:
- See folder convention

Purpose:
Shared/ Validation Scripts Partial screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 0
- Fields: None detected
- Buttons: None detected

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Support/Create.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Support/Create.cshtml`
- `src/Randevoo.AdminPanel/Pages/Support/Create.cshtml.cs`

Route:
`/Support/Create`

Accessible by:
- See folder convention

Purpose:
Support/Create screen in the AdminPanel.

Main UI elements:
- Forms: 1
- Tables: 0
- Fields: Input.Title, Input.TicketTypeId, Input.TicketRecipientTypeId, Input.EventId, Input.Body, Attachments
- Buttons: ارسال تیکت

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Support/Details.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Support/Details.cshtml`
- `src/Randevoo.AdminPanel/Pages/Support/Details.cshtml.cs`

Route:
`/Support/Details`

Accessible by:
- See folder convention

Purpose:
Support/Details screen in the AdminPanel.

Main UI elements:
- Forms: 3
- Tables: 3
- Fields: ReplyInput.TicketId, ReplyInput.RepresentedUserId, ReplyInput.Body, Attachments, StatusInput.TicketId, StatusInput.TicketStatusId, StatusInput.Note, ReassignInput.TicketId, ReassignInput.AssigneeUserId, ReassignInput.Note
- Buttons: گفتگو, پروفایل ثبت کننده, مالی, رویدادها, تیکت های قبلی, تاریخچه, ثبت پاسخ, مشاهده مالی

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Support/Index.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Support/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Support/Index.cshtml.cs`

Route:
`/Support`

Accessible by:
- SupportOrAdmin

Purpose:
Support/Index screen in the AdminPanel.

Main UI elements:
- Forms: 1
- Tables: 0
- Fields: CreatedFromJalali, CreatedToJalali, TicketStatusId, TicketTypeId, TicketRecipientTypeId, SubmitterRole, AssigneeUserId
- Buttons: اعمال فیلتر

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Support/My.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Support/My.cshtml`
- `src/Randevoo.AdminPanel/Pages/Support/My.cshtml.cs`

Route:
`/Support/My`

Accessible by:
- See folder convention

Purpose:
Support/My screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 1
- Fields: None detected
- Buttons: None detected

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Support/Received.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Support/Received.cshtml`
- `src/Randevoo.AdminPanel/Pages/Support/Received.cshtml.cs`

Route:
`/Support/Received`

Accessible by:
- AdminOrPlanner

Purpose:
Support/Received screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 1
- Fields: None detected
- Buttons: None detected

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Support/Tickets.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Support/Tickets.cshtml`
- `src/Randevoo.AdminPanel/Pages/Support/Tickets.cshtml.cs`

Route:
`/Support/Tickets`

Accessible by:
- SupportOrAdmin

Purpose:
Support/Tickets screen in the AdminPanel.

Main UI elements:
- Forms: 1
- Tables: 1
- Fields: CreatedFromJalali, CreatedToJalali, TicketTypeId, TicketRecipientTypeId, SubmitterRole, AssigneeUserId
- Buttons: اعمال فیلتر, @status.TitleFa @count

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Tags/Index.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Tags/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Tags/Index.cshtml.cs`

Route:
`/Tags`

Accessible by:
- AdminOnly

Purpose:
Tags/Index screen in the AdminPanel.

Main UI elements:
- Forms: 2
- Tables: 1
- Fields: Id, Input.Name, Input.IsActive, id
- Buttons: @(Model.IsEditing ? "نمایش فرم ویرایش" : "افزودن تگ"), ذخیره تگ, عملیات, حذف غیرفعال است, حذف تگ

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: UserProfiles/AdminEdit.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/UserProfiles/AdminEdit.cshtml`
- `src/Randevoo.AdminPanel/Pages/UserProfiles/AdminEdit.cshtml.cs`

Route:
`/UserProfiles/AdminEdit`

Accessible by:
- AdminOnly

Purpose:
User Profiles/Admin Edit screen in the AdminPanel.

Main UI elements:
- Forms: 6
- Tables: 0
- Fields: ProfileInput.DisplayName, ProfileInput.MobileNumber, DateOfBirthText, ProfileInput.Gender, ProfileInput.HeightCentimeters, ProfileInput.CountryId, ProfileInput.CityId, ProfileInput.EducationLevelId, ProfileInput.ZodiacSignId, ProfileInput.Smoking, ProfileInput.IsActive, imageUrl, imageInput.ImageUrl, interestName, interestInput.InterestName, smsInput.Message
- Buttons: ذخیره تغییرات, حذف, افزودن تصویر, @interest, افزودن علاقه, ثبت پیامک فوری

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: UserProfiles/Details.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/UserProfiles/Details.cshtml`
- `src/Randevoo.AdminPanel/Pages/UserProfiles/Details.cshtml.cs`

Route:
`/UserProfiles/Details`

Accessible by:
- AdminOrPlanner

Purpose:
User Profiles/Details screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 1
- Fields: None detected
- Buttons: None detected

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: UserProfiles/Index.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/UserProfiles/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/UserProfiles/Index.cshtml.cs`

Route:
`/UserProfiles`

Accessible by:
- AdminOnly

Purpose:
User Profiles/Index screen in the AdminPanel.

Main UI elements:
- Forms: 1
- Tables: 1
- Fields: PageNumber, Search, CityId, GenderId, ZodiacSignId, IsActive, IsProfileComplete, Sort
- Buttons: فیلترها, اعمال فیلتر, عملیات

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: Users/Index.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/Users/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Users/Index.cshtml.cs`

Route:
`/Users`

Accessible by:
- AdminOnly

Purpose:
Users/Index screen in the AdminPanel.

Main UI elements:
- Forms: 1
- Tables: 1
- Fields: UserId, Input.FullName, Input.Mobile, Input.Role, Input.IsActive
- Buttons: ذخیره حساب, عملیات

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: _ViewImports.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/_ViewImports.cshtml`

Route:
`/_ViewImports`

Accessible by:
- See folder convention

Purpose:
 View Imports screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 0
- Fields: None detected
- Buttons: None detected

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Screen/Page: _ViewStart.cshtml

Source files:
- `src/Randevoo.AdminPanel/Pages/_ViewStart.cshtml`

Route:
`/_ViewStart`

Accessible by:
- See folder convention

Purpose:
 View Start screen in the AdminPanel.

Main UI elements:
- Forms: 0
- Tables: 0
- Fields: None detected
- Buttons: None detected

User actions:
- View and submit data according to the page model handlers.
- Follow page-specific buttons/links.

API calls:
- Needs Verification in the paired PageModel and API client dependencies.

Related entities:
- Infer from page folder/name and PageModel service dependencies.

UX notes:
- Current strengths: server-rendered forms/tables are straightforward for operational workflows.
- Current issues: loading, error, empty, and permission-denied states need page-by-page verification.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
