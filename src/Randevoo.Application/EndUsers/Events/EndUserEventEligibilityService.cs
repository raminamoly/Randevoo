using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.EndUsers.Events;

public sealed class EndUserEventEligibilityService : IEndUserEventEligibilityService
{
    public EndUserEventEligibilityResult Evaluate(UserProfile? profile, DatingEvent datingEvent, DateTime nowUtc)
    {
        if (datingEvent.IsCancelled || datingEvent.DateTimeEnd <= nowUtc)
            return Blocked("event_closed", "این رویداد دیگر برای خرید بلیت فعال نیست.");

        if (!datingEvent.IsOpenForSell)
            return Blocked("sale_closed", "فروش بلیت این رویداد باز نیست.");

        if (profile is null)
            return Blocked("profile_missing", "برای خرید بلیت ابتدا پروفایل را تکمیل کنید.");

        profile.RefreshProfileStatus();
        if (profile.ProfileStatus == UserProfileStatus.Incomplete)
            return Blocked("profile_incomplete", "برای خرید بلیت ابتدا پروفایل را تکمیل کنید.");

        if (profile.Gender is not (Gender.Male or Gender.Female))
            return Blocked("gender_missing", "برای بررسی ظرفیت و قیمت، جنسیت پروفایل باید مشخص باشد.");

        var activeTickets = datingEvent.Tickets.Where(ticket => !ticket.IsRefunded && !ticket.IsRemoved).ToList();
        var soldForGender = activeTickets.Count(ticket => ticket.Gender == profile.Gender);
        var capacityForGender = profile.Gender == Gender.Male ? datingEvent.MaleCapacity : datingEvent.FemaleCapacity;
        if (soldForGender >= capacityForGender)
            return Blocked("capacity_full", "ظرفیت مناسب پروفایل شما در این رویداد تکمیل شده است.");

        var ageRange = profile.Gender == Gender.Male ? datingEvent.AgeRangeForMale : datingEvent.AgeRangeForFemale;
        if (!ageRange.IsWithinRange(profile.Age))
            return Blocked("age_not_allowed", "بازه سنی این رویداد با پروفایل شما سازگار نیست.");

        if (!MeetsEducationRequirement(profile, datingEvent))
            return Blocked("education_not_allowed", "مدرک تحصیلی پروفایل شما با شرط این رویداد سازگار نیست.");

        return new EndUserEventEligibilityResult(true, "eligible", "امکان خرید بلیت برای شما وجود دارد.");
    }

    private static EndUserEventEligibilityResult Blocked(string reasonCode, string message) =>
        new(false, reasonCode, message);

    private static bool MeetsEducationRequirement(UserProfile profile, DatingEvent datingEvent)
    {
        var requiredRank = EducationLevelIdRank(datingEvent.MinimumEducationLevelId)
            ?? EventRestrictionRank(datingEvent.EducationLevelRestriction);
        if (requiredRank <= 0)
            return true;

        var profileRank = EducationLevelIdRank(profile.EducationLevelId)
            ?? ProfileEducationRank(profile.EducationLevel);
        return profileRank >= requiredRank;
    }

    private static int EventRestrictionRank(EventEducationLevelRestriction restriction) => restriction switch
    {
        EventEducationLevelRestriction.WithoutLimit => 0,
        EventEducationLevelRestriction.DiplomaOrHigher => 1,
        EventEducationLevelRestriction.BachelorOrHigher => 2,
        EventEducationLevelRestriction.MasterOrHigher => 3,
        EventEducationLevelRestriction.ProfessionalDoctorateOrPhD => 4,
        _ => 0
    };

    private static int? EducationLevelIdRank(long? educationLevelId) => educationLevelId switch
    {
        null => null,
        1 => 0,
        2 => 1,
        3 => 2,
        4 => 3,
        5 => 4,
        _ => null
    };

    private static int ProfileEducationRank(EducationLevel educationLevel) => educationLevel switch
    {
        EducationLevel.Diploma => 1,
        EducationLevel.Undergraduate => 2,
        EducationLevel.Graduated => 2,
        EducationLevel.Postgraduate => 3,
        EducationLevel.PhD => 4,
        EducationLevel.PostDoc => 4,
        _ => 0
    };
}
