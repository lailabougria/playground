using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging;
using NServiceBus;

class Program
{
    static async Task Main()
    {
        Console.Title = "Publisher";

        var endpointConfiguration = new EndpointConfiguration("Samples.CloudEvents.Publisher");

        endpointConfiguration.EnableInstallers();

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            //Converters = {  }
        };
        endpointConfiguration.UseSerialization<SystemJsonSerializer>()
            .Options(jsonSerializerOptions);
        
        endpointConfiguration.Conventions().DefiningEventsAs(type => type.Name == nameof(CloudEvent) || type.Name == nameof(EventOne));

        var connectionString = Environment.GetEnvironmentVariable("AzureServiceBus_ConnectionString");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception("Could not read the 'AzureServiceBus_ConnectionString' environment variable. Check the sample prerequisites.");
        }

        var transport = new AzureServiceBusTransport(connectionString);
        endpointConfiguration.UseTransport(transport);

        var endpointInstance = await Endpoint.Start(endpointConfiguration);
        
        Console.WriteLine("Press any key to publish events");
        Console.ReadKey();
        Console.WriteLine();

        await endpointInstance.Publish(new CloudEvent(
            "publisher", 
            typeof(EventOne).FullName,
                BinaryData.FromObjectAsJson(new EventOne(){
                    Content = $"{nameof(EventOne)} sample content",
                    PublishedOnUtc = DateTime.UtcNow
                }), 
                "application/json",
                CloudEventDataFormat.Json)
        );

        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        await endpointInstance.Stop();
    }
}