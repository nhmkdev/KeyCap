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
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.checkOutputNothing = new System.Windows.Forms.CheckBox();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStripNotify = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toggleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelKeySetup = new System.Windows.Forms.Panel();
            this.groupBoxOutputKey = new System.Windows.Forms.GroupBox();
            this.btnMouseRight = new System.Windows.Forms.Button();
            this.btnMouseMiddle = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnMouseLeft = new System.Windows.Forms.Button();
            this.btnAddOutputString = new System.Windows.Forms.Button();
            this.checkOutputCancel = new System.Windows.Forms.CheckBox();
            this.checkOutputDelay = new System.Windows.Forms.CheckBox();
            this.checkOutputRepeat = new System.Windows.Forms.CheckBox();
            this.checkOutputUp = new System.Windows.Forms.CheckBox();
            this.checkOutputDown = new System.Windows.Forms.CheckBox();
            this.checkOutputToggle = new System.Windows.Forms.CheckBox();
            this.numericUpDownOutputParameter = new System.Windows.Forms.NumericUpDown();
            this.panelKeySetupControls = new System.Windows.Forms.Panel();
            this.btnUpdateInput = new System.Windows.Forms.Button();
            this.btnAppendAt = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.groupBoxInputKey = new System.Windows.Forms.GroupBox();
            this.numericUpDownInputParameter = new System.Windows.Forms.NumericUpDown();
            this.checkInputLongPress = new System.Windows.Forms.CheckBox();
            this.btnRemoveAt = new System.Windows.Forms.Button();
            this.menuStripMain.SuspendLayout();
            this.contextMenuStripNotify.SuspendLayout();
            this.panelKeySetup.SuspendLayout();
            this.groupBoxOutputKey.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOutputParameter)).BeginInit();
            this.panelKeySetupControls.SuspendLayout();
            this.groupBoxInputKey.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInputParameter)).BeginInit();
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
            this.txtKeyIn.Size = new System.Drawing.Size(250, 20);
            this.txtKeyIn.TabIndex = 0;
            this.txtKeyIn.Enter += new System.EventHandler(this.txtKey_Enter);
            this.txtKeyIn.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtKeyIn_KeyDown);
            this.txtKeyIn.Leave += new System.EventHandler(this.txtKey_Leave);
            this.txtKeyIn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txtKeyIn_MouseDown);
            // 
            // txtKeyOut
            // 
            this.txtKeyOut.AcceptsTab = true;
            this.txtKeyOut.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtKeyOut.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtKeyOut.Location = new System.Drawing.Point(6, 18);
            this.txtKeyOut.Multiline = true;
            this.txtKeyOut.Name = "txtKeyOut";
            this.txtKeyOut.ReadOnly = true;
            this.txtKeyOut.Size = new System.Drawing.Size(250, 20);
            this.txtKeyOut.TabIndex = 50;
            this.txtKeyOut.Enter += new System.EventHandler(this.txtKey_Enter);
            this.txtKeyOut.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtKeyOut_KeyDown);
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
            this.menuStripMain.Size = new System.Drawing.Size(784, 24);
            this.menuStripMain.TabIndex = 4;
            this.menuStripMain.Text = "menuStripMain";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.exitMainToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.loadToolStripMenuItem.Text = "&Open...";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // exitMainToolStripMenuItem
            // 
            this.exitMainToolStripMenuItem.Name = "exitMainToolStripMenuItem";
            this.exitMainToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.exitMainToolStripMenuItem.Text = "&Exit";
            this.exitMainToolStripMenuItem.Click += new System.EventHandler(this.exitMainToolStripMenuItem_Click);
            // 
            // previousConfigurationsToolStripMenuItem
            // 
            this.previousConfigurationsToolStripMenuItem.Name = "previousConfigurationsToolStripMenuItem";
            this.previousConfigurationsToolStripMenuItem.Size = new System.Drawing.Size(137, 20);
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
            this.listViewKeys.Location = new System.Drawing.Point(12, 170);
            this.listViewKeys.Name = "listViewKeys";
            this.listViewKeys.Size = new System.Drawing.Size(651, 240);
            this.listViewKeys.TabIndex = 75;
            this.listViewKeys.UseCompatibleStateImageBehavior = false;
            this.listViewKeys.View = System.Windows.Forms.View.Details;
            this.listViewKeys.SelectedIndexChanged += new System.EventHandler(this.listViewKeys_SelectedIndexChanged);
            this.listViewKeys.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewKeys_MouseDoubleClick);
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
            this.columnHeaderOutput.Width = 405;
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.Location = new System.Drawing.Point(3, 177);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(96, 23);
            this.btnRemove.TabIndex = 92;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(3, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(96, 23);
            this.btnAdd.TabIndex = 90;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnAppend
            // 
            this.btnAppend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAppend.Enabled = false;
            this.btnAppend.Location = new System.Drawing.Point(3, 90);
            this.btnAppend.Name = "btnAppend";
            this.btnAppend.Size = new System.Drawing.Size(96, 23);
            this.btnAppend.TabIndex = 91;
            this.btnAppend.Text = "Append";
            this.btnAppend.UseVisualStyleBackColor = true;
            this.btnAppend.Click += new System.EventHandler(this.btnAppend_Click);
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStart.Location = new System.Drawing.Point(12, 446);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(78, 23);
            this.btnStart.TabIndex = 80;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // checkInputAlt
            // 
            this.checkInputAlt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkInputAlt.Location = new System.Drawing.Point(262, 20);
            this.checkInputAlt.Name = "checkInputAlt";
            this.checkInputAlt.Size = new System.Drawing.Size(80, 20);
            this.checkInputAlt.TabIndex = 10;
            this.checkInputAlt.Text = "Alt";
            this.checkInputAlt.UseVisualStyleBackColor = true;
            // 
            // checkInputControl
            // 
            this.checkInputControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkInputControl.Location = new System.Drawing.Point(348, 20);
            this.checkInputControl.Name = "checkInputControl";
            this.checkInputControl.Size = new System.Drawing.Size(80, 20);
            this.checkInputControl.TabIndex = 11;
            this.checkInputControl.Text = "Control";
            this.checkInputControl.UseVisualStyleBackColor = true;
            // 
            // checkInputShift
            // 
            this.checkInputShift.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkInputShift.Location = new System.Drawing.Point(434, 20);
            this.checkInputShift.Name = "checkInputShift";
            this.checkInputShift.Size = new System.Drawing.Size(80, 20);
            this.checkInputShift.TabIndex = 12;
            this.checkInputShift.Text = "Shift";
            this.checkInputShift.UseVisualStyleBackColor = true;
            // 
            // checkOutputShift
            // 
            this.checkOutputShift.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkOutputShift.Location = new System.Drawing.Point(434, 18);
            this.checkOutputShift.Name = "checkOutputShift";
            this.checkOutputShift.Size = new System.Drawing.Size(80, 20);
            this.checkOutputShift.TabIndex = 53;
            this.checkOutputShift.Text = "Shift";
            this.checkOutputShift.UseVisualStyleBackColor = true;
            // 
            // checkOutputControl
            // 
            this.checkOutputControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkOutputControl.Location = new System.Drawing.Point(348, 18);
            this.checkOutputControl.Name = "checkOutputControl";
            this.checkOutputControl.Size = new System.Drawing.Size(80, 20);
            this.checkOutputControl.TabIndex = 52;
            this.checkOutputControl.Text = "Control";
            this.checkOutputControl.UseVisualStyleBackColor = true;
            // 
            // checkOutputAlt
            // 
            this.checkOutputAlt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkOutputAlt.Location = new System.Drawing.Point(262, 17);
            this.checkOutputAlt.Name = "checkOutputAlt";
            this.checkOutputAlt.Size = new System.Drawing.Size(80, 20);
            this.checkOutputAlt.TabIndex = 51;
            this.checkOutputAlt.Text = "Alt";
            this.checkOutputAlt.UseVisualStyleBackColor = true;
            // 
            // checkOutputNothing
            // 
            this.checkOutputNothing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkOutputNothing.Location = new System.Drawing.Point(520, 44);
            this.checkOutputNothing.Name = "checkOutputNothing";
            this.checkOutputNothing.Size = new System.Drawing.Size(80, 20);
            this.checkOutputNothing.TabIndex = 59;
            this.checkOutputNothing.Text = " Nothing";
            this.checkOutputNothing.UseVisualStyleBackColor = true;
            this.checkOutputNothing.CheckedChanged += new System.EventHandler(this.checkGenericInputOutput_CheckedChanged);
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStripNotify;
            this.notifyIcon.Text = "KeyCap";
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
            this.contextMenuStripNotify.Size = new System.Drawing.Size(166, 76);
            // 
            // toggleToolStripMenuItem
            // 
            this.toggleToolStripMenuItem.Name = "toggleToolStripMenuItem";
            this.toggleToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.toggleToolStripMenuItem.Text = "Toggle";
            this.toggleToolStripMenuItem.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // restoreConfigurationToolStripMenuItem
            // 
            this.restoreConfigurationToolStripMenuItem.Name = "restoreConfigurationToolStripMenuItem";
            this.restoreConfigurationToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.restoreConfigurationToolStripMenuItem.Text = "Restore Configuration";
            this.restoreConfigurationToolStripMenuItem.Click += new System.EventHandler(this.restoreConfigurationToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(162, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitMainToolStripMenuItem_Click);
            // 
            // panelKeySetup
            // 
            this.panelKeySetup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelKeySetup.Controls.Add(this.groupBoxOutputKey);
            this.panelKeySetup.Controls.Add(this.panelKeySetupControls);
            this.panelKeySetup.Controls.Add(this.groupBoxInputKey);
            this.panelKeySetup.Controls.Add(this.listViewKeys);
            this.panelKeySetup.Location = new System.Drawing.Point(0, 27);
            this.panelKeySetup.Name = "panelKeySetup";
            this.panelKeySetup.Size = new System.Drawing.Size(784, 413);
            this.panelKeySetup.TabIndex = 17;
            // 
            // groupBoxOutputKey
            // 
            this.groupBoxOutputKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxOutputKey.Controls.Add(this.btnMouseRight);
            this.groupBoxOutputKey.Controls.Add(this.btnMouseMiddle);
            this.groupBoxOutputKey.Controls.Add(this.label1);
            this.groupBoxOutputKey.Controls.Add(this.btnMouseLeft);
            this.groupBoxOutputKey.Controls.Add(this.btnAddOutputString);
            this.groupBoxOutputKey.Controls.Add(this.checkOutputCancel);
            this.groupBoxOutputKey.Controls.Add(this.checkOutputDelay);
            this.groupBoxOutputKey.Controls.Add(this.checkOutputRepeat);
            this.groupBoxOutputKey.Controls.Add(this.checkOutputUp);
            this.groupBoxOutputKey.Controls.Add(this.checkOutputDown);
            this.groupBoxOutputKey.Controls.Add(this.checkOutputToggle);
            this.groupBoxOutputKey.Controls.Add(this.numericUpDownOutputParameter);
            this.groupBoxOutputKey.Controls.Add(this.txtKeyOut);
            this.groupBoxOutputKey.Controls.Add(this.checkOutputNothing);
            this.groupBoxOutputKey.Controls.Add(this.checkOutputControl);
            this.groupBoxOutputKey.Controls.Add(this.checkOutputAlt);
            this.groupBoxOutputKey.Controls.Add(this.checkOutputShift);
            this.groupBoxOutputKey.Location = new System.Drawing.Point(12, 61);
            this.groupBoxOutputKey.Name = "groupBoxOutputKey";
            this.groupBoxOutputKey.Size = new System.Drawing.Size(760, 103);
            this.groupBoxOutputKey.TabIndex = 19;
            this.groupBoxOutputKey.TabStop = false;
            this.groupBoxOutputKey.Text = "Output Key";
            // 
            // btnMouseRight
            // 
            this.btnMouseRight.Location = new System.Drawing.Point(201, 43);
            this.btnMouseRight.Name = "btnMouseRight";
            this.btnMouseRight.Size = new System.Drawing.Size(46, 23);
            this.btnMouseRight.TabIndex = 66;
            this.btnMouseRight.Text = "Right";
            this.btnMouseRight.UseVisualStyleBackColor = true;
            this.btnMouseRight.Click += new System.EventHandler(this.btnMouseGeneric_Click);
            // 
            // btnMouseMiddle
            // 
            this.btnMouseMiddle.Location = new System.Drawing.Point(149, 43);
            this.btnMouseMiddle.Name = "btnMouseMiddle";
            this.btnMouseMiddle.Size = new System.Drawing.Size(46, 23);
            this.btnMouseMiddle.TabIndex = 65;
            this.btnMouseMiddle.Text = "Middle";
            this.btnMouseMiddle.UseVisualStyleBackColor = true;
            this.btnMouseMiddle.Click += new System.EventHandler(this.btnMouseGeneric_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(7, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 23);
            this.label1.TabIndex = 64;
            this.label1.Text = "Mouse Button:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnMouseLeft
            // 
            this.btnMouseLeft.Location = new System.Drawing.Point(97, 44);
            this.btnMouseLeft.Name = "btnMouseLeft";
            this.btnMouseLeft.Size = new System.Drawing.Size(46, 23);
            this.btnMouseLeft.TabIndex = 63;
            this.btnMouseLeft.Text = "Left";
            this.btnMouseLeft.UseVisualStyleBackColor = true;
            this.btnMouseLeft.Click += new System.EventHandler(this.btnMouseGeneric_Click);
            // 
            // btnAddOutputString
            // 
            this.btnAddOutputString.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddOutputString.Location = new System.Drawing.Point(6, 74);
            this.btnAddOutputString.Name = "btnAddOutputString";
            this.btnAddOutputString.Size = new System.Drawing.Size(116, 23);
            this.btnAddOutputString.TabIndex = 62;
            this.btnAddOutputString.Text = "Add Output String";
            this.btnAddOutputString.UseVisualStyleBackColor = true;
            this.btnAddOutputString.Click += new System.EventHandler(this.btnAddOutputString_Click);
            // 
            // checkOutputCancel
            // 
            this.checkOutputCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkOutputCancel.Location = new System.Drawing.Point(598, 44);
            this.checkOutputCancel.Name = "checkOutputCancel";
            this.checkOutputCancel.Size = new System.Drawing.Size(156, 20);
            this.checkOutputCancel.TabIndex = 62;
            this.checkOutputCancel.Text = "Cancel Outputs";
            this.checkOutputCancel.UseVisualStyleBackColor = true;
            this.checkOutputCancel.CheckedChanged += new System.EventHandler(this.checkGenericInputOutput_CheckedChanged);
            // 
            // checkOutputDelay
            // 
            this.checkOutputDelay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkOutputDelay.Location = new System.Drawing.Point(348, 69);
            this.checkOutputDelay.Name = "checkOutputDelay";
            this.checkOutputDelay.Size = new System.Drawing.Size(80, 20);
            this.checkOutputDelay.TabIndex = 61;
            this.checkOutputDelay.Text = "Delay";
            this.checkOutputDelay.UseVisualStyleBackColor = true;
            this.checkOutputDelay.CheckedChanged += new System.EventHandler(this.checkGenericInputOutput_CheckedChanged);
            // 
            // checkOutputRepeat
            // 
            this.checkOutputRepeat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkOutputRepeat.Location = new System.Drawing.Point(262, 69);
            this.checkOutputRepeat.Name = "checkOutputRepeat";
            this.checkOutputRepeat.Size = new System.Drawing.Size(80, 20);
            this.checkOutputRepeat.TabIndex = 60;
            this.checkOutputRepeat.Text = "Repeat";
            this.checkOutputRepeat.UseVisualStyleBackColor = true;
            this.checkOutputRepeat.CheckedChanged += new System.EventHandler(this.checkGenericInputOutput_CheckedChanged);
            // 
            // checkOutputUp
            // 
            this.checkOutputUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkOutputUp.Checked = true;
            this.checkOutputUp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkOutputUp.Location = new System.Drawing.Point(262, 43);
            this.checkOutputUp.Name = "checkOutputUp";
            this.checkOutputUp.Size = new System.Drawing.Size(80, 20);
            this.checkOutputUp.TabIndex = 55;
            this.checkOutputUp.Text = "Up";
            this.checkOutputUp.UseVisualStyleBackColor = true;
            // 
            // checkOutputDown
            // 
            this.checkOutputDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkOutputDown.Checked = true;
            this.checkOutputDown.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkOutputDown.Location = new System.Drawing.Point(348, 44);
            this.checkOutputDown.Name = "checkOutputDown";
            this.checkOutputDown.Size = new System.Drawing.Size(80, 20);
            this.checkOutputDown.TabIndex = 56;
            this.checkOutputDown.Text = "Down";
            this.checkOutputDown.UseVisualStyleBackColor = true;
            // 
            // checkOutputToggle
            // 
            this.checkOutputToggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkOutputToggle.Location = new System.Drawing.Point(434, 44);
            this.checkOutputToggle.Name = "checkOutputToggle";
            this.checkOutputToggle.Size = new System.Drawing.Size(80, 20);
            this.checkOutputToggle.TabIndex = 57;
            this.checkOutputToggle.Text = "Toggle";
            this.checkOutputToggle.UseVisualStyleBackColor = true;
            this.checkOutputToggle.CheckedChanged += new System.EventHandler(this.checkGenericInputOutput_CheckedChanged);
            // 
            // numericUpDownOutputParameter
            // 
            this.numericUpDownOutputParameter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownOutputParameter.Enabled = false;
            this.numericUpDownOutputParameter.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownOutputParameter.Location = new System.Drawing.Point(434, 69);
            this.numericUpDownOutputParameter.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownOutputParameter.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownOutputParameter.Name = "numericUpDownOutputParameter";
            this.numericUpDownOutputParameter.Size = new System.Drawing.Size(80, 20);
            this.numericUpDownOutputParameter.TabIndex = 58;
            this.numericUpDownOutputParameter.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownOutputParameter.ValueChanged += new System.EventHandler(this.numericUpDownOutputParameter_ValueChanged);
            // 
            // panelKeySetupControls
            // 
            this.panelKeySetupControls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelKeySetupControls.Controls.Add(this.btnRemoveAt);
            this.panelKeySetupControls.Controls.Add(this.btnUpdateInput);
            this.panelKeySetupControls.Controls.Add(this.btnAppendAt);
            this.panelKeySetupControls.Controls.Add(this.btnUpdate);
            this.panelKeySetupControls.Controls.Add(this.btnAdd);
            this.panelKeySetupControls.Controls.Add(this.btnRemove);
            this.panelKeySetupControls.Controls.Add(this.btnAppend);
            this.panelKeySetupControls.Location = new System.Drawing.Point(669, 170);
            this.panelKeySetupControls.Name = "panelKeySetupControls";
            this.panelKeySetupControls.Size = new System.Drawing.Size(103, 240);
            this.panelKeySetupControls.TabIndex = 18;
            // 
            // btnUpdateInput
            // 
            this.btnUpdateInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdateInput.Enabled = false;
            this.btnUpdateInput.Location = new System.Drawing.Point(3, 61);
            this.btnUpdateInput.Name = "btnUpdateInput";
            this.btnUpdateInput.Size = new System.Drawing.Size(96, 23);
            this.btnUpdateInput.TabIndex = 95;
            this.btnUpdateInput.Text = "Update Input";
            this.btnUpdateInput.UseVisualStyleBackColor = true;
            this.btnUpdateInput.Click += new System.EventHandler(this.btnUpdateInput_Click);
            // 
            // btnAppendAt
            // 
            this.btnAppendAt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAppendAt.Enabled = false;
            this.btnAppendAt.Location = new System.Drawing.Point(3, 119);
            this.btnAppendAt.Name = "btnAppendAt";
            this.btnAppendAt.Size = new System.Drawing.Size(96, 23);
            this.btnAppendAt.TabIndex = 94;
            this.btnAppendAt.Text = "Append At...";
            this.btnAppendAt.UseVisualStyleBackColor = true;
            this.btnAppendAt.Click += new System.EventHandler(this.btnAppendAt_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdate.Enabled = false;
            this.btnUpdate.Location = new System.Drawing.Point(3, 32);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(96, 23);
            this.btnUpdate.TabIndex = 93;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // groupBoxInputKey
            // 
            this.groupBoxInputKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxInputKey.Controls.Add(this.numericUpDownInputParameter);
            this.groupBoxInputKey.Controls.Add(this.checkInputLongPress);
            this.groupBoxInputKey.Controls.Add(this.txtKeyIn);
            this.groupBoxInputKey.Controls.Add(this.checkInputAlt);
            this.groupBoxInputKey.Controls.Add(this.checkInputControl);
            this.groupBoxInputKey.Controls.Add(this.checkInputShift);
            this.groupBoxInputKey.Location = new System.Drawing.Point(12, 3);
            this.groupBoxInputKey.Name = "groupBoxInputKey";
            this.groupBoxInputKey.Size = new System.Drawing.Size(760, 52);
            this.groupBoxInputKey.TabIndex = 18;
            this.groupBoxInputKey.TabStop = false;
            this.groupBoxInputKey.Text = "Input Key";
            // 
            // numericUpDownInputParameter
            // 
            this.numericUpDownInputParameter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownInputParameter.Enabled = false;
            this.numericUpDownInputParameter.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDownInputParameter.Location = new System.Drawing.Point(593, 19);
            this.numericUpDownInputParameter.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numericUpDownInputParameter.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownInputParameter.Name = "numericUpDownInputParameter";
            this.numericUpDownInputParameter.Size = new System.Drawing.Size(80, 20);
            this.numericUpDownInputParameter.TabIndex = 63;
            this.numericUpDownInputParameter.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // checkInputLongPress
            // 
            this.checkInputLongPress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkInputLongPress.Location = new System.Drawing.Point(510, 19);
            this.checkInputLongPress.Name = "checkInputLongPress";
            this.checkInputLongPress.Size = new System.Drawing.Size(80, 20);
            this.checkInputLongPress.TabIndex = 13;
            this.checkInputLongPress.Text = "Long Press";
            this.checkInputLongPress.UseVisualStyleBackColor = true;
            this.checkInputLongPress.CheckedChanged += new System.EventHandler(this.checkGenericInputOutput_CheckedChanged);
            // 
            // btnRemoveAt
            // 
            this.btnRemoveAt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveAt.Enabled = false;
            this.btnRemoveAt.Location = new System.Drawing.Point(3, 148);
            this.btnRemoveAt.Name = "btnRemoveAt";
            this.btnRemoveAt.Size = new System.Drawing.Size(96, 23);
            this.btnRemoveAt.TabIndex = 96;
            this.btnRemoveAt.Text = "Remove At...";
            this.btnRemoveAt.UseVisualStyleBackColor = true;
            this.btnRemoveAt.Click += new System.EventHandler(this.btnRemoveAt_Click);
            // 
            // KeyCaptureConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 481);
            this.Controls.Add(this.panelKeySetup);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.menuStripMain);
            this.MainMenuStrip = this.menuStripMain;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 520);
            this.Name = "KeyCaptureConfig";
            this.ShowInTaskbar = false;
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
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOutputParameter)).EndInit();
            this.panelKeySetupControls.ResumeLayout(false);
            this.groupBoxInputKey.ResumeLayout(false);
            this.groupBoxInputKey.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInputParameter)).EndInit();
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
        private System.Windows.Forms.CheckBox checkOutputNothing;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripNotify;
        private System.Windows.Forms.ToolStripMenuItem toggleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restoreConfigurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Panel panelKeySetup;
        private System.Windows.Forms.Panel panelKeySetupControls;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.GroupBox groupBoxInputKey;
        private System.Windows.Forms.GroupBox groupBoxOutputKey;
        private System.Windows.Forms.NumericUpDown numericUpDownOutputParameter;
        private System.Windows.Forms.ToolStripMenuItem previousConfigurationsToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkOutputToggle;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkOutputUp;
        private System.Windows.Forms.CheckBox checkOutputDown;
        private System.Windows.Forms.CheckBox checkOutputRepeat;
        private System.Windows.Forms.CheckBox checkOutputDelay;
        private System.Windows.Forms.CheckBox checkOutputCancel;
        private System.Windows.Forms.Button btnAddOutputString;
        private System.Windows.Forms.CheckBox checkInputLongPress;
        private System.Windows.Forms.NumericUpDown numericUpDownInputParameter;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnMouseRight;
        private System.Windows.Forms.Button btnMouseMiddle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnMouseLeft;
        private System.Windows.Forms.Button btnAppendAt;
        private System.Windows.Forms.Button btnUpdateInput;
        private System.Windows.Forms.Button btnRemoveAt;
    }
}

