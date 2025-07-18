using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Model
{
    public class CommonPlateRight: CommonPlateBase
    {
        public const string TEMPLATE_LOWCOMPLTRIGHT_NAME = "3DA_Template_LOWCOMPLT_RIGHT-V00.prt";
        public const string LOWCOMPLT_RIGHT = "LowCommonPlateRight";

        public CommonPlateRight(double length, double width, double thickness, string fileName = null) : base(length, width, thickness, fileName)
        {
        }

        public override void CreateNewCommonPlate(string folderPath)
        {
            Session session = Session.GetSession();
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = TEMPLATE_LOWCOMPLTRIGHT_NAME;
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = NXDrawing.MODEL_TEMPLATE;
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = LOWCOMPLT_RIGHT;
            fileNew.SetCanCreateAltrep(false);
            fileNew.NewFileName = $"{folderPath}{GetFileName()}{NXDrawing.EXTENSION}";
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
            workPart.Expressions.EditExpression(expressionWidth, GetWidth().ToString());
            workPart.Expressions.EditExpression(expressionLength, GetLength().ToString());
            workPart.Expressions.EditExpression(expressionThk, GetThickness().ToString());

            NXOpen.Session.UndoMarkId undoMark = session.SetUndoMark(Session.MarkVisibility.Invisible, "Create Low Common Plate");
            session.UpdateManager.DoUpdate(undoMark);

            BasePart.SaveComponents saveComponentParts = BasePart.SaveComponents.True;
            BasePart.CloseAfterSave close = BasePart.CloseAfterSave.True;
            workPart.Save(saveComponentParts, close);
        }
    }   
}
