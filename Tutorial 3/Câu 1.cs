using System;

public class Person
{
    public string Name;
    public int Age;
}

public struct Point
{
    public int X;
    public int Y;
}

class Program
{
    static void Main()
    {
        // SCOPE: Từ đây đến cuối Main()
        // LIFETIME: Tạo khi Main() bắt đầu, xóa khi Main() kết thúc
        // LOCATION: Stack (value type)
        int number = 10;

        // SCOPE: Từ đây đến cuối Main()
        // LIFETIME: Tạo khi Main() bắt đầu, xóa khi Main() kết thúc
        // LOCATION: Stack (struct - value type)
        Point point = new Point { X = 5, Y = 10 };

        // SCOPE: Reference từ đây đến cuối Main()
        // LIFETIME: 
        //   - Reference trên stack: xóa khi Main() kết thúc
        //   - Object trên heap: eligible for GC khi Main() kết thúc
        // LOCATION: Reference trên Stack, Object trên Heap
        Person person = new Person { Name = "Thai", Age = 20 };

        Console.WriteLine($"Number: {number}");
        Console.WriteLine($"Point: ({point.X}, {point.Y})");
        Console.WriteLine($"Person: {person.Name}, Age: {person.Age}");

        TestMethod();
        Console.ReadLine();
        // Sau khi TestMethod() return, tất cả biến trong đó đã bị xóa
    }

    static void TestMethod()
    {
        // SCOPE: Chỉ tồn tại trong TestMethod(), không thể truy cập từ bên ngoài
        // LIFETIME: Tạo khi method được gọi, XÓA NGAY khi method return
        // LOCATION: Stack
        int localNum = 100;

        // SCOPE: Chỉ trong TestMethod()
        // LIFETIME:
        //   - Reference: Tạo khi method gọi, XÓA NGAY khi method return
        //   - Object: Tạo ở đây, ELIGIBLE FOR GC ngay khi method return
        //              (vì không còn reference nào trỏ đến nó)
        // LOCATION: Reference trên Stack, Object trên Heap
        Person localPerson = new Person { Name = "Local", Age = 25 };

        Console.WriteLine($"\nInside TestMethod:");
        Console.WriteLine($"LocalNum: {localNum}");
        Console.WriteLine($"LocalPerson: {localPerson.Name}, Age: {localPerson.Age}");

        // KHI METHOD KẾT THÚC:
        // 1. localNum → Xóa khỏi stack NGAY LẬP TỨC
        // 2. localPerson reference → Xóa khỏi stack NGAY LẬP TỨC
        // 3. Person object trên heap → Không còn reference → Eligible for GC
        //    (GC sẽ thu gom sau này, không phải ngay lập tức)
    }
}