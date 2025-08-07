using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ToolingStructureCreation.Infrastructure.FileSystem
{
    /// <summary>
    /// File system operations for tooling structure creation   
    /// </summary>
    public class FileSystemService
    {
        /// <summary>
        /// Create project directory structure
        /// </summary>
        public string CreateProjectDirectory(string baseDirectory, string projectName)
        {
            if (string.IsNullOrWhiteSpace(baseDirectory))
                throw new ArgumentException("Base directory cannot be empty", nameof(baseDirectory));

            if (string.IsNullOrWhiteSpace(projectName))
                throw new ArgumentException("Project name cannot be empty", nameof(projectName));

            var projectPath = Path.Combine(baseDirectory, projectName);

            // Create main project directory only
            Directory.CreateDirectory(projectPath);

            return projectPath;
        }

        /// <summary>
        /// Get file path in project directory
        /// </summary>
        public string GetComponentPath(string projectPath, string fileName)
        {
            return Path.Combine(projectPath, fileName);
        }

        /// <summary>
        /// Validate template file exists
        /// </summary>
        public void ValidateTemplate(string templatePath)
        {
            if (string.IsNullOrWhiteSpace(templatePath))
                throw new ArgumentException("Template path cannot be empty", nameof(templatePath));

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Template file not found: {templatePath}");
        }

        /// <summary>
        /// Ensure directory exists, create if not
        /// </summary>
        public void EnsureDirectoryExists(string directoryPath)
        {
            if (!string.IsNullOrWhiteSpace(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        /// <summary>
        /// Generate unique file name if file already exists
        /// </summary>
        public string GetUniqueFileName(string filePath)
        {
            if (!File.Exists(filePath))
                return filePath;

            var directory = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var extension = Path.GetExtension(filePath);

            var counter = 1;
            string newFilePath;

            do
            {
                var newFileName = $"{fileName}_{counter:D2}{extension}";
                newFilePath = Path.Combine(directory, newFileName);
                counter++;
            }
            while (File.Exists(newFilePath) && counter < 100);

            if (counter >= 100)
                throw new InvalidOperationException($"Could not generate unique filename for: {filePath}");

            return newFilePath;
        }

        /// <summary>
        /// Copy template file to target location
        /// </summary>
        public void CopyTemplate(string templatePath, string targetPath, bool overwrite = true)
        {
            ValidateTemplate(templatePath);

            var targetDirectory = Path.GetDirectoryName(targetPath);
            EnsureDirectoryExists(targetDirectory);

            File.Copy(templatePath, targetPath, overwrite);
        }

        /// <summary>
        /// Get available templates in directory
        /// </summary>
        public List<string> GetAvailableTemplates(string templateDirectory)
        {
            var templates = new List<string>();

            if (Directory.Exists(templateDirectory))
            {
                templates.AddRange(Directory.GetFiles(templateDirectory, "*.prt"));
            }

            return templates;
        }

        /// <summary>
        /// Clean up temporary files
        /// </summary>
        public void CleanupTempFiles(string projectPath)
        {
            var tempPatterns = new[] { "*.tmp", "*.bak", "*~" };

            foreach (var pattern in tempPatterns)
            {
                var tempFiles = Directory.GetFiles(projectPath, pattern, SearchOption.AllDirectories);
                foreach (var tempFile in tempFiles)
                {
                    try
                    {
                        File.Delete(tempFile);
                    }
                    catch
                    {
                        // Ignore cleanup errors
                    }
                }
            }
        }

    }
}
