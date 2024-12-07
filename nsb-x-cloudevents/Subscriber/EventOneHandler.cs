using System;
using System.Threading.Tasks;
using Azure.Messaging;
using NServiceBus;

public class EventOneHandler : IHandleMessages<CloudEvent>
{
    public Task Handle(CloudEvent message, IMessageHandlerContext context)
    {
        Console.WriteLine("Received a CloudEvent");
        var eventOne = message.Data.ToObjectFromJson<EventOne>();
        if (eventOne != null)
        {
            Console.WriteLine($"Received EventOne: {eventOne.Content}");
        }
        else
        {
            Console.WriteLine("Received an event that is not EventOne");
        }
        return Task.CompletedTask;
    }
}