namespace Orders.Web.Exceptions;

public class OrderNotFoundException : Exception
{
    public OrderNotFoundException(long orderId) : base($"Order with id {orderId} was not found")
    {
    }
}