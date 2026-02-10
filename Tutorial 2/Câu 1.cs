using System;

struct PointStruct
{
    public int X;
    public int Y;
}

class PointClass
{
    public int X;
    public int Y;
}

class Program
{
    static void Main()
    {
        Console.WriteLine("===== VALUE TYPE (STRUCT) =====");

        PointStruct s1 = new PointStruct { X = 10, Y = 20 };
        PointStruct s2 = s1;   // Copy giá trị
        s2.X = 100;            // Thay đổi bản sao

        Console.WriteLine($"s1.X = {s1.X}, s1.Y = {s1.Y}");
        Console.WriteLine($"s2.X = {s2.X}, s2.Y = {s2.Y}");

        Console.WriteLine("\n===== REFERENCE TYPE (CLASS) =====");

        PointClass c1 = new PointClass { X = 10, Y = 20 };
        PointClass c2 = c1;    // Copy tham chiếu
        c2.X = 100;            // Thay đổi qua c2

        Console.WriteLine($"c1.X = {c1.X}, c1.Y = {c1.Y}");
        Console.WriteLine($"c2.X = {c2.X}, c2.Y = {c2.Y}");

        Console.ReadLine();
    }
}
