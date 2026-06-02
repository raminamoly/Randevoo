# Randevoo Control Center

Randevoo.ControlCenter is the first Blazor-based operations cockpit for Admin and EventPlanner users. It is intentionally separate from the dating participant experience and does not include EndUser UI.

## How to run

From the repository root:

```powershell
dotnet restore Randevoo.sln
dotnet run --project src/Randevoo.ControlCenter/Randevoo.ControlCenter.csproj
```

Then open the local URL printed by `dotnet run`.

## Render mode

The project is a .NET 10 Blazor Web App using Interactive Server render mode. MudBlazor provides the dashboard layout, navigation, tables, cards, dialogs, and form controls.

## Mock authentication

Authentication is mocked for this first version. The login flow has two steps:

1. Enter a mobile number.
2. Enter an SMS code and choose either Admin or EventPlanner.

The selected mock user is stored in a scoped `MockAuthState` service for the current Blazor circuit.

## Role-based navigation

The sidebar filters links by role. Admin users can access platform operations such as `/dashboard`, `/events`, moderation, participants, and event planners. EventPlanner users are routed to `/my/dashboard` and `/my/events`.

Routes use a local `ControlCenterAuthorizeAttribute`. Unauthenticated users are redirected to `/login`; users who open a page outside their mock role are sent to `/forbidden`.

## Future backend integration

The Control Center behaves like a client of `Randevoo.WebApi`. It does not reference `Randevoo.Infrastructure`, `Randevoo.Domain`, or `Randevoo.Application`, and it does not use DbContext, repositories, or direct database access.

Future backend work should replace the mock implementations in `Services/ApiClients` with typed HTTP clients that call `Randevoo.WebApi` endpoints and map responses into local Control Center DTOs.
