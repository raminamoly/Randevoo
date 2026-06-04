using System.ComponentModel.DataAnnotations;

namespace Randevoo.AdminPanel.Models.Users;

public sealed class AdminUserProfileInterestInput
{
    [Required(ErrorMessage = "علاقه الزامی است.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "عنوان علاقه معتبر نیست.")]
    public string InterestName { get; set; } = string.Empty;
}
