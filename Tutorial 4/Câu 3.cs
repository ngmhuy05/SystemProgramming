using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

class Program
{
    static string logFilePath = "log.txt";
    static object fileLock = new object();

    static void Main()
    {
        File.Delete(logFilePath); // Xóa file cũ nếu có

        Console.WriteLine("=== KHÔNG có synchronization ===");
        TestWithoutSync();
        Console.WriteLine($"Kiểm tra file: {logFilePath}\n");
        Thread.Sleep(1000);

        File.Delete(logFilePath);

        Console.WriteLine("=== CÓ lock synchronization ===");
        TestWithLock();
        Console.WriteLine($"Kiểm tra file: {logFilePath}\n");
        Thread.Sleep(1000);

        File.Delete(logFilePath);

        Console.WriteLine("=== Dùng Queue Pattern ===");
        TestWithQueue();
        Console.WriteLine($"Kiểm tra file: {logFilePath}");

        Console.ReadLine();
    }

    // KHÔNG AN TOÀN - Dữ liệu bị corrupt
    static void TestWithoutSync()
    {
        Task[] tasks = new Task[5];

        for (int i = 0; i < 5; i++)
        {
            int taskId = i;
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    // NHIỀU threads ghi cùng lúc → CORRUPT!
                    File.AppendAllText(logFilePath,
                        $"Task {taskId}: Log entry {j}\n");
                }
            });
        }

        Task.WaitAll(tasks);
        Console.WriteLine("Hoàn thành - Có thể bị lỗi!");
    }

    // AN TOÀN - Dùng lock
    static void TestWithLock()
    {
        Task[] tasks = new Task[5];

        for (int i = 0; i < 5; i++)
        {
            int taskId = i;
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    lock (fileLock)
                    {
                        // Chỉ 1 thread được ghi tại 1 thời điểm
                        File.AppendAllText(logFilePath,
                            $"Task {taskId}: Log entry {j}\n");
                    }
                }
            });
        }

        Task.WaitAll(tasks);
        Console.WriteLine("Hoàn thành - An toàn!");
    }

    // AN TOÀN - Dùng Queue Pattern
    static void TestWithQueue()
    {
        BlockingCollection<string> logQueue = new BlockingCollection<string>();

        // Dedicated logging task
        Task loggingTask = Task.Run(() =>
        {
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                foreach (var message in logQueue.GetConsumingEnumerable())
                {
                    writer.WriteLine(message);
                }
            }
        });

        // Producer tasks
        Task[] tasks = new Task[5];
        for (int i = 0; i < 5; i++)
        {
            int taskId = i;
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    logQueue.Add($"Task {taskId}: Log entry {j}");
                }
            });
        }

        Task.WaitAll(tasks);
        logQueue.CompleteAdding(); // Báo hiệu không còn message
        loggingTask.Wait();

        Console.WriteLine("Hoàn thành - An toàn và hiệu quả!");
    }
}
