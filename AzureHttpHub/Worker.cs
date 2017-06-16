using System;
using System.Collections.Generic;
using AzureHttpHub.Model;

namespace AzureHttpHub
{
    public class Worker
    {
        public void Go(List<Sender> senders, Func<PingData, byte[]> serialize, int iterations = 10)
        {
            for (var i = 0; i < iterations; i++)
            {
                var data = new PingData()
                {
                    DeviceId = Guid.NewGuid().ToString(),
                    Message = "Value x y z",
                    PingTime = DateTime.UtcNow
                };

                var byteData = serialize(data);

                foreach (var sender in senders)
                {
                    sender.Send(byteData).Wait();
                }
            }
        }
    }
}