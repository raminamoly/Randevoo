using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Randevoo.Web.Services;

public sealed class EndUserProfileApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly EndUserSessionService _session;

    public EndUserProfileApiClient(IHttpClientFactory httpClientFactory, EndUserSessionService session)
    {
        _httpClientFactory = httpClientFactory;
        _session = session;
    }

    public async Task<DatingProfileViewModel?> GetMineAsync(CancellationToken cancellationToken)
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync("/api/v1/platform/profile/me", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorAsync(response, cancellationToken));

        return await response.Content.ReadFromJsonAsync<DatingProfileViewModel>(cancellationToken);
    }

    public async Task<DatingProfileViewModel> CreateAsync(ProfileFormModel form, CancellationToken cancellationToken)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PostAsJsonAsync("/api/v1/platform/profile/me", form.ToApiRequest(), cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorAsync(response, cancellationToken));

        return await response.Content.ReadFromJsonAsync<DatingProfileViewModel>(cancellationToken)
            ?? throw new InvalidOperationException("پاسخ پروفایل نامعتبر است.");
    }

    public async Task UpdateAsync(long profileId, ProfileFormModel form, CancellationToken cancellationToken)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PutAsJsonAsync("/api/v1/platform/profile/me", form.ToApiRequest(), cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorAsync(response, cancellationToken));
    }

    private HttpClient CreateAuthorizedClient()
    {
        var token = _session.GetAccessToken();
        if (string.IsNullOrWhiteSpace(token))
            throw new UnauthorizedAccessException("برای ادامه باید وارد شوید.");

        var client = _httpClientFactory.CreateClient("RandevooApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        return string.IsNullOrWhiteSpace(body) ? "درخواست پروفایل انجام نشد." : body;
    }
}

public sealed record DatingProfileViewModel(
    long Id,
    long UserId,
    string DisplayName,
    int Gender,
    long? GenderId,
    DateOnly DateOfBirth,
    int Age,
    long? ZodiacSignId,
    int HeightCm,
    int EducationLevel,
    int ProfileStatus,
    bool Smoking,
    string Country,
    string City,
    string? Region,
    decimal Latitude,
    decimal Longitude,
    IReadOnlyList<string> Interests,
    string? PrimaryImageUrl,
    IReadOnlyList<string> ImageUrls);

public sealed class ProfileFormModel
{
    public string DisplayName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; } = new(1995, 1, 1);
    public int Gender { get; set; } = 2;
    public string Country { get; set; } = "Iran";
    public string City { get; set; } = "Tehran";
    public string? Region { get; set; }
    public decimal Latitude { get; set; } = 35.6895m;
    public decimal Longitude { get; set; } = 51.3890m;
    public int HeightCm { get; set; } = 170;
    public int EducationLevel { get; set; } = 3;
    public bool Smoking { get; set; }
    public long? ZodiacSignId { get; set; } = 2;
    public string? PrimaryImageUrl { get; set; }
    public List<string> PhotoUrls { get; set; } = [];
    public List<string> SelectedInterestNames { get; set; } = [];

    public object ToApiRequest() => new
    {
        DisplayName,
        DateOfBirth,
        Gender = ToGenderEnumValue(Gender),
        Country,
        City,
        Latitude,
        Longitude,
        HeightCm,
        EducationLevel,
        Smoking,
        Region,
        InterestNames = SelectedInterestNames
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(4)
            .ToList(),
        ZodiacSignId,
        PhotoUrls = PhotoUrls
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(3)
            .ToList(),
        PrimaryImageUrl
    };

    public static ProfileFormModel FromProfile(DatingProfileViewModel profile) => new()
    {
        DisplayName = profile.DisplayName,
        DateOfBirth = profile.DateOfBirth,
        Gender = (int)(profile.GenderId ?? 2),
        ZodiacSignId = profile.ZodiacSignId,
        Country = profile.Country,
        City = profile.City,
        Region = profile.Region,
        Latitude = profile.Latitude,
        Longitude = profile.Longitude,
        HeightCm = profile.HeightCm,
        EducationLevel = profile.EducationLevel,
        Smoking = profile.Smoking,
        PrimaryImageUrl = profile.PrimaryImageUrl,
        PhotoUrls = profile.ImageUrls.ToList(),
        SelectedInterestNames = profile.Interests.Take(4).ToList()
    };

    private static int ToGenderEnumValue(int genderId) => genderId switch
    {
        2 => 1,
        3 => 2,
        _ => 0
    };
}
