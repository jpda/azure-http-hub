using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AzureHttpHub
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;

            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true);
            var config = builder.Build();

            var jsonSerializer = true;
            Console.WriteLine("How would you like to serialize data? Options are (j)son (default) or (p)rotobuf");
            var answer = Console.ReadLine();
            if (!string.IsNullOrEmpty(answer))
            {
                if (answer == "p")
                {
                    jsonSerializer = false;
                }
            }

            Console.WriteLine("How many messages to send? default 10");
            var iterations = 10;
            var iterationAnswer = Console.ReadLine();
            if (!string.IsNullOrEmpty(iterationAnswer))
            {
                int.TryParse(iterationAnswer, out iterations);
            }

            //StartReaders(jsonSerializer ? "json" : "protobuf");

            var iotConnectionString = config["AppSettings:IotHubConnectionString"];
            var sbConnectionString = config["AppSettings:EventHubConnectionString"];

            var senders = new List<Sender>()
            {
                new IotSender(iotConnectionString),
                new EventHubSender(sbConnectionString)
            };

            Console.WriteLine($" === Using {(jsonSerializer ? "JSON" : "Protobuf")} serializer, sending {iterations} items ====");

            new Worker().Go(senders, x =>
            {
                if (jsonSerializer)
                {
                    var j = JsonConvert.SerializeObject(x);
                    return System.Text.Encoding.UTF8.GetBytes(j);
                }
                using (var ms = new MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(ms, x);
                    return ms.ToArray();
                }
            });

            Console.ReadLine();
        }

        public static void StartReaders(string serialization)
        {
            var command = $@"start-{serialization}.bat";
            var pi = new ProcessStartInfo("cmd.exe", "/c " + command) { CreateNoWindow = true };
            Process.Start(pi);
        }
    }
}