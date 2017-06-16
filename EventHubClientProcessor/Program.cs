using System.IO;
using System.Linq;
using AzureHttpHub.Model;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace EventHubClientProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true);
            var config = builder.Build();

            var mode = "iot";
            var serializer = "json";
            if (args.Any())
            {
                mode = args[0];
                serializer = args[1];
            }

            var processorConfiguration = new EventProcessorConfiguration()
            {
                StorageConnection = config["AppSettings:StorageConnection"],
                ConsumerGroup = PartitionReceiver.DefaultConsumerGroupName
            };

            if (mode == "iot")
            {
                processorConfiguration.ConnectionString = config["AppSettings:IotHubConnectionString"];
                processorConfiguration.EntityPath = config["AppSettings:IotEntityPath"];
            }
            else
            {
                processorConfiguration.ConnectionString = config["AppSettings:EventHubConnectionString"];
                processorConfiguration.EntityPath = config["AppSettings:EventHubEntityPath"];
            }

            if (serializer == "json")
            {
                processorConfiguration.Deserializer = x => JsonConvert.DeserializeObject<PingData>(System.Text.Encoding.UTF8.GetString(x));
            }
            else
            {
                processorConfiguration.Deserializer = x =>
                {
                    using (var ms = new MemoryStream(x))
                    {
                        return ProtoBuf.Serializer.Deserialize<PingData>(ms);
                    }
                };
            }

            var p = new Processor(processorConfiguration);
            p.StartAsync().Wait();
        }
    }
}