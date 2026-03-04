using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TradingService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private ServiceConfiguration _config;
        private int _executionCount = 0;

        // FileSystemWatcher ð? theo d?i folder
        private FileSystemWatcher? _watcher;

        // ConcurrentDictionary dùng ð? track file ðang ðý?c x? l?
        // Key = tên file, Value = true n?u ðang x? l?
        // Dùng ð? tránh double processing khi nhi?u event cùng trigger
        private readonly ConcurrentDictionary<string, bool> _processingFiles = new();

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("????????????????????????????????????????");
            _logger.LogInformation("Trading Background Service STARTING...");
            _logger.LogInformation($"Start Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            _logger.LogInformation("????????????????????????????????????????");

            // Ð?c configuration t? Registry
            RegistryConfigHelper configHelper = new RegistryConfigHelper(_logger);
            _config = configHelper.ReadConfiguration();

            // T?o folders n?u chýa có
            CreateFoldersIfNotExist();

            // Kh?i t?o FileSystemWatcher (câu 3)
            InitializeFileSystemWatcher();

            _logger.LogInformation("????????????????????????????????????????");
            return base.StartAsync(cancellationToken);
        }

        private void CreateFoldersIfNotExist()
        {
            try
            {
                if (!Directory.Exists(_config.InputFolder))
                {
                    Directory.CreateDirectory(_config.InputFolder);
                    _logger.LogInformation($"? Created InputFolder: {_config.InputFolder}");
                }

                if (!Directory.Exists(_config.ProcessedFolder))
                {
                    Directory.CreateDirectory(_config.ProcessedFolder);
                    _logger.LogInformation($"? Created ProcessedFolder: {_config.ProcessedFolder}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"? Error creating folders: {ex.Message}");
            }
        }

        // ????????????????????????????????????????????????????????????
        // Q3 - Task 1: Kh?i t?o FileSystemWatcher
        // ????????????????????????????????????????????????????????????
        private void InitializeFileSystemWatcher()
        {
            _watcher = new FileSystemWatcher(_config.InputFolder)
            {
                Filter = "*.json",               // Ch? theo d?i file .json
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                EnableRaisingEvents = true        // B?t ð?u l?ng nghe
            };

            // Ðãng k? event handler cho file m?i ðý?c t?o ho?c copy vào folder
            _watcher.Created += OnFileCreated;

            _logger.LogInformation($"? FileSystemWatcher started. Watching: {_config.InputFolder}");
        }

        // ????????????????????????????????????????????????????????????
        // Q3 - Task 2 + 3: X? l? file khi có event Created
        // Ch?y trên thread pool ? c?n ð?m b?o thread safety
        // ????????????????????????????????????????????????????????????
        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            string filePath = e.FullPath;
            string fileName = e.Name ?? Path.GetFileName(filePath);

            // Task 3 - Prevent double processing:
            // TryAdd tr? v? false n?u key ð? t?n t?i ? file ðang ðý?c x? l? ? b? qua
            if (!_processingFiles.TryAdd(fileName, true))
            {
                _logger.LogWarning($"? File already being processed, skipping: {fileName}");
                return;
            }

            // Ch?y x? l? trên background thread ð? không block FileSystemWatcher
            Task.Run(() => ProcessSingleFile(filePath, fileName));
        }

        private async Task ProcessSingleFile(string filePath, string fileName)
        {
            try
            {
                _logger.LogInformation($"????????????????????????????????????????");
                _logger.LogInformation($"?? Processing file: {fileName}");

                // Task 1 - Ð?c n?i dung file
                // Ch? file ðý?c ghi xong trý?c khi ð?c (tránh l?i file lock)
                await WaitForFileReady(filePath);

                string content = await File.ReadAllTextAsync(filePath);
                _logger.LogInformation($"  ? Read {content.Length} characters from {fileName}");
                _logger.LogInformation($"  Content preview: {content[..Math.Min(100, content.Length)]}...");

                // Task 2 - Simulate processing (gi? l?p x? l? nghi?p v?)
                _logger.LogInformation($"  ? Simulating trade processing...");
                await Task.Delay(TimeSpan.FromSeconds(2)); // Gi? l?p th?i gian x? l?
                _logger.LogInformation($"  ? Trade data processed successfully");

                // Task 3 - Move file sang ProcessedFolder
                string destPath = Path.Combine(_config.ProcessedFolder, fileName);

                // N?u file ð? t?n t?i ? ðích th? ð?i tên thêm timestamp
                if (File.Exists(destPath))
                {
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                    destPath = Path.Combine(_config.ProcessedFolder, $"{nameWithoutExt}_{timestamp}.json");
                }

                File.Move(filePath, destPath);
                _logger.LogInformation($"  ? Moved to Processed: {Path.GetFileName(destPath)}");
                _logger.LogInformation($"????????????????????????????????????????");
            }
            catch (Exception ex)
            {
                _logger.LogError($"? Error processing file {fileName}: {ex.Message}");
            }
            finally
            {
                // Luôn xóa kh?i dictionary dù thành công hay l?i
                // ð? file có th? ðý?c x? l? l?i n?u c?n
                _processingFiles.TryRemove(fileName, out _);
            }
        }

        // Ch? file ðý?c ghi xong (tránh ð?c file ðang b? lock)
        private async Task WaitForFileReady(string filePath, int maxAttempts = 10)
        {
            for (int i = 0; i < maxAttempts; i++)
            {
                try
                {
                    using FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
                    return; // File s?n sàng
                }
                catch (IOException)
                {
                    await Task.Delay(500); // Ch? 500ms r?i th? l?i
                }
            }
        }

        // ????????????????????????????????????????????????????????????
        // ExecuteAsync: V?n gi? polling loop t? câu 1
        // Dùng ð? log tr?ng thái ð?nh k? bên c?nh FileSystemWatcher
        // ????????????????????????????????????????????????????????????
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Service running. FileSystemWatcher active on: {_config.InputFolder}");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _executionCount++;
                    _logger.LogInformation($"????????????????????????????????????????");
                    _logger.LogInformation($"Status check #{_executionCount} at {DateTime.Now:HH:mm:ss}");
                    _logger.LogInformation($"Currently processing: {_processingFiles.Count} file(s)");
                    _logger.LogInformation($"????????????????????????????????????????");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"? ERROR in background loop: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromSeconds(_config.IntervalSeconds), stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("========================================");
            _logger.LogInformation("Trading Background Service STOPPING...");
            _logger.LogInformation($"Total Status Checks: {_executionCount}");
            _logger.LogInformation($"Stop Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            _logger.LogInformation("========================================");

            // Dispose FileSystemWatcher khi service d?ng
            _watcher?.Dispose();

            return base.StopAsync(cancellationToken);
        }
    }
}