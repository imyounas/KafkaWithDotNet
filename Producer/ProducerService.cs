using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Confluent.Kafka.SyncOverAsync;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MyDomain;
using KafkaUtils;

namespace Producer
{
    public class ProducerService : IHostedService
    {
        private readonly ILogger _logger;
        private IProducer<string, OrderRequest> _producer;
        private readonly string _topicName = "OrderRequest-Topic";
        public ProducerService(ILogger<ProducerService> logger ) 
        {
            _logger = logger;
            var config = new ProducerConfig()
            {
                BootstrapServers = "localhost:9092",
                ClientId = "local-producer",
                SecurityProtocol = SecurityProtocol.Plaintext,
                //SaslMechanism = SaslMechanism.Plain,
                Acks = Acks.Leader,
                MessageTimeoutMs = 5000,
                BatchNumMessages = 10,
                LingerMs = 10,
                CompressionType = CompressionType.Gzip
            };

            _producer = new ProducerBuilder<string, OrderRequest>(config)
                .SetValueSerializer(new MyJsonSerializer<OrderRequest>()).Build();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            int c = 1;
            while(true)
            {
                var orderRequest = new OrderRequest()
                {
                    OrderId = c,
                    ProductId = $"Product_{c}",
                    Quantity = c + 10,
                    CustomerId = c % 3 == 0 ? "Customer_1" : c % 5 == 0 ? "Customer_2" : "Customer_3"
                };

                _logger.LogDebug($"Sending Order Request Id {c}");
                await _producer.ProduceAsync(_topicName, new Message<string, OrderRequest>() 
                {
                    Key = orderRequest.CustomerId,
                    Value = orderRequest

                } , cancellationToken
                );

                c++;

                if(c % 100 == 0 )
                {
                    _producer.Flush(TimeSpan.FromSeconds(1));
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _producer?.Dispose();
            return Task.CompletedTask;
        }
    }
}
