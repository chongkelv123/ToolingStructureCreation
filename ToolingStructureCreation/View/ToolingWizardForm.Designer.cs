namespace ToolingStructureCreation.View
{
    partial class ToolingWizardForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.btnCreateToolStructure = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlMainContent = new System.Windows.Forms.Panel();
            this.grpShoeSketch = new System.Windows.Forms.GroupBox();
            this.lblShoeSketchStatus = new System.Windows.Forms.Label();
            this.btnSelectShoeSketch = new System.Windows.Forms.Button();
            this.lblShoeSketchInstructions = new System.Windows.Forms.Label();
            this.grpPlateSketch = new System.Windows.Forms.GroupBox();
            this.lblPlateSketchStatus = new System.Windows.Forms.Label();
            this.btnSelectPlateSketch = new System.Windows.Forms.Button();
            this.lblPlateSketchInstructions = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            this.pnlMainContent.SuspendLayout();
            this.grpShoeSketch.SuspendLayout();
            this.grpPlateSketch.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.RoyalBlue;
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(650, 60);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(404, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Tooling Structure Creation Wizard";
            // 
            // pnlFooter
            // 
            this.pnlFooter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFooter.Controls.Add(this.btnCreateToolStructure);
            this.pnlFooter.Controls.Add(this.btnCancel);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 350);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(650, 50);
            this.pnlFooter.TabIndex = 1;
            // 
            // btnCreateToolStructure
            // 
            this.btnCreateToolStructure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateToolStructure.Enabled = false;
            this.btnCreateToolStructure.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateToolStructure.Location = new System.Drawing.Point(488, 10);
            this.btnCreateToolStructure.Name = "btnCreateToolStructure";
            this.btnCreateToolStructure.Size = new System.Drawing.Size(150, 30);
            this.btnCreateToolStructure.TabIndex = 1;
            this.btnCreateToolStructure.Text = "Create Tool Structure";
            this.btnCreateToolStructure.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(382, 10);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // pnlMainContent
            // 
            this.pnlMainContent.Controls.Add(this.grpShoeSketch);
            this.pnlMainContent.Controls.Add(this.grpPlateSketch);
            this.pnlMainContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMainContent.Location = new System.Drawing.Point(0, 60);
            this.pnlMainContent.Name = "pnlMainContent";
            this.pnlMainContent.Padding = new System.Windows.Forms.Padding(20);
            this.pnlMainContent.Size = new System.Drawing.Size(650, 290);
            this.pnlMainContent.TabIndex = 2;
            // 
            // grpShoeSketch
            // 
            this.grpShoeSketch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpShoeSketch.Controls.Add(this.lblShoeSketchStatus);
            this.grpShoeSketch.Controls.Add(this.btnSelectShoeSketch);
            this.grpShoeSketch.Controls.Add(this.lblShoeSketchInstructions);
            this.grpShoeSketch.Location = new System.Drawing.Point(20, 150);
            this.grpShoeSketch.Name = "grpShoeSketch";
            this.grpShoeSketch.Size = new System.Drawing.Size(604, 100);
            this.grpShoeSketch.TabIndex = 1;
            this.grpShoeSketch.TabStop = false;
            this.grpShoeSketch.Text = "Select Shoe Sketch";
            // 
            // lblShoeSketchStatus
            // 
            this.lblShoeSketchStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblShoeSketchStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblShoeSketchStatus.ForeColor = System.Drawing.Color.Red;
            this.lblShoeSketchStatus.Location = new System.Drawing.Point(150, 67);
            this.lblShoeSketchStatus.Name = "lblShoeSketchStatus";
            this.lblShoeSketchStatus.Size = new System.Drawing.Size(434, 20);
            this.lblShoeSketchStatus.TabIndex = 2;
            this.lblShoeSketchStatus.Text = "No sketch selected";
            // 
            // btnSelectShoeSketch
            // 
            this.btnSelectShoeSketch.Location = new System.Drawing.Point(20, 62);
            this.btnSelectShoeSketch.Name = "btnSelectShoeSketch";
            this.btnSelectShoeSketch.Size = new System.Drawing.Size(120, 30);
            this.btnSelectShoeSketch.TabIndex = 1;
            this.btnSelectShoeSketch.Text = "Select Sketch...";
            this.btnSelectShoeSketch.UseVisualStyleBackColor = true;
            // 
            // lblShoeSketchInstructions
            // 
            this.lblShoeSketchInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblShoeSketchInstructions.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblShoeSketchInstructions.Location = new System.Drawing.Point(20, 22);
            this.lblShoeSketchInstructions.Name = "lblShoeSketchInstructions";
            this.lblShoeSketchInstructions.Size = new System.Drawing.Size(564, 20);
            this.lblShoeSketchInstructions.TabIndex = 0;
            this.lblShoeSketchInstructions.Text = "Select a shoe sketch from the NX model.";
            // 
            // grpPlateSketch
            // 
            this.grpPlateSketch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPlateSketch.Controls.Add(this.lblPlateSketchStatus);
            this.grpPlateSketch.Controls.Add(this.btnSelectPlateSketch);
            this.grpPlateSketch.Controls.Add(this.lblPlateSketchInstructions);
            this.grpPlateSketch.Location = new System.Drawing.Point(20, 20);
            this.grpPlateSketch.Name = "grpPlateSketch";
            this.grpPlateSketch.Size = new System.Drawing.Size(604, 100);
            this.grpPlateSketch.TabIndex = 0;
            this.grpPlateSketch.TabStop = false;
            this.grpPlateSketch.Text = "Select Plate Sketch";
            // 
            // lblPlateSketchStatus
            // 
            this.lblPlateSketchStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPlateSketchStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlateSketchStatus.ForeColor = System.Drawing.Color.Red;
            this.lblPlateSketchStatus.Location = new System.Drawing.Point(150, 67);
            this.lblPlateSketchStatus.Name = "lblPlateSketchStatus";
            this.lblPlateSketchStatus.Size = new System.Drawing.Size(434, 20);
            this.lblPlateSketchStatus.TabIndex = 2;
            this.lblPlateSketchStatus.Text = "No sketch selected";
            // 
            // btnSelectPlateSketch
            // 
            this.btnSelectPlateSketch.Location = new System.Drawing.Point(20, 62);
            this.btnSelectPlateSketch.Name = "btnSelectPlateSketch";
            this.btnSelectPlateSketch.Size = new System.Drawing.Size(120, 30);
            this.btnSelectPlateSketch.TabIndex = 1;
            this.btnSelectPlateSketch.Text = "Select Sketch...";
            this.btnSelectPlateSketch.UseVisualStyleBackColor = true;
            // 
            // lblPlateSketchInstructions
            // 
            this.lblPlateSketchInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPlateSketchInstructions.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlateSketchInstructions.Location = new System.Drawing.Point(20, 22);
            this.lblPlateSketchInstructions.Name = "lblPlateSketchInstructions";
            this.lblPlateSketchInstructions.Size = new System.Drawing.Size(564, 20);
            this.lblPlateSketchInstructions.TabIndex = 0;
            this.lblPlateSketchInstructions.Text = "Select a plate sketch from the NX model.";
            // 
            // ToolingWizardForm
            // 
            this.AcceptButton = this.btnCreateToolStructure;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(650, 400);
            this.Controls.Add(this.pnlMainContent);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "ToolingWizardForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tooling Structure Creation Wizard";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlFooter.ResumeLayout(false);
            this.pnlMainContent.ResumeLayout(false);
            this.grpShoeSketch.ResumeLayout(false);
            this.grpPlateSketch.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Button btnCreateToolStructure;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel pnlMainContent;
        private System.Windows.Forms.GroupBox grpPlateSketch;
        private System.Windows.Forms.Label lblPlateSketchInstructions;
        private System.Windows.Forms.Button btnSelectPlateSketch;
        private System.Windows.Forms.Label lblPlateSketchStatus;
        private System.Windows.Forms.GroupBox grpShoeSketch;
        private System.Windows.Forms.Label lblShoeSketchStatus;
        private System.Windows.Forms.Button btnSelectShoeSketch;
        private System.Windows.Forms.Label lblShoeSketchInstructions;
    }
}