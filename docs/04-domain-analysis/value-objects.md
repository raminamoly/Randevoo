# Value Objects

## Purpose
Catalog detected value objects.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.Domain/ValueObjects/AgeRange.cs`
- `src/Randevoo.Domain/ValueObjects/BaseValueObject.cs`
- `src/Randevoo.Domain/ValueObjects/Coordinates.cs`
- `src/Randevoo.Domain/ValueObjects/Hight.cs`
- `src/Randevoo.Domain/ValueObjects/Location.cs`

| Value object | Fields | Source |
| --- | --- | --- |
| AgeRange | Min: int<br>Max: int | `src/Randevoo.Domain/ValueObjects/AgeRange.cs` |
| BaseValueObject | See source | `src/Randevoo.Domain/ValueObjects/BaseValueObject.cs` |
| Coordinates | Latitude: decimal<br>Longitude: decimal | `src/Randevoo.Domain/ValueObjects/Coordinates.cs` |
| Height | Centimeters: int | `src/Randevoo.Domain/ValueObjects/Hight.cs` |
| Location | Country: string<br>City: string<br>Region: string?<br>Coordinates: Coordinates | `src/Randevoo.Domain/ValueObjects/Location.cs` |

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
