using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Services
{
    /// <summary>
    /// Extracts validation logic from formToolStructure to improve separation of concerns
    /// </summary>
    public class FormValidator
    {
        private readonly List<string> _requiredThicknessFields = new List<string>
        {
            nameof(FormValidationData.UpperShoeThk),
            nameof(FormValidationData.UpperPadThk),
            nameof(FormValidationData.PunHolderThk),
            nameof(FormValidationData.BottomPltThk),
            nameof(FormValidationData.StripperPltThk),
            nameof(FormValidationData.MatThk),
            nameof(FormValidationData.DiePltThk),
            nameof(FormValidationData.LowerPadThk),
            nameof(FormValidationData.LowerShoeThk),
            nameof(FormValidationData.ParallelBarThk),
            nameof(FormValidationData.CommonPltThk)
        };

        /// <summary>
        /// Validates all form inputs for enabling Apply button
        /// </summary>
        public ValidationResult ValidateForApply(FormValidationData data)
        {
            var result = new ValidationResult { IsValid = true };

            // Validate thickness fields
            var thicknessValidation = ValidateThicknessFields(data);
            if (!thicknessValidation.IsValid)
            {
                result.Errors.AddRange(thicknessValidation.Errors);
                result.IsValid = false;
            }

            // Validate sketch selections
            var sketchValidation = ValidateSketchSelections(data);
            if (!sketchValidation.IsValid)
            {
                result.Errors.AddRange(sketchValidation.Errors);
                result.IsValid = false;
            }

            // Validate directory
            var directoryValidation = ValidateDirectory(data.Path);
            if (!directoryValidation.IsValid)
            {
                result.Errors.AddRange(directoryValidation.Errors);
                result.IsValid = false;
            }

            return result;
        }

        /// <summary>
        /// Validates thickness field inputs
        /// </summary>
        public ValidationResult ValidateThicknessFields(FormValidationData data)
        {
            var result = new ValidationResult { IsValid = true };

            foreach (var fieldName in _requiredThicknessFields)
            {
                var value = GetFieldValue(data, fieldName);

                if (string.IsNullOrWhiteSpace(value))
                {
                    result.AddError($"{GetDisplayName(fieldName)} is required");
                    continue;
                }

                if (!double.TryParse(value, out double numericValue))
                {
                    result.AddError($"{GetDisplayName(fieldName)} must be a valid number");
                    continue;
                }

                if (numericValue <= 0)
                {
                    result.AddError($"{GetDisplayName(fieldName)} must be greater than 0");
                }
            }

            return result;
        }

        /// <summary>
        /// Validates sketch selections
        /// </summary>
        public ValidationResult ValidateSketchSelections(FormValidationData data)
        {
            var result = new ValidationResult { IsValid = true };

            if (!data.IsPlateSketchSelected)
            {
                result.AddError("Plate sketch must be selected");
            }

            if (!data.IsShoeSketchSelected)
            {
                result.AddError("Shoe sketch must be selected");
            }

            return result;
        }

        /// <summary>
        /// Validates directory path
        /// </summary>
        public ValidationResult ValidateDirectory(string path)
        {
            var result = new ValidationResult { IsValid = true };

            if (string.IsNullOrWhiteSpace(path))
            {
                result.AddError("Output directory path is required");
                return result;
            }

            if (!Directory.Exists(path))
            {
                result.AddError($"Directory does not exist: {path}");
            }

            return result;
        }

        /// <summary>
        /// Validates project information fields
        /// </summary>
        public ValidationResult ValidateProjectInfo(FormValidationData data)
        {
            var result = new ValidationResult { IsValid = true };

            if (string.IsNullOrWhiteSpace(data.Model))
                result.AddError("Model is required");

            if (string.IsNullOrWhiteSpace(data.Part))
                result.AddError("Part is required");

            if (string.IsNullOrWhiteSpace(data.CodePrefix))
                result.AddError("Code Prefix is required");

            if (string.IsNullOrWhiteSpace(data.Designer))
                result.AddError("Designer is required");

            return result;
        }

        /// <summary>
        /// Validates if a string input is numeric
        /// </summary>
        public bool IsNumericInput(char keyChar, string currentText)
        {
            // Allow control keys (e.g., backspace), digits, and optionally a single decimal point
            if (char.IsControl(keyChar) || char.IsDigit(keyChar))
                return true;

            // Allow decimal point if there isn't one already
            if (keyChar == '.' && !currentText.Contains("."))
                return true;

            return false;
        }

        /// <summary>
        /// Parses thickness value safely
        /// </summary>
        public double ParseThickness(string value)
        {
            return double.TryParse(value, out double result) ? result : 0.0;
        }

        private string GetFieldValue(FormValidationData data, string fieldName)
        {
            var property = typeof(FormValidationData).GetProperty(fieldName);
            return property?.GetValue(data) as string ?? string.Empty;
        }

        private string GetDisplayName(string fieldName)
        {
            // Convert property names to display names
            var displayNames = new Dictionary<string, string>
            {
                { nameof(FormValidationData.UpperShoeThk), "Upper Shoe Thickness" },
                { nameof(FormValidationData.UpperPadThk), "Upper Pad Thickness" },
                { nameof(FormValidationData.PunHolderThk), "Punch Holder Thickness" },
                { nameof(FormValidationData.BottomPltThk), "Bottom Plate Thickness" },
                { nameof(FormValidationData.StripperPltThk), "Stripper Plate Thickness" },
                { nameof(FormValidationData.MatThk), "Material Thickness" },
                { nameof(FormValidationData.DiePltThk), "Die Plate Thickness" },
                { nameof(FormValidationData.LowerPadThk), "Lower Pad Thickness" },
                { nameof(FormValidationData.LowerShoeThk), "Lower Shoe Thickness" },
                { nameof(FormValidationData.ParallelBarThk), "Parallel Bar Thickness" },
                { nameof(FormValidationData.CommonPltThk), "Common Plate Thickness" }
            };

            return displayNames.TryGetValue(fieldName, out string displayName) ? displayName : fieldName;
        }
    }
}
