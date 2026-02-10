using System;

class Program
{
    static void Main()
    {
        long memBefore = GC.GetTotalMemory(false);
        Console.WriteLine($"Memory trước: {memBefore / 1024} KB");

        // Tạo nhiều objects
        for (int i = 0; i < 100000; i++)
        {
            var data = new byte[1000];
        }

        long memAfter = GC.GetTotalMemory(false);
        Console.WriteLine($"Memory sau: {memAfter / 1024} KB");
        Console.WriteLine($"Tăng: {(memAfter - memBefore) / 1024} KB");

        Console.WriteLine($"\nGen 0: {GC.CollectionCount(0)}");
        Console.WriteLine($"Gen 1: {GC.CollectionCount(1)}");
        Console.WriteLine($"Gen 2: {GC.CollectionCount(2)}");

        // Force GC
        GC.Collect();
        GC.WaitForPendingFinalizers();

        long memAfterGC = GC.GetTotalMemory(true);
        Console.WriteLine($"\nMemory sau GC: {memAfterGC / 1024} KB");
        Console.WriteLine($"Giai phóng: {(memAfter - memAfterGC) / 1024} KB");
        Console.ReadLine();
    }
}