# azure-http-hub - WIP
Simple, sample HTTP ingest-only clients for Azure Event Hub and IoT Hub

# In progress
Main project needs some cleanup

# Why?
In some cases, switching between Event Hub and IoT Hub for _sending_ data to your cloud services is a requirement. In my case, a customer was interested in IoT Hub but there were some regional availability issues - rather than abandon IoT Hub entirely, it made sense to build a simple sender that used EH and IoTH's HTTP endpoints for inbound messages, without using any pre-built SDKs or clients. This meant the implementation could be switched quickly without republishing code to thousands or millions of devices.

In addition, as part of a larger ingestion solution, this pattern may make sense depending on where the devices are located, data sovereignty rules, etc.

## Should I use this?
Probably not. This is intended as a sample of talking via HTTP to the Azure Hubs. It hasn't been tested or written with a production scope in mind, although should be sufficient for proving a primitive path for sending messages to one of the Azure Hubs. Event and IoT Hub have some [pretty significant differences](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-compare-event-hubs) from a functionality and manageability standpoint, particularly:
- OOTB Two-way communication (IoT Hub only) - no extra queues or cloud-to-device schemes to cook up
- Granular, per-device security (IoT Hub only) - Event Hub policies are far fewer in number than IoT Hub, which is fine in many ingestion cases. However, if you need per-devices registration and keys, IoT Hub supports that while EH will not.
- A different protocol beyond HTTP - this code only sends `HTTP POST` requests. If you're using MQTT or AMQP, there's nothing here for you. Maybe one day. Event Hubs have limitations on the number of AMQP connections relative to IoT Hub, and MQTT is unavailable for EH.
- IoT Hub has lots of extra sugar for manageability, like device twins and broader protocol support.

## Code notes
- The `Sender` class makes a basic `HTTP POST` with a Shared Access Signature in the `Authorization` header. 
- `Sender` also does rudimentary connection string parsing.
- The `Sender.GetEndpoint()` method is used to build the endpoint address depending on the `Sender` derived type. Each type implements its own endpoint generator, but that is the extent of the difference. This could easily be moved somewhere else or to a single type.
- Event and IoT Hub have slightly different endpoint formats;
  - Event Hub: `https://<sb namespace>.servicebus.windows.net/<EH name>/publishers/<Key/Policy Id>/messages/events`
  - IoT Hub: `https://<iot hub name>.azure-devices.net/devices/<Key or Device Id>/messages/events`
- There are two serialization options implemented, one using JSON.Net and the other using protobuf-net.
-Therearetwoclientsin`EventHubClientProcessor`forreadinganddeserializingtheoutputfromboththeIoTHubandtheEventHub.Noticethesame`EventProcessorHost`canreadfrombothIoTHubandEventHubwithoutchanges.+
