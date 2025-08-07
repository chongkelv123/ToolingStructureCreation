using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToolingStructureCreation.Domain.Aggregates;
using ToolingStructureCreation.Domain.Services;
using ToolingStructureCreation.Domain.ValueObjects;
using ToolingStructureCreation.Integration;
using ToolingStructureCreation.Model;
using ToolingStructureCreation.Services;
using ToolingStructureCreation.View;

namespace ToolingStructureCreation.Controller
{
    public class Control
    {
        NXDrawing drawing;
        formToolStructure myForm;
        StripLayout stripLayout;

        public NXDrawing GetDrawing => drawing;
        public formToolStructure GetForm => myForm;
        public StripLayout GetStripLayout => stripLayout;

        //Dictionary<string, double> plateThicknesses = new Dictionary<string, double>();

        // Legacy support - keep existing constants
        private const string TEMPLATE_BASE_PATH = @"D:\NXCUSTOM\templates";
        private const string DATA_DIRECTORY = @"D:\NXCUSTOM\ToolingData";

        public Control()
        {
            drawing = new NXDrawing(this);
            if (!drawing.IsDrawingOpen())
            {
                return;
            }
            stripLayout = drawing.GetStripLayout();
            myForm = new formToolStructure(this);
            myForm.Show();
        }

        /// <summary>
        /// Legacy method - uses StationAssemblyFactory
        /// </summary>
        public void Start(StationAssemblyFactory stnAsmFactory)
        {
            string itemName = "MainToolAssembly";
            AsmCodeGeneratorServicecs asmCodeGenerator = new AsmCodeGeneratorServicecs(this, myForm.GetProjectInfo(), itemName);
            stnAsmFactory.CreateStnAsmFactory();
            stnAsmFactory.CreateToolAsmFactory(myForm.GetProjectInfo(), asmCodeGenerator.AskDrawingCode(), itemName);
        }

        /// <summary>
        /// New method - uses clean architecture with integration controller
        /// </summary>
        public async Task<bool> StartWithCleanArchitectureAsync()
        {
            try
            {
                // Create integration controller
                var integrationController = ToolingIntegrationController.Create(TEMPLATE_BASE_PATH, DATA_DIRECTORY);

                // Convert existing sketch lists to domain objects
                var stationSketches = ConvertToSketchGeometry(myForm.StationSketchLists, "STATION");
                var shoeSketches = ConvertToSketchGeometry(myForm.ShoeSketchLists, "SHOE");
                var commonPlateSketches = ConvertToSketchGeometry(myForm.ComPlateSketchList, "COMMON_PLATE");

                // Execute with clean architecture
                var result = await integrationController.CreateToolingStructureAsync(
                    myForm, stationSketches, shoeSketches, commonPlateSketches);

                return result.Success;
            }
            catch (Exception)
            {
                return false;                
            }
        }

        /// <summary>
        /// Convert Model.Sketch to domain SketchGeometry
        /// </summary>
        private List<SketchGeometry> ConvertToSketchGeometry(List<Model.Sketch> modelSketches, string type)
        {
            if (modelSketches == null)
                return new List<SketchGeometry>();

            return modelSketches.Select(sketch => new SketchGeometry(
                new Dimensions(sketch.Length, sketch.Width, 1.0), // Default thickness
                new Position3D(sketch.StartLocation.X, sketch.StartLocation.Y, sketch.StartLocation.Z),
                new Position3D(sketch.MidPoint.X, sketch.MidPoint.Y, sketch.MidPoint.Z)
                )).ToList();
        }

        /// <summary>
        /// Domain layer approach (for reference) 
        /// </summary>
        public void StartWithDomainLayer()
        {
            // Get form values (no hardcoding)
            var toolingParams = ToolingParameters.FromForm(
                myForm,
                MachineSpecification.GetByName(myForm.GetMachineName),
                new DrawingCode(myForm.GetCodePrefix),
                myForm.GetModel,
                myForm.GetDesginer
                );

            // Create domain aggregate using form values
            var toolingStructure = new ToolingStructureAggregate(toolingParams);

            // Generate complete structure using user inputs
            /*toolingStructure.GenerateCompleteToolingStructure(
                stationSketches,    // From sketch selection
                shoeSketches,       // From sketch selection  
                commonPlateSketches // From sketch selection (optional)
            );*/

        }

    }
}
