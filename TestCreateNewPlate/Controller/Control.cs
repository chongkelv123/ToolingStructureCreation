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
        formToolStructure myForm;

        public NXDrawing GetDrawing => drawing;
        public formToolStructure GetForm => myForm;

        Dictionary<string, double> plateThicknesses = new Dictionary<string, double>();

        string folderPath;

        public Control()
        {
            drawing = new NXDrawing(this);

            myForm = new formToolStructure(this);
            myForm.ShowDialog();                                       
        }

        public void Start()
        {
            plateThicknesses.Add(ToolingAssembly.LOWER_PAD, myForm.LowerPadThk);
            plateThicknesses.Add(ToolingAssembly.DIE_PLATE, myForm.DiePltThk);
            plateThicknesses.Add(ToolingAssembly.MAT_THK, myForm.MatThk); // Material thickness, not a plate
            plateThicknesses.Add(ToolingAssembly.STRIPPER_PLATE, myForm.StripperPltThk);
            plateThicknesses.Add(ToolingAssembly.BOTTOMING_PLATE, myForm.BottomPltThk);
            plateThicknesses.Add(ToolingAssembly.PUNCH_HOLDER, myForm.PunHolderThk);
            plateThicknesses.Add(ToolingAssembly.UPPER_PAD, myForm.UpperPadThk);

            folderPath = myForm.GetPath + "\\";
            ToolingAssembly stn1ToolStructure = new ToolingAssembly(300, 420, "Stn1", drawing, folderPath, plateThicknesses);
            ToolingAssembly stn2ToolStructure = new ToolingAssembly(300, 500, "Stn2", drawing, folderPath, plateThicknesses);
            ToolingAssembly stn3ToolStructure = new ToolingAssembly(300, 450, "Stn3", drawing, folderPath, plateThicknesses);
            ToolingAssembly stn4ToolStructure = new ToolingAssembly(300, 440, "Stn4", drawing, folderPath, plateThicknesses);

            stn1ToolStructure.CreateStationFactory();
            stn2ToolStructure.CreateStationFactory();
            stn3ToolStructure.CreateStationFactory();
            stn4ToolStructure.CreateStationFactory();

            Shoe upperShoe = new Shoe(Shoe.UPPER_SHOE, 1850, 500, myForm.UpperShoeThk, drawing);
            Shoe lowerShoe = new Shoe(Shoe.LOWER_SHOE, 1850, 500, myForm.LowerShoeThk, drawing);
            upperShoe.CreateNewShoe(folderPath);
            lowerShoe.CreateNewShoe(folderPath);

            ToolingAssembly.CreateToolAssembly(folderPath);
        }

        public double GetUpperShoe_ZValue()
        {
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
