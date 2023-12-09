////////////////////////////////////////////////////////////////////////////////
// The MIT License (MIT)
//
// Copyright (c) 2023 Tim Stair
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
////////////////////////////////////////////////////////////////////////////////

// #define LOG_KEYS

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using KeyCap.Format;
using KeyCap.Settings;
using KeyCap.Util;
using KeyCap.Wrapper;
using Support.IO;
using Support.UI;
using static KeyCap.Format.InputConfig;
using static KeyCap.Format.OutputConfig;

namespace KeyCap.Forms
{
    public partial class KeyCaptureConfig
    {
        private bool m_bHandlingGenericInputOutputEvent = false;
        private readonly ConfigFileManager m_zConfigFileManager = new ConfigFileManager();
        private readonly List<string> m_listRecentFiles = new List<string>();
        private readonly KeyCapInstanceState m_zInstanceState;
        private readonly IniManager m_zIniManager = new IniManager(Application.ProductName, false, true, false);
        private Dictionary<Control, ControlActiveProcessor> m_dictionaryControlActive =
            new Dictionary<Control, ControlActiveProcessor>();

        private readonly UIInputConfig m_zActiveInputConfig = new UIInputConfig(0);
        private readonly UIOutputConfig m_zActiveOutputConfig = new UIOutputConfig(0, false);

        private FormWindowState m_ePrevWindowState = FormWindowState.Normal;
        private bool m_bShutdownApplication = false;

        /// <summary>
        /// Text to display on the button to start/stop capturing
        /// </summary>
        private enum ActionText
        {
            Start,
            Stop
        }

        /// <summary>
        /// Constructs a new dialog
        /// </summary>
        /// <param name="args">The command line arguments (if a file is specified it is loaded)</param>
        public KeyCaptureConfig(IReadOnlyList<string> args)
        {
            InitializeComponent();
            m_sBaseTitle = Application.ProductName + " Configuration " + Application.ProductVersion;
            m_sFileOpenFilter = Application.ProductName + " Config files (*.kfg)|*.kfg|All files (*.*)|*.*";
            Text = m_sBaseTitle;

            m_zInstanceState = new KeyCapInstanceState(args);
        }

        #region Initialization

        private void KeyCaptureConfig_Load(object sender, EventArgs e)
        {
            // must be in the load event to avoid the location being incorrect
            IniManager.RestoreState(this, m_zIniManager.GetValue(Name));

            // set the notification icon accordingly
            notifyIcon.Icon = Resources.KeyCapIdle;
            Icon = notifyIcon.Icon;

            // populate the previously loaded configurations
            var arrayFiles = m_zIniManager.GetValue(IniSettings.PreviousFiles)
                .Split(new char[] { KeyCapConstants.CharFileSplit }, StringSplitOptions.RemoveEmptyEntries);
            if (0 < arrayFiles.Length)
            {
                foreach (var sFile in arrayFiles)
                {
                    m_listRecentFiles.Add(sFile);
                }
            }

            btnMouseLeft.Tag = MouseButton.MouseLeft;
            btnMouseMiddle.Tag = MouseButton.MouseMiddle;
            btnMouseRight.Tag = MouseButton.MouseRight;

            InitControlSets();
            ConfigureToolTips();

            // load the command line specified file
            if (m_zInstanceState.DefaultConfigFile != null)
            {
                InitOpen(m_zInstanceState.DefaultConfigFile);
            }

            // initialize capture from command line specified file
            if (0 != m_sLoadedFile.Length && m_zInstanceState.AutoStart)
            {
                btnStart_Click(sender, EventArgs.Empty);
                new Thread(MinimizeThread) { Name = "MinimizeThread" }.Start();
            }
        }

        private void InitControlSets()
        {
            m_dictionaryControlActive =
                new Dictionary<Control, ControlActiveProcessor>()
                {
                    [checkInputLongPress] = new ControlActiveProcessor(checkInputLongPress)
                    {
                        disabledControls = new List<Control> { checkInputAlt, checkInputControl, checkInputShift },
                        enabledControls = new List<Control> { numericUpDownInputParameter },
                        uncheckedControls = new List<CheckBox> { checkInputAlt, checkInputControl, checkInputShift },
                    },
                    [checkOutputCancel] = new ControlActiveProcessor(checkOutputCancel)
                    {
                        disabledControls = new List<Control>{checkOutputAlt, checkOutputControl, checkOutputShift, checkOutputUp, checkOutputDown,
                            checkOutputToggle, checkOutputNothing, checkOutputRepeat, checkOutputDelay, numericUpDownOutputParameter },
                        uncheckedControls = new List<CheckBox> { checkOutputToggle, checkOutputNothing, checkOutputRepeat, checkOutputDelay },
                        OnActiveChange = (bActive) =>
                        {
                            if (bActive) m_zActiveOutputConfig.Reset();
                            UpdateTextBox(txtKeyOut, null, CreateOutputConfigFromUI());
                        },
                    },
                    [checkOutputNothing] = new ControlActiveProcessor(checkOutputNothing)
                    {
                        disabledControls = new List<Control>{checkOutputAlt, checkOutputControl, checkOutputShift, checkOutputUp, checkOutputDown,
                            checkOutputToggle, checkOutputCancel, checkOutputRepeat, checkOutputDelay, numericUpDownOutputParameter, },
                        uncheckedControls = new List<CheckBox> { checkOutputToggle, checkOutputCancel, checkOutputRepeat, checkOutputDelay },
                        OnActiveChange = (bActive) =>
                        {
                            if (bActive) m_zActiveOutputConfig.Reset();
                            UpdateTextBox(txtKeyOut, null, CreateOutputConfigFromUI());
                        },
                    },
                    [checkOutputToggle] = new ControlActiveProcessor(checkOutputToggle)
                    {
                        disabledControls = new List<Control>{checkOutputAlt, checkOutputControl, checkOutputShift, checkOutputUp, checkOutputDown,
                            checkOutputNothing, checkOutputCancel, checkOutputRepeat, checkOutputDelay, numericUpDownOutputParameter},
                        uncheckedControls = new List<CheckBox> { checkOutputAlt, checkOutputControl, checkOutputShift, checkOutputNothing,
                            checkOutputCancel, checkOutputRepeat, checkOutputDelay },
                        checkedControls = new List<CheckBox> { checkOutputDown, checkOutputUp },
                    },
                    [checkOutputRepeat] = new ControlActiveProcessor(checkOutputRepeat)
                    {
                        disabledControls = new List<Control> { checkOutputUp, checkOutputDown, checkOutputNothing, checkOutputCancel, checkOutputDelay, checkOutputToggle },
                        enabledControls = new List<Control> { numericUpDownOutputParameter },
                        uncheckedControls = new List<CheckBox> { checkOutputToggle, checkOutputNothing, checkOutputCancel, checkOutputDelay },
                        checkedControls = new List<CheckBox> { checkOutputDown, checkOutputUp },
                        OnActiveChange = (bActive) =>
                        {
                            numericUpDownOutputParameter_ValueChanged(numericUpDownOutputParameter, null);
                            UpdateTextBox(txtKeyOut, null, CreateOutputConfigFromUI());
                        }
                    },
                    [checkOutputDelay] = new ControlActiveProcessor(checkOutputDelay)
                    {
                        disabledControls = new List<Control> { checkOutputUp, checkOutputDown, checkOutputNothing, checkOutputCancel, checkOutputRepeat,
                            checkOutputToggle, checkOutputAlt, checkOutputControl, checkOutputShift},
                        enabledControls = new List<Control> { numericUpDownOutputParameter },
                        uncheckedControls = new List<CheckBox> { checkOutputToggle, checkOutputNothing, checkOutputCancel, checkOutputRepeat },
                        OnActiveChange = (bActive) =>
                        {
                            numericUpDownOutputParameter_ValueChanged(numericUpDownOutputParameter, null);
                            if (bActive) m_zActiveOutputConfig.Reset();
                            UpdateTextBox(txtKeyOut, null, CreateOutputConfigFromUI());
                        }
                    }
                };
        }

        private void ConfigureToolTips()
        {
            var zToolTip = new ToolTip()
            {
                AutoPopDelay = 10000,
                InitialDelay = 500,
                ReshowDelay = 250,
                ShowAlways = false,
            };

            zToolTip.SetToolTip(checkInputAlt, "Requires Alt to be pressed");
            zToolTip.SetToolTip(checkInputControl, "Requires Ctrl to be pressed");
            zToolTip.SetToolTip(checkInputShift, "Requires Shift to be pressed");
            zToolTip.SetToolTip(checkInputLongPress, "Require the key to be held for a minimum amount of time");
            zToolTip.SetToolTip(numericUpDownInputParameter, "The minimum duration of the long press (milliseconds). \n\nNOTE: The 'Repeat Delay' setting in the Windows Keyboard control panel has a direct impact on how low this can be.");

            zToolTip.SetToolTip(checkOutputAlt, "Generates Alt event");
            zToolTip.SetToolTip(checkOutputControl, "Generates Ctrl event");
            zToolTip.SetToolTip(checkOutputShift, "Generates Shift event");
            zToolTip.SetToolTip(checkOutputUp, "Generates key up event (key released)");
            zToolTip.SetToolTip(checkOutputDown, "Generates key down event (key held)");
            zToolTip.SetToolTip(checkOutputToggle, "When enabled the same input key will alternate generating key up/down events.\n\n NOTE: Toggle does not support alt/ctrl/shift outputs.");
            zToolTip.SetToolTip(checkOutputNothing, "Consumes input, generating no events");
            zToolTip.SetToolTip(checkOutputCancel, "Cancels all active outputs");
            zToolTip.SetToolTip(checkOutputRepeat, "Generates the specified event perpetually");
            zToolTip.SetToolTip(checkOutputDelay, "Generates a delay based on the value specified (milliseconds)");
            zToolTip.SetToolTip(numericUpDownOutputParameter, "The duration of the delay or gap between repetitions (milliseconds)");

            zToolTip.SetToolTip(btnAdd, "Adds the configured input/output");
            zToolTip.SetToolTip(btnUpdate, "Updates/replaces the configured input/output");
            zToolTip.SetToolTip(btnAppend, "Appends the configured output");
            zToolTip.SetToolTip(btnRemove, "Removes the selected entries");
        }

        #endregion

        #region Entire Form Events

        private void KeyCaptureConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            // exit requested (can only be canceled with the save on close cancel)
            if (m_bShutdownApplication)
            {
                SaveOnClose(e);
                if (e.Cancel)
                {
                    m_bShutdownApplication = false;
                    return;
                }

                FlushIniSettings();
            }
            else
            {
                switch (e.CloseReason)
                {
                    case CloseReason.TaskManagerClosing:
                    case CloseReason.WindowsShutDown:
                        SaveOnClose(e);
                        break;
                    default:
                        e.Cancel = true;
                        Hide();
                        break;
                }
            }
        }

        private void KeyCaptureConfig_Resize(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                m_ePrevWindowState = WindowState;
            }
        }

        #endregion

        #region Form Menu Click Events

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var zCancelEvent = new CancelEventArgs();
            SaveOnClose(zCancelEvent);
            if (zCancelEvent.Cancel)
            {
                return;
            }

            listViewKeys.Items.Clear();
            InitNew();
        }

        private void exitMainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_bShutdownApplication = true;
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = m_ePrevWindowState;
            }

            KeyCaptureLib.Shutdown();
            m_zIniManager.SetValue(Name, IniManager.GetFormSettings(this));
            Close();
        }

        #endregion

        #region Key Capture Process

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (0 == listViewKeys.Items.Count)
            {
                return; // no keys, no point!
            }

            if (btnStart.Text.Equals(ActionText.Stop.ToString()))
            {
                KeyCaptureLib.Shutdown();
                ConfigureControlsForCapture(false);
            }
            else if (btnStart.Text.Equals(ActionText.Start.ToString()))
            {
                InitSave(false);
                if (!Dirty)
                {
                    ConfigureControlsForCapture(true);
                    var eReturn = KeyCaptureLib.LoadFileAndCapture(m_sLoadedFile);
                    switch (eReturn)
                    {
                        case CaptureMessage.HookCreationSuccess:
                            break;
                        case CaptureMessage.HookCreationFailure:
                        case CaptureMessage.InputBad:
                        case CaptureMessage.InputMissing:
                        case CaptureMessage.InputZero:
                        default:
                            MessageBox.Show(
                                "Failed to initialize key capture. Exited with error code: {0}".FormatString(eReturn),
                                "Key Capture Failed", 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Error);
                            ConfigureControlsForCapture(false);
                            break;
                    }
                }
            }
        }

        #endregion

        #region Text Capture Handling

        private void txtKeyIn_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyUtil.IsFlagKey(e.KeyCode))
            {
                return;
            }
#if LOG_KEYS
            Console.Out.WriteLine("Key Input: {0} 0x{1}".FormatString(e.KeyCode, e.KeyCode.ToString("x")));
#endif
            m_zActiveInputConfig.VirtualKey = (byte)e.KeyCode;
            UpdateTextBox((TextBox)sender, e, CreateInputConfigFromUI());
        }

        private void txtKeyOut_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyUtil.IsFlagKey(e.KeyCode))
            {
                return;
            }
#if LOG_KEYS
            Console.Out.WriteLine("Key Input: {0} 0x{1}".FormatString(e.KeyCode, e.KeyCode.ToString("x")));
#endif
            // reset these specific flags if a key is set
            checkOutputNothing.Checked = false;
            checkOutputCancel.Checked = false;
            checkOutputDelay.Checked = false;
            checkOutputDown.Checked = true;
            checkOutputUp.Checked = true;

            m_zActiveOutputConfig.VirtualKey = (byte)e.KeyCode;
            m_zActiveOutputConfig.MouseClick = false;
            UpdateTextBox((TextBox)sender, e, CreateOutputConfigFromUI());
        }

        private void txtKey_Enter(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.LightGoldenrodYellow;
        }

        private void txtKey_Leave(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = SystemColors.Control;
        }

        private void btnMouseGeneric_Click(object sender, EventArgs e)
        {
            m_zActiveOutputConfig.VirtualKey = (byte)(MouseButton)((Button)sender).Tag;
            m_zActiveOutputConfig.MouseClick = true;
            UpdateTextBox(txtKeyOut, null, CreateOutputConfigFromUI());
        }

        private void txtKeyIn_MouseDown(object sender, MouseEventArgs e)
        {
            byte keyCode = 0;
            switch (e.Button)
            {
                case MouseButtons.Middle:
                    keyCode = 0x04; // VK_MBUTTON
                    break;
                case MouseButtons.XButton1:
                    keyCode = 0x05; // VK_XBUTTON1
                    break;
                case MouseButtons.XButton2:
                    keyCode = 0x06; // VK_XBUTTON2
                    break;
            }

            if (keyCode == 0)
                return; // unsupported

#if LOG_KEYS
            Console.Out.WriteLine("Key Input: {0} 0x{1}".FormatString(keyCode, keyCode.ToString("x")));
#endif
            m_zActiveInputConfig.VirtualKey = keyCode;
            UpdateTextBox((TextBox)sender, null, CreateInputConfigFromUI());
        }

        #endregion

        #region Output String Generation

        private OutputConfig CreateOutputConfigFromCharacter(char cInput)
        {
            var bShift = false;
            var byteKey = KeyUtil.GetKeyByte(cInput, ref bShift);
            var nFlags = (bShift ? (int)OutputConfig.OutputFlag.Shift : 0) |
                         (int)OutputConfig.OutputFlag.Down | (int)OutputConfig.OutputFlag.Up;
            return new OutputConfig(nFlags, byteKey);
        }

        private void btnAddOutputString_Click(object sender, EventArgs e)
        {
            const string MACRO_STRING_KEY = "macro_string_key";
            var zQuery = new QueryPanelDialog("Enter String Macro", 400, false);
            zQuery.SetIcon(this.Icon);
            zQuery.AddTextBox("String", string.Empty, false, MACRO_STRING_KEY);
            if (DialogResult.OK != zQuery.ShowDialog(this))
            {
                return;
            }

            var sMacro = zQuery.GetString(MACRO_STRING_KEY);
            if (string.IsNullOrWhiteSpace(sMacro))
            {
                MessageBox.Show(this, "Please specify a string of output characters.", "Error!", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (!ValidateInputHasAction()) return;

            try
            {
                var zRemapEntry = new RemapEntry(CreateInputConfigFromUI(), CreateOutputConfigFromCharacter(sMacro[0]));
                for (var nIdx = 1; nIdx < sMacro.Length; nIdx++)
                {
                    zRemapEntry.AppendOutputConfig(CreateOutputConfigFromCharacter(sMacro[nIdx]));
                }

                if (!IsInputAlreadyDefined(zRemapEntry))
                {
                    AddRemapEntryToListView(zRemapEntry, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    "Unfortunately you have specified an unsupported character (at this time)." + ex.ToString(),
                    "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region AbstractDirtyForm overrides

        protected override bool SaveFormData(string sFileName)
        {
            var listConfigs = new List<RemapEntry>(listViewKeys.Items.Count);
            foreach (ListViewItem zItem in listViewKeys.Items)
            {
                listConfigs.Add((RemapEntry)zItem.Tag);
            }

            m_zConfigFileManager.SaveFile(listConfigs, sFileName);
            // on save the project list should be updated
            UpdateProjectsList(sFileName);
            return true;
        }

        protected override bool OpenFormData(string sFileName)
        {
            txtKeyIn.Text = string.Empty;
            txtKeyOut.Text = string.Empty;
            listViewKeys.Items.Clear();
            try
            {
                var listConfigs = m_zConfigFileManager.LoadFile(sFileName);
                listConfigs.ForEach(ioc =>
                {
                    listViewKeys.Items.Add(new ListViewItem(new string[]
                    {
                        ioc.GetInputString(),
                        ioc.GetOutputString()
                    })
                    {
                        Tag = ioc
                    });
                });
                UpdateProjectsList(sFileName);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return true;
        }

        #endregion

        #region Menu Events

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitOpen();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitSave(false);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitSave(true);
        }

        private void restoreConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = m_ePrevWindowState;
            Win32.ShowTopmost(Handle);
        }

        private void previousConfigurationsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            previousConfigurationsToolStripMenuItem.DropDownItems.Clear();
            foreach (var sFile in m_listRecentFiles)
            {
                previousConfigurationsToolStripMenuItem.DropDownItems.Add(sFile, null, recentConfiguration_Click);
            }
        }

        private void recentConfiguration_Click(object sender, EventArgs e)
        {
            var zItem = (ToolStripItem)sender;
            InitOpen(zItem.Text);
        }

        #endregion

        #region ListView Events

        private void listViewKeys_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateActionEnableStates();
        }

        private void listViewKeys_Resize(object sender, EventArgs e)
        {
            ListViewAssist.ResizeColumnHeaders(listViewKeys);
        }

        private void listViewKeys_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // attempt to load the item into the controls
            if (listViewKeys.SelectedItems.Count != 1)
            {
                return;
            }

            m_bHandlingGenericInputOutputEvent = true;
            ResetInputOutputUI();

            var zRemapEntry = (RemapEntry)listViewKeys.SelectedItems[0].Tag;
            var zInputConfig = zRemapEntry.InputConfig;
            var zFirstOutputConfig = zRemapEntry.OutputConfigs[0];

            m_zActiveInputConfig.VirtualKey = zRemapEntry.InputConfig.VirtualKey;
            m_zActiveOutputConfig.VirtualKey = zFirstOutputConfig.VirtualKey;
            m_zActiveOutputConfig.MouseClick = zFirstOutputConfig.IsFlaggedAs(OutputFlag.MouseOut);

            // input handling
            if (zInputConfig.IsFlaggedAs(InputFlag.LongPress))
            {
                checkInputLongPress.Checked = true;
            }
            else
            {
                checkInputLongPress.Checked = false;
                checkInputAlt.Checked = zInputConfig.IsFlaggedAs(InputFlag.Alt);
                checkInputShift.Checked = zInputConfig.IsFlaggedAs(InputFlag.Shift);
                checkInputControl.Checked = zInputConfig.IsFlaggedAs(InputFlag.Control);
            }

            UpdateTextBox(txtKeyIn, null, zInputConfig);

            checkOutputAlt.Enabled = true;
            checkOutputShift.Enabled = true;
            checkOutputControl.Enabled = true;

            m_bHandlingGenericInputOutputEvent = false;

            // output handling
            if (zFirstOutputConfig.IsFlaggedAs(OutputFlag.Toggle))
            {
                checkOutputToggle.Checked = true;
            }
            else if (zFirstOutputConfig.IsFlaggedAs(OutputFlag.CancelActiveOutputs))
            {
                checkOutputCancel.Checked = true;
            }
            else if (zFirstOutputConfig.IsFlaggedAs(OutputFlag.DoNothing))
            {
                checkOutputNothing.Checked = true;
            }
            else if (zFirstOutputConfig.IsFlaggedAs(OutputFlag.Delay))
            {
                checkOutputDelay.Checked = true;
                numericUpDownOutputParameter.Value = zFirstOutputConfig.Parameter;
            }
            else if (zFirstOutputConfig.IsFlaggedAs(OutputFlag.Repeat))
            {
                checkOutputRepeat.Checked = true;
                numericUpDownOutputParameter.Value = zFirstOutputConfig.Parameter;
            }
            else
            {
                checkOutputUp.Checked = zFirstOutputConfig.IsFlaggedAs(OutputFlag.Up);
                checkOutputDown.Checked = zFirstOutputConfig.IsFlaggedAs(OutputFlag.Down);
            }
            checkOutputAlt.Checked = zFirstOutputConfig.IsFlaggedAs(OutputFlag.Alt);
            checkOutputShift.Checked = zFirstOutputConfig.IsFlaggedAs(OutputFlag.Shift);
            checkOutputControl.Checked = zFirstOutputConfig.IsFlaggedAs(OutputFlag.Control);
            UpdateTextBox(txtKeyOut, null, CreateOutputConfigFromUI());
        }

        #endregion

        #region Config Action Events

        private void btnAdd_Click(object sender, EventArgs e)
        {
            RemapEntry zRemapEntry = null;
            if (!CreateRemapEntryFromActiveConfigs(ref zRemapEntry)) return;
            AddRemapEntryToListView(zRemapEntry, true);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (listViewKeys.SelectedItems.Count == 1)
            {
                var zSelectedEntry = (RemapEntry)listViewKeys.SelectedItems[0].Tag;
                if (zSelectedEntry.OutputConfigCount > 1)
                {
                    switch (MessageBox.Show(this, "This entry has multiple outputs. Do you want to overwrite the outputs?" +
                                                  "\nYES - overwrite input & outputs." +
                                                  "\nNO - overwrite input only.", "Remap has multiple outputs", MessageBoxButtons.YesNoCancel,
                                MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                    {
                        case DialogResult.Yes:
                            // nothing
                            break;
                        case DialogResult.No:
                            btnUpdateInput_Click(sender, e);
                            return;
                        case DialogResult.Cancel:
                            return;
                    }
                }
                RemapEntry zRemapEntry = null;
                if (!CreateRemapEntryFromActiveConfigs(ref zRemapEntry, (RemapEntry)listViewKeys.SelectedItems[0].Tag))
                {
                    MessageBox.Show("Unable to determine remap from configuration.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                AddRemapEntryToListView(zRemapEntry, true, listViewKeys.SelectedItems[0]);
            }
        }

        private void btnUpdateInput_Click(object sender, EventArgs e)
        {
            RemapEntry zRemapEntry = null;
            if (!CreateRemapEntryFromActiveConfigs(ref zRemapEntry, (RemapEntry)listViewKeys.SelectedItems[0].Tag,
                    true))
            {
                MessageBox.Show("Unable to determine remap from configuration.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            AddRemapEntryToListView(zRemapEntry, true, listViewKeys.SelectedItems[0]);
        }

        private void btnAppend_Click(object sender, EventArgs e)
        {
            if (1 != listViewKeys.SelectedItems.Count)
            {
                return;
            }

            var zItem = listViewKeys.SelectedItems[0];
            var zRemapEntry = (RemapEntry)zItem.Tag;
            OutputConfig zOutputConfig = null;
            if (!RetrieveOutputConfigForAppend(zRemapEntry, ref zOutputConfig))
            {
                MessageBox.Show("Unable to determine remap from configuration.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            zRemapEntry.AppendOutputConfig(zOutputConfig);
            zItem.SubItems[1].Text = zRemapEntry.GetOutputString();
            MarkDirty();
            txtKeyOut.Focus(); // restore focus to the output
            UpdateActionEnableStates();
        }

        private void btnAppendAt_Click(object sender, EventArgs e)
        {
            if (1 != listViewKeys.SelectedItems.Count)
            {
                return;
            }

            var zItem = listViewKeys.SelectedItems[0];
            var zRemapEntry = (RemapEntry)zItem.Tag;
            OutputConfig zOutputConfig = null;
            if (!RetrieveOutputConfigForAppend(zRemapEntry, ref zOutputConfig))
            {
                MessageBox.Show("Unable to determine remap from configuration.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                return;
            }

            const string IDX_QUERY_KEY = "idx";
            var zQuery = new QueryPanelDialog("Append output config", 450, false);
            zQuery.SetIcon(Icon);
            zQuery.AddLabel("Select the index to append the output at (the selected output will be pushed forward to insert the new output).", 48);
            var arrayOutputs = zRemapEntry.OutputConfigs.Select((zOutput, nIdx) => $"{nIdx}:{zOutput.GetDescription()}").ToArray();
            zQuery.AddPullDownBox("Output To Append At", arrayOutputs, 0, IDX_QUERY_KEY);
            if(DialogResult.Cancel == zQuery.ShowDialog(this))
            {
                return;
            }

            zRemapEntry.AppendOutputConfigAt(zOutputConfig, zQuery.GetIndex(IDX_QUERY_KEY));
            zItem.SubItems[1].Text = zRemapEntry.GetOutputString();
            MarkDirty();
            txtKeyOut.Focus(); // restore focus to the output
            UpdateActionEnableStates();
        }

        private void btnRemoveAt_Click(object sender, EventArgs e)
        {
            if (1 != listViewKeys.SelectedItems.Count)
            {
                return;
            }

            var zItem = listViewKeys.SelectedItems[0];
            var zRemapEntry = (RemapEntry)zItem.Tag;
            OutputConfig zOutputConfig = null;
            if (!RetrieveOutputConfigForAppend(zRemapEntry, ref zOutputConfig))
            {
                MessageBox.Show("Unable to determine remap from configuration.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            const string IDX_QUERY_KEY = "idx";
            var zQuery = new QueryPanelDialog("Remove output config at index", 450, false);
            zQuery.SetIcon(Icon);
            zQuery.AddLabel("Select the output to remove.", 24);
            var arrayOutputs = zRemapEntry.OutputConfigs.Select((zOutput, nIdx) => $"{nIdx}:{zOutput.GetDescription()}").ToArray();
            zQuery.AddPullDownBox("Output To Remove", arrayOutputs, 0, IDX_QUERY_KEY);
            if (DialogResult.Cancel == zQuery.ShowDialog(this))
            {
                return;
            }

            zRemapEntry.RemoveOutputConfigAt(zQuery.GetIndex(IDX_QUERY_KEY));
            zItem.SubItems[1].Text = zRemapEntry.GetOutputString();
            MarkDirty();
            UpdateActionEnableStates();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (0 == listViewKeys.SelectedItems.Count)
            {
                return;
            }

            if(DialogResult.No == MessageBox.Show(
                   "Are you sure you want to remove the selected items?", 
                   "Remove Confirmation", 
                   MessageBoxButtons.YesNo, 
                   MessageBoxIcon.Question, 
                   MessageBoxDefaultButton.Button1))
            {
                return;
            }

            foreach (ListViewItem zItem in listViewKeys.SelectedItems)
            {
                listViewKeys.Items.Remove(zItem);
            }

            MarkDirty();
        }

        #endregion

        #region Input / Output Settings Control Events

        private void checkGenericInputOutput_CheckedChanged(object sender, EventArgs e)
        {
            if (m_bHandlingGenericInputOutputEvent)
            {
                return;
            }
            m_bHandlingGenericInputOutputEvent = true;
            m_dictionaryControlActive[(Control)sender].Process(((CheckBox)sender).Checked);
            m_bHandlingGenericInputOutputEvent = false;
        }
        
        private void numericUpDownOutputParameter_ValueChanged(object sender, EventArgs e)
        {
            if (checkOutputDelay.Checked || checkOutputRepeat.Checked)
            {
                UpdateTextBox(txtKeyOut, null, CreateOutputConfigFromUI());
            }
        }

        #endregion

        #region Support Methods

        private void UpdateActionEnableStates()
        {
            btnAppend.Enabled = (1 == listViewKeys.SelectedIndices.Count);
            btnAppendAt.Enabled = (1 == listViewKeys.SelectedIndices.Count);
            btnUpdate.Enabled = (1 == listViewKeys.SelectedIndices.Count);
            btnUpdateInput.Enabled = (1 == listViewKeys.SelectedIndices.Count);
            btnRemoveAt.Enabled = (listViewKeys.SelectedItems.Count == 1) &&
                                  (((RemapEntry)listViewKeys.SelectedItems[0].Tag).OutputConfigCount > 1);
        }

        private void ResetInputOutputUI()
        {
            m_zActiveInputConfig.Reset();
            m_zActiveOutputConfig.Reset();

            var inputControls = new Control[]
                { checkInputAlt, checkInputShift, checkInputControl, checkInputLongPress };

            foreach (var c in inputControls)
            {
                c.Enabled = true;
                if (c is CheckBox)
                {
                    ((CheckBox)c).Checked = false;
                }
            }
            numericUpDownInputParameter.Enabled = false;
            numericUpDownInputParameter.Value = numericUpDownInputParameter.Minimum;

            var outputControls = new Control[]
            { checkOutputAlt, checkOutputShift, checkOutputControl, checkOutputDown, checkOutputUp, checkOutputToggle, checkOutputNothing,
                checkOutputCancel, checkOutputRepeat, checkOutputDelay };

            foreach (var c in outputControls)
            {
                c.Enabled = true;
                if (c is CheckBox)
                {
                    ((CheckBox)c).Checked = false;
                }
            }

            checkOutputDown.Checked = true;
            checkOutputUp.Checked = true;
            numericUpDownOutputParameter.Enabled = false;
            numericUpDownOutputParameter.Value = numericUpDownOutputParameter.Minimum;
        }

        private void AddRemapEntryToListView(RemapEntry zRemapEntry, bool bMarkDirty, ListViewItem zExistingItem = null)
        {
            var zItem = zExistingItem ?? new ListViewItem(new string[] {string.Empty, string.Empty});
            zItem.SubItems[0].Text = zRemapEntry.GetInputString();
            zItem.SubItems[1].Text = zRemapEntry.GetOutputString();
            zItem.Tag = zRemapEntry;
            zItem.ToolTipText = zRemapEntry.GetOutputString();
            if (zExistingItem == null)
            {
                listViewKeys.Items.Add(zItem);
            }
            listViewKeys.SelectedItems.Clear();
            zItem.Selected = true;
            if (bMarkDirty)
            {
                MarkDirty();
            }
        }

        private void UpdateTextBox<T>(TextBox txtBox, KeyEventArgs e, T config) where T : BaseIOConfig
        {
            txtBox.Text = config.GetActionOnlyDescription();
            if (e != null)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Updates the recent loaded file list
        /// </summary>
        /// <param name="sFileName">The most recently loaded file</param>
        private void UpdateProjectsList(string sFileName)
        {
            m_listRecentFiles.Remove(sFileName);
            m_listRecentFiles.Insert(0, sFileName);
            while (KeyCapConstants.MaxRecentProjects < m_listRecentFiles.Count)
            {
                m_listRecentFiles.RemoveAt(KeyCapConstants.MaxRecentProjects);
            }
        }

        private InputConfig CreateInputConfigFromUI()
        {
            var bAlt = checkInputAlt.Checked;
            var bControl = checkInputControl.Checked;
            var bShift = checkInputShift.Checked;
            var bLongPress = checkInputLongPress.Checked;

            var nFlags = 0;
            nFlags = BitUtil.UpdateFlag(nFlags, InputFlag.Alt, bAlt);
            nFlags = BitUtil.UpdateFlag(nFlags, InputFlag.Control, bControl);
            nFlags = BitUtil.UpdateFlag(nFlags, InputFlag.Shift, bShift);
            nFlags = BitUtil.UpdateFlag(nFlags, InputFlag.LongPress, bLongPress);
            return new InputConfig(m_zActiveInputConfig.VirtualKey)
            {
                Flags = nFlags,
                Parameter = bLongPress ? (int)numericUpDownInputParameter.Value : 0
            };
        }

        private OutputConfig CreateOutputConfigFromUI()
        {
            var bAlt = checkOutputAlt.Checked;
            var bControl = checkOutputControl.Checked;
            var bShift = checkOutputShift.Checked;
            var bToggle = checkOutputToggle.Checked;
            var bRepeat = checkOutputRepeat.Checked;
            var bDelay = checkOutputDelay.Checked;
            var bDown = checkOutputDown.Checked;
            var bUp = checkOutputUp.Checked;
            var bCancelOutputs = checkOutputCancel.Checked;
            var bNothing = checkOutputNothing.Checked;

            var nFlags = 0;
            nFlags = BitUtil.UpdateFlag(nFlags, OutputFlag.Shift, bShift);
            nFlags = BitUtil.UpdateFlag(nFlags, OutputFlag.Control, bControl);
            nFlags = BitUtil.UpdateFlag(nFlags, OutputFlag.Alt, bAlt);
            nFlags = BitUtil.UpdateFlag(nFlags, OutputFlag.MouseOut, m_zActiveOutputConfig.MouseClick);
            nFlags = BitUtil.UpdateFlag(nFlags, OutputFlag.Delay, bDelay);
            nFlags = BitUtil.UpdateFlag(nFlags, OutputFlag.CancelActiveOutputs, bCancelOutputs);
            nFlags = BitUtil.UpdateFlag(nFlags, OutputFlag.DoNothing, bNothing);

            nFlags = BitUtil.UpdateFlag(nFlags, OutputFlag.Toggle, bToggle);
            nFlags = BitUtil.UpdateFlag(nFlags, OutputFlag.Repeat, bRepeat);
            nFlags = BitUtil.UpdateFlag(nFlags, OutputFlag.Down, bDown);
            nFlags = BitUtil.UpdateFlag(nFlags, OutputFlag.Up, bUp);
            return new OutputConfig(nFlags, (bDelay || bCancelOutputs || bNothing) ? (byte)0 : m_zActiveOutputConfig.VirtualKey)
            {
                Flags = nFlags,
                Parameter = (bRepeat || bDelay) ? (int)numericUpDownOutputParameter.Value : 0
            };
        }

        /// <summary>
        /// Reconfigures the controls based on the state specified
        /// </summary>
        /// <param name="bCapturing">flag indicating if actively capturing keyboard input</param>
        private void ConfigureControlsForCapture(bool bCapturing)
        {
            menuStripMain.Enabled = !bCapturing;
            panelKeySetup.Enabled = !bCapturing;
            panelKeySetupControls.Enabled = !bCapturing;
            btnStart.Text = bCapturing ? ActionText.Stop.ToString() : ActionText.Start.ToString();
            notifyIcon.Icon = bCapturing? Resources.KeyCapActive : Resources.KeyCapIdle;
            Icon = notifyIcon.Icon;
        }

        /// <summary>
        /// Applications cannot start minimized (at least they can't be switched immediately in the form load event)
        /// </summary>
        private void MinimizeThread()
        {
            Thread.Sleep(1);
            // this is a close to force the icon to the tray and remove the window from the task bar
            this.InvokeAction(() => Close());
        }

        /// <summary>
        /// Flushes all the ini settings
        /// </summary>
        private void FlushIniSettings()
        {
            var zBuilder = new StringBuilder();
            var dictionaryFilenames = new Dictionary<string, object>();
            foreach (var sFile in m_listRecentFiles)
            {
                var sLowerFile = sFile.ToLower();
                if (dictionaryFilenames.ContainsKey(sLowerFile))
                    continue;
                dictionaryFilenames.Add(sLowerFile, null);
                zBuilder.Append(sFile + KeyCapConstants.CharFileSplit);
            }
            m_zIniManager.SetValue(IniSettings.PreviousFiles, zBuilder.ToString());
            m_zIniManager.FlushIniSettings();
        }

        private bool CreateRemapEntryFromActiveConfigs(ref RemapEntry zRemapEntry, RemapEntry updateEntry = null, bool bKeepOutputs = false)
        {
            var zCurrentInputConfig = CreateInputConfigFromUI();
            var zCurrentOutputConfig = CreateOutputConfigFromUI();
            if (0 == zCurrentInputConfig.VirtualKey || !ValidateOutputHasAction(zCurrentOutputConfig))
            {
                return false;
            }
            
            zRemapEntry = new RemapEntry(new InputConfig(zCurrentInputConfig), 
                (bKeepOutputs && updateEntry != null) ? updateEntry.OutputConfigs : new List<OutputConfig> {new OutputConfig(zCurrentOutputConfig)});

            // flip this result for indicator of a good remap entry
            return !IsInputAlreadyDefined(zRemapEntry, updateEntry);
        }

        private bool IsInputAlreadyDefined(RemapEntry zNewRemapEntry, RemapEntry ignoreEntry = null)
        {
            // verify this is not already defined
            foreach (ListViewItem zListItem in listViewKeys.Items)
            {
                var zExistingEntry = (RemapEntry)zListItem.Tag;
                if (ignoreEntry == zExistingEntry 
                    || zNewRemapEntry.GetHashCode() != zExistingEntry.GetHashCode())
                {
                    continue;
                }

                MessageBox.Show(this, "Duplicate inputs are not allowed!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return true;
            }

            return false;
        }

        private bool RetrieveOutputConfigForAppend(RemapEntry zRemapEntry, ref OutputConfig zOutputConfig)
        {
            if (zRemapEntry.OutputConfigCount == KeyCapConstants.MaxOutputs)
            {
                MessageBox.Show(this, "Failed to append item. The maximum number of outputs allowed is {0}.".FormatString(KeyCapConstants.MaxOutputs),
                    "Append Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            zOutputConfig = CreateOutputConfigFromUI();
            if (!ValidateOutputHasAction(zOutputConfig))
            {
                return false;
            }
            return true;
        }

        private bool ValidateInputHasAction()
        {
            if (0 == m_zActiveInputConfig.VirtualKey)
            {
                MessageBox.Show(this, "The input must be configured with a key.",
                    "Input Configuration Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            return true;
        }

        private bool ValidateOutputHasAction(OutputConfig outputConfig)
        {
            if (!outputConfig.IsAssignedAction())
            {
                MessageBox.Show(this, "The output must be configured to perform an action.",
                    "Output Configuration Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            return true;
        }

        #endregion

        #region UIIOConfig

        abstract class UIIOConfig
        {
            public byte VirtualKey { get; set; }

            public UIIOConfig(byte byVirtualKey)
            {
                VirtualKey = byVirtualKey;
            }

            public virtual void Reset()
            {
                VirtualKey = 0;
            }
        }

        class UIInputConfig : UIIOConfig
        {
            public UIInputConfig(byte byVirtualKey) : base(byVirtualKey)
            {
            }
        }

        class UIOutputConfig : UIIOConfig
        {
            public bool MouseClick { get; set; }

            public UIOutputConfig(byte byVirtualKey, bool bMouseClick) : base(byVirtualKey)
            {
                MouseClick = bMouseClick;
            }

            public override void Reset()
            {
                base.Reset();
                MouseClick = false;
            }
        }

        #endregion        
    }
}