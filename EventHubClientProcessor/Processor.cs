using System;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;

namespace EventHubClientProcessor
{
    public class Processor
    {
        private const string StorageAccountName = "{storage account name}";
        private const string StorageAccountKey = "{storage account key}";
        private static readonly string StorageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", StorageAccountName, StorageAccountKey);

        private readonly string _connectionString;
        private readonly string _entityPath;
        private readonly string _containerName;

        public Processor(string connectionString, string entityPath, string containerName)
        {
            _connectionString = connectionString;
            _entityPath = entityPath;
            _containerName = containerName;
        }

        public async Task StartAsync<T>() where T : IEventProcessor, new()
        {
            Console.WriteLine($"Registering {typeof(T).Name}, connecting to {_connectionString.Split(';')[0]}...");

            var eventProcessorHost = new EventProcessorHost(
                _entityPath,
                PartitionReceiver.DefaultConsumerGroupName,
                _connectionString,
                StorageConnectionString,
                _containerName);

            await eventProcessorHost.RegisterEventProcessorAsync<T>();

            Console.WriteLine("Receiving. Press ENTER to stop worker.");
            Console.ReadLine();

            // Disposes of the Event Processor Host
            await eventProcessorHost.UnregisterEventProcessorAsync();
        }
    }
}