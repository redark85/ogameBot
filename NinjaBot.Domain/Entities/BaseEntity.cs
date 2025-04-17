using NinjaBot.Domain.Interfaces;

namespace NinjaBot.Domain.Entities;

public abstract class BaseEntity : IBaseEntity
{
    public long Id { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
