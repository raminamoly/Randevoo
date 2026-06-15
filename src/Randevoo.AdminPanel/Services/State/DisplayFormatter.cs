using System.Globalization;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.Domain.Enums;
using AdminEventOperationalStatus = Randevoo.AdminPanel.Models.Events.EventOperationalStatus;
using AdminEventReviewStatus = Randevoo.AdminPanel.Models.Events.EventReviewStatus;

namespace Randevoo.AdminPanel.Services.State;

public static class DisplayFormatter
{
    public static string Money(decimal value, bool useRtl)
        => ToPersianDigits($"{value:N0} ریال");

    public static string TicketMoney(decimal value, string? currencyCode, bool useRtl)
        => ToPersianDigits($"{value:N0} {CurrencyLabel(currencyCode)}");

    public static string CurrencyMoney(decimal value, string? currencyCode, bool useRtl)
        => TicketMoney(value, currencyCode, useRtl);

    public static string ReportingMoney(decimal value, bool useRtl)
        => ToPersianDigits($"{value:N0} ریال ایران");

    public static string CurrencyLabel(string? currencyCode)
    {
        var normalized = string.IsNullOrWhiteSpace(currencyCode) ? "IRR" : currencyCode.Trim().ToUpperInvariant();
        return normalized switch
        {
            "IRR" => "ریال ایران",
            "EUR" => "یورو",
            "USD" => "دلار آمریکا",
            "CAD" => "دلار کانادا",
            "GBP" => "پوند انگلیس",
            "AED" => "درهم امارات",
            "TRY" => "لیر ترکیه",
            _ => normalized
        };
    }

    public static string Count(int value) => ToPersianDigits(value.ToString(CultureInfo.InvariantCulture));

    public static string Number(int value) => ToPersianDigits(value.ToString(CultureInfo.InvariantCulture));

    public static string Number(decimal value) => ToPersianDigits(value.ToString("N0", CultureInfo.InvariantCulture));

    public static string Percent(decimal value) => ToPersianDigits($"{value:N0}٪");

    public static string Role(AdminRole role) => role switch
    {
        AdminRole.Admin => "مدیر",
        AdminRole.EventPlanner => "برگزارکننده",
        AdminRole.SupportTeam => "کارشناس پشتیبانی",
        _ => role.ToString()
    };

    public static string OperationalStatus(AdminEventOperationalStatus status) => status switch
    {
        AdminEventOperationalStatus.SaleClosed => "فروش بسته",
        AdminEventOperationalStatus.SaleOpen => "فروش باز",
        AdminEventOperationalStatus.Completed => "تمام شده",
        AdminEventOperationalStatus.Cancelled => "لغو شده",
        _ => status.ToString()
    };

    public static string ProfileStatus(AdminEventReviewStatus status) => status switch
    {
        AdminEventReviewStatus.NotSubmitted => "پیش‌نویس",
        AdminEventReviewStatus.PendingReview => "در انتظار بررسی مدیر",
        AdminEventReviewStatus.Approved => "تایید شده",
        AdminEventReviewStatus.Rejected => "بازگشت برای اصلاح",
        _ => status.ToString()
    };

    public static string ProfileStatus(EventApprovalStatus status) => ApprovalStatus(status);

    public static string ReviewStatus(AdminEventReviewStatus status) => status switch
    {
        AdminEventReviewStatus.NotSubmitted => "پیش‌نویس",
        AdminEventReviewStatus.PendingReview => "در انتظار بررسی",
        AdminEventReviewStatus.Approved => "تایید شده",
        AdminEventReviewStatus.Rejected => "بازگشت برای اصلاح",
        _ => status.ToString()
    };

    public static string ApprovalStatus(EventApprovalStatus status) => status switch
    {
        EventApprovalStatus.Draft => "پیش‌نویس",
        EventApprovalStatus.PendingReview => "در انتظار بررسی مدیر",
        EventApprovalStatus.Approved => "تایید شده",
        _ => status.ToString()
    };

    public static string SaleStatus(EventSaleStatus status) => status switch
    {
        EventSaleStatus.Closed => "فروش بسته",
        EventSaleStatus.Open => "فروش باز",
        _ => status.ToString()
    };

    public static string LifecycleStatus(EventLifecycleStatus status) => status switch
    {
        EventLifecycleStatus.Active => "فعال",
        EventLifecycleStatus.Cancelled => "لغو شده",
        EventLifecycleStatus.Completed => "تمام شده",
        _ => status.ToString()
    };

    public static string OperationalStatusClass(AdminEventOperationalStatus status) => status switch
    {
        AdminEventOperationalStatus.SaleOpen => "status-approved",
        AdminEventOperationalStatus.Completed => "status-closed",
        AdminEventOperationalStatus.Cancelled => "status-cancelled",
        _ => "status-draft"
    };

    public static string ReviewStatusClass(AdminEventReviewStatus status) => status switch
    {
        AdminEventReviewStatus.PendingReview => "status-pending",
        AdminEventReviewStatus.Approved => "status-approved",
        AdminEventReviewStatus.Rejected => "status-rejected",
        _ => "status-draft"
    };

    public static string ApprovalStatusClass(EventApprovalStatus status) => status switch
    {
        EventApprovalStatus.PendingReview => "status-pending",
        EventApprovalStatus.Approved => "status-approved",
        _ => "status-draft"
    };

    public static string TransactionType(BalanceTransactionType type) => type switch
    {
        BalanceTransactionType.AdminAdjustment => "اصلاح مدیر",
        BalanceTransactionType.TicketPurchase => "خرید بلیت",
        BalanceTransactionType.TicketRefund => "بازگشت بلیت",
        BalanceTransactionType.EventPlannerIncome => "درآمد برگزارکننده",
        BalanceTransactionType.PlatformCommission => "کمیسیون پلتفرم",
        BalanceTransactionType.EmergencyRemovalRefund => "بازگشت حذف اضطراری",
        BalanceTransactionType.PlannerWithdrawalPayout => "تسویه برگزارکننده",
        BalanceTransactionType.EventPlannerIncomeReversal => "برگشت سهم برگزارکننده",
        BalanceTransactionType.EventSettlementCredit => "بستانکاری تسویه رویداد",
        BalanceTransactionType.EventSettlementReversal => "برگشت بستانکاری رویداد",
        BalanceTransactionType.PlatformCommissionRecognized => "شناسایی کمیسیون پلتفرم",
        BalanceTransactionType.ManualReceiptWalletCredit => "اعتبار کیف پول بابت رسید دستی",
        BalanceTransactionType.OrganizerManualReceiptLiability => "بدهی برگزارکننده بابت رسید دستی",
        _ => type.ToString()
    };

    public static string PaymentCollectionMethod(EventPaymentCollectionMethod method) => method switch
    {
        EventPaymentCollectionMethod.PlatformGateway => "پرداخت آنلاین از طریق درگاه پلتفرم",
        EventPaymentCollectionMethod.PlatformManualTransfer => "واریز به حساب پلتفرم و تایید توسط پشتیبانی",
        EventPaymentCollectionMethod.OrganizerManualTransfer => "واریز مستقیم به حساب برگزارکننده و تایید توسط برگزارکننده",
        _ => method.ToString()
    };

    public static string PayoutMethod(string? method)
        => Enum.TryParse<PlannerPayoutMethod>(method, out var payoutMethod)
            ? PayoutMethod(payoutMethod)
            : method ?? "نامشخص";

    public static string PayoutMethod(PlannerPayoutMethod method) => method switch
    {
        PlannerPayoutMethod.IranianBankCard => "کارت/شبا ایران",
        PlannerPayoutMethod.BankTransfer => "انتقال بانکی",
        PlannerPayoutMethod.IbanSwift => "IBAN / SWIFT",
        PlannerPayoutMethod.PayPal => "PayPal",
        PlannerPayoutMethod.Wise => "Wise",
        PlannerPayoutMethod.StripeConnect => "Stripe Connect",
        PlannerPayoutMethod.Other => "سایر",
        _ => method.ToString()
    };

    public static string PaymentCollectionSettlementNote(EventPaymentCollectionMethod method) => method switch
    {
        EventPaymentCollectionMethod.OrganizerManualTransfer => "پول مستقیم به حساب برگزارکننده واریز می‌شود و کمیسیون پلتفرم به عنوان بدهی برگزارکننده ثبت خواهد شد.",
        EventPaymentCollectionMethod.PlatformManualTransfer => "پول به حساب پلتفرم واریز می‌شود؛ پس از تایید رسید توسط پشتیبانی، سهم برگزارکننده قابل برداشت خواهد بود.",
        _ => "پرداخت از طریق پلتفرم انجام می‌شود و سهم برگزارکننده پس از کسر کمیسیون قابل برداشت خواهد بود."
    };

    public static string Gender(Gender gender) => gender switch
    {
        Randevoo.Domain.Enums.Gender.Male => "آقا",
        Randevoo.Domain.Enums.Gender.Female => "خانم",
        _ => "ثبت نشده"
    };

    public static string WithdrawalStatus(PlannerWithdrawalRequestStatus status) => status switch
    {
        PlannerWithdrawalRequestStatus.Pending => "در انتظار تایید",
        PlannerWithdrawalRequestStatus.Confirmed => "تایید شده",
        PlannerWithdrawalRequestStatus.Rejected => "رد شده",
        _ => status.ToString()
    };

    public static string EventTypeLabel(string? eventTypeName)
        => string.IsNullOrWhiteSpace(eventTypeName) ? "ثبت نشده" : eventTypeName.Trim();

    public static string EventTypeLabel(EventType eventType) => eventType switch
    {
        EventType.SocialEvening => "گردهمایی اجتماعی",
        EventType.Dinner => "شام",
        EventType.Coffee => "کافه",
        EventType.Rooftop => "روف تاپ",
        EventType.Workshop => "کارگاه",
        EventType.Gallery => "گالری",
        _ => eventType.ToString()
    };

    public static string Bool(bool value) => value ? "بله" : "خیر";

    public static string EducationRestriction(EventEducationLevelRestriction restriction) => restriction switch
    {
        EventEducationLevelRestriction.WithoutLimit => "بدون محدودیت",
        EventEducationLevelRestriction.DiplomaOrHigher => "دیپلم به بالا",
        EventEducationLevelRestriction.BachelorOrHigher => "لیسانس به بالا",
        EventEducationLevelRestriction.MasterOrHigher => "فوق لیسانس به بالا",
        EventEducationLevelRestriction.ProfessionalDoctorateOrPhD => "دکترای حرفه ای / PHD / پزشک / دندان پزشک",
        _ => restriction.ToString()
    };

    public static string ToPersianDigits(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return value
            .Replace('0', '۰')
            .Replace('1', '۱')
            .Replace('2', '۲')
            .Replace('3', '۳')
            .Replace('4', '۴')
            .Replace('5', '۵')
            .Replace('6', '۶')
            .Replace('7', '۷')
            .Replace('8', '۸')
            .Replace('9', '۹');
    }
}
