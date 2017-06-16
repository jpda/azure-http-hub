using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AzureHttpHub
{
    public abstract class Sender
    {
        public string HostName { get; set; }

        public string KeyId { get; set; }

        public string SharedAccessKey { get; set; }

        public string SasToken { get; set; }

        public string Path { get; set; }

        public bool IsEventHub => HostName.ToLower().Contains("servicebus.windows.net");

        protected Sender(string connectionString)
        {
            var pieces = connectionString.Split(';');

            foreach (var p in pieces)
            {
                var kvp = p.Split('=');
                if (kvp[0] == "HostName" || kvp[0] == "Endpoint")
                {
                    HostName = kvp[1].Replace("sb://", "");
                    continue;
                }
                if (kvp[0] == "DeviceId" || kvp[0] == "SharedAccessKeyName")
                {
                    KeyId = kvp[1];
                }
                if (kvp[0] == "SharedAccessKey")
                {
                    SharedAccessKey = kvp[1];
                    SharedAccessKey = SharedAccessKey + "=";
                }
                if (kvp[0] == "EntityPath")
                {
                    Path = kvp[1];
                }
            }
            Console.Write("Created ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(GetType().Name);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($", connecting to ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(HostName);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" over HTTPS.");
        }

        public virtual async Task Send(byte[] content)
        {
            var c = new HttpClient();
            c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("SharedAccessSignature", SasToken.Split(' ')[1]);
            try
            {
                var endpoint = GetEndpoint();
                Console.Write($"Sending byte array to {endpoint}...");
                var result = await c.PostAsync(endpoint, new ByteArrayContent(content));
                if (result.IsSuccessStatusCode)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Success");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                Console.WriteLine(await result.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        internal abstract string GetEndpoint();
    }
}