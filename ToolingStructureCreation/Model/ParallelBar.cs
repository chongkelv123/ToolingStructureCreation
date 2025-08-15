using NXOpen;
using NXOpen.Features.ShipDesign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NXOpen.Motion.HydrodynamicBearingBuilder;

namespace ToolingStructureCreation.Model
{
    public class ParallelBar
    {
        private string fileName;
        private double length;
        private double width;
        private double thickness;
        public int Quantity { get; set; }

        public const string TEMPLATE_PARALLELBAR_NAME = "3DA_Template_PARALLELBAR-V00.prt";
        public const string PBAR_PRESENTATION_NAME = "ParallelBar";

        public const string PARALLEL_BAR = "PARALLEL_BAR";

        public ParallelBar(string fileName, double length, double width, double thickness)
        {
            this.fileName = fileName;
            this.length = length;
            this.width = width;
            this.thickness = thickness;
        }

        public double GetParallelBarLength()
        {
            return length;
        }

        public double GetParallelBarWidth()
        {
            return width;
        }

        public double GetParallelBarThickness()
        {
            return thickness;
        }
        public void CreateNewParallelBar(string folderPath, ProjectInfo projectInfo, string drawingCode, string itemName)
        {
            Session session = Session.GetSession();
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = TEMPLATE_PARALLELBAR_NAME;
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = NXDrawing.MODEL_TEMPLATE;
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = PBAR_PRESENTATION_NAME;
            fileNew.SetCanCreateAltrep(false);
            fileNew.NewFileName = $"{folderPath}{fileName}{NXDrawing.EXTENSION}";
            fileNew.MakeDisplayedPart = true;
            fileNew.DisplayPartOption = NXOpen.DisplayPartOption.AllowAdditional;

            try
            {
                NXObject plateObject = fileNew.Commit();

                Part workPart = session.Parts.Work;
                Part displayPart = session.Parts.Display;

                fileNew.Destroy();

                session.ApplicationSwitchImmediate(NXDrawing.UG_APP_MODELING);

                NXOpen.Expression expressionWidth = ((NXOpen.Expression)workPart.Expressions.FindObject("Width"));
                NXOpen.Expression expressionLength = ((NXOpen.Expression)workPart.Expressions.FindObject("Length"));
                NXOpen.Expression expressionThk = ((NXOpen.Expression)workPart.Expressions.FindObject("Thk"));
                if (expressionWidth == null)
                {
                    NXDrawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'Width' not found.");
                    return;
                }
                else if (expressionLength == null)
                {
                    NXDrawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'Length' not found.");
                    return;
                }
                else if (expressionThk == null)
                {
                    NXDrawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'Thk' not found.");
                    return;
                }
                workPart.Expressions.EditExpression(expressionWidth, GetParallelBarWidth().ToString());
                workPart.Expressions.EditExpression(expressionLength, GetParallelBarLength().ToString());
                workPart.Expressions.EditExpression(expressionThk, GetParallelBarThickness().ToString());

                NXOpen.Session.UndoMarkId undoMark = session.SetUndoMark(Session.MarkVisibility.Invisible, "Create Parallel Bar");
                session.UpdateManager.DoUpdate(undoMark);

                NXDrawing.UpdatePartProperties(
                    projectInfo,
                drawingCode,
                    itemName,
                    length.ToString("F1"),
                    thickness.ToString("F2"),
                    width.ToString("F1"),
                    NXDrawing.HYPHEN,
                    NXDrawing.S50C,
                    PartProperties.SHOE);

                BasePart.SaveComponents saveComponentParts = BasePart.SaveComponents.True;
                BasePart.CloseAfterSave close = BasePart.CloseAfterSave.True;
                workPart.Save(saveComponentParts, close);
            }
            catch (NXOpen.NXException nxEx) when (nxEx.Message.Contains("File already exists"))
            {                
                // User-friendly error handling
                string message = $"File already exists: {fileName}{NXDrawing.EXTENSION}\n\n" +
                                $"Location: {folderPath}\n\n" +
                                "Please:\n" +
                                "• Delete the existing file, or\n" +
                                "• Choose a different output directory, or\n" +
                                "• Modify the project code prefix";

                string title = "File Conflict";
                NXDrawing.ShowMessageBox(title, NXOpen.NXMessageBox.DialogType.Warning, message);

                // Re-throw to stop the creation process
                throw new InvalidOperationException($"Cannot create plate '{fileName}' - file already exists", nxEx);
            }
            catch (NXOpen.NXException nxEx)
            {                
                // Handle other NX-specific errors
                string message = $"NX Error creating plate '{fileName}':\n{nxEx.Message}";
                string title = "NX Operation Error";
                NXDrawing.ShowMessageBox(title, NXOpen.NXMessageBox.DialogType.Error, message);

                throw new InvalidOperationException($"Failed to create plate '{fileName}'", nxEx);
            }
            catch (Exception ex)
            {                
                // Handle unexpected errors
                string message = $"Unexpected error creating plate '{fileName}':\n{ex.Message}";
                string title = "Unexpected Error";
                NXDrawing.ShowMessageBox(title, NXOpen.NXMessageBox.DialogType.Error, message);

                throw;
            }

        }
    }
}
