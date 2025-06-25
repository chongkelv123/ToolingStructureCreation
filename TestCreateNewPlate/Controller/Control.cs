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

        string folderPath = "C:\\CreateFolder\\Testing-Tooling-Structure\\";

        public Control()
        {
            drawing = new NXDrawing(this);

            myForm = new Form1(this);
            myForm.ShowDialog();
        }

        public void Start()
        {            
            StationToolingStructure stn1ToolStructure = new StationToolingStructure(300, 420, "Stn1", drawing, folderPath);
            StationToolingStructure stn2ToolStructure = new StationToolingStructure(300, 500, "Stn2", drawing, folderPath);
            stn1ToolStructure.CreateStationFactory();
            stn2ToolStructure.CreateStationFactory();
            drawing.CreateToolAssembly(folderPath);
        }
    }
}
