using System;
using System.Threading.Tasks;
using Azure.Messaging;
using NServiceBus;

class Program
{
    static async Task Main()
    {
        Console.Title = "Subscriber";

        var endpointConfiguration = new EndpointConfiguration("Samples.CloudEvents.Subscriber");

        endpointConfiguration.EnableInstallers();
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
        endpointConfiguration.Conventions().DefiningEventsAs(type => type.Name == nameof(CloudEvent) || type.Name == nameof(EventOne));

        var connectionString = Environment.GetEnvironmentVariable("AzureServiceBus_ConnectionString");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception("Could not read the 'AzureServiceBus_ConnectionString' environment variable. Check the sample prerequisites.");
        }

        var transport = new AzureServiceBusTransport(connectionString);
        endpointConfiguration.UseTransport(transport);

        var endpointInstance = await Endpoint.Start(endpointConfiguration);

        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        await endpointInstance.Stop();
    }
}