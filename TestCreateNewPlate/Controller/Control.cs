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
            Dictionary<string, double> list = new Dictionary<string, double>();
            list.Add("LOWER_PAD", 25.0);
            list.Add("DIE_PLATE", 35.0);
            list.Add("mat_thk", 1.55);
            list.Add("STRIPPER_PLATE", 30.0);
            list.Add("BOTTOMING_PLATE", 16.0);
            list.Add("PUNCH_HOLDER", 30.0);
            list.Add("UPPER_PAD", 27.0);

            foreach (var plate in list)
            {
                if(plate.Key.Equals("mat_thk", StringComparison.OrdinalIgnoreCase))
                {
                    // Skip material thickness, as it is not a plate
                    continue;
                }
                drawing.CreateNewPlate(plate.Key, plate.Value);
            }

            //double cummulativeThickness = 0.0;
            //foreach (var plate in list)
            //{
            //    cummulativeThickness += plate.Value;
            //    Guide.InfoWriteLine($"Plate Thickness: {plate.Key} : {plate.Value}, cumThk: {cummulativeThickness}");
            //}
            
            drawing.CreateAssembly(list);
        }
    }
}
