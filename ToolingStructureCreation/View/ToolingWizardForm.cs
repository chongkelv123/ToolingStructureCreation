using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToolingStructureCreation.Controller;

namespace ToolingStructureCreation.View
{
    public partial class ToolingWizardForm : Form
    {
        private Controller.Control _control;
        public ToolingWizardForm(Controller.Control control)
        {
            InitializeComponent();            
            _control = control;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
