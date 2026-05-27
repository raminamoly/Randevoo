---
applyTo: "tests/**/*.cs"
---

# Testing Standards

## Frameworks
- Use xUnit
- Use FluentAssertions
- Use Testcontainers for integration tests when appropriate

## Naming
- Use descriptive test names
- Prefer:
  - Method_Should_Behavior_When_Condition
- Example:
  - CreateUser_Should_ReturnValidationError_When_EmailExists

## Structure
- Follow Arrange / Act / Assert
- Keep tests isolated and deterministic
- Avoid hidden dependencies between tests

## Unit Tests
- Test business behavior, not implementation details
- Mock only external dependencies
- Prefer real domain objects over mocks
- Keep unit tests fast

## Integration Tests
- Test:
  - Database behavior
  - API endpoints
  - Infrastructure integrations
- Use realistic infrastructure where possible

## Assertions
- Prefer expressive assertions
- Assert meaningful outcomes only
- Avoid excessive assertions in one test

## Reliability
- Avoid flaky tests
- Avoid time-dependent tests without abstraction
- Use fixed/randomized test data carefully

## Coverage
- Prioritize critical business paths
- Prefer meaningful coverage over percentage metrics
