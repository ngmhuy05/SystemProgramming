using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("=== Test với Thread ===");
        Stopwatch sw1 = Stopwatch.StartNew();
        UseThread();
        sw1.Stop();
        Console.WriteLine($"Thời gian: {sw1.ElapsedMilliseconds}ms\n");

        Thread.Sleep(1000);

        Console.WriteLine("=== Test với ThreadPool ===");
        Stopwatch sw2 = Stopwatch.StartNew();
        UseThreadPool();
        sw2.Stop();
        Console.WriteLine($"Thời gian: {sw2.ElapsedMilliseconds}ms\n");

        Thread.Sleep(1000);

        Console.WriteLine("=== Test với Task ===");
        Stopwatch sw3 = Stopwatch.StartNew();
        UseTask();
        sw3.Stop();
        Console.WriteLine($"Thời gian: {sw3.ElapsedMilliseconds}ms");

        Console.ReadLine();
    }

    static void UseThread()
    {
        Thread[] threads = new Thread[100];
        for (int i = 0; i < 100; i++)
        {
            int id = i;
            threads[i] = new Thread(() => DoWork(id));
            threads[i].Start();
        }
        foreach (var t in threads) t.Join();
    }

    static void UseThreadPool()
    {
        using (var countdown = new CountdownEvent(100))
        {
            for (int i = 0; i < 100; i++)
            {
                int id = i;
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    DoWork(id);
                    countdown.Signal();
                });
            }
            countdown.Wait();
        }
    }

    static void UseTask()
    {
        Task[] tasks = new Task[100];
        for (int i = 0; i < 100; i++)
        {
            int id = i;
            tasks[i] = Task.Run(() => DoWork(id));
        }
        Task.WaitAll(tasks);
    }

    static void DoWork(int id)
    {
        Thread.Sleep(10);
        if (id % 20 == 0)
        {
            Console.WriteLine($"Work {id} - Thread ID: {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}