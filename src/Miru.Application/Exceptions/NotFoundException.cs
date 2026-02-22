namespace Miru.Application.Exceptions;

public class NotFoundException(string entityName, object key) : Exception($"{entityName} with key '{key}' not found");