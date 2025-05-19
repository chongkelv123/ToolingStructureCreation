using NXOpen;
using NXOpen.Features;
using System;
using System.ComponentModel.Design;
using ToolingStructureCreation.Interfaces;

namespace ToolingStructureCreation.Services
{
    public class SelectionService: INXSelectionService
    {
        private readonly INXSessionProvider _sessionProvider;
        private readonly IUIService _uiService;

        private TaggedObject _selectedComponent;
        private bool _isBaseComponentSelected;

        public SelectionService(INXSessionProvider sessionProvider, IUIService uiService)
        {
            _sessionProvider = sessionProvider ?? throw new ArgumentNullException(nameof(sessionProvider));
            _uiService = uiService ?? throw new ArgumentNullException(nameof(uiService));

            _selectedComponent = null;
            _isBaseComponentSelected = false;
        }

        public TaggedObject SelectComponent(ComponentType type)
        {
            try
            {
                // Set appropriate selection filter based on component type
                Selection.SelectionType[] selectionTypes;

                if (type == ComponentType.PlateSketch)
                {
                    selectionTypes = new Selection.SelectionType[] {
                        //Selection.SelectionType.Sketch
                    };
                }
                else // ShoeSketch
                {
                    selectionTypes = new Selection.SelectionType[] {
                        //Selection.SelectionType.Sketch
                    };
                }

                // Create NX selection object
                var ui = _sessionProvider.GetUI();
                var selectionUI = ui.SelectionManager;

                // Set selection filter
                //selectionUI.SetSelectionMask(selectionTypes);

                // Prompt for selection
                string prompt = $"Select {type} as base component";
                //var response = selectionUI.SelectTaggedObject(prompt, prompt, out _selectedComponent);

                //if (response == Selection.Response.ObjectSelected ||
                //    response == Selection.Response.ObjectSelectedByName)
                //{
                //    _isBaseComponentSelected = true;
                //    return _selectedComponent;
                //}

                _isBaseComponentSelected = false;
                return null;
            }
            catch (Exception ex)
            {
                _uiService.ShowError($"Error selecting component: {ex.Message}");
                _isBaseComponentSelected = false;
                return null;
            }
        }

        public bool IsBaseComponentSelected => _isBaseComponentSelected;

        public TaggedObject GetSelectedComponent() => _selectedComponent;
    }
}
