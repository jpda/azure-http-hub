using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureHttpHub.Model;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;

namespace EventHubClientProcessor
{
    public class EventHubProcessor : BaseEventProcessor
    {
        private readonly Func<byte[], PingData> _deserializer;

        public EventHubProcessor(Func<byte[], PingData> deserializer)
        {
            _deserializer = deserializer;
        }

        public override Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var eventData in messages)
            {
                var d = _deserializer(eventData.Body.Array);
                Console.WriteLine($"{GetType().Name}: Message received. Partition: '{context.PartitionId}', Data: '{d.DeviceId}, {d.Message}, {d.PingTime} (took {(DateTime.UtcNow - d.PingTime).Milliseconds}ms)'");
            }
            return context.CheckpointAsync();
        }
    }
}