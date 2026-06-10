# Local Development

## Purpose
Document local setup commands.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `Randevoo.sln`
- `src/Randevoo.WebApi/Properties/launchSettings.json`
- `src/Randevoo.AdminPanel/Properties/launchSettings.json`

## Prerequisites
- .NET SDK matching project target framework(s).
- Database provider configured in appsettings/connection strings.

## Commands
```powershell
dotnet restore Randevoo.sln
dotnet build Randevoo.sln
dotnet test Randevoo.sln
dotnet run --project src/Randevoo.WebApi/Randevoo.WebApi.csproj
dotnet run --project src/Randevoo.AdminPanel/Randevoo.AdminPanel.csproj
```

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
