using Support.UI;

namespace KeyCap.Forms
{
    public partial class KeyCaptureConfig : AbstractDirtyForm
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
            this.components = new System.ComponentModel.Container();
            this.txtKeyIn = new System.Windows.Forms.TextBox();
            this.txtKeyOut = new System.Windows.Forms.TextBox();
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previousConfigurationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listViewKeys = new System.Windows.Forms.ListView();
            this.columnHeaderInput = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderOutput = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnAppend = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.checkInputAlt = new System.Windows.Forms.CheckBox();
            this.checkInputControl = new System.Windows.Forms.CheckBox();
            this.checkInputShift = new System.Windows.Forms.CheckBox();
            this.checkOutputShift = new System.Windows.Forms.CheckBox();
            this.checkOutputControl = new System.Windows.Forms.CheckBox();
            this.checkOutputAlt = new System.Windows.Forms.CheckBox();
            this.checkOutputNone = new System.Windows.Forms.CheckBox();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStripNotify = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toggleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelKeySetup = new System.Windows.Forms.Panel();
            this.groupBoxOutputKey = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownDelay = new System.Windows.Forms.NumericUpDown();
            this.comboBoxMouseOut = new System.Windows.Forms.ComboBox();
            this.groupBoxInputKey = new System.Windows.Forms.GroupBox();
            this.panelKeySetupControls = new System.Windows.Forms.Panel();
            this.menuStripMain.SuspendLayout();
            this.contextMenuStripNotify.SuspendLayout();
            this.panelKeySetup.SuspendLayout();
            this.groupBoxOutputKey.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDelay)).BeginInit();
            this.groupBoxInputKey.SuspendLayout();
            this.panelKeySetupControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtKeyIn
            // 
            this.txtKeyIn.AcceptsTab = true;
            this.txtKeyIn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtKeyIn.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtKeyIn.Location = new System.Drawing.Point(6, 19);
            this.txtKeyIn.Multiline = true;
            this.txtKeyIn.Name = "txtKeyIn";
            this.txtKeyIn.ReadOnly = true;
            this.txtKeyIn.Size = new System.Drawing.Size(495, 20);
            this.txtKeyIn.TabIndex = 0;
            this.txtKeyIn.Enter += new System.EventHandler(this.txtKey_Enter);
            this.txtKeyIn.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            this.txtKeyIn.Leave += new System.EventHandler(this.txtKey_Leave);
            // 
            // txtKeyOut
            // 
            this.txtKeyOut.AcceptsTab = true;
            this.txtKeyOut.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtKeyOut.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtKeyOut.Location = new System.Drawing.Point(6, 19);
            this.txtKeyOut.Multiline = true;
            this.txtKeyOut.Name = "txtKeyOut";
            this.txtKeyOut.ReadOnly = true;
            this.txtKeyOut.Size = new System.Drawing.Size(495, 20);
            this.txtKeyOut.TabIndex = 1;
            this.txtKeyOut.Enter += new System.EventHandler(this.txtKey_Enter);
            this.txtKeyOut.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            this.txtKeyOut.Leave += new System.EventHandler(this.txtKey_Leave);
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.previousConfigurationsToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStripMain.Size = new System.Drawing.Size(875, 24);
            this.menuStripMain.TabIndex = 4;
            this.menuStripMain.Text = "menuStripMain";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.exitMainToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loadToolStripMenuItem.Text = "&Open...";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // exitMainToolStripMenuItem
            // 
            this.exitMainToolStripMenuItem.Name = "exitMainToolStripMenuItem";
            this.exitMainToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitMainToolStripMenuItem.Text = "&Exit";
            this.exitMainToolStripMenuItem.Click += new System.EventHandler(this.exitMainToolStripMenuItem_Click);
            // 
            // previousConfigurationsToolStripMenuItem
            // 
            this.previousConfigurationsToolStripMenuItem.Name = "previousConfigurationsToolStripMenuItem";
            this.previousConfigurationsToolStripMenuItem.Size = new System.Drawing.Size(126, 20);
            this.previousConfigurationsToolStripMenuItem.Text = "Recent Configurations";
            this.previousConfigurationsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.previousConfigurationsToolStripMenuItem_DropDownOpening);
            // 
            // listViewKeys
            // 
            this.listViewKeys.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewKeys.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderInput,
            this.columnHeaderOutput});
            this.listViewKeys.FullRowSelect = true;
            this.listViewKeys.GridLines = true;
            this.listViewKeys.HideSelection = false;
            this.listViewKeys.Location = new System.Drawing.Point(12, 138);
            this.listViewKeys.Name = "listViewKeys";
            this.listViewKeys.Size = new System.Drawing.Size(851, 325);
            this.listViewKeys.TabIndex = 5;
            this.listViewKeys.UseCompatibleStateImageBehavior = false;
            this.listViewKeys.View = System.Windows.Forms.View.Details;
            this.listViewKeys.SelectedIndexChanged += new System.EventHandler(this.listViewKeys_SelectedIndexChanged);
            this.listViewKeys.Resize += new System.EventHandler(this.listViewKeys_Resize);
            // 
            // columnHeaderInput
            // 
            this.columnHeaderInput.Text = "Input";
            this.columnHeaderInput.Width = 239;
            // 
            // columnHeaderOutput
            // 
            this.columnHeaderOutput.Text = "Output";
            this.columnHeaderOutput.Width = 481;
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.Location = new System.Drawing.Point(175, 0);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 6;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(13, 0);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 7;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnAppend
            // 
            this.btnAppend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAppend.Enabled = false;
            this.btnAppend.Location = new System.Drawing.Point(94, 0);
            this.btnAppend.Name = "btnAppend";
            this.btnAppend.Size = new System.Drawing.Size(75, 23);
            this.btnAppend.TabIndex = 8;
            this.btnAppend.Text = "Append";
            this.btnAppend.UseVisualStyleBackColor = true;
            this.btnAppend.Click += new System.EventHandler(this.btnAppend_Click);
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStart.Location = new System.Drawing.Point(12, 499);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(78, 23);
            this.btnStart.TabIndex = 9;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // checkInputAlt
            // 
            this.checkInputAlt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkInputAlt.Location = new System.Drawing.Point(507, 19);
            this.checkInputAlt.Name = "checkInputAlt";
            this.checkInputAlt.Size = new System.Drawing.Size(60, 20);
            this.checkInputAlt.TabIndex = 10;
            this.checkInputAlt.Text = "Alt";
            this.checkInputAlt.UseVisualStyleBackColor = true;
            // 
            // checkInputControl
            // 
            this.checkInputControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkInputControl.Location = new System.Drawing.Point(573, 19);
            this.checkInputControl.Name = "checkInputControl";
            this.checkInputControl.Size = new System.Drawing.Size(60, 20);
            this.checkInputControl.TabIndex = 11;
            this.checkInputControl.Text = "Control";
            this.checkInputControl.UseVisualStyleBackColor = true;
            // 
            // checkInputShift
            // 
            this.checkInputShift.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkInputShift.Location = new System.Drawing.Point(639, 19);
            this.checkInputShift.Name = "checkInputShift";
            this.checkInputShift.Size = new System.Drawing.Size(60, 20);
            this.checkInputShift.TabIndex = 12;
            this.checkInputShift.Text = "Shift";
            this.checkInputShift.UseVisualStyleBackColor = true;
            // 
            // checkOutputShift
            // 
            this.checkOutputShift.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkOutputShift.Location = new System.Drawing.Point(639, 19);
            this.checkOutputShift.Name = "checkOutputShift";
            this.checkOutputShift.Size = new System.Drawing.Size(60, 20);
            this.checkOutputShift.TabIndex = 15;
            this.checkOutputShift.Text = "Shift";
            this.checkOutputShift.UseVisualStyleBackColor = true;
            // 
            // checkOutputControl
            // 
            this.checkOutputControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkOutputControl.Location = new System.Drawing.Point(573, 19);
            this.checkOutputControl.Name = "checkOutputControl";
            this.checkOutputControl.Size = new System.Drawing.Size(60, 20);
            this.checkOutputControl.TabIndex = 14;
            this.checkOutputControl.Text = "Control";
            this.checkOutputControl.UseVisualStyleBackColor = true;
            // 
            // checkOutputAlt
            // 
            this.checkOutputAlt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkOutputAlt.Location = new System.Drawing.Point(507, 18);
            this.checkOutputAlt.Name = "checkOutputAlt";
            this.checkOutputAlt.Size = new System.Drawing.Size(60, 20);
            this.checkOutputAlt.TabIndex = 13;
            this.checkOutputAlt.Text = "Alt";
            this.checkOutputAlt.UseVisualStyleBackColor = true;
            // 
            // checkOutputNone
            // 
            this.checkOutputNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkOutputNone.Location = new System.Drawing.Point(507, 44);
            this.checkOutputNone.Name = "checkOutputNone";
            this.checkOutputNone.Size = new System.Drawing.Size(188, 20);
            this.checkOutputNone.TabIndex = 16;
            this.checkOutputNone.Text = "Do Nothing";
            this.checkOutputNone.UseVisualStyleBackColor = true;
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStripNotify;
            this.notifyIcon.Text = "Key2Key";
            this.notifyIcon.Visible = true;
            this.notifyIcon.DoubleClick += new System.EventHandler(this.restoreConfigurationToolStripMenuItem_Click);
            // 
            // contextMenuStripNotify
            // 
            this.contextMenuStripNotify.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toggleToolStripMenuItem,
            this.restoreConfigurationToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.contextMenuStripNotify.Name = "contextMenuStripNotify";
            this.contextMenuStripNotify.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenuStripNotify.ShowImageMargin = false;
            this.contextMenuStripNotify.Size = new System.Drawing.Size(156, 76);
            // 
            // toggleToolStripMenuItem
            // 
            this.toggleToolStripMenuItem.Name = "toggleToolStripMenuItem";
            this.toggleToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.toggleToolStripMenuItem.Text = "Toggle";
            this.toggleToolStripMenuItem.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // restoreConfigurationToolStripMenuItem
            // 
            this.restoreConfigurationToolStripMenuItem.Name = "restoreConfigurationToolStripMenuItem";
            this.restoreConfigurationToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.restoreConfigurationToolStripMenuItem.Text = "Restore Configuration";
            this.restoreConfigurationToolStripMenuItem.Click += new System.EventHandler(this.restoreConfigurationToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(152, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitMainToolStripMenuItem_Click);
            // 
            // panelKeySetup
            // 
            this.panelKeySetup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelKeySetup.Controls.Add(this.groupBoxOutputKey);
            this.panelKeySetup.Controls.Add(this.groupBoxInputKey);
            this.panelKeySetup.Controls.Add(this.listViewKeys);
            this.panelKeySetup.Location = new System.Drawing.Point(0, 27);
            this.panelKeySetup.Name = "panelKeySetup";
            this.panelKeySetup.Size = new System.Drawing.Size(875, 466);
            this.panelKeySetup.TabIndex = 17;
            // 
            // groupBoxOutputKey
            // 
            this.groupBoxOutputKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxOutputKey.Controls.Add(this.label1);
            this.groupBoxOutputKey.Controls.Add(this.numericUpDownDelay);
            this.groupBoxOutputKey.Controls.Add(this.txtKeyOut);
            this.groupBoxOutputKey.Controls.Add(this.checkOutputNone);
            this.groupBoxOutputKey.Controls.Add(this.comboBoxMouseOut);
            this.groupBoxOutputKey.Controls.Add(this.checkOutputControl);
            this.groupBoxOutputKey.Controls.Add(this.checkOutputAlt);
            this.groupBoxOutputKey.Controls.Add(this.checkOutputShift);
            this.groupBoxOutputKey.Location = new System.Drawing.Point(12, 61);
            this.groupBoxOutputKey.Name = "groupBoxOutputKey";
            this.groupBoxOutputKey.Size = new System.Drawing.Size(851, 71);
            this.groupBoxOutputKey.TabIndex = 19;
            this.groupBoxOutputKey.TabStop = false;
            this.groupBoxOutputKey.Text = "Output Key";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(705, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 20);
            this.label1.TabIndex = 19;
            this.label1.Text = "Delay (sec):";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numericUpDownDelay
            // 
            this.numericUpDownDelay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownDelay.Location = new System.Drawing.Point(784, 45);
            this.numericUpDownDelay.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDownDelay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownDelay.Name = "numericUpDownDelay";
            this.numericUpDownDelay.Size = new System.Drawing.Size(61, 20);
            this.numericUpDownDelay.TabIndex = 18;
            this.numericUpDownDelay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownDelay.ValueChanged += new System.EventHandler(this.numericUpDownDelay_ValueChanged);
            // 
            // comboBoxMouseOut
            // 
            this.comboBoxMouseOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxMouseOut.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMouseOut.FormattingEnabled = true;
            this.comboBoxMouseOut.Location = new System.Drawing.Point(705, 18);
            this.comboBoxMouseOut.Name = "comboBoxMouseOut";
            this.comboBoxMouseOut.Size = new System.Drawing.Size(140, 21);
            this.comboBoxMouseOut.TabIndex = 17;
            this.comboBoxMouseOut.SelectedIndexChanged += new System.EventHandler(this.comboBoxSpecialOut_SelectedIndexChanged);
            // 
            // groupBoxInputKey
            // 
            this.groupBoxInputKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxInputKey.Controls.Add(this.txtKeyIn);
            this.groupBoxInputKey.Controls.Add(this.checkInputAlt);
            this.groupBoxInputKey.Controls.Add(this.checkInputControl);
            this.groupBoxInputKey.Controls.Add(this.checkInputShift);
            this.groupBoxInputKey.Location = new System.Drawing.Point(12, 3);
            this.groupBoxInputKey.Name = "groupBoxInputKey";
            this.groupBoxInputKey.Size = new System.Drawing.Size(851, 52);
            this.groupBoxInputKey.TabIndex = 18;
            this.groupBoxInputKey.TabStop = false;
            this.groupBoxInputKey.Text = "Input Key";
            // 
            // panelKeySetupControls
            // 
            this.panelKeySetupControls.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panelKeySetupControls.Controls.Add(this.btnAdd);
            this.panelKeySetupControls.Controls.Add(this.btnRemove);
            this.panelKeySetupControls.Controls.Add(this.btnAppend);
            this.panelKeySetupControls.Location = new System.Drawing.Point(613, 499);
            this.panelKeySetupControls.Name = "panelKeySetupControls";
            this.panelKeySetupControls.Size = new System.Drawing.Size(250, 23);
            this.panelKeySetupControls.TabIndex = 18;
            // 
            // KeyCaptureConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(875, 534);
            this.Controls.Add(this.panelKeySetupControls);
            this.Controls.Add(this.panelKeySetup);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.menuStripMain);
            this.MainMenuStrip = this.menuStripMain;
            this.Name = "KeyCaptureConfig";
            this.Text = "KeyCap Configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.KeyCaptureConfig_FormClosing);
            this.Load += new System.EventHandler(this.KeyCaptureConfig_Load);
            this.Resize += new System.EventHandler(this.KeyCaptureConfig_Resize);
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.contextMenuStripNotify.ResumeLayout(false);
            this.panelKeySetup.ResumeLayout(false);
            this.groupBoxOutputKey.ResumeLayout(false);
            this.groupBoxOutputKey.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDelay)).EndInit();
            this.groupBoxInputKey.ResumeLayout(false);
            this.groupBoxInputKey.PerformLayout();
            this.panelKeySetupControls.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtKeyIn;
        private System.Windows.Forms.TextBox txtKeyOut;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitMainToolStripMenuItem;
        private System.Windows.Forms.ListView listViewKeys;
        private System.Windows.Forms.ColumnHeader columnHeaderInput;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ColumnHeader columnHeaderOutput;
        private System.Windows.Forms.Button btnAppend;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.CheckBox checkInputAlt;
        private System.Windows.Forms.CheckBox checkInputControl;
        private System.Windows.Forms.CheckBox checkInputShift;
        private System.Windows.Forms.CheckBox checkOutputShift;
        private System.Windows.Forms.CheckBox checkOutputControl;
        private System.Windows.Forms.CheckBox checkOutputAlt;
        private System.Windows.Forms.CheckBox checkOutputNone;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripNotify;
        private System.Windows.Forms.ToolStripMenuItem toggleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restoreConfigurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Panel panelKeySetup;
        private System.Windows.Forms.Panel panelKeySetupControls;
        private System.Windows.Forms.ComboBox comboBoxMouseOut;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.GroupBox groupBoxInputKey;
        private System.Windows.Forms.GroupBox groupBoxOutputKey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownDelay;
        private System.Windows.Forms.ToolStripMenuItem previousConfigurationsToolStripMenuItem;
    }
}

