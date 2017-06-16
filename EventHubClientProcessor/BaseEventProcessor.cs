using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;

namespace EventHubClientProcessor
{
    public abstract class BaseEventProcessor : IEventProcessor
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

        public abstract Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages);

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            Console.WriteLine($"{GetType().Name}: Error on Partition: {context.PartitionId}, Error: {error.Message}");
            return Task.CompletedTask;
        }
    }
}