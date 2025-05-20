using System;
using ToolingStructureCreation.Controller;
using ToolingStructureCreation.Interfaces;
using ToolingStructureCreation.Services;
using Unity;
using Unity.Lifetime;

namespace ToolingStructureCreation
{
    public partial class MyProgram
    {
        public static void Main(string[] args)
        {
            try
            {
                // Create service instances
                var nxService = new NXService(); // Implements both INXSessionProvider and IUIService
                var selectionService = new SelectionService(nxService, nxService);
                var componentFactory = new ToolingComponentFactory(nxService);
                var toolingProcessor = new ToolingProcessor(nxService, nxService);

                // Create controller with dependencies
                var controller = new Control(
                    nxService,        // INXSessionProvider
                    nxService,        // IUIService
                    selectionService, // ISelectionService
                    toolingProcessor  // IToolingProcessor
                );

                controller.Initialize();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Application error: {ex.Message}", "Error",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
    }
}
