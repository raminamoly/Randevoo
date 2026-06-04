using Randevoo.Domain.Common;
using Randevoo.Domain.Events;

namespace Randevoo.Domain.Entities;

public class PlannerBankAccount : BaseEntity, IAggregateRoot
{
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string CardNumber { get; private set; } = null!;
    public string Iban { get; private set; } = null!;
    public string BankName { get; private set; } = null!;
    public bool IsActive { get; private set; }

    private PlannerBankAccount() { }

    public PlannerBankAccount(User user, string cardNumber, string iban, string bankName, bool isActive = true)
    {
        User = GuardAgainst.Object.Null(user, nameof(user));
        UserId = user.Id;
        CardNumber = NormalizeCardNumber(cardNumber);
        Iban = NormalizeIban(iban);
        BankName = GuardAgainst.String.InvalidLength(bankName.Trim(), nameof(bankName), 2, 80);
        IsActive = isActive;
        AddDomainEvent(new EntityCreatedEvent<PlannerBankAccount>(this));
    }

    public void Update(string cardNumber, string iban, string bankName, bool isActive)
    {
        CardNumber = NormalizeCardNumber(cardNumber);
        Iban = NormalizeIban(iban);
        BankName = GuardAgainst.String.InvalidLength(bankName.Trim(), nameof(bankName), 2, 80);
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
