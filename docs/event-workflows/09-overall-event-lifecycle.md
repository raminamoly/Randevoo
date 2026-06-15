# 09 - نمای کلی چرخه رویداد

این نمودار نمای فشرده کل چرخه رویداد، فروش، اتمام، تسویه و برداشت است.

```mermaid
flowchart LR
    A["Draft"] --> B["PendingReview"]
    B -->|رد| A
    B -->|تایید| C["Approved"]
    C --> D["Sale Closed"]
    D <--> E["Sale Open"]
    E --> F["Completed"]
    D --> F
    C --> G["Cancelled"]
    D --> G
    E --> G
    F --> H["Settlement Request"]
    H --> I["Organizer Credit"]
    I --> J["Withdrawal"]
```

وضعیت‌های درگیر: `EventApprovalStatus`, `EventSaleStatus`, `EventLifecycleStatus`

لاگ‌ها: همه اکشن‌های `EventWorkflowActionType`
