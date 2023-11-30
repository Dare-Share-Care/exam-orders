namespace Orders.Web.Exceptions;

public class InvalidCatalogueException : Exception
{
    public InvalidCatalogueException(string message) : base(message)
    {
    }
}