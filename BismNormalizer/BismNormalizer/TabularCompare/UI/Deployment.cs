using BismNormalizer.TabularCompare.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BismNormalizer.TabularCompare.UI
{
    public partial class Deployment : Form
    {
        private Comparison _comparison;
        private ComparisonInfo _comparisonInfo;
        private float _dpiScaleFactor;
        private DeploymentStatus _deployStatus;
        private const string _deployRowWorkItem = "Deploy metadata";
        ProcessingErrorMessage _errorMessageForm;

        public Deployment()
        {
            InitializeComponent();
        }

        private void Deploy_Load(object sender, EventArgs e)
        {
            try
            {
                //DPI
                if (_dpiScaleFactor > 1)
                {
                    float fudgedDpiScaleFactor = _dpiScaleFactor * HighDPIUtils.PrimaryFudgeFactor;

                    this.Scale(new SizeF(fudgedDpiScaleFactor, fudgedDpiScaleFactor));
                    picStatus.Scale(new SizeF(fudgedDpiScaleFactor, fudgedDpiScaleFactor));
                    gridProcessing.Scale(new SizeF(fudgedDpiScaleFactor * HighDPIUtils.SecondaryFudgeFactor, fudgedDpiScaleFactor * HighDPIUtils.SecondaryFudgeFactor));
                    this.Font = new Font(this.Font.FontFamily,
                                     this.Font.Size * fudgedDpiScaleFactor,
                                     this.Font.Style);
                    foreach (Control control in HighDPIUtils.GetChildInControl(this)) //.OfType<Button>())
                    {
                        if (control is DataGridView || control is Button)
                        {
                            control.Font = new Font(control.Font.FontFamily,
                                                    control.Font.Size * fudgedDpiScaleFactor,
                                                    control.Font.Style);
                        }
                    }
                    foreach (DataGridViewColumn col in gridProcessing.Columns)
                    {
                        col.Width = Convert.ToInt32(col.Width * fudgedDpiScaleFactor * 1.5f);
                    }
                    HighDPIUtils.ScaleStreamedImageListByDpi(DeployImageList);
                }

                this.KeyPreview = true;
                AddRow(_deployRowWorkItem, "Deploying ...");
                _deployStatus = DeploymentStatus.Deploying;

                _comparison.PasswordPrompt += HandlePasswordPrompt;
                _comparison.BlobKeyPrompt += HandleBlobPrompt;
                _comparison.DeploymentMessage += HandleDeploymentMessage;
                _comparison.DeploymentComplete += HandleDeploymentComplete;

                btnStopProcessing.Enabled = false;
                btnClose.Enabled = false;
                _errorMessageForm = new ProcessingErrorMessage();

                ProcessingTableCollection tablesToProcess = _comparison.GetTablesToProcess();
                foreach (ProcessingTable table in tablesToProcess)
                {
                    AddRow(table.Name, "Processing in progress ...");
                }
                if (tablesToProcess.Count > 0)
                {
                    btnStopProcessing.Enabled = true;
                    lblStatus.Text = "Processing ...";
                }
                _comparison.DatabaseDeployAndProcess(tablesToProcess);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "BISM Normalizer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Set status methods

        private void HandleDeploymentMessage(object sender, DeploymentMessageEventArgs e)
        {
            foreach (DataGridViewRow row in gridProcessing.Rows)
            {
                if (Convert.ToString(row.Cells[1].Value) == e.WorkItem)
                {
                    row.Cells[0].Value = DeployImageList.Images[Convert.ToInt32(e.DeploymentStatus)];
                    row.Cells[2].Value = e.Message;
                }
            }
        }

        private void HandleDeploymentComplete(object sender, DeploymentCompleteEventArgs e)
        {
            switch (e.DeploymentStatus)
            {
                case DeploymentStatus.Success:
                    picStatus.Image = (_dpiScaleFactor > 1 ? HighDPIUtils.ScaleByDpi(Resources.ProgressSuccess) : Resources.ProgressSuccess);
                    lblStatus.Text = "Success";
                    _deployStatus = DeploymentStatus.Success;
                    break;

                case DeploymentStatus.Cancel:
                    picStatus.Image = (_dpiScaleFactor > 1 ? HighDPIUtils.ScaleByDpi(Resources.ProgressCancel) : Resources.ProgressCancel);
                    lblStatus.Text = "Cancelled";
                    _deployStatus = DeploymentStatus.Cancel;
                    break;

                case DeploymentStatus.Error:
                    SetErrorStatus(e.ErrorMessage);
                    break;

                default:
                    break;
            }

            btnStopProcessing.Enabled = false;
            btnClose.Enabled = true;
            btnClose.Select();
        }

        private delegate void SetErrorStatusDelegate(string errorMessage);
        private void SetErrorStatus(string errorMessage)
        {
            //might not be on UI thread
            if (this.InvokeRequired || _errorMessageForm.InvokeRequired)
            {
                SetErrorStatusDelegate SetErrorStatusCallback = new SetErrorStatusDelegate(SetErrorStatus);
                this.Invoke(SetErrorStatusCallback, new object[] { errorMessage });
            }
            else
            {
                picStatus.Image = (_dpiScaleFactor > 1 ? HighDPIUtils.ScaleByDpi(Resources.ProgressError) : Resources.ProgressError);
                lblStatus.Text = "Error";
                _deployStatus = DeploymentStatus.Error;

                if (!String.IsNullOrEmpty(errorMessage) && String.IsNullOrEmpty(_errorMessageForm.ErrorMessage)) //just in case already shown
                {
                    _errorMessageForm.ErrorMessage = errorMessage;
                    _errorMessageForm.StartPosition = FormStartPosition.CenterParent;
                    _errorMessageForm.DpiScaleFactor = _dpiScaleFactor;
                    _errorMessageForm.ShowDialog();
                }

                btnStopProcessing.Enabled = false;
                btnClose.Enabled = true;
                btnClose.Select();
            }
        }

        #endregion

        private void AddRow(string workItem, string status)
        {
            DataGridViewRow row = (DataGridViewRow)gridProcessing.RowTemplate.Clone();
            row.CreateCells(gridProcessing);
            row.Cells[0].Value = DeployImageList.Images[0];
            row.Cells[1].Value = workItem;
            row.Cells[2].Value = status;
            int rowIndex = gridProcessing.Rows.Add(row);
            gridProcessing.AutoResizeRow(rowIndex, DataGridViewAutoSizeRowMode.AllCells);
        }

        private void btnStopProcessing_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to attempt to stop processing?", "BISM Normalizer", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            _comparison.StopProcessing();
            btnStopProcessing.Enabled = false;
        }

        private void HandlePasswordPrompt(object sender, PasswordPromptEventArgs e)
        {
            ImpersonationCredentials credentialsForm = new ImpersonationCredentials();
            credentialsForm.AuthenticationKind = e.AuthenticationKind;
            credentialsForm.ConnectionName = e.DataSourceName;
            credentialsForm.Username = e.Username;
            credentialsForm.StartPosition = FormStartPosition.CenterParent;
            credentialsForm.Scale(new SizeF(_dpiScaleFactor, _dpiScaleFactor));
            credentialsForm.Font = new Font(credentialsForm.Font.FontFamily,
                                            credentialsForm.Font.Size * _dpiScaleFactor,
                                            credentialsForm.Font.Style);
            credentialsForm.DpiScaleFactor = _dpiScaleFactor;
            credentialsForm.ShowDialog();
            if (credentialsForm.DialogResult == DialogResult.OK)
            {
                e.Username = credentialsForm.Username;
                e.Password = credentialsForm.Password;
                e.PrivacyLevel = credentialsForm.PrivacyLevel;
                e.UserCancelled = false;
            }
            else
            {
                e.Password = null;
                e.UserCancelled = true;
            }
        }

        private void HandleBlobPrompt(object sender, BlobKeyEventArgs e)
        {
            BlobCredentials credentialsForm = new BlobCredentials();
            credentialsForm.AuthenticationKind = e.AuthenticationKind;
            credentialsForm.ConnectionName = e.DataSourceName;
            credentialsForm.StartPosition = FormStartPosition.CenterParent;
            credentialsForm.Scale(new SizeF(_dpiScaleFactor, _dpiScaleFactor));
            credentialsForm.Font = new Font(credentialsForm.Font.FontFamily,
                                            credentialsForm.Font.Size * _dpiScaleFactor,
                                            credentialsForm.Font.Style);
            credentialsForm.DpiScaleFactor = _dpiScaleFactor;
            credentialsForm.ShowDialog();
            if (credentialsForm.DialogResult == DialogResult.OK)
            {
                e.AccountKey = credentialsForm.AccountKey;
                e.PrivacyLevel = credentialsForm.PrivacyLevel;
                e.UserCancelled = false;
            }
            else
            {
                e.AccountKey = null;
                e.UserCancelled = true;
            }
        }

        public Comparison Comparison
        {
            get { return _comparison; }
            set { _comparison = value; }
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            switch (_deployStatus)
            {
                case DeploymentStatus.Success:
                    this.DialogResult = DialogResult.OK;
                    break;
                default:
                    this.DialogResult = DialogResult.Abort;
                    break;
            }
            this.Close();
        }

        private void Deploy_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = (btnClose.Enabled == false);
        }
    }
}
