using System.ComponentModel.DataAnnotations;

namespace Randevoo.AdminPanel.Models.Users;

public sealed class AdminUserProfileImageInput
{
    [Required(ErrorMessage = "آدرس تصویر الزامی است.")]
    [StringLength(500, MinimumLength = 2, ErrorMessage = "آدرس تصویر معتبر نیست.")]
    public string ImageUrl { get; set; } = string.Empty;
}
