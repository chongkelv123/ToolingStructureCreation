using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToolingStructureCreation.Model;
using ToolingStructureCreation.Services;
using ToolingStructureCreation.View;
using System.Reflection;

namespace ToolingStructureCreation.Controller
{
    public class Control
    {
        NXDrawing drawing;
        formToolStructure myForm;
        StripLayout stripLayout;

        public NXDrawing GetDrawing => drawing;
        public formToolStructure GetForm => myForm;

        Dictionary<string, double> plateThicknesses = new Dictionary<string, double>();


        public Control()
        {
            drawing = new NXDrawing(this);
            if (!drawing.IsDrawingOpen())
            {
                return;
            }
            // Start usage tracking session
            string moduleVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
            UsageTrackingService.Instance.StartSession("Unknown", moduleVersion); // Will be updated from from
            UsageTrackingService.Instance.LogAction("CONTROL_INITIALIZED", "Main control initialized");

            stripLayout = drawing.GetStripLayout();
            myForm = new formToolStructure(this);
            myForm.Show();
        }

        public void Start(StationAssemblyFactory stnAsmFactory)
        {
            var startTime = DateTime.Now;
            try
            {
                string itemName = "MainToolAssembly";

                var asmCodeGenerator = UnifiedCodeGeneratorService.CreateForAssembly(
                    this,
                    myForm.GetProjectInfo(),
                    itemName);

                UsageTrackingService.Instance.LogAction("ASSEMBLY_CREATION_START",
                    $"Starting assembly creation for {itemName}");

                stnAsmFactory.CreateStnAsmFactory(myForm.GetToolingInfo());
                stnAsmFactory.CreateToolAsmFactory(
                    myForm.GetProjectInfo(),
                    asmCodeGenerator.AskDrawingCode(),
                    itemName);

                var duration = (DateTime.Now - startTime).TotalMilliseconds;
                UsageTrackingService.Instance.LogAction("ASSEMBLY_CREATION_COMPLETE",
                    $"Assembly creation completed in {duration:F0} ms");

                // End session successfully
                UsageTrackingService.Instance.EndSession(true);
            }
            catch (Exception ex)
            {
                var duration = (DateTime.Now - startTime).TotalMilliseconds;
                UsageTrackingService.Instance.LogAction("ASSEMBLY_CREATION_ERROR",
                    $"Assembly creation failed after {duration:F0}ms: {ex.Message}");

                // End session with failure
                UsageTrackingService.Instance.EndSession(false);
                throw; // Re-throw to maintain existing error handling
            }

        }
        public StripLayout GetStripLayout => stripLayout;

    }
}
