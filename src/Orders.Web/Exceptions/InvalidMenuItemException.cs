namespace Orders.Web.Exceptions;

public class InvalidMenuItemException : Exception
{
    public InvalidMenuItemException(string message) : base(message)
    {
    }
}