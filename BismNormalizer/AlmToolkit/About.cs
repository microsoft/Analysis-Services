using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlmToolkit
{
    partial class About : Form
    {
        private bool _newerVersionAvailable = false;
        private string _latestVersion = null;

        public About()
        {
            InitializeComponent();
        }

        public bool NewerVersionAvailable
        {
            get { return _newerVersionAvailable; }
            set { _newerVersionAvailable = value; }
        }

        public string LatestVersion
        {
            get { return _latestVersion; }
            set { _latestVersion = value; }
        }

        private void About_Load(object sender, EventArgs e)
        {
            this.Text = Utils.AssemblyProduct;
            this.lblProductName.Text = this.Text;

            Version installedVersion = new Version(Utils.AssemblyVersion);
            string installedVersionMajorMinorBuild = String.Format("{0}.{1}.{2}", installedVersion.Major, installedVersion.Minor, installedVersion.Build);

            this.lblProductVersion.Text = String.Format("Installed version {0}", installedVersionMajorMinorBuild);
            if (_newerVersionAvailable)
            {
                linkLatestVersion.Visible = true;
                linkLatestVersion.Text = String.Format("Latest version {0}", _latestVersion);
            }
            this.linkReportIssue.LinkVisited = false;
        }

        private void linkALMTWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkALMTWebsite.LinkVisited = true;
            System.Diagnostics.Process.Start("http://alm-toolkit.com/");
        }

        private void linkHowToUse_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkHowToUse.LinkVisited = true;
            System.Diagnostics.Process.Start("http://alm-toolkit.com/HowToUse");
        }

        private void linkDocumentation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkDocumentation.LinkVisited = true;
            System.Diagnostics.Process.Start("https://github.com/microsoft/Analysis-Services/blob/master/BismNormalizer/Model%20Comparison%20and%20Merging%20for%20Analysis%20Services.pdf");
        }

        private void linkLatestVersion_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLatestVersion.LinkVisited = true;
            System.Diagnostics.Process.Start(Utils.LatestVersionDownloadUrl);
        }

        private void linkReportIssue_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkReportIssue.LinkVisited = true;
            System.Diagnostics.Process.Start("https://github.com/microsoft/Analysis-Services/issues");
        }

        private void linkTwitter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkTwitter.LinkVisited = true;
            System.Diagnostics.Process.Start("https://twitter.com/_christianWade");
        }
    }
}
