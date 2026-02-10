using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace SimpleRPC
{
    // Request structure (same as server)
    public class RpcRequest
    {
        public string MethodName { get; set; }
        public object Parameters { get; set; }
    }

    // Response structure (same as server)
    public class RpcResponse
    {
        public bool Success { get; set; }
        public JsonElement Result { get; set; }
        public string Error { get; set; }
    }

    class RPCClient
    {
        private const string SERVER_IP = "127.0.0.1";
        private const int SERVER_PORT = 5000;

        static void Main(string[] args)
        {
            Console.WriteLine("RPC Client Starting...\n");

            try
            {
                // Connect to server
                TcpClient client = new TcpClient(SERVER_IP, SERVER_PORT);
                Console.WriteLine($"Connected to server at {SERVER_IP}:{SERVER_PORT}");

                NetworkStream stream = client.GetStream();

                // Create RPC request
                var request = new RpcRequest
                {
                    MethodName = "MoneyExchange",
                    Parameters = new
                    {
                        currency = "VND",
                        amount = 100.0
                    }
                };

                // Serialize request
                string requestJson = JsonSerializer.Serialize(request);
                Console.WriteLine($"\nSending request: {requestJson}");

                // Send request
                byte[] requestData = Encoding.UTF8.GetBytes(requestJson);
                stream.Write(requestData, 0, requestData.Length);

                // Read response
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string responseJson = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received response: {responseJson}");

                // Deserialize response
                RpcResponse response = JsonSerializer.Deserialize<RpcResponse>(responseJson);

                // Display result
                Console.WriteLine("\n--- Result ---");
                if (response.Success)
                {
                    Console.WriteLine($"Success: {response.Success}");
                    Console.WriteLine($"Result: {response.Result}");
                    Console.WriteLine($"\nConversion: 100.0 USD = {response.Result} VND");
                }
                else
                {
                    Console.WriteLine($"Error: {response.Error}");
                }

                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}