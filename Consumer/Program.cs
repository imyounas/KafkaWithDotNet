using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Consumer
{
    public static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, Consumer!");

            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<ConsumerService>();
            builder.Logging.SetMinimumLevel(LogLevel.Debug);


            var app = builder.Build();           
            app.Run();
        }
    }
}