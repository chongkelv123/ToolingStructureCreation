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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
            CheckInputAndEnableApply();
        }

        private bool IsDirectoryExists()
        {
            if (Directory.Exists(txtPath.Text))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void txtUpperShoeThk_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressEvent_NumericalOnly(sender, e);
        }

        private void txtUpperPadThk_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressEvent_NumericalOnly(sender, e);
        }

        private void txtPunHolderThk_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressEvent_NumericalOnly(sender, e);
        }

        private void txtBottomPltThk_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressEvent_NumericalOnly(sender, e);
        }

        private void txtStripperPlt_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressEvent_NumericalOnly(sender, e);
        }

        private void txtMatThk_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressEvent_NumericalOnly(sender, e);
        }

        private static void KeyPressEvent_NumericalOnly(object sender, KeyPressEventArgs e)
        {
            // Allow control keys (e.g., backspace), digits, and optionally a single decimal point
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true; // Ignore the input
            }

            // Only allow one decimal point
            if (e.KeyChar == '.' && (sender as System.Windows.Forms.TextBox).Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void txtDiePltThk_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressEvent_NumericalOnly(sender, e);
        }

        private void txtLowerPadThk_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressEvent_NumericalOnly(sender, e);
        }

        private void txtLowerShoeThk_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressEvent_NumericalOnly(sender, e);
        }

        private void txtParallelBarThk_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressEvent_NumericalOnly(sender, e);
        }

        private void txtCommonPltThk_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressEvent_NumericalOnly(sender, e);
        }

        private void txtUpperShoeThk_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
        }
        private void CheckInputAndEnableApply()
        {
            bool allFilled =
                !string.IsNullOrWhiteSpace(txtUpperShoeThk.Text) &&
                !string.IsNullOrWhiteSpace(txtUpperPadThk.Text) &&
                !string.IsNullOrWhiteSpace(txtPunHolderThk.Text) &&
                !string.IsNullOrWhiteSpace(txtBottomPltThk.Text) &&
                !string.IsNullOrWhiteSpace(txtStripperPlt.Text) &&
                !string.IsNullOrWhiteSpace(txtMatThk.Text) &&
                !string.IsNullOrWhiteSpace(txtDiePltThk.Text) &&
                !string.IsNullOrWhiteSpace(txtLowerPadThk.Text) &&
                !string.IsNullOrWhiteSpace(txtLowerShoeThk.Text) &&
                !string.IsNullOrWhiteSpace(txtParallelBarThk.Text) &&
                !string.IsNullOrWhiteSpace(txtCommonPltThk.Text) &&
                IsDirectoryExists();

            btnApply.Enabled = allFilled;
        }

        private void txtUpperPadThk_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
        }

        private void txtPunHolderThk_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
        }

        private void txtBottomPltThk_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
        }

        private void txtStripperPlt_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
        }

        private void txtMatThk_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
        }

        private void txtDiePltThk_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
        }

        private void txtLowerPadThk_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
        }

        private void txtLowerShoeThk_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
        }

        private void txtParallelBarThk_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
        }

        private void txtCommonPltThk_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
        }
    }
}
