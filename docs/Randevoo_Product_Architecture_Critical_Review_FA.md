# سند پیشنهادی و نقد تحلیلی پروژه Randevoo

> تاریخ تهیه: 2026-06-10  
> دامنه بررسی: مستندات موجود در `docs` به همراه `README.md` و `HANDOFF.md` در ریشه مخزن.  
> نکته مهم: این گزارش بر اساس مستندات و شواهد پیاده‌سازی فعلی نوشته شده است، نه مصاحبه با مالک محصول. هر جا تصمیم محصولی در مستندات صریح نبود، با برچسب **فرض** مشخص شده است.

## 0. فهرست مستندات یافت‌شده و برداشت کوتاه

| فایل | برداشت کوتاه |
| --- | --- |
| `README.md` | معرفی محصول، معماری، نقش‌ها، احراز هویت، اجرای پروژه و شکاف‌های شناخته‌شده. |
| `HANDOFF.md` | وضعیت تحویل محلی، قابلیت‌های اخیر ادمین‌پنل، اجرای محلی، تست‌ها و پرامپت پیشنهادی بعدی. |
| `docs/00-index.md` | فهرست اصلی مستندات تولیدشده و یافته‌های سطح بالا. |
| `docs/01-repository-analysis/repository-map.md` | نقشه پوشه‌ها و نواحی مالکیت کد. |
| `docs/01-repository-analysis/solution-structure.md` | پروژه‌های solution و وابستگی‌های بین لایه‌ها. |
| `docs/01-repository-analysis/project-dependencies.md` | جهت وابستگی پروژه‌ها و ریسک coupling. |
| `docs/01-repository-analysis/package-inventory.md` | فهرست بسته‌های NuGet. |
| `docs/01-repository-analysis/source-code-inventory.md` | موجودی فایل‌های منبع در نواحی مختلف. |
| `docs/02-product-overview/product-vision.md` | چشم‌انداز محصول بر اساس شواهد کد، نه سند محصول مستقل. |
| `docs/02-product-overview/problem-solution.md` | مسئله و راه‌حل استنباط‌شده از پیاده‌سازی. |
| `docs/02-product-overview/personas.md` | پرسونای مهمان، کاربر، برگزارکننده، پشتیبان و ادمین. |
| `docs/02-product-overview/user-roles.md` | نقش‌های کشف‌شده در enum و policyها. |
| `docs/02-product-overview/glossary.md` | واژه‌نامه موجودیت‌ها و مفاهیم دامنه. |
| `docs/02-product-overview/current-feature-map.md` | قابلیت‌های پیاده‌سازی‌شده و نیمه‌کامل. |
| `docs/03-requirements/functional-requirements.md` | نیازمندی‌های تابعی استخراج‌شده از endpointها و handlerها. |
| `docs/03-requirements/non-functional-requirements.md` | وضعیت NFRها و شکاف‌هایی مثل نبود بودجه کارایی و DR. |
| `docs/03-requirements/user-stories.md` | user storyهای استنباطی از use caseهای موجود. |
| `docs/03-requirements/business-rules.md` | قواعد کسب‌وکار قابل مشاهده در entity/handler. |
| `docs/03-requirements/permissions-matrix.md` | ماتریس نقش‌ها و دسترسی‌ها. |
| `docs/04-domain-analysis/domain-overview.md` | نمای کلی دامنه، entityها، value objectها و enumها. |
| `docs/04-domain-analysis/entity-catalog.md` | کاتالوگ همه entityهای دامنه. |
| `docs/04-domain-analysis/aggregate-boundaries.md` | مرزهای احتمالی aggregateها بر اساس repositoryها. |
| `docs/04-domain-analysis/value-objects.md` | value objectهای فعلی مثل AgeRange، Location، Height. |
| `docs/04-domain-analysis/enums.md` | enumهای lifecycle، پرداخت، نقش، پشتیبانی و moderation. |
| `docs/04-domain-analysis/domain-events.md` | زیرساخت domain event؛ handlerهای واقعی کامل تایید نشده‌اند. |
| `docs/04-domain-analysis/business-invariants.md` | invariantهای فعلی مثل یکتایی موبایل، ظرفیت و وضعیت‌ها. |
| `docs/04-domain-analysis/state-machines.md` | state machineهای ساده برای event، payment/order و moderation. |
| `docs/05-database/database-overview.md` | نمای EF Core و 54 DbSet فعلی. |
| `docs/05-database/dbcontext-analysis.md` | indexها، relationshipها، owned typeها و delete behavior. |
| `docs/05-database/tables-and-fields.md` | فهرست بسیار جزئی فیلدهای جدول‌ها و entityها. |
| `docs/05-database/relationships.md` | رابطه‌های تشخیص‌داده‌شده از FKها. |
| `docs/05-database/indexes-and-constraints.md` | indexها و constraintهای قابل مشاهده. |
| `docs/05-database/migrations.md` | فهرست 39 migration و روند تکامل schema. |
| `docs/05-database/seed-data.md` | seed data و هشدار درباره داده نمونه در تولید. |
| `docs/05-database/erd.md` | ERD تولیدشده از entityها. |
| `docs/06-backend/backend-overview.md` | نقش لایه‌های Domain، Application، Infrastructure و WebApi. |
| `docs/06-backend/application-layer.md` | کاتالوگ command/query/handler/DTOها. |
| `docs/06-backend/domain-layer.md` | مسئولیت‌ها و inventory لایه دامنه. |
| `docs/06-backend/infrastructure-layer.md` | repositoryها، EF، JWT، audit، SMS/Email console و سرویس‌ها. |
| `docs/06-backend/webapi-layer.md` | endpointها، middleware، hub و composition HTTP. |
| `docs/06-backend/dependency-injection.md` | ترکیب DI در WebApi و AdminPanel. |
| `docs/06-backend/background-jobs.md` | نبود scheduler/hosted service واقعی؛ SMS queue هست ولی worker تایید نشده. |
| `docs/06-backend/validation-and-error-handling.md` | middleware خطا و validation پراکنده در guard/handlerها. |
| `docs/07-api/api-overview.md` | خلاصه سطح API. |
| `docs/07-api/endpoints-catalog.md` | کاتالوگ endpointهای Minimal API. |
| `docs/07-api/request-response-examples.md` | نمونه‌های درخواست/پاسخ که نیاز به اعتبارسنجی DTO دارند. |
| `docs/07-api/authentication-and-authorization.md` | مدل auth و policyهای API. |
| `docs/07-api/error-handling.md` | قراردادهای خطا در API. |
| `docs/08-ui-ux/ui-overview.md` | نمای کلی AdminPanel. |
| `docs/08-ui-ux/sitemap.md` | سایت‌مپ Razor Pages. |
| `docs/08-ui-ux/screens-catalog.md` | کاتالوگ صفحه‌های ادمین‌پنل. |
| `docs/08-ui-ux/components-catalog.md` | partialها، layoutها، script و style مشترک. |
| `docs/08-ui-ux/forms-and-fields.md` | فیلدهای فرم‌های Razor. |
| `docs/08-ui-ux/user-journeys.md` | journeyهای UI در AdminPanel. |
| `docs/08-ui-ux/admin-panel.md` | backend integration، policyها و صفحه‌های عملیاتی ادمین. |
| `docs/08-ui-ux/ux-findings.md` | قوت‌ها و شکاف‌های UX از بررسی استاتیک. |
| `docs/09-system-flows/use-case-diagram.md` | actorها و use caseهای اصلی. |
| `docs/09-system-flows/registration-flow.md` | جریان login/ثبت‌نام با کد موبایل. |
| `docs/09-system-flows/profile-flow.md` | ساخت/ویرایش پروفایل کاربر. |
| `docs/09-system-flows/event-discovery-flow.md` | مشاهده eventهای باز. |
| `docs/09-system-flows/event-creation-flow.md` | ایجاد event توسط برگزارکننده/ادمین. |
| `docs/09-system-flows/event-join-flow.md` | خرید بلیت و ورود به event. |
| `docs/09-system-flows/matching-flow.md` | مدل like/conversation به جای Match مستقل. |
| `docs/09-system-flows/messaging-flow.md` | پیام‌رسانی event-scoped با SignalR. |
| `docs/09-system-flows/payment-flow.md` | ticket order، online payment و manual receipt. |
| `docs/09-system-flows/moderation-flow.md` | گزارش تخلف و review ادمین. |
| `docs/09-system-flows/notification-flow.md` | queue/console sender و نبود provider تولید. |
| `docs/10-architecture/architecture-overview.md` | معماری monolith/API لایه‌ای. |
| `docs/10-architecture/c4-context-diagram.md` | نمودار context و وابستگی به DB/SMS/Payment. |
| `docs/10-architecture/c4-container-diagram.md` | containerهای AdminPanel، WebApi، Application، Domain، Infrastructure. |
| `docs/10-architecture/c4-component-diagram.md` | componentهای endpoint، handler، repository، middleware و hub. |
| `docs/10-architecture/deployment-diagram.md` | فرض‌های deployment با IIS/Kestrel/Reverse proxy. |
| `docs/10-architecture/clean-architecture-boundaries.md` | مرزهای Clean Architecture و وضعیت فعلی. |
| `docs/10-architecture/dependency-rules.md` | قواعد وابستگی بین لایه‌ها. |
| `docs/10-architecture/architecture-risks.md` | ریسک‌های معماری مثل DbContext بزرگ، finance و notification. |
| `docs/10-architecture/architecture-decisions/adr-001-project-structure.md` | ADR ساختار پروژه. |
| `docs/10-architecture/architecture-decisions/adr-002-database-strategy.md` | ADR استراتژی EF Core/database. |
| `docs/10-architecture/architecture-decisions/adr-003-authentication-strategy.md` | ADR احراز هویت mobile/JWT/cookie. |
| `docs/10-architecture/architecture-decisions/adr-004-event-matching-strategy.md` | ADR matching فعلی مبتنی بر like/conversation. |
| `docs/10-architecture/architecture-decisions/adr-005-ui-architecture.md` | ADR Razor Pages AdminPanel. |
| `docs/11-security-privacy/security-overview.md` | مدل امنیت، privacy، audit و داده حساس. |
| `docs/11-security-privacy/authentication.md` | جریان‌های auth API و AdminPanel. |
| `docs/11-security-privacy/authorization.md` | policyها و نقش‌ها. |
| `docs/11-security-privacy/sensitive-data.md` | دسته‌های داده حساس: هویت، پروفایل، چت، مالی، پشتیبانی. |
| `docs/11-security-privacy/privacy-model.md` | export/delete و نگرانی retention. |
| `docs/11-security-privacy/abuse-prevention.md` | گزارش، block، support و audit؛ نبود rate limit/automated abuse تاییدشده. |
| `docs/11-security-privacy/moderation-policy.md` | workflow فعلی moderation. |
| `docs/11-security-privacy/security-gaps.md` | شکاف‌های provider، webhook، upload، CSRF، secret و retention. |
| `docs/12-configuration-devops/configuration-overview.md` | فایل‌های config و environment. |
| `docs/12-configuration-devops/appsettings.md` | appsettings بدون افشای secret. |
| `docs/12-configuration-devops/environment-variables.md` | انتظارات configuration محیطی. |
| `docs/12-configuration-devops/local-development.md` | setup محلی. |
| `docs/12-configuration-devops/build-and-run.md` | فرمان‌های build/run. |
| `docs/12-configuration-devops/database-migrations.md` | روند EF migration. |
| `docs/12-configuration-devops/iis-deployment.md` | فرض‌های IIS. |
| `docs/12-configuration-devops/docker.md` | وضعیت Docker. |
| `docs/12-configuration-devops/ci-cd.md` | وضعیت CI/CD، بیشتر instructionها تا workflow واقعی. |
| `docs/12-configuration-devops/logging-monitoring.md` | logging، audit و نبود APM/monitoring تولید. |
| `docs/13-testing/testing-overview.md` | پروژه‌های تست و تعداد testها. |
| `docs/13-testing/existing-tests.md` | موجودی تست‌های unit/integration. |
| `docs/13-testing/test-coverage-summary.md` | پوشش فعلی و ضعف‌های UI/payment/privacy. |
| `docs/13-testing/recommended-test-scenarios.md` | سناریوهای پیشنهادی تست. |
| `docs/13-testing/unit-tests.md` | وضعیت unit testها. |
| `docs/13-testing/integration-tests.md` | وضعیت integration testها. |
| `docs/13-testing/api-tests.md` | نبود پروژه API test مستقل. |
| `docs/13-testing/ui-tests.md` | نبود تست UI مرورگری. |
| `docs/14-roadmap/current-state.md` | وضعیت فعلی سیستم و dirty worktree. |
| `docs/14-roadmap/known-gaps.md` | شکاف‌های شناخته‌شده مثل payment provider، notification، CI، Docker. |
| `docs/14-roadmap/technical-debt.md` | بدهی فنی در DbContext، finance، AdminPanel و authorization. |
| `docs/14-roadmap/recommended-next-steps.md` | گام‌های پیشنهادی بعدی. |
| `docs/14-roadmap/future-features.md` | قابلیت‌های احتمالی آینده. |
| `docs/15-ai-agent-context/ai-coding-guidelines.md` | راهنمای کار agentهای آینده. |
| `docs/15-ai-agent-context/safe-change-rules.md` | قواعد جلوگیری از خراب‌کردن کار فعال. |
| `docs/15-ai-agent-context/repository-context-for-future-agents.md` | context فشرده برای agentهای آینده. |
| `docs/15-ai-agent-context/documentation-extraction-report.md` | metadata استخراج مستندات، entityها، APIها، نقش‌ها و ریسک‌ها. |

## 1. خلاصه مدیریتی

Randevoo یک dating app ساده نیست؛ ایده اصلی آن «آشنایی از مسیر رویداد واقعی» است. این تفاوت محصولی ارزشمند است، چون اعتماد، کیفیت تعامل، تجربه مشترک و کنترل عملیاتی را بالاتر از swipe/like خام قرار می‌دهد. در مستندات و کد فعلی، ستون‌های اصلی محصول دیده می‌شود: احراز هویت موبایلی، پروفایل، برگزارکننده، رویداد، بلیت، پرداخت، گفت‌وگوی بعد از event، survey، moderation، support، finance و admin panel.

نقطه قوت اصلی این است که محصول از روز اول فقط روی matching دیجیتال بنا نشده و برای operational reality آماده‌تر از یک dating MVP خام است. entityهای مالی، support ticket، audit log، role permission، manual payment receipt، planner bank account و SMS queue نشان می‌دهند تیم به عملیات فکر کرده است.

اما ریسک اصلی جدی است: business processهای حیاتی هنوز به صورت lifecycle رسمی و غیرقابل‌ابهام مدل نشده‌اند. ظرفیت، بالانس جنسیتی، حداقل ظرفیت، waitlist، cancellation، refund dispute، no-show، confirmation نهایی event و reconciliation مالی بیشتر به صورت تکه‌های پراکنده دیده می‌شوند، نه قرارداد دامنه‌ای یکپارچه. اگر همین مدل با هزاران کاربر و صدها event لانچ شود، اولین فشار هم‌زمان روی خرید بلیت، کنسلی، پرداخت ناموفق یا event کم‌ظرفیت می‌تواند inconsistency عملیاتی و مالی تولید کند.

جمع‌بندی بی‌پرده: foundation مهندسی بد نیست؛ اما policy محصول، state machineها، concurrency، refund/cancellation و trust/safety هنوز برای production-grade event dating کافی نیستند.

## 2. برداشت من از مدل کسب‌وکار فعلی

**برداشت فعلی:** Randevoo مارکت‌پلیس یا پلتفرم رویدادهای dating/social است که برگزارکننده event ایجاد می‌کند، کاربر بلیت می‌خرد، platform یا organizer پرداخت را دریافت می‌کند، و platform از commission، ticketing، settlement یا خدمات جانبی درآمد می‌گیرد.

**فرض:** مدل درآمدی فعلی ترکیبی از کمیسیون فروش بلیت، کارمزد برگزارکننده، احتمالا subscription/premium بعدی، و شاید promotion/discount campaign است. مستندات مدل درآمدی مستقل، سیاست قیمت‌گذاری، CAC/LTV، margin، break-even رویداد و سیاست settlement را صریح نکرده‌اند.

ریسک شکست مدل کسب‌وکار:

- اگر eventها به حداقل ظرفیت نرسند، اعتماد کاربر و برگزارکننده آسیب می‌بیند.
- اگر gender balance وعده داده شود ولی عملی نشود، core value محصول زیر سوال می‌رود.
- اگر سیاست کنسلی و refund مبهم باشد، support و dispute هزینه‌زا می‌شود.
- اگر برگزارکننده کیفیت ضعیف داشته باشد، برند platform آسیب می‌بیند حتی اگر event توسط شخص ثالث برگزار شود.
- اگر کاربر fake/low-quality وارد event شود، تجربه حضوری خطرناک‌تر از dating آنلاین است.
- اگر payment به صورت دستی و gateway هم‌زمان رشد کند، reconciliation بدون ledger جدی سخت می‌شود.

مدل کسب‌وکار باید قبل از رشد، چند تصمیم روشن داشته باشد: چه کسی merchant of record است، پول چه زمانی به برگزارکننده آزاد می‌شود، event چه زمانی confirm می‌شود، refund چه زمانی full/partial/none است، و هزینه no-show یا late cancellation چطور مدیریت می‌شود.

## 3. نقد دامنه و موجودیت‌ها

| موجودیت | کفایت فعلی | کمبود/اصلاح لازم |
| --- | --- | --- |
| `User` | برای auth، role و active بودن کافی شروع شده است. | نیاز به risk score، verification level، suspension reason، trust status، duplicate detection، device/IP history و consent flags دارد. |
| `UserProfile` | اطلاعات پایه dating profile را دارد. | باید completeness score، visibility status، moderation status، verification status، privacy flags، relationship preferences و last reviewed fields داشته باشد. |
| `DatingEvent` | entity مرکزی قوی است اما بیش از حد پرمسئولیت شده. | event status باید رسمی‌تر شود؛ ظرفیت، gender balance، cancellation policy، confirmation rules و operational checklist بهتر است جدا یا embedded contract روشن داشته باشند. |
| `EventTicket` | نقش participant/ticket را تا حدی پوشش می‌دهد. | با `EventRegistration` یکی گرفته شده است؛ باید registration lifecycle، attendance، cancellation، refund eligibility، waitlist rank و check-in جدا شود. |
| `TicketOrder` | برای پرداخت و سفارش لازم است. | باید idempotency key، payment attempt count، expiresAt، reservation hold، source channel و reconciliation status داشته باشد. |
| `EventCapacity` | entity مستقل دیده نمی‌شود. | باید برای total/male/female/minimum/hold/reserved/confirmed/cancelled ظرفیت snapshot داشته باشد. |
| `EventGenderBalance` | بیشتر در فیلدهای capacity male/female و gender ticket دیده می‌شود. | باید rule مستقل داشته باشد: ratio، tolerance، lock point، imbalance action و waitlist promotion rules. |
| `OnlinePayment` | مدل پرداخت آنلاین وجود دارد. | gateway provider، external transaction id، webhook signature state، retry، failure reason، idempotency و reconciliation لازم است. |
| `ManualPaymentReceipt` | برای پرداخت دستی خوب است. | fraud review، duplicate receipt detection، bank reference، reviewer SLA، rejection category و audit attachment policy لازم است. |
| `Refund` | entity مستقل دیده نمی‌شود. | باید مستقل شود؛ refund request، refund transaction، eligibility decision، processor reference، partial amount و dispute link لازم است. |
| `Match` | مستقل وجود ندارد؛ like/conversation استفاده شده است. | اگر product نیاز به match رسمی دارد، Match باید جدا باشد؛ اگر ندارد، واژه match در محصول باید با conversation/like دقیق شود. |
| `Notification` | `SmsQueueItem` هست، اما notification عمومی نیست. | Notification/NotificationDelivery/Template/Preference برای SMS، email، push و in-app لازم است. |
| `Organizer` | به شکل `EventPlannerProfile` وجود دارد. | organizer quality، contract status، payout policy، verification، blacklisting، SLA و team members باید اضافه شود. |
| `Admin` | نقش است، entity عملیاتی نیست. | Admin action، intervention case، approval workflow و permission audit باید رسمی‌تر شود. |
| `Cancellation` | احتمالا در status و commandها پراکنده است. | CancellationRequest/Policy/Reason/Actor/Deadline/RefundImpact باید entity یا value object شود. |
| `WaitingList` | دیده نمی‌شود. | برای full/balance/minimum capacity ضروری است: rank، gender bucket، auto-promote، expiration. |
| `AuditLog` | وجود دارد و مهم است. | باید coverage الزامی برای finance، role change، cancellation، refund، moderation، event status و manual override داشته باشد. |
| `ModerationReport` | موجود است. | escalation، evidence، action taken، user sanction، appeal و SLA لازم است. |
| `Report/Complaint` | با moderation/support پوشش جزئی دارد. | complaint مالی/رویدادی/امنیتی باید از moderation محتوایی تفکیک شود. |
| `UserVerification` | مستقل دیده نمی‌شود. | KYC/light verification، mobile/email/photo/selfie/social trust، verification provider و expiry لازم است. |
| `EventFeedback` | survey response/rating وجود دارد. | no-show reason، organizer score، participant quality score، public/private feedback و complaint link لازم است. |
| `NoShowTracking` | مستقل دیده نمی‌شود. | check-in، no-show، late arrival، repeat no-show penalty و refund impact لازم است. |

نتیجه دامنه: مدل فعلی entityهای زیادی دارد، اما چند aggregate حیاتی اشتباه در هم تنیده شده‌اند. `EventTicket` نباید هم‌زمان ticket، registration، attendance و refund anchor باشد. `DatingEvent` نباید تمام قواعد capacity، gender, pricing, review, operational status, online/offline و finance را بدون boundary روشن حمل کند.

## 4. نقد فرآیندهای اصلی سیستم

| فرآیند | مشکل احتمالی | ریسک عملیاتی | پیشنهاد اصلاح |
| --- | --- | --- | --- |
| ثبت‌نام کاربر | auth موبایلی هست، اما trust onboarding کم‌رنگ است. | ورود fake/abusive user. | افزودن verification level، device/IP rate limit، profile approval اختیاری. |
| تکمیل پروفایل | profile هست ولی completeness/approval lifecycle روشن نیست. | کاربران ناقص وارد خرید یا interaction شوند. | تعریف `ProfileStatus`: Draft, Complete, UnderReview, Approved, Suspended. |
| مشاهده و انتخاب ایونت | فیلتر و لیست هست، اما performance contract و eligibility filter کامل نیست. | query سنگین، نمایش event نامناسب به کاربر. | pagination اجباری، index composite، eligibility pre-filter. |
| ثبت‌نام در ایونت | خرید بلیت ثبت می‌شود، اما reservation hold و waitlist formal نیست. | oversell در رقابت هم‌زمان. | transaction boundary با rowversion/serializable section و `RegistrationHold`. |
| پرداخت | online/manual مدل شده، gateway واقعی و webhook تایید نشده. | پول نامشخص، سفارش paid نشده یا duplicate. | idempotency، payment attempt، webhook verification، reconciliation job. |
| ظرفیت و بالانس جنسیتی | capacity male/female هست، ولی rule lifecycle کامل نیست. | ظرفیت full شود ولی balance خراب بماند. | `EventCapacitySnapshot` و `GenderBalanceRule` با promotion از waitlist. |
| لغو ثبت‌نام | lifecycle مستقل دیده نمی‌شود. | refund اشتباه و seat آزادنشده. | `CancellationRequest` و policy زمان‌محور. |
| لغو ایونت | cancel endpoint هست، اما cascade عملیاتی کامل نیست. | refund گروهی، notification و settlement ناقص. | workflow cancel با batch refund، notification، organizer penalty و audit. |
| لیست رزرو | دیده نمی‌شود. | lost revenue و نارضایتی هنگام full شدن. | `WaitingListEntry` با rank، gender bucket، expiry. |
| تایید نهایی ایونت | status ساده است. | event بدون حداقل ظرفیت/بالانس confirm شود. | state machine رسمی event و pre-confirmation checklist. |
| حضور در ایونت | check-in/no-show entity نیست. | quality score غلط، refund dispute. | `EventAttendance` با QR/check-in/manual override. |
| بعد از ایونت | chat/survey هست. | interaction بدون safety guard. | window زمانی، visibility rules، abuse monitoring و close conversation policy. |
| امتیازدهی و بازخورد | survey هست. | feedback قابل اقدام برای organizer ناکافی. | public/private feedback، organizer KPI و action plan. |
| گزارش تخلف | moderation وجود دارد. | escalation امنیتی کند. | severity، SLA، emergency path، user sanction، appeal. |
| فرایندهای ادمین | admin panel وسیع است. | manual intervention بدون playbook. | admin case management، queue، runbook، audit-required actions. |

## 5. چالش‌های آینده در مقیاس بالا

وقتی کاربران، eventها و registrationها زیاد شوند، مشکل اصلی فقط CPU یا database نیست؛ مشکل «تصمیم هم‌زمان کسب‌وکار» است. چند مثال:

- چند کاربر هم‌زمان آخرین ظرفیت زن/مرد را می‌خرند.
- payment callback دیر می‌رسد، اما seat هنوز hold شده است.
- کاربر آخرین لحظه cancel می‌کند و gender balance event را خراب می‌کند.
- organizer event را cancel می‌کند، اما پول بخشی به برگزارکننده settle شده است.
- admin باید بین refund، waitlist promotion، notification و complaint تصمیم بگیرد.

ریسک‌های فنی:

- **Database indexing:** برای event list باید indexهای `(IsCancelled, IsOpenForSell, DateTimeEnd)`, `(ReviewStatus, DateTimeStart)`, location/type/tag و payment status کافی باشند؛ برای گزارش‌ها indexهای ترکیبی date/status/entity لازم است.
- **Query performance:** صفحات admin مثل participants، buyers، finance، support و logs باید server-side paging و projection داشته باشند؛ includeهای عمیق با رشد داده کند می‌شوند.
- **Pagination:** هیچ list بزرگ نباید بدون page size محدود باشد.
- **Caching:** lookupها، event type، city/country، tags و public event detail قابل cache هستند؛ capacity/payment نباید cache ساده شود مگر با invalidation دقیق.
- **Background jobs:** SMS delivery، email، payment reconciliation، refund batch، waitlist promotion، event auto-state transitions و report aggregation باید job شوند.
- **Message queues:** notification و finance side-effectها نباید فقط synchronous handler باشند.
- **Concurrency control:** خرید بلیت و discount usage باید optimistic locking/rowversion یا transaction isolation مناسب داشته باشد.
- **Transaction boundaries:** payment/order/ticket/balance باید atomic یا با outbox قابل جبران باشد.
- **Idempotency:** endpoint خرید، receipt submission، payment webhook، refund و admin action باید idempotent باشند.
- **Event state machine:** وضعیت‌ها باید transition guard داشته باشند، نه booleanهای پراکنده.
- **Audit logging:** finance، cancellation، refund، role/permission، moderation و manual override باید audit اجباری داشته باشند.
- **Monitoring:** dashboard فنی برای error rate، queue lag، failed notification، failed payment، oversell attempt، refund backlog و support SLA لازم است.

## 6. پیشنهاد مدل وضعیت برای Event

وضعیت‌های پیشنهادی:

| وضعیت | معنی |
| --- | --- |
| `Draft` | event در حال آماده‌سازی است و عمومی نیست. |
| `Published` | صفحه event قابل مشاهده است اما registration هنوز باز نیست. |
| `OpenForRegistration` | ثبت‌نام/خرید فعال است. |
| `WaitingForMinimumCapacity` | ثبت‌نام باز است ولی حداقل ظرفیت پر نشده. |
| `Balanced` | حداقل ظرفیت و قاعده gender balance فعلا رعایت شده. |
| `Full` | ظرفیت قابل فروش پر شده؛ فقط waitlist ممکن است. |
| `Confirmed` | event از نظر عملیاتی نهایی شده و cancellation سخت‌تر می‌شود. |
| `CancelledByOrganizer` | organizer لغو کرده؛ نیازمند refund و penalty بررسی‌شده. |
| `CancelledByAdmin` | admin به دلایل عملیاتی/امنیتی/کیفی لغو کرده. |
| `Completed` | event برگزار شده و وارد survey/chat/post-event شده. |
| `Archived` | پرونده عملیاتی و مالی event بسته شده است. |

transitionهای معتبر:

- `Draft -> Published -> OpenForRegistration`
- `OpenForRegistration -> WaitingForMinimumCapacity`
- `WaitingForMinimumCapacity -> Balanced`
- `Balanced -> Full`
- `Balanced|Full -> Confirmed`
- `Confirmed -> Completed -> Archived`
- `Published|OpenForRegistration|WaitingForMinimumCapacity|Balanced|Full|Confirmed -> CancelledByOrganizer|CancelledByAdmin`

transitionهای نامعتبر:

- `Cancelled -> OpenForRegistration` بدون event جدید یا admin recovery رسمی.
- `Completed -> Cancelled`؛ بعد از برگزاری باید dispute/refund جدا ثبت شود.
- `Archived -> هر وضعیت عملیاتی`؛ فقط audit/correction مجاز است.
- `Draft -> Confirmed` بدون publish، registration و checklist.

## 7. پیشنهاد مدل وضعیت برای Registration

وضعیت‌های پیشنهادی:

| وضعیت | معنی |
| --- | --- |
| `Pending` | ثبت اولیه یا hold کوتاه‌مدت ایجاد شده است. |
| `WaitingForPayment` | seat موقت نگه داشته شده و پرداخت لازم است. |
| `Paid` | پرداخت موفق یا receipt تایید شده است. |
| `Waitlisted` | ظرفیت یا gender bucket پر است؛ کاربر در انتظار است. |
| `Confirmed` | حضور کاربر در event نهایی شده است. |
| `CancelRequested` | کاربر/ادمین/organizer درخواست لغو داده است. |
| `Cancelled` | ثبت‌نام لغو شده و seat آزاد شده است. |
| `Refunded` | بازگشت وجه انجام شده یا transaction refund ثبت شده است. |
| `Rejected` | به دلیل rule/profile/payment/manual review رد شده است. |
| `Attended` | کاربر check-in شده است. |
| `NoShow` | کاربر حاضر نشده است. |

edge caseهای ضروری:

- `WaitingForPayment` باید expiry داشته باشد؛ بعد از expiry به `Cancelled` یا `WaitlistedPromotionExpired` برود.
- `Paid` لزوما `Confirmed` نیست اگر event هنوز minimum/balance ندارد.
- `Confirmed -> CancelRequested -> Cancelled -> Refunded` باید policy زمان‌محور داشته باشد.
- `Confirmed -> NoShow` ممکن است refund را صفر کند و trust score را کاهش دهد.
- `Waitlisted -> WaitingForPayment` باید deadline کوتاه و notification مطمئن داشته باشد.

## 8. پیشنهاد موجودیت‌های جدید

| نام موجودیت | هدف | فیلدهای پیشنهادی | ارتباط | دلیل اهمیت |
| --- | --- | --- | --- | --- |
| `EventRegistration` | جداکردن registration از ticket/payment. | EventId, UserId, Status, Gender, RegisteredAt, ConfirmedAt, Source | Event, User, TicketOrder | محور lifecycle حضور. |
| `EventCapacitySnapshot` | کنترل ظرفیت در لحظه. | EventId, Total, Male, Female, Reserved, Paid, Waitlisted, Version | Event | جلوگیری از oversell و گزارش دقیق. |
| `GenderBalanceRule` | تعریف policy بالانس. | EventId, MaleMin/Max, FemaleMin/Max, Ratio, Tolerance, LockAt | Event | جلوگیری از تصمیم‌های سلیقه‌ای. |
| `WaitingListEntry` | مدیریت رزرو. | EventId, UserId, Gender, Rank, Status, ExpiresAt | EventRegistration | پرکردن seat آزاد و حفظ balance. |
| `CancellationRequest` | ثبت لغو. | ActorUserId, Reason, RequestedAt, PolicyResult, Status | Registration/Event | audit و refund صحیح. |
| `RefundRequest` | درخواست بازگشت وجه. | RegistrationId, Amount, Reason, Status, ApprovedBy | TicketOrder/Payment | dispute و finance clarity. |
| `RefundTransaction` | transaction مالی refund. | PaymentId, ProviderRef, Amount, Status, ProcessedAt | OnlinePayment/Balance | reconciliation. |
| `PaymentAttempt` | هر تلاش پرداخت. | OrderId, Provider, ExternalId, Status, FailureCode, IdempotencyKey | TicketOrder | payment reliability. |
| `Notification` | پیام منطقی. | UserId, Type, TemplateKey, Payload, Priority | User | ارتباط چندکاناله. |
| `NotificationDelivery` | ارسال کانالی. | NotificationId, Channel, Status, AttemptCount, ProviderMessageId | Notification | retry و delivery audit. |
| `UserVerification` | trust و احراز هویت. | UserId, Type, Status, Provider, VerifiedAt, ExpiresAt | User/Profile | کاهش fake user. |
| `EventAttendance` | check-in/no-show. | EventId, UserId, Status, CheckedInAt, Method, ReviewedBy | Registration | کیفیت event و dispute. |
| `AdminInterventionCase` | پرونده دخالت ادمین. | EntityType, EntityId, Reason, Status, AssignedTo, Resolution | همه entityها | مدیریت عملیات دستی. |
| `OrganizerQualityScore` | کیفیت برگزارکننده. | OrganizerId, Score, Components, UpdatedAt | EventPlannerProfile | کنترل کیفیت marketplace. |
| `FinancialReconciliationBatch` | بستن مالی دوره‌ای. | Period, Status, Total, MismatchCount, ClosedBy | Payments/Balance | جلوگیری از اختلاف مالی. |

## 9. پیشنهاد اصلاح موجودیت‌های فعلی

| موجودیت فعلی | مشکل فعلی | تغییر پیشنهادی | تاثیر |
| --- | --- | --- | --- |
| `DatingEvent` | مسئولیت بیش از حد و وضعیت عملیاتی ناکافی. | استخراج capacity/balance/policy و افزودن state machine رسمی. | کاهش bug در lifecycle. |
| `EventTicket` | نقش registration/attendance/refund را قاطی می‌کند. | محدود به ticket entitlement؛ registration مستقل شود. | خوانایی و کنترل بهتر. |
| `TicketOrder` | payment lifecycle و idempotency کامل نیست. | افزودن hold expiry، idempotency، reconciliation status. | کاهش double charge/oversell. |
| `OnlinePayment` | provider/webhook جزئیات ندارد. | افزودن provider ref، webhook status، signature validation result. | آمادگی gateway واقعی. |
| `ManualPaymentReceipt` | fraud/dispute کم‌رنگ است. | duplicate detection، reject category، review SLA. | کاهش خطای انسانی و تقلب. |
| `ModerationReport` | از action/sanction جدا نیست. | افزودن `ModerationAction` و appeal. | trust/safety قابل پیگیری. |
| `SmsQueueItem` | فقط SMS و نه notification عمومی. | افزودن notification abstraction یا گسترش به delivery. | multi-channel delivery. |
| `AuditLog` | وجود دارد اما coverage policy نامشخص است. | audit policy per action و required reason. | مسئولیت‌پذیری ادمین. |
| `SupportTicket` | support و complaint/business dispute ممکن است مخلوط شود. | categoryهای مالی/امنیتی/رویدادی و SLA جدا. | صف‌بندی بهتر عملیات. |

## 10. پیشنهادهای بیزنسی و محصولی

- **مدل درآمدی:** کمیسیون بلیت، fee برگزارکننده، promotion fee، subscription برای organizer، و بعدا premium user features. تا قبل از MVP فقط یک مدل اصلی را قطعی کنید.
- **سیاست کنسلی:** بازه‌های زمانی شفاف: full refund تا X ساعت، partial refund تا Y ساعت، no refund بعد از confirmation، exception برای لغو organizer/admin.
- **سیاست بازگشت وجه:** refund باید با status، actor، reason، amount و SLA مستند شود؛ manual refund بدون audit ممنوع.
- **اعتبارسنجی کاربران:** mobile کافی نیست. حداقل photo/profile review، abuse score، duplicate phone/device/IP و optional identity verification لازم است.
- **امتیازدهی کاربران:** no-show، complaint، block، survey و attendance باید به trust score تبدیل شود؛ این score نباید خام به کاربر نمایش داده شود.
- **اعتماد و امنیت:** برای event حضوری، emergency report، organizer safety checklist، blocked user conflict detection و incident workflow ضروری است.
- **جلوگیری از fake user:** rate limit، device fingerprint سبک، photo moderation، profile completeness، manual review برای رفتار پرخطر.
- **عضویت/اشتراک:** فعلا زود است؛ اول ticketing و organizer commission را پایدار کنید.
- **پروموشن و تخفیف:** discount code باید max usage atomic و gender-scope safe باشد.
- **دعوت دوستان:** بعد از trust foundation اضافه شود، چون referral بدون anti-abuse fake account می‌آورد.
- **KPIهای داشبورد:** event fill rate، gender balance health، registration conversion، payment success rate، cancellation rate، refund amount، no-show rate، repeat attendance، organizer NPS، complaint rate، support SLA، notification delivery rate، revenue/commission، reconciliation mismatch.

## 11. پیشنهادهای فنی و معماری

- **Clean Architecture:** جهت وابستگی فعلی قابل قبول است؛ اما business ruleهای critical نباید در Razor/API client یا handlerهای پراکنده دفن شوند.
- **DDD:** aggregateهای پیشنهادی: `Event`, `Registration`, `Order/Payment`, `User/Profile`, `Organizer`, `Conversation`, `SupportCase`, `ModerationCase`, `Ledger`.
- **Domain events:** برای `RegistrationPaid`, `EventBalanced`, `EventConfirmed`, `RegistrationCancelled`, `RefundApproved`, `UserReported`, `NotificationRequested` event واقعی با outbox لازم است.
- **Background jobs:** Hangfire/Quartz/Worker Service برای queue delivery، payment reconciliation، waitlist promotion و event status automation.
- **CQRS:** برای dashboard/reporting مفید است؛ read model برای finance/event/admin dashboards بسازید.
- **Database schema:** برای registration/capacity/refund/attendance entity مستقل اضافه شود؛ ledger-like transactionها immutable باشند.
- **Indexing:** indexهای composite بر اساس queryهای admin و public event list؛ full-text/search جدا برای title/tag/city اگر رشد کرد.
- **Caching:** lookup/static public detail cache؛ capacity/payment/reconciliation cache با احتیاط.
- **Notification architecture:** template + user preference + delivery attempt + retry + provider fallback.
- **Payment reliability:** idempotency، webhook verification، payment attempt، reconciliation batch، ledger immutability.
- **Audit logging:** audit اجباری برای role, permission, finance, refund, cancellation, moderation, profile review, event status.
- **Admin panel:** admin actions باید reason، preview impact، confirmation و rollback/compensation guidance داشته باشند.
- **API design:** versioning، problem details استاندارد، pagination contract، idempotency header برای mutationهای مالی.
- **Testing:** تست concurrency خرید، refund، waitlist promotion، duplicate payment callback، cancellation edge، no-show و role permission matrix.

## 12. ریسک‌های مهم پروژه

| ریسک | شدت | احتمال | اثر روی کسب‌وکار | راهکار پیشنهادی |
| --- | --- | --- | --- | --- |
| oversell ظرفیت event | بالا | متوسط | بی‌اعتمادی و refund اجباری | transaction/locking، capacity snapshot، تست هم‌زمانی. |
| خراب‌شدن gender balance | بالا | بالا | از بین رفتن وعده اصلی محصول | GenderBalanceRule و waitlist bucket. |
| پرداخت موفق ولی ticket نامعتبر | بالا | متوسط | dispute مالی | idempotency، webhook، reconciliation. |
| refund مبهم | بالا | بالا | هزینه support و ریسک حقوقی | Refund policy/entity/workflow. |
| fake/unsafe users | بالا | متوسط | خطر حضوری و آسیب برند | verification، moderation، risk score. |
| no-show بالا | متوسط | بالا | event کم‌کیفیت و نارضایتی organizer | attendance، penalty، waitlist. |
| لغو organizer در لحظه آخر | بالا | متوسط | refund جمعی و خشم کاربر | organizer SLA، penalty، auto notification. |
| notification failure | متوسط | بالا | missed payment/waitlist/event updates | delivery queue، retry، provider monitoring. |
| admin manual error | بالا | متوسط | ضرر مالی یا privacy breach | permission، confirmation، audit، runbook. |
| query/report کند | متوسط | بالا | admin unusable در رشد | read model، index، pagination. |
| نبود CI/CD واقعی | متوسط | متوسط | regression مکرر | pipeline build/test/migration validation. |
| privacy deletion ناقص | بالا | متوسط | ریسک قانونی و اعتماد | data map، retention policy، tests. |
| file upload ناامن | بالا | متوسط | abuse/security incident | validation، virus scan، storage policy. |
| dirty/uncommitted feature drift | متوسط | بالا | مستندات و کد ناپایدار | branch discipline و documentation freshness. |

## 13. اولویت‌بندی فازهای بعدی

### فاز ۱: اصلاحات ضروری قبل از MVP

- تعریف رسمی `EventLifecycle` و `RegistrationLifecycle`.
- افزودن `EventRegistration`, `CancellationRequest`, `RefundRequest`, `EventAttendance`.
- قفل هم‌زمانی ظرفیت و خرید بلیت.
- idempotency برای خرید، receipt، webhook و refund.
- policy کنسلی/refund مکتوب و قابل تست.
- audit اجباری برای finance/status/admin actions.
- تست‌های concurrency و finance critical.

### فاز ۲: امکانات مهم برای لانچ

- waitlist با gender bucket.
- production SMS/email provider و delivery retry.
- payment gateway واقعی با webhook verification.
- admin runbook برای cancellation، refund، manual intervention.
- moderation escalation و user sanction.
- dashboard KPIهای لانچ: fill rate، payment success، refund، cancellation، no-show.
- Playwright smoke tests برای صفحات اصلی admin.

### فاز ۳: امکانات رشد و مقیاس‌پذیری

- read model/reporting tables برای dashboardها.
- background worker/outbox.
- notification templates/preferences.
- organizer quality score و settlement automation.
- search/filter بهینه با index strategy.
- reconciliation batch مالی.
- support SLA و queue routing.

### فاز ۴: امکانات پیشرفته و هوشمند

- risk scoring هوشمند برای fake/abuse/no-show.
- moderation خودکار تصویر/متن.
- recommendation event بر اساس profile و behavior.
- dynamic pricing/promotion.
- advanced matching اگر product واقعا Match مستقل بخواهد.
- anomaly detection برای payment/refund/support spikes.

## 14. پیشنهاد برای فایل‌های جدید مستندات

| فایل پیشنهادی | هدف |
| --- | --- |
| `docs/03-requirements/BusinessModel.md` | مدل درآمدی، commission، settlement و assumptions. |
| `docs/04-domain-analysis/EventLifecycle.md` | lifecycle رسمی event و transition guardها. |
| `docs/04-domain-analysis/RegistrationLifecycle.md` | lifecycle registration/ticket/attendance. |
| `docs/03-requirements/CancellationAndRefundPolicy.md` | سیاست کنسلی و refund با مثال. |
| `docs/04-domain-analysis/DomainModelReview.md` | تصمیم‌های DDD، aggregateها و bounded contextها. |
| `docs/10-architecture/ScalabilityRisks.md` | ریسک‌های performance، concurrency و data growth. |
| `docs/08-ui-ux/AdminPanelRequirements.md` | نیازمندی‌های دقیق admin workflows و manual intervention. |
| `docs/03-requirements/BusinessRules.md` | نسخه انسانی و رسمی rules، جدا از استخراج کد. |
| `docs/14-roadmap/ProductRoadmap.md` | roadmap محصولی با معیار success. |
| `docs/14-roadmap/RiskRegister.md` | risk register زنده با owner و mitigation. |
| `docs/14-roadmap/KPIAndAnalytics.md` | KPIها، event analytics، funnel و finance metrics. |
| `docs/11-security-privacy/TrustAndSafetyPolicy.md` | سیاست safety، moderation، sanction و incident response. |
| `docs/12-configuration-devops/OperationalRunbooks.md` | runbook برای payment failure، event cancel، queue failure. |
| `docs/13-testing/ConcurrencyAndFinanceTestPlan.md` | سناریوهای تست پرریسک مالی/هم‌زمانی. |

## 15. خروجی نهایی قابل استفاده برای تیم توسعه

چک‌لیست فوری برای تیم محصول:

- [ ] تعریف دقیق مدل درآمدی و merchant of record.
- [ ] تصویب سیاست کنسلی، refund، no-show و event cancellation.
- [ ] تعریف وعده gender balance و حد تحمل آن.
- [ ] تعیین معیارهای تایید organizer و event.
- [ ] تعریف KPIهای launch و dashboard مدیریت.
- [ ] تعیین playbook برای incident، complaint و dispute.

چک‌لیست فوری برای تیم فنی:

- [ ] طراحی و پیاده‌سازی `EventRegistration` مستقل از `EventTicket`.
- [ ] اضافه‌کردن lifecycle رسمی event و registration.
- [ ] اضافه‌کردن capacity snapshot و optimistic locking/rowversion.
- [ ] پیاده‌سازی idempotency برای mutationهای مالی.
- [ ] اضافه‌کردن refund/cancellation entity و audit اجباری.
- [ ] اضافه‌کردن waitlist و promotion job.
- [ ] اضافه‌کردن notification delivery worker و provider تولید.
- [ ] اضافه‌کردن payment reconciliation و webhook verification.
- [ ] نوشتن تست‌های concurrency، payment، refund، cancellation و role matrix.
- [ ] ساخت admin runbook و UI برای manual intervention امن.

چک‌لیست فوری برای عملیات:

- [ ] تعریف SLA برای support، refund، receipt review و moderation.
- [ ] تعریف monitoring برای queue lag، failed payment، failed notification و refund backlog.
- [ ] تعریف گزارش مالی روزانه و mismatch handling.
- [ ] تعریف فرآیند لغو اضطراری event و اطلاع‌رسانی گروهی.
- [ ] تعریف retention policy برای chat، support attachment، audit و finance data.

جمع‌بندی نهایی: Randevoo ایده و اسکلت فنی خوبی دارد، اما برای MVP واقعی باید از «موجودیت‌های زیاد» به «قراردادهای عملیاتی دقیق» برسد. تا وقتی registration، capacity، gender balance، cancellation، refund، attendance، notification و reconciliation state machineهای روشن و تست‌شده نداشته باشند، رشد کاربر به جای موفقیت، فقط سرعت تولید خطا را بالا می‌برد.
