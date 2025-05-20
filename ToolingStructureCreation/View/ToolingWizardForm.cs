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
using ToolingStructureCreation.Interfaces;
using ToolingStructureCreation.Model;

namespace ToolingStructureCreation.View
{
    public partial class ToolingWizardForm : Form
    {
        private readonly IController _controller;
        private readonly INXSelectionService _selectionService;

        public ToolingParameters Parameters { get; private set; }

        public ToolingWizardForm(IController controller, INXSelectionService selectionService)
        {
            InitializeComponent();

            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _selectionService = selectionService ?? throw new ArgumentNullException(nameof(selectionService));

            Parameters = new ToolingParameters();

            // Initialize event handlers
            SetupEventHandlers();
        }

        private void SetupEventHandlers()
        {
            // Button click events
            btnSelectPlateSketch.Click += BtnSelectPlateSketch_Click;
            btnSelectShoeSketch.Click += BtnSelectShoeSketch_Click;
            btnCreateToolStructure.Click += BtnCreateToolStructure_Click;
            btnCancel.Click += BtnCancel_Click;

            // Form loading event
            this.Load += ToolingWizardForm_Load;
        }

        private void ToolingWizardForm_Load(object sender, EventArgs e)
        {
            // Initialize form state
            UpdateCreateButtonStatus();
        }

        private void BtnSelectPlateSketch_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Debugger.Launch();
            try
            {
                var component = _selectionService.SelectComponent(ComponentType.PlateSketch);
                if (component != null)
                {
                    Parameters.BaseComponent = component;
                    Parameters.BaseComponentType = ComponentType.PlateSketch;
                    UpdatePlateSketchStatus(true);

                    // Disable shoe sketch if plate sketch is selected
                    btnSelectShoeSketch.Enabled = false;

                    UpdateCreateButtonStatus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting plate sketch: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSelectShoeSketch_Click(object sender, EventArgs e)
        {
            try
            {
                var component = _selectionService.SelectComponent(ComponentType.ShoeSketch);
                if (component != null)
                {
                    Parameters.BaseComponent = component;
                    Parameters.BaseComponentType = ComponentType.ShoeSketch;
                    UpdateShoeSketchStatus(true);
                    UpdateCreateButtonStatus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting shoe sketch: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCreateToolStructure_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                try
                {
                    _controller.Start(Parameters);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating tooling structure: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void UpdatePlateSketchStatus(bool selected)
        {
            if (selected)
            {
                lblPlateSketchStatus.Text = "Plate sketch selected";
                lblPlateSketchStatus.ForeColor = Color.Green;
            }
            else
            {
                lblPlateSketchStatus.Text = "No sketch selected";
                lblPlateSketchStatus.ForeColor = Color.Red;
            }
        }

        private void UpdateShoeSketchStatus(bool selected)
        {
            if (selected)
            {
                lblShoeSketchStatus.Text = "Shoe sketch selected";
                lblShoeSketchStatus.ForeColor = Color.Green;
            }
            else
            {
                lblShoeSketchStatus.Text = "No sketch selected";
                lblShoeSketchStatus.ForeColor = Color.Red;
            }
        }

        private void UpdateCreateButtonStatus()
        {
            btnCreateToolStructure.Enabled = Parameters.BaseComponent != null;
        }

        private bool ValidateInputs()
        {
            if (Parameters.BaseComponent == null)
            {
                MessageBox.Show("Please select a base component first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Add more validation as needed

            return true;
        }
    }
}
