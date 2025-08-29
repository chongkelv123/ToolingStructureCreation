using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Model;
using Newtonsoft.Json;

namespace ToolingStructureCreation.Services
{
    /// <summary>
    /// CSV-based usage tracking service for time reduction analysis
    /// </summary>
    public class UsageTrackingService
    {
        private static UsageTrackingService _instance;
        private UsageSessionData _currentSession;
        private readonly string _logDirectory;
        private DateTime _sessionStartTime;

        private UsageTrackingService()
        {
            _logDirectory = @"\\SPL-SMB\DE\Common\SGA\3DA setup folder\logs\usage_logs";
            EnsureLogDirectoryExists();
        }

        public static UsageTrackingService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UsageTrackingService();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Starts a new usage tracking session
        /// </summary>
        public void StartSession(string moduleVersion)
        {
            _sessionStartTime = DateTime.Now;
            _currentSession = new UsageSessionData
            {
                SessionId = Guid.NewGuid().ToString(),
                Timestamp = _sessionStartTime,
                ModuleVersion = moduleVersion,
                PlateShoeCount = 0,
                AssemblyCount = 0,
                TotalFileSizeBytes = 0
            };
        }

        /// <summary>
        /// Updates session with engineer information (called when Apply is clicked)
        /// </summary>
        public void UpdateEngineerInfo(string engineerName)
        {
            if (_currentSession == null) return;
            _currentSession.EngineerName = engineerName;
        }

        /// <summary>
        /// Updates session with project information (called when Apply is clicked)
        /// </summary>
        public void UpdateProjectInfo(string model, string part)
        {
            if (_currentSession == null) return;
            _currentSession.Model = model ?? "Unknown";
            _currentSession.Part = part ?? "Unknown";
        }

        /// <summary>
        /// Updates session configuration (called when Apply is clicked)
        /// </summary>
        public void UpdateConfiguration(string machineType, MaterialGuideType guideType, int stationCount)
        {
            if (_currentSession == null) return;

            _currentSession.MachineType = machineType;
            _currentSession.GuideType = guideType.ToString();
            _currentSession.StationCount = stationCount;
        }

        /// <summary>
        /// Records component creation with type classification
        /// </summary>
        public void LogComponentCreated(string componentType, long fileSizeBytes, double processingTimeMs)
        {
            if (_currentSession == null) return;

            // Classify component type
            if (IsAssemblyComponent(componentType))
            {
                _currentSession.AssemblyCount++;
            }
            else
            {
                _currentSession.PlateShoeCount++;
            }

            _currentSession.TotalFileSizeBytes += fileSizeBytes;
        }

        /// <summary>
        /// Ends session and saves to CSV
        /// </summary>
        public void EndSession(bool completedSuccessfully)
        {            
            if (_currentSession == null) return;

            _currentSession.CompletedSuccessfully = completedSuccessfully;

            // Calculate actual time in minutes
            double actualTimeMinutes = (DateTime.Now - _sessionStartTime).TotalMinutes;
            _currentSession.ActualTimeMin = Math.Round(actualTimeMinutes, 1);

            // Calculate time reduction metrics
            CalculateTimeReductionMetrics();

            // Save to CSV
            SaveToCsv();

            _currentSession = null;
        }

        /// <summary>
        /// Determines if a component is an assembly (complex) vs individual plate/shoe
        /// </summary>
        private bool IsAssemblyComponent(string componentType)
        {
            var assemblyKeywords = new[] { "assembly", "asm", "main", "station", "tooling" };
            return assemblyKeywords.Any(keyword =>
                componentType.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        /// <summary>
        /// Calculates time reduction metrics using the agreed formula
        /// </summary>
        private void CalculateTimeReductionMetrics()
        {
            if (_currentSession == null) return;

            // EstimatedManualTime = (PlateShoeCount × 30 min) + (AssemblyCount × 50 min)
            double estimatedManualTime = (_currentSession.PlateShoeCount * 30.0) +
                                       (_currentSession.AssemblyCount * 50.0);

            _currentSession.EstimatedManualTimeMin = Math.Round(estimatedManualTime, 1);

            // TimeSaved = EstimatedManualTime - ActualTime
            double timeSaved = estimatedManualTime - _currentSession.ActualTimeMin;
            _currentSession.TimeSavedMin = Math.Round(Math.Max(0, timeSaved), 1);

            // ReductionPercentage = (TimeSaved / EstimatedManualTime) × 100
            if (estimatedManualTime > 0)
            {
                double reductionPercentage = (timeSaved / estimatedManualTime) * 100;
                _currentSession.ReductionPercentage = Math.Round(Math.Max(0, reductionPercentage), 1);
            }
            else
            {
                _currentSession.ReductionPercentage = 0.0;
            }

            // Total components for summary
            _currentSession.TotalComponents = _currentSession.PlateShoeCount + _currentSession.AssemblyCount;

            // Convert file size to MB
            _currentSession.TotalFileSizeMB = Math.Round(_currentSession.TotalFileSizeBytes / (1024.0 * 1024.0), 2);
        }

        /// <summary>
        /// Saves session data to monthly CSV file
        /// </summary>
        private void SaveToCsv()
        {
            try
            {
                string fileName = $"usage_{DateTime.Now:yyyy-MM}.csv";
                string filePath = Path.Combine(_logDirectory, fileName);

                bool fileExists = File.Exists(filePath);

                using (var writer = new StreamWriter(filePath, append: true, Encoding.UTF8))
                {
                    // Write header if file is new
                    if (!fileExists)
                    {
                        writer.WriteLine(GetCsvHeader());
                    }

                    // Write data row
                    writer.WriteLine(GetCsvDataRow());
                }
            }
            catch (Exception ex)
            {
                // Silent fail - don't disrupt user workflow
                LogError($"CSV tracking error: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns CSV header row
        /// </summary>
        private string GetCsvHeader()
        {
            return "SessionId,Timestamp,EngineerName,ToolingModel," +
                   "MachineType,GuideType,StationCount," +
                   "PlateShoeCount,AssemblyCount,TotalComponents," +
                   "EstimatedManualTimeMin,ActualTimeMin,TimeSavedMin,ReductionPercentage," +
                   "CompletedSuccessfully,TotalFileSizeMB,ModuleVersion";
        }

        /// <summary>
        /// Returns formatted CSV data row for current session
        /// </summary>
        private string GetCsvDataRow()
        {
            return $"{_currentSession.SessionId}," +
                   $"{_currentSession.Timestamp:yyyy-MM-dd HH:mm:ss}," +
                   $"{EscapeCsvField(_currentSession.EngineerName)}," +
                   $"{EscapeCsvField(_currentSession.Model + "_" + _currentSession.Part)}," +                   
                   $"{EscapeCsvField(_currentSession.MachineType)}," +
                   $"{_currentSession.GuideType}," +
                   $"{_currentSession.StationCount}," +
                   $"{_currentSession.PlateShoeCount}," +
                   $"{_currentSession.AssemblyCount}," +
                   $"{_currentSession.TotalComponents}," +
                   $"{_currentSession.EstimatedManualTimeMin}," +
                   $"{_currentSession.ActualTimeMin}," +
                   $"{_currentSession.TimeSavedMin}," +
                   $"{_currentSession.ReductionPercentage}," +
                   $"{_currentSession.CompletedSuccessfully}," +
                   $"{_currentSession.TotalFileSizeMB}," +
                   $"{EscapeCsvField(_currentSession.ModuleVersion)}";
        }

        /// <summary>
        /// Escapes CSV field values containing commas or quotes
        /// </summary>
        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field)) return "";

            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
            {
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            }

            return field;
        }

        private void EnsureLogDirectoryExists()
        {
            try
            {
                if (!Directory.Exists(_logDirectory))
                    Directory.CreateDirectory(_logDirectory);
            }
            catch
            {
                // Silent fail if directory creation fails
            }
        }

        private void LogError(string error)
        {
            try
            {
                string errorFile = Path.Combine(_logDirectory, "csv_errors.log");
                File.AppendAllText(errorFile, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {error}\n");
            }
            catch
            {
                // Ultimate silent fail
            }
        }

        // Legacy method stubs for backward compatibility (will be removed)
        public void LogAction(string actionType, string details = "") { }
        public void UpdateSessionProjectInfo(string model, string part) => UpdateProjectInfo(model, part);
        public void UpdateSessionEngineerInfo(string engineerName) => UpdateEngineerInfo(engineerName);
        public void UpdateSessionConfiguration(string machineType, MaterialGuideType guideType, int stationCount)
            => UpdateConfiguration(machineType, guideType, stationCount);
        public void LogValidationError(string errorType, string errorMessage) { }
    }
}
