# Security Review Skill

## Goal
Review code for security vulnerabilities and unsafe practices.

## Checklist

### Input Validation
- Are all external inputs validated?
- Is FluentValidation used?
- Are dangerous payloads sanitized?

### Authentication / Authorization
- Are endpoints protected?
- Is ownership validation present?
- Are role checks enforced?

### Secrets
- Are secrets externalized?
- Are tokens/passwords excluded from logs?

### Database
- Are queries parameterized?
- Any SQL injection risks?
- Any overexposed data?

### Logging
- Are secrets excluded from logs?
- Are exceptions logged safely?

### APIs
- Are proper HTTP status codes returned?
- Are internal details hidden?

## Rules
- Never trust client input
- Use least privilege
- Prefer deny-by-default
