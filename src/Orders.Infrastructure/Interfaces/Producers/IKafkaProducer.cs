namespace Orders.Infrastructure.Interfaces.Producers;

public interface IKafkaProducer : IDisposable
{
    Task ProduceAsync<T>(string topic, T value);
}