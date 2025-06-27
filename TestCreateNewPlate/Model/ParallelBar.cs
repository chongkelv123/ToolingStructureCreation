using NXOpen;
using NXOpen.Features.ShipDesign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCreateNewPlate.Model
{
    public class ParallelBar
    {        
        private double parallelBarLength;
        private double parallelBarWidth;
        private double parallelBarThickness;
        public int Quantity { get; set; }
        NXDrawing drawing;

        public const string TEMPLATE_PARALLELBAR_NAME = "3DA_Template_PARALLELBAR-V00.prt";
        public const string PARALLELBAR = "ParallelBar";

        public const string PARALLEL_BAR = "PARALLEL_BAR";

        public ParallelBar(double length, double width, double thickness, NXDrawing drawing)
        {            
            this.parallelBarLength = length;
            this.parallelBarWidth = width;
            this.parallelBarThickness = thickness;
            this.drawing = drawing;
        }

        public double GetParallelBarLength()
        {
            return parallelBarLength;
        }

        public double GetParallelBarWidth()
        {
            return parallelBarWidth;
        }

        public double GetParallelBarThickness()
        {
            return parallelBarThickness;
        }
        public void CreateNewParallelBar(string folderPath)
        {
            Session session = Session.GetSession();
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = TEMPLATE_PARALLELBAR_NAME;
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = NXDrawing.MODEL_TEMPLATE;
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = PARALLELBAR;
            fileNew.SetCanCreateAltrep(false);
            fileNew.NewFileName = $"{folderPath}{PARALLEL_BAR}{NXDrawing.EXTENSION}";
            fileNew.MakeDisplayedPart = true;
            fileNew.DisplayPartOption = NXOpen.DisplayPartOption.AllowAdditional;
            NXObject plateObject;
            plateObject = fileNew.Commit();

            Part workPart = session.Parts.Work;
            Part displayPart = session.Parts.Display;

            fileNew.Destroy();

            session.ApplicationSwitchImmediate(NXDrawing.UG_APP_MODELING);

            NXOpen.Expression expressionWidth = ((NXOpen.Expression)workPart.Expressions.FindObject("Width"));
            NXOpen.Expression expressionLength = ((NXOpen.Expression)workPart.Expressions.FindObject("Length"));
            NXOpen.Expression expressionThk = ((NXOpen.Expression)workPart.Expressions.FindObject("Thk"));
            if (expressionWidth == null)
            {
                drawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'Width' not found.");
                return;
            }
            else if (expressionLength == null)
            {
                drawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'Length' not found.");
                return;
            }
            else if (expressionThk == null)
            {
                drawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'Thk' not found.");
                return;
            }
            workPart.Expressions.EditExpression(expressionWidth, GetParallelBarWidth().ToString());
            workPart.Expressions.EditExpression(expressionLength, GetParallelBarLength().ToString());
            workPart.Expressions.EditExpression(expressionThk, GetParallelBarThickness().ToString());

            NXOpen.Session.UndoMarkId undoMark = session.SetUndoMark(Session.MarkVisibility.Invisible, "Create Parallel Bar");
            session.UpdateManager.DoUpdate(undoMark);

            BasePart.SaveComponents saveComponentParts = BasePart.SaveComponents.True;
            BasePart.CloseAfterSave close = BasePart.CloseAfterSave.True;
            workPart.Save(saveComponentParts, close);
        }
    }
}
