using NXOpen;
using NXOpen.Annotations;
using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Infrastructure.NX
{
    /// <summary>
    /// Manages NX session lifecycle and provides centralized session access
    /// </summary>
    public class NXSessionManager : IDisposable
    {
        private Session _nxSession;
        private UFSession _ufSession;
        private bool _disposed = false;

        public Session NXSession => _nxSession ?? throw new InvalidOperationException("NX Session is not initialized.");
        public UFSession UFSession => _ufSession ?? throw new InvalidOperationException("UF Session is not initialized.");
        public bool IsInitialized => _nxSession != null && _ufSession != null;

        /// <summary>
        /// Initialize NX session - gets existing session or creates new one
        /// </summary>
        public void Initialize()
        {
            try
            {
                // Get existing NX session
                _nxSession = Session.GetSession();

                // Initialize UF session
                _ufSession = UFSession.GetUFSession();

                if (_nxSession == null)
                    throw new InvalidOperationException("Failed to get NX Session");

                if (_ufSession == null)
                    throw new InvalidOperationException("Failed to get UF Session");

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to initialize NX session: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get the current work part
        /// </summary>        
        public Part GetWorkPart()
        {
            EnsureInitialized();
            var workPart = NXSession.Parts.Work;

            if (workPart == null)
                throw new InvalidOperationException("No work part is currently loaded.");

            return workPart;
        }

        /// <summary>
        /// Get the current display part
        /// </summary>
        public Part GetDisplayPart()
        {
            EnsureInitialized();
            var displayPart = NXSession.Parts.Display;

            if (displayPart == null)
                throw new InvalidOperationException("No display part is currently loaded.");

            return displayPart;
        }

        /// <summary>
        /// Create undo mark for operation tracking
        /// </summary>
        public Session.UndoMarkId CreateUndoMark(string operationName, Session.MarkVisibility visibility = Session.MarkVisibility.Visible)
        {
            EnsureInitialized();
            return NXSession.SetUndoMark(visibility, operationName);
        }

        /// <summary>
        /// Update model after changes
        /// </summary>
        public void UpdateModel(Session.UndoMarkId undoMark)
        {
            EnsureInitialized();
            NXSession.UpdateManager.DoUpdate(undoMark);
        }

        /// <summary>
        /// Validate session is ready for operations
        /// </summary>
        public void ValidateSession()
        {
            EnsureInitialized();

            var workPart = GetWorkPart();
            if (workPart.IsReadOnly)
                throw new InvalidOperationException("Work part is read-only. Cannot perform modifications.");
        }

        private void EnsureInitialized()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("NX session is not initialized. Call Initialize() first.");
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                // NX Session is managed by NX itself, don't dispose
                // Just clear references
                _nxSession = null;
                _ufSession = null;
                _disposed = true;
            }
        }
    }
}
