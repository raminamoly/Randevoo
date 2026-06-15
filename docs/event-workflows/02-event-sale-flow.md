# 02 - فروش رویداد

بعد از تایید مدیر، فروش می‌تواند توسط مدیر یا برگزارکننده باز و بسته شود. بستن فروش به معنی لغو رویداد نیست.

```mermaid
flowchart TD
    A["رویداد Approved"] --> B["SaleStatus = Closed"]
    B -->|باز کردن فروش توسط برگزارکننده یا مدیر| C["SaleStatus = Open"]
    C -->|خرید بلیت فعال است| C
    C -->|بستن فروش| B
    B -->|باز کردن دوباره، اگر زمان تمام نشده و لغو نشده| C
    C -->|زمان پایان می‌رسد| D["Completed<br/>فروش دیگر باز نمی‌شود"]
    B -->|زمان پایان می‌رسد| D
```

وضعیت‌های درگیر: `EventApprovalStatus.Approved`, `EventSaleStatus`, `EventLifecycleStatus`

لاگ‌ها: `EventSaleOpened`, `EventSaleClosed`, `EventCompleted`
