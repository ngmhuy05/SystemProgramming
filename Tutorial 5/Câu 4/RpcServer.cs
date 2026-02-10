using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace SimpleRPC
{
    // Request structure
    public class RpcRequest
    {
        public string MethodName { get; set; }
        public JsonElement Parameters { get; set; }
    }

    // Response structure
    public class RpcResponse
    {
        public bool Success { get; set; }
        public object Result { get; set; }
        public string Error { get; set; }
    }

    class RPCServer
    {
        private const int PORT = 5000;

        static void Main(string[] args)
        {
            Console.WriteLine("RPC Server Starting...");

            TcpListener listener = new TcpListener(IPAddress.Loopback, PORT);
            listener.Start();
            Console.WriteLine($"Server listening on port {PORT}...");

            while (true)
            {
                try
                {
                    // Accept client connection
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("\nClient connected!");

                    NetworkStream stream = client.GetStream();

                    // Read request
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string requestJson = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received request: {requestJson}");

                    // Deserialize request
                    RpcRequest request = JsonSerializer.Deserialize<RpcRequest>(requestJson);

                    // Process request and execute method
                    RpcResponse response = ProcessRequest(request);

                    // Serialize and send response
                    string responseJson = JsonSerializer.Serialize(response);
                    byte[] responseData = Encoding.UTF8.GetBytes(responseJson);
                    stream.Write(responseData, 0, responseData.Length);
                    Console.WriteLine($"Sent response: {responseJson}");

                    client.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        static RpcResponse ProcessRequest(RpcRequest request)
        {
            try
            {
                if (request.MethodName == "MoneyExchange")
                {
                    // Extract parameters
                    string currency = request.Parameters.GetProperty("currency").GetString();
                    double amount = request.Parameters.GetProperty("amount").GetDouble();

                    // Execute the function
                    double result = MoneyExchange(currency, amount);

                    return new RpcResponse
                    {
                        Success = true,
                        Result = result,
                        Error = null
                    };
                }
                else
                {
                    return new RpcResponse
                    {
                        Success = false,
                        Result = null,
                        Error = $"Unknown method: {request.MethodName}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new RpcResponse
                {
                    Success = false,
                    Result = null,
                    Error = ex.Message
                };
            }
        }

        // Remote function implementation
        static double MoneyExchange(string currency, double amount)
        {
            Console.WriteLine($"\nExecuting MoneyExchange({currency}, {amount})");

            // Exchange rates (USD base)
            var rates = new System.Collections.Generic.Dictionary<string, double>
            {
                { "USD", 1.0 },
                { "EUR", 0.92 },
                { "GBP", 0.79 },
                { "JPY", 149.50 },
                { "VND", 24500.0 }
            };

            if (!rates.ContainsKey(currency.ToUpper()))
            {
                throw new ArgumentException($"Unsupported currency: {currency}");
            }

            double result = amount * rates[currency.ToUpper()];
            Console.WriteLine($"Result: {amount} USD = {result} {currency}");

            return result;
        }
    }
}