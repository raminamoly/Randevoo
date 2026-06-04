# Randevoo Handoff

## Current Checkpoint

- Branch: `master`
- Repository: `https://github.com/raminamoly/Randevoo`
- Workspace: `C:\_D\Randevoo`
- Local admin panel URL: `http://localhost:5075`
- Database used for local verification: `DESKTOP-5QNHMHJ\SQL2019`, database `Randevoo`

## Completed Feature Batch

This workspace contains the admin-panel/database feature batch for:

- Replacing mock admin-panel data with database-backed API clients.
- Admin dashboard, user management, event management, event types, tags, planner approvals, finance, SMS requests, and buyer grids.
- Planner profile approval workflow and admin review pages.
- Planner finance balances, commission transactions, withdrawal requests, and admin payout review.
- Event participant SMS request approval with queue/log entities.
- Event education restrictions linked to user profile education.
- Country/city, gender, and education lookup tables linked to profiles/events.
- Event tag normalization with `Tags` and `EventTags`.
- Iranian Rial display across admin/control-center pricing and finance UI.
- `/Events/Buyers/{eventId}` with filterable buyer grid, emergency refund for admins, planner-safe mobile privacy, and profile links.
- Reusable `/UserProfiles/Details/{userId}` page with profile gallery, facts, interests, tickets, and admin-only mobile visibility.
- `UserProfileImages` aggregate/table with max 3 profile images.
- Sample profiles for `رامین`, `آرین`, `بهاره`, `علی رضا`, `شایان`, and `یاسمن` using images from `wwwroot/images/sample-profiles`.

## Verification

Last verified locally:

```powershell
dotnet build Randevoo.sln --no-restore
dotnet test Randevoo.sln --no-build
```

Result:

- Build passed with 0 warnings and 0 errors.
- Unit tests passed: 22/22.
- Integration tests passed: 22/22.
- Admin panel was published and restarted on `http://localhost:5075`.
- Smoke checks passed for `/Events/Buyers/1` and `/UserProfiles/Details/6`.

## Run Notes

To restart the local admin panel:

```powershell
Get-NetTCPConnection -LocalPort 5075 -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.OwningProcess -ErrorAction SilentlyContinue }
dotnet publish src\Randevoo.AdminPanel\Randevoo.AdminPanel.csproj -c Release -o artifacts\live-adminpanel-sms-v2 --no-restore
$env:ASPNETCORE_ENVIRONMENT='Development'
Start-Process -FilePath "C:\_D\Randevoo\artifacts\live-adminpanel-sms-v2\Randevoo.AdminPanel.exe" -ArgumentList "--urls","http://localhost:5075" -WorkingDirectory "C:\_D\Randevoo\artifacts\live-adminpanel-sms-v2" -WindowStyle Hidden
```

Admin-panel test login:

- Admin: `09125177721`
- Planner: `09125550000`
- Verification code in Development: `123456`

Use `curl.exe --noproxy "*"` for localhost HTTP smoke tests.

## Important Implementation Notes

- `src/Randevoo.AdminPanel/appsettings.Development.json` now contains the local Development SQL Server connection and enables sample data.
- `Location_Country`, `Location_City`, and `EventTagsSerialized` were removed from mapped usage in favor of lookup IDs and normalized tags.
- DTO mappings include lookup-ID fallbacks so API responses remain stable even when lookup navigation properties are not explicitly included.
- The published `artifacts/` folder and local `*.log` files are ignored and should not be committed.
- GitHub CLI (`gh`) is not installed in this environment, so local push is possible but PR automation through `gh` is not.

## Better Prompt For Next Codex Pass

Use this prompt as the next implementation request. It is intentionally split into three steps so Codex can work safely, verify each layer, and avoid mixing unrelated UI, finance, and event-domain changes.

```text
You are working in the Randevoo admin panel. Plan first, then implement in three validated steps. Preserve the existing RTL Persian visual language, use database-backed services, and keep admin/planner privacy rules strict.

Step 1: Admin Shell, Navigation, Icons, And Practical Width
- Move the admin-panel side menu to the right side of the master layout and make it slimmer so practical pages have more width.
- Make the header logo bigger.
- Rename all visible branding from "رندوو" to "راندوو" and use the tagline "پلتفرم برگزاری رویداد با چاشنی Dating".
- Separate AdminUser and PlannerUser menus.
- For AdminUser, support two-level menus with Bootstrap Icons/Glyphicons-style icons for every menu and action button.
- Suggested admin menu groups:
  - عملیات برگزار کننده: لیست برگزارکنندگان, تایید پروفایل های تغییر یافته, تایید درخواست تسویه حساب
  - رویدادها: لیست رویدادهای فعال و در حال آماده سازی, لیست رویدادهای آرشیو شده و تمام شده, داشبورد رویدادها
  - شرکت کنندگان: لیست شرکت کنندگان, داشبورد شرکت کنندگان
  - مالی: تراکنش های خرید بلیت به تفکیک رویداد, بررسی درخواست های تسویه, تراکنش های مالی برگزارکننده
- Apply consistent icon buttons and dropdown action menu styling across all grids. The "اقدام ها" dropdown must not push scroll or layout and must open above content with correct z-index/positioning.

Step 2: Buyers, End User Operations, Payments, And Planner Bank Accounts
- In `/Events/Buyers/{eventId}`, show a small user image in the grid for event-place check-in.
- Add grid/card view toggle at the top.
- Add buyer actions:
  - نمایش پروفایل کاربر
  - نمایش موجودی کلی کاربر
  - نمایش پرداخت های کلی کاربر
- Admin-only buyer actions:
  - ویرایش کامل پروفایل کاربر in a separate admin page, including disable user, add/remove images, add/remove interests.
  - ارسال پیامک فوری به کاربر and save/log the message.
- Create an `OnlinePayments` table linked to user balance/ticket sale transactions. Do not implement real online payment gateway yet; only model records and display them.
- In planner profile, show planner mobile publicly because it already comes from user login.
- Add planner bank account entity/page visible only to admin and the planner themself:
  - شماره کارت
  - شماره شبا
  - بانک
  - فعال/غیرفعال
- Add admin page for "تراکنش های مالی برگزار کننده" for a specific planner and link it from planner profile/actions.

Step 3: Event Domain Expansion, Filtering UX, FAQ, Online Events, Monitoring
- Add event type/mode lookup for "Online = آنلاین" and "In-person = حضوری". Implement full backend, DB migration, admin UI, create/edit forms, details/profile display, and DTO mapping.
- For online events, add online platform fields such as Google Meet, Zoom, اسکای روم, Adobe Connect, join link, and any required online access notes.
- If event mode is online, planner should not have to fill location fields, and event profile should not show location.
- Add event FAQ as a separate DB table/list of Q&A items. Planner completes it when creating/editing an event. Add a management page linked from event grids. Show FAQ on event profile only when at least one Q&A exists.
- Improve Events grid for future scale of 1000+ events:
  - Collapsible/toggle filter bar for tag, city, type/mode, date, planner, title.
  - Sort/order combo for important factors.
  - Server-side paging.
- Add event-grid actions:
  - Monitor EventSurveyRatings and show user feedback; seed sample data.
  - Admin-only Monitor EventConversations and chats; seed sample data.
- Create separate scalable admin pages for:
  - "تراکنش های خرید بلیت به تفکیک رویداد"
  - "بررسی درخواست های تسویه برگزارکنندگان"
- Redirect/link from "خریداران بلیت" to the relevant event transaction page.

Acceptance criteria:
- Add or update EF migrations and sample data.
- Keep clean architecture boundaries: domain rules in domain entities, app behavior in handlers/services, Razor Pages thin.
- Protect privacy: planners must not see end-user mobile numbers or private payment details unless explicitly allowed.
- Verify with `dotnet build Randevoo.sln --no-restore` and `dotnet test Randevoo.sln --no-build`.
- Smoke-test the main admin pages on `http://localhost:5075`.
```
