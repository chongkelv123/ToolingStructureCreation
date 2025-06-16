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
            StationToolingStructure toolingStructure = new StationToolingStructure();
            var list = toolingStructure.GetPlateThicknesses();
            foreach (var plate in list)
            {
                if(plate.Key.Equals("mat_thk", StringComparison.OrdinalIgnoreCase))
                {
                    // Skip material thickness, as it is not a plate
                    continue;
                }
                drawing.CreateNewPlate(plate.Key, plate.Value);
            }

            drawing.CreateStationAssembly(list, "Stn1_Assembly");
            
        }
    }
}
