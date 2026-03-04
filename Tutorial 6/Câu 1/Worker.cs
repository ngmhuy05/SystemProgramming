using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TradingService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly string _monitorFolder;
        private int _executionCount = 0;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            
            // Folder để monitor (có thể config từ Registry sau)
            _monitorFolder = @"C:\TradingFiles\Input";
            
            // Tạo folder nếu chưa có
            if (!Directory.Exists(_monitorFolder))
            {
                Directory.CreateDirectory(_monitorFolder);
                _logger.LogInformation($"Created monitor folder: {_monitorFolder}");
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("═══════════════════════════════════════");
            _logger.LogInformation("Trading Background Service STARTING...");
            _logger.LogInformation($"Monitor Folder: {_monitorFolder}");
            _logger.LogInformation($"Start Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            _logger.LogInformation("═══════════════════════════════════════");
            
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background processing started. Checking folder every 30 seconds...");

            // Periodic execution - chạy mỗi 30 giây
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _executionCount++;
                    
                    _logger.LogInformation("───────────────────────────────────────");
                    _logger.LogInformation($"Execution #{_executionCount} at {DateTime.Now:HH:mm:ss}");
                    
                    // Kiểm tra folder và process files
                    ProcessTradeFiles();
                    
                    _logger.LogInformation($"Waiting 30 seconds until next check...");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"ERROR in background processing: {ex.Message}");
                }

                // Đợi 30 giây trước lần check tiếp theo
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        private void ProcessTradeFiles()
        {
            // Lấy danh sách files trong folder
            string[] files = Directory.GetFiles(_monitorFolder, "*.json");
            
            if (files.Length == 0)
            {
                _logger.LogInformation("  → No trade files found in folder.");
            }
            else
            {
                _logger.LogInformation($"  → Found {files.Length} trade file(s):");
                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    _logger.LogInformation($"     • {fileName}");
                }
                _logger.LogInformation("  → Files are ready for processing (implementation in next questions)");
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("═══════════════════════════════════════");
            _logger.LogInformation("Trading Background Service STOPPING...");
            _logger.LogInformation($"Total Executions: {_executionCount}");
            _logger.LogInformation($"Stop Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            _logger.LogInformation("═══════════════════════════════════════");
            
            return base.StopAsync(cancellationToken);
        }
    }
}
