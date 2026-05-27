# Create CQRS Feature Skill

## Goal
Create a production-ready CQRS feature using Clean Architecture.

## Workflow
1. Create Command or Query
2. Create Validator
3. Create DTOs
4. Create Handler
5. Add Repository Abstractions
6. Add Logging
7. Add Unit Tests
8. Add Integration Tests
9. Add API Endpoint

## Rules
- Keep handlers focused
- Use MediatR
- Use FluentValidation
- Use Result<T> pattern
- Avoid business logic in controllers
- Use CancellationToken

## Checklist
- Validation added
- Logging added
- Async used
- Tests added
- DTOs separated
- Business rules inside Domain/Application
