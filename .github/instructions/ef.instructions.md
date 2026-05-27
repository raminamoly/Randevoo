---
applyTo: "src/**/*.cs"
---

# Entity Framework Core Standards

## Performance
- Avoid N+1 queries
- Use projection for readonly operations
- Use AsNoTracking for readonly queries
- Use pagination for large datasets
- Load only required columns

## Entity Configuration
- Use Fluent API for complex configuration
- Keep entity configuration in separate files
- Explicitly configure relationships and indexes

## DbContext
- Keep DbContext focused
- Avoid business logic inside DbContext
- Avoid exposing DbContext outside Infrastructure

## Transactions
- Keep transaction scopes minimal
- Handle concurrency explicitly where needed

## Migrations
- Review generated migrations before applying
- Keep migrations small and reversible
- Avoid destructive migrations without safeguards

## Query Design
- Prefer compiled queries for hot paths
- Avoid unnecessary Include chains
- Optimize frequently executed queries
