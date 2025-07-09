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
        StripLayout stripLayout;

        public NXDrawing GetDrawing => drawing;
        public formToolStructure GetForm => myForm;

        Dictionary<string, double> plateThicknesses = new Dictionary<string, double>();        


        public Control()
        {
            drawing = new NXDrawing(this);
            if (!drawing.IsDrawingOpen())
            {
                return;
            }
            stripLayout = drawing.GetStripLayout();
            myForm = new formToolStructure(this);
            myForm.Show();
        }

        public void Start(StationAssemblyFactory stnAsmFactory)
        {
            stnAsmFactory.CreateStnAsmFactory();
            stnAsmFactory.CreateToolAsmFactory();
        }
        public StripLayout GetStripLayout => stripLayout;

    }
}
