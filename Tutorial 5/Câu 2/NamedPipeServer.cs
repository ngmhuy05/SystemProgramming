using System;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace NamedPipeServer
{
    class NamedPipeServer
    {
        static void Main(string[] args)
        {
            Console.Title = "Named Pipe Server";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("      NAMED PIPE SERVER                 ");
            Console.ResetColor();

            string pipeName = "MyTestPipe";
            Console.WriteLine($"\nServer starting...");
            Console.WriteLine($"Pipe Name: {pipeName}");
            Console.WriteLine($"Waiting for client connection...\n");

            NamedPipeServerStream pipeServer = null;
            StreamReader reader = null;
            StreamWriter writer = null;

            try
            {
                // Tạo Named Pipe Server
                pipeServer = new NamedPipeServerStream(
                    pipeName,
                    PipeDirection.InOut,
                    1); // Max 1 client

                // Chờ client connect
                pipeServer.WaitForConnection();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✓ Client connected!");
                Console.ResetColor();

                // Đọc message từ client
                reader = new StreamReader(pipeServer, Encoding.UTF8);
                string clientMessage = reader.ReadLine();

                Console.WriteLine($"\n[RECEIVED] Client says: \"{clientMessage}\"");
                Console.WriteLine($"[TIME] {DateTime.Now:HH:mm:ss}");

                // Gửi response về client
                writer = new StreamWriter(pipeServer, Encoding.UTF8);
                writer.AutoFlush = true;

                string response = $"Hello Client! Server received your message at {DateTime.Now:HH:mm:ss}";
                writer.WriteLine(response);

                Console.WriteLine($"\n[SENT] Response: \"{response}\"");

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
                if (reader != null) reader.Dispose();
                if (writer != null) writer.Dispose();
                if (pipeServer != null) pipeServer.Dispose();

                Console.WriteLine("\nServer shutting down...");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}