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
        private ServiceConfiguration _config;
        private int _executionCount = 0;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("========================================");
            _logger.LogInformation("Trading Background Service STARTING...");
            _logger.LogInformation($"Start Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            _logger.LogInformation("========================================");

            // ??c configuration t? Registry
            RegistryConfigHelper configHelper = new RegistryConfigHelper(_logger);
            _config = configHelper.ReadConfiguration();

            // T?o folders n?u ch?a c?
            CreateFoldersIfNotExist();

            _logger.LogInformation("========================================");
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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Background processing started. Checking folder every {_config.IntervalSeconds} seconds...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _executionCount++;

                    _logger.LogInformation("========================================");
                    _logger.LogInformation($"Execution #{_executionCount} at {DateTime.Now:HH:mm:ss}");

                    ProcessTradeFiles();

                    _logger.LogInformation($"Waiting {_config.IntervalSeconds} seconds until next check...");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"? ERROR in background processing: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromSeconds(_config.IntervalSeconds), stoppingToken);
            }
        }

        private void ProcessTradeFiles()
        {
            try
            {
                string[] files = Directory.GetFiles(_config.InputFolder, "*.json");

                if (files.Length == 0)
                {
                    _logger.LogInformation($"  ? No trade files found in {_config.InputFolder}");
                }
                else
                {
                    _logger.LogInformation($"  ? Found {files.Length} trade file(s):");
                    foreach (string file in files)
                    {
                        string fileName = Path.GetFileName(file);
                        _logger.LogInformation($"     ? {fileName}");
                    }
                    _logger.LogInformation("  ? Files ready for processing (implementation in Q3)");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"? Error processing files: {ex.Message}");
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("========================================");
            _logger.LogInformation("Trading Background Service STOPPING...");
            _logger.LogInformation($"Total Executions: {_executionCount}");
            _logger.LogInformation($"Stop Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            _logger.LogInformation("========================================");

            return base.StopAsync(cancellationToken);
        }
    }
}