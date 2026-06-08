using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Events;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace Randevoo.Domain.Entities;

public class DatingEvent : BaseEntity, IAggregateRoot
{
    private readonly List<EventTicket> _tickets = new();
    private readonly List<EventTag> _eventTags = new();
    private readonly List<EventFaq> _faqs = new();
    private readonly List<EventDiscountCode> _discountCodes = new();

    public string Title { get; private set; } = null!;
    public Location Location { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public DateTime DateTimeStart { get; private set; }
    public DateTime DateTimeEnd { get; private set; }
    public long EventTypeId { get; private set; }
    public EventType EventType { get; private set; } = null!;
    public long EventModeId { get; private set; } = 2L;
    public EventModeLookup EventMode { get; private set; } = null!;
    public long? OnlineEventPlatformId { get; private set; }
    public OnlineEventPlatform? OnlineEventPlatform { get; private set; }
    public string? OnlineJoinUrl { get; private set; }
    public string? OnlineAccessInstructions { get; private set; }
    public long? CountryId { get; private set; }
    public Country? Country { get; private set; }
    public long? CityId { get; private set; }
    public City? City { get; private set; }
    public AgeRange AgeRangeForMale { get; private set; } = null!;
    public AgeRange AgeRangeForFemale { get; private set; } = null!;
    public bool IsOpenForSell { get; private set; }
    public bool IsCancelled { get; private set; }
    public EventReviewStatus ReviewStatus { get; private set; }
    public long EventPlannerUserId { get; private set; }
    public User EventPlannerUser { get; private set; } = null!;
    public decimal EventPlannerCommissionPercent { get; private set; }
    public int MaleCapacity { get; private set; }
    public int FemaleCapacity { get; private set; }
    public int NumberOfLikesAllowed { get; private set; }
    public decimal MaleTicketPrice { get; private set; }
    public string MaleTicketCurrencyCode { get; private set; } = "IRR";
    public decimal FemaleTicketPrice { get; private set; }
    public string FemaleTicketCurrencyCode { get; private set; } = "IRR";
    [NotMapped]
    public decimal TicketPrice => Math.Min(MaleTicketPrice, FemaleTicketPrice);
    public EventEducationLevelRestriction EducationLevelRestriction { get; private set; }
    public long? MinimumEducationLevelId { get; private set; }
    public EducationLevelLookup? MinimumEducationLevel { get; private set; }
    public string? EventImage1 { get; private set; }
    public string? EventImage2 { get; private set; }
    public string? EventImage3 { get; private set; }
    public string EventDescriptionHtml { get; private set; } = null!;
    public IReadOnlyList<string> Tags => _eventTags
        .Where(item => item.Tag is not null && item.Tag.IsActive)
        .Select(item => item.Tag.Name)
        .ToList()
        .AsReadOnly();
    public IReadOnlyList<EventTicket> Tickets => _tickets.AsReadOnly();
    public IReadOnlyList<EventTag> EventTags => _eventTags.AsReadOnly();
    public IReadOnlyList<EventFaq> Faqs => _faqs.AsReadOnly();
    public IReadOnlyList<EventDiscountCode> DiscountCodes => _discountCodes.AsReadOnly();
    [NotMapped]
    public EventOperationalStatus OperationalStatus => ResolveOperationalStatus(DateTime.UtcNow);

    private DatingEvent() { }

    public DatingEvent(
        User eventPlannerUser,
        string title,
        Location location,
        string address,
        DateTime dateTimeStart,
        DateTime dateTimeEnd,
        EventType eventType,
        AgeRange ageRangeForMale,
        AgeRange ageRangeForFemale,
        int maleCapacity,
        int femaleCapacity,
        int numberOfChatAllowed,
        decimal maleTicketPrice,
        decimal femaleTicketPrice,
        EventEducationLevelRestriction educationLevelRestriction,
        IReadOnlyCollection<string>? tags,
        string? eventImage1,
        string? eventImage2,
        string? eventImage3,
        string eventDescriptionHtml,
        decimal eventPlannerCommissionPercent = 10,
        string maleTicketCurrencyCode = "IRR",
        string femaleTicketCurrencyCode = "IRR")
    {
        EventPlannerUser = GuardAgainst.Object.Null(eventPlannerUser, nameof(eventPlannerUser));
        if (eventPlannerUser.Role != UserRole.EventPlanner && eventPlannerUser.Role != UserRole.Admin)
            throw new BusinessRuleViolationException("Invalid event planner", "Only event planners can own dating events");

        SetCoreDetails(title, location, address, dateTimeStart, dateTimeEnd, eventType, ageRangeForMale, ageRangeForFemale, maleCapacity, femaleCapacity, numberOfChatAllowed, maleTicketPrice, femaleTicketPrice, educationLevelRestriction, tags, eventImage1, eventImage2, eventImage3, eventDescriptionHtml, maleTicketCurrencyCode, femaleTicketCurrencyCode);
        SetCommissionPercent(eventPlannerCommissionPercent);
        IsOpenForSell = false;
        IsCancelled = false;
        ReviewStatus = EventReviewStatus.NotSubmitted;
        AddDomainEvent(new EntityCreatedEvent<DatingEvent>(this));
    }

    public decimal GetTicketPriceForGender(Gender gender) => gender switch
    {
        Gender.Male => MaleTicketPrice,
        Gender.Female => FemaleTicketPrice,
        _ => throw new BusinessRuleViolationException("Unsupported gender", "Ticket pricing is not available for this user gender.")
    };

    public string GetTicketCurrencyForGender(Gender gender) => gender switch
    {
        Gender.Male => MaleTicketCurrencyCode,
        Gender.Female => FemaleTicketCurrencyCode,
        _ => throw new BusinessRuleViolationException("Unsupported gender", "Ticket pricing is not available for this user gender.")
    };

    public EventTicket SellTicket(User buyer, UserProfile buyerProfile, decimal? finalPriceOverride = null, EventDiscountCode? discountCode = null)
    {
        if (!IsOpenForSell || IsCancelled)
            throw new BusinessRuleViolationException("Event is not open", "Tickets cannot be sold for this event");

        GuardAgainst.Object.Null(buyer, nameof(buyer));
        GuardAgainst.Object.Null(buyerProfile, nameof(buyerProfile));

        if (buyer.Id != buyerProfile.UserId)
            throw new BusinessRuleViolationException("Invalid profile", "Buyer profile does not belong to buyer");

        var capacity = buyerProfile.Gender == Gender.Male ? MaleCapacity : FemaleCapacity;
        var sold = _tickets.Count(t => !t.IsRefunded && t.Gender == buyerProfile.Gender);
        if (sold >= capacity)
            throw new BusinessRuleViolationException("Capacity full", $"{buyerProfile.Gender} capacity is full");

        var range = buyerProfile.Gender == Gender.Male ? AgeRangeForMale : AgeRangeForFemale;
        if (!range.IsWithinRange(buyerProfile.Age))
            throw new BusinessRuleViolationException("Age out of range", "User age is not allowed for this event");

        if (!MeetsEducationRestriction(buyerProfile))
            throw new BusinessRuleViolationException("Education level not eligible", "User education level does not meet this event's minimum requirement");

        var basePrice = GetTicketPriceForGender(buyerProfile.Gender);
        var finalPrice = finalPriceOverride.HasValue
            ? GuardAgainst.Number.OutOfRange(finalPriceOverride.Value, nameof(finalPriceOverride), 0.01m, basePrice)
            : basePrice;

        var ticket = new EventTicket(this, buyer, buyerProfile.Gender, basePrice, finalPrice, GetTicketCurrencyForGender(buyerProfile.Gender), discountCode);
        _tickets.Add(ticket);
        UpdateTimestamp();
        return ticket;
    }

    public void ChangeAddressLocation(Location location, string address)
    {
        Location = GuardAgainst.Object.Null(location, nameof(location));
        Address = GuardAgainst.String.InvalidLength(address, nameof(address), 5, 300);
        UpdateTimestamp();
    }

    public void UpdateDetails(
        string title,
        Location location,
        string address,
        DateTime dateTimeStart,
        DateTime dateTimeEnd,
        EventType eventType,
        AgeRange ageRangeForMale,
        AgeRange ageRangeForFemale,
        int maleCapacity,
        int femaleCapacity,
        int numberOfChatAllowed,
        decimal maleTicketPrice,
        decimal femaleTicketPrice,
        EventEducationLevelRestriction educationLevelRestriction,
        IReadOnlyCollection<string>? tags,
        string? eventImage1,
        string? eventImage2,
        string? eventImage3,
        string eventDescriptionHtml,
        string maleTicketCurrencyCode = "IRR",
        string femaleTicketCurrencyCode = "IRR")
    {
        if (IsCancelled)
            throw new BusinessRuleViolationException("Event cancelled", "Cancelled events cannot be edited");

        SetCoreDetails(
            title,
            location,
            address,
            dateTimeStart,
            dateTimeEnd,
            eventType,
            ageRangeForMale,
            ageRangeForFemale,
            maleCapacity,
            femaleCapacity,
            numberOfChatAllowed,
            maleTicketPrice,
            femaleTicketPrice,
            educationLevelRestriction,
            tags,
            eventImage1,
            eventImage2,
            eventImage3,
            eventDescriptionHtml,
            maleTicketCurrencyCode,
            femaleTicketCurrencyCode);

        SubmitForReview();
        UpdateTimestamp();
    }

    public void ReassignPlanner(User eventPlannerUser)
    {
        EventPlannerUser = GuardAgainst.Object.Null(eventPlannerUser, nameof(eventPlannerUser));
        if (eventPlannerUser.Role != UserRole.EventPlanner && eventPlannerUser.Role != UserRole.Admin)
            throw new BusinessRuleViolationException("Invalid event planner", "Only event planners can own dating events");

        EventPlannerUserId = eventPlannerUser.Id;
        UpdateTimestamp();
    }

    public void OpenForSell()
    {
        if (IsCancelled)
            throw new BusinessRuleViolationException("Event cancelled", "Cancelled events cannot be opened");

        if (ReviewStatus != EventReviewStatus.Approved)
            throw new BusinessRuleViolationException("Event is not approved", "Only admin approved events can be opened for sale");

        if (DateTimeEnd <= DateTime.UtcNow)
            throw new BusinessRuleViolationException("Event closed", "Closed events cannot be opened for sale");

        IsOpenForSell = true;
        UpdateTimestamp();
    }

    public void CloseForSell()
    {
        IsOpenForSell = false;
        UpdateTimestamp();
    }

    public IReadOnlyList<EventTicket> Cancel()
    {
        IsCancelled = true;
        IsOpenForSell = false;
        foreach (var ticket in _tickets.Where(t => !t.IsRefunded))
            ticket.MarkRefunded();
        UpdateTimestamp();
        return _tickets;
    }

    public void SubmitForReview()
    {
        if (IsCancelled)
            throw new BusinessRuleViolationException("Event cancelled", "Cancelled events cannot be submitted for review");

        ReviewStatus = EventReviewStatus.PendingReview;
        IsOpenForSell = false;
        UpdateTimestamp();
    }

    public void ApproveByAdmin()
    {
        if (IsCancelled)
            throw new BusinessRuleViolationException("Event cancelled", "Cancelled events cannot be approved");

        ReviewStatus = EventReviewStatus.Approved;
        UpdateTimestamp();
    }

    public void RejectByAdmin()
    {
        if (IsCancelled)
            throw new BusinessRuleViolationException("Event cancelled", "Cancelled events cannot be rejected");

        ReviewStatus = EventReviewStatus.Rejected;
        IsOpenForSell = false;
        UpdateTimestamp();
    }

    public EventOperationalStatus ResolveOperationalStatus(DateTime nowUtc)
    {
        if (IsCancelled)
            return EventOperationalStatus.Cancelled;

        if (DateTimeEnd <= nowUtc)
            return EventOperationalStatus.Closed;

        if (IsOpenForSell)
            return EventOperationalStatus.Selling;

        return EventOperationalStatus.Draft;
    }

    public void SetCommissionPercent(decimal percent)
    {
        EventPlannerCommissionPercent = GuardAgainst.Number.OutOfRange(percent, nameof(percent), 0, 100);
        UpdateTimestamp();
    }

    private void SetCoreDetails(
        string title,
        Location location,
        string address,
        DateTime dateTimeStart,
        DateTime dateTimeEnd,
        EventType eventType,
        AgeRange ageRangeForMale,
        AgeRange ageRangeForFemale,
        int maleCapacity,
        int femaleCapacity,
        int numberOfChatAllowed,
        decimal maleTicketPrice,
        decimal femaleTicketPrice,
        EventEducationLevelRestriction educationLevelRestriction,
        IReadOnlyCollection<string>? tags,
        string? eventImage1,
        string? eventImage2,
        string? eventImage3,
        string eventDescriptionHtml,
        string maleTicketCurrencyCode = "IRR",
        string femaleTicketCurrencyCode = "IRR")
    {
        if (dateTimeEnd <= dateTimeStart)
            throw new BusinessRuleViolationException("Invalid event time", "End time must be after start time");

        Title = GuardAgainst.String.InvalidLength(title, nameof(title), 2, 150);
        Location = GuardAgainst.Object.Null(location, nameof(location));
        Address = GuardAgainst.String.InvalidLength(address, nameof(address), 5, 300);
        DateTimeStart = dateTimeStart;
        DateTimeEnd = dateTimeEnd;
        EventType = GuardAgainst.Object.Null(eventType, nameof(eventType));
        EventTypeId = eventType.Id;
        AgeRangeForMale = GuardAgainst.Object.Null(ageRangeForMale, nameof(ageRangeForMale));
        AgeRangeForFemale = GuardAgainst.Object.Null(ageRangeForFemale, nameof(ageRangeForFemale));
        MaleCapacity = GuardAgainst.Number.Positive(maleCapacity, nameof(maleCapacity));
        FemaleCapacity = GuardAgainst.Number.Positive(femaleCapacity, nameof(femaleCapacity));
        NumberOfLikesAllowed = GuardAgainst.Number.OutOfRange(numberOfChatAllowed, nameof(numberOfChatAllowed), 0, 10);
        MaleTicketPrice = GuardAgainst.Number.OutOfRange(maleTicketPrice, nameof(maleTicketPrice), 0.01m, 1_000_000_000m);
        FemaleTicketPrice = GuardAgainst.Number.OutOfRange(femaleTicketPrice, nameof(femaleTicketPrice), 0.01m, 1_000_000_000m);
        MaleTicketCurrencyCode = NormalizeCurrencyCode(maleTicketCurrencyCode);
        FemaleTicketCurrencyCode = NormalizeCurrencyCode(femaleTicketCurrencyCode);
        EducationLevelRestriction = GuardAgainst.Number.AgainstInvalidEnum<EventEducationLevelRestriction>((int)educationLevelRestriction, nameof(educationLevelRestriction));
        MinimumEducationLevelId = MapRestrictionEducationLevelId(EducationLevelRestriction);
        EventImage1 = NormalizeImage(eventImage1, nameof(eventImage1));
        EventImage2 = NormalizeImage(eventImage2, nameof(eventImage2));
        EventImage3 = NormalizeImage(eventImage3, nameof(eventImage3));
        EventDescriptionHtml = GuardAgainst.String.InvalidLength(eventDescriptionHtml, nameof(eventDescriptionHtml), 10, 10000);
    }

    private static string NormalizeTags(IReadOnlyCollection<string>? tags)
    {
        if (tags is null || tags.Count == 0)
            return string.Empty;

        var normalized = tags
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalized.Count > 10)
            throw new BusinessRuleViolationException("Too many event tags", "Each event can have at most 10 tags");

        foreach (var tag in normalized)
        {
            if (tag.Length is < 2 or > 30)
                throw new BusinessRuleViolationException("Invalid event tag", "Each event tag must be between 2 and 30 characters");
        }

        return string.Join('|', normalized);
    }

    private static string? NormalizeImage(string? image, string parameterName)
    {
        return string.IsNullOrWhiteSpace(image) ? null : GuardAgainst.String.MaxLength(image, parameterName, 500);
    }

    private static string NormalizeCurrencyCode(string? currencyCode)
    {
        var normalized = CurrencyLookup.NormalizeCode(string.IsNullOrWhiteSpace(currencyCode) ? "IRR" : currencyCode);
        var allowedCodes = new[] { "IRR", "EUR", "USD", "CAD", "GBP", "AED", "TRY" };
        if (!allowedCodes.Contains(normalized, StringComparer.OrdinalIgnoreCase))
            throw new BusinessRuleViolationException("Invalid ticket currency", "Ticket currency is not supported.");

        return normalized;
    }

    public void SetLocationLookup(long? countryId, long? cityId)
    {
        CountryId = countryId;
        CityId = cityId;
        UpdateTimestamp();
    }

    public EventDiscountCode AddDiscountCode(
        string code,
        EventDiscountGenderScope genderScope,
        EventDiscountType discountType,
        decimal value,
        DateTime startsAtUtc,
        DateTime endsAtUtc,
        int maxUsageCount,
        bool isActive,
        string? title = null,
        string? description = null)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        if (_discountCodes.Any(item => item.Code == normalizedCode))
            throw new BusinessRuleViolationException("Duplicate discount code", "Discount code already exists for this event.");

        var discountCode = new EventDiscountCode(
            this,
            normalizedCode,
            genderScope,
            discountType,
            value,
            startsAtUtc,
            endsAtUtc,
            maxUsageCount,
            isActive,
            title,
            description);

        _discountCodes.Add(discountCode);
        UpdateTimestamp();
        return discountCode;
    }

    public void ReplaceTags(IEnumerable<Tag> tags)
    {
        var normalizedTags = GuardAgainst.Object.Null(tags, nameof(tags))
            .Where(tag => tag.IsActive)
            .GroupBy(tag => tag.Id)
            .Select(group => group.First())
            .ToList();

        if (normalizedTags.Count > 10)
            throw new BusinessRuleViolationException("Too many event tags", "Each event can have at most 10 tags");

        _eventTags.Clear();
        foreach (var tag in normalizedTags)
        {
            _eventTags.Add(new EventTag(this, tag));
        }

        UpdateTimestamp();
    }

    public void SetMinimumEducationLevel(long? minimumEducationLevelId)
    {
        MinimumEducationLevelId = minimumEducationLevelId;
        EducationLevelRestriction = MapMinimumEducationLevelRestriction(minimumEducationLevelId);
        UpdateTimestamp();
    }

    public void SetEventDelivery(
        EventModeLookup eventMode,
        OnlineEventPlatform? onlineEventPlatform,
        string? onlineJoinUrl,
        string? onlineAccessInstructions)
    {
        EventMode = GuardAgainst.Object.Null(eventMode, nameof(eventMode));
        EventModeId = eventMode.Id;

        if (!eventMode.IsOnline)
        {
            OnlineEventPlatform = null;
            OnlineEventPlatformId = null;
            OnlineJoinUrl = null;
            OnlineAccessInstructions = null;
            UpdateTimestamp();
            return;
        }

        var platform = GuardAgainst.Object.Null(onlineEventPlatform!, nameof(onlineEventPlatform));
        OnlineEventPlatform = platform;
        OnlineEventPlatformId = platform.Id;
        OnlineJoinUrl = string.IsNullOrWhiteSpace(onlineJoinUrl)
            ? null
            : GuardAgainst.String.MaxLength(onlineJoinUrl.Trim(), nameof(onlineJoinUrl), 500);
        OnlineAccessInstructions = string.IsNullOrWhiteSpace(onlineAccessInstructions)
            ? null
            : GuardAgainst.String.MaxLength(onlineAccessInstructions.Trim(), nameof(onlineAccessInstructions), 1200);
        UpdateTimestamp();
    }

    public void ReplaceFaqs(IEnumerable<(string Question, string Answer)> faqs)
    {
        var normalizedFaqs = GuardAgainst.Object.Null(faqs, nameof(faqs))
            .Select(item => (Question: item.Question.Trim(), Answer: item.Answer.Trim()))
            .Where(item => !string.IsNullOrWhiteSpace(item.Question) || !string.IsNullOrWhiteSpace(item.Answer))
            .ToList();

        if (normalizedFaqs.Count > 10)
            throw new BusinessRuleViolationException("Too many event FAQs", "Each event can have at most 10 FAQ items");

        _faqs.Clear();
        for (var index = 0; index < normalizedFaqs.Count; index++)
        {
            var item = normalizedFaqs[index];
            _faqs.Add(new EventFaq(this, item.Question, item.Answer, index + 1));
        }

        UpdateTimestamp();
    }

    private bool MeetsEducationRestriction(UserProfile buyerProfile)
    {
        if (MinimumEducationLevelId is null && EducationLevelRestriction == EventEducationLevelRestriction.WithoutLimit)
            return true;

        var buyerEducationRank = MapProfileEducationLevelId(buyerProfile.EducationLevelId)
            ?? MapProfileEducationLevel(buyerProfile.EducationLevel);
        var requiredEducationRank = MapProfileEducationLevelId(MinimumEducationLevelId)
            ?? (EducationLevelRestriction == EventEducationLevelRestriction.WithoutLimit ? 0 : (int)EducationLevelRestriction);

        return buyerEducationRank >= requiredEducationRank;
    }

    private static int MapProfileEducationLevel(EducationLevel educationLevel) => educationLevel switch
    {
        EducationLevel.Diploma => 1,
        EducationLevel.Undergraduate => 2,
        EducationLevel.Graduated => 2,
        EducationLevel.Postgraduate => 3,
        EducationLevel.PhD => 4,
        EducationLevel.PostDoc => 4,
        _ => 0
    };

    private static int? MapProfileEducationLevelId(long? educationLevelId) => educationLevelId switch
    {
        1 => 0,
        2 => 1,
        3 => 2,
        4 => 3,
        5 => 4,
        _ => null
    };

    private static long? MapRestrictionEducationLevelId(EventEducationLevelRestriction restriction) => restriction switch
    {
        EventEducationLevelRestriction.WithoutLimit => null,
        EventEducationLevelRestriction.DiplomaOrHigher => 2,
        EventEducationLevelRestriction.BachelorOrHigher => 3,
        EventEducationLevelRestriction.MasterOrHigher => 4,
        EventEducationLevelRestriction.ProfessionalDoctorateOrPhD => 5,
        _ => null
    };

    private static EventEducationLevelRestriction MapMinimumEducationLevelRestriction(long? minimumEducationLevelId) => minimumEducationLevelId switch
    {
        null => EventEducationLevelRestriction.WithoutLimit,
        2 => EventEducationLevelRestriction.DiplomaOrHigher,
        3 => EventEducationLevelRestriction.BachelorOrHigher,
        4 => EventEducationLevelRestriction.MasterOrHigher,
        5 => EventEducationLevelRestriction.ProfessionalDoctorateOrPhD,
        _ => EventEducationLevelRestriction.WithoutLimit
    };
}
