using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static string watchFolder = @"C:\WatchFolder";
    static string outputFolder = @"C:\CompressedFolder";
    static object processLock = new object();

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Directory.CreateDirectory(watchFolder);
        Directory.CreateDirectory(outputFolder);

        Console.WriteLine($"Monitoring folder: {watchFolder}");
        Console.WriteLine($"Filter: *.txt");
        Console.WriteLine("Tạo file mới trong folder để test...\n");

        using (FileSystemWatcher watcher = new FileSystemWatcher(watchFolder))
        {
            watcher.Filter = "*.txt";
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;

            // THÊM LOG ĐỂ DEBUG
            watcher.Created += (sender, e) =>
            {
                Console.WriteLine($">>> EVENT FIRED: {e.Name}"); // DEBUG
                OnFileCreated(sender, e);
            };

            watcher.Error += (sender, e) =>
            {
                Console.WriteLine($">>> ERROR: {e.GetException()}"); // DEBUG
            };

            watcher.EnableRaisingEvents = true;

            Console.WriteLine(">>> Watcher started!"); // DEBUG
            Console.WriteLine("Nhấn Enter để thoát...");
            Console.ReadLine();
        }
    }

    static void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] File detected: {e.Name}");

        // Xử lý async để không block event handler
        Task.Run(() => ProcessFile(e.FullPath));
    }

    static void ProcessFile(string filePath)
    {
        // Thread-safe processing
        lock (processLock)
        {
            try
            {
                // Đợi file được release bởi process tạo nó
                Thread.Sleep(500);

                // Đọc nội dung
                Console.WriteLine($"  Reading: {Path.GetFileName(filePath)}");
                string content = File.ReadAllText(filePath);

                // Compress
                Console.WriteLine($"  Compressing...");
                byte[] compressedData = CompressString(content);

                // Save
                string outputPath = Path.Combine(outputFolder,
                    Path.GetFileNameWithoutExtension(filePath) + ".gz");
                File.WriteAllBytes(outputPath, compressedData);

                Console.WriteLine($"  ✓ Saved: {Path.GetFileName(outputPath)}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Error: {ex.Message}\n");
            }
        }
    }

    static byte[] CompressString(string text)
    {
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);

        using (MemoryStream ms = new MemoryStream())
        {
            using (GZipStream gzip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                gzip.Write(buffer, 0, buffer.Length);
            }
            return ms.ToArray();
        }
    }
}