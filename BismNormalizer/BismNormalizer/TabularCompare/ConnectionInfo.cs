using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Principal;
using Microsoft.AnalysisServices;
using EnvDTE;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;

namespace BismNormalizer.TabularCompare
{
    /// <summary>
    /// Information about a connection. This is serialized/deserialized to/from the BSMN file.
    /// </summary>
    public class ConnectionInfo
    {
        #region Private Variables

        private bool _useProject = false;
        private string _serverName;
        private string _databaseName;
        private string _projectName;
        private string _projectFile;
        private int _compatibilityLevel;
        private string _dataSourceVersion;
        private bool _directQuery;
        private string _bimFileFullName;
        private EnvDTE.Project _project;
        private string _deploymentServerName;
        private string _deploymentServerDatabase;
        private string _deploymentServerCubeName;
        private DirectoryInfo _projectDirectoryInfo;
        private bool _credsProvided = false;
        private string _username;
        private string _password;
        private bool _workspaceServerProvided = false;
        private string _workspaceServer;

        #endregion

        /// <summary>
        /// Initializes a new instance of the ConnectionInfo class.
        /// </summary>
        public ConnectionInfo()
        {
        }

        /// <summary>
        /// A Boolean specifying whether the connection represents a project in Visual Studio or a database on a server.
        /// </summary>
        public bool UseProject
        {
            get { return _useProject; }
            set { _useProject = value; }
        }

        /// <summary>
        /// Name of the server on which the tabular model resides.
        /// </summary>
        public string ServerName
        {
            get { return _serverName; }
            set { _serverName = value; }
        }

        /// <summary>
        /// The name of the database for the connection.
        /// </summary>
        public string DatabaseName
        {
            get { return _databaseName; }
            set { _databaseName = value; }
        }

        /// <summary>
        /// The name of the project for the connection.
        /// </summary>
        public string ProjectName
        {
            get { return _projectName; }
            set { _projectName = value; }
        }

        /// <summary>
        /// The full path and name of the project file. Used for running in command-line mode.
        /// </summary>
        public string ProjectFile
        {
            get { return _projectFile; }
            set { _projectFile = value; }
        }

        /// <summary>
        /// Compatibility level for the connection.
        /// </summary>
        [XmlIgnore()]
        public int CompatibilityLevel => _compatibilityLevel;

        /// <summary>
        /// Default data source version for the connection.
        /// </summary>
        [XmlIgnore()]
        public string DataSourceVersion => _dataSourceVersion;

        /// <summary>
        /// A Boolean specifying whether the tabular model for the connection is running in DirectQuery mode.
        /// </summary>
        [XmlIgnore()]
        public bool DirectQuery => _directQuery;

        /// <summary>
        /// Project running in Visual Studio.
        /// </summary>
        [XmlIgnore()]
        public EnvDTE.Project Project
        {
            get { return _project; }
            set { _project = value; }
        }

        /// <summary>
        /// Full path to the BIM file for the project.
        /// </summary>
        [XmlIgnore()]
        public string BimFileFullName => _bimFileFullName;

        /// <summary>
        /// The deployment server from the project file.
        /// </summary>
        [XmlIgnore()]
        public string DeploymentServerName => _deploymentServerName;

        /// <summary>
        /// The deployment database from the project file.
        /// </summary>
        [XmlIgnore()]
        public string DeploymentServerDatabase => _deploymentServerDatabase;

        /// <summary>
        /// The deployment cube from the project file.
        /// </summary>
        [XmlIgnore()]
        public string DeploymentServerCubeName => _deploymentServerCubeName;

        /// <summary>
        /// Use credentials are provided for command line execution.
        /// </summary>
        [XmlIgnore()]
        public bool CredsProvided
        {
            get { return _credsProvided; }
            set { _credsProvided = value; }
        }

        /// <summary>
        /// Username for command line execution.
        /// </summary>
        [XmlIgnore()]
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        /// <summary>
        /// Password for command line execution.
        /// </summary>
        [XmlIgnore()]
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        /// <summary>
        /// Flag depending on whether workspace server was provided. Used for command line mode/automated build.
        /// </summary>
        [XmlIgnore()]
        public bool WorkspaceServerProvided
        {
            get { return _workspaceServerProvided; }
            set { _workspaceServerProvided = value; }
        }

        /// <summary>
        /// Workspace server name for when WorkspaceServerProvided = true. Used for command line mode/automated build.
        /// </summary>
        [XmlIgnore()]
        public string WorkspaceServer
        {
            get { return _workspaceServer; }
            set { _workspaceServer = value; }
        }

        private void ReadSettingsFile()
        {
            FileInfo[] files = _projectDirectoryInfo.GetFiles("*.settings", SearchOption.TopDirectoryOnly);
            FileInfo settingsFile = null;

            string currentUserName = WindowsIdentity.GetCurrent().Name;
            int startPos1 = currentUserName.IndexOf("\\") + 1;
            if (startPos1 != -1)
            {
                currentUserName = currentUserName.Substring(startPos1);
            }

            foreach (FileInfo file in files)
            {
                if (file.Name.ToUpper().Contains(currentUserName.ToUpper()))
                {
                    settingsFile = file;
                    break;
                }
            }

            if (settingsFile == null)
            {
                if (_project == null)
                {
                    //Probably running in command-line mode
                    if (String.IsNullOrEmpty(_serverName) || String.IsNullOrEmpty(_databaseName))
                    {
                        throw new ConnectionException($"Project {_projectName} Server Name and/or Database Name not populated.\nGenerate a new BSMN in Visual Studio.");
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    throw new ConnectionException($"Could not read workspace settings file for Project {_projectName}.\nGenerate a new settings file by opening the .bim file in Visual Studio.");
                }
            }

            /* can't use xmlreader or xmldoc because throws error saying "Data at the root level is invalid"
            XmlDocument document = new XmlDocument();
            document.Load(settingsFile.FullName);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);
            nsmgr.AddNamespace("myns1", "http://schemas.microsoft.com/myns1");

            XmlNode serverNameNode = document.SelectSingleNode("//myns1:ServerName", nsmgr);
            if (serverNameNode == null) throw new ConnectionException("Could not read workspace server name from settings file for Project " + _projectName);
            _serverName = serverNameNode.InnerText;

            XmlNode databaseNameNode = document.SelectSingleNode("//myns1:DatabaseName", nsmgr);
            if (databaseNameNode == null) throw new ConnectionException("Could not read workspace database name from settings file for Project " + _projectName);
            _databaseName = databaseNameNode.InnerText;
            */

            using (StreamReader settingsFileStreamReader = settingsFile.OpenText())
            {
                string settingsFileContents = settingsFileStreamReader.ReadToEnd();

                int startPos = settingsFileContents.IndexOf("<ServerName>");
                int endPos = settingsFileContents.IndexOf("</ServerName>");
                if (startPos != -1 && endPos != -1 && startPos < endPos)
                {
                    startPos = startPos + 12;
                    _serverName = settingsFileContents.Substring(startPos, endPos - startPos);
                }
                else
                {
                    throw new ConnectionException("Could not read workspace server name from settings file for Project " + _projectName);
                }

                startPos = settingsFileContents.IndexOf("<DatabaseName>");
                endPos = settingsFileContents.IndexOf("</DatabaseName>");
                if (startPos != -1 && endPos != -1 && startPos < endPos)
                {
                    startPos = startPos + 14;
                    _databaseName = settingsFileContents.Substring(startPos, endPos - startPos);
                }
                else
                {
                    throw new ConnectionException("Could not read workspace database name from settings file for Project " + _projectName);
                }
            }
        }

        public void ReadProjectFile()
        {
            XmlDocument projectFileDoc = new XmlDocument();
            projectFileDoc.Load(_projectFile);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(projectFileDoc.NameTable);
            nsmgr.AddNamespace("myns1", "http://schemas.microsoft.com/developer/msbuild/2003");

            //Populate deployment server properties
            XmlNode deploymentServerNameNode = null;
            XmlNode deploymentServerDatabaseNode = null;
            XmlNode deploymentServerCubeNameNode = null;

            //Try to populate from active configuration
            if (_project != null)
            {
                string configurationName = _project.ConfigurationManager?.ActiveConfiguration?.ConfigurationName;

                if (!String.IsNullOrEmpty(configurationName))
                {
                    deploymentServerNameNode =     projectFileDoc.SelectSingleNode($"//myns1:PropertyGroup[contains(@Condition,'{configurationName}')]/myns1:DeploymentServerName", nsmgr);
                    deploymentServerDatabaseNode = projectFileDoc.SelectSingleNode($"//myns1:PropertyGroup[contains(@Condition,'{configurationName}')]/myns1:DeploymentServerDatabase", nsmgr);
                    deploymentServerCubeNameNode = projectFileDoc.SelectSingleNode($"//myns1:PropertyGroup[contains(@Condition,'{configurationName}')]/myns1:DeploymentServerCubeName", nsmgr);
                }
            }

            //If not populated - e.g. in command-line mode, get values without Condition attribute filter
            if (deploymentServerNameNode == null)
            {
                deploymentServerNameNode = projectFileDoc.SelectSingleNode("//myns1:PropertyGroup/myns1:DeploymentServerName", nsmgr);
            }
            if (deploymentServerDatabaseNode == null)
            {
                deploymentServerDatabaseNode = projectFileDoc.SelectSingleNode("//myns1:PropertyGroup/myns1:DeploymentServerDatabase", nsmgr);
            }
            if (deploymentServerCubeNameNode == null)
            {
                deploymentServerCubeNameNode = projectFileDoc.SelectSingleNode("//myns1:PropertyGroup/myns1:DeploymentServerCubeName", nsmgr);
            }

            _deploymentServerName = deploymentServerNameNode?.InnerText;
            _deploymentServerDatabase = deploymentServerDatabaseNode?.InnerText;
            _deploymentServerCubeName = deploymentServerCubeNameNode?.InnerText;

            //Get path to BIM file
            if (_project != null)
            {
                foreach (ProjectItem projectItem in _project.ProjectItems)
                {
                    if (projectItem.Name.EndsWith(".bim") && projectItem.FileCount > 0)
                    {
                        _bimFileFullName = projectItem.FileNames[0];
                        break;
                    }
                }
            }
            else
            {
                //Probably running in command-line mode
                XmlNodeList compileNodes = projectFileDoc.SelectNodes("//myns1:ItemGroup/myns1:Compile", nsmgr);
                if (compileNodes != null)
                {
                    foreach (XmlNode compileNode in compileNodes)
                    {
                        if (compileNode.Attributes["Include"] != null && compileNode.Attributes["Include"].Value.ToUpper().EndsWith(".bim".ToUpper()))
                        {
                            FileInfo[] files = _projectDirectoryInfo.GetFiles(compileNode.Attributes["Include"].Value, SearchOption.TopDirectoryOnly);
                            if (files.Length > 0)
                            {
                                _bimFileFullName = files[0].FullName;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method ensures the tabular model is online and populates the CompatibilityLevel property.
        /// </summary>
        /// <param name="closedBimFile">A Boolean specifying if the user cancelled the comparison. For the case where running in Visual Studio, the user has the option of cancelling if the project BIM file is open.</param>
        public void InitializeCompatibilityLevel(bool closedBimFile = false)
        {
            if (UseProject)
            {
                //Initialize _projectDirectoryInfo
                FileInfo projectFileInfo;
                if (_project == null)
                {
                    //Probably running in command-line mode
                    projectFileInfo = new FileInfo(_projectFile);
                }
                else
                {
                    projectFileInfo = new FileInfo(_project.FullName);
                }
                _projectDirectoryInfo = new DirectoryInfo(projectFileInfo.Directory.FullName);

                //Read settings file to get workspace server/db
                ReadSettingsFile();

                //Read project file to get deployment server/cube names, and bim file
                ReadProjectFile();

                //Overwrite the server if a workspace server provided
                if (_workspaceServerProvided)
                {
                    this.ServerName = _workspaceServer;
                }
            }

            Server amoServer = new Server();
            try
            {
                amoServer.Connect(BuildConnectionString());
            }
            catch (ConnectionException) when (UseProject)
            {
                //See if can find integrated workspace server

                bool foundServer = false;

                string tempDataDir = Path.GetTempPath() + @"Microsoft\Microsoft SQL Server\OLAP\LocalServer\Data";
                if (Directory.Exists(tempDataDir))
                {
                    var subDirs = Directory.GetDirectories(tempDataDir).OrderByDescending(d => new DirectoryInfo(d).CreationTime); //Need to order by descending in case old folders hanging around when VS was killed and SSDT didn't get a chance to clean up after itself
                    foreach (string subDir in subDirs)
                    {
                        string[] iniFilePath = Directory.GetFiles(subDir, "msmdsrv.ini");
                        if (iniFilePath.Length == 1 && File.ReadAllText(iniFilePath[0]).Contains("<DataDir>" + _projectDirectoryInfo.FullName + @"\bin\Data</DataDir>")) //Todo: proper xml lookup
                        {
                            //Assuming this must be the folder, so now get the port number
                            string[] portFilePath = Directory.GetFiles(subDir, "msmdsrv.port.txt");
                            if (portFilePath.Length == 1)
                            {
                                string port = File.ReadAllText(portFilePath[0]).Replace("\0", "");
                                this.ServerName = $"localhost:{Convert.ToString(port)}";
                                amoServer.Connect(BuildConnectionString());
                                foundServer = true;
                                break;
                            }
                        }
                    }
                }

                if (!foundServer)
                    throw;
            }

            ////non-admins can't see any ServerProperties: social.msdn.microsoft.com/Forums/sqlserver/en-US/3d0bf49c-9034-4416-9c51-77dc32bf8b73/determine-current-user-permissionscapabilities-via-amo-or-xmla
            //if (!(amoServer.ServerProperties.Count > 0)) //non-admins can't see any ServerProperties
            //{
            //    throw new Microsoft.AnalysisServices.ConnectionException($"Current user {WindowsIdentity.GetCurrent().Name} is not an administrator on the Analysis Server " + this.ServerName);
            //}

            if (amoServer.ServerMode != ServerMode.Tabular && amoServer.ServerMode != ServerMode.SharePoint) //SharePoint is what Power BI Desktop runs as
            {
                throw new ConnectionException($"Analysis Server {this.ServerName} is not running in Tabular mode");
            }

            Database tabularDatabase = amoServer.Databases.FindByName(this.DatabaseName);
            if (tabularDatabase == null)
            {
                if (!this.UseProject)
                {
                    throw new ConnectionException("Could not connect to database " + this.DatabaseName);
                }
                else
                {
                    /* Check if folder exists using SystemGetSubdirs. If so attach. If not, do nothing - when execute BIM file below will create automatically.
                       Using XMLA to run SystemGetSubdirs rather than ADOMD.net here don't want a reference to ADOMD.net Dll.
                       Also, can't use Server.Execute method because it only takes XMLA execute commands (as opposed to XMLA discover commands), so need to submit the full soap envelope
                    */

                    string dataDir = amoServer.ServerProperties["DataDir"].Value;
                    if (dataDir.EndsWith("\\")) dataDir = dataDir.Substring(0, dataDir.Length - 1);
                    string commandStatement = String.Format("SystemGetSubdirs '{0}'", dataDir);
                    XmlNodeList rows = Core.Comparison.ExecuteXmlaCommand(amoServer, "", commandStatement);

                    string dbDir = "";
                    foreach (XmlNode row in rows)
                    {
                        XmlNode dirNode = null;
                        XmlNode allowedNode = null;

                        foreach (XmlNode childNode in row.ChildNodes)
                        {
                            if (childNode.Name == "Dir")
                            {
                                dirNode = childNode;
                            }
                            else if (childNode.Name == "Allowed")
                            {
                                allowedNode = childNode;
                            }
                        }

                        if (dirNode != null && allowedNode != null && dirNode.InnerText.Length >= this.DatabaseName.Length && dirNode.InnerText.Substring(0, this.DatabaseName.Length) == this.DatabaseName && allowedNode.InnerText.Length > 0 && allowedNode.InnerText == "1")
                        {
                            dbDir = dataDir + "\\" + dirNode.InnerText;
                            break;
                        }
                    }

                    if (dbDir != "")
                    {
                        //attach
                        amoServer.Attach(dbDir);
                        amoServer.Refresh();
                        tabularDatabase = amoServer.Databases.FindByName(this.DatabaseName);
                    }
                }
            }

            if (this.UseProject)
            {
                //_bimFileFullName = GetBimFileFullName();
                if (String.IsNullOrEmpty(_bimFileFullName))
                {
                    throw new ConnectionException("Could not load BIM file for Project " + this.ProjectName);
                }

                if (!closedBimFile) //If just closed BIM file, no need to execute it
                {
                    //Execute BIM file contents as script on workspace database

                    //We don't know the compatibility level yet, so try parsing json, if fail, try xmla ...
                    try
                    {
                        //Replace "SemanticModel" with db name.
                        JObject jDocument = JObject.Parse(File.ReadAllText(_bimFileFullName));

                        if (jDocument["name"] == null || jDocument["id"] == null)
                        {
                            throw new ConnectionException("Could not read JSON in BIM file " + _bimFileFullName);
                        }

                        jDocument["name"] = DatabaseName;
                        jDocument["id"] = DatabaseName;

                        //Todo: see if Tabular helper classes for this once documentation available after CTP
                        string command = 
$@"{{
  ""createOrReplace"": {{
    ""object"": {{
      ""database"": ""{DatabaseName}""
    }},
    ""database"": {jDocument.ToString()}
    }}
}}
";
                        amoServer.Execute(command);

                    }
                    catch (JsonReaderException)
                    {
                        //Replace "SemanticModel" with db name.  Could do a global replace, but just in case it's not called SemanticModel, use dom instead
                        //string xmlaScript = File.ReadAllText(xmlaFileFullName);
                        XmlDocument document = new XmlDocument();
                        document.Load(_bimFileFullName);
                        XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);
                        nsmgr.AddNamespace("myns1", "http://schemas.microsoft.com/analysisservices/2003/engine");

                        XmlNode objectDatabaseIdNode = document.SelectSingleNode("//myns1:Object/myns1:DatabaseID", nsmgr);
                        XmlNode objectDefinitionDatabaseIdNode = document.SelectSingleNode("//myns1:ObjectDefinition/myns1:Database/myns1:ID", nsmgr);
                        XmlNode objectDefinitionDatabaseNameNode = document.SelectSingleNode("//myns1:ObjectDefinition/myns1:Database/myns1:Name", nsmgr);

                        if (objectDatabaseIdNode == null || objectDefinitionDatabaseIdNode == null || objectDefinitionDatabaseNameNode == null)
                        {
                            throw new ConnectionException("Could not access XMLA in BIM file " + _bimFileFullName);
                        }

                        objectDatabaseIdNode.InnerText = DatabaseName;
                        objectDefinitionDatabaseIdNode.InnerText = DatabaseName;
                        objectDefinitionDatabaseNameNode.InnerText = DatabaseName;

                        //1103, 1100 projects store the xmla as Alter (equivalent to createOrReplace), so just need to execute
                        amoServer.Execute(document.OuterXml);
                    }
                }

                //need next lines in case just created the db using the Execute method
                //amoServer.Refresh(); //todo workaround for bug 9719887 on 3/10/17 need to disconnect and reconnect
                amoServer.Disconnect();
                amoServer.Connect(BuildConnectionString());

                tabularDatabase = amoServer.Databases.FindByName(this.DatabaseName);
            }

            if (tabularDatabase == null)
            {
                throw new ConnectionException($"Can not load/find database {this.DatabaseName}.");
            }
            _compatibilityLevel = tabularDatabase.CompatibilityLevel;
            _dataSourceVersion = tabularDatabase.Model.DefaultPowerBIDataSourceVersion.ToString();
            _directQuery = ((tabularDatabase.Model != null && tabularDatabase.Model.DefaultMode == Microsoft.AnalysisServices.Tabular.ModeType.DirectQuery) ||
                             tabularDatabase.DirectQueryMode == DirectQueryMode.DirectQuery || tabularDatabase.DirectQueryMode == DirectQueryMode.InMemoryWithDirectQuery || tabularDatabase.DirectQueryMode == DirectQueryMode.DirectQueryWithInMemory);
        }

        /// <summary>
        /// Build connection string.
        /// </summary>
        /// <returns></returns>
        public string BuildConnectionString()
        {
            string connectionString = $"Provider=MSOLAP;Data Source={this.ServerName};";
            if (this.CredsProvided)
            {
                connectionString += $"User ID={this.Username};Password={this.Password};";
            }

            return connectionString;
        }
    }
}
