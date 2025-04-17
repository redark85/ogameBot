namespace NinjaBot.Domain.Dtos;

public class GetTokerRequestDto
{
    public string Blackbox { get; set; } = string.Empty;

    public string GameEnvironmentId { get; set; } = string.Empty;

    public string Language { get; set; } = string.Empty;
    public string Identity { get; set; } = string.Empty;

    public string Locale { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string PlatformGameId { get; set; } = string.Empty;
}
