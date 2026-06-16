using Randevoo.Domain.Enums;

namespace Randevoo.Domain.Constants;

public static class OperationPermissionCatalog
{
    public static readonly IReadOnlyList<UserRole> AdminPanelRoles =
    [
        UserRole.Admin,
        UserRole.EventPlanner,
        UserRole.PlatformSupportTeam
    ];

    public static readonly IReadOnlyList<OperationPermissionDefinition> All =
    [
        Permission("dashboard", "داشبورد", "viewAdmin", "مشاهده داشبورد مدیر", "مشاهده داشبوردهای کلان مدیر.", "analytics", "داشبورد و تحلیل", "/Dashboard", null, "PageAccess", "Low", 10),
        Permission("dashboard", "داشبورد", "viewMy", "مشاهده داشبورد من", "مشاهده داشبورد عملیاتی نقش جاری.", "analytics", "داشبورد و تحلیل", "/Dashboard/My", null, "PageAccess", "Low", 20, planner: true),
        Permission("dashboard", "داشبورد", "viewEvents", "تحلیل رویدادها", "مشاهده گزارش‌های رویدادها.", "analytics", "داشبورد و تحلیل", "/Dashboard/Events", null, "PageAccess", "Low", 30),
        Permission("dashboard", "داشبورد", "viewSales", "تحلیل فروش", "مشاهده گزارش‌های فروش.", "analytics", "داشبورد و تحلیل", "/Dashboard/Sales", null, "PageAccess", "Medium", 40),
        Permission("dashboard", "داشبورد", "viewMoney", "تحلیل مالی", "مشاهده نمودارها و شاخص‌های مالی.", "analytics", "داشبورد و تحلیل", "/Dashboard/Money", null, "PageAccess", "Medium", 50),
        Permission("dashboard", "داشبورد", "viewUsers", "تحلیل کاربران", "مشاهده گزارش‌های کاربران.", "analytics", "داشبورد و تحلیل", "/Dashboard/Users", null, "PageAccess", "Low", 60),

        Permission("participants", "شرکت‌کنندگان", "list", "مشاهده فهرست شرکت‌کنندگان", "دیدن فهرست شرکت‌کنندگان؛ برای غیرمدیر فقط در زمینه رویداد مجاز است.", "people", "شرکت‌کنندگان", "/Participants", "OnGet", "PageAccess", "Medium", 100, planner: true, support: true),
        Permission("participants", "شرکت‌کنندگان", "viewDetails", "مشاهده جزئیات شرکت‌کننده", "باز کردن پروفایل و اطلاعات عملیاتی شرکت‌کننده.", "people", "شرکت‌کنندگان", "/UserProfiles/Details", "OnGet", "GridAction", "Medium", 110, planner: true, support: true),
        Permission("participants", "شرکت‌کنندگان", "viewContactInfo", "مشاهده اطلاعات تماس", "نمایش شماره موبایل و اطلاعات تماس شرکت‌کننده.", "people", "شرکت‌کنندگان", "/Participants", null, "SensitiveData", "High", 120, planner: true, support: true),
        Permission("participants", "شرکت‌کنندگان", "editProfile", "ویرایش پروفایل شرکت‌کننده", "ویرایش مستقیم اطلاعات پروفایل کاربر از پنل ادمین.", "people", "شرکت‌کنندگان", "/UserProfiles/AdminEdit", "OnPostSave", "SensitiveAction", "High", 130),
        Permission("participants", "شرکت‌کنندگان", "addProfileImage", "افزودن تصویر پروفایل", "افزودن تصویر به پروفایل شرکت‌کننده.", "people", "شرکت‌کنندگان", "/UserProfiles/AdminEdit", "OnPostAddImage", "FormSubmit", "High", 140),
        Permission("participants", "شرکت‌کنندگان", "removeProfileImage", "حذف تصویر پروفایل", "حذف تصویر از پروفایل شرکت‌کننده.", "people", "شرکت‌کنندگان", "/UserProfiles/AdminEdit", "OnPostRemoveImage", "SensitiveAction", "High", 150),
        Permission("participants", "شرکت‌کنندگان", "editInterests", "مدیریت علایق پروفایل", "افزودن یا حذف علایق پروفایل شرکت‌کننده.", "people", "شرکت‌کنندگان", "/UserProfiles/AdminEdit", "OnPostAddInterest/OnPostRemoveInterest", "FormSubmit", "Medium", 160),
        Permission("participants", "شرکت‌کنندگان", "sendSms", "ارسال پیامک فوری", "ارسال پیامک فوری به شرکت‌کننده.", "people", "شرکت‌کنندگان", "/UserProfiles/AdminEdit", "OnPostSendSms", "SensitiveAction", "High", 170),
        Permission("participants", "شرکت‌کنندگان", "resendProfileLink", "ارسال لینک تکمیل پروفایل", "ارسال یا ارسال مجدد دعوت تکمیل پروفایل.", "people", "شرکت‌کنندگان", "/Participants", null, "GridAction", "Medium", 180, planner: true, support: true),
        Permission("participants", "شرکت‌کنندگان", "changeStatus", "تغییر وضعیت شرکت‌کننده", "تغییر وضعیت شرکت‌کننده در رویداد.", "people", "شرکت‌کنندگان", "/Participants", null, "GridAction", "High", 190, support: true),
        Permission("participants", "شرکت‌کنندگان", "replaceParticipant", "جایگزینی شرکت‌کننده", "جایگزین کردن شرکت‌کننده یک بلیت.", "people", "شرکت‌کنندگان", "/Participants", null, "SensitiveAction", "High", 200),
        Permission("participants", "شرکت‌کنندگان", "emergencyRefund", "بازگشت اضطراری وجه", "حذف اضطراری شرکت‌کننده و بازگشت وجه.", "people", "شرکت‌کنندگان", "/Participants", "OnPostRefund", "SensitiveAction", "Critical", 210),
        Permission("participants", "شرکت‌کنندگان", "export", "خروجی گرفتن", "دریافت خروجی از فهرست شرکت‌کنندگان.", "people", "شرکت‌کنندگان", "/Participants", null, "Export", "High", 220),

        Permission("orders", "سفارش‌ها و خریداران", "viewList", "مشاهده فهرست سفارش‌ها", "دیدن فهرست خریداران و سفارش‌های بلیت.", "orders", "سفارش‌ها و خرید", "/Buyers", "OnGet", "PageAccess", "Medium", 300, planner: true, support: true),
        Permission("orders", "سفارش‌ها و خریداران", "view", "مشاهده سفارش", "مشاهده سفارش و تراکنش مرتبط با بلیت.", "orders", "سفارش‌ها و خرید", "/Buyers", null, "GridAction", "Medium", 310, planner: true, support: true),
        Permission("orders", "سفارش‌ها و خریداران", "viewPayments", "مشاهده پرداخت‌های سفارش", "مشاهده پرداخت‌ها و وضعیت مالی سفارش.", "orders", "سفارش‌ها و خرید", "/Finance/TicketTransactions", "OnGet", "SensitiveData", "High", 320),

        Permission("events", "رویدادها", "viewList", "مشاهده فهرست رویدادها", "دیدن فهرست رویدادهای سیستم.", "events", "رویدادها", "/Events", "OnGet", "PageAccess", "Low", 400),
        Permission("events", "رویدادها", "viewMy", "مشاهده رویدادهای من", "دیدن رویدادهای برگزارکننده جاری.", "events", "رویدادها", "/Events/My", "OnGet", "PageAccess", "Low", 410, planner: true),
        Permission("events", "رویدادها", "viewActive", "مشاهده رویدادهای فعال", "دیدن رویدادهای فعال و قابل فروش.", "events", "رویدادها", "/Events", "OnGet", "PageAccess", "Low", 420, planner: true, support: true),
        Permission("events", "رویدادها", "viewArchived", "مشاهده رویدادهای آرشیو", "دیدن رویدادهای پایان‌یافته یا لغوشده.", "events", "رویدادها", "/Events", "OnGet", "PageAccess", "Low", 430, planner: true),
        Permission("events", "رویدادها", "viewDetails", "مشاهده جزئیات رویداد", "باز کردن صفحه جزئیات رویداد.", "events", "رویدادها", "/Events/Details", "OnGet", "PageAccess", "Low", 440, planner: true, support: true),
        Permission("events", "رویدادها", "create", "ایجاد رویداد", "ایجاد رویداد جدید.", "events", "رویدادها", "/Events/Edit", "OnPost", "FormSubmit", "Medium", 450, planner: true),
        Permission("events", "رویدادها", "edit", "ویرایش رویداد", "ویرایش اطلاعات رویداد.", "events", "رویدادها", "/Events/Edit", "OnPost", "FormSubmit", "Medium", 460, planner: true),
        Permission("events", "رویدادها", "approve", "تایید رویداد", "تایید رویداد برای انتشار.", "events", "رویدادها", "/Events/Details", "OnPostApprove", "SensitiveAction", "High", 470),
        Permission("events", "رویدادها", "approveAndOpen", "تایید و باز کردن فروش", "تایید رویداد و باز کردن فروش بلیت.", "events", "رویدادها", "/Events/Details", "OnPostApproveAndOpen", "SensitiveAction", "High", 480),
        Permission("events", "رویدادها", "reject", "رد رویداد", "رد کردن رویداد و ثبت علت.", "events", "رویدادها", "/Events/Details", "OnPostReject", "SensitiveAction", "High", 490),
        Permission("events", "رویدادها", "openSale", "باز کردن فروش", "فعال کردن فروش بلیت رویداد.", "events", "رویدادها", "/Events/Details", "OnPostOpenSale", "SensitiveAction", "High", 500, planner: true),
        Permission("events", "رویدادها", "closeSale", "بستن فروش", "غیرفعال کردن فروش بلیت رویداد.", "events", "رویدادها", "/Events/Details", "OnPostCloseSale", "SensitiveAction", "High", 510, planner: true),
        Permission("events", "رویدادها", "cancel", "لغو رویداد", "لغو رویداد.", "events", "رویدادها", "/Events/Details", "OnPostCancel", "SensitiveAction", "Critical", 520, planner: true),
        Permission("events", "رویدادها", "manageFaqs", "مدیریت سوالات متداول", "ایجاد و ویرایش FAQ رویداد.", "events", "رویدادها", "/Events/Faqs", "OnPost", "FormSubmit", "Low", 530, planner: true),
        Permission("events", "رویدادها", "viewParticipants", "فهرست شرکت‌کنندگان رویداد", "ورود از رویداد به فهرست شرکت‌کنندگان.", "events", "رویدادها", "/Participants", "OnGet", "GridAction", "Medium", 540, planner: true, support: true),
        Permission("events", "رویدادها", "viewConversations", "مشاهده گفتگوهای رویداد", "مشاهده گفتگوهای بین شرکت‌کنندگان رویداد.", "events", "گفتگوهای رویداد", "/Events/Conversations", "OnGet", "SensitiveData", "High", 550),
        Permission("events", "رویدادها", "viewSurveyRatings", "مشاهده نظرسنجی رویداد", "مشاهده امتیازها و بازخوردهای رویداد.", "events", "نظرسنجی رویداد", "/Events/SurveyRatings", "OnGet", "PageAccess", "Medium", 560, planner: true),

        Permission("eventSms", "پیامک رویداد", "view", "مشاهده پیامک‌های رویداد", "دیدن درخواست‌ها و برنامه پیامکی رویداد.", "events", "پیامک رویداد", "/Events/Sms", "OnGet", "PageAccess", "Medium", 600, planner: true),
        Permission("eventSms", "پیامک رویداد", "createRequest", "ثبت درخواست پیامک", "ثبت پیامک جدید برای شرکت‌کنندگان رویداد.", "events", "پیامک رویداد", "/Events/Sms", "OnPostCreate", "FormSubmit", "Medium", 610, planner: true),
        Permission("eventSms", "پیامک رویداد", "approve", "تایید پیامک رویداد", "تایید ارسال پیامک درخواستی.", "events", "پیامک رویداد", "/Events/Sms", "OnPostApprove", "SensitiveAction", "High", 620),
        Permission("eventSms", "پیامک رویداد", "reject", "رد پیامک رویداد", "رد کردن درخواست ارسال پیامک.", "events", "پیامک رویداد", "/Events/Sms", "OnPostReject", "SensitiveAction", "High", 630),

        Permission("planners", "برگزارکنندگان", "viewList", "مشاهده فهرست برگزارکنندگان", "دیدن فهرست برگزارکنندگان.", "planners", "برگزارکنندگان", "/Planner", "OnGet", "PageAccess", "Low", 700),
        Permission("planners", "برگزارکنندگان", "viewDetails", "مشاهده جزئیات برگزارکننده", "مشاهده پروفایل عمومی و عملیاتی برگزارکننده.", "planners", "برگزارکنندگان", "/Planner/Details", "OnGet", "PageAccess", "Medium", 710, planner: true),
        Permission("planners", "برگزارکنندگان", "viewApprovals", "مشاهده درخواست‌های تایید", "دیدن صف تغییرات پروفایل برگزارکنندگان.", "planners", "برگزارکنندگان", "/Planner/Approvals", "OnGet", "PageAccess", "Medium", 720),
        Permission("planners", "برگزارکنندگان", "reviewProfile", "بررسی پروفایل برگزارکننده", "باز کردن صفحه بررسی تغییرات برگزارکننده.", "planners", "برگزارکنندگان", "/Planner/Review", "OnGet", "PageAccess", "Medium", 730),
        Permission("planners", "برگزارکنندگان", "approveProfile", "تایید پروفایل برگزارکننده", "تایید و انتشار تغییرات پروفایل برگزارکننده.", "planners", "برگزارکنندگان", "/Planner/Review", "OnPostApprove", "SensitiveAction", "High", 740),
        Permission("planners", "برگزارکنندگان", "rejectProfile", "رد پروفایل برگزارکننده", "رد کردن تغییرات پروفایل برگزارکننده.", "planners", "برگزارکنندگان", "/Planner/Review", "OnPostReject", "SensitiveAction", "High", 750),
        Permission("planners", "برگزارکنندگان", "editOwnProfile", "ویرایش پروفایل برگزارکننده", "ویرایش پروفایل برگزارکننده توسط خودش یا مدیر.", "planners", "برگزارکنندگان", "/Planner/Profile", "OnPost", "FormSubmit", "Medium", 760, planner: true),
        Permission("plannerBankAccounts", "حساب‌های بانکی برگزارکننده", "view", "مشاهده حساب‌های بانکی", "مشاهده حساب‌های بانکی برگزارکننده.", "planners", "حساب بانکی برگزارکننده", "/Planner/BankAccounts", "OnGet", "SensitiveData", "High", 800, planner: true),
        Permission("plannerBankAccounts", "حساب‌های بانکی برگزارکننده", "save", "ثبت یا ویرایش حساب بانکی", "ایجاد یا ویرایش حساب بانکی برگزارکننده.", "planners", "حساب بانکی برگزارکننده", "/Planner/BankAccounts", "OnPostSave", "SensitiveAction", "High", 810, planner: true),
        Permission("plannerBankAccounts", "حساب‌های بانکی برگزارکننده", "toggle", "فعال/غیرفعال کردن حساب بانکی", "تغییر وضعیت حساب بانکی برگزارکننده.", "planners", "حساب بانکی برگزارکننده", "/Planner/BankAccounts", "OnPostToggle", "SensitiveAction", "High", 820, planner: true),

        Permission("finance", "مالی", "viewDashboard", "مشاهده داشبورد مالی", "مشاهده داشبورد مالی مدیر.", "finance", "مالی", "/Finance", "OnGet", "PageAccess", "High", 900),
        Permission("finance", "مالی", "viewMy", "مشاهده مالی من", "مشاهده داشبورد مالی برگزارکننده.", "finance", "مالی", "/Finance/My", "OnGet", "PageAccess", "High", 910, planner: true),
        Permission("finance", "مالی", "requestWithdrawal", "ثبت درخواست تسویه", "ثبت درخواست تسویه توسط برگزارکننده.", "finance", "تسویه", "/Finance/My", "OnPostRequestWithdrawal", "SensitiveAction", "High", 920, planner: true),
        Permission("finance", "مالی", "viewTicketTransactions", "مشاهده تراکنش‌های بلیت", "مشاهده تراکنش‌های خرید و بازگشت وجه.", "finance", "تراکنش‌ها", "/Finance/TicketTransactions", "OnGet", "SensitiveData", "High", 930),
        Permission("finance", "مالی", "viewUserFinance", "مشاهده مالی کاربر", "مشاهده مانده و پرداخت‌های یک کاربر.", "finance", "تراکنش‌ها", "/Finance/User", "OnGet", "SensitiveData", "High", 940),
        Permission("withdrawals", "تسویه‌ها", "view", "مشاهده درخواست‌های تسویه", "مشاهده فهرست درخواست‌های تسویه.", "finance", "تسویه‌ها", "/Finance/Withdrawals", "OnGet", "PageAccess", "High", 950),
        Permission("withdrawals", "تسویه‌ها", "confirm", "تایید پرداخت تسویه", "تایید درخواست تسویه و ثبت پرداخت.", "finance", "تسویه‌ها", "/Finance/Withdrawals", "OnPostConfirmWithdrawal", "SensitiveAction", "Critical", 960),
        Permission("withdrawals", "تسویه‌ها", "reject", "رد درخواست تسویه", "رد کردن درخواست تسویه.", "finance", "تسویه‌ها", "/Finance/Withdrawals", "OnPostRejectWithdrawal", "SensitiveAction", "Critical", 970),
        Permission("paymentReceipts", "رسیدهای پرداخت", "view", "مشاهده رسیدهای پرداخت", "مشاهده رسیدهای پرداخت دستی.", "finance", "رسیدهای پرداخت", "/Finance/PaymentReceipts", "OnGet", "PageAccess", "High", 980, support: true),
        Permission("paymentReceipts", "رسیدهای پرداخت", "approve", "تایید رسید پرداخت", "تایید رسید پرداخت دستی.", "finance", "رسیدهای پرداخت", "/Finance/PaymentReceipts", "OnPostApprove", "SensitiveAction", "Critical", 990, support: true),
        Permission("paymentReceipts", "رسیدهای پرداخت", "reject", "رد رسید پرداخت", "رد کردن رسید پرداخت دستی.", "finance", "رسیدهای پرداخت", "/Finance/PaymentReceipts", "OnPostReject", "SensitiveAction", "Critical", 1000, support: true),
        Permission("paymentReceipts", "رسیدهای پرداخت", "viewReceived", "مشاهده رسیدهای دریافتی برگزارکننده", "مشاهده رسیدهایی که برای برگزارکننده ثبت شده‌اند.", "finance", "رسیدهای پرداخت", "/Finance/ReceivedReceipts", "OnGet", "PageAccess", "High", 1010, planner: true),
        Permission("paymentReceipts", "رسیدهای پرداخت", "reviewReceived", "بررسی رسید دریافتی", "تایید یا رد رسید دریافتی برگزارکننده.", "finance", "رسیدهای پرداخت", "/Finance/ReceivedReceipts", "OnPostApprove/OnPostReject", "SensitiveAction", "Critical", 1020, planner: true),
        Permission("refunds", "بازگشت وجه", "view", "مشاهده درخواست‌های بازگشت وجه", "مشاهده صف درخواست‌های Refund بلیت.", "finance", "بازگشت وجه", "/Finance/RefundRequests", "OnGet", "SensitiveData", "High", 1030, planner: true, support: true),
        Permission("refunds", "بازگشت وجه", "approve", "تایید بازگشت وجه", "تایید Refund و اعتبار کیف پول خریدار.", "finance", "بازگشت وجه", "/Finance/RefundRequests", "OnPostApprove", "SensitiveAction", "Critical", 1040, support: true),
        Permission("refunds", "بازگشت وجه", "reject", "رد بازگشت وجه", "رد درخواست Refund بلیت.", "finance", "بازگشت وجه", "/Finance/RefundRequests", "OnPostReject", "SensitiveAction", "High", 1050, support: true),

        Permission("specialOperations", "عملیات ویژه", "view", "مشاهده عملیات ویژه", "ورود به کنسول عملیات ویژه پشتیبانی.", "operations", "عملیات ویژه", "/SpecialOperations", "OnGet", "PageAccess", "Critical", 1060, support: true),
        Permission("specialOperations", "عملیات ویژه", "cancelTicketRefundToWallet", "کنسل بلیت و برگشت به کیف پول", "کنسل کردن یک بلیت مشخص و اعتبار مبلغ به کیف پول خریدار.", "operations", "عملیات ویژه", "/SpecialOperations", "OnPostPreviewCancelTicket/OnPostExecuteCancelTicket", "SensitiveAction", "Critical", 1070, support: true),
        Permission("specialOperations", "عملیات ویژه", "manualIssueTicketWithWalletDebit", "صدور دستی بلیت با کسر کیف پول", "صدور بلیت برای کاربر مشخص و کسر مبلغ از کیف پول همان کاربر.", "operations", "عملیات ویژه", "/SpecialOperations", "OnPostPreviewManualIssue/OnPostExecuteManualIssue", "SensitiveAction", "Critical", 1080, support: true),
        Permission("specialOperations", "عملیات ویژه", "manualWalletCredit", "شارژ دستی کیف پول", "افزایش دستی موجودی کیف پول با ثبت لاگ عملیات ویژه.", "operations", "عملیات ویژه", "/SpecialOperations", "OnPostPreviewWalletCredit/OnPostExecuteWalletCredit", "SensitiveAction", "Critical", 1090),
        Permission("specialOperations", "عملیات ویژه", "manualWalletDebit", "کسر دستی کیف پول", "کاهش دستی موجودی کیف پول با ثبت لاگ عملیات ویژه.", "operations", "عملیات ویژه", "/SpecialOperations", "OnPostPreviewWalletDebit/OnPostExecuteWalletDebit", "SensitiveAction", "Critical", 1100),
        Permission("specialOperations", "عملیات ویژه", "viewAuditLog", "مشاهده تاریخچه عملیات ویژه", "مشاهده تاریخچه و نتیجه عملیات ویژه.", "operations", "عملیات ویژه", "/SpecialOperations", "OnGet", "SensitiveData", "Critical", 1110, support: true),
        Permission("specialOperations", "عملیات ویژه", "userReportsView", "مشاهده کاربران ریپورت‌شده", "مشاهده فهرست و جزئیات گزارش‌های کاربران.", "operations", "عملیات ویژه", "/SpecialOperations", "OnGet", "SensitiveData", "High", 1120, support: true),
        Permission("specialOperations", "عملیات ویژه", "userReportsReview", "بررسی گزارش کاربر", "تغییر وضعیت گزارش‌ها و ثبت یادداشت بررسی.", "operations", "عملیات ویژه", "/SpecialOperations", "OnPostReviewUserReport", "SensitiveAction", "High", 1130, support: true),
        Permission("specialOperations", "عملیات ویژه", "userReportsRestrictTicketPurchase", "محدود کردن خرید بلیت", "بستن امکان خرید بلیت برای کاربر ریپورت‌شده و ارسال پیام.", "operations", "عملیات ویژه", "/SpecialOperations", "OnPostPreviewRestrictTicketPurchase/OnPostExecuteRestrictTicketPurchase", "SensitiveAction", "Critical", 1140, support: true),
        Permission("specialOperations", "عملیات ویژه", "userReportsRemoveRestriction", "برداشتن محدودیت خرید", "غیرفعال کردن محدودیت خرید بلیت کاربر.", "operations", "عملیات ویژه", "/SpecialOperations", "OnPostRemoveTicketPurchaseRestriction", "SensitiveAction", "Critical", 1150, support: true),
        Permission("specialOperations", "عملیات ویژه", "userReportsSendWarning", "ارسال هشدار به کاربر", "ارسال پیام هشدار مدیریتی به کاربر ریپورت‌شده.", "operations", "عملیات ویژه", "/SpecialOperations", "OnPostSendUserReportWarning", "SensitiveAction", "High", 1160, support: true),
        Permission("specialOperations", "عملیات ویژه", "userReportsSendNotification", "ارسال نوتیفیکیشن به کاربر", "ارسال نوتیفیکیشن داخلی دلخواه به کاربر ریپورت‌شده بدون تغییر وضعیت حساب.", "operations", "عملیات ویژه", "/SpecialOperations", "OnPostSendUserReportNotification", "SensitiveAction", "High", 1170, support: true),
        Permission("specialOperations", "عملیات ویژه", "userReportsDeactivateUser", "غیرفعال کردن کاربر ریپورت‌شده", "غیرفعال کردن حساب کاربر ریپورت‌شده و ثبت نوتیفیکیشن و لاگ عملیات ویژه.", "operations", "عملیات ویژه", "/SpecialOperations", "OnPostDeactivateReportedUser", "SensitiveAction", "Critical", 1180, support: true),

        Permission("notifications", "پیام‌ها", "viewInbox", "مشاهده صندوق پیام‌ها", "مشاهده اعلان‌ها و پیام‌های داخلی.", "support", "پیام‌ها", "/Notifications", "OnGet", "PageAccess", "Low", 1120, planner: true, support: true),
        Permission("notifications", "پیام‌ها", "create", "ارسال پیام", "ثبت پیام داخلی یا پیامک برای گیرندگان مجاز.", "support", "پیام‌ها", "/Notifications/Create", "OnPost", "SensitiveAction", "High", 1130, planner: true, support: true),
        Permission("notifications", "پیام‌ها", "approve", "تایید پیام", "تایید پیام‌های نیازمند بررسی قبل از ارسال.", "support", "پیام‌ها", "/Notifications/Approvals", "OnPostApprove", "SensitiveAction", "High", 1140, support: true),
        Permission("notifications", "پیام‌ها", "reject", "رد پیام", "رد پیام‌های نیازمند بررسی.", "support", "پیام‌ها", "/Notifications/Approvals", "OnPostReject", "SensitiveAction", "High", 1150, support: true),

        Permission("support", "پشتیبانی", "viewDashboard", "مشاهده داشبورد پشتیبانی", "مشاهده داشبورد پشتیبانی.", "support", "پشتیبانی", "/Support", "OnGet", "PageAccess", "Low", 1100, support: true),
        Permission("support", "پشتیبانی", "viewSystemTickets", "مشاهده تیکت‌های سیستم", "مشاهده صف تیکت‌های قابل رسیدگی.", "support", "پشتیبانی", "/Support/Tickets", "OnGet", "PageAccess", "Medium", 1110, support: true),
        Permission("support", "پشتیبانی", "viewMyTickets", "مشاهده تیکت‌های من", "مشاهده تیکت‌های ثبت‌شده توسط کاربر پنل.", "support", "پشتیبانی", "/Support/My", "OnGet", "PageAccess", "Low", 1120, planner: true, support: true),
        Permission("support", "پشتیبانی", "viewReceived", "مشاهده تیکت‌های دریافتی", "مشاهده تیکت‌هایی که برای برگزارکننده ارسال شده‌اند.", "support", "پشتیبانی", "/Support/Received", "OnGet", "PageAccess", "Medium", 1130, planner: true),
        Permission("support", "پشتیبانی", "create", "ایجاد تیکت", "ثبت تیکت پشتیبانی جدید.", "support", "پشتیبانی", "/Support/Create", "OnPost", "FormSubmit", "Low", 1140, planner: true, support: true),
        Permission("support", "پشتیبانی", "viewDetails", "مشاهده جزئیات تیکت", "باز کردن جزئیات تیکت.", "support", "پشتیبانی", "/Support/Details", "OnGet", "PageAccess", "Medium", 1150, planner: true, support: true),
        Permission("support", "پشتیبانی", "reply", "ثبت پاسخ تیکت", "ارسال پاسخ و پیوست برای تیکت.", "support", "پشتیبانی", "/Support/Details", "OnPostReply", "FormSubmit", "Medium", 1160, planner: true, support: true),
        Permission("support", "پشتیبانی", "changeStatus", "تغییر وضعیت تیکت", "تغییر وضعیت تیکت پشتیبانی.", "support", "پشتیبانی", "/Support/Details", "OnPostStatus", "SensitiveAction", "Medium", 1170, support: true),
        Permission("support", "پشتیبانی", "reassign", "ارجاع تیکت", "ارجاع تیکت به پشتیبان دیگر.", "support", "پشتیبانی", "/Support/Details", "OnPostReassign", "SensitiveAction", "High", 1180, support: true),
        Permission("support", "پشتیبانی", "viewContext", "مشاهده زمینه مالی/رویداد", "مشاهده زمینه مالی، رویدادها و تیکت‌های قبلی در جزئیات تیکت.", "support", "پشتیبانی", "/Support/Details", null, "SensitiveData", "High", 1190, support: true),

        Permission("discountCodes", "کدهای تخفیف", "view", "مشاهده کدهای تخفیف", "مشاهده فهرست کدهای تخفیف.", "baseData", "کدهای تخفیف", "/DiscountCodes", "OnGet", "PageAccess", "Medium", 1300),
        Permission("discountCodes", "کدهای تخفیف", "save", "ثبت یا ویرایش کد تخفیف", "ایجاد یا ویرایش کد تخفیف.", "baseData", "کدهای تخفیف", "/DiscountCodes", "OnPost", "FormSubmit", "High", 1310),
        Permission("discountCodes", "کدهای تخفیف", "toggle", "فعال/غیرفعال کردن کد تخفیف", "تغییر وضعیت کد تخفیف.", "baseData", "کدهای تخفیف", "/DiscountCodes", "OnPostToggle", "SensitiveAction", "High", 1320),
        Permission("discountCodes", "کدهای تخفیف", "viewUsage", "مشاهده مصرف کد تخفیف", "مشاهده سفارش‌ها و استفاده‌های یک کد تخفیف.", "baseData", "کدهای تخفیف", "/DiscountCodes", "OnGet", "SensitiveData", "Medium", 1330),
        Permission("eventTypes", "نوع رویداد", "view", "مشاهده نوع‌های رویداد", "مشاهده اطلاعات پایه نوع رویداد.", "baseData", "اطلاعات پایه", "/EventTypes", "OnGet", "PageAccess", "Low", 1340),
        Permission("eventTypes", "نوع رویداد", "save", "ثبت یا ویرایش نوع رویداد", "ایجاد یا ویرایش نوع رویداد.", "baseData", "اطلاعات پایه", "/EventTypes", "OnPost", "FormSubmit", "Medium", 1350),
        Permission("eventTypes", "نوع رویداد", "delete", "حذف نوع رویداد", "حذف نوع رویداد استفاده‌نشده.", "baseData", "اطلاعات پایه", "/EventTypes", "OnPostDelete", "SensitiveAction", "High", 1360),
        Permission("tags", "تگ‌ها", "view", "مشاهده تگ‌ها", "مشاهده اطلاعات پایه تگ‌ها.", "baseData", "اطلاعات پایه", "/Tags", "OnGet", "PageAccess", "Low", 1370),
        Permission("tags", "تگ‌ها", "save", "ثبت یا ویرایش تگ", "ایجاد یا ویرایش تگ.", "baseData", "اطلاعات پایه", "/Tags", "OnPost", "FormSubmit", "Medium", 1380),
        Permission("tags", "تگ‌ها", "delete", "حذف تگ", "حذف تگ استفاده‌نشده.", "baseData", "اطلاعات پایه", "/Tags", "OnPostDelete", "SensitiveAction", "High", 1390),
        Permission("locations", "کشورها و شهرها", "view", "مشاهده کشورها و شهرها", "مشاهده اطلاعات پایه کشورها و شهرها.", "baseData", "اطلاعات پایه", "/Settings/Locations", "OnGet", "PageAccess", "Medium", 1400),
        Permission("locations", "کشورها و شهرها", "manageCountries", "مدیریت کشورها", "ایجاد، ویرایش و فعال یا غیرفعال کردن کشورها.", "baseData", "اطلاعات پایه", "/Settings/Locations", "OnPostSaveCountry/OnPostToggleCountry", "SensitiveAction", "High", 1410),
        Permission("locations", "کشورها و شهرها", "manageCities", "مدیریت شهرها", "ایجاد، ویرایش و فعال یا غیرفعال کردن شهرها.", "baseData", "اطلاعات پایه", "/Settings/Locations", "OnPostSaveCity/OnPostToggleCity", "SensitiveAction", "High", 1420),

        Permission("users", "کاربران پنل", "viewList", "مشاهده کاربران پنل", "مشاهده فهرست کاربران و نقش‌ها.", "users", "کاربران و پروفایل‌ها", "/Users", "OnGet", "PageAccess", "High", 1500),
        Permission("users", "کاربران پنل", "save", "ثبت یا ویرایش کاربر پنل", "ایجاد یا ویرایش کاربر و نقش او.", "users", "کاربران و پروفایل‌ها", "/Users", "OnPost", "SensitiveAction", "Critical", 1510),
        Permission("userProfiles", "پروفایل کاربران", "viewList", "مشاهده فهرست پروفایل‌ها", "مشاهده فهرست پروفایل شرکت‌کنندگان.", "users", "کاربران و پروفایل‌ها", "/UserProfiles", "OnGet", "PageAccess", "Medium", 1520),
        Permission("userProfiles", "پروفایل کاربران", "viewDetails", "مشاهده جزئیات پروفایل", "مشاهده جزئیات کامل پروفایل.", "users", "کاربران و پروفایل‌ها", "/UserProfiles/Details", "OnGet", "SensitiveData", "Medium", 1530, planner: true, support: true),
        Permission("userProfiles", "پروفایل کاربران", "adminEdit", "ویرایش ادمین پروفایل", "باز کردن فرم ویرایش ادمین پروفایل.", "users", "کاربران و پروفایل‌ها", "/UserProfiles/AdminEdit", "OnGet", "SensitiveAction", "High", 1540),

        Permission("logs", "لاگ‌ها", "viewAuditLogs", "مشاهده لاگ فعالیت", "مشاهده لاگ‌های فعالیت پنل.", "logs", "لاگ‌ها", "/Logs", "OnGet", "SensitiveData", "High", 1700),
        Permission("logs", "لاگ‌ها", "viewSmsQueue", "مشاهده صف پیامک", "مشاهده وضعیت صف پیامک.", "logs", "لاگ‌ها", "/Logs/SmsQueue", "OnGet", "SensitiveData", "High", 1710),
        Permission("settings", "تنظیمات", "view", "مشاهده تنظیمات", "مشاهده صفحه تنظیمات و اطلاعات پایه.", "settings", "تنظیمات", "/Settings", "OnGet", "PageAccess", "High", 1800),
        Permission("settings", "تنظیمات", "manageCurrencies", "مدیریت نرخ ارز", "ثبت و ویرایش نرخ‌های تبدیل ارز.", "settings", "تنظیمات", "/Settings", "OnPostSaveRate", "SensitiveAction", "Critical", 1810),
        Permission("operationPermissions", "دسترسی عملیات", "view", "مشاهده مدیریت دسترسی عملیات", "مشاهده صفحه مدیریت دسترسی عملیات.", "settings", "دسترسی عملیات", "/Settings/OperationPermissions", "OnGet", "PageAccess", "Critical", 1900),
        Permission("operationPermissions", "دسترسی عملیات", "manage", "مدیریت دسترسی عملیات", "تغییر سطح دسترسی نقش‌ها و کاربران.", "settings", "دسترسی عملیات", "/Settings/OperationPermissions", "OnPostSaveRolePermissions/OnPostSaveOverride/OnPostDeleteOverride", "SensitiveAction", "Critical", 1910)
    ];

    public static OperationPermissionDefinition? Find(string entity, string action)
    {
        var normalizedEntity = Normalize(entity);
        var normalizedAction = Normalize(action);
        return All.FirstOrDefault(item =>
            Normalize(item.Entity) == normalizedEntity
            && Normalize(item.Action) == normalizedAction);
    }

    private static OperationPermissionDefinition Permission(
        string entity,
        string entityLabel,
        string action,
        string label,
        string? description,
        string groupKey,
        string groupLabel,
        string? pagePath,
        string? handlerName,
        string uiSurface,
        string riskLevel,
        int displayOrder,
        bool admin = true,
        bool planner = false,
        bool support = false,
        bool isSystemAction = true)
    {
        return new OperationPermissionDefinition(
            Normalize(entity),
            entityLabel,
            Normalize(action),
            label,
            description,
            Normalize(groupKey),
            groupLabel,
            pagePath,
            handlerName,
            uiSurface,
            riskLevel,
            displayOrder,
            admin,
            planner,
            support,
            isSystemAction);
    }

    private static string Normalize(string value) => (value ?? string.Empty).Trim();
}
