using System;
using System.Threading;

namespace ProcessA_Producer
{
    class ProcessA_Producer
    {
        // Biến data trong memory của Process A
        private static string sharedData = "Secret data from Process A";
        private static int counter = 0;

        static void Main(string[] args)
        {
            Console.Title = "Process A - Producer";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("     PROCESS A - DATA PRODUCER          ");
            Console.ResetColor();

            Console.WriteLine($"\nProcess ID: {System.Diagnostics.Process.GetCurrentProcess().Id}");
            Console.WriteLine($"Memory Address of 'sharedData': 0x{GetMemoryAddress(sharedData):X}");
            Console.WriteLine($"\nData stored in Process A's memory:");
            Console.WriteLine($"  sharedData = \"{sharedData}\"");
            Console.WriteLine($"  counter = {counter}");

            Console.WriteLine("\n" + new string('─', 50));
            Console.WriteLine("Process A is now producing data...");
            Console.WriteLine(new string('─', 50));

            // Liên tục produce data
            while (true)
            {
                counter++;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Producing data... Counter = {counter}");
                Thread.Sleep(2000);

                if (counter % 5 == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n⚠ Process B CANNOT access this data directly!");
                    Console.WriteLine("⚠ This data is isolated in Process A's memory space!");
                    Console.ResetColor();
                }
            }
        }

        // Giả lập lấy memory address (chỉ để demo)
        static long GetMemoryAddress(object obj)
        {
            return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        }
    }
}