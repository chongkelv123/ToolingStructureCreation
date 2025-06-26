using NXOpen;
using NXOpen.Annotations;
using NXOpen.Assemblies;
using NXOpen.CAE;
using NXOpen.CAE.Connections;
using NXOpen.Display;
using NXOpen.Features;
using NXOpen.Layout2d;
using NXOpen.UF;
using NXOpenUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestCreateNewPlate.Controller;
using TestCreateNewPlate.View;

namespace TestCreateNewPlate.Model
{
    public enum PlateColor
    {
        UPPERSHOE = 127,
        UPPERPAD = 91,
        PUNCHHOLDER = 59,
        BOTTOMINGPLATE = 124,
        STRIPPERPLATE = 55,
        DIEPLATE = 108,
        LOWERPAD = 92,
        LOWERSHOE = 60,
        COMMONPLATE = 80,
        PARALLELBAR = 87,
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

        public Session GetSession()
        {
            return session;
        }
        public UI GetUI()
        {
            return ui;
        }
        public UFSession GetUFSession()
        {
            return ufs;
        }
        public Part GetWorkPart()
        {
            return workPart;
        }

        public double GetUpperShoe_ZValue()
        {
            formToolStructure myForm = control.GetForm;
            double totalPlateThickness =
                myForm.UpperPadThk +
                myForm.PunHolderThk +
                myForm.BottomPltThk +
                myForm.StripperPltThk +
                myForm.MatThk +
                myForm.DiePltThk +
                myForm.LowerPadThk +
                myForm.UpperShoeThk;

            return totalPlateThickness;
        }
    }
}


