using System.ComponentModel.DataAnnotations;

namespace Tehilim.Service;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    [Required] public string Issuer { get; set; } = "";
    [Required] public string Audience { get; set; } = "";
    [Required, MinLength(32)] public string Key { get; set; } = "";
    [Range(1, 1440)] public int ExpiryMinutes { get; set; } = 60;
}