# Optimize EF Core Query Skill

## Goal
Improve Entity Framework Core query performance.

## Workflow
1. Analyze generated SQL
2. Detect N+1 queries
3. Reduce Includes
4. Use projection
5. Add pagination
6. Add indexes if required
7. Benchmark query

## Rules
- Use AsNoTracking for readonly queries
- Load only required columns
- Avoid unnecessary materialization
- Prefer projection over entities
- Avoid client-side evaluation

## Checklist
- N+1 checked
- Projection used
- Pagination added
- Tracking disabled when possible
- Query benchmarked
