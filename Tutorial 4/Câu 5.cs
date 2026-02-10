using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("=== Async/Await Correct Usage ===");
        await CorrectUsage();

        Console.WriteLine("\n=== Async Void Problem ===");
        AsyncVoidProblem();
        await Task.Delay(2000);

        Console.WriteLine("\n=== Task.Result Problem ===");
        TaskResultProblem();

        Console.WriteLine("\n=== Task.Wait Problem ===");
        TaskWaitProblem();

        Console.ReadLine();
    }

    // ĐÚNG
    static async Task CorrectUsage()
    {
        Console.WriteLine($"Start - Thread {Thread.CurrentThread.ManagedThreadId}");
        await LongOperation("Task 1");
        await LongOperation("Task 2");
        Console.WriteLine($"End - Thread {Thread.CurrentThread.ManagedThreadId}");
    }

    // SAI 1
    static async void AsyncVoidMethod()
    {
        await Task.Delay(500);
        Console.WriteLine("Async void completed");
    }

    static void AsyncVoidProblem()
    {
        AsyncVoidMethod();
        Console.WriteLine("Cannot await async void!");
    }

    // SAI 2
    static void TaskResultProblem()
    {
        Console.WriteLine($"Blocking thread {Thread.CurrentThread.ManagedThreadId}");
        var result = LongOperation("Blocked").Result;
        Console.WriteLine(result);
    }

    // SAI 3
    static void TaskWaitProblem()
    {
        Console.WriteLine($"Blocking thread {Thread.CurrentThread.ManagedThreadId}");
        LongOperation("Blocked").Wait();
    }

    static async Task<string> LongOperation(string name)
    {
        Console.WriteLine($"  {name} starting...");
        await Task.Delay(1000);
        Console.WriteLine($"  {name} done");
        return $"Result: {name}";
    }
}