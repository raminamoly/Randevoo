# Solution Structure

## Purpose
List solution projects, purposes, and detected dependencies.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `Randevoo.sln`

| Project | Target framework | Project references | NuGet packages | Source |
| --- | --- | --- | --- | --- |
| Randevoo.AdminPanel | net10.0 | ../Randevoo.Application/Randevoo.Application.csproj<br>../Randevoo.Domain/Randevoo.Domain.csproj<br>../Randevoo.Infrastructure/Randevoo.Infrastructure.csproj | None | `src/Randevoo.AdminPanel/Randevoo.AdminPanel.csproj` |
| Randevoo.Application | net10.0 | ../Randevoo.Domain/Randevoo.Domain.csproj | MediatR 14.1.0<br>Microsoft.Extensions.Logging.Abstractions 10.0.0 | `src/Randevoo.Application/Randevoo.Application.csproj` |
| Randevoo.Domain | net10.0 | None | None | `src/Randevoo.Domain/Randevoo.Domain.csproj` |
| Randevoo.Infrastructure | net10.0 | ../Randevoo.Application/Randevoo.Application.csproj<br>../Randevoo.Domain/Randevoo.Domain.csproj | Microsoft.EntityFrameworkCore 10.0.8<br>Microsoft.EntityFrameworkCore.Design 10.0.8<br>Microsoft.EntityFrameworkCore.SqlServer 10.0.8<br>Microsoft.EntityFrameworkCore.Tools 10.0.8<br>Microsoft.Extensions.Hosting.Abstractions 10.0.0<br>System.IdentityModel.Tokens.Jwt 8.14.0 | `src/Randevoo.Infrastructure/Randevoo.Infrastructure.csproj` |
| Randevoo.WebApi | net10.0 | ../Randevoo.Application/Randevoo.Application.csproj<br>../Randevoo.Domain/Randevoo.Domain.csproj<br>../Randevoo.Infrastructure/Randevoo.Infrastructure.csproj | Microsoft.AspNetCore.Authentication.JwtBearer 10.0.8<br>Microsoft.EntityFrameworkCore.Design 10.0.8<br>Microsoft.AspNetCore.OpenApi 10.0.8<br>Scalar.AspNetCore 2.14.14<br>Serilog.AspNetCore 10.0.0<br>Serilog.Sinks.File 7.0.0<br>Serilog.Sinks.Seq 9.1.0 | `src/Randevoo.WebApi/Randevoo.WebApi.csproj` |
| Randevoo.Tests.Integration | net10.0 | ../../src/Randevoo.WebApi/Randevoo.WebApi.csproj<br>../../src/Randevoo.Infrastructure/Randevoo.Infrastructure.csproj<br>../../src/Randevoo.Domain/Randevoo.Domain.csproj | Microsoft.AspNetCore.Mvc.Testing 10.0.8<br>Microsoft.EntityFrameworkCore.InMemory 10.0.8<br>Microsoft.NET.Test.Sdk 18.5.1<br>Testcontainers.MsSql 4.12.0<br>xunit 2.9.3<br>xunit.runner.visualstudio 3.1.5 | `tests/Randevoo.Tests.Integration/Randevoo.Tests.Integration.csproj` |
| Randevoo.Tests.Unit | net10.0 | ../../src/Randevoo.Domain/Randevoo.Domain.csproj | Microsoft.EntityFrameworkCore 10.0.8<br>Microsoft.NET.Test.Sdk 18.5.1<br>FluentAssertions 8.10.0<br>xunit 2.9.3<br>xunit.runner.visualstudio 3.1.5 | `tests/Randevoo.Tests.Unit/Randevoo.Tests.Unit.csproj` |

## Architecture pattern
The project resembles Clean Architecture with DDD and CQRS/vertical-slice influences. Domain stays central, Application holds use cases, Infrastructure implements persistence/services, WebApi exposes HTTP, and AdminPanel consumes the API through client classes.

## Gaps or uncertainties
- Architecture boundaries should be continuously checked because AdminPanel references Domain/Application model concepts directly in some UI-facing models.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
