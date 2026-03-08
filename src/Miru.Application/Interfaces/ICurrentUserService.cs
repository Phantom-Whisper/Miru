namespace Miru.Shared.Services;

public interface ICurrentUserService
{
    Guid UserId { get; }
}