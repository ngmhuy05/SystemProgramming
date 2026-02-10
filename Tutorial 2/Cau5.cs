using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;

namespace MembershipCardPrinter
{
    class Program
    {
        // Import Windows API để cài font
        [DllImport("gdi32.dll")]
        private static extern int AddFontResource(string lpFileName);

        static void Main(string[] args)
        {
            Console.WriteLine("=== CRM MEMBERSHIP CARD PRINTER ===\n");

            // Kiểm tra và cài font
            CheckAndInstallFonts();

            // Dữ liệu mẫu sẵn
            string name = "Nguyen Van A";
            string id = "CRM-2026-001";
            string level = "Gold";

            Console.WriteLine($"Member Name: {name}");
            Console.WriteLine($"Member ID: {id}");
            Console.WriteLine($"Membership Level: {level}");

            // In thẻ
            PrintMembershipCard(name, id, level);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static void CheckAndInstallFonts()
        {
            Console.WriteLine("Checking required fonts...");

            // Kiểm tra fonts có sẵn trong hệ thống
            InstalledFontCollection fonts = new InstalledFontCollection();
            string[] requiredFonts = { "Arial", "Times New Roman" };

            foreach (string fontName in requiredFonts)
            {
                bool exists = fonts.Families.Any(f =>
                    f.Name.Equals(fontName, StringComparison.OrdinalIgnoreCase));

                if (exists)
                {
                    Console.WriteLine($"✓ {fontName} - Found");
                }
                else
                {
                    Console.WriteLine($"✗ {fontName} - Missing");
                    TryInstallFont(fontName);
                }
            }
            Console.WriteLine();
        }

        static void TryInstallFont(string fontName)
        {
            Console.WriteLine($"\nAttempting to install {fontName}...");

            // Đường dẫn font giả định (cần có file .ttf trong thư mục)
            string fontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{fontName}.ttf");

            if (File.Exists(fontPath))
            {
                try
                {
                    // Cài font (cần quyền Administrator)
                    int result = AddFontResource(fontPath);
                    if (result != 0)
                    {
                        Console.WriteLine($"✓ {fontName} installed successfully");
                    }
                    else
                    {
                        Console.WriteLine($"✗ Failed to install {fontName}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine("Note: Installing fonts requires Administrator privileges");
                }
            }
            else
            {
                Console.WriteLine($"Font file not found: {fontPath}");
                Console.WriteLine("Using system default fonts instead.");
            }
        }

        static void PrintMembershipCard(string name, string id, string level)
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("                    MEMBERSHIP CARD");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine();
            Console.WriteLine($"  Company: CRM SYSTEMS");
            Console.WriteLine();
            Console.WriteLine($"  Member Name:    {name.ToUpper()}");
            Console.WriteLine($"  Member ID:      {id}");
            Console.WriteLine($"  Level:          {level.ToUpper()} ★");
            Console.WriteLine();
            Console.WriteLine($"  Issue Date:     {DateTime.Now:dd/MM/yyyy}");
            Console.WriteLine($"  Valid Until:    {DateTime.Now.AddYears(1):dd/MM/yyyy}");
            Console.WriteLine();
            Console.WriteLine(new string('=', 60));

            // Tạo file ảnh thẻ
            SaveCardAsImage(name, id, level);
        }

        static void SaveCardAsImage(string name, string id, string level)
        {
            Bitmap card = null;
            Graphics g = null;

            try
            {
                int width = 800;
                int height = 500;
                card = new Bitmap(width, height);
                g = Graphics.FromImage(card);

                // Background color theo level
                Color bgColor = GetLevelColor(level);
                using (SolidBrush bgBrush = new SolidBrush(bgColor))
                {
                    g.FillRectangle(bgBrush, 0, 0, width, height);
                }

                // Vẽ border
                using (Pen borderPen = new Pen(Color.White, 5))
                {
                    g.DrawRectangle(borderPen, 10, 10, width - 20, height - 20);
                }

                // Vẽ text với fonts an toàn
                using (Font companyFont = new Font("Arial", 24, FontStyle.Bold))
                using (Font levelFont = new Font("Arial", 32, FontStyle.Bold))
                using (Font nameFont = new Font("Arial", 36, FontStyle.Bold))
                using (Font idFont = new Font("Arial", 20, FontStyle.Regular))
                using (Font dateFont = new Font("Arial", 16, FontStyle.Regular))
                using (SolidBrush whiteBrush = new SolidBrush(Color.White))
                {
                    g.DrawString("CRM SYSTEMS", companyFont, whiteBrush, 30, 30);
                    g.DrawString(level.ToUpper(), levelFont, whiteBrush, 30, 80);
                    g.DrawString(name, nameFont, whiteBrush, 30, 200);
                    g.DrawString($"ID: {id}", idFont, whiteBrush, 30, 280);
                    g.DrawString($"Member Since: {DateTime.Now:MMM yyyy}", dateFont, whiteBrush, 30, 420);
                }

                string fileName = $"MemberCard_{id}.png";
                card.Save(fileName, ImageFormat.Png);

                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
                Console.WriteLine($"\n✓ Card image saved as: {fullPath}");

                // Tự động mở ảnh (cho Windows)
                System.Diagnostics.Process.Start("explorer.exe", fullPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating card image: {ex.Message}");
            }
            finally
            {
                if (g != null) g.Dispose();
                if (card != null) card.Dispose();
            }
        }

        static Color GetLevelColor(string level)
        {
            switch (level.ToLower())
            {
                case "bronze": return Color.FromArgb(205, 127, 50);
                case "silver": return Color.FromArgb(192, 192, 192);
                case "gold": return Color.FromArgb(255, 215, 0);
                case "platinum": return Color.FromArgb(229, 228, 226);
                default: return Color.Gray;
            }
        }
    }
}