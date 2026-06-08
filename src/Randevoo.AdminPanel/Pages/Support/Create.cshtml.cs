using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Support;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Application.Features.SupportTickets.Common;

namespace Randevoo.AdminPanel.Pages.Support;

public class CreateModel : PageModel
{
    private readonly ISupportTicketsApiClient _supportApi;
    private readonly CurrentSessionState _session;
    private readonly IWebHostEnvironment _environment;

    public CreateModel(ISupportTicketsApiClient supportApi, CurrentSessionState session, IWebHostEnvironment environment)
    {
        _supportApi = supportApi;
        _session = session;
        _environment = environment;
    }

    [BindProperty]
    public SupportTicketCreateInput Input { get; set; } = new();

    [BindProperty]
    public List<IFormFile> Attachments { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return Page();

        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        try
        {
            var attachments = await SaveAttachmentsAsync(Attachments, _environment, cancellationToken);
            var ticket = await _supportApi.CreateTicketAsync(current, Input.Title, Input.Category, Input.Body, attachments, cancellationToken);
            StatusMessage = "تیکت پشتیبانی ثبت شد.";
            return RedirectToPage("/Support/Details", new { id = ticket.Id });
        }
        catch (Exception ex) when (ex is InvalidOperationException or Randevoo.Domain.Exceptions.DomainException)
        {
            StatusMessage = ex.Message;
            return Page();
        }
    }

    internal static async Task<IReadOnlyList<SupportTicketAttachmentInput>> SaveAttachmentsAsync(IEnumerable<IFormFile> files, IWebHostEnvironment environment, CancellationToken cancellationToken)
    {
        var result = new List<SupportTicketAttachmentInput>();
        foreach (var file in files.Where(file => file.Length > 0))
        {
            if (!file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("فقط فایل تصویری قابل بارگذاری است.");
            if (file.Length > 5 * 1024 * 1024)
                throw new InvalidOperationException("حجم هر تصویر باید کمتر از ۵ مگابایت باشد.");

            var extension = Path.GetExtension(file.FileName);
            var safeName = $"{Guid.NewGuid():N}{extension}";
            var folder = Path.Combine(environment.WebRootPath, "uploads", "support");
            Directory.CreateDirectory(folder);
            var path = Path.Combine(folder, safeName);
            await using var stream = System.IO.File.Create(path);
            await file.CopyToAsync(stream, cancellationToken);
            result.Add(new SupportTicketAttachmentInput(file.FileName, file.ContentType, file.Length, $"/uploads/support/{safeName}"));
        }

        return result;
    }
}
