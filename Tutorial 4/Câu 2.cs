using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Cau2
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("=== Task Coordination Demo ===\n");

        // Cách 1: Task.WhenAll
        TestTaskWhenAll();

        Console.WriteLine("\n" + new string('-', 50) + "\n");

        // Cách 2: CountdownEvent
        TestCountdownEvent();

        Console.WriteLine("\n" + new string('-', 50) + "\n");

        // Cách 3: ManualResetEventSlim
        TestManualResetEvent();

        Console.ReadLine();
    }

    static void TestTaskWhenAll()
    {
        Console.WriteLine("Sử dụng Task.WhenAll:");

        Task task1 = SimulateWork("Task 1", 1000);
        Task task2 = SimulateWork("Task 2", 1500);
        Task task3 = SimulateWork("Task 3", 2000);

        // Đợi tất cả tasks hoàn thành
        Task.WhenAll(task1, task2, task3).Wait();

        Console.WriteLine("✓ Tất cả tasks đã hoàn thành!");
    }

    static void TestCountdownEvent()
    {
        Console.WriteLine("Sử dụng CountdownEvent:");

        using (CountdownEvent countdown = new CountdownEvent(3))
        {
            Task.Run(() =>
            {
                SimulateWork("Task 1", 1000).Wait();
                countdown.Signal();
            });

            Task.Run(() =>
            {
                SimulateWork("Task 2", 1500).Wait();
                countdown.Signal();
            });

            Task.Run(() =>
            {
                SimulateWork("Task 3", 2000).Wait();
                countdown.Signal();
            });

            countdown.Wait(); // Đợi cho đến khi count = 0
        }

        Console.WriteLine("✓ Tất cả tasks đã hoàn thành!");
    }

    static void TestManualResetEvent()
    {
        Console.WriteLine("Sử dụng ManualResetEventSlim:");

        using (ManualResetEventSlim resetEvent = new ManualResetEventSlim(false))
        {
            int completedCount = 0;
            object lockObj = new object();

            Task.Run(() =>
            {
                SimulateWork("Task 1", 1000).Wait();
                lock (lockObj)
                {
                    completedCount++;
                    if (completedCount == 3) resetEvent.Set();
                }
            });

            Task.Run(() =>
            {
                SimulateWork("Task 2", 1500).Wait();
                lock (lockObj)
                {
                    completedCount++;
                    if (completedCount == 3) resetEvent.Set();
                }
            });

            Task.Run(() =>
            {
                SimulateWork("Task 3", 2000).Wait();
                lock (lockObj)
                {
                    completedCount++;
                    if (completedCount == 3) resetEvent.Set();
                }
            });

            resetEvent.Wait(); // Đợi signal
        }

        Console.WriteLine("✓ Tất cả tasks đã hoàn thành!");
    }

    static async Task SimulateWork(string taskName, int milliseconds)
    {
        Console.WriteLine($"  {taskName} bắt đầu...");
        await Task.Delay(milliseconds);
        Console.WriteLine($"  {taskName} hoàn thành!");
    }

}
