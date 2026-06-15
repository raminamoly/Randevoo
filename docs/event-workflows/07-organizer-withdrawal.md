# 07 - برداشت موجودی برگزارکننده

برداشت بعد از بستانکار شدن حساب برگزارکننده انجام می‌شود و مستقل از تسویه هر رویداد است.

```mermaid
flowchart TD
    A["برگزارکننده موجودی قابل برداشت دارد"] --> B["درخواست برداشت ثبت می‌کند"]
    B --> C["انتخاب حساب تاییدشده"]
    C --> D["بررسی مالی/مدیر"]
    D -->|رد| E["درخواست رد می‌شود<br/>موجودی باقی می‌ماند"]
    D -->|تایید پرداخت| F["ثبت Debit / Payout Transaction"]
    F --> G["وضعیت برداشت: Confirmed"]
    G --> H["لاگ پرداخت و شماره پیگیری"]
```

وضعیت‌های درگیر: `PlannerWithdrawalRequestStatus`, `BalanceTransactionType.PlannerWithdrawalPayout`

لاگ‌ها: `WithdrawalRequested`, `WithdrawalConfirmed`, `WithdrawalRejected`
