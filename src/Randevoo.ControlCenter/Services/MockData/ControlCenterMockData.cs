using MudBlazor;
using Randevoo.ControlCenter.Models.Common;
using Randevoo.ControlCenter.Models.EventPlanners;
using Randevoo.ControlCenter.Models.Events;

namespace Randevoo.ControlCenter.Services.MockData;

public sealed class ControlCenterMockData
{
    public IReadOnlyList<DashboardMetric> AdminMetrics { get; } =
    [
        new("Active events", "18", "5 selling this week", Icons.Material.Filled.EventAvailable, "#2563eb"),
        new("Ticket revenue", "$84.2k", "Mock financial snapshot", Icons.Material.Filled.Payments, "#059669"),
        new("Planner reviews", "12", "4 waiting for verification", Icons.Material.Filled.ManageSearch, "#7c3aed"),
        new("Moderation queue", "7", "2 high-priority reports", Icons.Material.Filled.Policy, "#dc2626")
    ];

    public IReadOnlyList<DashboardMetric> PlannerMetrics { get; } =
    [
        new("My events", "6", "3 upcoming", Icons.Material.Filled.EventNote, "#2563eb"),
        new("Tickets sold", "214", "Across upcoming events", Icons.Material.Filled.ConfirmationNumber, "#059669"),
        new("Available balance", "$12.6k", "Mock settlement data", Icons.Material.Filled.AccountBalanceWallet, "#0891b2"),
        new("Survey score", "4.7", "Average recent rating", Icons.Material.Filled.QueryStats, "#7c3aed")
    ];

    public IReadOnlyList<EventSummary> Events { get; } =
    [
        new(Guid.Parse("19733337-708f-45af-9c4d-01574dd8ac10"), "Tehran rooftop social", "Nava Events", "Tehran", DateTimeOffset.Now.AddDays(3), EventStatus.OnSale, 80, 54, 10800m),
        new(Guid.Parse("8db13d6a-22d2-4d1e-b5ee-9d7b15c09ad2"), "Shiraz gallery evening", "Orange Room", "Shiraz", DateTimeOffset.Now.AddDays(8), EventStatus.Scheduled, 42, 19, 3420m),
        new(Guid.Parse("b9376c3c-0ad2-4767-b074-3367a2cd9917"), "Isfahan coffee circle", "Nava Events", "Isfahan", DateTimeOffset.Now.AddDays(15), EventStatus.Draft, 36, 0, 0m),
        new(Guid.Parse("ef5d0283-38d9-4b4c-9704-2d6f65d912b1"), "Tehran founders dinner", "North Star Gatherings", "Tehran", DateTimeOffset.Now.AddDays(-2), EventStatus.Closed, 28, 28, 9800m)
    ];

    public IReadOnlyList<EventPlannerSummary> EventPlanners { get; } =
    [
        new(Guid.Parse("256d6de8-e275-4211-a903-34048ca9151d"), "Sara M.", "Nava Events", "Tehran", 12600m, 3, true),
        new(Guid.Parse("749de263-8bdb-482c-bcf1-f4c0e61548c0"), "Arman K.", "Orange Room", "Shiraz", 4800m, 1, false),
        new(Guid.Parse("67bf3ed1-ad2b-4ff9-80cd-f599a35f7537"), "Leila R.", "North Star Gatherings", "Tehran", 9100m, 2, true)
    ];
}
