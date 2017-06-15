using System;
using ProtoBuf;

namespace AzureHttpHub.Model
{
    [ProtoContract]
    public class PingData
    {
        [ProtoMember(1)]
        public string DeviceId { get; set; }
        [ProtoMember(2)]
        public DateTime PingTime { get; set; }
        [ProtoMember(3)]
        public string Message { get; set; }
    }
}