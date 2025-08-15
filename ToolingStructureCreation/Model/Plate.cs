using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Layout2d;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Model
{
    public class Plate
    {
        private string fileName;
        private double length;
        private double width;
        private double thickness;        

        public const string TEMPLATE_PLATE_NAME = "3DA_Template_PLATE-V00.prt";                
        public const string PLATE_PRESENTATION_NAME = "Plate";
        public const string HRC = "52~54";
        public const string MATERIAL = "GOA";        

        public Plate(string fileName, double length, double width, double thickness)
        {
            this.fileName = fileName;
            this.length = length;
            this.width = width;
            this.thickness = thickness;            
        }

        public string GetPlateName()
        {
            return fileName;
        }

        public double GetPlateLength()
        {
            return length;
        }

        public double GetPlateWidth()
        {
            return width;
        }

        public double GetPlateThickness()
        {
            return thickness;
        }

        public void CreateNewPlate(string folderPath, ProjectInfo projectInfo, string drawingCode, string itemName)
        {
            Session session = Session.GetSession();
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = TEMPLATE_PLATE_NAME;
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = NXDrawing.MODEL_TEMPLATE;
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = PLATE_PRESENTATION_NAME;
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

                NXOpen.Expression expressionPlateWidth = ((NXOpen.Expression)workPart.Expressions.FindObject("Width"));
                NXOpen.Expression expressionPlateLength = ((NXOpen.Expression)workPart.Expressions.FindObject("Length"));
                NXOpen.Expression expressionPlateThk = ((NXOpen.Expression)workPart.Expressions.FindObject("Thk"));
                if (expressionPlateWidth == null)
                {
                    NXDrawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'PlateWidth' not found.");
                    return;
                }
                else if (expressionPlateLength == null)
                {
                    NXDrawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'PlateLength' not found.");
                    return;
                }
                else if (expressionPlateThk == null)
                {
                    NXDrawing.ShowMessageBox("Error", NXMessageBox.DialogType.Error, "Expression 'PlateThk' not found.");
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
                    if (fileName.Contains(NXDrawing.UPPER_PAD))
                    {
                        body.Color = (int)PlateColor.UPPERPAD;
                    }
                    else if (fileName.Contains(NXDrawing.PUNCH_HOLDER))
                    {
                        body.Color = (int)PlateColor.PUNCHHOLDER;
                    }
                    else if (fileName.Contains(NXDrawing.BOTTOMING_PLATE))
                    {
                        body.Color = (int)PlateColor.BOTTOMINGPLATE;
                    }
                    else if (fileName.Contains(NXDrawing.STRIPPER_PLATE))
                    {
                        body.Color = (int)PlateColor.STRIPPERPLATE;
                    }
                    else if (fileName.Contains(NXDrawing.DIE_PLATE))
                    {
                        body.Color = (int)PlateColor.DIEPLATE;
                    }
                    else if (fileName.Contains(NXDrawing.LOWER_PAD))
                    {
                        body.Color = (int)PlateColor.LOWERPAD;
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
                    NXDrawing.FIFTYTWO_FIFTYFOUR,
                    NXDrawing.GOA,
                    PartProperties.PLATE);

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
        
        static public void InsertPlate(Part workAssy, string compName, double cumThk, string folderPath)
        {
            ComponentAssembly compAssy = workAssy.ComponentAssembly;
            PartLoadStatus status = null;
            int layer = 100;
            string referenceSetName = NXDrawing.MODEL;
            Point3d basePoint = new Point3d(0.0, 0.0, cumThk);
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

            if (compName.Contains(NXDrawing.DIE_PLATE) || compName.Contains(NXDrawing.LOWER_PAD))
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
