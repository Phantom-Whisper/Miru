using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Miru.Application.Exceptions;
using Miru.Application.Interfaces;

namespace Miru.Application.Services;

public class CurrentUserService : ICurrentUserService
{
    public Guid UserId { get; }

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim))
            throw new NotFoundException("User", UserId.ToString());
        
        UserId = Guid.Parse(userIdClaim);
    }
}