# Enterprise Copilot Instructions

## General Engineering Standards
- Follow Clean Architecture and SOLID principles
- Prefer maintainability and consistency over clever implementations
- Reuse existing patterns before introducing new approaches
- Keep classes cohesive and methods focused
- Avoid duplicated logic and hidden side effects
- Write production-ready code only
- Prefer explicit behavior over magic abstractions
- Follow existing solution conventions and naming standards

## Architecture
- Domain layer must never depend on Infrastructure
- Application layer orchestrates use cases
- Infrastructure implements external concerns
- Presentation layer must remain thin
- Avoid leaking infrastructure details into upper layers
- Prefer vertical slice organization for features when appropriate

## Security
- Never hardcode secrets, tokens, or connection strings
- Validate all external input
- Use least privilege principle
- Avoid logging sensitive information
- Use parameterized queries only

## Reliability
- Fail fast on invalid input
- Handle transient failures explicitly
- Add retry policies only for transient operations
- Use cancellation tokens for async operations
- Prefer idempotent operations for distributed workflows

## Code Quality
- Prefer immutable models when possible
- Avoid static mutable state
- Prefer composition over inheritance
- Keep dependencies minimal and explicit
- Optimize only after measurement
