using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ToolingStructureCreation.Model;

namespace ToolingStructureCreation.Services
{
    /// <summary>
    /// Unified service to eliminate ~650 lines of duplicated code across
    /// component creation methods (Plate, Shoe, CommonPlate, etc.)
    /// </summary>
    public class ComponentCreationService
    {
        /// <summary>
        /// Creates NX component using template method pattern to eliminate duplication
        /// </summary>
        public void CreateComponent(ComponentCreationConfig config)
        {
            var startTime = DateTime.Now;
            Session session = Session.GetSession();
            FileNew fileNew = session.Parts.FileNew();

            // Phase 1: File Setup (common across all components)
            ConfigureFileNew(fileNew, config);

            try
            {
                // Phase 2: File Commit & Basic Setup (common)
                NXObject componentObject = fileNew.Commit();
                Part workPart = session.Parts.Work;
                Part displayPart = session.Parts.Display;
                fileNew.Destroy();
                session.ApplicationSwitchImmediate(NXDrawing.UG_APP_MODELING);

                // Phase 3: Expression Management (common pattern)
                UpdateComponentExpressions(workPart, config);

                int stnNumber = CodeGeneratorService.GetStationNumber(config.DrawingCode);
                if (config.IsMatGuideFull && stnNumber > 1)
                {
                    // Phase 3A: Expression (materual guide full)
                    UpdateMatGuideExpressions(workPart, config);
                }

                // Phase 4: Update Operations (common)
                UpdateComponent(session, config);

                // Phase 5: Color Assignment (delegated to specific component logic)
                if (config.ColorAssignmentAction != null)
                {
                    config.ColorAssignmentAction(workPart, config.FileName);
                }

                // Phase 6: Part Properties Update (common pattern)
                UpdatePartProperties(config);

                // Phase 7: Save Operations (common)
                SaveComponent(workPart);

                // Calculate performance metrics
                var processingTime = (DateTime.Now - startTime).TotalMilliseconds;
                long fileSize = GetFileSize(config.FolderPath, config.FileName);

                // Log component creation success
                UsageTrackingService.Instance.LogComponentCreated(
                    config.ItemName,
                    fileSize,
                    processingTime);

                UsageTrackingService.Instance.LogAction("COMPONENT_CREATED",
                    $"{config.ItemName} created in {processingTime:F0}ms, size: {fileSize} bytes");
            }
            catch (NXOpen.NXException nxEx) when (nxEx.Message.Contains("File already exists"))
            {
                var processingTime = (DateTime.Now - startTime).TotalMilliseconds;

                UsageTrackingService.Instance.LogAction("COMPONENT_ERROR",
                    $"File exists error for {config.ItemName} after {processingTime:F0}ms");

                string message = $"File already exists: {config.FileName}{NXDrawing.EXTENSION}\n\n" +
                               $"Location: {config.FolderPath}\n\n" +
                               "Please:\n" +
                               "• Delete the existing file, or\n" +
                               "• Choose a different output directory, or\n" +
                               "• Modify the project code prefix";

                NXDrawing.ShowMessageBox("File Conflict", NXOpen.NXMessageBox.DialogType.Warning, message);
                throw new InvalidOperationException($"Cannot create component '{config.FileName}' - file already exists", nxEx);
            }
            catch (NXOpen.NXException nxEx)
            {
                var processingTime = (DateTime.Now - startTime).TotalMilliseconds;

                UsageTrackingService.Instance.LogAction("COMPONENT_ERROR",
                    $"NX error for {config.ItemName} after {processingTime:F0}ms: {nxEx.Message}");

                string message = $"NX Error creating component '{config.FileName}':\n{nxEx.Message}";
                NXDrawing.ShowMessageBox("NX Operation Error", NXOpen.NXMessageBox.DialogType.Error, message);
                throw new InvalidOperationException($"Failed to create component '{config.FileName}'", nxEx);
            }
            catch (Exception ex)
            {
                var processingTime = (DateTime.Now - startTime).TotalMilliseconds;

                UsageTrackingService.Instance.LogAction("COMPONENT_ERROR",
                    $"Unexpected error for {config.ItemName} after {processingTime:F0}ms: {ex.Message}");
                throw;
            }
        }

        // Add this helper method to ComponentCreationService:
        private long GetFileSize(string folderPath, string fileName)
        {
            try
            {
                string fullPath = Path.Combine(folderPath, fileName + NXDrawing.EXTENSION);
                if (File.Exists(fullPath))
                {
                    return new FileInfo(fullPath).Length;
                }
            }
            catch 
            {
                // Silent fail for file size calculation
            }
            return 0;
        }

        private void UpdateMatGuideExpressions(Part workPart, ComponentCreationConfig config)
        {
            string angle = "45.0"; // Default angle for relief, can be parameterized if needed
            string dist = "2.0"; // Default distance for relief, can be parameterized if needed

            // Find expressions
            var reliefAngle = workPart.Expressions.FindObject("ReliefAngle") as Expression;
            var reliefDist = workPart.Expressions.FindObject("ReliefDist") as Expression;

            // Validate expressions exist (common error handling pattern)
            if (reliefAngle == null)
            {
                NXDrawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'Relief Angle' not found.");
                return;
            }
            if (reliefDist == null)
            {
                NXDrawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'Relief Distance' not found.");
                return;
            }

            // Update expressions (common pattern)
            workPart.Expressions.EditExpression(reliefAngle, angle);
            workPart.Expressions.EditExpression(reliefDist, dist);
        }

        private void ConfigureFileNew(FileNew fileNew, ComponentCreationConfig config)
        {
            fileNew.TemplateFileName = config.TemplateFileName;
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = NXDrawing.MODEL_TEMPLATE;
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = config.PresentationName;
            fileNew.SetCanCreateAltrep(false);
            fileNew.NewFileName = $"{config.FolderPath}{config.FileName}{NXDrawing.EXTENSION}";
            fileNew.MakeDisplayedPart = true;
            fileNew.DisplayPartOption = NXOpen.DisplayPartOption.AllowAdditional;
        }

        private void UpdateComponentExpressions(Part workPart, ComponentCreationConfig config)
        {
            // Find expressions (common pattern across all components)
            var expressionWidth = workPart.Expressions.FindObject("Width") as Expression;
            var expressionLength = workPart.Expressions.FindObject("Length") as Expression;
            var expressionThk = workPart.Expressions.FindObject("Thk") as Expression;

            // Validate expressions exist (common error handling pattern)
            if (expressionWidth == null)
            {
                NXDrawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'Width' not found.");
                return;
            }
            if (expressionLength == null)
            {
                NXDrawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'Length' not found.");
                return;
            }
            if (expressionThk == null)
            {
                NXDrawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'Thk' not found.");
                return;
            }

            // Update expressions (common pattern)
            workPart.Expressions.EditExpression(expressionWidth, config.Width.ToString());
            workPart.Expressions.EditExpression(expressionLength, config.Length.ToString());
            workPart.Expressions.EditExpression(expressionThk, config.Thickness.ToString());
        }

        private void UpdateComponent(Session session, ComponentCreationConfig config)
        {
            Session.UndoMarkId undoMark = session.SetUndoMark(Session.MarkVisibility.Invisible, config.UndoDescription);
            session.UpdateManager.DoUpdate(undoMark);
        }

        private void UpdatePartProperties(ComponentCreationConfig config)
        {
            string thicknessFormat = config.PartPropertiesType == PartProperties.SHOE ? "F1" : "F2";

            NXDrawing.UpdatePartProperties(
                config.ProjectInfo,
                config.DrawingCode,
                config.ItemName,
                config.Length.ToString("F1"),
                config.Thickness.ToString(thicknessFormat),
                config.Width.ToString("F1"),
                config.HardnessOrGrade,
                config.Material,
                config.PartPropertiesType
            );
        }

        private void SaveComponent(Part workPart)
        {
            BasePart.SaveComponents saveComponentParts = BasePart.SaveComponents.True;
            BasePart.CloseAfterSave close = BasePart.CloseAfterSave.True;
            workPart.Save(saveComponentParts, close);
        }
    }
}
