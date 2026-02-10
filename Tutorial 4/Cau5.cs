using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        string originalFile = "data.txt";
        string encryptedFile = "data.encrypted";
        string password = "MySecurePassword123";

        // Tạo file gốc
        File.WriteAllText(originalFile, "This is sensitive data that needs protection!\nLine 2\nLine 3");
        Console.WriteLine("✓ File gốc đã tạo\n");

        // ENCRYPT + COMPRESS
        Console.WriteLine("=== Encrypting và Compressing ===");
        EncryptAndCompress(originalFile, encryptedFile, password);
        Console.WriteLine($"✓ Đã lưu: {encryptedFile}");

        long originalSize = new FileInfo(originalFile).Length;
        long encryptedSize = new FileInfo(encryptedFile).Length;
        Console.WriteLine($"Original: {originalSize} bytes");
        Console.WriteLine($"Encrypted+Compressed: {encryptedSize} bytes\n");

        // DECOMPRESS + DECRYPT
        Console.WriteLine("=== Decompressing và Decrypting ===");
        string decryptedFile = "data_decrypted.txt";
        DecompressAndDecrypt(encryptedFile, decryptedFile, password);
        Console.WriteLine($"✓ Đã khôi phục: {decryptedFile}\n");

        // Verify
        string original = File.ReadAllText(originalFile);
        string decrypted = File.ReadAllText(decryptedFile);
        Console.WriteLine($"Verification: {(original == decrypted ? "PASS ✓" : "FAIL ✗")}");

        Console.ReadLine();
    }

    static void EncryptAndCompress(string inputFile, string outputFile, string password)
    {
        // 1. Đọc data
        byte[] data = File.ReadAllBytes(inputFile);
        Console.WriteLine($"  1. Đọc {data.Length} bytes");

        // 2. Encrypt trước
        byte[] encrypted = EncryptData(data, password);
        Console.WriteLine($"  2. Encrypted: {encrypted.Length} bytes");

        // 3. Compress sau
        byte[] compressed = CompressData(encrypted);
        Console.WriteLine($"  3. Compressed: {compressed.Length} bytes");

        // 4. Save
        File.WriteAllBytes(outputFile, compressed);
        Console.WriteLine($"  4. Saved to disk");
    }

    static void DecompressAndDecrypt(string inputFile, string outputFile, string password)
    {
        // 1. Đọc
        byte[] compressed = File.ReadAllBytes(inputFile);
        Console.WriteLine($"  1. Đọc {compressed.Length} bytes");

        // 2. Decompress trước
        byte[] encrypted = DecompressData(compressed);
        Console.WriteLine($"  2. Decompressed: {encrypted.Length} bytes");

        // 3. Decrypt sau
        byte[] decrypted = DecryptData(encrypted, password);
        Console.WriteLine($"  3. Decrypted: {decrypted.Length} bytes");

        // 4. Save
        File.WriteAllBytes(outputFile, decrypted);
        Console.WriteLine($"  4. Saved to disk");
    }

    static byte[] EncryptData(byte[] data, string password)
    {
        using (Aes aes = Aes.Create())
        {
            // Generate key từ password
            var pdb = new Rfc2898DeriveBytes(password, new byte[]
                { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64 });
            aes.Key = pdb.GetBytes(32);
            aes.IV = pdb.GetBytes(16);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                }
                return ms.ToArray();
            }
        }
    }

    static byte[] DecryptData(byte[] encrypted, string password)
    {
        using (Aes aes = Aes.Create())
        {
            var pdb = new Rfc2898DeriveBytes(password, new byte[]
                { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64 });
            aes.Key = pdb.GetBytes(32);
            aes.IV = pdb.GetBytes(16);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(encrypted, 0, encrypted.Length);
                }
                return ms.ToArray();
            }
        }
    }

    static byte[] CompressData(byte[] data)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (GZipStream gzip = new GZipStream(ms, CompressionMode.Compress))
            {
                gzip.Write(data, 0, data.Length);
            }
            return ms.ToArray();
        }
    }

    static byte[] DecompressData(byte[] compressed)
    {
        using (MemoryStream input = new MemoryStream(compressed))
        using (GZipStream gzip = new GZipStream(input, CompressionMode.Decompress))
        using (MemoryStream output = new MemoryStream())
        {
            gzip.CopyTo(output);
            return output.ToArray();
        }
    }
}