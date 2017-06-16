using System;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;

namespace EventHubClientProcessor
{
    public class Processor
    {
        private readonly EventProcessorConfiguration _config;

        public Processor(EventProcessorConfiguration config)
        {
            _config = config;
        }

        public async Task StartAsync()
        {
            Console.WriteLine($"Registering EventHubProcessor, connecting to {_config.ConnectionString.Split(';')[0]}...");

            var eventProcessorHost = new EventProcessorHost(_config.EntityPath, _config.ConsumerGroup, _config.ConnectionString, _config.StorageConnection, _config.StorageContainer);
            await eventProcessorHost.RegisterEventProcessorFactoryAsync(new DeserializingEventProcessorFactory(_config.Deserializer));

            Console.WriteLine("Receiving. Press ENTER to stop worker.");
            Console.ReadLine();

            await eventProcessorHost.UnregisterEventProcessorAsync();
        }
    }
}