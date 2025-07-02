using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolingStructureCreation.Model;
using ToolingStructureCreation.View;

namespace ToolingStructureCreation.Controller
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
            plateThicknesses.Add(NXDrawing.LOWER_PAD, myForm.LowerPadThk);
            plateThicknesses.Add(NXDrawing.DIE_PLATE, myForm.DiePltThk);
            plateThicknesses.Add(NXDrawing.MAT_THK, myForm.MatThk); // Material thickness, not a plate
            plateThicknesses.Add(NXDrawing.STRIPPER_PLATE, myForm.StripperPltThk);
            plateThicknesses.Add(NXDrawing.BOTTOMING_PLATE, myForm.BottomPltThk);
            plateThicknesses.Add(NXDrawing.PUNCH_HOLDER, myForm.PunHolderThk);
            plateThicknesses.Add(NXDrawing.UPPER_PAD, myForm.UpperPadThk);

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

            ParallelBar parallelBar = new ParallelBar(60, 415, myForm.ParallelBarThk, drawing);
            parallelBar.CreateNewParallelBar(folderPath);

            CommonPlate commonPlate = new CommonPlate(2300, 980, myForm.CommonPltThk, drawing);
            commonPlate.CreateNewCommonPlate(folderPath);

            ToolingAssembly.CreateToolAssembly(folderPath);
        }
        
    }
}
