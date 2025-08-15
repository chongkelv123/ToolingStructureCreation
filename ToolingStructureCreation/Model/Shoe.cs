using NXOpen;
using NXOpen.Assemblies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NXOpen.Display.DecalBuilder;
using static NXOpen.Motion.HydrodynamicBearingBuilder;

namespace ToolingStructureCreation.Model
{
    public class Shoe
    {
        private string fileName;
        private double length;
        private double width;
        private double thickness;

        public const string UPPER_SHOE = "UPPER_SHOE";
        public const string LOWER_SHOE = "LOWER_SHOE";
        public const string TEMPLATE_SHOE_NAME = "3DA_Template_SHOE-V00.prt";
        public const string SHOE_PRESENTATION_NAME = "Shoe";

        public Shoe(string name, double length, double width, double thickness)
        {
            this.fileName = name;
            this.length = length;
            this.width = width;
            this.thickness = thickness;
        }

        public string GetShoeName()
        {
            return fileName;
        }

        public double GetShoeLength()
        {
            return length;
        }

        public double GetShoeWidth()
        {
            return width;
        }

        public double GetShoeHeight()
        {
            return thickness;
        }

        public void CreateNewShoe(string folderPath, ProjectInfo projectInfo, string drawingCode, string itemName)
        {
            Session session = Session.GetSession();
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = TEMPLATE_SHOE_NAME;
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = NXDrawing.MODEL_TEMPLATE;
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = SHOE_PRESENTATION_NAME;
            fileNew.SetCanCreateAltrep(false);
            fileNew.NewFileName = $"{folderPath}{fileName}{NXDrawing.EXTENSION}";
            fileNew.MakeDisplayedPart = true;
            fileNew.DisplayPartOption = NXOpen.DisplayPartOption.AllowAdditional;

            try
            {
                NXObject shoeObject = fileNew.Commit();

                Part workPart = session.Parts.Work;
                Part displayPart = session.Parts.Display;

                fileNew.Destroy();

                session.ApplicationSwitchImmediate(NXDrawing.UG_APP_MODELING);

                NXOpen.Expression expressionShoeWidth = ((NXOpen.Expression)workPart.Expressions.FindObject("Width"));
                NXOpen.Expression expressionShoeLength = ((NXOpen.Expression)workPart.Expressions.FindObject("Length"));
                NXOpen.Expression expressionShoeThk = ((NXOpen.Expression)workPart.Expressions.FindObject("Thk"));
                if (expressionShoeWidth == null)
                {
                    NXDrawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'ShoeWidth' not found.");
                    return;
                }
                else if (expressionShoeLength == null)
                {
                    NXDrawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'ShoeLength' not found.");
                    return;
                }
                else if (expressionShoeThk == null)
                {
                    NXDrawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'ShoeThk' not found.");
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
                    if (fileName.Contains(UPPER_SHOE))
                    {
                        body.Color = (int)PlateColor.UPPERSHOE;
                    }
                    else if (fileName.Contains(LOWER_SHOE))
                    {
                        body.Color = (int)PlateColor.LOWERSHOE;
                    }
                    else
                    {
                        body.Color = (int)PlateColor.COMMONPLATE;
                    }
                }

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
                fileNew.Destroy(); // Clean up

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
                fileNew.Destroy(); // Clean up

                // Handle other NX-specific errors
                string message = $"NX Error creating plate '{fileName}':\n{nxEx.Message}";
                string title = "NX Operation Error";
                NXDrawing.ShowMessageBox(title, NXOpen.NXMessageBox.DialogType.Error, message);

                throw new InvalidOperationException($"Failed to create plate '{fileName}'", nxEx);
            }
            catch (Exception ex)
            {
                fileNew.Destroy(); // Clean up

                // Handle unexpected errors
                string message = $"Unexpected error creating plate '{fileName}':\n{ex.Message}";
                string title = "Unexpected Error";
                NXDrawing.ShowMessageBox(title, NXOpen.NXMessageBox.DialogType.Error, message);

                throw;
            }

        }
        static public void Insert(Part workAssy, string compName, Point3d basePoint, string folderPath)
        {
            ComponentAssembly compAssy = workAssy.ComponentAssembly;
            PartLoadStatus status = null;
            int layer = 100;
            string referenceSetName = NXDrawing.MODEL;
            Matrix3x3 orientation = new Matrix3x3();
            orientation.Xx = 1.0;
            orientation.Xy = 0.0;
            orientation.Xz = 0.0;
            orientation.Yx = 0.0;
            orientation.Yy = 1.0;
            orientation.Yz = 0.0;
            orientation.Zx = 0.0;
            orientation.Zy = 0.0;
            orientation.Zz = 1.0;

            string partToAdd = $"{folderPath}{compName}{NXDrawing.EXTENSION}";

            if (compName.Equals(Shoe.LOWER_SHOE))
            {
                layer = 200;
            }

            NXOpen.Assemblies.Component component = compAssy.AddComponent(partToAdd, referenceSetName, compName, basePoint, orientation, layer, out status);

            NXOpen.Positioning.ComponentPositioner positioner = workAssy.ComponentAssembly.Positioner;
            NXOpen.Positioning.Network network = positioner.EstablishNetwork();
            NXOpen.Positioning.ComponentNetwork componentNetwork = ((NXOpen.Positioning.ComponentNetwork)network);
            NXOpen.Positioning.Constraint constraint = positioner.CreateConstraint(true);
            NXOpen.Positioning.ComponentConstraint componentConstraint = ((NXOpen.Positioning.ComponentConstraint)constraint);
            componentConstraint.ConstraintType = NXOpen.Positioning.Constraint.Type.Fix;
            NXOpen.Positioning.ConstraintReference constraintReference = componentConstraint.CreateConstraintReference(component, component, false, false, false);
            componentNetwork.Solve();

            NXOpen.Layer.StateInfo[] stateArray = new NXOpen.Layer.StateInfo[]
            {
                new NXOpen.Layer.StateInfo(layer, NXOpen.Layer.State.Selectable)
            };
            workAssy.Layers.ChangeStates(stateArray);
        }
    }
}
