﻿using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NXOpen.Motion.HydrodynamicBearingBuilder;

namespace ToolingStructureCreation.Model
{
    public class CommonPlate: CommonPlateBase
    {
        public const string TEMPLATE_LOWCOMPLT_NAME = "3DA_Template_LOWCOMPLT-V00.prt";
        public const string LOWCOMPLT = "LowCommonPlate";
        public const string LOWER_COMMON_PLATE = "LOWER_COMMON_PLATE";

        public CommonPlate(double length, double width, double thickness, string fileName = null) : 
            base(length, width, thickness, fileName)
        {
        }

        public override void CreateNewCommonPlate(string folderPath, ProjectInfo projectInfo, string drawingCode, string itemName)
        {
            Session session = Session.GetSession();
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = TEMPLATE_LOWCOMPLT_NAME;
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = NXDrawing.MODEL_TEMPLATE;
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = LOWCOMPLT;
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

            NXDrawing.UpdatePartProperties(
            projectInfo,
            drawingCode,
            itemName,
            GetLength().ToString("F1"),
            GetThickness().ToString("F2"),
            GetWidth().ToString("F1"),
            NXDrawing.HYPHEN,
            NXDrawing.S50C,
            PartProperties.SHOE);

            BasePart.SaveComponents saveComponentParts = BasePart.SaveComponents.True;
            BasePart.CloseAfterSave close = BasePart.CloseAfterSave.True;
            workPart.Save(saveComponentParts, close);
        }
    }
}
