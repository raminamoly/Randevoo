using System.ComponentModel.DataAnnotations;
using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Models.Users;

public sealed class AdminUserProfileEditorInput
{
    [Required(ErrorMessage = "نام نمایشی الزامی است.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "نام نمایشی باید بین ۲ تا ۵۰ کاراکتر باشد.")]
    public string DisplayName { get; set; } = string.Empty;

    [Required(ErrorMessage = "شماره موبایل الزامی است.")]
    public string MobileNumber { get; set; } = string.Empty;

    public DateOnly DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    public long? CountryId { get; set; }

    public long? CityId { get; set; }

    public long? EducationLevelId { get; set; }

    [Range(120, 230, ErrorMessage = "قد معتبر نیست.")]
    public int HeightCentimeters { get; set; }

    public bool Smoking { get; set; }

    public bool IsActive { get; set; }
}
