using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using ToolingStructureCreation.Model;

namespace ToolingStructureCreation.Services
{
    /// <summary>
    /// Factory methods and color assignment logic for different component types
    /// </summary>
    public class ComponentCreationConfigs
    {
        /// <summary>
        /// Creates configuration for Plate components
        /// </summary>
        public static ComponentCreationConfig CreatePlateConfig(string folderPath, string fileName,
            double length, double width, double thickness, ProjectInfo projectInfo,
            string drawingCode, string itemName)
        {
            return new ComponentCreationConfig
            {
                TemplateFileName = Plate.TEMPLATE_PLATE_NAME,
                PresentationName = Plate.PLATE_PRESENTATION_NAME,
                UndoDescription = "Create New Plate",
                FolderPath = folderPath,
                FileName = fileName,
                Width = width,
                Length = length,
                Thickness = thickness,
                ProjectInfo = projectInfo,
                DrawingCode = drawingCode,
                ItemName = itemName,
                ColorAssignmentAction = AssignPlateColors,
                PartPropertiesType = PartProperties.PLATE,
                Material = NXDrawing.GOA,
                HardnessOrGrade = NXDrawing.FIFTYTWO_FIFTYFOUR
            };
        }

        /// <summary>
        /// Creates configuration for Shoe components
        /// </summary>
        public static ComponentCreationConfig CreateShoeConfig(string folderPath, string fileName,
            double length, double width, double thickness, ProjectInfo projectInfo,
            string drawingCode, string itemName)
        {
            return new ComponentCreationConfig
            {
                TemplateFileName = Shoe.TEMPLATE_SHOE_NAME,
                PresentationName = Shoe.SHOE_PRESENTATION_NAME,
                UndoDescription = "Create New Shoe",
                FolderPath = folderPath,
                FileName = fileName,
                Width = width,
                Length = length,
                Thickness = thickness,
                ProjectInfo = projectInfo,
                DrawingCode = drawingCode,
                ItemName = itemName,
                ColorAssignmentAction = AssignShoeColors,
                PartPropertiesType = PartProperties.SHOE,
                Material = NXDrawing.S50C,
                HardnessOrGrade = NXDrawing.HYPHEN
            };
        }

        /// <summary>
        /// Creates configuration for CommonPlate components (all variants)
        /// </summary>
        public static ComponentCreationConfig CreateCommonPlateConfig(string templateFileName,
            string presentationName, string folderPath, string fileName, double length,
            double width, double thickness, ProjectInfo projectInfo, string drawingCode, string itemName)
        {
            return new ComponentCreationConfig
            {
                TemplateFileName = templateFileName,
                PresentationName = presentationName,
                UndoDescription = "Create Low Common Plate",
                FolderPath = folderPath,
                FileName = fileName,
                Width = width,
                Length = length,
                Thickness = thickness,
                ProjectInfo = projectInfo,
                DrawingCode = drawingCode,
                ItemName = itemName,
                ColorAssignmentAction = AssignCommonPlateColors,
                PartPropertiesType = PartProperties.SHOE,
                Material = NXDrawing.S50C,
                HardnessOrGrade = NXDrawing.HYPHEN
            };
        }

        /// <summary>
        /// Creates configuration for ParallelBar components
        /// </summary>
        public static ComponentCreationConfig CreateParallelBarConfig(string folderPath, string fileName,
            double length, double width, double thickness, ProjectInfo projectInfo,
            string drawingCode, string itemName)
        {
            return new ComponentCreationConfig
            {
                TemplateFileName = ParallelBar.TEMPLATE_PARALLELBAR_NAME, // Assuming this constant exists
                PresentationName = ParallelBar.PBAR_PRESENTATION_NAME,
                UndoDescription = "Create New Parallel Bar",
                FolderPath = folderPath,
                FileName = fileName,
                Width = width,
                Length = length,
                Thickness = thickness,
                ProjectInfo = projectInfo,
                DrawingCode = drawingCode,
                ItemName = itemName,
                ColorAssignmentAction = AssignParallelBarColors,
                PartPropertiesType = PartProperties.SHOE,
                Material = NXDrawing.S50C,
                HardnessOrGrade = NXDrawing.HYPHEN
            };
        }

        #region Color Assignment Methods

        /// <summary>
        /// Assigns colors to plate bodies based on plate type (extracted from original Plate.CreateNewPlate)
        /// </summary>
        private static void AssignPlateColors(Part workPart, string fileName)
        {
            foreach (Body body in workPart.Bodies)
            {
                if (fileName.Contains(NXDrawing.UPPER_PAD))
                    body.Color = (int)PlateColor.UPPERPAD;
                else if (fileName.Contains(NXDrawing.PUNCH_HOLDER))
                    body.Color = (int)PlateColor.PUNCHHOLDER;
                else if (fileName.Contains(NXDrawing.BOTTOMING_PLATE))
                    body.Color = (int)PlateColor.BOTTOMINGPLATE;
                else if (fileName.Contains(NXDrawing.STRIPPER_PLATE))
                    body.Color = (int)PlateColor.STRIPPERPLATE;
                else if (fileName.Contains(NXDrawing.DIE_PLATE))
                    body.Color = (int)PlateColor.DIEPLATE;
                else if (fileName.Contains(NXDrawing.LOWER_PAD))
                    body.Color = (int)PlateColor.LOWERPAD;
                else
                    body.Color = (int)PlateColor.COMMONPLATE;
            }
        }

        /// <summary>
        /// Assigns colors to shoe bodies based on shoe type (extracted from original Shoe.CreateNewShoe)
        /// </summary>
        private static void AssignShoeColors(Part workPart, string fileName)
        {
            foreach (Body body in workPart.Bodies)
            {
                if (fileName.Contains(Shoe.UPPER_SHOE))
                    body.Color = (int)PlateColor.UPPERSHOE;
                else if (fileName.Contains(Shoe.LOWER_SHOE))
                    body.Color = (int)PlateColor.LOWERSHOE;
                else
                    body.Color = (int)PlateColor.COMMONPLATE;
            }
        }

        /// <summary>
        /// Assigns colors to common plate bodies (extracted from original CommonPlate methods)
        /// </summary>
        private static void AssignCommonPlateColors(Part workPart, string fileName)
        {
            foreach (Body body in workPart.Bodies)
            {
                body.Color = (int)PlateColor.COMMONPLATE;
            }
        }

        /// <summary>
        /// Assigns colors to parallel bar bodies
        /// </summary>
        private static void AssignParallelBarColors(Part workPart, string fileName)
        {
            foreach (Body body in workPart.Bodies)
            {
                body.Color = (int)PlateColor.PARALLELBAR;
            }
        }

        #endregion
    }
}
