using Microsoft.EntityFrameworkCore;
using NinjaBot.Domain.Interfaces.Repositories;
using NinjaBot.Domain.Interfaces.Services;
using NinjaBot.Infrastructure.Persistence;
using NinjaBot.Infrastructure.Persistence.Repositories;

namespace NinjaBot.Infrastructure.Services
{
    public class AppDataService(
        AppDbContext dbContext) : IAppDataService, IDisposable
    {
        private bool _disposed = false;

        public IUserRepository User { get; } = new UseryRepository(dbContext);

        public async Task SaveChangesAsync()
        {
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;
                }
            } while (saveFailed);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    dbContext.Dispose();
                }
            }
            _disposed = true;
        }
    }
}
