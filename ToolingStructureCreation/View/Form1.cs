using NXOpen;
using NXOpen.Annotations;
using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToolingStructureCreation.Controller;
using ToolingStructureCreation.Model;
using ToolingStructureCreation.Services;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ToolingStructureCreation.View
{
    public partial class formToolStructure : System.Windows.Forms.Form
    {
        private readonly ManufacturingCalculationService _calculationService;
        private readonly FormValidator _validator;

        Controller.Control control;
        public string GetPath => txtPath.Text + "\\";
        public double UpperShoeThk => double.TryParse(txtUpperShoeThk.Text, out double value) ? value : 0.0;
        public double UpperPadThk => double.TryParse(txtUpperPadThk.Text, out double value) ? value : 0.0;
        public double PunHolderThk => double.TryParse(txtPunHolderThk.Text, out double value) ? value : 0.0;
        public double BottomPltThk => double.TryParse(txtBottomPltThk.Text, out double value) ? value : 0.0;
        public double StripperPltThk => double.TryParse(txtStripperPltThk.Text, out double value) ? value : 0.0;
        public double MatThk => double.TryParse(txtMatThk.Text, out double value) ? value : 0.0;
        public double DiePltThk => double.TryParse(txtDiePltThk.Text, out double value) ? value : 0.0;
        public double LowerPadThk => double.TryParse(txtLowerPadThk.Text, out double value) ? value : 0.0;
        public double LowerShoeThk => double.TryParse(txtLowerShoeThk.Text, out double value) ? value : 0.0;
        public double ParallelBarThk => double.TryParse(txtParallelBarThk.Text, out double value) ? value : 0.0;
        public double CommonPltThk => double.TryParse(txtCommonPltThk.Text, out double value) ? value : 0.0;
        public double CoilWidth => double.TryParse(txtCoilWidth.Text, out double value) ? value : 0.0;
        public string GetMachineName => cboMachine.SelectedItem.ToString();
        public MaterialGuideType MaterialGuideType { get; set; }
        public Machine GetMachine => machine;
        public string GetModel => txtModel.Text.Trim();
        public string GetPart => txtPart.Text.Trim();
        public string GetCodePrefix => txtCodePrefix.Text.Trim();
        public string GetDesginer => cboDesign.SelectedItem?.ToString() ?? cboDesign.Text;

        bool isPlateSketchSelected = false;
        bool isShoeSketchSelected = false;
        bool isComPltSketchSelected = false;
        bool showDebugMessage = false; // Set to true to show debug messages

        public bool IsPlateSketchSelected => isPlateSketchSelected;
        public bool IsShoeSketchSelected => isShoeSketchSelected;

        const string PLATE = "Plate";
        const string SHOE = "Shoe";
        const string COMPLATE = "ComPlt";

        TaggedObject[] plateTaggedObjects;
        TaggedObject[] shoeTaggedObjects;
        TaggedObject[] comPltTaggedObjects;
        List<Model.Sketch> stationSketchLists;
        List<Model.Sketch> shoeSketchLists;
        List<Model.Sketch> comPlateSketchList;
        Machine machine;        

        public formToolStructure(Controller.Control control)
        {
            InitializeComponent();
            InitializeCboMachine();
            InitializeCboDesign();

            _calculationService = new ManufacturingCalculationService();
            _validator = new FormValidator();

            this.control = control;
            UpdateMatGuideCoverStatus();
            UpdateAllCalculations();
        }

        // =============================================================================
        // 2. Create helper method to get current thickness data
        // =============================================================================
        private ThicknessData GetCurrentThicknessData()
        {
            return ThicknessData.FromForm(
                UpperShoeThk, UpperPadThk, PunHolderThk,
                BottomPltThk, StripperPltThk, MatThk,
                DiePltThk, LowerPadThk, LowerShoeThk,
                ParallelBarThk, CommonPltThk
            );
        }

        // =============================================================================
        // 4. Consolidated calculation update method
        // =============================================================================
        private void UpdateAllCalculations()
        {
            UpdateDieHeight();
            UpdatePunchLength();
            UpdatePenetration();
            UpdateFeedHeight();
        }

        public ToolingInfo GetToolingInfo()
        {
            return ToolingInfo.FromForm(
                CoilWidth,
                MaterialGuideType
            );
        }

        private void InitializeCboDesign()
        {
            Designer designer = new Designer();
            cboDesign.DataSource = designer.GetDesigners();
            cboDesign.SelectedIndex = 0; // Set default selection to the first designer
        }

        private void InitializeCboMachine()
        {
            machine = new Machine();
            cboMachine.DataSource = machine.GetMachines();
            cboMachine.SelectedIndex = 0; // Set default selection to the first machine
            UpdateCommonPltThk(machine);
        }

        private void UpdateCommonPltThk(Machine machine)
        {
            if (machine == null)
            {
                return;
            }
            var commonPlate = machine.GetCommonPlate(GetMachineName);

            txtCommonPltThk.Text = commonPlate.GetThickness().ToString();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            var startTime = DateTime.Now;

            // Capture engineer name at the moment of Apply click
            string engineerName = cboDesign.Text;
            if (!string.IsNullOrEmpty(engineerName))
            {
                UsageTrackingService.Instance.LogAction("ENGINEER_IDENTIFIED", engineerName);
            }

            // Capture project information (MODEL/PART)
            ProjectInfo projectInfo = GetProjectInfo();
            UsageTrackingService.Instance.UpdateSessionProjectInfo(
                projectInfo.Model ?? "Unknown",
                projectInfo.Part ?? "Unknown");

            // Capture engineer name
            UsageTrackingService.Instance.UpdateSessionEngineerInfo(engineerName);

            // Log configuration before processing
            UsageTrackingService.Instance.UpdateSessionConfiguration(
                GetMachineName,
                MaterialGuideType,
                stationSketchLists?.Count ?? 0);

            UsageTrackingService.Instance.LogAction("APPLY_CLICKED",
                $"Apply clicked by {engineerName} for Model:{projectInfo.Model}, Part:{projectInfo.Part}, Stations:{stationSketchLists?.Count ?? 0}");


            try
            {
                StationAssemblyFactory stnAsmFactory = new StationAssemblyFactory(
                new Dictionary<string, double>
                {
                    { NXDrawing.LOWER_PAD, LowerPadThk },
                    { NXDrawing.DIE_PLATE, DiePltThk },
                    { NXDrawing.MAT_THK, MatThk }, // Material thickness, not a plate
                    { NXDrawing.STRIPPER_PLATE, StripperPltThk },
                    { NXDrawing.BOTTOMING_PLATE, BottomPltThk },
                    { NXDrawing.PUNCH_HOLDER, PunHolderThk },
                    { NXDrawing.UPPER_PAD, UpperPadThk }
                },
                stationSketchLists,
                shoeSketchLists,
                comPlateSketchList,
                GetPath,
                control
            );

                control.Start(stnAsmFactory);

                var duration = (DateTime.Now - startTime).TotalMilliseconds;
                UsageTrackingService.Instance.LogAction("FORM_APPLY_SUCCESS", $"Form processing completed in {duration:F0}ms");

                this.Close();
            }
            catch (Exception ex)
            {
                var duration = (DateTime.Now - startTime).TotalMilliseconds;
                UsageTrackingService.Instance.LogAction("FORM_APPLY_ERROR",
                    $"Form processing failed after {duration:F0}ms: {ex.Message}");
                throw;
            }


        }

        private void txtPath_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
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

        private void CheckInputAndEnableApply()
        {
            var validationData = new FormValidationData
            {
                Path = txtPath.Text,
                UpperShoeThk = txtUpperShoeThk.Text,
                UpperPadThk = txtUpperPadThk.Text,
                PunHolderThk = txtPunHolderThk.Text,
                BottomPltThk = txtBottomPltThk.Text,
                StripperPltThk = txtStripperPltThk.Text,
                MatThk = txtMatThk.Text,
                DiePltThk = txtDiePltThk.Text,
                LowerPadThk = txtLowerPadThk.Text,
                LowerShoeThk = txtLowerShoeThk.Text,
                ParallelBarThk = txtParallelBarThk.Text,
                CommonPltThk = txtCommonPltThk.Text,
                CoilWidth = txtCoilWidth.Text,
                IsPlateSketchSelected = isPlateSketchSelected,
                IsShoeSketchSelected = isShoeSketchSelected,
                Model = GetModel,
                Part = GetPart,
                CodePrefix = GetCodePrefix,
                Designer = cboDesign.Text
            };

            var validationResult = _validator.ValidateForApply(validationData);
            btnApply.Enabled = validationResult.IsValid;

            // Optional: Show validation errors in status bar or tooltip
            if (!validationResult.IsValid && showDebugMessage)
            {
                string errorMessage = string.Join("\n", validationResult.Errors);
                // Display errors if needed
            }
        }

        public double GetDiePlt_LowPadThk()
        {
            var thickness = GetCurrentThicknessData();
            return _calculationService.CalculateDiePlt_LowPadThk(thickness);
        }

        public double GetUpperShoeZPosition()
        {
            var thickness = GetCurrentThicknessData();
            return _calculationService.CalculateUpperShoeZPosition(thickness);
        }

        public double GetParallelBarZPosition()
        {
            var thickness = GetCurrentThicknessData();
            return _calculationService.CalculateParallelBarZPosition(thickness);
        }

        public double GetCommonPlateZPosition()
        {
            var thickness = GetCurrentThicknessData();
            return _calculationService.CalculateCommonPlateZPosition(thickness);
        }

        private void UpdatePunchLength()
        {
            var thickness = GetCurrentThicknessData();
            double punchLength = _calculationService.CalculatePunchLength(thickness);
            txtPunchLength.Text = punchLength.ToString("F1");
        }

        private void UpdatePenetration()
        {
            var thickness = GetCurrentThicknessData();
            double penetration = _calculationService.CalculatePenetration(thickness);
            txtPenetration.Text = penetration.ToString("F1");
        }

        private void UpdateDieHeight()
        {
            var thickness = GetCurrentThicknessData();
            double dieHeight = _calculationService.CalculateDieHeight(thickness);
            txtDieHeight.Text = dieHeight.ToString("F1");
        }

        private void UpdateFeedHeight()
        {
            if (!string.IsNullOrWhiteSpace(txtLiftHeight.Text) &&
            double.TryParse(txtLiftHeight.Text, out double liftHeight))
            {
                var thicknesses = GetCurrentThicknessData();
                double feedHeight = _calculationService.CalculateFeedHeight(thicknesses, liftHeight);
                txtFeedHeight.Text = feedHeight.ToString("F1");
            }
            else
            {
                txtFeedHeight.Text = string.Empty;
            }
        }

        private void txtLiftHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressEvent_NumericalOnly(sender, e);
        }

        private void txtLiftHeight_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
            UpdateFeedHeight();
        }

        private void btnSelectPlateSketch_Click(object sender, EventArgs e)
        {
            this.Hide();
            UsageTrackingService.Instance.LogAction("SKETCH_SELECTION_START", "Plate sketch selection initiated");

            NXDrawing xDrawing = control.GetDrawing;
            Model.SketchSelection plateSketch = new Model.SketchSelection(xDrawing);
            plateTaggedObjects = plateSketch.SelectSketch();

            if (plateTaggedObjects != null && plateTaggedObjects.Length > 0)
            {
                isPlateSketchSelected = true;
                UpdateSketchStatus(PLATE, lblPlateSketchStatus);
                stationSketchLists = plateSketch.AskListFromTaggedObjects(plateTaggedObjects);

                UsageTrackingService.Instance.LogAction("PLATE_SKETCH_SELECTED",
                    $"Selected {plateTaggedObjects.Length} plate sketches");
            }
            else
            {
                isPlateSketchSelected = false;
                UpdateSketchStatus(PLATE, lblPlateSketchStatus);
                UsageTrackingService.Instance.LogAction("PLATE_SKETCH_CANCELLED", "Plate sketch selection cancelled");
            }

            CheckInputAndEnableApply();
            this.Show();
        }

        private void UpdateSketchStatus(string sketchType, System.Windows.Forms.Label label)
        {
            const string NO_SKETCH_SELECTED = "No sketch selected";
            string updateStatusText = $"{sketchType} sketch selected";

            if (label == lblPlateSketchStatus)
            {
                label.Text = isPlateSketchSelected ? updateStatusText : NO_SKETCH_SELECTED;
                label.ForeColor = isPlateSketchSelected ? Color.Green : Color.Red;
            }
            else if (label == lblShoeSketchStatus)
            {
                label.Text = isShoeSketchSelected ? updateStatusText : NO_SKETCH_SELECTED;
                label.ForeColor = isShoeSketchSelected ? Color.Green : Color.Red;
            }
            else if (label == lblComPltSketchStatus)
            {
                label.Text = isComPltSketchSelected ? updateStatusText : NO_SKETCH_SELECTED;
                label.ForeColor = isComPltSketchSelected ? Color.Green : Color.Red;
            }
        }

        private void btnSelectShoeSketch_Click(object sender, EventArgs e)
        {
            this.Hide();
            UsageTrackingService.Instance.LogAction("SKETCH_SELECTION_START", "Shoe sketch selection initiated");

            NXDrawing xDrawing = control.GetDrawing;
            Model.SketchSelection shoeSketch = new Model.SketchSelection(xDrawing);
            shoeTaggedObjects = shoeSketch.SelectSketch();
            if (shoeTaggedObjects != null && shoeTaggedObjects.Length > 0)
            {
                isShoeSketchSelected = true;
                UpdateSketchStatus(SHOE, lblShoeSketchStatus);
                shoeSketchLists = shoeSketch.AskListFromTaggedObjects(shoeTaggedObjects);

                UsageTrackingService.Instance.LogAction("SHOE_SKETCH_SELECTED",
                    $"Selected {shoeTaggedObjects.Length} shoe sketches");
            }
            else
            {
                isShoeSketchSelected = false;
                UpdateSketchStatus(SHOE, lblShoeSketchStatus);
                UsageTrackingService.Instance.LogAction("SHOE_SKETCH_CANCELLED", "Shoe sketch selection cancelled");
            }

            CheckInputAndEnableApply();
            this.Show();
        }

        private void cboMachine_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCommonPltThk(machine);
            UpdateChkComPltSketSelection();

            UsageTrackingService.Instance.LogAction("MACHINE_SELECTED", cboMachine.SelectedItem?.ToString() ?? "Unknown");

        }

        private void UpdateChkComPltSketSelection()
        {
            if (cboMachine.SelectedItem.ToString().Equals(Machine.MC1801) ||
                cboMachine.SelectedItem.ToString().Equals(Machine.MC1202))
            {
                chkActiveComPltSkt.Enabled = true;
            }
            else
            {
                chkActiveComPltSkt.Enabled = false;
                chkActiveComPltSkt.Checked = false; // Uncheck if not applicable
            }
        }

        private void chkRetriveProjInfo_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRetriveProjInfo.Checked)
            {
                UpdateProjectInfo();
            }
            else
            {
                ClearProjectInfo();
            }
        }

        private void UpdateProjectInfo()
        {
            var dictProjInfo = ProjectInfo.ReadFromFile();
            string model = dictProjInfo[ProjectInfo.MODEL];
            string part = dictProjInfo[ProjectInfo.PART];
            string codePrefix = dictProjInfo[ProjectInfo.CODE_PREFIX];
            string designer = dictProjInfo[ProjectInfo.DESIGNER];

            txtModel.Text = model;
            txtPart.Text = part;
            txtCodePrefix.Text = codePrefix;
            cboDesign.Text = designer;
        }

        private void ClearProjectInfo()
        {
            txtModel.Text = string.Empty;
            txtPart.Text = string.Empty;
            txtCodePrefix.Text = string.Empty;
            cboDesign.SelectedIndex = -1; // Clear selection
        }

        private void btnSaveProjInfo_Click(object sender, EventArgs e)
        {
            UpdateProjectInfoToFile();
        }
        private void UpdateProjectInfoToFile()
        {
            if (IsProjectInfoFilled())
            {
                List<string> info = new List<string>() {
                    GetModel,
                    GetPart,
                    GetCodePrefix,
                    GetDesginer };
                if (showDebugMessage)
                {
                    string message = "";
                    message += $"Model: {GetModel}\n";
                    message += $"Part: {GetPart}\n";
                    message += $"Code Prefix: {GetCodePrefix}\n";
                    message += $"Designer: {GetDesginer}\n";
                    Guide.InfoWriteLine(message);
                }

                ProjectInfo.WriteToFile(info);
            }
        }

        private bool IsProjectInfoFilled()
        {
            bool isFilled =
                !string.IsNullOrWhiteSpace(GetModel) &&
                !string.IsNullOrWhiteSpace(GetPart) &&
                !string.IsNullOrWhiteSpace(GetCodePrefix) &&
                !string.IsNullOrWhiteSpace(GetDesginer);

            return isFilled;
        }

        public ProjectInfo GetProjectInfo()
        {
            ProjectInfo projectInfo = new ProjectInfo(
                GetModel,
                GetPart,
                GetCodePrefix,
                GetDesginer
            );
            return projectInfo;
        }        

        private void chkActiveComPltSkt_CheckedChanged(object sender, EventArgs e)
        {
            if (chkActiveComPltSkt.Checked)
            {
                btnSelectComPltSketch.Enabled = true;
            }
            else
            {
                btnSelectComPltSketch.Enabled = false;
            }
        }

        private void btnSelectComPltSketch_Click(object sender, EventArgs e)
        {
            this.Hide();

            NXDrawing xDrawing = control.GetDrawing;
            Model.SketchSelection comPltSketch = new Model.SketchSelection(xDrawing);
            comPltTaggedObjects = comPltSketch.SelectSketch();
            if (comPltTaggedObjects != null && comPltTaggedObjects.Length > 0)
            {
                isComPltSketchSelected = true;
                UpdateSketchStatus(COMPLATE, lblComPltSketchStatus);
                comPlateSketchList = comPltSketch.AskListFromTaggedObjects(comPltTaggedObjects);
            }
            else
            {
                isComPltSketchSelected = false;
                UpdateSketchStatus(COMPLATE, lblComPltSketchStatus);
            }

            this.Show();
        }

        private void btnMatPickDim_Click(object sender, EventArgs e)
        {
            this.Hide();
            UsageTrackingService.Instance.LogAction("DIMENSION_PICK_START", "Material thickness dimension pick initiated");

            string result = NXDrawing.GetTextFromDimension();
            txtMatThk.Text = result;

            if (!string.IsNullOrEmpty(result))
            {
                UsageTrackingService.Instance.LogAction("DIMENSION_PICKED", $"Material thickness: {result}");
            }
            else
            {
                UsageTrackingService.Instance.LogAction("DIMENSION_PICK_CANCELLED", "Material thickness pick cancelled");
            }

            this.Show();
        }

        private void btnCWidthPickDim_Click(object sender, EventArgs e)
        {
            this.Hide();
            UsageTrackingService.Instance.LogAction("DIMENSION_PICK_START", "Coil width dimension pick initiated");

            string result = NXDrawing.GetTextFromDimension();
            txtCoilWidth.Text = result;

            if (!string.IsNullOrEmpty(result))
            {
                UsageTrackingService.Instance.LogAction("DIMENSION_PICKED", $"Coil width: {result}");
            }
            else
            {
                UsageTrackingService.Instance.LogAction("DIMENSION_PICK_CANCELLED", "Coil width pick cancelled");
            }

            this.Show();
        }


        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressEvent_NumericalOnly(sender, e);
        }

        private void txtBox_textChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
            UpdateAllCalculations();
        }

        private void MatGuideCoverType_CheckedChange(object sender, EventArgs e)
        {
            UpdateMatGuideCoverStatus();

            string coverageType = radFullCoverage.Checked ? "Full Coverage" : "Partial Coverage";
            UsageTrackingService.Instance.LogAction("COVERAGE_TYPE_CHANGED", coverageType);
        }

        private void UpdateMatGuideCoverStatus()
        {
            if (radFullCoverage.Checked)
            {
                pictureBox1.Image = Properties.Resources.FullCoverGuide;
                MaterialGuideType = MaterialGuideType.FullCoverage;
                PlateThkSettingFullCoverage();
            }
            else if (radPartialCoverage.Checked)
            {
                pictureBox1.Image = Properties.Resources.PartialCoverGuide;
                MaterialGuideType = MaterialGuideType.PartialCoverage;
                PlateThkSettingPartialCoverage();
            }
        }

        private void PlateThkSettingPartialCoverage()
        {
            txtUpperShoeThk.Text = "70.0";
            txtUpperPadThk.Text = "27.0";
            txtPunHolderThk.Text = "30.0";
            txtBottomPltThk.Text = "16.0";
            txtStripperPltThk.Text = "30.0";
            txtMatThk.Text = "1.55";
            txtDiePltThk.Text = "35.0";
            txtLowerPadThk.Text = "25.0";
            txtLowerShoeThk.Text = "70.0";
            txtParallelBarThk.Text = "155.0";
            txtCommonPltThk.Text = "60.0";
        }

        private void PlateThkSettingFullCoverage()
        {
            txtUpperShoeThk.Text = "70.0";
            txtUpperPadThk.Text = "27.0";
            txtPunHolderThk.Text = "30.0";
            txtBottomPltThk.Text = "29.0";
            txtStripperPltThk.Text = "17.0";
            txtMatThk.Text = "1.55";
            txtDiePltThk.Text = "35.0";
            txtLowerPadThk.Text = "25.0";
            txtLowerShoeThk.Text = "70.0";
            txtParallelBarThk.Text = "155.0";
            txtCommonPltThk.Text = "60.0";
        }

        private void cboDesign_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void txtCodePrefix_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
        }

        private void cboDesign_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
        }

        private void txtModel_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
        }

        private void txtPart_TextChanged(object sender, EventArgs e)
        {
            CheckInputAndEnableApply();
        }
    }
}
