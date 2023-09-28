using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Producer
{
    public static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, Kafka Producer");

            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<ProducerService>();
            
            builder.Logging.SetMinimumLevel(LogLevel.Debug);
           

            var app = builder.Build();
            app.Run();

        }
    }


}