using System;
using AzureHttpHub.Model;
using Microsoft.Azure.EventHubs.Processor;

namespace EventHubClientProcessor
{
    public class DeserializingEventProcessorFactory : IEventProcessorFactory
    {
        private readonly Func<byte[], PingData> _deserializer;

        public DeserializingEventProcessorFactory(Func<byte[], PingData> deserializer)
        {
            _deserializer = deserializer;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            return new EventHubProcessor(_deserializer);
        }
    }
}