using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCreateNewPlate.Model
{
    public class Plate
    {
        private string plateName;
        private double plateLength;
        private double plateWidth;
        private double plateThickness;
        NXDrawing drawing;

        public const string LOWER_PAD = "LOWER_PAD";
        public const string DIE_PLATE = "DIE_PLATE";
        public const string MAT_THK = "mat_thk";
        public const string STRIPPER_PLATE = "STRIPPER_PLATE";
        public const string BOTTOMING_PLATE = "BOTTOMING_PLATE";
        public const string PUNCH_HOLDER = "PUNCH_HOLDER";
        public const string UPPER_PAD = "UPPER_PAD";

        public Plate(string name, double length, double width, double thickness, NXDrawing drawing)
        {
            this.plateName = name;
            this.plateLength = length;
            this.plateWidth = width;
            this.plateThickness = thickness;
            this.drawing = drawing;
        }

        public string GetPlateName()
        {
            return plateName;
        }

        public double GetPlateLength()
        {
            return plateLength;
        }

        public double GetPlateWidth()
        {
            return plateWidth;
        }

        public double GetPlateThickness()
        {
            return plateThickness;
        }

        public void CreateNewPlate(string folderPath, string stationNumber)
        {
            Session session = Session.GetSession();
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = "3DA_Template_PLATE-V00.prt";
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = "ModelTemplate";
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = "Plate";
            fileNew.SetCanCreateAltrep(false);
            fileNew.NewFileName = $"{folderPath}{stationNumber}-{plateName}.prt";
            fileNew.MakeDisplayedPart = true;
            fileNew.DisplayPartOption = NXOpen.DisplayPartOption.AllowAdditional;
            NXObject plateObject;
            plateObject = fileNew.Commit();

            Part workPart = session.Parts.Work;
            Part displayPart = session.Parts.Display;

            fileNew.Destroy();

            session.ApplicationSwitchImmediate("UG_APP_MODELING");

            NXOpen.Expression expressionPlateWidth = ((NXOpen.Expression)workPart.Expressions.FindObject("PlateWidth"));
            NXOpen.Expression expressionPlateLength = ((NXOpen.Expression)workPart.Expressions.FindObject("PlateLength"));
            NXOpen.Expression expressionPlateThk = ((NXOpen.Expression)workPart.Expressions.FindObject("PlateThk"));
            if (expressionPlateWidth == null)
            {
                drawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'PlateWidth' not found.");
                return;
            }
            else if (expressionPlateLength == null)
            {
                drawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'PlateLength' not found.");
                return;
            }
            else if (expressionPlateThk == null)
            {
                drawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'PlateThk' not found.");
                return;
            }
            workPart.Expressions.EditExpression(expressionPlateWidth, GetPlateWidth().ToString());
            workPart.Expressions.EditExpression(expressionPlateLength, GetPlateLength().ToString());
            workPart.Expressions.EditExpression(expressionPlateThk, GetPlateThickness().ToString());

            NXOpen.Session.UndoMarkId undoMark = session.SetUndoMark(Session.MarkVisibility.Invisible, "Create New Plate");
            session.UpdateManager.DoUpdate(undoMark);

            /*
             * Change Color
             */
            NXOpen.BodyCollection bodyCollection = workPart.Bodies;
            foreach (NXOpen.Body body in bodyCollection)
            {
                if (plateName.Equals(UPPER_PAD))
                {
                    body.Color = (int)PlateColor.UPPERPAD;
                }
                else if (plateName.Equals(PUNCH_HOLDER))
                {
                    body.Color = (int)PlateColor.PUNCHHOLDER;
                }
                else if (plateName.Equals(BOTTOMING_PLATE))
                {
                    body.Color = (int)PlateColor.BOTTOMINGPLATE;
                }
                else if (plateName.Equals(STRIPPER_PLATE))
                {
                    body.Color = (int)PlateColor.STRIPPERPLATE;
                }
                else if (plateName.Equals(DIE_PLATE))
                {
                    body.Color = (int)PlateColor.DIEPLATE;
                }
                else if (plateName.Equals(LOWER_PAD))
                {
                    body.Color = (int)PlateColor.LOWERPAD;
                }
                else
                {
                    body.Color = (int)PlateColor.COMMONPLATE;
                }
            }

            BasePart.SaveComponents saveComponentParts = BasePart.SaveComponents.True;
            BasePart.CloseAfterSave close = BasePart.CloseAfterSave.True;
            workPart.Save(saveComponentParts, close);
        }
    }
}
