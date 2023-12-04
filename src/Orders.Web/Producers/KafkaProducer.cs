using System.Text.Json;
using Confluent.Kafka;
using Orders.Web.Interfaces.Producers;

namespace Orders.Web.Producers;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string, string> _producer;
    private const string BootstrapServers = "kafka:9093";
    
    public KafkaProducer()
    {
        var config = new ProducerConfig { BootstrapServers = BootstrapServers };
        _producer = new ProducerBuilder<string, string>(config).Build();
    }
    
    public async Task ProduceAsync<T>(string topic, T value)
    {
        var message = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = JsonSerializer.Serialize(value)
        };
        await _producer.ProduceAsync(topic, message);
    }
    
    
    public void Dispose()
    {
        _producer.Dispose();
    }
}