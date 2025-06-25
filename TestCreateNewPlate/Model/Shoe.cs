using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCreateNewPlate.Model
{
    public class Shoe
    {
        private string shoeName;
        private double shoeLength;
        private double shoeWidth;
        private double shoeHeight;
        private NXDrawing drawing;

        public const string UPPER_SHOE = "UPPER_SHOE";
        public const string LOWER_SHOE = "LOWER_SHOE";

        public Shoe(string name, double length, double width, double height, NXDrawing drawing)
        {
            this.shoeName = name;
            this.shoeLength = length;
            this.shoeWidth = width;
            this.shoeHeight = height;
            this.drawing = drawing;
        }

        public string GetShoeName()
        {
            return shoeName;
        }

        public double GetShoeLength()
        {
            return shoeLength;
        }

        public double GetShoeWidth()
        {
            return shoeWidth;
        }

        public double GetShoeHeight()
        {
            return shoeHeight;
        }

        public void CreateNewShoe(string folderPath)
        {
            Session session = Session.GetSession();
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = "3DA_Template_SHOE-V00.prt";
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = "ModelTemplate";
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = "Shoe";
            fileNew.SetCanCreateAltrep(false);
            fileNew.NewFileName = $"{folderPath}{shoeName}.prt";
            fileNew.MakeDisplayedPart = true;
            fileNew.DisplayPartOption = NXOpen.DisplayPartOption.AllowAdditional;
            NXObject shoeObject;
            shoeObject = fileNew.Commit();

            Part workPart = session.Parts.Work;
            Part displayPart = session.Parts.Display;

            fileNew.Destroy();

            session.ApplicationSwitchImmediate("UG_APP_MODELING");

            NXOpen.Expression expressionShoeWidth = ((NXOpen.Expression)workPart.Expressions.FindObject("ShoeWidth"));
            NXOpen.Expression expressionShoeLength = ((NXOpen.Expression)workPart.Expressions.FindObject("ShoeLength"));
            NXOpen.Expression expressionShoeThk = ((NXOpen.Expression)workPart.Expressions.FindObject("ShoeThk"));
            if (expressionShoeWidth == null)
            {
                drawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'ShoeWidth' not found.");
                return;
            }
            else if (expressionShoeLength == null)
            {
                drawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'ShoeLength' not found.");
                return;
            }
            else if (expressionShoeThk == null)
            {
                drawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'ShoeThk' not found.");
                return;
            }
            workPart.Expressions.EditExpression(expressionShoeWidth, GetShoeWidth().ToString());
            workPart.Expressions.EditExpression(expressionShoeLength, GetShoeLength().ToString());
            workPart.Expressions.EditExpression(expressionShoeThk, GetShoeHeight().ToString());

            NXOpen.Session.UndoMarkId undoMark = session.SetUndoMark(Session.MarkVisibility.Invisible, "Create New Shoe");
            session.UpdateManager.DoUpdate(undoMark);

            /*
             * Change Color
             */
            NXOpen.BodyCollection bodyCollection = workPart.Bodies;
            foreach (NXOpen.Body body in bodyCollection)
            {
                if (shoeName.Equals(UPPER_SHOE))
                {
                    body.Color = (int)PlateColor.UPPERSHOE;
                }
                else if (shoeName.Equals(LOWER_SHOE))
                {
                    body.Color = (int)PlateColor.LOWERSHOE;
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
