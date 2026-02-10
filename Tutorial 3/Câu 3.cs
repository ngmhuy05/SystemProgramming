//using System;
//using System.Collections.Generic;
//using System.Linq;

//class Program
//{
//    static void Main()
//    {
//        List<int> numbers = new List<int>();
//        for (int i = 0; i < 10000; i++)
//        {
//            numbers.Add(i);
//        }

//        var filtered = numbers.Where(x => x % 2 == 0).ToList();
//        var squared = filtered.Select(x => x * x).ToList();
//        var result = squared.Take(100).ToList();

//        string output = "";
//        foreach (var num in result)
//        {
//            output += num.ToString() + ", ";
//        }
//    }
//}


using System;
using System.Text;

class Program
{
    static void Main()
    {
        // Kỹ thuật 1: Array thay vì List
        int[] numbers = new int[10000];
        for (int i = 0; i < 10000; i++)
        {
            numbers[i] = i;
        }

        // Kỹ thuật 2: Xử lý in-place với Span
        Span<int> span = numbers.AsSpan();
        int count = 0;
        for (int i = 0; i < span.Length && count < 100; i++)
        {
            if (span[i] % 2 == 0)
            {
                span[i] = span[i] * span[i];
                count++;
            }
        }

        // Kỹ thuật 3: StringBuilder
        StringBuilder sb = new StringBuilder(200);
        for (int i = 0; i < numbers.Length && count > 0; i++)
        {
            if (i % 2 == 0)
            {
                sb.Append(numbers[i]);
                sb.Append(", ");
            }
        }
        string output = sb.ToString();
    }
}