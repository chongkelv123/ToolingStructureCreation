using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using NXOpen.Features.ShipDesign;
using ToolingStructureCreation.Domain.Repositories;
using ToolingStructureCreation.Infrastructure.FileSystem;
using ToolingStructureCreation.Domain.Aggregates;
using ToolingStructureCreation.Domain.ValueObjects;

namespace ToolingStructureCreation.Infrastructure.Repositories
{
    /// <summary>
    /// File-based repository for tooling structure persistence
    /// </summary>
    public class ToolingStructureRepository : IToolingStructureRepository
    {
        private readonly FileSystemService _fileSystemService;
        private readonly string _dataDirectory;
        private readonly JsonSerializerSettings _jsonSettings;

        public ToolingStructureRepository(FileSystemService fileSystemService, string dataDirectory)
        {
            _fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
            _dataDirectory = dataDirectory ?? throw new ArgumentNullException(nameof(dataDirectory));

            _jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };

            _fileSystemService.EnsureDirectoryExists(_dataDirectory);
        }

        public async Task<ToolingStructureAggregate> GetByDrawingCodeAsync(DrawingCode drawingCode)
        {
            if (drawingCode == null)
                throw new ArgumentNullException(nameof(drawingCode));

            var filePath = GetFilePath(drawingCode);

            if (!File.Exists(filePath))
                return null;

            var json = await Task.Run(() => File.ReadAllText(filePath));
            return JsonConvert.DeserializeObject<ToolingStructureAggregate>(json, _jsonSettings);
        }

        public async Task<List<ToolingStructureAggregate>> GetByProjectNameAsync(string projectName)
        {
            if (string.IsNullOrWhiteSpace(projectName))
                throw new ArgumentException("Project name cannot be empty", nameof(projectName));

            var allFiles = Directory.GetFiles(_dataDirectory, "*.json");
            var results = new List<ToolingStructureAggregate>();

            foreach (var file in allFiles)
            {
                try
                {
                    var json = await Task.Run(() => File.ReadAllText(file));
                    var tooling = JsonConvert.DeserializeObject<ToolingStructureAggregate>(json, _jsonSettings);

                    if (tooling?.ProjectName?.Equals(projectName, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        results.Add(tooling);
                    }
                }
                catch
                {
                    // Skip corrupted files
                    continue;
                }
            }

            return results;
        }

        public async Task<List<ToolingStructureAggregate>> GetByDesignerAsync(string designer)
        {
            if (string.IsNullOrWhiteSpace(designer))
                throw new ArgumentException("Designer cannot be empty", nameof(designer));

            var allFiles = Directory.GetFiles(_dataDirectory, "*.json");
            var results = new List<ToolingStructureAggregate>();

            foreach (var file in allFiles)
            {
                try
                {
                    var json = await Task.Run(() => File.ReadAllText(file));
                    var tooling = JsonConvert.DeserializeObject<ToolingStructureAggregate>(json, _jsonSettings);

                    if (tooling?.Designer?.Equals(designer, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        results.Add(tooling);
                    }
                }
                catch
                {
                    // Skip corrupted files
                    continue;
                }
            }

            return results;
        }

        public async Task SaveAsync(ToolingStructureAggregate toolingStructure)
        {
            if (toolingStructure == null)
                throw new ArgumentNullException(nameof(toolingStructure));

            var filePath = GetFilePath(toolingStructure.BaseDrawingCode);
            var json = JsonConvert.SerializeObject(toolingStructure, _jsonSettings);

            // Create backup if file exists
            if (File.Exists(filePath))
            {
                var backupPath = filePath + ".bak";
                File.Copy(filePath, backupPath, true);
            }

            await Task.Run(() => File.WriteAllText(filePath, json));
        }

        public async Task UpdateAsync(ToolingStructureAggregate toolingStructure)
        {
            if (toolingStructure == null)
                throw new ArgumentNullException(nameof(toolingStructure));

            if (!await ExistsAsync(toolingStructure.BaseDrawingCode))
                throw new InvalidOperationException($"Tooling structure {toolingStructure.BaseDrawingCode} does not exist");

            await SaveAsync(toolingStructure);
        }

        public async Task DeleteAsync(DrawingCode drawingCode)
        {
            if (drawingCode == null)
                throw new ArgumentNullException(nameof(drawingCode));

            var filePath = GetFilePath(drawingCode);

            if (File.Exists(filePath))
            {
                // Create backup before delete
                var backupPath = filePath + ".deleted";
                File.Move(filePath, backupPath);
            }

            await Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(DrawingCode drawingCode)
        {
            if (drawingCode == null)
                return false;

            var filePath = GetFilePath(drawingCode);
            return File.Exists(filePath);
        }

        public async Task<List<DrawingCode>> GetAllDrawingCodesAsync()
        {
            var allFiles = Directory.GetFiles(_dataDirectory, "*.json");
            var drawingCodes = new List<DrawingCode>();

            foreach (var file in allFiles)
            {
                try
                {
                    var json = await Task.Run(() => File.ReadAllText(file));
                    var tooling = JsonConvert.DeserializeObject<ToolingStructureAggregate>(json, _jsonSettings);

                    if (tooling?.BaseDrawingCode != null)
                    {
                        drawingCodes.Add(tooling.BaseDrawingCode);
                    }
                }
                catch
                {
                    // Skip corrupted files
                    continue;
                }
            }

            return drawingCodes.OrderBy(dc => dc.ToString()).ToList();
        }

        private string GetFilePath(DrawingCode drawingCode)
        {
            var fileName = $"{drawingCode.ToString().Replace("/", "_")}.json";
            return Path.Combine(_dataDirectory, fileName);
        }
    }
}
