using NXOpen;
using NXOpen.Annotations;
using NXOpen.CAE.Connections;
using NXOpen.Features;
using NXOpen.UF;
using NXOpenUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToolingStructureCreation.Controller;

namespace ToolingStructureCreation.Model
{
    public enum FaceColor
    {
        WCSP = 55,
        MILL = 211,
        WC = 181,
    }
    public class NXDrawing
    {
        Session session;
        Part workPart;
        UI ui;
        UFSession ufs;
        Controller.Control control;


        public NXDrawing()
        {
        }

        public NXDrawing(Controller.Control control)
        {
            session = Session.GetSession();
            ufs = UFSession.GetUFSession();
            workPart = session.Parts.Work;
            ui = UI.GetUI();


            this.control = control;
        }

        public void ShowMessageBox(string title, NXMessageBox.DialogType msgboxType, string message)
        {
            ui.NXMessageBox.Show(title, msgboxType, message);
        }

    }
}
