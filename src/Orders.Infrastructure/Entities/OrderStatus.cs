namespace Orders.Infrastructure.Entities;

public enum OrderStatus //ValueObject
{
    New = 0,
    InProgress = 1,
    InDelivery = 2,
    Completed = 3
}