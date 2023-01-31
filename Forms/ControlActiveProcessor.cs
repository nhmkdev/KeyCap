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

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KeyCap.Forms
{
    class ControlActiveProcessor
    {
        public Control Self { get; private set; }
        public Action<bool> OnActiveChange { get; set; }
        public List<Control> disabledControls { get; set; }
        public List<Control> enabledControls { get; set; }
        public List<CheckBox> uncheckedControls { get; set; }
        public List<CheckBox> checkedControls { get; set; }

        public ControlActiveProcessor(Control zControl)
        {
            Self = zControl;
            disabledControls = new List<Control>();
            enabledControls = new List<Control>();
            uncheckedControls = new List<CheckBox>();
            checkedControls = new List<CheckBox>();
        }

        public void Process(bool bActive)
        {
            foreach (var zControl in disabledControls)
            {
                zControl.Enabled = !bActive;
            }
            foreach (var zControl in enabledControls)
            {
                zControl.Enabled = bActive;
            }
            if (bActive)
            {
                foreach (var zCheckBox in uncheckedControls)
                {
                    zCheckBox.Checked = false;
                }

                foreach (var zCheckBox in checkedControls)
                {
                    zCheckBox.Checked = true;
                }

                Self.Enabled = true;
            }

            OnActiveChange?.Invoke(bActive);
        }
    }
}
