using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureHttpHub.Model;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Newtonsoft.Json;

public class StringEventHubProcessor : IEventProcessor
{
    public Task OpenAsync(PartitionContext context)
    {
        Console.WriteLine($"{GetType().Name}: initialized. Partition: '{context.PartitionId}'");
        return Task.CompletedTask;
    }

    public Task CloseAsync(PartitionContext context, CloseReason reason)
    {
        Console.WriteLine($"{GetType().Name} Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");
        return Task.CompletedTask;
    }

    public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
    {
        foreach (var eventData in messages)
        {
            var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
            var d = JsonConvert.DeserializeObject<PingData>(data);
            Console.WriteLine($"{GetType().Name}: Message received. Partition: '{context.PartitionId}', Data: '{d.DeviceId}, {d.Message}, {d.PingTime} (took {(DateTime.UtcNow - d.PingTime).Milliseconds}ms)'");
        }

        return context.CheckpointAsync();
    }

    public Task ProcessErrorAsync(PartitionContext context, Exception error)
    {
        Console.WriteLine($"{GetType().Name}: Error on Partition: {context.PartitionId}, Error: {error.Message}");
        return Task.CompletedTask;
    }
}