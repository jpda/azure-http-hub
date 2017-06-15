using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AzureHttpHub.Model;
using Newtonsoft.Json;

namespace AzureHttpHub
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            new Thing().Go();
            Console.ReadLine();
        }

        public static void StartReaders(string serialization)
        {
            var command = $@"..\..\..\start-{serialization}.bat";
            var pi = new ProcessStartInfo("cmd.exe", "/c " + command) { CreateNoWindow = false };
            Process.Start(pi);
        }
    }

    public class Thing
    {
        public void Go()
        {
            var iotConnectionString = "{iotHub xon string}";
            var sbConnectionString = "{sb/eh connection string}";

            var senders = new List<Sender>()
            {
                new IotSender(iotConnectionString),
                new EventHubSender(sbConnectionString)
            };

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

            Program.StartReaders(jsonSerializer ? "json" : "protobuf");

            Console.WriteLine("How many messages to send? default 10");
            var iterations = 10;
            var iterationAnswer = Console.ReadLine();
            if (!string.IsNullOrEmpty(iterationAnswer))
            {
                int.TryParse(iterationAnswer, out iterations);
            }

            Console.WriteLine($" === Using {(jsonSerializer ? "JSON" : "Protobuf")} serializer, sending {iterations} items ====");

            for (var i = 0; i < iterations; i++)
            {
                var data = new PingData()
                {
                    DeviceId = Guid.NewGuid().ToString(),
                    Message = "Value x y z",
                    PingTime = DateTime.UtcNow
                };

                byte[] byteData;

                if (jsonSerializer)
                {
                    var j = JsonConvert.SerializeObject(data);
                    byteData = System.Text.Encoding.UTF8.GetBytes(j);
                }
                else
                {
                    using (var ms = new MemoryStream())
                    {
                        ProtoBuf.Serializer.Serialize(ms, data);
                        byteData = ms.ToArray();
                    }
                }

                foreach (var sender in senders)
                {
                    sender.Send(byteData).Wait();
                }
            }
        }
    }
}