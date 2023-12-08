namespace Orders.Core.Exceptions;

public class FeeNotFoundException : Exception
{
    public FeeNotFoundException(string message) : base(message)
    {
    }
}