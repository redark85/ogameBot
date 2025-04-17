using NinjaBot.Domain.Entities;
using NinjaBot.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaBot.Infrastructure.Persistence.Repositories
{
    public class UseryRepository(AppDbContext context) :
        RepositoryBase<User, AppDbContext>(context), IUserRepository
    {
    }
}
