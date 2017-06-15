using System.Linq;

namespace EventHubClientProcessor
{
    class Program
    {
        private const string EhConnectionString = "{eh connection string}";
        private const string IotHConnectionString = "{iot hub xon string}";
        private const string EhEntityPath = "{eh entity name}";
        private const string IotEntityPath = "{iot entity name}";

        static void Main(string[] args)
        {
            var mode = "iot";
            var serializer = "json";
            if (args.Any())
            {
                mode = args[0];
                serializer = args[1];
            }

            var p = mode == "iot" ? new Processor(IotHConnectionString, IotEntityPath, "iot") : new Processor(EhConnectionString, EhEntityPath, "eventhub");

            if (serializer == "json")
            {
                p.StartAsync<StringEventHubProcessor>().Wait();
            }
            else
            {
                p.StartAsync<ProtobufEventHubProcessor>().Wait();
            }
        }
    }
}