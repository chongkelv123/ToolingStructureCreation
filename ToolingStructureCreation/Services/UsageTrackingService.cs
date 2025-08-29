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
    /// Captures and stores engineer usage data for statistical analysis
    /// </summary>
    public class UsageTrackingService
    {
        private static UsageTrackingService _instance;
        private UsageSession _currentSession;
        private readonly string _logDirectory;
        private readonly string _logFileName;

        private UsageTrackingService()
        {
            _logDirectory = @"\\SPL-SMB\DE\Common\SGA\3DA setup folder\logs\usage_logs";
            _logFileName = $"tooling_usage_{DateTime.Now:yyyy-MM}.json";
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
        /// Updates session with project information (MODEL/PART)
        /// Called when Apply button is clicked
        /// </summary>
        public void UpdateSessionProjectInfo(string model, string part)
        {
            if (_currentSession == null) return;

            _currentSession.Model = model;
            _currentSession.Part = part;

            LogAction("PROJECT_INFO_UPDATE", $"Model:{model}|Part:{part}");

        }

        /// <summary>
        /// Starts a new usage tracking session
        /// </summary>
        public void StartSession(string engineerName, string moduleVersion)
        {
            _currentSession = new UsageSession
            {
                SessionId = Guid.NewGuid().ToString(),
                StartTime = DateTime.Now,
                EngineerName = engineerName,
                ModuleVersion = moduleVersion,
                Components = new List<ComponentCreated>(),
                Actions = new List<UserAction>(),
                Metrics = new ProjectMetrics()
            };

            LogAction("SESSION_START", "New session initiated");
        }

        /// <summary>
        /// Records user actions within the session
        /// </summary>
        public void LogAction(string actionType, string details = "")
        {
            if (_currentSession == null) return;

            _currentSession.Actions.Add(new UserAction
            {
                Timestamp = DateTime.Now,
                ActionType = actionType,
                Details = details ?? ""
            });
        }

        /// <summary>
        /// Records component creation events
        /// </summary>
        public void LogComponentCreated(string componentType, long fileSizeBytes, double processingTimeMs)
        {
            if (_currentSession == null) return;

            _currentSession.Components.Add(new ComponentCreated
            {
                ComponentType = componentType,
                CreatedAt = DateTime.Now,
                FileSizeBytes = fileSizeBytes,
                ProcessingTimeMs = processingTimeMs
            });

            _currentSession.Metrics.TotalComponents++;
            _currentSession.Metrics.TotalProcessingTimeMs += processingTimeMs;
            _currentSession.Metrics.TotalFileSizeBytes += fileSizeBytes;
        }

        /// <summary>
        /// Updates session configuration data
        /// </summary>
        public void UpdateSessionConfiguration(string machineType, MaterialGuideType guideType, int stationCount)
        {
            if (_currentSession == null) return;

            _currentSession.MachineType = machineType;
            _currentSession.GuideType = guideType;
            _currentSession.StationCount = stationCount;

            LogAction("CONFIG_UPDATE", $"Machine: {machineType}, Guide: {guideType}, Stations: {stationCount}");
        }

        /// <summary>
        /// Records validation errors for analysis
        /// </summary>
        public void LogValidationError(string errorType, string errorMessage)
        {
            LogAction("VALIDATION_ERROR", $"{errorType}: {errorMessage}");
        }

        /// <summary>
        /// Ends the current session and saves to file
        /// </summary>
        public void EndSession(bool completedSuccessfully)
        {
            if (_currentSession == null) return;

            _currentSession.EndTime = DateTime.Now;
            _currentSession.CompletedSuccessfully = completedSuccessfully;
            _currentSession.Metrics.SessionDurationMs =
                (_currentSession.EndTime.Value - _currentSession.StartTime).TotalMilliseconds;

            LogAction("SESSION_END", $"Success: {completedSuccessfully}");
            SaveSessionToFile();
            _currentSession = null;
        }

        private void SaveSessionToFile()
        {
            try
            {
                string filePath = Path.Combine(_logDirectory, _logFileName);
                List<UsageSession> existingSessions = LoadExistingSessions(filePath);

                existingSessions.Add(_currentSession);

                string jsonContent = JsonConvert.SerializeObject(existingSessions, Formatting.Indented);
                File.WriteAllText(filePath, jsonContent);
            }
            catch (Exception ex)
            {
                // Silent fail - don't disrupt user workflow
                LogToErrorFile($"Usage tracking error: {ex.Message}");
            }
        }

        private List<UsageSession> LoadExistingSessions(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<UsageSession>();

            try
            {
                string content = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<UsageSession>>(content)
                       ?? new List<UsageSession>();
            }
            catch
            {
                return new List<UsageSession>();
            }
        }

        private void EnsureLogDirectoryExists()
        {
            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);
        }

        private void LogToErrorFile(string error)
        {
            try
            {
                string errorFile = Path.Combine(_logDirectory, "tracking_errors.log");
                File.AppendAllText(errorFile, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: {error}\n");
            }
            catch
            {
                // Ultimate silent fail
            }
        }
    }
}
