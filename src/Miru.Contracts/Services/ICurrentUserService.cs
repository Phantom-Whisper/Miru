namespace Miru.Contracts.Services;

public interface ICurrentUserService
{
    Guid UserId { get; }
}