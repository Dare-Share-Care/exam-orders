using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace Orders.Test.Drivers;

public static class TestTopicManager
{
    private const string BootstrapServers = "kafka:9093";
    
   //Create test topic
    public static async Task CreateTopic(string topic)
    {
        var config = new AdminClientConfig { BootstrapServers = BootstrapServers };
        using var adminClient = new AdminClientBuilder(config).Build();
        await adminClient.CreateTopicsAsync(new TopicSpecification[] { new TopicSpecification { Name = topic, ReplicationFactor = 1, NumPartitions = 1 } });
    }
    
    public static async Task DeleteTopic(string topic)
    {
        var config = new AdminClientConfig { BootstrapServers = BootstrapServers };
        using var adminClient = new AdminClientBuilder(config).Build();

        await adminClient.DeleteTopicsAsync(new[] { topic });
    }
    
    //Get topic messages
    public async static Task<List<string>> GetTopicMessages(string topic)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = BootstrapServers,
            GroupId = Guid.NewGuid().ToString(),
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(topic);

        var messageList = new List<string>();

        try
        {
            while (true)
            {
                try
                {
                    var consumeResult = consumer.Consume(TimeSpan.FromSeconds(1));

                    if (consumeResult == null)
                        break;

                    messageList.Add(consumeResult.Message.Value);
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Error occurred: {e.Error.Reason}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            consumer.Close();
        }
        finally
        {
            consumer.Close();
        }

        return messageList;
    }
}