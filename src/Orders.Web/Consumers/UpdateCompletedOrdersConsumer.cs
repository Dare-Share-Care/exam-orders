using System.Text.Json;
using Confluent.Kafka;
using Orders.Web.Interfaces.DomainServices;
using Orders.Web.Models.Dto;
using Orders.Web.Models.Enums;

namespace Orders.Web.Consumers
{
    public class UpdatedCompletedOrdersConsumer : BackgroundService
    {
        private const string BootstrapServers = "localhost:9092";
        private const string GroupId = "mtogo-completed-deliveries-group";
        private const string Topic = "mtogo-completed-deliveries";

        private readonly IServiceProvider _serviceProvider;

        public UpdatedCompletedOrdersConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield(); //Find out what this does, cannot compile without

            var config = new ConsumerConfig
            {
                GroupId = GroupId,
                BootstrapServers = BootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest, // Always start at the beginning of the topic
                AllowAutoCreateTopics = true
            };

            using (var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                var cancelToken = new CancellationTokenSource();

                //Subscribe to topic
                consumerBuilder.Subscribe(Topic);

                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var consumeResult = consumerBuilder.Consume(cancelToken.Token);
                        var jsonObj = consumeResult.Message.Value;

                        using var scope = _serviceProvider.CreateScope();
                        var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                        var dto = JsonSerializer.Deserialize<ClaimOrderDto>(jsonObj);

                        if (dto != null)
                        {
                            await orderService.UpdateOrderStatusAsync(dto.OrderId, OrderStatus.Completed);
                        }
                    }
                }
                catch (Exception e)
                {
                    consumerBuilder.Close();
                }
            }
        }
    }
}