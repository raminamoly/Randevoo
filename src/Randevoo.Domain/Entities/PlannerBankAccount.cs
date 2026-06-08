using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Events;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class PlannerBankAccount : BaseEntity, IAggregateRoot
{
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string CurrencyCode { get; private set; } = "IRR";
    public PlannerPayoutMethod PayoutMethod { get; private set; } = PlannerPayoutMethod.IranianBankCard;
    public string AccountHolderName { get; private set; } = string.Empty;
    public string? Country { get; private set; }
    public string? CardNumber { get; private set; }
    public string? Iban { get; private set; }
    public string? BankName { get; private set; }
    public string? AccountNumber { get; private set; }
    public string? SwiftCode { get; private set; }
    public string? AccountIdentifier { get; private set; }
    public string? PublicPaymentInstructions { get; private set; }
    public bool IsActive { get; private set; }

    private PlannerBankAccount() { }

    public PlannerBankAccount(User user, string cardNumber, string iban, string bankName, bool isActive = true)
        : this(
            user,
            "IRR",
            PlannerPayoutMethod.IranianBankCard,
            user.Profile?.DisplayName ?? user.MobileNumber,
            "ایران",
            cardNumber,
            iban,
            bankName,
            null,
            null,
            null,
            null,
            isActive)
    {
    }

    public PlannerBankAccount(
        User user,
        string currencyCode,
        PlannerPayoutMethod payoutMethod,
        string accountHolderName,
        string? country,
        string? cardNumber,
        string? iban,
        string? bankName,
        string? accountNumber,
        string? swiftCode,
        string? accountIdentifier,
        string? publicPaymentInstructions,
        bool isActive = true)
    {
        User = GuardAgainst.Object.Null(user, nameof(user));
        UserId = user.Id;
        SetDetails(currencyCode, payoutMethod, accountHolderName, country, cardNumber, iban, bankName, accountNumber, swiftCode, accountIdentifier, publicPaymentInstructions);
        IsActive = isActive;
        AddDomainEvent(new EntityCreatedEvent<PlannerBankAccount>(this));
    }

    public void Update(string cardNumber, string iban, string bankName, bool isActive)
    {
        Update("IRR", PlannerPayoutMethod.IranianBankCard, AccountHolderName, "ایران", cardNumber, iban, bankName, AccountNumber, SwiftCode, AccountIdentifier, PublicPaymentInstructions, isActive);
    }

    public void Update(
        string currencyCode,
        PlannerPayoutMethod payoutMethod,
        string accountHolderName,
        string? country,
        string? cardNumber,
        string? iban,
        string? bankName,
        string? accountNumber,
        string? swiftCode,
        string? accountIdentifier,
        string? publicPaymentInstructions,
        bool isActive)
    {
        SetDetails(currencyCode, payoutMethod, accountHolderName, country, cardNumber, iban, bankName, accountNumber, swiftCode, accountIdentifier, publicPaymentInstructions);
        IsActive = isActive;
        UpdateTimestamp();
    }

    public void Activate()
    {
        IsActive = true;
        UpdateTimestamp();
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdateTimestamp();
    }

    private void SetDetails(
        string currencyCode,
        PlannerPayoutMethod payoutMethod,
        string accountHolderName,
        string? country,
        string? cardNumber,
        string? iban,
        string? bankName,
        string? accountNumber,
        string? swiftCode,
        string? accountIdentifier,
        string? publicPaymentInstructions)
    {
        CurrencyCode = CurrencyLookup.NormalizeCode(string.IsNullOrWhiteSpace(currencyCode) ? "IRR" : currencyCode);
        PayoutMethod = GuardAgainst.Number.AgainstInvalidEnum<PlannerPayoutMethod>((int)payoutMethod, nameof(payoutMethod));
        AccountHolderName = GuardAgainst.String.InvalidLength(accountHolderName.Trim(), nameof(accountHolderName), 2, 120);
        Country = NormalizeOptional(country, nameof(country), 2, 80);
        CardNumber = NormalizeOptionalCardNumber(cardNumber);
        Iban = NormalizeOptionalIban(iban);
        BankName = NormalizeOptional(bankName, nameof(bankName), 2, 80);
        AccountNumber = NormalizeOptional(accountNumber, nameof(accountNumber), 3, 80);
        SwiftCode = NormalizeOptional(swiftCode, nameof(swiftCode), 6, 20)?.ToUpperInvariant();
        AccountIdentifier = NormalizeOptional(accountIdentifier, nameof(accountIdentifier), 3, 160);
        PublicPaymentInstructions = NormalizeOptional(publicPaymentInstructions, nameof(publicPaymentInstructions), 10, 1200);
        ValidateCurrencySpecificFields();
    }

    private void ValidateCurrencySpecificFields()
    {
        if (CurrencyCode == "IRR")
        {
            if (string.IsNullOrWhiteSpace(CardNumber) || string.IsNullOrWhiteSpace(Iban) || string.IsNullOrWhiteSpace(BankName))
                throw new BusinessRuleViolationException("Invalid IRR payout account", "IRR payout account requires card number, IBAN, and bank name.");
            return;
        }

        if (string.IsNullOrWhiteSpace(AccountIdentifier)
            && string.IsNullOrWhiteSpace(Iban)
            && string.IsNullOrWhiteSpace(SwiftCode)
            && string.IsNullOrWhiteSpace(PublicPaymentInstructions))
        {
            throw new BusinessRuleViolationException("Invalid payout account", "Foreign currency payout account requires bank/IBAN/SWIFT, account identifier, or payment instructions.");
        }
    }

    private static string? NormalizeOptional(string? value, string parameterName, int minLength, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return GuardAgainst.String.InvalidLength(value.Trim(), parameterName, minLength, maxLength);
    }

    private static string? NormalizeOptionalCardNumber(string? cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return null;

        return NormalizeCardNumber(cardNumber);
    }

    private static string? NormalizeOptionalIban(string? iban)
    {
        if (string.IsNullOrWhiteSpace(iban))
            return null;

        return NormalizeIban(iban);
    }

    private static string NormalizeCardNumber(string cardNumber)
    {
        var normalized = cardNumber.Replace(" ", string.Empty).Replace("-", string.Empty).Trim();
        return GuardAgainst.String.InvalidLength(normalized, nameof(cardNumber), 12, 19);
    }

    private static string NormalizeIban(string iban)
    {
        var normalized = iban.Replace(" ", string.Empty).Replace("-", string.Empty).Trim().ToUpperInvariant();
        return GuardAgainst.String.InvalidLength(normalized, nameof(iban), 18, 34);
    }
}
