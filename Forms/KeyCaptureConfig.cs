////////////////////////////////////////////////////////////////////////////////
// The MIT License (MIT)
//
// Copyright (c) 2022 Tim Stair
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using KeyCap.Format;
using KeyCap.Settings;
using KeyCap.Util;
using KeyCap.Wrapper;
using Support.IO;
using Support.UI;

namespace KeyCap.Forms
{
    public partial class KeyCaptureConfig
    {
        private readonly ConfigFileManager m_zConfigFileManager = new ConfigFileManager();
        private readonly List<string> m_listRecentFiles = new List<string>();
        private readonly KeyCapInstanceState m_zInstanceState;
        private readonly IniManager m_zIniManager = new IniManager(Application.ProductName, false, true, false);

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

            // load the command line specified file
            if (m_zInstanceState.DefaultConfigFile != null)
            {
                InitOpen(m_zInstanceState.DefaultConfigFile);
            }
        }

        #region Form Events

        private void KeyCaptureConfig_Load(object sender, EventArgs e)
        {
            // must be in the load event to avoid the location being incorrect
            IniManager.RestoreState(this, m_zIniManager.GetValue(Name));

            // setup the various mouse output options
            foreach (OutputConfig.MouseButton sName in Enum.GetValues(typeof(OutputConfig.MouseButton)))
            {
                comboBoxOutMouse.Items.Add(sName);
            }
            comboBoxOutMouse.SelectedIndex = 0;

            // set the notification icon accordingly
            notifyIcon.Icon = Resources.KeyCapIdle;
            Icon = notifyIcon.Icon;

            // populate the previously loaded configurations
            var arrayFiles = m_zIniManager.GetValue(IniSettings.PreviousFiles).Split(new char[] { KeyCapConstants.CharFileSplit }, StringSplitOptions.RemoveEmptyEntries);
            if (0 < arrayFiles.Length)
            {
                foreach (var sFile in arrayFiles)
                {
                    m_listRecentFiles.Add(sFile);
                }
            }

            // initialize capture from command line specified file
            if (0 != m_sLoadedFile.Length && m_zInstanceState.AutoStart)
            {
                btnStart_Click(sender, new EventArgs());
                new Thread(MinimizeThread) { Name = "MinimizeThread" }.Start();
            }
        }

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
                MessageBox.Show(this, "Please specify a string of output characters.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var zCurrentInputConfig = (InputConfig)txtKeyIn.Tag;
            if (null == zCurrentInputConfig)
            {
                ShowKeysNotDefinedError();
                return;
            }

            try
            {
                var zRemapEntry = new RemapEntry(zCurrentInputConfig, CreateOutputConfigFromCharacter(sMacro[0]));
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
                MessageBox.Show(this, "Unfortunately you have specified an unsupported character (at this time)." + ex.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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

        #region Text Capture Handling

        private void txtKeyIn_KeyDown(object sender, KeyEventArgs e)
        {
            //            Console.Out.WriteLine("Key Input: {0} 0x{1}".FormatString(e.KeyCode, e.KeyCode.ToString("x")));
            UpdateTextBox((TextBox)sender, e, new InputConfig((byte)e.KeyCode, e));
        }

        private void txtKeyOut_KeyDown(object sender, KeyEventArgs e)
        {
            //            Console.Out.WriteLine("Key Input: {0} 0x{1}".FormatString(e.KeyCode, e.KeyCode.ToString("x")));
            UpdateTextBox((TextBox)sender, e, new OutputConfig(0, (byte)e.KeyCode, 0, e));
            // delay is toggled off if a key is specified
            checkOutputDelay.Checked = false;
        }

        private void UpdateTextBox<T>(TextBox txtBox, KeyEventArgs e, T config) where T : BaseIOConfig
        {
            txtBox.Text = config.GetDescription();
            txtBox.Tag = config;
            e.Handled = true;
        }

        private void txtKey_Enter(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = Color.LightGoldenrodYellow;
        }

        private void txtKey_Leave(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = SystemColors.Control;
        }

    #endregion

        #region AbstractDirtyForm overrides

        protected override bool SaveFormData(string sFileName)
        {
            var listConfigs = new List<RemapEntry>(listViewKeys.Items.Count);
            foreach (ListViewItem zItem in listViewKeys.Items)
            {
                listConfigs.Add((RemapEntry) zItem.Tag);
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

        #region Control Events

        private void listViewKeys_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnAppend.Enabled = (1 == listViewKeys.SelectedIndices.Count);
            btnAppendExtra.Enabled = (1 == listViewKeys.SelectedIndices.Count);
            btnRemove.Enabled = (0 < listViewKeys.SelectedIndices.Count);
        }

        private void listViewKeys_Resize(object sender, EventArgs e)
        {
            ListViewAssist.ResizeColumnHeaders(listViewKeys);
        }

        private void comboBoxMouseOut_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBoxOutMouse.SelectedIndex == 0)
            {
                txtKeyOut.Text = string.Empty;
                txtKeyOut.Tag = null;
            }
            else
            {
                var zOutputConfig = new OutputConfig(
                    (int)OutputConfig.OutputFlag.MouseOut,
                    (byte)(OutputConfig.MouseButton)comboBoxOutMouse.SelectedItem);
                txtKeyOut.Text = zOutputConfig.GetDescription();
                txtKeyOut.Tag = zOutputConfig;
            }
        }

        private void checkOutputDelay_CheckedChanged(object sender, EventArgs e)
        {
            if (checkOutputDelay.Checked)
            {
                numericUpDownDelay_ValueChanged(sender, e);
            }
        }

        private void numericUpDownDelay_ValueChanged(object sender, EventArgs e)
        {
            var nFlag = 0;
            if (checkOutputDelay.Checked)
            {
                nFlag = (int) OutputConfig.OutputFlag.Delay;
            }
            // TODO: support other types that use the param this way...
            if (nFlag == 0)
            {
                return;
            }

            var zOutputConfig = new OutputConfig(
                nFlag,
                0,
                (int) numericUpDownOutputParameter.Value);

            txtKeyOut.Text = zOutputConfig.GetDescription();
            txtKeyOut.Tag = zOutputConfig;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            InputConfig zInputConfig = null;
            OutputConfig zOutputConfig = null;
            RemapEntry zRemapEntry = null;
            if (!RetrieveAddConfigs(ref zInputConfig, ref zOutputConfig, ref zRemapEntry)) return;
            AddRemapEntryToListView(zRemapEntry, true);
        }

        private void AddRemapEntryToListView(RemapEntry zRemapEntry, bool bMarkDirty)
        {
            var zItem = new ListViewItem(new string[]
            {
                zRemapEntry.GetInputString(),
                zRemapEntry.GetOutputString()
            })
            {
                Tag = zRemapEntry,
                ToolTipText = zRemapEntry.GetOutputString()
            };
            listViewKeys.Items.Add(zItem);
            listViewKeys.SelectedItems.Clear();
            zItem.Selected = true;
            if (bMarkDirty)
            {
                MarkDirty();
            }
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
            if (!RetrieveAppendConfigs(ref zOutputConfig, zRemapEntry)) return;

            zRemapEntry.AppendOutputConfig(zOutputConfig);
            zItem.SubItems[1].Text = zRemapEntry.GetOutputString();
            MarkDirty();
            txtKeyOut.Focus(); // restore focus to the output
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (0 == listViewKeys.SelectedItems.Count)
            {
                return;
            }

            foreach (ListViewItem zItem in listViewKeys.SelectedItems)
            {
                listViewKeys.Items.Remove(zItem);
            }
            MarkDirty();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (0 == listViewKeys.Items.Count)
            {
                return; // no keys, no point!
            }
            if(btnStart.Text.Equals(ActionText.Stop.ToString()))
            {
                KeyCaptureLib.Shutdown();
                ConfigureControls(false);
            }
            else if(btnStart.Text.Equals(ActionText.Start.ToString()))
            {
                InitSave(false);
                if (!Dirty)
                {
                    ConfigureControls(true);
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
                            Console.WriteLine("Error: " + eReturn);
                            ConfigureControls(false);
                            break;
                    }
                }
            }
        }

        private void checkOutputToggle_CheckedChanged(object sender, EventArgs e)
        {
            checkOutputUp.Checked = checkOutputToggle.Checked;
            checkOutputDown.Checked = checkOutputToggle.Checked;

            checkOutputUp.Enabled = !checkOutputToggle.Checked;
            checkOutputDown.Enabled = !checkOutputToggle.Checked;
            checkOutputRepeat.Enabled = !checkOutputToggle.Checked;
            checkOutputDelay.Enabled = !checkOutputToggle.Checked;
            checkOutputNothing.Enabled = !checkOutputToggle.Checked;

            checkOutputAlt.Enabled = !checkOutputToggle.Checked;
            checkOutputShift.Enabled = !checkOutputToggle.Checked;
            checkOutputControl.Enabled = !checkOutputToggle.Checked;
            if (checkOutputToggle.Checked)
            {
                checkOutputAlt.Checked = 
                checkOutputShift.Checked = 
                checkOutputControl.Checked =
                checkOutputRepeat.Checked =
                checkOutputDelay.Checked =
                checkOutputNothing.Checked = false;
            }
        }

        private void checkOutputDoNothing_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxOutMouse.Enabled = 
            checkOutputAlt.Enabled =
            checkOutputShift.Enabled = 
            checkOutputControl.Enabled = 
            checkOutputUp.Enabled = 
            checkOutputDown.Enabled = 
            checkOutputToggle.Enabled = 
            checkOutputDelay.Enabled =
            checkOutputRepeat.Enabled =
            checkOutputCancel.Enabled =
                !checkOutputNothing.Checked;
            if (checkOutputNothing.Checked)
            {
                var zOutputConfig = new OutputConfig((int)OutputConfig.OutputFlag.DoNothing, 0);
                txtKeyOut.Text = zOutputConfig.GetDescription();
                txtKeyOut.Tag = zOutputConfig;
            }
        }

        private void checkOutputCancel_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxOutMouse.Enabled =
            checkOutputAlt.Enabled =
            checkOutputShift.Enabled =
            checkOutputControl.Enabled =
            checkOutputUp.Enabled =
            checkOutputDown.Enabled =
            checkOutputToggle.Enabled =
            checkOutputDelay.Enabled =
            checkOutputRepeat.Enabled =
            checkOutputNothing.Enabled = 
                !checkOutputCancel.Checked;
            if (checkOutputCancel.Checked)
            {
                var zOutputConfig = new OutputConfig((int)OutputConfig.OutputFlag.CancelActiveOutputs, 0);
                txtKeyOut.Text = zOutputConfig.GetDescription();
                txtKeyOut.Tag = zOutputConfig;
            }
        }

        #endregion

        #region Support Methods

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

        /// <summary>
        /// Gets the flags byte based on the definition and the type of input/output
        /// </summary>
        /// <param name="zInputConfig">the io definition</param>
        /// <param name="eFlag">the type of io</param>
        /// <returns>New flags value based on the settings of the ui (any prior flags are lost)</returns>
        private InputConfig UpdateInputFlags(InputConfig zInputConfig)
        {
            var bAlt = checkInputAlt.Checked;
            var bControl = checkInputControl.Checked;
            var bShift = checkInputShift.Checked;

            var nFlags = 0;
            nFlags = BitUtil.UpdateFlag(nFlags, bAlt, InputConfig.InputFlag.Alt);
            nFlags = BitUtil.UpdateFlag(nFlags, bControl, InputConfig.InputFlag.Control);
            nFlags = BitUtil.UpdateFlag(nFlags, bShift, InputConfig.InputFlag.Shift);
            zInputConfig.Flags = nFlags;
            return zInputConfig;
        }

        private OutputConfig UpdateOutputFlags(OutputConfig zOutputConfig)
        {
            if (zOutputConfig.IsFlaggedAs(OutputConfig.OutputFlag.DoNothing) 
                || zOutputConfig.IsFlaggedAs(OutputConfig.OutputFlag.CancelActiveOutputs))
            {
                return zOutputConfig;
            }


            // get the flags from the check boxes (always, both mouse and keyboard support them in some fashion)
            var bAlt = checkOutputAlt.Checked;
            var bControl = checkOutputControl.Checked;
            var bShift = checkOutputShift.Checked;
            var bToggle = checkOutputToggle.Checked;
            var bRepeat = checkOutputRepeat.Checked;
            var bDown = checkOutputDown.Checked;
            var bUp = checkOutputUp.Checked;

            var nFlags = 0;
            nFlags = BitUtil.UpdateFlag(nFlags, bShift, OutputConfig.OutputFlag.Shift);
            nFlags = BitUtil.UpdateFlag(nFlags, bControl, OutputConfig.OutputFlag.Control);
            nFlags = BitUtil.UpdateFlag(nFlags, bAlt, OutputConfig.OutputFlag.Alt);
            nFlags = BitUtil.UpdateFlag(nFlags, zOutputConfig.IsFlaggedAs(OutputConfig.OutputFlag.MouseOut), OutputConfig.OutputFlag.MouseOut);
            nFlags = BitUtil.UpdateFlag(nFlags, zOutputConfig.IsFlaggedAs(OutputConfig.OutputFlag.Delay), OutputConfig.OutputFlag.Delay);

            nFlags = BitUtil.UpdateFlag(nFlags, bToggle, OutputConfig.OutputFlag.Toggle);
            nFlags = BitUtil.UpdateFlag(nFlags, bRepeat, OutputConfig.OutputFlag.Repeat);
            nFlags = BitUtil.UpdateFlag(nFlags, bDown, OutputConfig.OutputFlag.Down);
            nFlags = BitUtil.UpdateFlag(nFlags, bUp, OutputConfig.OutputFlag.Up);
            zOutputConfig.Flags = nFlags;

            if (bRepeat)
            {
                zOutputConfig.Parameter = (int)numericUpDownOutputParameter.Value;
            }

            return zOutputConfig;
        }

        /// <summary>
        /// Reconfigures the controls based on the state specified
        /// </summary>
        /// <param name="bCapturing">flag indicating if actively capturing keyboard input</param>
        private void ConfigureControls(bool bCapturing)
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

        private bool RetrieveAddConfigs(ref InputConfig zInputConfig, ref OutputConfig zOutputConfig, ref RemapEntry zRemapEntry)
        {
            var zCurrentInputConfig = (InputConfig)txtKeyIn.Tag;
            var zCurrentOutputConfig = (OutputConfig)txtKeyOut.Tag;
            if (null == zCurrentInputConfig || null == zCurrentOutputConfig)
            {
                MessageBox.Show(this, "Please specify both an input and output key.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // build cloned configs based on the ui state
            zInputConfig = UpdateInputFlags(new InputConfig(zCurrentInputConfig));
            zOutputConfig = UpdateOutputFlags(new OutputConfig(zCurrentOutputConfig));

            if (!ValidateOutputHasAction(zOutputConfig))
            {
                return false;
            }

            zRemapEntry = new RemapEntry(zInputConfig, zOutputConfig);

            // flip this result for indicator of a good remap entry
            return !IsInputAlreadyDefined(zRemapEntry);
        }

        private bool IsInputAlreadyDefined(RemapEntry zNewRemapEntry)
        {
            // verify this is not already defined
            foreach (ListViewItem zListItem in listViewKeys.Items)
            {
                var zKeyOldDef = (RemapEntry)zListItem.Tag;
                if (zNewRemapEntry.GetHashCode() != zKeyOldDef.GetHashCode())
                {
                    continue;
                }

                MessageBox.Show(this, "Duplicate inputs are not allowed!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return true;
            }

            return false;
        }

        private bool RetrieveAppendConfigs(ref OutputConfig zOutputConfig, RemapEntry zRemapEntry)
        {
            var zCurrentOutputConfig = (OutputConfig)txtKeyOut.Tag;
            if (null == zCurrentOutputConfig)
            {
                MessageBox.Show(this, "Please specify an output key.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (zRemapEntry.OutputConfigCount == KeyCapConstants.MaxOutputs)
            {
                MessageBox.Show(this, "Failed to append item. The maximum number of outputs allowed is {0}.".FormatString(KeyCapConstants.MaxOutputs),
                    "Append Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            zOutputConfig = UpdateOutputFlags(new OutputConfig(zCurrentOutputConfig));
            if (!ValidateOutputHasAction(zOutputConfig))
            {
                return false;
            }
            return true;
        }

        private bool ValidateOutputHasAction(OutputConfig zOutputConfig)
        {
            if (!zOutputConfig.IsAssignedAction())
            {
                MessageBox.Show(this, "The output must be configured to perform an action.",
                    "Output Configuration Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            return true;
        }

        private OutputConfig CreateOutputConfigFromCharacter(char cInput)
        {
            var bShift = false;
            var byteKey = GetKeysByte(cInput, ref bShift);
            var nFlags = (bShift ? (int)OutputConfig.OutputFlag.Shift : 0) |
                (int)OutputConfig.OutputFlag.Down | (int)OutputConfig.OutputFlag.Up;
            return new OutputConfig(nFlags, byteKey);
        }

        private byte GetKeysByte(char cInput, ref bool bShift)
        {
            bShift = char.IsUpper(cInput);
            try
            {
                return (byte)(Keys)Enum.Parse(typeof(Keys), cInput.ToString(), true);
            }
            catch (Exception)
            {
                // HACK
                // ignore and try the switch
            }
#warning HACK This is super limited to a very specific keyboard
            switch (cInput)
            {
                case ' ':
                    return (byte)Keys.Space;
                case '!':
                    bShift = true;
                    return (byte)Keys.D1;
                case '@':
                    bShift = true;
                    return (byte)Keys.D2;
                case '#':
                    bShift = true;
                    return (byte)Keys.D3;
                case '$':
                    bShift = true;
                    return (byte)Keys.D4;
                case '%':
                    bShift = true;
                    return (byte)Keys.D5;
                case '^':
                    bShift = true;
                    return (byte)Keys.D6;
                case '&':
                    bShift = true;
                    return (byte)Keys.D7;
                case '*':
                    bShift = true;
                    return (byte)Keys.D8;
                case '(':
                    bShift = true;
                    return (byte)Keys.D9;
                case ')':
                    bShift = true;
                    return (byte)Keys.D0;
                case '~':
                    bShift = true;
                    return (byte)Keys.Oemtilde;
                case '{':
                    bShift = true;
                    return (byte)Keys.OemOpenBrackets;
                case '}':
                    bShift = true;
                    return (byte)Keys.Oem6;
                case '|':
                    bShift = true;
                    return (byte)Keys.Oem5;
                case ':':
                    bShift = true;
                    return (byte)Keys.Oem1;
                case '"':
                    bShift = true;
                    return (byte)Keys.Oem7;
                case '<':
                    bShift = true;
                    return (byte)Keys.Oemcomma;
                case '>':
                    bShift = true;
                    return (byte)Keys.OemPeriod;
                case '?':
                    bShift = true;
                    return (byte)Keys.OemQuestion;
            }

            throw new Exception("Unsupported character: " + cInput);
        }

        private void ShowKeysNotDefinedError()
        {
            MessageBox.Show(this, "Please specify both an input and output key.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        #endregion

    }
}