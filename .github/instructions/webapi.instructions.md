---
applyTo: "src/**/*.cs"
---

# Web API Standards

## API Design
- Follow RESTful conventions
- Use resource-oriented routing
- Use proper HTTP verbs
- Return correct status codes

## Controllers / Endpoints
- Keep controllers thin
- Delegate business logic to Application layer
- Use DTOs for requests/responses
- Avoid exposing entities directly

## Validation
- Validate all incoming requests
- Use FluentValidation
- Return standardized validation errors

## Error Handling
- Use centralized exception handling middleware
- Avoid leaking internal implementation details
- Return consistent problem details responses

## Security
- Validate authorization explicitly
- Never trust client input
- Validate ownership and access rules

## Performance
- Use pagination for large datasets
- Minimize payload size
- Support cancellation tokens
- Use caching when beneficial

## Versioning
- Use API versioning for public APIs
- Avoid breaking changes without versioning
