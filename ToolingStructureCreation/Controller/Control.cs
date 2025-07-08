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

        string folderPath = "";

        public Control()
        {
            drawing = new NXDrawing(this);

            myForm = new formToolStructure(this);
            myForm.Show();                                
        }

        public void Start(StationAssemblyFactory stnAsmFactory)
        {                     
            stnAsmFactory.CreateStnAsmFactory();
            stnAsmFactory.CreateToolAsmFactory();            
        }
        
    }
}
