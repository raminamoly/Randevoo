# Package Inventory

## Purpose
Inventory NuGet packages from project files.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.AdminPanel/Randevoo.AdminPanel.csproj`
- `src/Randevoo.Application/Randevoo.Application.csproj`
- `src/Randevoo.Domain/Randevoo.Domain.csproj`
- `src/Randevoo.Infrastructure/Randevoo.Infrastructure.csproj`
- `src/Randevoo.WebApi/Randevoo.WebApi.csproj`
- `tests/Randevoo.Tests.Integration/Randevoo.Tests.Integration.csproj`
- `tests/Randevoo.Tests.Unit/Randevoo.Tests.Unit.csproj`

| Project | Packages |
| --- | --- |
| Randevoo.AdminPanel | None detected |
| Randevoo.Application | MediatR 14.1.0<br>Microsoft.Extensions.Logging.Abstractions 10.0.0 |
| Randevoo.Domain | None detected |
| Randevoo.Infrastructure | Microsoft.EntityFrameworkCore 10.0.8<br>Microsoft.EntityFrameworkCore.Design 10.0.8<br>Microsoft.EntityFrameworkCore.SqlServer 10.0.8<br>Microsoft.EntityFrameworkCore.Tools 10.0.8<br>Microsoft.Extensions.Hosting.Abstractions 10.0.0<br>System.IdentityModel.Tokens.Jwt 8.14.0 |
| Randevoo.WebApi | Microsoft.AspNetCore.Authentication.JwtBearer 10.0.8<br>Microsoft.EntityFrameworkCore.Design 10.0.8<br>Microsoft.AspNetCore.OpenApi 10.0.8<br>Scalar.AspNetCore 2.14.14<br>Serilog.AspNetCore 10.0.0<br>Serilog.Sinks.File 7.0.0<br>Serilog.Sinks.Seq 9.1.0 |
| Randevoo.Tests.Integration | Microsoft.AspNetCore.Mvc.Testing 10.0.8<br>Microsoft.EntityFrameworkCore.InMemory 10.0.8<br>Microsoft.NET.Test.Sdk 18.5.1<br>Testcontainers.MsSql 4.12.0<br>xunit 2.9.3<br>xunit.runner.visualstudio 3.1.5 |
| Randevoo.Tests.Unit | Microsoft.EntityFrameworkCore 10.0.8<br>Microsoft.NET.Test.Sdk 18.5.1<br>FluentAssertions 8.10.0<br>xunit 2.9.3<br>xunit.runner.visualstudio 3.1.5 |

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
