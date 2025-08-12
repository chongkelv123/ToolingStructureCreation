using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using NXOpen;
using ToolingStructureCreation.Domain.Aggregates;
using ToolingStructureCreation.Domain.Entities;
using ToolingStructureCreation.Domain.Enums;


namespace ToolingStructureCreation.Infrastructure.NX
{
    /// <summary>
    /// Creates NX part files and components from domain entities
    /// </summary>
    public class NXModelFactory
    {
        private readonly NXSessionManager _sessionManager;
        private readonly string _templateBasePath;

        // Template constants
        private const string TEMPLATE_PLATE_NAME = "3DA_Template_PLATE-V00.prt";
        private const string TEMPLATE_SHOE_NAME = "3DA_Template_SHOE-V00.prt";
        private const string TEMPLATE_PARALLELBAR_NAME = "3DA_Template_PARALLELBAR-V00.prt";
        private const string TEMPLATE_LOWCOMPLT_NAME = "3DA_Template_LOWCOMPLT-V00.prt";
        private const string TEMPLATE_LOWCOMPLTLEFT_NAME = "3DA_Template_LOWCOMPLT_LEFT-V00.prt";
        private const string TEMPLATE_LOWCOMPLTRIGHT_NAME = "3DA_Template_LOWCOMPLT_RIGHT-V00.prt";
        private const string TEMPLATE_STP_NAME = "3DA_Template_STP-V00.prt";

        public NXModelFactory(NXSessionManager sessionManager, string templateBasePath)
        {
            _sessionManager = sessionManager ?? throw new ArgumentNullException(nameof(sessionManager));
            _templateBasePath = templateBasePath ?? throw new ArgumentNullException(nameof(templateBasePath));
        }

        /// <summary>
        /// Create NX part from plate entity following PlateLegacy.CreateNewPlate logic
        /// </summary>
        public Part CreatePlate(Plate plate, string outputDirectory, string projectCode)
        {
            _sessionManager.ValidateSession();

            var session = Session.GetSession();
            var fileName = GeneratePlateFileName(plate, projectCode);
            var fullPath = Path.Combine(outputDirectory, fileName);
            Debug.WriteLine(fullPath);

            // Create part from template following PlateLegacy pattern
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = TEMPLATE_PLATE_NAME;
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = "ModelTemplate";
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = "Plate";
            fileNew.SetCanCreateAltrep(false);
            fileNew.NewFileName = fullPath + ".prt";
            fileNew.MakeDisplayedPart = true;
            fileNew.DisplayPartOption = DisplayPartOption.AllowAdditional;

            var plateObject = fileNew.Commit();
            var workPart = session.Parts.Work;
            fileNew.Destroy();

            session.ApplicationSwitchImmediate("UG_APP_MODELING");

            // Update plate dimensions following existing pattern
            UpdatePlateExpressions(workPart, plate);

            // Apply color based on plate type
            ApplyPlateColor(workPart, plate.Type, fileName);

            // Update and save
            var undoMark = session.SetUndoMark(Session.MarkVisibility.Invisible, "Create New Plate");
            session.UpdateManager.DoUpdate(undoMark);

            // Update part properties (simplified - no ProjectInfo available in clean architecture)
            SaveAndClosePart(workPart);

            return workPart;
        }

        // <summary>
        /// Create NX part from shoe entity following ShoeLegacy.CreateNewShoe logic
        /// </summary>
        public Part CreateShoe(Shoe shoe, string outputDirectory, string projectCode)
        {
            _sessionManager.ValidateSession();

            var session = _sessionManager.NXSession;
            var fileName = GenerateShoeFileName(shoe, projectCode);
            var fullPath = Path.Combine(outputDirectory, fileName);

            // Create part from template following ShoeLegacy pattern
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = TEMPLATE_SHOE_NAME;
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = "UG_APP_MODELING";
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = "Shoe";
            fileNew.SetCanCreateAltrep(false);
            fileNew.NewFileName = fullPath + ".prt";
            fileNew.MakeDisplayedPart = true;
            fileNew.DisplayPartOption = DisplayPartOption.AllowAdditional;

            var shoeObject = fileNew.Commit();
            var workPart = session.Parts.Work;
            fileNew.Destroy();

            session.ApplicationSwitchImmediate("UG_APP_MODELING");

            // Update shoe dimensions following existing pattern
            UpdateShoeExpressions(workPart, shoe);

            // Update and save
            var undoMark = session.SetUndoMark(Session.MarkVisibility.Invisible, "Create New Shoe");
            session.UpdateManager.DoUpdate(undoMark);

            SaveAndClosePart(workPart);

            return workPart;
        }

        /// <summary>
        /// Create NX part from parallel bar entity following ParallelBarLegacy.CreateNewParallelBar logic
        /// </summary>
        public Part CreateParallelBar(ParallelBar parallelBar, string outputDirectory, string projectCode)
        {
            _sessionManager.ValidateSession();

            var session = _sessionManager.NXSession;
            var fileName = GenerateParallelBarFileName(parallelBar, projectCode);
            var fullPath = Path.Combine(outputDirectory, fileName);

            // Create part from template following ParallelBarLegacy pattern
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = TEMPLATE_PARALLELBAR_NAME;
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = "UG_APP_MODELING";
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = "ParallelBar";
            fileNew.SetCanCreateAltrep(false);
            fileNew.NewFileName = fullPath + ".prt";
            fileNew.MakeDisplayedPart = true;
            fileNew.DisplayPartOption = DisplayPartOption.AllowAdditional;

            var parallelBarObject = fileNew.Commit();
            var workPart = session.Parts.Work;
            fileNew.Destroy();

            session.ApplicationSwitchImmediate("UG_APP_MODELING");

            // Update parallel bar dimensions following existing pattern
            UpdateParallelBarExpressions(workPart, parallelBar);

            // Update and save
            var undoMark = session.SetUndoMark(Session.MarkVisibility.Invisible, "Create Parallel Bar");
            session.UpdateManager.DoUpdate(undoMark);

            SaveAndClosePart(workPart);

            return workPart;
        }

        /// <summary>
        /// Create NX part from common plate entity following CommonPlateLegacy logic
        /// </summary>
        public Part CreateCommonPlate(CommonPlate commonPlate, string outputDirectory, string projectCode)
        {
            _sessionManager.ValidateSession();

            var session = _sessionManager.NXSession;
            var fileName = GenerateCommonPlateFileName(commonPlate, projectCode);
            var fullPath = Path.Combine(outputDirectory, fileName);

            // Select template based on common plate type
            string templateName = GetCommonPlateTemplate(commonPlate.Type);
            string presentationName = GetCommonPlatePresentationName(commonPlate.Type);

            // Create part from template following CommonPlateLegacy pattern
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = templateName;
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = "UG_APP_MODELING";
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = presentationName;
            fileNew.SetCanCreateAltrep(false);
            fileNew.NewFileName = fullPath + ".prt";
            fileNew.MakeDisplayedPart = true;
            fileNew.DisplayPartOption = DisplayPartOption.AllowAdditional;

            var commonPlateObject = fileNew.Commit();
            var workPart = session.Parts.Work;
            fileNew.Destroy();

            session.ApplicationSwitchImmediate("UG_APP_MODELING");

            // Update common plate dimensions following existing pattern
            UpdateCommonPlateExpressions(workPart, commonPlate);

            // Update and save
            var undoMark = session.SetUndoMark(Session.MarkVisibility.Invisible, "Create Low Common Plate");
            session.UpdateManager.DoUpdate(undoMark);

            SaveAndClosePart(workPart);

            return workPart;
        }

        /// <summary>
        /// Create station assembly from StationAggregate following StationAssemblyFactory logic
        /// </summary>
        public Part CreateStationAssembly(StationAggregate station, string outputDirectory, string projectCode)
        {
            _sessionManager.ValidateSession();

            var session = _sessionManager.NXSession;
            var assemblyName = $"STATION_{station.StationNumber:D2}";
            var fileName = $"{projectCode}_{assemblyName}";
            var fullPath = Path.Combine(outputDirectory, fileName);

            // Create assembly part following StationAssemblyFactory pattern
            FileNew fileNew = session.Parts.FileNew();
            fileNew.TemplateFileName = TEMPLATE_STP_NAME;
            fileNew.UseBlankTemplate = false;
            fileNew.ApplicationName = "UG_APP_ASM";
            fileNew.Units = Part.Units.Millimeters;
            fileNew.TemplatePresentationName = "Assembly";
            fileNew.SetCanCreateAltrep(false);
            fileNew.NewFileName = fullPath + ".prt";
            fileNew.MakeDisplayedPart = true;
            fileNew.DisplayPartOption = DisplayPartOption.AllowAdditional;

            var assemblyObject = fileNew.Commit();
            var workPart = session.Parts.Work;
            fileNew.Destroy();

            session.ApplicationSwitchImmediate("UG_APP_MODELING");

            // Orient to isometric view
            workPart.ModelingViews.WorkView.Orient(NXOpen.View.Canned.Isometric, NXOpen.View.ScaleAdjustment.Fit);

            SaveAndClosePart(workPart);

            return workPart;
        }

        private void UpdatePlateExpressions(Part workPart, Plate plate)
        {
            var expressionWidth = workPart.Expressions.FindObject("Width");
            var expressionLength = workPart.Expressions.FindObject("Length");
            var expressionThk = workPart.Expressions.FindObject("Thk");

            if (expressionWidth != null)
                workPart.Expressions.EditExpression(expressionWidth, plate.Dimensions.Width.ToString());
            if (expressionLength != null)
                workPart.Expressions.EditExpression(expressionLength, plate.Dimensions.Length.ToString());
            if (expressionThk != null)
                workPart.Expressions.EditExpression(expressionThk, plate.Dimensions.Thickness.ToString());
        }

        private void UpdateShoeExpressions(Part workPart, Shoe shoe)
        {
            // Follow ShoeLegacy pattern for expression names
            var expressionWidth = workPart.Expressions.FindObject("ShoeWidth") as Expression;
            var expressionLength = workPart.Expressions.FindObject("ShoeLength") as Expression;
            var expressionThk = workPart.Expressions.FindObject("ShoeThk") as Expression;

            if (expressionWidth != null)
                workPart.Expressions.EditExpression(expressionWidth, shoe.Dimensions.Width.ToString());
            if (expressionLength != null)
                workPart.Expressions.EditExpression(expressionLength, shoe.Dimensions.Length.ToString());
            if (expressionThk != null)
                workPart.Expressions.EditExpression(expressionThk, shoe.Dimensions.Thickness.ToString());
        }

        private void UpdateParallelBarExpressions(Part workPart, ParallelBar parallelBar)
        {
            // Follow ParallelBarLegacy pattern for expression names
            var expressionWidth = workPart.Expressions.FindObject("Width") as Expression;
            var expressionLength = workPart.Expressions.FindObject("Length") as Expression;
            var expressionThk = workPart.Expressions.FindObject("Thk") as Expression;

            if (expressionWidth != null)
                workPart.Expressions.EditExpression(expressionWidth, parallelBar.Dimensions.Width.ToString());
            if (expressionLength != null)
                workPart.Expressions.EditExpression(expressionLength, parallelBar.Dimensions.Length.ToString());
            if (expressionThk != null)
                workPart.Expressions.EditExpression(expressionThk, parallelBar.Dimensions.Thickness.ToString());
        }

        private void UpdateCommonPlateExpressions(Part workPart, CommonPlate commonPlate)
        {
            // Follow CommonPlateLegacy pattern for expression names
            var expressionWidth = workPart.Expressions.FindObject("Width") as Expression;
            var expressionLength = workPart.Expressions.FindObject("Length") as Expression;
            var expressionThk = workPart.Expressions.FindObject("Thk") as Expression;

            if (expressionWidth != null)
                workPart.Expressions.EditExpression(expressionWidth, commonPlate.Dimensions.Width.ToString());
            if (expressionLength != null)
                workPart.Expressions.EditExpression(expressionLength, commonPlate.Dimensions.Length.ToString());
            if (expressionThk != null)
                workPart.Expressions.EditExpression(expressionThk, commonPlate.Dimensions.Thickness.ToString());
        }

        private void ApplyPlateColor(Part workPart, PlateType plateType, string fileName)
        {
            // Follow PlateLegacy color assignment logic
            foreach (Body body in workPart.Bodies)
            {
                if (fileName.Contains("UPPER_PAD"))
                    body.Color = 186; // Light Blue
                else if (fileName.Contains("PUNCH_HOLDER"))
                    body.Color = 78;  // Orange
                else if (fileName.Contains("BOTTOMING_PLATE"))
                    body.Color = 211; // Yellow
                else if (fileName.Contains("STRIPPER_PLATE"))
                    body.Color = 36;  // Green
                else if (fileName.Contains("DIE_PLATE"))
                    body.Color = 125; // Red
                else if (fileName.Contains("LOWER_PAD"))
                    body.Color = 198; // Purple
                else
                    body.Color = 1;   // Default
            }
        }

        private void SaveAndClosePart(Part workPart)
        {
            // Follow existing save pattern
            var saveComponents = BasePart.SaveComponents.True;
            var closeAfterSave = BasePart.CloseAfterSave.True;
            workPart.Save(saveComponents, closeAfterSave);
        }

        private string GetCommonPlateTemplate(CommonPlateType plateType)
        {
            switch (plateType)
            {
                case CommonPlateType.Single:
                    return TEMPLATE_LOWCOMPLT_NAME;
                case CommonPlateType.DoubleLeft:
                    return TEMPLATE_LOWCOMPLTLEFT_NAME;
                case CommonPlateType.DoubleRight:
                    return TEMPLATE_LOWCOMPLTRIGHT_NAME;
                default:
                    return TEMPLATE_LOWCOMPLT_NAME;
            }
        }

        private string GetCommonPlatePresentationName(CommonPlateType plateType)
        {
            switch (plateType)
            {
                case CommonPlateType.Single:
                    return "LowCommonPlate";
                case CommonPlateType.DoubleLeft:
                    return "LowCommonPlateLeft";
                case CommonPlateType.DoubleRight:
                    return "LowCommonPlateRight";
                default:
                    return "LowCommonPlate";
            }
        }

        private string GeneratePlateFileName(Plate plate, string projectCode)
        {
            return $"{projectCode}_{plate.Name}";
        }

        private string GenerateShoeFileName(Shoe shoe, string projectCode)
        {
            return $"{projectCode}_{shoe.Name}";
        }

        private string GenerateParallelBarFileName(ParallelBar parallelBar, string projectCode)
        {
            return $"{projectCode}_{parallelBar.Name}";
        }

        private string GenerateCommonPlateFileName(CommonPlate commonPlate, string projectCode)
        {
            return $"{projectCode}_{commonPlate.Name}";
        }
    }
}
