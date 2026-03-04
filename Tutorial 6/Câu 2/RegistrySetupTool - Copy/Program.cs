using System;
using TradingService;

namespace RegistrySetupTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Trading Service - Registry Setup Tool";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║  REGISTRY SETUP TOOL                   ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.ResetColor();

            Console.WriteLine("\nThis tool will create registry configuration for Trading Service");
            Console.WriteLine("Registry Path: HKEY_LOCAL_MACHINE\\SOFTWARE\\TradingService");
            Console.WriteLine("\n⚠ Make sure you run this as Administrator!\n");

            Console.Write("Press ENTER to create registry keys, or ESC to cancel...");
            var key = Console.ReadKey();
            Console.WriteLine();

            if (key.Key == ConsoleKey.Escape)
            {
                Console.WriteLine("Cancelled.");
                return;
            }

            Console.WriteLine();
            RegistryConfigHelper.CreateRegistryConfiguration();

            Console.WriteLine("\n────────────────────────────────────────");
            Console.WriteLine("You can now verify in Registry Editor:");
            Console.WriteLine("1. Press Win+R");
            Console.WriteLine("2. Type: regedit");
            Console.WriteLine("3. Navigate to: HKEY_LOCAL_MACHINE\\SOFTWARE\\TradingService");
            Console.WriteLine("────────────────────────────────────────");

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}