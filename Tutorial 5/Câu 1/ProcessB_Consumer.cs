using System;
using System.Threading;

namespace ProcessB_Consumer
{
    class ProcessB_Consumer
    {
        static void Main(string[] args)
        {
            Console.Title = "Process B - Consumer";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("     PROCESS B - DATA CONSUMER          ");
            Console.ResetColor();

            Console.WriteLine($"\nProcess ID: {System.Diagnostics.Process.GetCurrentProcess().Id}");
            Console.WriteLine("\nProcess B is trying to access data from Process A...");
            Console.WriteLine(new string('─', 50));

            // Cố gắng access data từ Process A (sẽ FAIL)
            Thread.Sleep(1000);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n❌ ERROR: Cannot access Process A's memory!");
            Console.WriteLine("❌ Variable 'sharedData' does not exist in this process!");
            Console.WriteLine("❌ Process isolation prevents direct memory access!");
            Console.ResetColor();

            Console.WriteLine("\n" + new string('─', 50));
            Console.WriteLine("Why Process B cannot access Process A's data:");
            Console.WriteLine(new string('─', 50));
            Console.WriteLine("1. Each process has its own VIRTUAL MEMORY SPACE");
            Console.WriteLine("2. Operating System enforces MEMORY PROTECTION");
            Console.WriteLine("3. Processes are ISOLATED from each other");
            Console.WriteLine("4. Direct memory access would cause ACCESS VIOLATION");

            Console.WriteLine("\n" + new string('─', 50));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("💡 SOLUTION: Use IPC mechanisms!");
            Console.ResetColor();
            Console.WriteLine(new string('─', 50));

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}