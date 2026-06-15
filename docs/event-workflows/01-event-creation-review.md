# 01 - ثبت و بررسی رویداد

رویداد اول همیشه پیش‌نویس است. ذخیره معمولی فقط پیش‌نویس را نگه می‌دارد؛ ارسال برای مدیر رویداد را وارد صف بررسی می‌کند.

```mermaid
flowchart TD
    A["برگزارکننده رویداد جدید می‌سازد"] --> B["Draft<br/>پیش‌نویس"]
    B -->|ذخیره پیش‌نویس| B
    B -->|ثبت و ارسال برای مدیر| C["PendingReview<br/>در انتظار بررسی مدیر"]
    C -->|رد توسط مدیر + توضیح| D["Draft<br/>نیازمند اصلاح"]
    D -->|ویرایش و ارسال دوباره| C
    C -->|تایید مدیر| E["Approved<br/>تایید شده"]
    E --> F["SaleStatus = Closed<br/>فروش هنوز بسته است"]
```

وضعیت‌های درگیر: `EventApprovalStatus`, `EventSaleStatus`

لاگ‌ها: `EventDraftSaved`, `EventSubmittedForReview`, `EventApproved`, `EventReturnedToDraft`
