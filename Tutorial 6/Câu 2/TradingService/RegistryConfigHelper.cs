using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;

namespace TradingService
{
    public class RegistryConfigHelper
    {
        private readonly ILogger _logger;
        private const string RegistryKeyPath = @"SOFTWARE\TradingService";

        public RegistryConfigHelper(ILogger logger)
        {
            _logger = logger;
        }

        public ServiceConfiguration ReadConfiguration()
        {
            _logger.LogInformation("Reading configuration from Windows Registry...");
            _logger.LogInformation($"Registry Path: HKEY_LOCAL_MACHINE\\{RegistryKeyPath}");

            if (!OperatingSystem.IsWindows())
            {
                _logger.LogWarning("⚠ Registry is only supported on Windows. Using default configuration.");
                return GetDefaultConfiguration();
            }

            ServiceConfiguration config = new ServiceConfiguration();

            try
            {
                // Mở registry key (LocalMachine = HKLM)
                using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(RegistryKeyPath))
                {
                    if (key == null)
                    {
                        _logger.LogWarning("⚠ Registry key does not exist!");
                        _logger.LogWarning("Using default configuration values...");
                        return GetDefaultConfiguration();
                    }

                    // Đọc InputFolder
                    config.InputFolder = (key.GetValue("InputFolder") as string) ?? string.Empty;
                    if (string.IsNullOrEmpty(config.InputFolder))
                    {
                        _logger.LogError("❌ InputFolder is missing or empty in registry!");
                        config.InputFolder = @"C:\TradingFiles\Input"; // fallback
                    }
                    else
                    {
                        _logger.LogInformation($"  ✓ InputFolder: {config.InputFolder}");
                    }

                    // Đọc ProcessedFolder
                    config.ProcessedFolder = (key.GetValue("ProcessedFolder") as string) ?? string.Empty;
                    if (string.IsNullOrEmpty(config.ProcessedFolder))
                    {
                        _logger.LogError("❌ ProcessedFolder is missing or empty in registry!");
                        config.ProcessedFolder = @"C:\TradingFiles\Processed"; // fallback
                    }
                    else
                    {
                        _logger.LogInformation($"  ✓ ProcessedFolder: {config.ProcessedFolder}");
                    }

                    // Đọc IntervalSeconds (DWORD)
                    object? intervalValue = key.GetValue("IntervalSeconds");
                    if (intervalValue == null)
                    {
                        _logger.LogError("❌ IntervalSeconds is missing in registry!");
                        config.IntervalSeconds = 30; // fallback
                    }
                    else
                    {
                        config.IntervalSeconds = Convert.ToInt32(intervalValue);
                        _logger.LogInformation($"  ✓ IntervalSeconds: {config.IntervalSeconds}");
                    }
                }

                // Validate configuration
                if (!ValidateConfiguration(config))
                {
                    _logger.LogError("❌ Configuration validation failed!");
                    return GetDefaultConfiguration();
                }

                _logger.LogInformation("✓ Configuration loaded successfully from registry");
                return config;
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError($"❌ Access denied reading registry: {ex.Message}");
                _logger.LogError("Make sure the service runs with Administrator privileges!");
                return GetDefaultConfiguration();
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error reading registry: {ex.Message}");
                return GetDefaultConfiguration();
            }
        }

        private bool ValidateConfiguration(ServiceConfiguration config)
        {
            bool isValid = true;

            // Validate InputFolder path
            if (string.IsNullOrEmpty(config.InputFolder))
            {
                _logger.LogError("❌ InputFolder cannot be empty");
                isValid = false;
            }
            else if (!System.IO.Path.IsPathRooted(config.InputFolder))
            {
                _logger.LogError($"❌ InputFolder must be absolute path: {config.InputFolder}");
                isValid = false;
            }

            // Validate ProcessedFolder path
            if (string.IsNullOrEmpty(config.ProcessedFolder))
            {
                _logger.LogError("❌ ProcessedFolder cannot be empty");
                isValid = false;
            }
            else if (!System.IO.Path.IsPathRooted(config.ProcessedFolder))
            {
                _logger.LogError($"❌ ProcessedFolder must be absolute path: {config.ProcessedFolder}");
                isValid = false;
            }

            // Validate IntervalSeconds
            if (config.IntervalSeconds < 5 || config.IntervalSeconds > 3600)
            {
                _logger.LogError($"❌ IntervalSeconds must be between 5 and 3600: {config.IntervalSeconds}");
                _logger.LogWarning("Using default: 30 seconds");
                config.IntervalSeconds = 30;
                isValid = false;
            }

            return isValid;
        }

        private ServiceConfiguration GetDefaultConfiguration()
        {
            _logger.LogInformation("Loading default configuration...");
            return new ServiceConfiguration
            {
                InputFolder = @"C:\TradingFiles\Input",
                ProcessedFolder = @"C:\TradingFiles\Processed",
                IntervalSeconds = 30
            };
        }

        // Method để tạo registry key (chạy 1 lần để setup)
        public static void CreateRegistryConfiguration()
        {
            Console.WriteLine("Creating registry configuration...");

            if (!OperatingSystem.IsWindows())
            {
                Console.WriteLine("❌ Registry is only supported on Windows!");
                return;
            }

            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(RegistryKeyPath))
                {
                    if (key == null)
                    {
                        Console.WriteLine("❌ Failed to create registry key!");
                        Console.WriteLine("Make sure you run this with Administrator privileges!");
                        return;
                    }

                    key.SetValue("InputFolder", @"C:\TradingFiles\Input", RegistryValueKind.String);
                    key.SetValue("ProcessedFolder", @"C:\TradingFiles\Processed", RegistryValueKind.String);
                    key.SetValue("IntervalSeconds", 30, RegistryValueKind.DWord);

                    Console.WriteLine("✓ Registry configuration created successfully!");
                    Console.WriteLine($"  Location: HKEY_LOCAL_MACHINE\\{RegistryKeyPath}");
                    Console.WriteLine($"  InputFolder = C:\\TradingFiles\\Input");
                    Console.WriteLine($"  ProcessedFolder = C:\\TradingFiles\\Processed");
                    Console.WriteLine($"  IntervalSeconds = 30");
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("❌ Access denied! Run as Administrator to create registry keys.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }
    }

    public class ServiceConfiguration
    {
        public string InputFolder { get; set; } = string.Empty;
        public string ProcessedFolder { get; set; } = string.Empty;
        public int IntervalSeconds { get; set; }
    }
}