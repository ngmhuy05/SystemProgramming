using System;
using System.Diagnostics;

struct DataStruct
{
    public int A, B, C, D;
}

class DataClass
{
    public int A, B, C, D;
}

class Program
{
    static void Main()
    {
        const int SIZE = 5_000_000;

        Console.WriteLine("===== TEST STRUCT ARRAY =====");
        GC.Collect();
        long memBeforeStruct = GC.GetTotalMemory(true);

        DataStruct[] structArray = new DataStruct[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            structArray[i].A = i;
            structArray[i].B = i;
            structArray[i].C = i;
            structArray[i].D = i;
        }

        long memAfterStruct = GC.GetTotalMemory(true);
        Console.WriteLine($"Struct Memory Used: {(memAfterStruct - memBeforeStruct) / 1024 / 1024} MB");

        Stopwatch swStruct = Stopwatch.StartNew();
        long sumStruct = 0;
        for (int i = 0; i < SIZE; i++)
            sumStruct += structArray[i].A;
        swStruct.Stop();
        Console.WriteLine($"Struct Access Time: {swStruct.ElapsedMilliseconds} ms");

        Console.WriteLine("\n===== TEST CLASS ARRAY =====");
        GC.Collect();
        long memBeforeClass = GC.GetTotalMemory(true);

        DataClass[] classArray = new DataClass[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            classArray[i] = new DataClass
            {
                A = i,
                B = i,
                C = i,
                D = i
            };
        }

        long memAfterClass = GC.GetTotalMemory(true);
        Console.WriteLine($"Class Memory Used: {(memAfterClass - memBeforeClass) / 1024 / 1024} MB");

        Stopwatch swClass = Stopwatch.StartNew();
        long sumClass = 0;
        for (int i = 0; i < SIZE; i++)
            sumClass += classArray[i].A;
        swClass.Stop();
        Console.WriteLine($"Class Access Time: {swClass.ElapsedMilliseconds} ms");

        Console.ReadLine();
    }
}
