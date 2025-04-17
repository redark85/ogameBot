
namespace NinjaBot.Domain.Interfaces;

public interface IBaseEntity
{
    long Id { get; set; }
    string CreatedBy { get; set; }
    DateTime CreatedAt { get; set; }

    string? UpdatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
}
