using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestCreateNewPlate.Controller;

namespace TestCreateNewPlate.View
{
    public partial class formToolStructure : System.Windows.Forms.Form
    {
        Controller.Control control;
        public string GetPath => txtPath.Text;
        public formToolStructure(Controller.Control control)
        {
            InitializeComponent();
            this.control = control;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            control.Start();
            this.Close();
        }

        private void txtPath_TextChanged(object sender, EventArgs e)
        {
            CheckApplyButtonStatus();
        }

        private void CheckApplyButtonStatus()
        {
            if (Directory.Exists(txtPath.Text))
            {
                btnApply.Enabled = true;
            }
            else
            {
                btnApply.Enabled = false;
            }
        }
    }
}
