using NXOpen;
using NXOpen.Features;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using ToolingStructureCreation.Interfaces;

namespace ToolingStructureCreation.Services
{
    /// <summary>
    /// Service for selecting components in NX, particularly sketches for tooling structure creation.
    /// </summary>
    public class SelectionService : INXSelectionService
    {
        private readonly INXSessionProvider _sessionProvider;
        private readonly IUIService _uiService;

        private TaggedObject _selectedComponent;
        private bool _isBaseComponentSelected;
        private Dictionary<ComponentType, TaggedObject> _cachedSelections = new Dictionary<ComponentType, TaggedObject>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionService"/> class.
        /// </summary>
        /// <param name="sessionProvider">The NX session provider.</param>
        /// <param name="uiService">The UI service for user feedback.</param>
        public SelectionService(INXSessionProvider sessionProvider, IUIService uiService)
        {
            _sessionProvider = sessionProvider ?? throw new ArgumentNullException(nameof(sessionProvider));
            _uiService = uiService ?? throw new ArgumentNullException(nameof(uiService));

            _selectedComponent = null;
            _isBaseComponentSelected = false;
        }

        /// <summary>
        /// Selects a component in NX based on the specified component type.
        /// </summary>
        /// <param name="type">The type of component to select.</param>
        /// <returns>The selected TaggedObject, or null if selection failed or was canceled.</returns>
        public TaggedObject SelectComponent(ComponentType type)
        {
            try
            {
                // Validate active part
                if (!_sessionProvider.IsPartOpen())
                {
                    _uiService.ShowWarning("No active part. Please open a part file first.");
                    return null;
                }

                // Get UI and selection manager
                var ui = _sessionProvider.GetUI();
                var selectionUI = ui.SelectionManager;

                // Start progress feedback
                StartProgressFeedback("Component Selection");

                // Create selection filter for sketches
                Selection.MaskTriple[] maskTriple = CreateSelectionFilter(type);

                // Clear current selection
                //selectionUI.ClearSelectionList();

                // Set up prompt based on component type
                string prompt = GetSelectionPrompt(type);
                

                // Update progress
                UpdateProgressFeedback(1, "Waiting for user selection...");

                // Display prompt and wait for user to select sketch
                TaggedObject selectedObject;
                Point3d cursor;
                Selection.Response response = selectionUI.SelectTaggedObject(
                    prompt,
                    prompt,
                    Selection.SelectionScope.WorkPart,
                    Selection.SelectionAction.ClearAndEnableSpecific,
                    false,
                    false,
                    maskTriple,
                    out selectedObject,
                    out cursor);

                // Update progress
                UpdateProgressFeedback(2, "Processing selection...");

                if (response == Selection.Response.ObjectSelected ||
                    response == Selection.Response.ObjectSelectedByName)
                {
                    // Check if selected object is valid for the component type
                    if (ValidateSelection(selectedObject, type))
                    {
                        // Cache the selection for future use
                        _cachedSelections[type] = selectedObject;

                        // Store the selected component and update state
                        _selectedComponent = selectedObject;
                        _isBaseComponentSelected = true;

                        // Log successful selection
                        var sketchName = GetObjectName(selectedObject);
                        _uiService.ShowInfo($"{type} sketch '{sketchName}' selected successfully");

                        // End progress
                        EndProgressFeedback();

                        return _selectedComponent;
                    }
                    else
                    {
                        _uiService.ShowWarning($"Selected object is not a valid {type} sketch. Please select a valid sketch.");
                    }
                }
                else if (response == Selection.Response.Cancel)
                {
                    _uiService.ShowInfo("Selection was cancelled");
                }                

                // End progress
                EndProgressFeedback();

                // If we reach here, no valid selection was made
                _isBaseComponentSelected = false;
                return null;
            }
            catch (Exception ex)
            {
                _uiService.ShowError($"Error selecting component: {ex.Message}");
                EndProgressFeedback();
                _isBaseComponentSelected = false;
                return null;
            }
        }

        /// <summary>
        /// Gets whether a base component is currently selected.
        /// </summary>
        public bool IsBaseComponentSelected => _isBaseComponentSelected;

        /// <summary>
        /// Gets the currently selected component.
        /// </summary>
        /// <returns>The selected component, or null if no component is selected.</returns>
        public TaggedObject GetSelectedComponent() => _selectedComponent;

        /// <summary>
        /// Gets a previously cached selection for the specified component type.
        /// </summary>
        /// <param name="type">The component type to get the cached selection for.</param>
        /// <returns>The cached selection, or null if no selection is cached for the specified type.</returns>
        public TaggedObject GetCachedSelection(ComponentType type)
        {
            return _cachedSelections.TryGetValue(type, out var cachedSelection)
                ? cachedSelection
                : null;
        }

        /// <summary>
        /// Clears all cached selections.
        /// </summary>
        public void ClearCachedSelections()
        {
            _cachedSelections.Clear();
        }

        #region Private Helper Methods

        /// <summary>
        /// Creates a selection filter for the specified component type.
        /// </summary>
        /// <param name="type">The component type to create a filter for.</param>
        /// <returns>An array of selection mask triples.</returns>
        private Selection.MaskTriple[] CreateSelectionFilter(ComponentType type)
        {
            Selection.MaskTriple[] maskTriple = new Selection.MaskTriple[1];

            // For now, both types use the same filter (sketches only)
            // This can be customized per component type if needed
            maskTriple[0] = new Selection.MaskTriple(
                NXOpen.UF.UFConstants.UF_sketch_type,
                NXOpen.UF.UFConstants.UF_all_subtype,
                0);  // 0 means any value for the solid type

            return maskTriple;
        }

        /// <summary>
        /// Gets the selection prompt for the specified component type.
        /// </summary>
        /// <param name="type">The component type to get a prompt for.</param>
        /// <returns>A user-friendly selection prompt.</returns>
        private string GetSelectionPrompt(ComponentType type)
        {
            switch (type)
            {
                case ComponentType.PlateSketch:
                    return "Select Plate Sketch";
                case ComponentType.ShoeSketch:
                    return "Select Shoe Sketch";
                default:
                    return "Select Component";
            }
        }

        /// <summary>
        /// Validates that the selected object is appropriate for the specified component type.
        /// </summary>
        /// <param name="obj">The object to validate.</param>
        /// <param name="type">The component type to validate against.</param>
        /// <returns>True if the object is valid for the component type; otherwise, false.</returns>
        private bool ValidateSelection(TaggedObject obj, ComponentType type)
        {
            // First, check if the object is a sketch
            if (!(obj is Sketch sketch))
                return false;

            switch (type)
            {
                case ComponentType.PlateSketch:
                    // Add plate-specific validation logic if needed
                    // For example, checking sketch dimensions, properties, etc.
                    return true;

                case ComponentType.ShoeSketch:
                    // Add shoe-specific validation logic if needed
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets the name of an NX object.
        /// </summary>
        /// <param name="obj">The object to get the name of.</param>
        /// <returns>The name of the object, or "Unnamed" if it has no name.</returns>
        private string GetObjectName(TaggedObject obj)
        {
            if (obj == null)
                return "Unnamed";

            try
            {
                DisplayableObject displayableObject = obj as DisplayableObject;
                if (displayableObject != null && !string.IsNullOrEmpty(displayableObject.Name))
                {
                    return displayableObject.Name;
                }

                // For other objects, try to get a meaningful identifier
                NXObject nXObject = obj as NXObject;
                return nXObject.JournalIdentifier ?? "Unnamed";
            }
            catch
            {
                return "Unnamed";
            }
        }

        #endregion

        #region Progress Feedback

        private Session.UndoMarkId _undoMarkId;
        private bool _progressActive = false;

        /// <summary>
        /// Starts progress feedback for a long-running operation.
        /// </summary>
        /// <param name="title">The title of the operation.</param>
        private void StartProgressFeedback(string title)
        {
            try
            {
                if (_progressActive)
                    return;

                _progressActive = true;

                var session = _sessionProvider.GetNXSession();
                _undoMarkId = session.SetUndoMark(Session.MarkVisibility.Invisible, title);

                // For more complex implementations, you could use NXOpen's progress bar
                // session.ProgressBar.StartProgress(title, 3); // 3 steps
            }
            catch
            {
                // Silently handle progress bar failures - they shouldn't stop the main operation
            }
        }

        /// <summary>
        /// Updates progress feedback for a long-running operation.
        /// </summary>
        /// <param name="step">The current step.</param>
        /// <param name="message">A message describing the current step.</param>
        private void UpdateProgressFeedback(int step, string message)
        {
            try
            {
                if (!_progressActive)
                    return;

                // For more complex implementations:
                // var session = _sessionProvider.GetNXSession();
                // session.ProgressBar.UpdateProgress(step, message);
            }
            catch
            {
                // Silently handle progress bar failures
            }
        }

        /// <summary>
        /// Ends progress feedback for a long-running operation.
        /// </summary>
        private void EndProgressFeedback()
        {
            try
            {
                if (!_progressActive)
                    return;

                _progressActive = false;

                // For more complex implementations:
                // var session = _sessionProvider.GetNXSession();
                // session.ProgressBar.EndProgress();
            }
            catch
            {
                // Silently handle progress bar failures
            }
        }

        #endregion
    }

    /// <summary>
    /// Extension methods for TaggedObject.
    /// </summary>
    public static class TaggedObjectExtensions
    {
        /// <summary>
        /// Determines whether the specified object is a sketch.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the object is a sketch; otherwise, false.</returns>
        public static bool IsSketch(this TaggedObject obj)
        {
            return obj is Sketch;
        }

        /// <summary>
        /// Tries to get the name of an NX object.
        /// </summary>
        /// <param name="obj">The object to get the name of.</param>
        /// <returns>The name of the object, or null if it has no name.</returns>
        public static string TryGetName(this TaggedObject obj)
        {
            if (obj == null)
                return null;

            try
            {
                DisplayableObject displayableObject = obj as DisplayableObject;
                if (displayableObject != null)
                {
                    return displayableObject.Name;
                }
                NXObject nXObject = obj as NXObject;
                return nXObject.JournalIdentifier;

            }
            catch
            {
                return null;
            }
        }
    }
}
