using NinjaBot.Domain.Models;
using NinjaBot.Shared.Dtos.Requests;

namespace NinjaBot.Core.Services;

public interface IOGameLoginService
{
    Task<OGameLoginPayload?> LoginWithCredentialsAsync(LoginRequestDto dto);
}
