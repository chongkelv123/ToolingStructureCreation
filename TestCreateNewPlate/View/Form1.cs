using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestCreateNewPlate.Controller;

namespace TestCreateNewPlate.View
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        Controller.Control control;
        public Form1(Controller.Control control)
        {
            InitializeComponent();
            this.control = control;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            control.Start();
            this.Close();
        }
    }
}
