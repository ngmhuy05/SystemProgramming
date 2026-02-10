using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TcpSocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "TCP Socket Client";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("      TCP SOCKET CLIENT                 ");
            Console.ResetColor();

            string serverIP = "127.0.0.1";  // localhost
            int port = 5000;

            Console.WriteLine($"\nClient starting...");
            Console.WriteLine($"Target: {serverIP}:{port}");

            // Đợi 2 giây để đảm bảo server đã khởi động
            Console.WriteLine("Waiting for server to start...\n");
            Thread.Sleep(2000);

            TcpClient client = null;
            NetworkStream stream = null;

            try
            {
                // Kết nối tới TCP Server
                Console.WriteLine("Connecting to server...");
                client = new TcpClient(serverIP, port);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Connected to server!");
                Console.ResetColor();

                // Lấy network stream
                stream = client.GetStream();

                // Gửi request tới server
                string message = "Hello from TCP Client! Please process this message.";
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);

                Console.WriteLine($"\n[SENT] Request to server:");
                Console.WriteLine($"  Message: \"{message}\"");
                Console.WriteLine($"  Bytes: {data.Length}");
                Console.WriteLine($"  Time: {DateTime.Now:HH:mm:ss}");

                // Đọc response từ server
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                Console.WriteLine($"\n[RECEIVED] Response from server:");
                Console.WriteLine($"  Message: \"{response}\"");
                Console.WriteLine($"  Bytes: {bytesRead}");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n✓ Communication completed successfully!");
                Console.ResetColor();

            }
            catch (SocketException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ Connection Error: {ex.Message}");
                Console.WriteLine("Make sure the server is running first!");
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

                Console.WriteLine("\nClient shutting down...");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}