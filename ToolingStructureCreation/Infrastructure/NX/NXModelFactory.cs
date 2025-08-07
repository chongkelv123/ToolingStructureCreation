using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using ToolingStructureCreation.Domain.Aggregates;
using ToolingStructureCreation.Domain.Entities;
using ToolingStructureCreation.Domain.Enums;

namespace ToolingStructureCreation.Infrastructure.NX
{
    /// <summary>
    /// Creates NX part files and components from domain entities
    /// </summary>
    public class NXModelFactory
    {
        private readonly NXSessionManager _sessionManager;
        private readonly string _templateBasePath;

        public NXModelFactory(NXSessionManager sessionManager, string templateBasePath)
        {
            _sessionManager = sessionManager ?? throw new ArgumentNullException(nameof(sessionManager));
            _templateBasePath = templateBasePath ?? throw new ArgumentNullException(nameof(templateBasePath));
        }

        /// <summary>
        /// Create NX part from plate entity
        /// </summary>
        public Part CreatePlate(Plate plate, string outputDirectory, string projectCode)
        {
            _sessionManager.ValidateSession();

            var templatePath = GetPlateTemplatePath(plate.Type);
            var fileName = GeneratePlateFilename(plate, projectCode);
            var fullPath = Path.Combine(outputDirectory, fileName);

            var part = CreatePartFromTemplate(templatePath, fullPath, plate.Name);
            UpdatePlateDimensions(part, plate);
            ApplyPlateColor(part, plate.Type);

            return part;
        }

        // <summary>
        /// Create NX part from shoe entity
        /// </summary>
        public Part CreateShoe(Shoe shoe, string outputDirectory, string projectCode)
        {
            _sessionManager.ValidateSession();

            var templatePath = GetShoeTemplatePath(shoe.Type);
            var fileName = GenerateShoeFileName(shoe, projectCode);
            var fullPath = Path.Combine(outputDirectory, fileName);

            var part = CreatePartFromTemplate(templatePath, fullPath, shoe.Name);
            UpdateShoeDimensions(part, shoe);

            return part;
        }

        /// <summary>
        /// Create NX part from parallel bar entity
        /// </summary>
        public Part CreateParallelBar(ParallelBar parallelBar, string outputDirectory, string projectCode)
        {
            _sessionManager.ValidateSession();

            var templatePath = GetParallelBarTemplatePath();
            var fileName = GenerateParallelBarFileName(parallelBar, projectCode);
            var fullPath = Path.Combine(outputDirectory, fileName);

            var part = CreatePartFromTemplate(templatePath, fullPath, parallelBar.Name);
            UpdateParallelBarDimensions(part, parallelBar);

            return part;
        }

        /// <summary>
        /// Create NX part from common plate entity
        /// </summary>
        public Part CreateCommonPlate(CommonPlate commonPlate, string outputDirectory, string projectCode)
        {
            _sessionManager.ValidateSession();

            var templatePath = GetCommonPlateTemplatePath(commonPlate.Type);
            var fileName = GenerateCommonPlateFileName(commonPlate, projectCode);
            var fullPath = Path.Combine(outputDirectory, fileName);

            var part = CreatePartFromTemplate(templatePath, fullPath, commonPlate.Name);
            UpdateCommonPlateDimensions(part, commonPlate);

            return part;
        }

        /// <summary>
        /// Create station assembly from StationAggregate
        /// </summary>
        public Part CreateStationAssembly(StationAggregate station, string outputDirectory, string projectCode)
        {
            _sessionManager.ValidateSession();

            var assemblyName = $"STATION_{station.StationNumber:D2}";
            var fileName = $"{projectCode}_{assemblyName}.prt";
            var fullPath = Path.Combine(outputDirectory, fileName);

            // Create assembly part
            var undoMark = _sessionManager.CreateUndoMark("Create Station Assembly");

            var session = _sessionManager.NXSession;
            var newPart = session.Parts.NewDisplay(fullPath, Part.Units.Millimeters);

            // Convert to assembly
            session.ApplicationSwitchImmediate("UG_APP_MODELING");

            _sessionManager.UpdateModel(undoMark);

            return newPart;
        }

        private Part CreatePartFromTemplate(string templatePath, string outputPath, string partName)
        {
            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Template not found: {templatePath}");

            var undoMark = _sessionManager.CreateUndoMark($"Create part from template: {partName}");
            var session = _sessionManager.NXSession;

            // Copy template to new location
            File.Copy(templatePath, outputPath, true);

            // Open the copied part
            var part = session.Parts.OpenActiveDisplay(outputPath, DisplayPartOption.AllowAdditional, out var status);

            if (status != null)
            {
                // Update part
                _sessionManager.UpdateModel(undoMark);
            }

            return part as Part;
        }

        private void UpdatePlateDimensions(Part part, Plate plate)
        {
            var expressions = part.Expressions;

            // Update standard plate expressions
            UpdateExpression(expressions, "Length", plate.Dimensions.Length);
            UpdateExpression(expressions, "Width", plate.Dimensions.Width);
            UpdateExpression(expressions, "Thk", plate.Dimensions.Thickness);
        }

        private void UpdateShoeDimensions(Part part, Shoe shoe)
        {
            var expressions = part.Expressions;

            UpdateExpression(expressions, "Length", shoe.Dimensions.Length);
            UpdateExpression(expressions, "Width", shoe.Dimensions.Width);
            UpdateExpression(expressions, "Thk", shoe.Dimensions.Thickness);
        }

        private void UpdateParallelBarDimensions(Part part, ParallelBar parallelBar)
        {
            var expressions = part.Expressions;

            UpdateExpression(expressions, "Length", parallelBar.Dimensions.Length);
            UpdateExpression(expressions, "Width", parallelBar.Dimensions.Width);
            UpdateExpression(expressions, "Thk", parallelBar.Dimensions.Thickness);
        }

        private void UpdateCommonPlateDimensions(Part part, CommonPlate commonPlate)
        {
            var expressions = part.Expressions;

            UpdateExpression(expressions, "Length", commonPlate.Dimensions.Length);
            UpdateExpression(expressions, "Width", commonPlate.Dimensions.Width);
            UpdateExpression(expressions, "Thk", commonPlate.Dimensions.Thickness);
        }

        private void UpdateExpression(ExpressionCollection expressions, string name, double value)
        {
            var expression = expressions.FindObject(name);
            if (expression != null)
            {
                expressions.EditExpression(expression, value.ToString("F3"));
            }
        }

        private void ApplyPlateColor(Part part, PlateType plateType)
        {
            // Color mapping based on business rules
            var colorMap = new Dictionary<PlateType, int>
            {
                { PlateType.Upper_Pad, 186 },        // Light Blue
                { PlateType.Punch_Holder, 78 },      // Orange
                { PlateType.Bottoming_Plate, 211 },  // Yellow
                { PlateType.Stripper_Plate, 36 },    // Green
                { PlateType.Die_Plate, 125 },        // Red
                { PlateType.Lower_Pad, 198 }         // Purple
            };

            if (colorMap.TryGetValue(plateType, out var colorId))
            {
                // Apply color to part bodies
                foreach (Body body in part.Bodies)
                {
                    var displayModification = _sessionManager.NXSession.DisplayManager.NewDisplayModification();
                    displayModification.ApplyToAllFaces = true;
                    displayModification.NewColor = colorId;
                    displayModification.Apply(new DisplayableObject[] { body });
                    displayModification.Dispose();
                }
            }
        }

        private string GetPlateTemplatePath(PlateType plateType)
        {
            return Path.Combine(_templateBasePath, "3DA_Template_STP-V00.prt");
        }

        private string GetShoeTemplatePath(ShoeType shoeType)
        {
            return Path.Combine(_templateBasePath, "3DA_Template_STP-V00.prt");
        }

        private string GetParallelBarTemplatePath()
        {
            return Path.Combine(_templateBasePath, "3DA_Template_STP-V00.prt");
        }

        private string GetCommonPlateTemplatePath(CommonPlateType plateType)
        {
            return Path.Combine(_templateBasePath, "3DA_Template_STP-V00.prt");
        }

        private string GeneratePlateFileName(Plate plate, string projectCode)
        {
            return $"{projectCode}_{plate.Name}.prt";
        }

        private string GenerateShoeFileName(Shoe shoe, string projectCode)
        {
            return $"{projectCode}_{shoe.Name}.prt";
        }

        private string GenerateParallelBarFileName(ParallelBar parallelBar, string projectCode)
        {
            return $"{projectCode}_{parallelBar.Name}.prt";
        }

        private string GenerateCommonPlateFileName(CommonPlate commonPlate, string projectCode)
        {
            return $"{projectCode}_{commonPlate.Name}.prt";
        }
    }
}
