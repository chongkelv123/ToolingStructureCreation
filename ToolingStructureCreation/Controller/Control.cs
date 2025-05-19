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
        ToolingWizardForm myForm;

        public NXDrawing GetDrawing => drawing;
        public ToolingWizardForm GetForm => myForm;

        public Control()
        {
            drawing = new NXDrawing(this);

            myForm = new ToolingWizardForm(this);
            myForm.ShowDialog();
        }

        public void Start()
        {

        }
    }
}
