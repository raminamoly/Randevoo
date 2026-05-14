// Domain/Exceptions/DomainException.cs (Base)
namespace Randevoo.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
    protected DomainException(string message, Exception inner) : base(message, inner) { }
}

// Specific exceptions
public class InvalidEntityStateException : DomainException
{
    public InvalidEntityStateException(string message) : base(message) { }
}

public class BusinessRuleViolationException : DomainException
{
    public BusinessRuleViolationException(string rule, string details)
        : base($"Business rule violated: {rule}. {details}") { }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object id)
        : base($"{entityName} with id {id} was not found") { }
}