using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCreateNewPlate.Model;
using TestCreateNewPlate.View;

namespace TestCreateNewPlate.Controller
{
    public class Control
    {
        NXDrawing drawing;
        Form1 myForm;

        public NXDrawing GetDrawing => drawing;
        public Form1 GetForm => myForm;

        public Control()
        {
            drawing = new NXDrawing(this);

            myForm = new Form1(this);
            myForm.ShowDialog();
        }

        public void Start()
        {
            //System.Diagnostics.Debugger.Launch();
            StationToolingStructure stn1ToolStructure = new StationToolingStructure(300, 420, "Stn1");
            StationToolingStructure stn2ToolStructure = new StationToolingStructure(300, 500, "Stn2");
            drawing.CreateStationFactory(stn1ToolStructure);
            drawing.CreateStationFactory(stn2ToolStructure);
            drawing.CreateToolAssembly();
        }
    }
}
