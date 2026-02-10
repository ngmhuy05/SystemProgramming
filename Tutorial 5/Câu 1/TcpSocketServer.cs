using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpSocketServer
{
    class TcpSocketServer
    {
        static void Main(string[] args)
        {
            Console.Title = "TCP Socket Server";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("      TCP SOCKET SERVER                 ");
            Console.ResetColor();

            int port = 5000;
            TcpListener server = null;
            TcpClient client = null;
            NetworkStream stream = null;

            try
            {
                // Tạo TCP Server lắng nghe trên localhost:5000
                server = new TcpListener(IPAddress.Loopback, port);
                server.Start();

                Console.WriteLine($"\nServer started on {IPAddress.Loopback}:{port}");
                Console.WriteLine("Waiting for client connection...\n");

                // Chờ client connect (blocking call)
                client = server.AcceptTcpClient();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Client connected!");
                Console.ResetColor();

                // Lấy network stream để đọc/ghi data
                stream = client.GetStream();

                // Đọc request từ client
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                Console.WriteLine($"\n[RECEIVED] Request from client:");
                Console.WriteLine($"  Message: \"{request}\"");
                Console.WriteLine($"  Bytes: {bytesRead}");
                Console.WriteLine($"  Time: {DateTime.Now:HH:mm:ss}");

                // Xử lý request (ví dụ: chuyển thành uppercase)
                string processedData = request.ToUpper();
                Console.WriteLine($"\n[PROCESSING] Converting to uppercase...");
                Console.WriteLine($"  Result: \"{processedData}\"");

                // Gửi response về client
                string response = $"SERVER RESPONSE: {processedData} (Processed at {DateTime.Now:HH:mm:ss})";
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                stream.Write(responseBytes, 0, responseBytes.Length);

                Console.WriteLine($"\n[SENT] Response sent to client:");
                Console.WriteLine($"  Message: \"{response}\"");
                Console.WriteLine($"  Bytes: {responseBytes.Length}");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n✓ Communication completed successfully!");
                Console.ResetColor();

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ Error: {ex.Message}");
                Console.ResetColor();
            }
            finally
            {
                // Cleanup resources
                if (stream != null) stream.Close();
                if (client != null) client.Close();
                if (server != null) server.Stop();

                Console.WriteLine("\nServer shutting down...");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}