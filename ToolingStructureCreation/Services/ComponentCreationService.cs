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

                // **UPDATED: CSV-based component logging with enhanced type classification**
                LogComponentWithTypeClassification(config.ItemName, fileSize, processingTime, config);
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

        /// <summary>
        /// Enhanced component logging with detailed type classification for CSV tracking
        /// </summary>
        public static void LogComponentWithTypeClassification(string itemName, long fileSize, double processingTime, ComponentCreationConfig config = null)
        {
            // Determine component type based on multiple factors for accurate classification
            string componentType = ClassifyComponentType(itemName, config);

            // Log to CSV-based tracking system
            UsageTrackingService.Instance.LogComponentCreated(componentType, fileSize, processingTime);
        }

        /// <summary>
        /// Classifies component types for accurate PlateShoe vs Assembly counting
        /// </summary>
        private static string ClassifyComponentType(string itemName, ComponentCreationConfig config)
        {
            // Normalize item name for comparison
            string normalizedName = itemName?.ToUpper().Replace(" ", "_") ?? "";

            // **ASSEMBLY COMPONENTS** (Complex assemblies requiring significant manual effort)
            var assemblyIndicators = new[]
            {
                "ASSEMBLY", "ASM", "MAIN", "STATION", "TOOLING",
                "MAINASSEMBLY", "TOOLINGASSEMBLY", "STATIONASSEMBLY",
                "STN1-ASSEMBLY", "STN2-ASSEMBLY", "STN3-ASSEMBLY", "STN4-ASSEMBLY"
            };

            if (assemblyIndicators.Any(indicator => normalizedName.Contains(indicator)))
            {
                return "Assembly"; // Will be counted as AssemblyCount
            }

            // **PLATE/SHOE COMPONENTS** (Individual components with standardized creation)
            var plateShoeIndicators = new[]
            {
                // Plates
                "UPPER_PAD", "PUNCH_HOLDER", "BOTTOMING_PLATE", "STRIPPER_PLATE",
                "DIE_PLATE", "LOWER_PAD", "PLATE",
                
                // Shoes and structural components  
                "UPPER_SHOE", "LOWER_SHOE", "SHOE",
                "PARALLEL_BAR", "PARALLELBAR",
                "COMMON_PLATE", "COMMONPLATE", "LOWER_COMMON_PLATE",
                
                // Material guides and inserts
                "MATERIAL_GUIDE", "MATGUIDE", "INSERT"
            };

            if (plateShoeIndicators.Any(indicator => normalizedName.Contains(indicator)))
            {
                return "PlateShoe"; // Will be counted as PlateShoeCount
            }

            // **FALLBACK CLASSIFICATION**
            // Use template filename and part properties type as secondary indicators
            if (config.PartPropertiesType == PartProperties.ASM)
            {
                return "Assembly";
            }

            if (config.PartPropertiesType == PartProperties.PLATE ||
                config.PartPropertiesType == PartProperties.SHOE ||
                config.PartPropertiesType == PartProperties.INSERT)
            {
                return "PlateShoe";
            }

            // **DEFAULT**: Most individual components are PlateShoe type
            return "PlateShoe";
        }

        // Add this helper method to ComponentCreationService:
        public static long GetFileSize(string folderPath, string fileName)
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
