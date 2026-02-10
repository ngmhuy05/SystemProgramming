using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace JSONRPC
{
    // JSON-RPC 2.0 Request structure
    public class JsonRpcRequest
    {
        public string jsonrpc { get; set; } = "2.0";
        public string method { get; set; }
        public object @params { get; set; }
        public object id { get; set; }
    }

    // JSON-RPC 2.0 Response structure
    public class JsonRpcResponse
    {
        public string jsonrpc { get; set; }
        public JsonElement result { get; set; }
        public JsonRpcError error { get; set; }
        public JsonElement id { get; set; }
    }

    // JSON-RPC Error structure
    public class JsonRpcError
    {
        public int code { get; set; }
        public string message { get; set; }
        public JsonElement data { get; set; }
    }

    class JSONRPCClient
    {
        private const string SERVER_IP = "127.0.0.1";
        private const int SERVER_PORT = 5000;
        private static int requestIdCounter = 1;

        static void Main(string[] args)
        {
            Console.WriteLine("JSON-RPC 2.0 Client Starting...");
            Console.WriteLine("═══════════════════════════════════\n");

            // Test 1: Valid MoneyExchange request
            Console.WriteLine("TEST 1: MoneyExchange - Valid Request");
            SendRequest("MoneyExchange", new { currency = "EUR", amount = 100.0 });

            Console.WriteLine("\n═══════════════════════════════════\n");

            // Test 2: Valid CalculateInterest request
            Console.WriteLine("TEST 2: CalculateInterest - Valid Request");
            SendRequest("CalculateInterest", new { principal = 10000.0, rate = 5.5, years = 3 });

            Console.WriteLine("\n═══════════════════════════════════\n");

            // Test 3: Invalid method name
            Console.WriteLine("TEST 3: Invalid Method Name");
            SendRequest("GetWeather", new { city = "Hanoi" });

            Console.WriteLine("\n═══════════════════════════════════\n");

            // Test 4: Missing parameters
            Console.WriteLine("TEST 4: Missing Parameters");
            SendRequest("MoneyExchange", new { amount = 50.0 }); // missing 'currency'

            Console.WriteLine("\n═══════════════════════════════════\n");

            // Test 5: Invalid parameter values
            Console.WriteLine("TEST 5: Invalid Parameter Values");
            SendRequest("CalculateInterest", new { principal = -1000.0, rate = 5.0, years = 2 });

            Console.WriteLine("\n═══════════════════════════════════");
            Console.WriteLine("\nAll tests completed. Press any key to exit...");
            Console.ReadKey();
        }

        static void SendRequest(string method, object parameters)
        {
            try
            {
                // Create JSON-RPC request
                var request = new JsonRpcRequest
                {
                    method = method,
                    @params = parameters,
                    id = requestIdCounter++
                };

                // Connect to server
                TcpClient client = new TcpClient(SERVER_IP, SERVER_PORT);
                NetworkStream stream = client.GetStream();

                // Serialize and send request
                string requestJson = JsonSerializer.Serialize(request);
                Console.WriteLine($"Sending: {requestJson}");

                byte[] requestData = Encoding.UTF8.GetBytes(requestJson);
                stream.Write(requestData, 0, requestData.Length);

                // Read response
                byte[] buffer = new byte[2048];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string responseJson = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {responseJson}");

                // Deserialize response
                JsonRpcResponse response = JsonSerializer.Deserialize<JsonRpcResponse>(responseJson);

                // Display result
                Console.WriteLine("\n--- Response Details ---");

                if (response.error != null)
                {
                    // Error response
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"❌ ERROR");
                    Console.WriteLine($"   Code: {response.error.code}");
                    Console.WriteLine($"   Message: {response.error.message}");
                    Console.ResetColor();
                }
                else
                {
                    // Success response
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"✓ SUCCESS");
                    Console.WriteLine($"   Request ID: {response.id}");
                    Console.WriteLine($"   Result: {response.result}");
                    Console.ResetColor();

                    // Pretty print result based on method
                    if (method == "MoneyExchange" && response.result.ValueKind == JsonValueKind.Object)
                    {
                        var result = response.result;
                        Console.WriteLine($"\n   Conversion Details:");
                        Console.WriteLine($"   - From: {result.GetProperty("amount")} {result.GetProperty("fromCurrency")}");
                        Console.WriteLine($"   - To: {result.GetProperty("convertedAmount")} {result.GetProperty("toCurrency")}");
                        Console.WriteLine($"   - Rate: {result.GetProperty("exchangeRate")}");
                    }
                    else if (method == "CalculateInterest" && response.result.ValueKind == JsonValueKind.Object)
                    {
                        var result = response.result;
                        Console.WriteLine($"\n   Interest Calculation:");
                        Console.WriteLine($"   - Principal: ${result.GetProperty("principal")}");
                        Console.WriteLine($"   - Rate: {result.GetProperty("interestRate")}%");
                        Console.WriteLine($"   - Years: {result.GetProperty("years")}");
                        Console.WriteLine($"   - Interest Earned: ${result.GetProperty("interestEarned")}");
                        Console.WriteLine($"   - Total Amount: ${result.GetProperty("totalAmount")}");
                    }
                }

                client.Close();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Client Error: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}