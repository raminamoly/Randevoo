using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Events;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.ValueObjects;

namespace Randevoo.Domain.Entities;

public class User : BaseEntity, IAggregateRoot
{
    public string MobileNumber { get; private set; } = null!;
    public string? Email { get; private set; }
    public bool IsEmailConfirmed { get; private set; }
    public string? PendingEmail { get; private set; }
    public string? MobileLoginCodeHash { get; private set; }
    public DateTime? MobileLoginCodeExpiresAt { get; private set; }
    public DateTime? MobileLoginCodeRequestWindowStartedAt { get; private set; }
    public int MobileLoginCodeRequestCount { get; private set; }
    public int MobileLoginFailedAttemptCount { get; private set; }
    public DateTime? MobileLoginLockedUntil { get; private set; }
    public string? EmailConfirmationTokenHash { get; private set; }
    public DateTime? EmailConfirmationTokenExpiresAt { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public UserProfile? Profile { get; private set; }

    private User() { }

    public User(string mobileNumber)
    {
        MobileNumber = NormalizeMobileNumber(mobileNumber);
        Role = UserRole.EndUser;
        IsActive = true;

        AddDomainEvent(new EntityCreatedEvent<User>(this));
    }

    public void StartMobileLogin(string codeHash, DateTime nowUtc, DateTime expiresAtUtc)
    {
        if (MobileLoginLockedUntil > nowUtc)
            throw new BusinessRuleViolationException("Login temporarily locked", "Too many incorrect login attempts. Try again later");

        if (MobileLoginCodeRequestWindowStartedAt == null || MobileLoginCodeRequestWindowStartedAt <= nowUtc.AddMinutes(-15))
        {
            MobileLoginCodeRequestWindowStartedAt = nowUtc;
            MobileLoginCodeRequestCount = 0;
        }

        if (MobileLoginCodeRequestCount >= 3)
            throw new BusinessRuleViolationException("Too many login code requests", "Please wait before requesting another login code");

        MobileLoginCodeRequestCount++;
        MobileLoginCodeHash = GuardAgainst.String.NullOrWhiteSpace(codeHash, nameof(codeHash));
        MobileLoginCodeExpiresAt = expiresAtUtc;
        UpdateTimestamp();
    }

    public void CompleteMobileLogin(string codeHash, DateTime nowUtc)
    {
        if (MobileLoginLockedUntil > nowUtc)
            throw new BusinessRuleViolationException("Login temporarily locked", "Too many incorrect login attempts. Try again later");

        if (MobileLoginCodeHash == null || MobileLoginCodeExpiresAt == null)
            throw new BusinessRuleViolationException("Invalid login code", "No active login code exists for this user");

        if (MobileLoginCodeExpiresAt <= nowUtc)
            throw new BusinessRuleViolationException("Expired login code", "The mobile login code has expired");

        if (!string.Equals(MobileLoginCodeHash, codeHash, StringComparison.Ordinal))
        {
            MobileLoginFailedAttemptCount++;
            if (MobileLoginFailedAttemptCount >= 5)
                MobileLoginLockedUntil = nowUtc.AddMinutes(15);
            UpdateTimestamp();
            throw new BusinessRuleViolationException("Invalid login code", "The mobile login code is incorrect");
        }

        MobileLoginCodeHash = null;
        MobileLoginCodeExpiresAt = null;
        MobileLoginFailedAttemptCount = 0;
        MobileLoginLockedUntil = null;
        UpdateTimestamp();
    }

    public void StartEmailConfirmation(string email, string tokenHash, DateTime expiresAtUtc)
    {
        PendingEmail = GuardAgainst.String.InvalidEmail(email, nameof(email)).Trim().ToLowerInvariant();
        EmailConfirmationTokenHash = GuardAgainst.String.NullOrWhiteSpace(tokenHash, nameof(tokenHash));
        EmailConfirmationTokenExpiresAt = expiresAtUtc;
        IsEmailConfirmed = false;
        UpdateTimestamp();

        AddDomainEvent(new EntityUpdatedEvent<User>(this, nameof(PendingEmail), Email ?? string.Empty, PendingEmail));
    }

    public void ConfirmEmail(string tokenHash, DateTime nowUtc)
    {
        if (PendingEmail == null || EmailConfirmationTokenHash == null || EmailConfirmationTokenExpiresAt == null)
            throw new BusinessRuleViolationException("Invalid email confirmation", "No active email confirmation request exists");

        if (EmailConfirmationTokenExpiresAt <= nowUtc)
            throw new BusinessRuleViolationException("Expired email confirmation", "The email confirmation link has expired");

        if (!string.Equals(EmailConfirmationTokenHash, tokenHash, StringComparison.Ordinal))
            throw new BusinessRuleViolationException("Invalid email confirmation", "The email confirmation token is incorrect");

        var oldEmail = Email;
        Email = PendingEmail;
        IsEmailConfirmed = true;
        PendingEmail = null;
        EmailConfirmationTokenHash = null;
        EmailConfirmationTokenExpiresAt = null;
        UpdateTimestamp();

        AddDomainEvent(new EntityUpdatedEvent<User>(this, nameof(Email), oldEmail ?? string.Empty, Email));
    }

    public void CreateProfile(
        string displayName,
        DateOnly dateOfBirth,
        Gender gender,
        Location location,
        Height? height = null)
    {
        GuardAgainst.Entity.AlreadyExists(Profile, nameof(UserProfile), $"UserId: {Id}");

        Profile = new UserProfile(this, displayName, dateOfBirth, gender, location, height);
    }

    public void Deactivate()
    {
        var oldIsActive = IsActive;
        IsActive = false;
        UpdateTimestamp();
        AddDomainEvent(new EntityUpdatedEvent<User>(this, nameof(IsActive), oldIsActive, IsActive));
    }

    public void ChangeUserRole(UserRole userRole)
    {
        var oldUserRole = Role;
        Role = userRole;
        UpdateTimestamp();
        AddDomainEvent(new EntityUpdatedEvent<User>(this, nameof(Role), oldUserRole, userRole));
    }

    public void BecomeEventPlanner()
    {
        if (Role == UserRole.Admin)
            return;

        ChangeUserRole(UserRole.EventPlanner);
    }

    private static string NormalizeMobileNumber(string mobileNumber)
    {
        var normalized = GuardAgainst.String.NullOrWhiteSpace(mobileNumber, nameof(mobileNumber)).Trim();
        if (normalized.Length < 8 || normalized.Length > 20)
            throw new BusinessRuleViolationException("Invalid mobile number", "Mobile number must be between 8 and 20 characters");

        if (!normalized.All(c => char.IsDigit(c) || c == '+'))
            throw new BusinessRuleViolationException("Invalid mobile number", "Mobile number can contain only digits and an optional plus sign");

        if (normalized.Count(c => c == '+') > 1 || (normalized.Contains('+') && !normalized.StartsWith("+", StringComparison.Ordinal)))
            throw new BusinessRuleViolationException("Invalid mobile number", "Plus sign is only allowed at the beginning");

        return normalized;
    }
}
