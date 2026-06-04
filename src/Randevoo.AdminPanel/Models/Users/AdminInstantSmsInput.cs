using System.ComponentModel.DataAnnotations;

namespace Randevoo.AdminPanel.Models.Users;

public sealed class AdminInstantSmsInput
{
    [Required(ErrorMessage = "متن پیامک الزامی است.")]
    [StringLength(480, MinimumLength = 5, ErrorMessage = "متن پیامک باید بین ۵ تا ۴۸۰ کاراکتر باشد.")]
    public string Message { get; set; } = string.Empty;
}
