using System;
using AzureHttpHub.Sas;

namespace AzureHttpHub
{
    public class IotSender : Sender
    {
        private const string ApiVersion = "2016-11-14";

        public IotSender(string connectionString) : base(connectionString)
        {
            Path = "/devices";

            var a = new SharedAccessSignatureBuilder()
            {
                TargetService = "iothub",
                Key = SharedAccessKey,
                KeyName = KeyId,
                Target = $"{HostName}{Path}/{KeyId}",
                TimeToLive = TimeSpan.FromDays(1)
            };
            SasToken = a.ToSignature();
        }

        internal override string GetEndpoint()
        {
            return $"https://{HostName}{Path}/{KeyId}/messages/events?api-version={ApiVersion}";
        }
    }
}