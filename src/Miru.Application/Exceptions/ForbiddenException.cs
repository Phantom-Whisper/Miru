namespace Miru.Application.Exceptions;

public class ForbiddenException(string message = "You do not have permission to access this resource")
    : Exception(message);