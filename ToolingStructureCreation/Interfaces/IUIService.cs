using NXOpen;

namespace ToolingStructureCreation.Interfaces
{
    /// <summary>
    /// Interface for displaying various types of messages in the UI.
    /// </summary>
    public interface IUIService
    {
        /// <summary>
        /// Shows an informational message.
        /// </summary>
        /// <param name="message">The message to display.</param>
        void ShowInfo(string message);

        /// <summary>
        /// Shows an error message.
        /// </summary>
        /// <param name="message">The error message to display.</param>
        void ShowError(string message);

        /// <summary>
        /// Shows a warning message.
        /// </summary>
        /// <param name="message">The warning message to display.</param>
        void ShowWarning(string message); 
    }
}
