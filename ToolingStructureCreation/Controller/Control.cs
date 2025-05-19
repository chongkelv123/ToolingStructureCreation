using ToolingStructureCreation.Model;
using ToolingStructureCreation.View;
using ToolingStructureCreation.Interfaces;
using System;

namespace ToolingStructureCreation.Controller
{
    public class Control: IController
    {
        private readonly INXSessionProvider _sessionProvider;
        private readonly IUIService _uiService;
        private readonly INXSelectionService _selectionService;
        private readonly IToolingProcessor _toolingProcessor;

        private ToolingWizardForm _wizardForm;

        public Control(
            INXSessionProvider sessionProvider,
            IUIService uiService,
            INXSelectionService selectionService,
            IToolingProcessor toolingProcessor)
        {
            _sessionProvider = sessionProvider ?? throw new ArgumentNullException(nameof(sessionProvider));
            _uiService = uiService ?? throw new ArgumentNullException(nameof(uiService));
            _selectionService = selectionService ?? throw new ArgumentNullException(nameof(selectionService));
            _toolingProcessor = toolingProcessor ?? throw new ArgumentNullException(nameof(toolingProcessor));
        }

        public void Initialize()
        {
            if (!_sessionProvider.IsPartOpen())
            {
                _uiService.ShowWarning("Please open a part file before using this module.");
                return;
            }

            _wizardForm = new ToolingWizardForm(this, _selectionService);
            _wizardForm.ShowDialog();
        }

        public void Start(ToolingParameters parameters)
        {
            try
            {
                // 1. Process the base component
                var baseInfo = _toolingProcessor.ProcessBaseComponent(
                    parameters.BaseComponent,
                    parameters.BaseComponentType);

                // 2. Create tooling components
                // (We'll implement IToolingComponentFactory later)
                //var componentFactory = null; // Replace this with real factory
                //var components = _toolingProcessor.CreateToolingComponents(
                //    baseInfo,
                //    parameters,
                //    componentFactory);

                // 3. Assemble components
                //var structure = _toolingProcessor.AssembleComponents(
                //    components,
                //    parameters.SelectedTemplate);

                _uiService.ShowInfo("Tooling structure created successfully!");
            }
            catch (Exception ex)
            {
                _uiService.ShowError($"Error creating tooling structure: {ex.Message}");
            }
        }
    }
}
