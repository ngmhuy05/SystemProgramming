using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace NamedPipeClient
{
    class NamedPipeClient
    {
        static void Main(string[] args)
        {
            Console.Title = "Named Pipe Client";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("      NAMED PIPE CLIENT                 ");
            Console.ResetColor();

            string pipeName = "MyTestPipe";
            Console.WriteLine($"\nClient starting...");
            Console.WriteLine($"Pipe Name: {pipeName}");
            Console.WriteLine("Connecting to server...\n");

            // Đợi 2 giây để đảm bảo server đã khởi động
            Thread.Sleep(2000);

            NamedPipeClientStream pipeClient = null;
            StreamWriter writer = null;
            StreamReader reader = null;

            try
            {
                // Tạo Named Pipe Client
                pipeClient = new NamedPipeClientStream(
                    ".",           // Server name (. = local machine)
                    pipeName,
                    PipeDirection.InOut);

                // Connect tới server (timeout 5 seconds)
                Console.WriteLine("Attempting to connect...");
                pipeClient.Connect(5000);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Connected to server!");
                Console.ResetColor();

                // Gửi message tới server
                writer = new StreamWriter(pipeClient, Encoding.UTF8);
                writer.AutoFlush = true;

                string message = "Hello Server! This is a message from Client.";
                writer.WriteLine(message);

                Console.WriteLine($"\n[SENT] Message: \"{message}\"");
                Console.WriteLine($"[TIME] {DateTime.Now:HH:mm:ss}");

                // Đọc response từ server
                reader = new StreamReader(pipeClient, Encoding.UTF8);
                string serverResponse = reader.ReadLine();

                Console.WriteLine($"\n[RECEIVED] Server says: \"{serverResponse}\"");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n✓ Communication completed successfully!");
                Console.ResetColor();

            }
            catch (TimeoutException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n❌ Error: Connection timeout!");
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
                if (reader != null) reader.Dispose();
                if (writer != null) writer.Dispose();
                if (pipeClient != null) pipeClient.Dispose();

                Console.WriteLine("\nClient shutting down...");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}