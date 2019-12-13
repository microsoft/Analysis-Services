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
            this.linkReleaseHistory.LinkVisited = false;
        }

        private void linkALMTWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkALMTWebsite.LinkVisited = true;
            System.Diagnostics.Process.Start(Utils.AlmtWebsiteUrl);
        }

        private void linkDocumentation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkDocumentation.LinkVisited = true;
            System.Diagnostics.Process.Start(Utils.DocumentationUrl);
        }

        private void linkLatestVersion_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLatestVersion.LinkVisited = true;
            System.Diagnostics.Process.Start(Utils.LatestVersionDownloadUrl);
        }

        private void linkReleaseHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkReleaseHistory.LinkVisited = true;
            System.Diagnostics.Process.Start(Utils.ReleaseHistoryUrl);
        }

        private void linkTwitter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkTwitter.LinkVisited = true;
            System.Diagnostics.Process.Start("https://twitter.com/_christianWade");
        }
    }
}
