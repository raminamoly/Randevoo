---
applyTo: "src/**/*.cs"
---

# Repository Pattern Standards

## Repository Design
- Repositories persist aggregates
- Keep repositories persistence-focused only
- Avoid business logic in repositories
- Prefer aggregate-oriented repositories

## Queries
- Use specification/query objects for complex filtering
- Avoid exposing IQueryable outside repositories
- Project directly to DTOs when appropriate
- Avoid over-fetching

## EF Core
- Use repositories with DbContext internally
- Keep transaction handling explicit
- Avoid generic repositories when they reduce clarity

## Unit Of Work
- Use Unit Of Work for transactional consistency
- Save changes once per business operation
