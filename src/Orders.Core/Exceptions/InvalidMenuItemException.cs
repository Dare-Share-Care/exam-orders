namespace Orders.Core.Exceptions;

public class InvalidMenuItemException : Exception
{
    public InvalidMenuItemException(string message) : base(message)
    {
    }
}