namespace Shopniu_api.Domain.Exceptions.Common;

public class ValidationsException : DomainException
{
    public IEnumerable<string> Errors { get; }

    public ValidationsException(string message) : base(message)
    {
        Errors = new List<string> { message };
    }

    public ValidationsException(IEnumerable<string> errors) : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}