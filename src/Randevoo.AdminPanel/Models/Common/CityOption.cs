namespace Randevoo.AdminPanel.Models.Common;

public sealed class CityOption
{
    public long Id { get; set; }
    public long CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public bool IsActive { get; set; }
}
