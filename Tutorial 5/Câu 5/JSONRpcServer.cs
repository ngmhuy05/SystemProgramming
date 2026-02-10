using System;
using System.Net;
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
        public JsonElement @params { get; set; }
        public object id { get; set; }
    }

    // JSON-RPC 2.0 Response structure
    public class JsonRpcResponse
    {
        public string jsonrpc { get; set; } = "2.0";
        public object result { get; set; }
        public JsonRpcError error { get; set; }
        public object id { get; set; }
    }

    // JSON-RPC Error structure
    public class JsonRpcError
    {
        public int code { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }

    class JSONRPCServer
    {
        private const int PORT = 5000;

        // Error codes (JSON-RPC 2.0 standard)
        private const int PARSE_ERROR = -32700;
        private const int INVALID_REQUEST = -32600;
        private const int METHOD_NOT_FOUND = -32601;
        private const int INVALID_PARAMS = -32602;
        private const int INTERNAL_ERROR = -32603;

        static void Main(string[] args)
        {
            Console.WriteLine("JSON-RPC 2.0 Server Starting...");
            Console.WriteLine("Supported methods:");
            Console.WriteLine("  1. MoneyExchange(currency, amount)");
            Console.WriteLine("  2. CalculateInterest(principal, rate, years)");
            Console.WriteLine();

            TcpListener listener = new TcpListener(IPAddress.Loopback, PORT);
            listener.Start();
            Console.WriteLine($"Server listening on port {PORT}...\n");

            while (true)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("═══════════════════════════════════");
                    Console.WriteLine("Client connected!");

                    NetworkStream stream = client.GetStream();

                    // Read request
                    byte[] buffer = new byte[2048];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string requestJson = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received: {requestJson}");

                    // Process request
                    JsonRpcResponse response = ProcessJsonRpcRequest(requestJson);

                    // Send response
                    string responseJson = JsonSerializer.Serialize(response);
                    byte[] responseData = Encoding.UTF8.GetBytes(responseJson);
                    stream.Write(responseData, 0, responseData.Length);
                    Console.WriteLine($"Sent: {responseJson}");
                    Console.WriteLine("═══════════════════════════════════\n");

                    client.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Server Error: {ex.Message}\n");
                }
            }
        }

        static JsonRpcResponse ProcessJsonRpcRequest(string requestJson)
        {
            try
            {
                // Parse request
                JsonRpcRequest request = JsonSerializer.Deserialize<JsonRpcRequest>(requestJson);

                // Validate JSON-RPC version
                if (request.jsonrpc != "2.0")
                {
                    return CreateErrorResponse(null, INVALID_REQUEST,
                        "Invalid JSON-RPC version. Must be '2.0'");
                }

                // Validate request ID exists
                if (request.id == null)
                {
                    return CreateErrorResponse(null, INVALID_REQUEST,
                        "Missing request ID");
                }

                // Route to appropriate method
                switch (request.method)
                {
                    case "MoneyExchange":
                        return HandleMoneyExchange(request);

                    case "CalculateInterest":
                        return HandleCalculateInterest(request);

                    default:
                        return CreateErrorResponse(request.id, METHOD_NOT_FOUND,
                            $"Method '{request.method}' not found");
                }
            }
            catch (JsonException ex)
            {
                return CreateErrorResponse(null, PARSE_ERROR,
                    "Parse error: " + ex.Message);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(null, INTERNAL_ERROR,
                    "Internal error: " + ex.Message);
            }
        }

        static JsonRpcResponse HandleMoneyExchange(JsonRpcRequest request)
        {
            try
            {
                // Extract and validate parameters
                if (!request.@params.TryGetProperty("currency", out JsonElement currencyElement) ||
                    !request.@params.TryGetProperty("amount", out JsonElement amountElement))
                {
                    return CreateErrorResponse(request.id, INVALID_PARAMS,
                        "Missing required parameters: 'currency' and 'amount'");
                }

                string currency = currencyElement.GetString();
                double amount = amountElement.GetDouble();

                Console.WriteLine($"→ Executing MoneyExchange({currency}, {amount})");

                // Execute method
                double result = MoneyExchange(currency, amount);

                Console.WriteLine($"→ Result: {amount} USD = {result} {currency}");

                // Return success response
                return new JsonRpcResponse
                {
                    result = new
                    {
                        fromCurrency = "USD",
                        toCurrency = currency,
                        amount = amount,
                        convertedAmount = result,
                        exchangeRate = result / amount
                    },
                    id = request.id
                };
            }
            catch (ArgumentException ex)
            {
                return CreateErrorResponse(request.id, INVALID_PARAMS, ex.Message);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(request.id, INTERNAL_ERROR, ex.Message);
            }
        }

        static JsonRpcResponse HandleCalculateInterest(JsonRpcRequest request)
        {
            try
            {
                // Extract and validate parameters
                if (!request.@params.TryGetProperty("principal", out JsonElement principalElement) ||
                    !request.@params.TryGetProperty("rate", out JsonElement rateElement) ||
                    !request.@params.TryGetProperty("years", out JsonElement yearsElement))
                {
                    return CreateErrorResponse(request.id, INVALID_PARAMS,
                        "Missing required parameters: 'principal', 'rate', and 'years'");
                }

                double principal = principalElement.GetDouble();
                double rate = rateElement.GetDouble();
                int years = yearsElement.GetInt32();

                Console.WriteLine($"→ Executing CalculateInterest({principal}, {rate}, {years})");

                // Validate business logic
                if (principal <= 0)
                    throw new ArgumentException("Principal must be positive");
                if (rate < 0)
                    throw new ArgumentException("Rate cannot be negative");
                if (years <= 0)
                    throw new ArgumentException("Years must be positive");

                // Execute method
                var result = CalculateInterest(principal, rate, years);

                Console.WriteLine($"→ Result: Total = {result.totalAmount:F2}");

                // Return success response
                return new JsonRpcResponse
                {
                    result = result,
                    id = request.id
                };
            }
            catch (ArgumentException ex)
            {
                return CreateErrorResponse(request.id, INVALID_PARAMS, ex.Message);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(request.id, INTERNAL_ERROR, ex.Message);
            }
        }

        // Business logic methods
        static double MoneyExchange(string currency, double amount)
        {
            var rates = new System.Collections.Generic.Dictionary<string, double>
            {
                { "USD", 1.0 },
                { "EUR", 0.92 },
                { "GBP", 0.79 },
                { "JPY", 149.50 },
                { "VND", 24500.0 },
                { "CNY", 7.24 }
            };

            if (!rates.ContainsKey(currency.ToUpper()))
            {
                throw new ArgumentException($"Unsupported currency: {currency}");
            }

            return amount * rates[currency.ToUpper()];
        }

        static dynamic CalculateInterest(double principal, double rate, int years)
        {
            // Simple interest formula: A = P(1 + rt)
            double interest = principal * (rate / 100) * years;
            double totalAmount = principal + interest;

            return new
            {
                principal = principal,
                interestRate = rate,
                years = years,
                interestEarned = Math.Round(interest, 2),
                totalAmount = Math.Round(totalAmount, 2)
            };
        }

        static JsonRpcResponse CreateErrorResponse(object id, int code, string message)
        {
            Console.WriteLine($"→ Error: [{code}] {message}");

            return new JsonRpcResponse
            {
                error = new JsonRpcError
                {
                    code = code,
                    message = message
                },
                id = id
            };
        }
    }
}