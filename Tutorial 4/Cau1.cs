using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Cau1
{
    static int counter = 0;
    static int counterWithLock = 0;
    static int counterWithInterlocked = 0;
    static object lockObject = new object();

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("=== Race Condition Demo ===\n");

        // Test KHÔNG có synchronization - SAI
        TestWithoutSync();

        // Test với lock - ĐÚNG
        TestWithLock();

        // Test với Interlocked - ĐÚNG
        TestWithInterlocked();

        Console.ReadLine();
    }

    static void TestWithoutSync()
    {
        counter = 0;
        Task[] tasks = new Task[5];

        for (int i = 0; i < 5; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 10000; j++)
                {
                    counter++; // KHÔNG AN TOÀN
                }
            });
        }

        Task.WaitAll(tasks);
        Console.WriteLine($"WITHOUT Sync: {counter} (Expected: 50000)");
        Console.WriteLine($"→ SAI! Bị mất dữ liệu do race condition\n");
    }

    static void TestWithLock()
    {
        counterWithLock = 0;
        Task[] tasks = new Task[5];

        for (int i = 0; i < 5; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 10000; j++)
                {
                    lock (lockObject)
                    {
                        counterWithLock++; // AN TOÀN
                    }
                }
            });
        }

        Task.WaitAll(tasks);
        Console.WriteLine($"WITH Lock: {counterWithLock} (Expected: 50000)");
        Console.WriteLine($"→ ĐÚNG! Lock đảm bảo thread-safe\n");
    }

    static void TestWithInterlocked()
    {
        counterWithInterlocked = 0;
        Task[] tasks = new Task[5];

        for (int i = 0; i < 5; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 10000; j++)
                {
                    Interlocked.Increment(ref counterWithInterlocked); // AN TOÀN
                }
            });
        }

        Task.WaitAll(tasks);
        Console.WriteLine($"WITH Interlocked: {counterWithInterlocked} (Expected: 50000)");
        Console.WriteLine($"→ ĐÚNG! Interlocked nhanh hơn lock\n");
    }
}