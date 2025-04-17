using NinjaBot.Domain.Interfaces.Repositories;

namespace NinjaBot.Domain.Interfaces.Services;

public interface IAppDataService : IDisposable
{
    IUserRepository User { get; }
    Task SaveChangesAsync();
}
