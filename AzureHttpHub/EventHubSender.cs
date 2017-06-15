using System;
using AzureHttpHub.Sas;

namespace AzureHttpHub
{
    public class EventHubSender : Sender
    {
        public EventHubSender(string connectionString) : base(connectionString)
        {
            var a = new SharedAccessSignatureBuilder()
            {
                TargetService = "servicebus",
                Key = SharedAccessKey,
                KeyName = KeyId,
                Target = HostName,
                TimeToLive = TimeSpan.FromDays(1)
            };
            SasToken = a.ToSignature();
        }

        internal override string GetEndpoint()
        {
            return $"https://{HostName}{Path}/publishers/{KeyId}/messages";
        }
    }
}