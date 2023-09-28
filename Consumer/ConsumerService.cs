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
using static Confluent.Kafka.ConfigPropertyNames;
using System.Reflection.PortableExecutable;

namespace Consumer
{
    public class ConsumerService : IHostedService
    {
        private readonly ILogger _logger;
        private IConsumer<string, OrderRequest> _consumer;
        private readonly string _topicName = "OrderRequest-Topic";
        public ConsumerService(ILogger<ConsumerService> logger)
        {
            _logger = logger;
            var config = new ConsumerConfig()
            {
                BootstrapServers = "localhost:9092",
                ClientId = "local-consumer",
                SecurityProtocol = SecurityProtocol.Plaintext,
                //SaslMechanism = SaslMechanism.Plain,
                //SaslUsername = "",
                //SaslPassword = "",
                GroupId = "consumer-1",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true,

            };

            _consumer = new ConsumerBuilder<string, OrderRequest>(config)
                .SetValueDeserializer(new MyJsonDeserializer<OrderRequest>().AsSyncOverAsync()).Build();
            _consumer.Subscribe(_topicName);

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            while(!cancellationToken.IsCancellationRequested) 
            {
                try
                {
                    var result = _consumer.Consume(cancellationToken);
                    var orderRequest = result.Message?.Value as OrderRequest;
                    var headers = result.Message?.Headers;

                    _logger.LogDebug($"Received at Consumer - Partition = {result.Topic}-{result.Partition}-{result.Offset} : OrderId = {orderRequest?.OrderId}, CustomerId = {orderRequest?.CustomerId}");
                    _logger.LogDebug($"Received Headers are {headers?.Count}");
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogError($"Error at Consumer {ex.Message}");
                    _consumer.Close();
                }
            }

            _consumer?.Close();
            return Task.CompletedTask;

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer?.Close();
            _consumer?.Dispose();
            return Task.CompletedTask;
        }
    }
}
