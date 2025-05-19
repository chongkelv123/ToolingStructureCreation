using NXOpen;
using NXOpen.UF;
using NXOpenUI;

namespace ToolingStructureCreation.Interfaces
{
    /// <summary>
    /// Provides methods to access NX session, UF session, UI, work part, and check if a part is open.
    /// </summary>
    public interface INXSessionProvider    
    {
        /// <summary>
        /// Gets the NX session.
        /// </summary>
        /// <returns>The NX session.</returns>
        Session GetNXSession();

        /// <summary>
        /// Gets the NX open user function.
        /// </summary>
        /// <returns>The NX open user function.</returns>
        UFSession GetUFSession();

        /// <summary>
        /// Gets the NX open user interface.
        /// </summary>
        /// <returns>The NX open user interface.</returns>
        UI GetUI();
        /// <summary>
        /// Gets the work part.
        /// </summary>
        /// <returns>The work part.</returns>
        Part GetWorkPart();
        /// <summary>
        /// Checks if a part is open.
        /// </summary>
        /// <returns>True if a part is open, false otherwise.</returns>
        bool IsPartOpen();

    }
}
