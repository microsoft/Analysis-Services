using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BismNormalizer.TabularCompare.Core;

namespace BismNormalizer.TabularCompare.UI
{
    public partial class Options : Form
    {
        private ComparisonInfo _comparisonInfo;
        private float _dpiScaleFactor;

        public Options()
        {
            InitializeComponent();
        }

        private void Options_Load(object sender, EventArgs e)
        {
            if (_dpiScaleFactor > 1)
            {
                float dpiScaleFactorFudged = _dpiScaleFactor * HighDPIUtils.PrimaryFudgeFactor;
                //DPI
                this.Scale(new SizeF(dpiScaleFactorFudged * (_dpiScaleFactor > 1.7 ? 1 : HighDPIUtils.SecondaryFudgeFactor), dpiScaleFactorFudged * HighDPIUtils.SecondaryFudgeFactor));
                this.Width = Convert.ToInt32(this.Width * dpiScaleFactorFudged);
                foreach (Control control in HighDPIUtils.GetChildInControl(this)) //.OfType<Button>())
                {
                    if (control is GroupBox || control is Button)
                    {
                        control.Font = new Font(control.Font.FontFamily,
                                                control.Font.Size * dpiScaleFactorFudged * HighDPIUtils.SecondaryFudgeFactor,
                                                control.Font.Style);
                    }
                }
                this.cboProcessingOption.Left = label1.Right + Convert.ToInt32(12 * dpiScaleFactorFudged);
            }

            this.KeyPreview = true;
            chkMeasureDependencies.Text = "Display warnings for measure dependencies (DAX\nreference to missing measure/column)";

            chkPerspectives.Checked = _comparisonInfo.OptionsInfo.OptionPerspectives;
            chkMergePerspectives.Checked = _comparisonInfo.OptionsInfo.OptionMergePerspectives;
            chkCultures.Checked = _comparisonInfo.OptionsInfo.OptionCultures;
            chkMergeCultures.Checked = _comparisonInfo.OptionsInfo.OptionMergeCultures;
            chkRoles.Checked = _comparisonInfo.OptionsInfo.OptionRoles;
            //chkActions.Checked = _comparisonInfo.OptionsInfo.OptionActions;
            chkPartitions.Checked = _comparisonInfo.OptionsInfo.OptionPartitions;
            chkRetainPartitions.Checked = _comparisonInfo.OptionsInfo.OptionRetainPartitions;
            chkMeasureDependencies.Checked = _comparisonInfo.OptionsInfo.OptionMeasureDependencies;
            string processingOption = _comparisonInfo.OptionsInfo.OptionProcessingOption.ToString();
            cboProcessingOption.Text = processingOption == "DoNotProcess" ? "Do Not Process" : processingOption;
            //chkTransaction.Checked = _comparisonInfo.OptionsInfo.OptionTransaction;
            chkAffectedTables.Checked = _comparisonInfo.OptionsInfo.OptionAffectedTables;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _comparisonInfo.OptionsInfo.OptionPerspectives = chkPerspectives.Checked;
            _comparisonInfo.OptionsInfo.OptionMergePerspectives = chkMergePerspectives.Checked;
            _comparisonInfo.OptionsInfo.OptionCultures = chkCultures.Checked;
            _comparisonInfo.OptionsInfo.OptionMergeCultures = chkMergeCultures.Checked;
            _comparisonInfo.OptionsInfo.OptionRoles = chkRoles.Checked;
            //_comparisonInfo.OptionsInfo.OptionActions = chkActions.Checked;
            _comparisonInfo.OptionsInfo.OptionActions = false;
            _comparisonInfo.OptionsInfo.OptionPartitions = chkPartitions.Checked;
            _comparisonInfo.OptionsInfo.OptionRetainPartitions = chkRetainPartitions.Checked;
            _comparisonInfo.OptionsInfo.OptionMeasureDependencies = chkMeasureDependencies.Checked;
            _comparisonInfo.OptionsInfo.OptionProcessingOption = (ProcessingOption)Enum.Parse(typeof(ProcessingOption), cboProcessingOption.Text.Replace(" ", ""));
            //_comparisonInfo.OptionsInfo.OptionTransaction = chkTransaction.Checked;
            _comparisonInfo.OptionsInfo.OptionTransaction = false;
            _comparisonInfo.OptionsInfo.OptionAffectedTables = chkAffectedTables.Checked;

            _comparisonInfo.OptionsInfo.Save();
        }

        private void chkPerspectives_CheckedChanged(object sender, EventArgs e)
        {
            chkMergePerspectives.Enabled = chkPerspectives.Checked;
        }

        private void chkCultures_CheckedChanged(object sender, EventArgs e)
        {
            chkMergeCultures.Enabled = chkCultures.Checked;
        }

        private void Options_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.Shift && e.KeyCode == Keys.D)
            {
                if (MessageBox.Show($"Are you sure you want to toggle 192 Device DPI from optimized for {(Settings.Default.OptionHighDpiLocal ? "local" : "Remote Desktop")} to {(Settings.Default.OptionHighDpiLocal ? "Remote Desktop" : "local")}?", "BISM Normalizer", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Settings.Default.OptionHighDpiLocal = !Settings.Default.OptionHighDpiLocal;
                }
            }
        }

        public ComparisonInfo ComparisonInfo
        {
            get { return _comparisonInfo; }
            set { _comparisonInfo = value; }
        }

        public float DpiScaleFactor
        {
            get { return _dpiScaleFactor; }
            set { _dpiScaleFactor = value; }
        }
    }
}
