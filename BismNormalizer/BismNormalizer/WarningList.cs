using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using BismNormalizer.TabularCompare.UI;

namespace BismNormalizer
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    ///
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane, 
    /// usually implemented by the package implementer.
    ///
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its 
    /// implementation of the IVsUIElementPane interface.
    /// </summary>
    [Guid("64406588-e0ae-4b2b-b613-47a552d608a3")]
    public class WarningList : ToolWindowPane
    {
        private ValidationOutput _validationOutput;

        public WarningList()
            : base(null)
        {
            this.Caption = Resources.ToolWindowTitle;
            this.BitmapResourceID = 301;
            this.BitmapIndex = 1;

            _validationOutput = new ValidationOutput();
        }

        public override System.Windows.Forms.IWin32Window Window => _validationOutput;
    }
}
