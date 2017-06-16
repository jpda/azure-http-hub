using System;
using AzureHttpHub.Model;

namespace EventHubClientProcessor
{
    public class EventProcessorConfiguration
    {
        public string EntityPath { get; set; }
        public string ConsumerGroup { get; set; }
        public string ConnectionString { get; set; }
        public string StorageConnection { get; set; }
        public string StorageContainer { get; set; }
        public Func<byte[], PingData> Deserializer { get; set; }
    }
}