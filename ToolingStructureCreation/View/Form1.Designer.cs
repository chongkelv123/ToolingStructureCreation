namespace ToolingStructureCreation.View
{
    partial class formToolStructure
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formToolStructure));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtCommonPltThk = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtParallelBarThk = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtLowerShoeThk = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtLowerPadThk = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtDieHeight = new System.Windows.Forms.TextBox();
            this.txtDiePltThk = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtMatThk = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtStripperPlt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtBottomPltThk = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPunHolderThk = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUpperShoeThk = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtUpperPadThk = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(590, 477);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(117, 35);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Enabled = false;
            this.btnApply.Location = new System.Drawing.Point(713, 477);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(117, 35);
            this.btnApply.TabIndex = 0;
            this.btnApply.Text = "&Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.txtPath);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(819, 76);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Where would you like the Tooling Assembly to be saved to?";
            // 
            // txtPath
            // 
            this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPath.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.HistoryList;
            this.txtPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPath.Location = new System.Drawing.Point(6, 21);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(807, 22);
            this.txtPath.TabIndex = 0;
            this.txtPath.TextChanged += new System.EventHandler(this.txtPath_TextChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox2.Controls.Add(this.txtCommonPltThk);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.txtParallelBarThk);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.txtLowerShoeThk);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.txtLowerPadThk);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtDieHeight);
            this.groupBox2.Controls.Add(this.txtDiePltThk);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.txtMatThk);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtStripperPlt);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtBottomPltThk);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtPunHolderThk);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtUpperShoeThk);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.txtUpperPadThk);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.pictureBox1);
            this.groupBox2.Location = new System.Drawing.Point(12, 94);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(818, 377);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tooling Structure: Assign Plate Thickness";
            // 
            // txtCommonPltThk
            // 
            this.txtCommonPltThk.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCommonPltThk.Location = new System.Drawing.Point(121, 305);
            this.txtCommonPltThk.Name = "txtCommonPltThk";
            this.txtCommonPltThk.Size = new System.Drawing.Size(150, 22);
            this.txtCommonPltThk.TabIndex = 11;
            this.txtCommonPltThk.Text = "60.0";
            this.txtCommonPltThk.TextChanged += new System.EventHandler(this.txtCommonPltThk_TextChanged);
            this.txtCommonPltThk.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCommonPltThk_KeyPress);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 308);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(99, 16);
            this.label11.TabIndex = 1;
            this.label11.Text = "COMMON PLT:";
            // 
            // txtParallelBarThk
            // 
            this.txtParallelBarThk.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtParallelBarThk.Location = new System.Drawing.Point(121, 277);
            this.txtParallelBarThk.Name = "txtParallelBarThk";
            this.txtParallelBarThk.Size = new System.Drawing.Size(150, 22);
            this.txtParallelBarThk.TabIndex = 10;
            this.txtParallelBarThk.Text = "155.0";
            this.txtParallelBarThk.TextChanged += new System.EventHandler(this.txtParallelBarThk_TextChanged);
            this.txtParallelBarThk.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtParallelBarThk_KeyPress);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 280);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(108, 16);
            this.label10.TabIndex = 1;
            this.label10.Text = "PARALLEL BAR:";
            // 
            // txtLowerShoeThk
            // 
            this.txtLowerShoeThk.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLowerShoeThk.Location = new System.Drawing.Point(121, 249);
            this.txtLowerShoeThk.Name = "txtLowerShoeThk";
            this.txtLowerShoeThk.Size = new System.Drawing.Size(150, 22);
            this.txtLowerShoeThk.TabIndex = 9;
            this.txtLowerShoeThk.Text = "70.0";
            this.txtLowerShoeThk.TextChanged += new System.EventHandler(this.txtLowerShoeThk_TextChanged);
            this.txtLowerShoeThk.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtLowerShoeThk_KeyPress);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 252);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(100, 16);
            this.label9.TabIndex = 1;
            this.label9.Text = "LOWER SHOE:";
            // 
            // txtLowerPadThk
            // 
            this.txtLowerPadThk.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLowerPadThk.Location = new System.Drawing.Point(121, 221);
            this.txtLowerPadThk.Name = "txtLowerPadThk";
            this.txtLowerPadThk.Size = new System.Drawing.Size(150, 22);
            this.txtLowerPadThk.TabIndex = 8;
            this.txtLowerPadThk.Text = "25.0";
            this.txtLowerPadThk.TextChanged += new System.EventHandler(this.txtLowerPadThk_TextChanged);
            this.txtLowerPadThk.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtLowerPadThk_KeyPress);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 224);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 16);
            this.label7.TabIndex = 1;
            this.label7.Text = "LOWER PAD:";
            // 
            // txtDieHeight
            // 
            this.txtDieHeight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDieHeight.Enabled = false;
            this.txtDieHeight.Location = new System.Drawing.Point(315, 41);
            this.txtDieHeight.Name = "txtDieHeight";
            this.txtDieHeight.Size = new System.Drawing.Size(150, 22);
            this.txtDieHeight.TabIndex = 7;
            // 
            // txtDiePltThk
            // 
            this.txtDiePltThk.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDiePltThk.Location = new System.Drawing.Point(121, 193);
            this.txtDiePltThk.Name = "txtDiePltThk";
            this.txtDiePltThk.Size = new System.Drawing.Size(150, 22);
            this.txtDiePltThk.TabIndex = 7;
            this.txtDiePltThk.Text = "35.0";
            this.txtDiePltThk.TextChanged += new System.EventHandler(this.txtDiePltThk_TextChanged);
            this.txtDiePltThk.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDiePltThk_KeyPress);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(312, 22);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(86, 16);
            this.label12.TabIndex = 1;
            this.label12.Text = "DIE HEIGHT:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 196);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 16);
            this.label6.TabIndex = 1;
            this.label6.Text = "DIE PLATE:";
            // 
            // txtMatThk
            // 
            this.txtMatThk.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMatThk.Location = new System.Drawing.Point(121, 165);
            this.txtMatThk.Name = "txtMatThk";
            this.txtMatThk.Size = new System.Drawing.Size(150, 22);
            this.txtMatThk.TabIndex = 6;
            this.txtMatThk.Text = "1.55";
            this.txtMatThk.TextChanged += new System.EventHandler(this.txtMatThk_TextChanged);
            this.txtMatThk.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMatThk_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 168);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 16);
            this.label5.TabIndex = 1;
            this.label5.Text = "MAT THK:";
            // 
            // txtStripperPlt
            // 
            this.txtStripperPlt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtStripperPlt.Location = new System.Drawing.Point(121, 137);
            this.txtStripperPlt.Name = "txtStripperPlt";
            this.txtStripperPlt.Size = new System.Drawing.Size(150, 22);
            this.txtStripperPlt.TabIndex = 5;
            this.txtStripperPlt.Text = "30.0";
            this.txtStripperPlt.TextChanged += new System.EventHandler(this.txtStripperPlt_TextChanged);
            this.txtStripperPlt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtStripperPlt_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 140);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 16);
            this.label4.TabIndex = 1;
            this.label4.Text = "STRIPPER PLT:";
            // 
            // txtBottomPltThk
            // 
            this.txtBottomPltThk.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBottomPltThk.Location = new System.Drawing.Point(121, 109);
            this.txtBottomPltThk.Name = "txtBottomPltThk";
            this.txtBottomPltThk.Size = new System.Drawing.Size(150, 22);
            this.txtBottomPltThk.TabIndex = 4;
            this.txtBottomPltThk.Text = "16.0";
            this.txtBottomPltThk.TextChanged += new System.EventHandler(this.txtBottomPltThk_TextChanged);
            this.txtBottomPltThk.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBottomPltThk_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 16);
            this.label3.TabIndex = 1;
            this.label3.Text = "BOTTOM PLT:";
            // 
            // txtPunHolderThk
            // 
            this.txtPunHolderThk.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPunHolderThk.Location = new System.Drawing.Point(121, 81);
            this.txtPunHolderThk.Name = "txtPunHolderThk";
            this.txtPunHolderThk.Size = new System.Drawing.Size(150, 22);
            this.txtPunHolderThk.TabIndex = 3;
            this.txtPunHolderThk.Text = "30.0";
            this.txtPunHolderThk.TextChanged += new System.EventHandler(this.txtPunHolderThk_TextChanged);
            this.txtPunHolderThk.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPunHolderThk_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "PUN HOLDER:";
            // 
            // txtUpperShoeThk
            // 
            this.txtUpperShoeThk.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUpperShoeThk.Location = new System.Drawing.Point(121, 25);
            this.txtUpperShoeThk.Name = "txtUpperShoeThk";
            this.txtUpperShoeThk.Size = new System.Drawing.Size(150, 22);
            this.txtUpperShoeThk.TabIndex = 1;
            this.txtUpperShoeThk.Text = "70.0";
            this.txtUpperShoeThk.TextChanged += new System.EventHandler(this.txtUpperShoeThk_TextChanged);
            this.txtUpperShoeThk.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtUpperShoeThk_KeyPress);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 28);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(98, 16);
            this.label8.TabIndex = 1;
            this.label8.Text = "UPPER SHOE:";
            // 
            // txtUpperPadThk
            // 
            this.txtUpperPadThk.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUpperPadThk.Location = new System.Drawing.Point(121, 53);
            this.txtUpperPadThk.Name = "txtUpperPadThk";
            this.txtUpperPadThk.Size = new System.Drawing.Size(150, 22);
            this.txtUpperPadThk.TabIndex = 2;
            this.txtUpperPadThk.Text = "27.0";
            this.txtUpperPadThk.TextChanged += new System.EventHandler(this.txtUpperPadThk_TextChanged);
            this.txtUpperPadThk.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtUpperPadThk_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "UPPER PAD:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(559, 21);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(253, 316);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // formToolStructure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(843, 524);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnCancel);
            this.Name = "formToolStructure";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create Tool Structure Form";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox txtLowerPadThk;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtDiePltThk;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtMatThk;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtStripperPlt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtBottomPltThk;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPunHolderThk;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUpperPadThk;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtParallelBarThk;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtLowerShoeThk;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtUpperShoeThk;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtCommonPltThk;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtDieHeight;
        private System.Windows.Forms.Label label12;
    }
}