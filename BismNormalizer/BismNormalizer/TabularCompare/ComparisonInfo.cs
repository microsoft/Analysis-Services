using EnvDTE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace BismNormalizer.TabularCompare
{
    /// <summary>
    /// Information about the comparison. This is serialized/deserialized to/from the BSMN file.
    /// </summary>
    public class ComparisonInfo
    {
        private ConnectionInfo _connectionInfoSource;
        private ConnectionInfo _connectionInfoTarget;
        private OptionsInfo _optionsInfo;
        private SkipSelectionCollection _skipSelectionCollection;
        private int _sourceCompatibilityLevel;
        private int _targetCompatibilityLevel;
        private bool _sourceDirectQuery;
        private bool _targetDirectQuery;
        private bool _promptForDatabaseProcessing;

        /// <summary>
        /// Initializes a new instance of the ComparisonInfo class.
        /// </summary>
        /// <param name="comparisonInfo"></param>
        public ComparisonInfo()
        {
            _connectionInfoSource = new ConnectionInfo();
            _connectionInfoTarget = new ConnectionInfo();
            _optionsInfo = new OptionsInfo();
            _skipSelectionCollection = new SkipSelectionCollection();
        }

        #region Properties

        /// <summary>
        /// Information about the source connection.
        /// </summary>
        public ConnectionInfo ConnectionInfoSource 
        {
            get { return _connectionInfoSource; }
            set { _connectionInfoSource = value; }
        }

        /// <summary>
        /// Information about the target connection.
        /// </summary>
        public ConnectionInfo ConnectionInfoTarget
        {
            get { return _connectionInfoTarget; }
            set { _connectionInfoTarget = value; }
        }

        /// <summary>
        /// Information about the options selected for the comparison.
        /// </summary>
        public OptionsInfo OptionsInfo
        {
            get { return _optionsInfo; }
            set { _optionsInfo = value; }
        }

        /// <summary>
        /// Collection of SkipSelection objects.
        /// </summary>
        public SkipSelectionCollection SkipSelections
        {
            get { return _skipSelectionCollection; }
            set { _skipSelectionCollection = value; }
        }

        /// <summary>
        /// SSAS compatibility level for the source tabular model.
        /// </summary>
        [XmlIgnore()]
        public int SourceCompatibilityLevel => _sourceCompatibilityLevel;

        /// <summary>
        /// SSAS compatibility level for the target tabular model.
        /// </summary>
        [XmlIgnore()]
        public int TargetCompatibilityLevel => _targetCompatibilityLevel;

        /// <summary>
        /// Flag depending on whehter source tabular model is in DirectQuery mode.
        /// </summary>
        [XmlIgnore()]
        public bool SourceDirectQuery => _sourceDirectQuery;

        /// <summary>
        /// Flag depending on whehter target tabular model is in DirectQuery mode.
        /// </summary>
        [XmlIgnore()]
        public bool TargetDirectQuery => _targetDirectQuery;

        /// <summary>
        /// Flag is false if simple database deployment occurs without processing, and without raising the Comparison.DatabaseDeployment event. Typically set for execution from command line.
        /// </summary>
        [XmlIgnore()]
        public bool PromptForDatabaseProcessing
        {
            get { return _promptForDatabaseProcessing; }
            set { _promptForDatabaseProcessing = value; }
        }

        #endregion

        /// <summary>
        /// Deserialize BSMN file into instance of ComparisonInfo.
        /// </summary>
        /// <param name="bsmnFile">BSMN file to be deserialized.</param>
        /// <returns>Deserialized instance of ComparisonInfo.</returns>
        public static ComparisonInfo DeserializeBsmnFile(string bsmnFile)
        {
            if (!File.Exists(bsmnFile))
            {
                throw new FileNotFoundException($"File not found {bsmnFile}.");
            }
            XmlSerializer reader = new XmlSerializer(typeof(ComparisonInfo));
            StreamReader file = new StreamReader(bsmnFile);
            return (ComparisonInfo)reader.Deserialize(file);
        }

        /// <summary>
        /// Finds models' compatibility levels (and preps databases on workspace servers for comparison). This overload to be used when client is not Visual Studio - e.g. command line.
        /// </summary>
        /// <param name="compatibilityLevelSource"></param>
        /// <param name="compatibilityLevelTarget"></param>
        public void InitializeCompatibilityLevels()
        {
            ConnectionInfoSource.InitializeCompatibilityLevel();
            ConnectionInfoTarget.InitializeCompatibilityLevel();

            PopulateDatabaseProperties();
        }

        /// <summary>
        /// Finds model compatibility levels (and preps databases on workspace servers for comparison). This overload to be used when running in Visual Studio. Allows user to cancel if doesn't want to close .bim file(s).
        /// </summary>
        /// <param name="compatibilityLevelSource"></param>
        /// <param name="compatibilityLevelTarget"></param>
        /// <param name="userCancelled"></param>
        public void InitializeCompatibilityLevels(out bool userCancelled)
        {
            //Check if any open bim files that need to be closed
            bool closedSourceBimFile;
            bool closedTargetBimFile;
            CloseProjectBimFiles(out closedSourceBimFile, out closedTargetBimFile, out userCancelled);
            if (userCancelled)
            {
                return;
            }

            //Passing closedSourceBimFile so doesn't run bim file script if user just closed it (more efficient)
            ConnectionInfoSource.InitializeCompatibilityLevel(closedSourceBimFile);
            ConnectionInfoTarget.InitializeCompatibilityLevel(closedTargetBimFile);

            PopulateDatabaseProperties();
        }

        private void PopulateDatabaseProperties()
        {
            _sourceCompatibilityLevel = ConnectionInfoSource.CompatibilityLevel;
            _targetCompatibilityLevel = ConnectionInfoTarget.CompatibilityLevel;

            _sourceDirectQuery = ConnectionInfoSource.DirectQuery;
            _targetDirectQuery = ConnectionInfoTarget.DirectQuery;
        }

        private void CloseProjectBimFiles(out bool closeSourceBimFile, out bool closeTargetBimFile, out bool userCancelled)
        {
            closeSourceBimFile = false;
            closeTargetBimFile = false;
            userCancelled = false;
            List<ProjectItem> projectItemsToClose = new List<ProjectItem>();

            if (ConnectionInfoSource.UseProject)
            {
                foreach (ProjectItem sourceProjectItem in ConnectionInfoSource.Project.ProjectItems)
                {
                    if (sourceProjectItem.Name.EndsWith(".bim") && sourceProjectItem.IsOpen)
                    {
                        projectItemsToClose.Add(sourceProjectItem);
                        closeSourceBimFile = true;
                        break;
                    }
                }
            }

            if (ConnectionInfoTarget.UseProject)
            {
                foreach (ProjectItem targetProjectItem in ConnectionInfoTarget.Project.ProjectItems)
                {
                    if (targetProjectItem.Name.EndsWith(".bim") && targetProjectItem.IsOpen)
                    {
                        // check if user has source/target as the same project
                        if (!(projectItemsToClose.Count == 1 && projectItemsToClose[0].Document.FullName == targetProjectItem.Document.FullName))
                        {
                            projectItemsToClose.Add(targetProjectItem);
                            closeTargetBimFile = true;
                            break;
                        }
                    }
                }
            }
            if (projectItemsToClose.Count > 0)
            {
                string filesToClose = "";
                foreach (ProjectItem projectItemToClose in projectItemsToClose)
                {
                    filesToClose += $"\n- {projectItemToClose.ContainingProject.Name.Replace(".smproj", "")}\\{projectItemToClose.Name}";
                }

                if (MessageBox.Show($"BISM Normalizer needs to close the following file(s) that are\nopen in Visual Studio.  Do you want to continue?{filesToClose}", "BISM Normalizer", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    userCancelled = true;
                }
                else
                {
                    foreach (ProjectItem projectItemToClose in projectItemsToClose)
                    {
                        if (projectItemToClose.Document.Saved)
                        {
                            projectItemToClose.Document.Close(vsSaveChanges.vsSaveChangesNo);
                        }
                        else
                        {
                            projectItemToClose.Document.Close(vsSaveChanges.vsSaveChangesYes);
                        }
                    }
                }
            }
        }
    }
}
