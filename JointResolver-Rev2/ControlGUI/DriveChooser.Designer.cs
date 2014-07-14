﻿
partial class DriveChooser
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
            this.cmbJointDriver = new System.Windows.Forms.ComboBox();
            this.grpChooseDriver = new System.Windows.Forms.GroupBox();
            this.grpDriveOptions = new System.Windows.Forms.GroupBox();
            this.txtHighLimit = new System.Windows.Forms.NumericUpDown();
            this.lblLimits = new System.Windows.Forms.Label();
            this.txtLowLimit = new System.Windows.Forms.NumericUpDown();
            this.txtPortB = new System.Windows.Forms.NumericUpDown();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtPortA = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbWheelType = new System.Windows.Forms.ComboBox();
            this.grpChooseDriver.SuspendLayout();
            this.grpDriveOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtHighLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLowLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPortB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPortA)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbJointDriver
            // 
            this.cmbJointDriver.FormattingEnabled = true;
            this.cmbJointDriver.Location = new System.Drawing.Point(15, 21);
            this.cmbJointDriver.Name = "cmbJointDriver";
            this.cmbJointDriver.Size = new System.Drawing.Size(317, 24);
            this.cmbJointDriver.TabIndex = 0;
            this.cmbJointDriver.SelectedIndexChanged += new System.EventHandler(this.cmbJointDriver_SelectedIndexChanged);
            // 
            // grpChooseDriver
            // 
            this.grpChooseDriver.Controls.Add(this.cmbJointDriver);
            this.grpChooseDriver.Location = new System.Drawing.Point(12, 12);
            this.grpChooseDriver.Name = "grpChooseDriver";
            this.grpChooseDriver.Size = new System.Drawing.Size(338, 56);
            this.grpChooseDriver.TabIndex = 1;
            this.grpChooseDriver.TabStop = false;
            this.grpChooseDriver.Text = "Joint Driver";
            // 
            // grpDriveOptions
            // 
            this.grpDriveOptions.Controls.Add(this.txtHighLimit);
            this.grpDriveOptions.Controls.Add(this.lblLimits);
            this.grpDriveOptions.Controls.Add(this.txtLowLimit);
            this.grpDriveOptions.Controls.Add(this.txtPortB);
            this.grpDriveOptions.Controls.Add(this.lblPort);
            this.grpDriveOptions.Controls.Add(this.txtPortA);
            this.grpDriveOptions.Location = new System.Drawing.Point(13, 75);
            this.grpDriveOptions.Name = "grpDriveOptions";
            this.grpDriveOptions.Size = new System.Drawing.Size(337, 128);
            this.grpDriveOptions.TabIndex = 2;
            this.grpDriveOptions.TabStop = false;
            this.grpDriveOptions.Text = "Joint Options";
            this.grpDriveOptions.Enter += new System.EventHandler(this.grpDriveOptions_Enter);
            // 
            // txtHighLimit
            // 
            this.txtHighLimit.DecimalPlaces = 4;
            this.txtHighLimit.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.txtHighLimit.Location = new System.Drawing.Point(140, 92);
            this.txtHighLimit.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.txtHighLimit.Name = "txtHighLimit";
            this.txtHighLimit.Size = new System.Drawing.Size(120, 22);
            this.txtHighLimit.TabIndex = 5;
            // 
            // lblLimits
            // 
            this.lblLimits.AutoSize = true;
            this.lblLimits.Location = new System.Drawing.Point(11, 72);
            this.lblLimits.Name = "lblLimits";
            this.lblLimits.Size = new System.Drawing.Size(78, 17);
            this.lblLimits.TabIndex = 4;
            this.lblLimits.Text = "Joint Limits";
            // 
            // txtLowLimit
            // 
            this.txtLowLimit.DecimalPlaces = 4;
            this.txtLowLimit.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.txtLowLimit.Location = new System.Drawing.Point(14, 92);
            this.txtLowLimit.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.txtLowLimit.Name = "txtLowLimit";
            this.txtLowLimit.Size = new System.Drawing.Size(120, 22);
            this.txtLowLimit.TabIndex = 3;
            this.txtLowLimit.ValueChanged += new System.EventHandler(this.txtLowLimit_ValueChanged);
            // 
            // txtPortB
            // 
            this.txtPortB.Location = new System.Drawing.Point(140, 42);
            this.txtPortB.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtPortB.Name = "txtPortB";
            this.txtPortB.Size = new System.Drawing.Size(120, 22);
            this.txtPortB.TabIndex = 2;
            this.txtPortB.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtPortB.ValueChanged += new System.EventHandler(this.txtPortB_ValueChanged);
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(11, 22);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(51, 17);
            this.lblPort.TabIndex = 1;
            this.lblPort.Text = "Port ID";
            this.lblPort.Click += new System.EventHandler(this.lblPort_Click);
            // 
            // txtPortA
            // 
            this.txtPortA.Location = new System.Drawing.Point(14, 42);
            this.txtPortA.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtPortA.Name = "txtPortA";
            this.txtPortA.Size = new System.Drawing.Size(120, 22);
            this.txtPortA.TabIndex = 0;
            this.txtPortA.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtPortA.ValueChanged += new System.EventHandler(this.txtPortA_ValueChanged);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(11, 278);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(338, 28);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbWheelType);
            this.groupBox2.Location = new System.Drawing.Point(11, 209);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(339, 63);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Wheel Type";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // cmbWheelType
            // 
            this.cmbWheelType.FormattingEnabled = true;
            this.cmbWheelType.Items.AddRange(new object[] {
            "Normal",
            "Omni",
            "Mecanum"});
            this.cmbWheelType.Location = new System.Drawing.Point(16, 21);
            this.cmbWheelType.Name = "cmbWheelType";
            this.cmbWheelType.Size = new System.Drawing.Size(317, 24);
            this.cmbWheelType.TabIndex = 0;
            this.cmbWheelType.SelectedIndexChanged += new System.EventHandler(this.cmbWheelType_SelectedIndexChanged);
            // 
            // DriveChooser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 314);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpDriveOptions);
            this.Controls.Add(this.grpChooseDriver);
            this.Name = "DriveChooser";
            this.Text = "Configure Joint";
            this.grpChooseDriver.ResumeLayout(false);
            this.grpDriveOptions.ResumeLayout(false);
            this.grpDriveOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtHighLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLowLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPortB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPortA)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ComboBox cmbJointDriver;
    private System.Windows.Forms.GroupBox grpChooseDriver;
    private System.Windows.Forms.GroupBox grpDriveOptions;
    private System.Windows.Forms.NumericUpDown txtHighLimit;
    private System.Windows.Forms.Label lblLimits;
    private System.Windows.Forms.NumericUpDown txtLowLimit;
    private System.Windows.Forms.NumericUpDown txtPortB;
    private System.Windows.Forms.Label lblPort;
    private System.Windows.Forms.NumericUpDown txtPortA;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.ComboBox cmbWheelType;
}