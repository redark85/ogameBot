namespace NinjaBot.Domain.Entities;

public class User : BaseEntity
{
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Blackbox { get; set; } = string.Empty;
    public string Universe { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;

}
