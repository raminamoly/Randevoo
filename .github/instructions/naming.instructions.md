---
applyTo: "src/**/*.cs"
---

# Naming Standards

## General
- Use business/domain terminology consistently
- Prefer clarity over brevity
- Avoid vague names like Data, Manager, Helper, Utility

## Conventions
- PascalCase:
  - Classes
  - Methods
  - Properties
  - Enums
- camelCase:
  - Parameters
  - Local variables
- Prefix interfaces with I

## Suffixes
- Command
- Query
- Handler
- Repository
- Service
- Validator
- Request
- Response
- Event

## Async
- Async methods must end with Async

## Booleans
- Use positive naming:
  - IsEnabled
  - HasAccess
  - CanExecute
