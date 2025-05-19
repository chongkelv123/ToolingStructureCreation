using NXOpen;
using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Interfaces;

namespace ToolingStructureCreation.Services
{
    public class NXService: INXSessionProvider, IUIService
    {
        private readonly Session _session;
        private readonly UFSession _ufSession;
        private readonly UI _ui;

        public NXService()
        {
            _session = Session.GetSession();
            _ufSession = UFSession.GetUFSession();
            _ui = UI.GetUI();
        }

        // INXSessionProvider implementation
        public Session GetNXSession() => _session;
        public UFSession GetUFSession() => _ufSession;
        public UI GetUI() => _ui;
        public Part GetWorkPart() => _session.Parts.Work;
        public bool IsPartOpen() => _session.Parts.Work != null;

        // IUIService implementation
        public void ShowInfo(string message)
        {
            _ui.NXMessageBox.Show("Information", NXMessageBox.DialogType.Information, message);
        }

        public void ShowError(string message)
        {
            _ui.NXMessageBox.Show("Error", NXMessageBox.DialogType.Error, message);
        }

        public void ShowWarning(string message)
        {
            _ui.NXMessageBox.Show("Warning", NXMessageBox.DialogType.Warning, message);
        }

    }
}
