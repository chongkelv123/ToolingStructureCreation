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
using ToolingStructureCreation.Controller;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ToolingStructureCreation.View
{
    public partial class formToolStructure : System.Windows.Forms.Form
    {
        Controller.Control control;
        public string GetPath => txtPath.Text;
        public double UpperShoeThk => double.TryParse(txtUpperShoeThk.Text, out double value) ? value : 0.0;
        public double UpperPadThk => double.TryParse(txtUpperPadThk.Text, out double value) ? value : 0.0;
        public double PunHolderThk => double.TryParse(txtPunHolderThk.Text, out double value) ? value : 0.0;
        public double BottomPltThk => double.TryParse(txtBottomPltThk.Text, out double value) ? value : 0.0;
        public double StripperPltThk => double.TryParse(txtStripperPlt.Text, out double value) ? value : 0.0;
        public double MatThk => double.TryParse(txtMatThk.Text, out double value) ? value : 0.0;
        public double DiePltThk => double.TryParse(txtDiePltThk.Text, out double value) ? value : 0.0;
        public double LowerPadThk => double.TryParse(txtLowerPadThk.Text, out double value) ? value : 0.0;
        public double LowerShoeThk => double.TryParse(txtLowerShoeThk.Text, out double value) ? value : 0.0;
        public double ParallelBarThk => double.TryParse(txtParallelBarThk.Text, out double value) ? value : 0.0;
        public double CommonPltThk => double.TryParse(txtCommonPltThk.Text, out double value) ? value : 0.0;
        public formToolStructure(Controller.Control control)
        {
            InitializeComponent();
            this.control = control;
            UpdateDieHeight();
            UpdatePunchLength();
            UpdatePenetration();
            UpdateFeedHeight();
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
            UpdateDieHeight();            
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
            UpdateDieHeight();
        }

        private void txtPunHolderThk_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
            UpdateDieHeight();
            UpdatePunchLength();
            UpdatePenetration();
        }

        private void txtBottomPltThk_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
            UpdateDieHeight();
            UpdatePunchLength();
            UpdatePenetration();
        }

        private void txtStripperPlt_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
            UpdateDieHeight();
            UpdatePunchLength();
            UpdatePenetration();
        }

        private void txtMatThk_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
            UpdateDieHeight();
            UpdatePunchLength();
            UpdatePenetration();
        }

        private void txtDiePltThk_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
            UpdateDieHeight();
            UpdateFeedHeight();
        }

        private void txtLowerPadThk_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
            UpdateDieHeight();
            UpdateFeedHeight();
        }

        private void txtLowerShoeThk_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
            UpdateDieHeight();
            UpdateFeedHeight();
        }

        private void txtParallelBarThk_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
            UpdateDieHeight();
            UpdateFeedHeight();
        }

        private void txtCommonPltThk_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
            UpdateDieHeight();
            UpdateFeedHeight();
        }

        private double SumAllThickness(params System.Windows.Forms.TextBox[] textBoxes)
        {
            double sum = 0;
            foreach (var tb in textBoxes)
            {
                if (double.TryParse(tb.Text, out double value))
                {
                    sum += value;
                }
            }
            return sum;
        }

        private double SnapToNearestBand(double value, List<double> bands)
        {
            foreach (var band in bands.OrderByDescending(b => b))
            {
                if (value > band && value <= band + 10.0)
                {
                    return band + 10.0;
                }
                if (value == band)
                {
                    return band;
                }
            }
            return value;
        }

        private double GetLowerDieSetThickness()
        {
            return SumAllThickness(
                txtLowerShoeThk, txtLowerPadThk, txtDiePltThk,
                txtParallelBarThk, txtCommonPltThk
                );
        }
        private void UpdateFeedHeight()
        {
            bool IsLiftHeightFilled = !string.IsNullOrWhiteSpace(txtLiftHeight.Text);
            if (IsLiftHeightFilled)
            {
                double liftHeight = double.TryParse(txtLiftHeight.Text, out double value) ? value : 0.0;
                double lowerDieSetThickness = GetLowerDieSetThickness();
                txtFeedHeight.Text = (liftHeight + lowerDieSetThickness).ToString();
            }
            else
            {
                txtFeedHeight.Text = string.Empty;
            }
        }

        private double GetPHld_BPlt_SPlt_MatThk()
        {          
            return SumAllThickness(
                txtPunHolderThk,
                txtBottomPltThk, txtStripperPlt, txtMatThk
                );
        }

        private double GetPunchLength()
        {
            List<double> punchLengthList = new List<double>
            {
                50.0, 60.0, 70.0, 80.0
            };

            double value = GetPHld_BPlt_SPlt_MatThk();

            return SnapToNearestBand(value, punchLengthList);
        }

        private void UpdatePunchLength()
        {
            txtPunchLength.Text = GetPunchLength().ToString();
        }

        private void UpdatePenetration()
        {
            double penetration = GetPunchLength() - GetPHld_BPlt_SPlt_MatThk();
            txtPenetration.Text = penetration.ToString();
        }

        private void UpdateDieHeight()
        {
            double totalThickness = SumAllThickness(
                txtUpperShoeThk, txtUpperPadThk, txtPunHolderThk,
                txtBottomPltThk, txtStripperPlt, txtMatThk,
                txtDiePltThk, txtLowerPadThk, txtLowerShoeThk,
                txtParallelBarThk, txtCommonPltThk);

            txtDieHeight.Text = totalThickness.ToString();
        }

        private void txtLiftHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressEvent_NumericalOnly(sender, e);
        }

        private void txtLiftHeight_TextChanged(object sender, EventArgs e)
        {
            UpdateFeedHeight();
        }
    }
}
