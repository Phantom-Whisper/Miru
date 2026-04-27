namespace Miru.Application.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
}