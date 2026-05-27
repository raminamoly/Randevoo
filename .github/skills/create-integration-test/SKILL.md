# Create Integration Test Skill

## Goal
Create reliable integration tests for APIs and infrastructure.

## Workflow
1. Prepare isolated test environment
2. Configure test database/container
3. Seed required data
4. Execute real workflow
5. Assert business behavior
6. Clean up resources

## Rules
- Test real infrastructure when possible
- Keep tests deterministic
- Avoid shared mutable state
- Avoid flaky timing-based tests

## Checklist
- Database isolated
- Seed data minimal
- Assertions meaningful
- Cleanup implemented
