using System.Text.Json;
using Confluent.Kafka;
using Orders.Core.Interfaces;
using Orders.Core.Models.Dto;
using Orders.Infrastructure.Entities;

namespace Orders.Web.Consumers
{
    public class UpdatedClaimedOrdersConsumer : BackgroundService
    {
        private const string BootstrapServers = "kafka:9093";
        private const string GroupId = "mtogo-claimed-deliveries-group";
        private const string Topic = "mtogo-claimed-deliveries";

        private readonly IServiceProvider _serviceProvider;

        public UpdatedClaimedOrdersConsumer(IServiceProvider serviceProvider)
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
                            await orderService.UpdateOrderStatusAsync(dto.OrderId, OrderStatus.InDelivery);
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