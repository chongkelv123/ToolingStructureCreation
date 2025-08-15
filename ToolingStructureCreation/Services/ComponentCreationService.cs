using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            }
            catch (NXOpen.NXException nxEx) when (nxEx.Message.Contains("File already exists"))
            {
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
                string message = $"NX Error creating component '{config.FileName}':\n{nxEx.Message}";
                NXDrawing.ShowMessageBox("NX Operation Error", NXOpen.NXMessageBox.DialogType.Error, message);
                throw new InvalidOperationException($"Failed to create component '{config.FileName}'", nxEx);
            }
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
