using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Xml;
using System.Threading;
using System.Security.Principal;
using Microsoft.AnalysisServices;
using EnvDTE;
using System.Windows.Forms;
using BismNormalizer.TabularCompare.Core;

namespace BismNormalizer.TabularCompare.MultidimensionalMetadata
{
    /// <summary>
    /// Abstraction of a tabular model table with properties and methods for comparison purposes. This class can represent a database on a server, or a project in Visual Studio.
    /// </summary>
    public class TabularModel : IDisposable
    {
        #region Private Members

        private Comparison _parentComparison;
        private ConnectionInfo _connectionInfo;
        private ComparisonInfo _comparisonInfo;
        private Server _amoServer;
        private Database _amoDatabase;
        private DataSourceCollection _dataSources = new DataSourceCollection();
        private TableCollection _tables = new TableCollection();
        private MeasureCollection _measures = new MeasureCollection();
        private KpiCollection _kpis = new KpiCollection();
        private PerspectiveCollection _perspectives = new PerspectiveCollection();
        private RoleCollection _roles = new RoleCollection();
        private ActionCollection _actions = new ActionCollection();
        private List<string> _activeRelationshipIds = new List<string>();
        private bool _disposed = false;

        #endregion

        /// <summary>
        /// Initializes a new instance of the TabularModel class using multiple parameters.
        /// </summary>
        /// <param name="parentComparison">Comparison object that the tabular model belongs to.</param>
        /// <param name="connectionInfo">ConnectionInfo object for the tabular model.</param>
        /// <param name="comparisonInfo">ComparisonInfo object for the tabular model.</param>
        public TabularModel(Comparison parentComparison, ConnectionInfo connectionInfo, ComparisonInfo comparisonInfo)
        {
            _parentComparison = parentComparison;
            _connectionInfo = connectionInfo;
            _comparisonInfo = comparisonInfo;
        }

        /// <summary>
        /// Connect to SSAS server and instantiate properties of the TabularModel object.
        /// </summary>
        public void Connect()
        {
            this.Disconnect();

            _amoServer = new Server();
            _amoServer.Connect("Provider=MSOLAP;Data Source=" + _connectionInfo.ServerName);

            _amoDatabase = _amoServer.Databases.FindByName(_connectionInfo.DatabaseName);
            if (_amoDatabase == null)
            {
                //We don't need try to load from project here as will already be done before instantiated Comparison
                throw new Microsoft.AnalysisServices.ConnectionException("Could not connect to database " + _connectionInfo.DatabaseName);
            }

            //direct query check
            if (_amoDatabase.DirectQueryMode == DirectQueryMode.DirectQuery || _amoDatabase.DirectQueryMode == DirectQueryMode.InMemoryWithDirectQuery || _amoDatabase.DirectQueryMode == DirectQueryMode.DirectQueryWithInMemory)
            {
                throw new ConnectionException((_connectionInfo.UseProject ? "Project " + _connectionInfo.ProjectName : "Database " + _amoDatabase.Name) + " has DirectQuery Mode property set to " + Convert.ToString(_amoDatabase.DirectQueryMode) + ", which is not supported for Compatibility Level " + Convert.ToString(_connectionInfo.CompatibilityLevel) + ".");
            }

            // shell model            
            foreach (Microsoft.AnalysisServices.DataSource datasource in _amoDatabase.DataSources)
            {
                _dataSources.Add(new DataSource(this, datasource));
            }
            foreach (Dimension dimension in _amoDatabase.Dimensions)
            {
                _tables.Add(new Table(this, dimension));
            }
            foreach (Microsoft.AnalysisServices.Role role in _amoDatabase.Roles)
            {
                _roles.Add(new Role(this, role));
            }
            if (_amoDatabase.Cubes.Count > 0)
            {
                foreach (Microsoft.AnalysisServices.Action action in _amoDatabase.Cubes[0].Actions)
                {
                    _actions.Add(new Action(this, action));
                }
            }
            PopulateMeasures();
            //need to populate perspectives after measures because obj def refers to measures collection
            if (_amoDatabase.Cubes.Count > 0)
            {
                foreach (Microsoft.AnalysisServices.Perspective perspective in _amoDatabase.Cubes[0].Perspectives)
                {
                    _perspectives.Add(new Perspective(this, perspective));
                }
            }
        }

        /// <summary>
        /// Disconnect from the SSAS server.
        /// </summary>
        public void Disconnect()
        {
            if (_amoServer != null) _amoServer.Disconnect();
        }

        #region Private Methods

        private struct ParsedCommand
        {
            public ParsedCommand(IEnumerable<string> expressions, string fullName, string table)
            {
                Expressions = new List<string>();
                Expressions.AddRange(expressions);
                FullName = fullName;
                Table = table;
            }
            public List<string> Expressions;
            public string FullName;
            public string Table;
        }
        private void PopulateMeasures()
        {
            if (_amoDatabase.Cubes.Count > 0)
            {
                MdxScript mdxScript = _amoDatabase.Cubes[0].MdxScripts["MdxScript"];
                if (mdxScript.Commands.Count > 0)
                {
                    List<ParsedCommand> parsedCommands = new List<ParsedCommand>();

                    //Review all commands in MdxScripts and extract all MDX expressions found there
                    for (int i = 0; i < mdxScript.Commands.Count; i++)
                    {
                        if (mdxScript.Commands[i].Text != null)
                        {
                            List<string> expressions = new List<string>();
                            expressions.AddRange(ParseMdxScript(mdxScript.Commands[i].Text));
                            if (expressions.Count > 0)
                            {
                                string fullName = "";
                                if (mdxScript.Commands[i].Annotations.Contains("FullName")) fullName = mdxScript.Commands[i].Annotations["FullName"].Value.InnerText;
                                ////yet another microsoft fudge
                                //if (fullName.Length > 1 && fullName.Substring(fullName.Length - 2, 2) == "]]") fullName = fullName.Substring(0, fullName.Length - 1);
                                string table = "";
                                if (mdxScript.Commands[i].Annotations.Contains("Table")) table = mdxScript.Commands[i].Annotations["Table"].Value.InnerText;
                                parsedCommands.Add(new ParsedCommand(expressions, fullName, table));
                            }
                        }
                    }

                    MeasureCollection kpiReferenceMeasures = new MeasureCollection();
                    //KPIs declared in MDX script
                    List<Microsoft.AnalysisServices.Kpi> kpisDeclaredInScript = new List<Microsoft.AnalysisServices.Kpi>();

                    if (_amoDatabase.CompatibilityLevel < 1103)
                    {
                        //This block only applies pre SP1
                        //Populate the KPI reference measures first - they start with "CREATE MEMBER CURRENTCUBE.Measures" (and use MDX) rather than "CREATE MEASURE"

                        foreach (ParsedCommand parsedCommand in parsedCommands)
                        {
                            foreach (string statement in parsedCommand.Expressions)
                            {
                                if ((statement.Length >= 35) && (statement.Substring(0, 35) == "CREATE MEMBER CURRENTCUBE.Measures."))
                                {
                                    //Find table name/measure/expression
                                    int openSquareBracketPosition = statement.IndexOf('[', 0);
                                    int closeSquareBracketPosition = statement.IndexOf(']', openSquareBracketPosition + 1);
                                    string statementMeasureName = statement.Substring(openSquareBracketPosition + 1, closeSquareBracketPosition - openSquareBracketPosition - 1);

                                    int openExpressionQuotePosition = statement.IndexOf('\'', 0);
                                    if (openExpressionQuotePosition != -1)
                                    {
                                        int closeExpressionQuotePosition = statement.IndexOf('\'', openExpressionQuotePosition + 1);
                                        string statementMeasureExpression = statement.Substring(openExpressionQuotePosition + 1, closeExpressionQuotePosition - openExpressionQuotePosition - 1);

                                        int associatedMeasureGroupEndPosition = statement.IndexOf("ASSOCIATED_MEASURE_GROUP", closeExpressionQuotePosition) + 24;
                                        int openTableQuotePosition = statement.IndexOf('\'', associatedMeasureGroupEndPosition + 1);
                                        int closeTableQuotePosition = statement.IndexOf('\'', openTableQuotePosition + 1);
                                        string statementTableName = statement.Substring(openTableQuotePosition + 1, closeTableQuotePosition - openTableQuotePosition - 1);

                                        kpiReferenceMeasures.Add(new Measure(this, statementTableName, statementMeasureName, statementMeasureExpression));
                                    }
                                }
                            }
                        }
                    }

                    /* KPIs can be created in 2 different ways: either declared in the MDX script as "CREATE KPI" (in which case won't be populated
                       in _amoDatabase.Cubes[0].Kpis), or using the object model (in which case it will be in the AMO KPI collection).
                       -PRE -SP1 CAN BE EITHER!
                       -POST-SP1 ALWAYS IN SCRIPT
                       So we need to check both.
                       BUT WAIT, THERE'S MORE ... if created in script pre-sp1 will be MDX syntax, if created in script post sp1, will be dax syntax
                       AND ... post-sp1 replaces ']' characters with ']]'
                       Thank you Microsoft. You are the best.
                    */

                    foreach (ParsedCommand parsedCommand in parsedCommands)
                    {
                        foreach (string statement in parsedCommand.Expressions)
                        {
                            if ((statement.Length >= 23) && (statement.Substring(0, 23) == "CREATE KPI CURRENTCUBE."))
                            {
                                int lastCharacterPosition = 0;
                                string kpiName;
                                string kpiExpression;
                                string goalName;
                                string goalExpression;
                                string statusName;
                                string statusExpression;
                                string trendName;
                                string trendExpression;

                                if (_amoDatabase.CompatibilityLevel < 1103)
                                {
                                    int openSquareBracketPosition = statement.IndexOf('[', 0);
                                    int closeSquareBracketPosition = statement.IndexOf(']', openSquareBracketPosition + 1);
                                    kpiName = statement.Substring(openSquareBracketPosition + 1, closeSquareBracketPosition - openSquareBracketPosition - 1);

                                    int goalEndPosition = statement.IndexOf("GOAL", closeSquareBracketPosition) + "GOAL".Length;
                                    int goalOpenPosition = statement.IndexOf("Measures.[", goalEndPosition + 1) + "Measures.[".Length;
                                    int goalClosePosition = statement.IndexOf("]", goalOpenPosition) - 1;
                                    goalName = statement.Substring(goalOpenPosition, goalClosePosition - goalOpenPosition + 1);

                                    int statusEndPosition = statement.IndexOf("STATUS", closeSquareBracketPosition) + "STATUS".Length;
                                    int statusOpenPosition = statement.IndexOf("Measures.[", statusEndPosition + 1) + "Measures.[".Length;
                                    int statusClosePosition = statement.IndexOf("]", statusOpenPosition) - 1;
                                    statusName = statement.Substring(statusOpenPosition, statusClosePosition - statusOpenPosition + 1);

                                    int trendEndPosition = statement.IndexOf("TREND", closeSquareBracketPosition) + "TREND".Length;
                                    int trendOpenPosition = statement.IndexOf("Measures.[", trendEndPosition + 1) + "Measures.[".Length;
                                    int trendClosePosition = statement.IndexOf("]", trendOpenPosition) - 1;
                                    trendName = statement.Substring(trendOpenPosition, trendClosePosition - trendOpenPosition + 1);
                                }
                                else
                                {
                                    ParseMeasureAndExpression(parsedCommand.FullName, statement, out kpiName, out kpiExpression, ref lastCharacterPosition);

                                    lastCharacterPosition = statement.IndexOf("GOAL", lastCharacterPosition) + "GOAL".Length;
                                    ParseMeasureAndExpression(parsedCommand.FullName, statement, out goalName, out goalExpression, ref lastCharacterPosition);

                                    lastCharacterPosition = statement.IndexOf("STATUS", lastCharacterPosition) + "STATUS".Length;
                                    ParseMeasureAndExpression(parsedCommand.FullName, statement, out statusName, out statusExpression, ref lastCharacterPosition);

                                    lastCharacterPosition = statement.IndexOf("TREND", lastCharacterPosition) + "TREND".Length;
                                    ParseMeasureAndExpression(parsedCommand.FullName, statement, out trendName, out trendExpression, ref lastCharacterPosition);
                                }

                                int statusGraphicEndPosition = statement.IndexOf("STATUS_GRAPHIC", lastCharacterPosition) + "STATUS_GRAPHIC".Length;
                                int statusGraphicOpenQuotePosition = statement.IndexOf('\'', statusGraphicEndPosition + 1);
                                int statusGraphicCloseQuotePosition = statement.IndexOf('\'', statusGraphicOpenQuotePosition + 1);
                                string mdxStmtStatusGraphic = statement.Substring(statusGraphicOpenQuotePosition + 1, statusGraphicCloseQuotePosition - statusGraphicOpenQuotePosition - 1);

                                int trendGraphicEndPosition = statement.IndexOf("TREND_GRAPHIC", lastCharacterPosition) + "TREND_GRAPHIC".Length;
                                int trendGraphicOpenQuotePosition = statement.IndexOf('\'', trendGraphicEndPosition + 1);
                                int trendGraphicCloseQuotePosition = statement.IndexOf('\'', trendGraphicOpenQuotePosition + 1);
                                string mdxStmtTrendGraphic = statement.Substring(trendGraphicOpenQuotePosition + 1, trendGraphicCloseQuotePosition - trendGraphicOpenQuotePosition - 1);

                                //Kpi kpiDeclaredInScript = new Kpi(mdxStmtKpiName, mdxStmtKpiName); //ok to use a guid as id because this is just a temporary store of the KPI in the kpisDeclaredInScript variable.  It is referred to below without using the id/amo instance.  This resolves issue where special characters are in the name (e.g. (, %, etc), which causes error if in the ID.
                                Microsoft.AnalysisServices.Kpi kpiDeclaredInScript = new Microsoft.AnalysisServices.Kpi(kpiName, Convert.ToString(Guid.NewGuid()));
                                kpiDeclaredInScript.Goal = goalName;
                                kpiDeclaredInScript.Status = statusName;
                                kpiDeclaredInScript.Trend = trendName;
                                kpiDeclaredInScript.StatusGraphic = mdxStmtStatusGraphic;
                                kpiDeclaredInScript.TrendGraphic = mdxStmtTrendGraphic;
                                kpisDeclaredInScript.Add(kpiDeclaredInScript);
                            }
                        }
                    }
                    
                    //Note: here we are making the assumption that Measures are created in a FIXED way
                    //      This routine will only find measures that have been created by following fixed pattern
                    //      string.Format("CREATE MEASURE '{0}'[{1}]={2};", cmTableName, cmName, newCalculatedMeasureExpression.Text)
                    _measures.Clear();
                    _kpis.Clear();

                    foreach (ParsedCommand parsedCommand in parsedCommands)
                    {
                        foreach (string statement in parsedCommand.Expressions)
                        {
                            if ((statement.Length >= 14) && (statement.Substring(0, 14) == "CREATE MEASURE"))
                            {
                                //Find table name/measure/expression
                                string statementTableName = "";
                                if (parsedCommand.Table == "")  //post SP1, there should always be a table/fullname and one single statement, so this check SHOULD be redundant
                                {
                                    int openQuotePosition = statement.IndexOf('\'', 0);
                                    int closeQuotePosition = statement.IndexOf('\'', openQuotePosition + 1);
                                    statementTableName = statement.Substring(openQuotePosition + 1, closeQuotePosition - openQuotePosition - 1);
                                }
                                else
                                {
                                    statementTableName = parsedCommand.Table;
                                }

                                //int closeSquareBracketPosition = statement.IndexOf(']', openSquareBracketPosition + 1);
                                int lastCharacterPosition = 0;
                                string statementMeasureName;
                                string statementMeasureExpression;
                                ParseMeasureAndExpression(parsedCommand.FullName, statement, out statementMeasureName, out statementMeasureExpression, ref lastCharacterPosition);

                                //check if it's a kpi measure
                                Microsoft.AnalysisServices.Kpi kpi = null;
                                if (_amoDatabase.CompatibilityLevel < 1103)
                                {

                                    kpi = _amoDatabase.Cubes[0].Kpis.FindByName(statementMeasureName); //these are populated using the object model (pre SP1 only SOMETIMES)
                                }

                                // Could be declared in script (post-SP1, it should always be, pre SP1 SOMETIMES)
                                if (kpi == null)
                                {
                                    //check if declared in MDX script instead (it normally would be)
                                    foreach (Microsoft.AnalysisServices.Kpi kpiDeclaredInScript in kpisDeclaredInScript)
                                    {
                                        if (kpiDeclaredInScript.Name == statementMeasureName)
                                        {
                                            kpi = kpiDeclaredInScript;
                                            break;
                                        }
                                    }
                                }

                                if (kpi == null)
                                {
                                    // it's really a measure (not a KPI)
                                    _measures.Add(new Measure(this, statementTableName, statementMeasureName, statementMeasureExpression.Trim()));
                                }
                                else
                                {
                                    // it's really a KPI

                                    //note: the kpiReferenceMeasures will be empty post SP1, but will fix it below.  Can't fix it here because might not have all the measures populated yet (post sp1, kpi reference measures will be populated in _measures)
                                    _kpis.Add(new Kpi(this,
                                                                statementTableName,
                                                                statementMeasureName,
                                                                statementMeasureExpression,
                                                                kpiReferenceMeasures.FindByName(kpi.Goal),
                                                                kpiReferenceMeasures.FindByName(kpi.Status),
                                                                kpiReferenceMeasures.FindByName(kpi.Trend),
                                                                kpi.StatusGraphic,
                                                                kpi.TrendGraphic
                                                                //,kpi
                                                                ));
                                }
                            }
                        }
                    }

                    //post SP1, fix kpi reference measures
                    if (_amoDatabase.CompatibilityLevel >= 1103)
                    {
                        foreach (Kpi kpi in _kpis)
                        {
                            //get the AMO KPI
                            Microsoft.AnalysisServices.Kpi amoKpi = _amoDatabase.Cubes[0].Kpis.FindByName(kpi.Name); //these are populated using the object model - can also be declared as CREATE KPI in the MDX script, which will not be in this Kpi collection
                            if (amoKpi == null)
                            {
                                //check if declared in MDX script instead (it normally would be)
                                foreach (Microsoft.AnalysisServices.Kpi kpiDeclaredInScript in kpisDeclaredInScript)
                                {
                                    if (kpiDeclaredInScript.Name == kpi.Name)
                                    {
                                        amoKpi = kpiDeclaredInScript;
                                        break;
                                    }
                                }
                            }
                            if (amoKpi != null)
                            {
                                // Now check _measures to get the kpi reference measures.  Flag them as KPI reference measures and populate KPI reference measure properties
                                Measure kpiGoalReferenceMeasure = _measures.FindByName(amoKpi.Goal);
                                Measure kpiStatusReferenceMeasure = _measures.FindByName(amoKpi.Status);
                                Measure kpiTrendReferenceMeasure = _measures.FindByName(amoKpi.Trend);

                                //Flag the public KPI measures (like Measures.[M Goal] as IsKpiReferenceMeasure so doesn't show up on grid)
                                if (kpiGoalReferenceMeasure != null)
                                {
                                    kpiGoalReferenceMeasure.IsKpiReferenceMeasure = true;
                                    kpi.GoalMeasure = kpiGoalReferenceMeasure;
                                }
                                if (kpiStatusReferenceMeasure != null)
                                {
                                    kpiStatusReferenceMeasure.IsKpiReferenceMeasure = true;
                                    kpi.StatusMeasure = kpiStatusReferenceMeasure;
                                }
                                if (kpiTrendReferenceMeasure != null)
                                {
                                    kpiTrendReferenceMeasure.IsKpiReferenceMeasure = true;
                                    kpi.TrendMeasure = kpiTrendReferenceMeasure;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ParseMeasureAndExpression(string measureNameFromAnnotations, string statement, out string statementMeasureName, out string statementMeasureExpression, ref int lastCharacterPosition)
        {
            //There is some major Microsoft hacking going on here.  ] chars are replaced with ]], dependent on all sorts of formatting to identify KPI reference measures (e.g. Measures.[_xxx Goal]), ... the list goes on.
            int openSquareBracketPosition = statement.IndexOf('[', lastCharacterPosition);
            // following line doesn't work for MS hacked measures like "Measures.[_M1 Goal]" becuase of the hacked underscore
            //int closeSquareBracketPosition = statement.IndexOf(']', tableName.Replace("]", "  ").Length + openSquareBracketPosition + 1);
            // so have to do this instead
            string msHackedMeasureFullName = measureNameFromAnnotations.Replace("]", "]]");
            int closeMeasurePosition = statement.IndexOf(msHackedMeasureFullName, openSquareBracketPosition + 1) + msHackedMeasureFullName.Length - 1;
            lastCharacterPosition = statement.IndexOf(']', closeMeasurePosition + 1); ;
            statementMeasureName = statement.Substring(openSquareBracketPosition + 1, lastCharacterPosition - openSquareBracketPosition - 1);

            int equalSigPosition = statement.IndexOf('=', lastCharacterPosition);
            statementMeasureExpression = statement.Substring(equalSigPosition + 1);
        }

        private string[] ParseMdxScript(string commandText)
        {
            List<string> mdxExpressions = new List<string>();
            List<string> subLines = new List<string>();
            using (StringReader lines = new StringReader(commandText))
            {
                string line = string.Empty;
                Boolean continuedLine = false;
                Boolean partialSubLine = false;
                Boolean inCommentBlock = false;
                StringBuilder mdxExpression = new StringBuilder();

                while ((line = lines.ReadLine()) != null)
                {
                    line = line.TrimEnd();

                    if (inCommentBlock)
                    {
                        if (line.Contains("*/"))
                        {
                            inCommentBlock = false;
                            int closeCommentBlockPosition = line.IndexOf("*/") + 2;
                            if (line.Length > closeCommentBlockPosition)
                            {
                                // check if text after comment block
                                line = line.Substring(closeCommentBlockPosition, line.Length - closeCommentBlockPosition);
                            }
                            else
                                continue;
                        }
                        else
                            continue;
                    }

                    if (line.Contains("/*")) //start of comment
                    {
                        if (!line.Contains("*/"))  // does not complete comment in one line
                        {
                            inCommentBlock = true;

                            //check if there is text before comment block
                            line = line.Substring(0, line.IndexOf("/*"));
                        }
                        else
                        {   //does complete comment in one line - so check if text following comment block

                            int closeCommentBlockPosition = line.IndexOf("*/") + 2;
                            if (line.Length > closeCommentBlockPosition)
                            {
                                line = line.Substring(closeCommentBlockPosition, line.Length - closeCommentBlockPosition);
                            }
                            else
                                continue;
                        }
                    }

                    if (IsBlankLine(line))
                        continue; // Ignore comment lines or empty line

                    if (line.Contains(';'))
                    {
                        subLines.Clear();

                        //8/22/14 commented out following sections.  No longer support multiple statements on 1 line.  Can have a problem if ';' in measure name and also '"' in measure name.
                        //// Check the semi-colon is not part a string literal before spliting the line
                        //if (line.Contains('"'))
                        //{
                        //    //Have to do manual split... to avoid spliting a string literal, just in case there is a semi-colon in the string
                        //    int pk = 0;
                        //    int npk = 0;
                        //    do
                        //    {
                        //        int pq = line.IndexOf('"', pk);
                        //        int npq = ((pq + 1) < line.Length) ? line.IndexOf('"', pq + 1) : -1;
                        //        while (((npk = line.IndexOf(';', pk)) != -1) && (npk < pq))
                        //        {
                        //            if (GetExpressionFromLineFlagEOL(subLines, line, ref pk, ref partialSubLine))
                        //                break;
                        //        }
                        //        if (npk > pq)
                        //        {
                        //            //if ((npk = line.IndexOf(';', npq)) != -1)
                        //            if ((npk = line.LastIndexOf(';')) != -1)
                        //            {
                        //                if (GetExpressionFromLineFlagEOL(subLines, line, ref pk, ref partialSubLine))
                        //                    break;
                        //            }
                        //        }

                        //    } while ((npk != -1));

                        //}
                        //else
                        //{
                            int pk = 0;
                            while (!GetExpressionFromLineFlagEOL(subLines, line, ref pk, ref partialSubLine)) ;
                        //}

                        if (continuedLine)
                        {
                            //subLines[0] = string.Concat(mdxExpression.ToString(), '\n', subLines[0]);
                            //----------
                            if (!mdxExpression.ToString().TrimStart().StartsWith("//"))
                            {
                                subLines[0] = string.Concat(mdxExpression.ToString(), subLines[0]);
                            }
                            mdxExpression.Clear();
                            //----------
                            continuedLine = false;
                        }
                        for (int i = 0; i < subLines.Count - 1; i++)
                        {
                            mdxExpressions.Add(subLines[i].TrimStart());
                        }
                        if (!partialSubLine)
                        {
                            mdxExpressions.Add(subLines[subLines.Count - 1].TrimStart());
                        }
                        else
                        {
                            mdxExpression.Clear();
                            mdxExpression.AppendLine(subLines[subLines.Count - 1]);
                            continuedLine = true;
                        }

                    }
                    else
                    {
                        continuedLine = true;
                        mdxExpression.AppendLine(line);
                    }

                }
            }
            return mdxExpressions.ToArray();
        }

        private Boolean IsBlankLine(string line) =>
            (string.IsNullOrWhiteSpace(line) ||
              (line.TrimStart().Length >= 2 && line.TrimStart().Substring(0, 2) == "--") ||
              (line.TrimStart().Length >= 2 && line.TrimStart().Substring(0, 2) == "//")
            );

        private Boolean GetExpressionFromLineFlagEOL(List<string> SubLines, string Line, ref int pk, ref bool partialSubLine)
        {
            //changed following line 8/19/14
            //this won't work because can have ';' in measure names post SP1:                       int npk = Line.IndexOf(';', pk);
            //this won't work because can have ';' in measure names AND not be the end of line:     int npk = Line.LastIndexOf(';');
            //so have to use this instead, which does not allow multiple statements on a single line separated by ';':
            int npk = Line.IndexOf(';', Line.TrimEnd().Length - 1);
            if (npk != -1)
            {
                SubLines.Add(Line.Substring(pk, (npk - pk) + 1)); // to include both endpoints
                pk = npk + 1;
                partialSubLine = false;
                if (pk >= Line.Length)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (!IsBlankLine(Line.Substring(pk)))
                {
                    SubLines.Add(Line.Substring(pk));
                    partialSubLine = true;
                }
                return true; //EOL reached
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Analysis Management Objects Database object abtstracted by the TabularModel class.
        /// </summary>
        public Database AmoDatabase
        {
            get { return _amoDatabase; }
            set { _amoDatabase = value; }
        }

        /// <summary>
        /// Collection of data sources for the TabularModel object.
        /// </summary>
        public DataSourceCollection DataSources => _dataSources;

        /// <summary>
        /// Collection of tables for the TabularModel object.
        /// </summary>
        public TableCollection Tables => _tables;

        /// <summary>
        /// Collection of measures for the TabularModel object, excluding those that are KPI references.
        /// </summary>
        public MeasureCollection Measures
        {
            get
            {
                //exclude the measures that are kpi reference measures, which are internal
                MeasureCollection _returnMeasures = new MeasureCollection();
                foreach (Measure measure in _measures)
                {
                    if (measure.IsKpiReferenceMeasure == false)
                    {
                        _returnMeasures.Add(measure);
                    }
                }
                return _returnMeasures;
            }
        }

        /// <summary>
        /// Collection of measures for the TabularModel object, including those that are KPI references.
        /// </summary>
        public MeasureCollection MeasuresFull => _measures;

        /// <summary>
        /// Collection of KPIs for the TabularModel object.
        /// </summary>
        public KpiCollection Kpis => _kpis;

        /// <summary>
        /// Collection of perspectives for the TabularModel object.
        /// </summary>
        public PerspectiveCollection Perspectives => _perspectives;

        /// <summary>
        /// Collection of roles for the TabularModel object.
        /// </summary>
        public RoleCollection Roles => _roles;

        /// <summary>
        /// Collection of actions for the TabularModel object.
        /// </summary>
        public ActionCollection Actions => _actions;

        /// <summary>
        /// List of active relationship ids for the TabularModel object.
        /// </summary>
        public List<string> ActiveRelationshipIds => _activeRelationshipIds;

        /// <summary>
        /// ConnectionInfo object for the tabular model.
        /// </summary>
        public ConnectionInfo ConnectionInfo => _connectionInfo;

        /// <summary>
        /// ComparisonInfo object for the tabular model.
        /// </summary>
        public ComparisonInfo ComparisonInfo => _comparisonInfo;

        #endregion

        #region Actions

        /// <summary>
        /// Remove all reference dimensions from the AMO tabular model when starting to validate actions. They are added back dynamically at the end of validation.
        /// </summary>
        public void FlushReferenceDimensions()
        {
            if (_amoDatabase.Cubes.Count == 0) return;

            // Clear out reference dimensions - will recreate them later according to new model
            foreach (MeasureGroup measureGroup in _amoDatabase.Cubes[0].MeasureGroups)
            {
                List<string> measureGroupDimensionIdsToDelete = new List<string>();
                foreach (MeasureGroupDimension measureGroupDimension in measureGroup.Dimensions)
                {
                    if (measureGroupDimension is ReferenceMeasureGroupDimension)
                    {
                        ReferenceMeasureGroupDimension referenceMeasureGroupDimension = ((ReferenceMeasureGroupDimension)measureGroupDimension);

                        // If there is a reference dimension for the relationship, then it is active
                        //_activeRelationshipIds.Add(referenceMeasureGroupDimension.RelationshipID);  //unfortunately, the RelationshipID property is not always populated - only if the intermediate dim is the fact dim
                        foreach (Table table in _tables)
                        {
                            bool foundRelationship = false;
                            foreach (Relationship relationship in table.Relationships)
                            {
                                if (referenceMeasureGroupDimension.IntermediateCubeDimensionID == relationship.AmoRelationship.FromRelationshipEnd.DimensionID &&
                                    referenceMeasureGroupDimension.CubeDimensionID == relationship.AmoRelationship.ToRelationshipEnd.DimensionID)
                                {
                                    if (!_activeRelationshipIds.Contains(relationship.Id))
                                    {
                                        _activeRelationshipIds.Add(relationship.Id);
                                    }
                                    foundRelationship = true;
                                    break;
                                }
                            }
                            if (foundRelationship)
                            {
                                break;
                            }
                        }

                        referenceMeasureGroupDimension.IntermediateCubeDimensionID = null;
                        measureGroupDimensionIdsToDelete.Add(measureGroupDimension.CubeDimensionID);
                    }
                }
                foreach (string dimensionId in measureGroupDimensionIdsToDelete)
                {
                    measureGroup.Dimensions.Remove(dimensionId);
                }
            }
        }

        /// <summary>
        /// Dynamically add back reference dimensions when finishing validation of actions. This includes checking for ambigious paths and setting to inactive as required.
        /// </summary>
        public void PopulateReferenceDimensions()
        {
            //7. Repopulate reference dims based on relationships.  When iterating the tables, keep a record (string array) of 
            //   relationships added (or having ref dim implemented) that are active relationships.  If come across a 2nd one to the same
            //   table, the one that was already in the target wins.

            if (_amoDatabase.Cubes.Count > 0)
            {
                foreach (MeasureGroup measureGroup in _amoDatabase.Cubes[0].MeasureGroups)
                {
                    // If have any relationships, the measure group will have a fact dimension that acts as the intermediate dimension
                    if (measureGroup.Dimensions.Count == 1)
                    {
                        string degenerateDimensionId = measureGroup.Dimensions[0].CubeDimensionID;
                        Table measureGroupTable = _tables.FindById(measureGroup.ID);

                        foreach (Relationship relationship in measureGroupTable.Relationships)
                        {
                            PopulateReferenceDimension(measureGroup, degenerateDimensionId, relationship);
                        }
                    }
                }
            }
        }

        private void PopulateReferenceDimension(MeasureGroup measureGroup, string degenerateDimensionId, Relationship relationship)
        {
            if (relationship.IsActive)
            {
                Dimension referencedDimension = _tables.FindById(relationship.AmoRelationship.ToRelationshipEnd.DimensionID).AmoDimension;
                bool willAddReference = false;

                if (measureGroup.Dimensions.Contains(referencedDimension.ID))
                {
                    // If we are here, we have identified 2 paths to get to the same reference dimension - because when combining source/target models would result in 2 active relationship paths to the same table.  So, the one that was already there in the target should win.
                    if (relationship.CopiedFromSource)
                    {
                        // So we just ignore [relationship] (don't populate it).  But we also need to call DeleteAlternateActiveRelationship to ensure it's not flagged as active anymore - and also flush any reference dims that might have already been populated with it ...
                        DeleteAlternateActiveRelationship(relationship);

                        _parentComparison.OnValidationMessage(new ValidationMessageEventArgs(
                            "Relationship " + relationship.Name.Trim() + " (which is active in the source) has been created in the target, but it is set to inactive because there is already an active set of relationships between '" + measureGroup.Name + "' and '" + referencedDimension.Name + "'.", 
                            ValidationMessageType.Relationship, 
                            ValidationMessageStatus.Warning));
                        willAddReference = false;
                    }
                    else
                    {
                        // [relationship] is the one that was already in the target.  So need to remove the existing one (which was copied from source)
                        ReferenceMeasureGroupDimension referenceDimToDelete = (ReferenceMeasureGroupDimension)measureGroup.Dimensions[referencedDimension.ID];
                        bool foundRelationship = false;

                        //We need to delete the relationship that has already been populated - which was copied from source
                        foreach (Table table in _tables)
                        {
                            foreach (Relationship potentiallyRelationshipToDelete in table.Relationships)
                            {
                                if (potentiallyRelationshipToDelete.Id != relationship.Id &&
                                    potentiallyRelationshipToDelete.AmoRelationship.FromRelationshipEnd.DimensionID == referenceDimToDelete.IntermediateCubeDimensionID &&
                                    potentiallyRelationshipToDelete.AmoRelationship.ToRelationshipEnd.DimensionID == referenceDimToDelete.CubeDimensionID)
                                {
                                    DeleteAlternateActiveRelationship(potentiallyRelationshipToDelete);
                                    _parentComparison.OnValidationMessage(new ValidationMessageEventArgs(
                                        "Relationship " + potentiallyRelationshipToDelete.Name.Trim() + " (which is active in the source) has been created in the target, but it is set to inactive because there is already an active set of relationships between '" + measureGroup.Name + "' and '" + referencedDimension.Name + "'.", 
                                        ValidationMessageType.Relationship, 
                                        ValidationMessageStatus.Warning));
                                    foundRelationship = true;
                                    break;
                                }
                            }
                            if (foundRelationship)
                            {
                                break;
                            }
                        }
                        willAddReference = true;
                    }
                }
                else
                {
                    willAddReference = true;
                }

                if (willAddReference)
                {
                    ReferenceMeasureGroupDimension referenceMeasuregroupDimension = new ReferenceMeasureGroupDimension();
                    referenceMeasuregroupDimension.CubeDimensionID = referencedDimension.ID;
                    foreach (DimensionAttribute attribute in referencedDimension.Attributes)
                    {
                        MeasureGroupAttribute mgAttr = referenceMeasuregroupDimension.Attributes.Add(attribute.ID);
                        if (relationship.AmoRelationship.ToRelationshipEnd.Attributes.Contains(attribute.ID))
                        {
                            mgAttr.Type = MeasureGroupAttributeType.Granularity;
                        }
                        foreach (DataItem di in attribute.KeyColumns)
                        {
                            mgAttr.KeyColumns.Add(di.Clone());
                        }
                    }

                    Dimension intermediateDimension = _tables.FindById(relationship.AmoRelationship.FromRelationshipEnd.DimensionID).AmoDimension;
                    referenceMeasuregroupDimension.IntermediateCubeDimensionID = intermediateDimension.ID;
                    referenceMeasuregroupDimension.IntermediateGranularityAttributeID = relationship.AmoRelationship.FromRelationshipEnd.Attributes[0].AttributeID;

                    // these last properties are only set if the intermediate dimension is the fact dimension
                    if (intermediateDimension.ID == degenerateDimensionId)
                    {
                        referenceMeasuregroupDimension.Materialization = ReferenceDimensionMaterialization.Regular;
                        referenceMeasuregroupDimension.RelationshipID = relationship.Id;
                    }

                    measureGroup.Dimensions.Add(referenceMeasuregroupDimension);

                    foreach (Relationship referenceChainRelationship in _tables.FindById(relationship.AmoRelationship.ToRelationshipEnd.DimensionID).Relationships)
                    {
                        PopulateReferenceDimension(measureGroup, degenerateDimensionId, referenceChainRelationship);
                    }
                }
            }
        }

        private void DeleteAlternateActiveRelationship(Relationship relationship)
        {
            // remove from db's active relationshps collection
            relationship.Table.TabularModel.ActiveRelationshipIds.Remove(relationship.Id);

            // We also need to check all the existing reference relationships (it's possible it's in there)
            foreach (MeasureGroup measureGroup in _amoDatabase.Cubes[0].MeasureGroups)
            {
                List<string> measureGroupDimensionIdsToDelete = new List<string>();
                foreach (MeasureGroupDimension measureGroupDimension in measureGroup.Dimensions)
                {
                    if (measureGroupDimension is ReferenceMeasureGroupDimension)
                    {
                        ReferenceMeasureGroupDimension referenceMeasureGroupDimension = ((ReferenceMeasureGroupDimension)measureGroupDimension);

                        if (referenceMeasureGroupDimension.IntermediateCubeDimensionID == relationship.AmoRelationship.FromRelationshipEnd.DimensionID &&
                            referenceMeasureGroupDimension.CubeDimensionID == relationship.AmoRelationship.ToRelationshipEnd.DimensionID &&
                            relationship.AmoRelationship.FromRelationshipEnd.Attributes.Contains(referenceMeasureGroupDimension.IntermediateGranularityAttributeID))
                        {
                            referenceMeasureGroupDimension.IntermediateCubeDimensionID = null;
                            measureGroupDimensionIdsToDelete.Add(measureGroupDimension.CubeDimensionID);
                        }
                    }
                }
                foreach (string dimensionId in measureGroupDimensionIdsToDelete)
                {
                    measureGroup.Dimensions.Remove(dimensionId);
                }
            }
        }

        /// <summary>
        /// Check whether the TabularModel object contains a relationship.
        /// </summary>
        /// <param name="relationshipId">The id of the relationship.</param>
        /// <returns>True if found; false if not.</returns>
        public bool ContainsRelationship(string relationshipId)
        {
            bool foundRelationship = false;

            foreach (Table table in _tables)
            {
                foreach (Relationship relationship in table.Relationships)
                {
                    if (relationship.Id == relationshipId)
                    {
                        foundRelationship = true;
                        break;
                    }
                }
                if (foundRelationship)
                {
                    break;
                }
            }

            return foundRelationship;
        }

        /// <summary>
        /// Find a relationship by its id.
        /// </summary>
        /// <param name="relationshipId">The id of the relationship.</param>
        /// <returns>Relationship if found; null if not.</returns>
        public Relationship FindRelationshipById(string relationshipId)
        {
            Relationship returnRelationship = null;

            foreach (Table table in _tables)
            {
                foreach (Relationship relationship in table.Relationships)
                {
                    if (relationship.Id == relationshipId)
                    {
                        returnRelationship = relationship;
                        break;
                    }
                }
                if (returnRelationship != null)
                {
                    break;
                }
            }

            return returnRelationship;
        }


        #region DataSources

        /// <summary>
        /// Delete datasource associated with the TabularModel object.
        /// </summary>
        /// <param name="id">The id of the datasource to be deleted.</param>
        public void DeleteDataSource(string id)
        {
            if (_amoDatabase.DataSources.Contains(id))
            {
                _amoDatabase.DataSources.Remove(id);
            }

            //check if DataSourceViews[0].DataSourceID refers to the datasource to be deleted
            if (_amoDatabase.DataSourceViews.Count > 0 && _amoDatabase.DataSourceViews[0].DataSourceID == id)
            {
                //set it to the first data source in the cube (should be fine because all the existing tables that use this datasource will also be deleted)
                if (_amoDatabase.DataSources.Count > 0)
                {
                    _amoDatabase.DataSourceViews[0].DataSourceID = _amoDatabase.DataSources[0].ID;
                }
                else
                {
                    _amoDatabase.DataSourceViews[0].DataSourceID = null;
                }
            }            

            // shell model
            if (_dataSources.ContainsId(id))
            {
                _dataSources.RemoveById(id);
            }
        }

        /// <summary>
        /// Create data source associated with the TabularModel object.
        /// </summary>
        /// <param name="dataSourceSource">DataSource object from the source tabular model to be created in the target.</param>
        public void CreateDataSource(DataSource dataSourceSource)
        {
            Microsoft.AnalysisServices.DataSource amoDataSourceTarget = dataSourceSource.AmoDataSource.Clone();

            // Need to check if there is an existing datasource with same ID (some clever clogs might have renamed the object in source and kept same ID).  If so, replace it with a new one and store it as substitute ID in source.
            if (_amoDatabase.DataSources.Contains(dataSourceSource.Id))
            {
                amoDataSourceTarget.ID = Convert.ToString(Guid.NewGuid());
                dataSourceSource.SubstituteId = amoDataSourceTarget.ID;
            }

            _amoDatabase.DataSources.Add(amoDataSourceTarget);

            // in the event we deleted the only datasource in the DeleteDataSource method above, ...
            if (_amoDatabase.DataSourceViews.Count > 0 && _amoDatabase.DataSourceViews[0].DataSourceID == null)
            {
                _amoDatabase.DataSourceViews[0].DataSourceID = amoDataSourceTarget.ID;
            }

            // shell model
            _dataSources.Add(new DataSource(this, amoDataSourceTarget));
        }

        /// <summary>
        /// Update datasource associated with the TabularModel object.
        /// </summary>
        /// <param name="dataSourceSource">DataSource object from the source tabular model to be updated in the target.</param>
        /// <param name="dataSourceTarget">DataSource object in the target tabular model to be updated.</param>
        public void UpdateDataSource(DataSource dataSourceSource, DataSource dataSourceTarget)
        {
            dataSourceTarget.AmoDataSource.ConnectionString = dataSourceSource.AmoDataSource.ConnectionString;

            if (dataSourceSource.Id != dataSourceTarget.Id)
            {
                // If the names are the same, but the IDs are different, need to store the ID from the target in the source connection so that when Create/Update subsequent tables (partitions, DSVs and special dimension properties), we know to substitute the Connection ID
                dataSourceSource.SubstituteId = dataSourceTarget.Id;
            }
        }

        #endregion

        #region Tables

        /// <summary>
        /// Delete table associated with the TabularModel object.
        /// </summary>
        /// <param name="id">Id of the table to be deleted.</param>
        /// <param name="deleteChildRelatoinships">Flag indicatign whether to delete child relationships of the table.</param>
        public void DeleteTable(string id, bool deleteChildRelatoinships = true)
        {
            if (deleteChildRelatoinships)
            {
                // Check if any other tables refer to the one about to be deleted - if so, delete relationship
                foreach (Table table in _tables)
                {
                    List<string> relationshipIdsToDelete = new List<string>(); //can't remove from collection whilst iterating it
                    foreach (Relationship relationship in table.Relationships)
                    {
                        if (relationship.AmoRelationship.ToRelationshipEnd.DimensionID == id)
                        {
                            relationshipIdsToDelete.Add(relationship.Id);
                        }
                    }
                    foreach (string relationshipId in relationshipIdsToDelete)
                    {
                        table.DeleteRelationship(relationshipId);
                    }
                }
            }

            // DSV table
            if (_amoDatabase.DataSourceViews[0].Schema.Tables.Contains(id))
            {
                _amoDatabase.DataSourceViews[0].Schema.Tables.Remove(id);
            }

            // Dim/Measure group
            if (_amoDatabase.Cubes[0].Dimensions.Contains(id))
            {
                if (_amoDatabase.Cubes[0].MeasureGroups.Contains(id))
                {
                    _amoDatabase.Cubes[0].MeasureGroups[id].Measures.Clear();
                    _amoDatabase.Cubes[0].MeasureGroups[id].Partitions.Clear();
                    _amoDatabase.Cubes[0].MeasureGroups.Remove(id, true);
                }
                _amoDatabase.Cubes[0].Dimensions.Remove(id, true);
                _amoDatabase.Dimensions.Remove(id);
            }

            // shell model
            if (_tables.ContainsId(id))
            {
                _tables.RemoveById(id);
            }
        }

        /// <summary>
        /// Create table associated with the TabularModel object.
        /// </summary>
        /// <param name="tableSource">Table object from the source tabular model to be created in the target.</param>
        /// <param name="sourceObjectSubstituteId">Substitute id from the source table.</param>
        /// <param name="useSubstituteId">Flag indicating whether use of the substitute id is required.</param>
        public void CreateTable(Table tableSource, ref string sourceObjectSubstituteId, ref bool useSubstituteId)
        {
            #region If blank db, need to create dsv/cube/mdx script

            if (_amoDatabase.Cubes.Count == 0)
            {
                string newDataSourceViewName = tableSource.TabularModel.AmoDatabase.DataSourceViews[0].Name;
                DataSet newDataSourceViewDataSet = new DataSet(newDataSourceViewName);
                DataSourceView newDatasourceView = _amoDatabase.DataSourceViews.AddNew(newDataSourceViewName, newDataSourceViewName);
                newDatasourceView.DataSourceID = tableSource.TabularModel.DataSources.FindById(tableSource.DataSourceID).SubstituteId;
                newDatasourceView.Schema = newDataSourceViewDataSet;

                Cube sandboxCube = _amoDatabase.Cubes.Add(tableSource.TabularModel.AmoDatabase.Cubes[0].Name, tableSource.TabularModel.AmoDatabase.Cubes[0].ID);
                sandboxCube.Source = new DataSourceViewBinding(newDatasourceView.ID);
                sandboxCube.StorageMode = StorageMode.InMemory;
                sandboxCube.Language = tableSource.TabularModel.AmoDatabase.Language;
                sandboxCube.Collation = tableSource.TabularModel.AmoDatabase.Collation;

                //Create initial MdxScript
                MdxScript mdxScript = sandboxCube.MdxScripts.Add(tableSource.TabularModel.AmoDatabase.Cubes[0].MdxScripts[0].Name, tableSource.TabularModel.AmoDatabase.Cubes[0].MdxScripts[0].ID);
                mdxScript.Commands.Add(new Microsoft.AnalysisServices.Command(tableSource.TabularModel.AmoDatabase.Cubes[0].MdxScripts[0].Commands[0].Text));
            }
            // check to add 2nd command here just in case get a cube with only the first default command populated
            if (_amoDatabase.Cubes[0].MdxScripts[0].Commands.Count == 1)
            {
                _amoDatabase.Cubes[0].MdxScripts[0].Commands.Add(new Microsoft.AnalysisServices.Command(""));  //blank 2nd command to hold measures
            }

            #endregion

            #region Need to check if there is an existing table with same ID (some clever clogs might have renamed the object in source and kept same ID).  If so, replace it with a new one and store it as substitute ID in source.

            if (_amoDatabase.Dimensions.Contains(tableSource.Id))
            {
                tableSource.SubstituteId = tableSource.Name + "_" + Convert.ToString(Guid.NewGuid());
                sourceObjectSubstituteId = tableSource.SubstituteId;
                useSubstituteId = true;
            }

            string substituteDataSourceId = tableSource.TabularModel.DataSources.FindById(tableSource.DataSourceID).SubstituteId;

            #endregion

            #region DSV Table

            if (tableSource.AmoTable != null)
            {
                //DataTable tableTarget = tableSource.AmoTable.Clone();
                DataTable tableTarget = tableSource.AmoTable.Copy();
                tableTarget.ExtendedProperties["DataSourceID"] = substituteDataSourceId;
                if (useSubstituteId) tableTarget.TableName = tableSource.SubstituteId;

                if (_amoDatabase.DataSourceViews[0].Schema.Tables.Contains(tableTarget.TableName))
                {
                    _amoDatabase.DataSourceViews[0].Schema.Tables.Remove(tableTarget.TableName);
                }

                _amoDatabase.DataSourceViews[0].Schema.Tables.Add(tableTarget);
            }
            #endregion

            #region Dimension / Relationships

            Dimension dimensionTarget = tableSource.AmoDimension.Clone();
            if (tableSource.AmoDimension.Source is DataSourceViewBinding)
            {
                dimensionTarget.Source = new DataSourceViewBinding(_amoDatabase.DataSourceViews[0].ID);
            }

            if (useSubstituteId)
            {
                dimensionTarget.ID = tableSource.SubstituteId;

                foreach (DimensionAttribute attribute in dimensionTarget.Attributes)
                {
                    foreach (DataItem keyColumn in attribute.KeyColumns)
                    {
                        if (keyColumn.Source is ColumnBinding && ((ColumnBinding)keyColumn.Source).TableID != tableSource.SubstituteId)
                        {
                            ((ColumnBinding)keyColumn.Source).TableID = tableSource.SubstituteId;
                        }
                    }
                    if (attribute.NameColumn.Source is ColumnBinding && ((ColumnBinding)attribute.NameColumn.Source).TableID != tableSource.SubstituteId)
                    {
                        ((ColumnBinding)attribute.NameColumn.Source).TableID = tableSource.SubstituteId;
                    }
                }
            }

            // clear all relationships inherited from source table; they will be added back later only if required
            dimensionTarget.Relationships.Clear();
            _amoDatabase.Dimensions.Add(dimensionTarget);
            if (useSubstituteId) _amoDatabase.Cubes[0].Dimensions.Add(tableSource.SubstituteId);

            if (!_amoDatabase.Cubes[0].Dimensions.Contains(tableSource.SubstituteId))
            {
                _amoDatabase.Cubes[0].Dimensions.Add(tableSource.SubstituteId);
            }

            if (tableSource.AmoCubeDimension != null && tableSource.AmoCubeDimension.Visible == false)
            {
                _amoDatabase.Cubes[0].Dimensions[tableSource.SubstituteId].Visible = false;
            }

            #endregion

            #region Measure Group

            MeasureGroup measureGroupTarget = tableSource.AmoMeasureGroup.Clone();

            if (useSubstituteId)
            {
                measureGroupTarget.ID = tableSource.SubstituteId;

                string measureGroupDimensionIdToRename = ""; //can't rename it while in MeasureGroupDimension (.Dimensions) collection
                foreach (MeasureGroupDimension measureGroupDimension in measureGroupTarget.Dimensions)
                {
                    if (measureGroupDimension is DegenerateMeasureGroupDimension)
                    {
                        measureGroupDimensionIdToRename = measureGroupDimension.CubeDimensionID;
                        break;
                    }
                }
                if (measureGroupDimensionIdToRename != "")
                {
                    DegenerateMeasureGroupDimension measureGroupDimensionClone = (DegenerateMeasureGroupDimension)measureGroupTarget.Dimensions[measureGroupDimensionIdToRename].Clone();
                    measureGroupDimensionClone.CubeDimensionID = tableSource.SubstituteId;
                    foreach (MeasureGroupAttribute attribute in measureGroupDimensionClone.Attributes)
                    {
                        foreach (DataItem keyColumn in attribute.KeyColumns)
                        {
                            if (keyColumn.Source is ColumnBinding && ((ColumnBinding)keyColumn.Source).TableID != tableSource.SubstituteId)
                            {
                                ((ColumnBinding)keyColumn.Source).TableID = tableSource.SubstituteId;
                            }
                        }
                    }

                    if (measureGroupTarget.Measures.Contains(measureGroupDimensionIdToRename))
                    {
                        Microsoft.AnalysisServices.Measure measureClone = measureGroupTarget.Measures[measureGroupDimensionIdToRename].Clone();
                        measureClone.ID = tableSource.SubstituteId;
                        if (measureClone.Source.Source is RowBinding && ((RowBinding)measureClone.Source.Source).TableID != tableSource.SubstituteId)
                        {
                            ((RowBinding)measureClone.Source.Source).TableID = tableSource.SubstituteId;
                        }

                        measureGroupTarget.Measures.Remove(measureGroupDimensionIdToRename);
                        measureGroupTarget.Measures.Add(measureClone);
                    }

                    measureGroupTarget.Dimensions.Remove(measureGroupDimensionIdToRename);
                    measureGroupTarget.Dimensions.Add(measureGroupDimensionClone);
                }
            }

            //Now make sure all partitions are hooked up
            List<string> partitionIdsToRename = new List<string>();
            foreach (Partition partition in measureGroupTarget.Partitions)
            {
                if (partition.Source is QueryBinding) partition.Source = new QueryBinding(substituteDataSourceId, ((QueryBinding)partition.Source).QueryDefinition);
                if (useSubstituteId && partition.ID == tableSource.Id) partitionIdsToRename.Add(partition.ID);
            }
            foreach (string partitionIdToRename in partitionIdsToRename)
            {
                Partition partition = measureGroupTarget.Partitions[partitionIdToRename];
                Partition partitionClone = partition.Clone();
                partitionClone.ID = tableSource.SubstituteId;
                measureGroupTarget.Partitions.Remove(partition.ID);
                measureGroupTarget.Partitions.Add(partitionClone);
            }

            //And finally add it to the target cube
            _amoDatabase.Cubes[0].MeasureGroups.Add(measureGroupTarget);

            #endregion

            #region Shell model

            _tables.Add(new Table(this, dimensionTarget));

            #endregion
        }

        /// <summary>
        /// Update relationships with substitute ids to avoid unique id conflict.
        /// </summary>
        /// <param name="oldTableId">Old table id.</param>
        /// <param name="newTableSubstituteId">New table substitute id.</param>
        public void UpdateRelationshipsWithSubstituteTableIds(string oldTableId, string newTableSubstituteId)
        {
            foreach (Table table in _tables)
            {
                foreach (Relationship relationship in table.Relationships)
                {
                    if (relationship.AmoRelationship.FromRelationshipEnd.DimensionID == oldTableId)
                    {
                        relationship.AmoRelationship.FromRelationshipEnd.DimensionID = newTableSubstituteId;
                    }
                    if (relationship.AmoRelationship.ToRelationshipEnd.DimensionID == oldTableId)
                    {
                        relationship.AmoRelationship.ToRelationshipEnd.DimensionID = newTableSubstituteId;
                    }
                }
            }
        }

        /// <summary>
        /// Update tablre associated with the TabularModel object.
        /// </summary>
        /// <param name="tableSource">Table object from the source tabular model to be updated in the target.</param>
        /// <param name="tableTarget">Table object in the target tabular model to be updated.</param>
        /// <param name="sourceObjectSubstituteId">Substitute id of the source object.</param>
        /// <param name="useSubstituteId">Flag indicating whether it is required to use the substitute id.</param>
        public void UpdateTable(Table tableSource, Table tableTarget, ref string sourceObjectSubstituteId, ref bool useSubstituteId)
        {
            if (tableSource.Id != tableTarget.Id)
            {
                // If the names are the same, but the IDs are different, need to store the ID from the target in the source Table so that maintains existing object relationships
                tableSource.SubstituteId = tableTarget.Id;
                sourceObjectSubstituteId = tableSource.SubstituteId;
                useSubstituteId = true;
            }

            #region Backup the target db for reference (otherwise perspectives can't get at dim attribute names)
            Database dbTargetBackup = _amoDatabase.Clone();
            #endregion

            Dimension dimensionTargetBackup = tableTarget.AmoDimension.Clone();
            DeleteTable(tableTarget.Id, deleteChildRelatoinships: false);
            CreateTable(tableSource, ref sourceObjectSubstituteId, ref useSubstituteId);
            //get back the newly created table
            tableTarget = _tables.FindById(tableSource.SubstituteId);
            tableTarget.AmoOldDimensionBackup = dimensionTargetBackup;

            #region Add back table/columns to perspectives if required
            foreach (Microsoft.AnalysisServices.Perspective perspective in _amoDatabase.Cubes[0].Perspectives)
            {
                if (dbTargetBackup.Cubes[0].Perspectives.Contains(perspective.ID))
                {
                    Microsoft.AnalysisServices.Perspective perspectiveBackup = dbTargetBackup.Cubes[0].Perspectives.Find(perspective.ID);
                    if (perspectiveBackup.Dimensions.Contains(tableTarget.Id))
                    {
                        PerspectiveDimension perspectiveDimensionBackup = perspectiveBackup.Dimensions.Find(tableTarget.Id);
                        PerspectiveDimension perspectiveDimension = perspective.Dimensions.Find(tableTarget.Id);

                        //table
                        if (perspectiveDimension == null)
                        {
                            perspectiveDimension = perspective.Dimensions.Add(tableTarget.Id);
                        }

                        //attributes
                        foreach (PerspectiveAttribute attributeBackup in perspectiveDimensionBackup.Attributes)
                        {
                            bool foundMatch = false;
                            foreach (PerspectiveAttribute attribute in perspectiveDimension.Attributes)
                            {
                                if (attributeBackup.Attribute.Name == attribute.Attribute.Name)
                                {
                                    foundMatch = true;
                                    break;
                                }
                            }
                            if (!foundMatch)
                            {
                                //we know the attribute is not already in the dim perspective.  Now see if it's in the actual dim.
                                DimensionAttribute dimAttribute = tableTarget.AmoDimension.Attributes.FindByName(attributeBackup.Attribute.Name);
                                if (dimAttribute != null)
                                {
                                    perspectiveDimension.Attributes.Add(dimAttribute.ID);
                                }
                            }
                        }

                        //hierarchies
                        foreach (PerspectiveHierarchy hierarchyBackup in perspectiveDimensionBackup.Hierarchies)
                        {
                            bool foundMatch = false;
                            foreach (PerspectiveHierarchy hierarchy in perspectiveDimension.Hierarchies)
                            {
                                if (hierarchyBackup.Hierarchy.Name == hierarchy.Hierarchy.Name)
                                {
                                    foundMatch = true;
                                    break;
                                }
                            }
                            if (!foundMatch)
                            {
                                //we know the hierarchy is not already in the dim perspective.  Now see if it's in the actual dim.
                                Hierarchy dimHierarchy = tableTarget.AmoDimension.Hierarchies.FindByName(hierarchyBackup.Hierarchy.Name);
                                if (dimHierarchy != null)
                                {
                                    perspectiveDimension.Hierarchies.Add(dimHierarchy.ID);
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            
            //Add back parent relationships from target clone (assuming necessary tables/columns exist).
            List<string> relationshipIdsToAddBack = new List<string>();
            foreach (Microsoft.AnalysisServices.Relationship amoRelationship in dimensionTargetBackup.Relationships)
            {
                if (_tables.ContainsId(amoRelationship.ToRelationshipEnd.DimensionID))
                {
                    relationshipIdsToAddBack.Add(amoRelationship.ID);
                }
            }
            foreach (string relationshipIdToAddBack in relationshipIdsToAddBack)
            {
                Microsoft.AnalysisServices.Relationship amoRelationship = dimensionTargetBackup.Relationships.Find(relationshipIdToAddBack);
                Table parentTable = _tables.FindById(amoRelationship.ToRelationshipEnd.DimensionID);

                // it is possible that the parent table's relationship column's ID has changed (if table been updated) ...
                if (!parentTable.AmoDimension.Attributes.Contains(amoRelationship.ToRelationshipEnd.Attributes[0].AttributeID))
                {
                    // get the Name from the backup
                    string nameOfAttributeWithWrongId = parentTable.AmoOldDimensionBackup.Attributes.Find(amoRelationship.ToRelationshipEnd.Attributes[0].AttributeID).Name;
                    RelationshipEndAttribute relationshipEndAttributeClone = amoRelationship.ToRelationshipEnd.Attributes[0].Clone();
                    DimensionAttribute parentAttribute = parentTable.AmoDimension.Attributes.FindByName(nameOfAttributeWithWrongId);
                    if (parentAttribute != null)
                    {
                        relationshipEndAttributeClone.AttributeID = parentAttribute.ID;
                        amoRelationship.ToRelationshipEnd.Attributes.Remove(amoRelationship.ToRelationshipEnd.Attributes[0].AttributeID);
                        amoRelationship.ToRelationshipEnd.Attributes.Add(relationshipEndAttributeClone);
                    }
                }
                // note: in this case we can pass in [parentTable.AmoDimension] as the 2nd parameter [parentDimSource] because we are just copying back the relationships that were already there in the target
                string warningMessage = "";
                tableTarget.CreateRelationship(amoRelationship, parentTable.AmoDimension, "", ref warningMessage, tableTarget.TabularModel.ActiveRelationshipIds.Contains(amoRelationship.ID));
            }
        }

        /// <summary>
        /// Update the relationships for children of updated tables to maintain referential integrity in the model.
        /// </summary>
        /// <param name="tableTarget">Target Table object.</param>
        public void UpdateRelationshipsForChildrenOfUpdatedTables(Table tableTarget)
        {
            //Now we have to check child relationships that referred to this table.  They might need to be deleted if the parent column is no longer there.
            //Or they might even use an AttributeId that is not valid, but the column is actually there (same name, different attributeId)
            foreach (Table table in _tables)
            {
                // might need to delete relationships, or modify the attributeids, but can't do while in collection
                List<string> relationshipIdsToDelete = new List<string>();

                foreach (Relationship relationship in table.Relationships)
                {
                    if (relationship.AmoRelationship.ToRelationshipEnd.DimensionID == tableTarget.Id)
                    {
                        Microsoft.AnalysisServices.Relationship amoRelationshipTemp = null;

                        foreach (RelationshipEndAttribute attribute in relationship.AmoRelationship.ToRelationshipEnd.Attributes)
                        {
                            DimensionAttribute dimAttributeParentBackup = tableTarget.AmoOldDimensionBackup.Attributes.Find(attribute.AttributeID);
                            if (dimAttributeParentBackup != null)  // will only be null if changed parent attribute id in UpdateTable - in which case don't need to worry about it
                            {
                                DimensionAttribute dimAttributeParent = tableTarget.AmoDimension.Attributes.FindByName(dimAttributeParentBackup.Name);
                                if (dimAttributeParent == null)
                                {
                                    //parent attribute is definitely not there (not even with a different id), so need to delete this relationship
                                    if (!relationshipIdsToDelete.Contains(relationship.Id))
                                    {
                                        relationshipIdsToDelete.Add(relationship.Id);
                                    }
                                    break;
                                }
                                else
                                {
                                    //parent attribute is there.  If has same Id, we are good. Othersise, need to change to new Id
                                    if (dimAttributeParentBackup.ID != dimAttributeParent.ID)
                                    {
                                        amoRelationshipTemp = relationship.AmoRelationship.Clone();
                                        RelationshipEndAttribute parentDimAttributeTemp = attribute.Clone();
                                        parentDimAttributeTemp.AttributeID = dimAttributeParent.ID;
                                        amoRelationshipTemp.ToRelationshipEnd.Attributes.Remove(attribute.AttributeID);
                                        amoRelationshipTemp.ToRelationshipEnd.Attributes.Add(parentDimAttributeTemp);
                                    }

                                    //check that the parent attribute allows only unique values
                                    if (dimAttributeParent.Parent.Attributes.Contains("RowNumber") &&
                                        dimAttributeParent.Parent.Attributes["RowNumber"].AttributeRelationships.Contains(dimAttributeParent.ID) &&
                                        dimAttributeParent.Parent.Attributes["RowNumber"].AttributeRelationships[dimAttributeParent.ID].Cardinality != Cardinality.One)
                                    {
                                        dimAttributeParent.Parent.Attributes["RowNumber"].AttributeRelationships[dimAttributeParent.ID].Cardinality = Cardinality.One;
                                        foreach (DataItem di in dimAttributeParent.KeyColumns)
                                        {
                                            di.NullProcessing = NullProcessing.Error;
                                        }
                                        if (_amoDatabase.Cubes.Count > 0)
                                        {
                                            foreach (MeasureGroup mg in _amoDatabase.Cubes[0].MeasureGroups)
                                            {
                                                if (mg.ID == dimAttributeParent.Parent.ID)
                                                {
                                                    foreach (MeasureGroupDimension mgd in mg.Dimensions)
                                                    {
                                                        if (mgd.CubeDimensionID == dimAttributeParent.Parent.ID && mgd is DegenerateMeasureGroupDimension)
                                                        {
                                                            foreach (MeasureGroupAttribute mga in ((DegenerateMeasureGroupDimension)mgd).Attributes)
                                                            {
                                                                if (mga.AttributeID == dimAttributeParent.ID)
                                                                {
                                                                    mga.KeyColumns[0].NullProcessing = NullProcessing.Error;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (amoRelationshipTemp != null) //i.e. we had to replace at least one attribute id
                        {
                            table.AmoDimension.Relationships.Remove(relationship.AmoRelationship.ID);
                            table.AmoDimension.Relationships.Add(amoRelationshipTemp);
                            relationship.AmoRelationship = amoRelationshipTemp;
                        }
                    }
                }
                foreach (string relationshipIdToDelete in relationshipIdsToDelete)
                {
                    table.DeleteRelationship(relationshipIdToDelete);
                }
            }
        }

        /// <summary>
        /// Check relationship validity to maintain referential integrity in the model.
        /// </summary>
        public void CheckRelationshipValidity()
        {
            //in rare cases where tables updated and old table had relationship to updated table, renamed tables, etc., need this safety net
            foreach (Table table in _tables)
            {
                List<string> relationshipIdsToDelete = new List<string>();
                foreach (Relationship relationship in table.Relationships)
                {
                    bool deleteRelationship = false;

                    foreach (RelationshipEndAttribute attribute in relationship.AmoRelationship.FromRelationshipEnd.Attributes)
                    {
                        if (!table.AmoDimension.Attributes.Contains(attribute.AttributeID))
                        {
                            if (!relationshipIdsToDelete.Contains(relationship.Id))
                            {
                                relationshipIdsToDelete.Add(relationship.Id);
                            }
                            deleteRelationship = true;
                            break;
                        }
                    }

                    if (!deleteRelationship)
                    {
                        Table parentTable = _tables.FindById(relationship.AmoRelationship.ToRelationshipEnd.DimensionID);
                        if (parentTable == null)
                        {
                            if (!relationshipIdsToDelete.Contains(relationship.Id))
                            {
                                relationshipIdsToDelete.Add(relationship.Id);
                            }
                        }
                        else
                        {
                            foreach (RelationshipEndAttribute attribute in relationship.AmoRelationship.ToRelationshipEnd.Attributes)
                            {
                                if (!parentTable.AmoDimension.Attributes.Contains(attribute.AttributeID))
                                {
                                    if (!relationshipIdsToDelete.Contains(relationship.Id))
                                    {
                                        relationshipIdsToDelete.Add(relationship.Id);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                foreach (string relationshipIdToDelete in relationshipIdsToDelete)
                {
                    table.DeleteRelationship(relationshipIdToDelete);
                }
            }
        }

        #endregion

        #region Measures

        /// <summary>
        /// Delete measure associated with the TabularModel object.
        /// </summary>
        /// <param name="id">The id of the measure to be deleted.</param>
        public void DeleteMeasure(string id)
        {
            DeleteCalculationProperty(_measures.FindById(id).CalculationReference);

            // shell model
            if (_measures.ContainsId(id))
            {
                _measures.RemoveById(id);
            }
        }

        /// <summary>
        /// Create measure associated with the TabularModel object.
        /// </summary>
        /// <param name="measureSource">Measure object from the source tabular model to be created in the target.</param>
        public void CreateMeasure(Measure measureSource)
        {
            CreateCalculationProperty(measureSource.AmoCalculationProperty, measureSource.CalculationReference);

            // shell model
            _measures.Add(new Measure(this, measureSource.TableName, measureSource.Name, measureSource.Expression));
        }

        /// <summary>
        /// Update measure associated with the TabularModel object.
        /// </summary>
        /// <param name="measureSource">Measure object from the source tabular model to be updated in the target.</param>
        /// <param name="measureTarget">Measure object in the target tabular model to be updated.</param>
        public void UpdateMeasure(Measure measureSource, Measure measureTarget)
        {
            DeleteCalculationProperty(measureTarget.CalculationReference);
            CreateCalculationProperty(measureSource.AmoCalculationProperty, measureSource.CalculationReference);

            measureTarget.Expression = measureSource.Expression;
        }

        #endregion

        #region KPIs

        /// <summary>
        /// Delete KPI associated with the TabularModel object.
        /// </summary>
        /// <param name="id">The id of the KPI to be deleted.</param>
        public void DeleteKpi(string id)
        {
            Kpi kpiToDelete = _kpis.FindById(id);

            DeleteCalculationProperty(kpiToDelete.CalculationReference);
            DeleteCalculationProperty(kpiToDelete.GoalCalculationReference);
            DeleteCalculationProperty(kpiToDelete.StatusCalculationReference);
            if (_amoDatabase.CompatibilityLevel < 1103) DeleteCalculationProperty(kpiToDelete.TrendCalculationReference);
            DeleteCalculationProperty(kpiToDelete.KpiCalculationReference);
            
            //_amoDatabase.Cubes[0].Kpis.Remove(kpiToDelete.AmoKpi.ID);

            // shell model
            _kpis.RemoveById(id);
        }

        /// <summary>
        /// Create KPI associated with the TabularModel object.
        /// </summary>
        /// <param name="kpiSource">Kpi object from the source tabular model to be created in the target.</param>
        public void CreateKpi(Kpi kpiSource)
        {
            /* No longer using AMO KPI model since latest version of Tabular Editor has switched to use MDX script declarations instead */
            //if (kpiSource.AmoKpi != null)
            //{
            //    Kpi amoKpiTarget = kpiSource.AmoKpi.Clone();

            //    // Need to check if there is an existing KPI with same ID (some clever clogs might have renamed the object in source and kept same ID).  If so, replace it with a new one and store it as substitute ID in source.
            //    if (_amoDatabase.Cubes[0].Kpis.Contains(kpiSource.Id))
            //    {
            //        amoKpiTarget.ID = Convert.ToString(Guid.NewGuid());
            //        kpiSource.SubstituteId = amoKpiTarget.ID;
            //    }
            
            //    _amoDatabase.Cubes[0].Kpis.Add(amoKpiTarget);
            //}

            CreateCalculationProperty(kpiSource.AmoCalculationProperty, kpiSource.CalculationReference);
            CreateCalculationProperty(kpiSource.AmoGoalCalculationProperty, kpiSource.GoalCalculationReference);
            CreateCalculationProperty(kpiSource.AmoStatusCalculationProperty, kpiSource.StatusCalculationReference);
            //CreateCalculationProperty(kpiSource.AmoTrendCalculationProperty, kpiSource.TrendCalculationReference);
            CreateCalculationProperty(kpiSource.AmoKpiCalculationProperty, kpiSource.KpiCalculationReference);

            // shell model
            _kpis.Add(new Kpi(this, kpiSource.TableName, kpiSource.Name, kpiSource.Expression, kpiSource.GoalMeasure, kpiSource.StatusMeasure, kpiSource.TrendMeasure, kpiSource.StatusGraphic, kpiSource.TrendGraphic)); //, amoKpiTarget));
        }

        /// <summary>
        /// Update KPI associated with the TabularModel object.
        /// </summary>
        /// <param name="kpiSource">KPI object from the source tabular model to be updated in the target.</param>
        /// <param name="kpiTarget">KPI object in the target tabular model to be updated.</param>
        public void UpdateKpi(Kpi kpiSource, Kpi kpiTarget)
        {
            //base measure
            DeleteCalculationProperty(kpiTarget.CalculationReference);
            CreateCalculationProperty(kpiSource.AmoCalculationProperty, kpiSource.CalculationReference);
            kpiTarget.Expression = kpiSource.Expression;

            //goal
            DeleteCalculationProperty(kpiTarget.GoalCalculationReference);
            CreateCalculationProperty(kpiSource.AmoGoalCalculationProperty, kpiSource.GoalCalculationReference);
            kpiTarget.GoalMeasure.Expression = kpiSource.GoalMeasure.Expression;

            //status
            DeleteCalculationProperty(kpiTarget.StatusCalculationReference);
            CreateCalculationProperty(kpiSource.AmoStatusCalculationProperty, kpiSource.StatusCalculationReference);
            kpiTarget.StatusMeasure.Expression = kpiSource.StatusMeasure.Expression;

            if (_amoDatabase.CompatibilityLevel < 1103)
            {
                //trend
                DeleteCalculationProperty(kpiTarget.TrendCalculationReference);
                CreateCalculationProperty(kpiSource.AmoTrendCalculationProperty, kpiSource.TrendCalculationReference);
                kpiTarget.TrendMeasure.Expression = kpiSource.TrendMeasure.Expression;
            }

            //kpi calc ref
            DeleteCalculationProperty(kpiTarget.KpiCalculationReference);
            CreateCalculationProperty(kpiSource.AmoKpiCalculationProperty, kpiSource.KpiCalculationReference);
        }

        #endregion

        #region Actions

        /// <summary>
        /// Delete action associated with the TabularModel object.
        /// </summary>
        /// <param name="id">The id of the action to be deleted.</param>
        public void DeleteAction(string id)
        {
            if (_amoDatabase.Cubes.Count > 0)
            {
                if (_amoDatabase.Cubes[0].Actions.Contains(id))
                {
                    _amoDatabase.Cubes[0].Actions.Remove(id);
                }
            }

            // shell model
            if (_actions.ContainsId(id))
            {
                _actions.RemoveById(id);
            }
        }

        /// <summary>
        /// Create action associated with the TabularModel object.
        /// </summary>
        /// <param name="actionSource">Action object from the source tabular model to be created in the target.</param>
        public void CreateAction(Action actionSource)
        {
            if (_amoDatabase.Cubes.Count > 0)
            {

                Microsoft.AnalysisServices.Action amoActionTarget = actionSource.AmoAction.Clone();

                // Need to check if there is an existing Action with same ID (some clever clogs might have renamed the object in source and kept same ID).  If so, replace it with a new one and store it as substitute ID in source.
                if (_amoDatabase.Cubes[0].Actions.Contains(actionSource.Id))
                {
                    amoActionTarget.ID = Convert.ToString(Guid.NewGuid());
                    actionSource.SubstituteId = amoActionTarget.ID;
                }

                _amoDatabase.Cubes[0].Actions.Add(amoActionTarget);

                // shell model
                _actions.Add(new Action(this, amoActionTarget));
            }
        }

        /// <summary>
        /// Update action associated with the TabularModel object.
        /// </summary>
        /// <param name="ActionSource">Action object from the source tabular model to be updated in the target.</param>
        /// <param name="ActionTarget">Action object in the target tabular model to be updated.</param>
        public void MergeAction(Action ActionSource, Action ActionTarget)
        {
            if (ActionSource.Id != ActionTarget.Id)
            {
                // If the names are the same, but the IDs are different, need to store the ID from the target in the source Action so that when Create/Update subsequent tables (partitions, DSVs and special dimension properties), we know to substitute the Action ID
                ActionSource.SubstituteId = ActionTarget.Id;
            }

            DeleteAction(ActionTarget.Id);
            CreateAction(ActionSource);
        }

        #endregion

        #region Perspectives

        /// <summary>
        /// Delete perspective associated with the TabularModel object.
        /// </summary>
        /// <param name="id">The id of the perspective to be deleted.</param>
        public void DeletePerspective(string id)
        {
            if (_amoDatabase.Cubes[0].Perspectives.Contains(id))
            {
                _amoDatabase.Cubes[0].Perspectives.Remove(id);
            }

            // shell model
            if (_perspectives.ContainsId(id))
            {
                _perspectives.RemoveById(id);
            }
        }

        /// <summary>
        /// Create perspective associated with the TabularModel object.
        /// </summary>
        /// <param name="perspectiveSource">Perspective object from the source tabular model to be created in the target.</param>
        public void CreatePerspective(Perspective perspectiveSource)
        {
            if (_amoDatabase.Cubes.Count > 0)
            {
                //easier to just create a copy rather than clone ...

                Microsoft.AnalysisServices.Perspective amoPerspectiveTarget = _amoDatabase.Cubes[0].Perspectives.Add(perspectiveSource.Name);

                //Tables
                foreach (PerspectiveDimension perspectiveDimensionSource in perspectiveSource.AmoPerspective.Dimensions)
                {
                    Table tableTarget = _tables.FindByName(perspectiveDimensionSource.Dimension.Name);

                    if (tableTarget != null)
                    {
                        PerspectiveDimension perspectiveDimensionTarget = amoPerspectiveTarget.Dimensions.Add(tableTarget.AmoDimension.ID);

                        //Columns
                        foreach (PerspectiveAttribute perspectiveAttributeSource in perspectiveDimensionSource.Attributes)
                        {
                            DimensionAttribute dimensionAttributeTarget = tableTarget.AmoDimension.Attributes.FindByName(perspectiveAttributeSource.Attribute.Name);

                            if (dimensionAttributeTarget != null)
                            {
                                perspectiveDimensionTarget.Attributes.Add(dimensionAttributeTarget.ID);
                            }
                        }

                        //Hierarchies
                        foreach (PerspectiveHierarchy perspectiveHierarchySource in perspectiveDimensionSource.Hierarchies)
                        {
                            Hierarchy hierarchyTarget = tableTarget.AmoDimension.Hierarchies.FindByName(perspectiveHierarchySource.Hierarchy.Name);

                            if (hierarchyTarget != null)
                            {
                                perspectiveDimensionTarget.Hierarchies.Add(hierarchyTarget.ID);
                            }
                        }
                    }
                }

                //Measures
                foreach (PerspectiveCalculation perspectiveCalculationSource in perspectiveSource.AmoPerspective.Calculations)
                {
                    string measureName = perspectiveCalculationSource.Name.Replace("[Measures].[", "").Replace("]", "");

                    if (perspectiveSource.ParentTabularModel.Measures.ContainsName(measureName)) // this if clause shouldn't be necessary, but it is
                    {
                        Measure measureTarget = _measures.FindByName(measureName);

                        if (measureTarget != null)
                        {
                            amoPerspectiveTarget.Calculations.Add(perspectiveCalculationSource.Name);
                        }
                    }
                }

                //KPIs
                foreach (PerspectiveKpi perspectiveKpiSource in perspectiveSource.AmoPerspective.Kpis)
                {
                    string KpiName = perspectiveKpiSource.ToString();

                    if (perspectiveSource.ParentTabularModel.Kpis.ContainsName(KpiName))
                    {
                        Kpi kpiTarget = _kpis.FindByName(KpiName);

                        if (kpiTarget != null)
                        {
                            amoPerspectiveTarget.Kpis.Add(perspectiveKpiSource.ToString());
                        }
                    }
                }

                //Actions
                foreach (PerspectiveAction perspectiveActionSource in perspectiveSource.AmoPerspective.Actions)
                {
                    if (perspectiveActionSource.ParentCube.Actions.Contains(perspectiveActionSource.ActionID))  //need this check or .Action returns error
                    {
                        string actionName = perspectiveActionSource.Action.Name;

                        if (perspectiveSource.ParentTabularModel.Actions.ContainsName(actionName))
                        {
                            Action actionTarget = _actions.FindByName(actionName);

                            if (actionTarget != null)
                            {
                                amoPerspectiveTarget.Actions.Add(actionTarget.Id);
                            }
                        }
                    }
                }

                //Translations
                foreach (Translation perspectiveTranslationSource in perspectiveSource.AmoPerspective.Translations)
                {
                    Translation perspectiveTranslationTarget = perspectiveTranslationSource.Clone();
                    amoPerspectiveTarget.Translations.Add(perspectiveTranslationTarget);
                }

                // shell model
                _perspectives.Add(new Perspective(this, amoPerspectiveTarget));
            }
        }

        /// <summary>
        /// Update perspective associated with the TabularModel object.
        /// </summary>
        /// <param name="perspectiveSource">Perspective object from the source tabular model to be updated in the target.</param>
        /// <param name="perspectiveTarget">Perspective object in the target tabular model to be updated.</param>
        public void UpdatePerspective(Perspective perspectiveSource, Perspective perspectiveTarget)
        {
            if (_comparisonInfo.OptionsInfo.OptionMergePerspectives)
            {
                //Tables
                foreach (PerspectiveDimension perspectiveDimensionSource in perspectiveSource.AmoPerspective.Dimensions)
                {
                    PerspectiveDimension perspectiveDimensionTarget = null;

                    //is this table selected in the target perspective?
                    foreach (PerspectiveDimension perspectiveDimensionTarget2 in perspectiveTarget.AmoPerspective.Dimensions)
                    {
                        if (perspectiveDimensionTarget2.Dimension.Name == perspectiveDimensionSource.Dimension.Name)
                        {
                            perspectiveDimensionTarget = perspectiveDimensionTarget2;
                            break;
                        }
                    }

                    //If perspectiveDimensionTarget == null, then this table is not selected in the target perspective.  But does it exist in the target db?  if so, we should select it.
                    if (perspectiveDimensionTarget == null && _amoDatabase.Dimensions.ContainsName(perspectiveDimensionSource.Dimension.Name))
                    {
                        perspectiveDimensionTarget = perspectiveTarget.AmoPerspective.Dimensions.Add(_amoDatabase.Dimensions.FindByName(perspectiveDimensionSource.Dimension.Name).ID);
                    }

                    //if perspectiveDimensionTarget is still null here then we don't have a matching table in the target perspective at all, and we can move onto the next table
                    if (perspectiveDimensionTarget != null)
                    {
                        //Columns
                        foreach (PerspectiveAttribute perspectiveAttributeSource in perspectiveDimensionSource.Attributes)
                        {
                            PerspectiveAttribute perspectiveAttributeTarget = null;

                            foreach (PerspectiveAttribute perspectiveAttributeTarget2 in perspectiveDimensionTarget.Attributes)
                            {
                                if (perspectiveAttributeTarget2.Attribute.Name == perspectiveAttributeSource.Attribute.Name)
                                {
                                    perspectiveAttributeTarget = perspectiveAttributeTarget2;
                                    break;
                                }
                            }

                            if (perspectiveAttributeTarget == null)
                            {
                                //There is no selection in the target dim for this attribute.  Is there an attribute in the target dim with the same name?
                                if (perspectiveDimensionTarget.Dimension.Attributes.ContainsName(perspectiveAttributeSource.Attribute.Name))
                                {
                                    perspectiveAttributeTarget = perspectiveDimensionTarget.Attributes.Add(perspectiveDimensionTarget.Dimension.Attributes.FindByName(perspectiveAttributeSource.Attribute.Name).ID);
                                }
                                else break; //attribute doesn't exist in target dim, so move onto the next attribute
                            }
                        }

                        //Hierarchies
                        foreach (PerspectiveHierarchy perspectiveHierarchySource in perspectiveDimensionSource.Hierarchies)
                        {
                            PerspectiveHierarchy perspectiveHierarchyTarget = null;

                            foreach (PerspectiveHierarchy perspectiveHierarchyTarget2 in perspectiveDimensionTarget.Hierarchies)
                            {
                                if (perspectiveHierarchyTarget2.Hierarchy.Name == perspectiveHierarchySource.Hierarchy.Name)
                                {
                                    perspectiveHierarchyTarget = perspectiveHierarchyTarget2;
                                    break;
                                }
                            }

                            if (perspectiveHierarchyTarget == null)
                            {
                                //There is no selection in the target dim for this hierarchy.  Is there a hierarchy in the target dim with the same name?
                                if (perspectiveDimensionTarget.Dimension.Hierarchies.ContainsName(perspectiveHierarchySource.Hierarchy.Name))
                                {
                                    perspectiveHierarchyTarget = perspectiveDimensionTarget.Hierarchies.Add(perspectiveDimensionTarget.Dimension.Hierarchies.FindByName(perspectiveHierarchySource.Hierarchy.Name).ID);
                                }
                                else break; //hierarchy doesn't exist in target dim, so move onto the next hierarchy
                            }
                        }
                    }
                }

                //Measures
                foreach (PerspectiveCalculation perspectiveCalculationSource in perspectiveSource.AmoPerspective.Calculations)
                {
                    PerspectiveCalculation perspectiveCalculationTarget = null;

                    foreach (PerspectiveCalculation perspectiveCalculationTarget2 in perspectiveTarget.AmoPerspective.Calculations)
                    {
                        if (perspectiveCalculationTarget2.Name == perspectiveCalculationSource.Name)
                        {
                            perspectiveCalculationTarget = perspectiveCalculationTarget2;
                            break;
                        }
                    }

                    if (perspectiveCalculationTarget == null)
                    {
                        //There is no selection in the target db for this calculation.  Is there a calculation in the target db with the same name?
                        if (perspectiveTarget.ParentTabularModel.Measures.ContainsName(perspectiveCalculationSource.Name.Replace("[Measures].[", "").Replace("]", "")))
                        {
                            perspectiveCalculationTarget = perspectiveTarget.AmoPerspective.Calculations.Add(perspectiveCalculationSource.Name);
                        }
                    }
                }

                //Kpis
                foreach (PerspectiveKpi perspectiveKpiSource in perspectiveSource.AmoPerspective.Kpis)
                {
                    PerspectiveKpi perspectiveKpiTarget = null;

                    foreach (PerspectiveKpi perspectiveKpiTarget2 in perspectiveTarget.AmoPerspective.Kpis)
                    {
                        if (perspectiveKpiTarget2.ToString() == perspectiveKpiSource.ToString())
                        {
                            perspectiveKpiTarget = perspectiveKpiTarget2;
                            break;
                        }
                    }

                    if (perspectiveKpiTarget == null)
                    {
                        //There is no selection in the target db for this Kpi.  Is there a Kpi in the target db with the same name?
                        if (perspectiveTarget.ParentTabularModel.Kpis.ContainsName(perspectiveKpiSource.ToString()))
                        {
                            perspectiveKpiTarget = perspectiveTarget.AmoPerspective.Kpis.Add(perspectiveKpiSource.ToString());
                        }
                    }
                }

                //Actions
                foreach (PerspectiveAction perspectiveActionSource in perspectiveSource.AmoPerspective.Actions)
                {
                    if (perspectiveActionSource.ParentCube.Actions.Contains(perspectiveActionSource.ActionID))  //need this check or .Action returns error
                    {
                        PerspectiveAction perspectiveActionTarget = null;

                        foreach (PerspectiveAction perspectiveActionTarget2 in perspectiveTarget.AmoPerspective.Actions)
                        {
                            if (perspectiveActionTarget2.ParentCube.Actions.Contains(perspectiveActionTarget2.ActionID) &&  //need this check or .Action returns error
                                perspectiveActionTarget2.Action.Name == perspectiveActionSource.Action.Name)
                            {
                                perspectiveActionTarget = perspectiveActionTarget2;
                                break;
                            }
                        }

                        if (perspectiveActionTarget == null)
                        {
                            //There is no selection in the target db for this Action.  Is there an action in the target db with the same name?
                            if (perspectiveTarget.ParentTabularModel.Actions.ContainsName(perspectiveActionSource.Action.Name))
                            {
                                perspectiveActionTarget = perspectiveTarget.AmoPerspective.Actions.Add(_amoDatabase.Cubes[0].Actions.FindByName(perspectiveActionSource.Action.Name).ID);
                            }
                        }
                    }
                }

                //Translations
                foreach (Translation perspectiveTranslationSource in perspectiveSource.AmoPerspective.Translations)
                {
                    if (perspectiveTarget.AmoPerspective.Translations.Contains(perspectiveTranslationSource.Language))
                    {
                        perspectiveTarget.AmoPerspective.Translations.FindByLanguage(perspectiveTranslationSource.Language).Caption = perspectiveTranslationSource.Caption;
                    }
                    else
                    {
                        Translation perspectiveTranslationTarget = perspectiveTranslationSource.Clone();
                        perspectiveTarget.AmoPerspective.Translations.Add(perspectiveTranslationTarget);
                    }
                }
            }
            else
            {
                if (perspectiveSource.Id != perspectiveTarget.Id)
                {
                    // If the names are the same, but the IDs are different, need to store the ID from the target in the source perspective so that when Create/Update subsequent tables (partitions, DSVs and special dimension properties), we know to substitute the Perspective ID
                    perspectiveSource.SubstituteId = perspectiveTarget.Id;
                }

                DeletePerspective(perspectiveTarget.Id);
                CreatePerspective(perspectiveSource);
            }
        }

        #endregion

        #region Roles

        /// <summary>
        /// Delete role associated with the TabularModel object.
        /// </summary>
        /// <param name="id">The id of the role to be deleted.</param>
        public void DeleteRole(string id)
        {
            if (_amoDatabase.Roles.Contains(id))
            {
                _amoDatabase.Roles.Remove(id);
            }

            // Cube permissions
            if (_amoDatabase.Cubes.Count > 0)
            {
                List<string> cubePermissionIdsToDelete = new List<string>();

                foreach (CubePermission cubePermission in _amoDatabase.Cubes[0].CubePermissions)
                {
                    if (cubePermission.RoleID == id)
                    {
                        cubePermissionIdsToDelete.Add(cubePermission.ID);
                    }
                }

                foreach (string cubePermissionIdToDelete in cubePermissionIdsToDelete)
                {
                    _amoDatabase.Cubes[0].CubePermissions.Remove(cubePermissionIdToDelete);
                }
            }

            // Dimension permissions
            foreach (Dimension dim in _amoDatabase.Dimensions)
            {
                List<string> dimPermissionIdsToDelete = new List<string>();

                foreach (DimensionPermission dimPermission in dim.DimensionPermissions)
                {
                    if (dimPermission.RoleID == id)
                    {
                        dimPermissionIdsToDelete.Add(dimPermission.ID);
                    }
                }

                foreach (string dimPermissionIdToDelete in dimPermissionIdsToDelete)
                {
                    dim.DimensionPermissions.Remove(dimPermissionIdToDelete);
                }
            }

            // Database permissions
            List<string> dbPermissionIdsToDelete = new List<string>();

            foreach (DatabasePermission dbPermission in _amoDatabase.DatabasePermissions)
            {
                if (dbPermission.RoleID == id)
                {
                    dbPermissionIdsToDelete.Add(dbPermission.ID);
                }
            }

            foreach (string dbPermissionIdToDelete in dbPermissionIdsToDelete)
            {
                _amoDatabase.DatabasePermissions.Remove(dbPermissionIdToDelete);
            }

            // shell model
            if (_roles.ContainsId(id))
            {
                _roles.RemoveById(id);
            }
        }

        /// <summary>
        /// Create role associated with the TabularModel object.
        /// </summary>
        /// <param name="roleSource">Role object from the source tabular model to be created in the target.</param>
        public void CreateRole(Role roleSource)
        {
            Microsoft.AnalysisServices.Role amoRoleTarget = roleSource.AmoRole.Clone();

            // Need to check if there is an existing role with same ID (some clever clogs might have renamed the object in source and kept same ID).  If so, replace it with a new one and store it as substitute ID in source.
            if (_amoDatabase.Roles.Contains(roleSource.Id))
            {
                amoRoleTarget.ID = Convert.ToString(Guid.NewGuid());
                roleSource.SubstituteId = amoRoleTarget.ID;
            }

            // Database permissions
            foreach (DatabasePermission dbPermissionSource in roleSource.ParentTabularModel.AmoDatabase.DatabasePermissions)
            {
                if (dbPermissionSource.RoleID == roleSource.Id)
                {
                    DatabasePermission dbPermissionTarget = dbPermissionSource.Clone();
                    dbPermissionTarget.RoleID = amoRoleTarget.ID;
                    if (_amoDatabase.DatabasePermissions.Contains(dbPermissionTarget.ID))
                    {
                        dbPermissionTarget.ID = Convert.ToString(Guid.NewGuid());
                    }
                    if (_amoDatabase.DatabasePermissions.ContainsName(dbPermissionTarget.Name))
                    {
                        if (_amoDatabase.DatabasePermissions.ContainsName(dbPermissionTarget.ID))
                        {
                            dbPermissionTarget.Name = Convert.ToString(Guid.NewGuid());
                        }
                        else
                        {
                            dbPermissionTarget.Name = dbPermissionTarget.ID;
                        }
                    }
                    _amoDatabase.DatabasePermissions.Add(dbPermissionTarget);
                }
            }

            // Dimension permissions
            foreach (Dimension dimSource in roleSource.ParentTabularModel.AmoDatabase.Dimensions)
            {
                Dimension dimTarget = _amoDatabase.Dimensions.FindByName(dimSource.Name);
                if (dimTarget != null)
                {
                    foreach (DimensionPermission dimPermissionSource in dimSource.DimensionPermissions)
                    {
                        if (dimPermissionSource.RoleID == roleSource.Id)
                        {
                            DimensionPermission dimPermissionTarget = dimPermissionSource.Clone();
                            dimPermissionTarget.RoleID = amoRoleTarget.ID;
                            if (dimSource.DimensionPermissions.Contains(dimPermissionTarget.ID))
                            {
                                dimPermissionTarget.ID = Convert.ToString(Guid.NewGuid());
                            }
                            if (!dimTarget.DimensionPermissions.ContainsName(dimPermissionTarget.Name))
                                dimTarget.DimensionPermissions.Add(dimPermissionTarget);
                        }
                    }
                }
            }

            // Cube permissions
            if (roleSource.ParentTabularModel.AmoDatabase.Cubes.Count > 0 && _amoDatabase.Cubes.Count > 0)
            {
                foreach (CubePermission cubePermissionSource in roleSource.ParentTabularModel.AmoDatabase.Cubes[0].CubePermissions)
                {
                    if (cubePermissionSource.RoleID == roleSource.Id)
                    {
                        CubePermission cubePermissionTarget = cubePermissionSource.Clone();
                        cubePermissionTarget.RoleID = amoRoleTarget.ID;
                        if (_amoDatabase.Cubes[0].CubePermissions.Contains(cubePermissionTarget.ID))
                        {
                            cubePermissionTarget.ID = Convert.ToString(Guid.NewGuid());
                        }
                        if (_amoDatabase.Cubes[0].CubePermissions.ContainsName(cubePermissionTarget.Name))
                        {
                            if (_amoDatabase.Cubes[0].CubePermissions.ContainsName(cubePermissionTarget.ID))
                            {
                                cubePermissionTarget.Name = Convert.ToString(Guid.NewGuid());
                            }
                            else
                            {
                                cubePermissionTarget.Name = cubePermissionTarget.ID;
                            }
                        }
                        _amoDatabase.Cubes[0].CubePermissions.Add(cubePermissionTarget);
                    }
                }
            }

            _amoDatabase.Roles.Add(amoRoleTarget);

            // shell model
            _roles.Add(new Role(this, amoRoleTarget));
        }

        /// <summary>
        /// Update role associated with the TabularModel object.
        /// </summary>
        /// <param name="roleSource">Role object from the source tabular model to be updated in the target.</param>
        /// <param name="roleTarget">Role object in the target tabular model to be updated.</param>
        public void UpdateRole(Role roleSource, Role roleTarget)
        {
            if (roleSource.Id != roleTarget.Id)
            {
                // If the names are the same, but the IDs are different, need to store the ID from the target in the source role so that when Create/Update subsequent tables (partitions, DSVs and special dimension properties), we know to substitute the Role ID
                roleSource.SubstituteId = roleTarget.Id;
            }

            DeleteRole(roleTarget.Id);
            CreateRole(roleSource);
        }

        #endregion

        /// <summary>
        /// Delete calculation property associated with the TabularModel object.
        /// </summary>
        /// <param name="calculationReference">The calculation reference to be deleted.</param>
        public void DeleteCalculationProperty(string calculationReference)
        {
            if (_amoDatabase.Cubes[0].MdxScripts[0].CalculationProperties.Contains(calculationReference))
            {
                _amoDatabase.Cubes[0].MdxScripts[0].CalculationProperties.Remove(calculationReference);
            }
        }

        /// <summary>
        /// Create calculation property associated with the TabularModel object.
        /// </summary>
        /// <param name="calculationPropertySource">Calculation property object from the source tabular model to be created in the target.</param>
        /// <param name="calculationReference">Calculation reference from the source tabular model.</param>
        public void CreateCalculationProperty(CalculationProperty calculationPropertySource, string calculationReference)
        {
            if (_amoDatabase.Cubes[0].MdxScripts[0].CalculationProperties.Contains(calculationReference))
            {
                _amoDatabase.Cubes[0].MdxScripts[0].CalculationProperties.Remove(calculationReference);
            }

            if (calculationPropertySource == null)
            {
                _amoDatabase.Cubes[0].MdxScripts[0].CalculationProperties.Add(calculationReference);
            }
            else
            {
                _amoDatabase.Cubes[0].MdxScripts[0].CalculationProperties.Add(calculationPropertySource.Clone());
            }
        }

        /// <summary>
        /// Populate MDX script in target tabular model based on Measures collection.
        /// </summary>
        public void PopulateMdxScript()
        {
            if (_amoDatabase.Cubes.Count > 0 && _amoDatabase.Cubes[0].MdxScripts[0].Commands.Count > 0)
            {
                if (_amoDatabase.CompatibilityLevel >= 1103)
                {
                    // since sp1, each measure gets its own command, so need to clear out and recreate them

                    // delete all commands except the first one
                    for (int i = _amoDatabase.Cubes[0].MdxScripts[0].Commands.Count - 1; i >= 0; i--)
                    {
                        //following is a clumsy check. We don't want to mess with the command containing "CREATE MEMBER CURRENTCUBE.Measures.[__No measures defined] AS 1;"
                        if (_amoDatabase.Cubes[0].MdxScripts[0].Commands[i].Text == "" || _amoDatabase.Cubes[0].MdxScripts[0].Commands[i].Text.Contains("-- PowerPivot measures command (do not modify manually) --"))
                        {
                            _amoDatabase.Cubes[0].MdxScripts[0].Commands.RemoveAt(i);
                        }
                    }

                    foreach (Measure measure in this.Measures)
                    {
                        CreateMdxScriptCommand(measure);
                    }
                    foreach (Kpi kpi in _kpis)
                    {
                        CreateMdxScriptCommand(kpi);
                    }
                }
                #region pre SP1
                else if (_amoDatabase.Cubes[0].MdxScripts[0].Commands.Count > 1)
                {
                    //pre sp1
                    StringBuilder measuresCommand = new StringBuilder();

                    measuresCommand.AppendLine("----------------------------------------------------------");
                    measuresCommand.AppendLine("-- PowerPivot measures command (do not modify manually) --");
                    measuresCommand.AppendLine("----------------------------------------------------------");
                    measuresCommand.AppendLine("");
                    measuresCommand.AppendLine("");

                    foreach (Measure measure in this.Measures)
                    {
                        measuresCommand.AppendLine(String.Format("CREATE MEASURE '{0}'[{1}]={2}", measure.TableName, measure.Name.Replace("]", "]]"), measure.Expression));
                    }

                    foreach (Kpi kpi in _kpis)
                    {
                        measuresCommand.AppendLine(String.Format("CREATE MEASURE '{0}'[{1}]={2}", kpi.TableName, kpi.Name, kpi.Expression));
                        measuresCommand.AppendLine(String.Format("CREATE MEMBER CURRENTCUBE.Measures.[{0}] AS '{1}', ASSOCIATED_MEASURE_GROUP = '{2}';", kpi.GoalMeasure.Name, kpi.GoalMeasure.Expression, kpi.GoalMeasure.TableName));
                        measuresCommand.AppendLine(String.Format("CREATE MEMBER CURRENTCUBE.Measures.[{0}] AS '{1}', ASSOCIATED_MEASURE_GROUP = '{2}';", kpi.StatusMeasure.Name, kpi.StatusMeasure.Expression, kpi.StatusMeasure.TableName));
                        measuresCommand.AppendLine(String.Format("CREATE MEMBER CURRENTCUBE.Measures.[{0}] AS '{1}', ASSOCIATED_MEASURE_GROUP = '{2}';", kpi.TrendMeasure.Name, kpi.TrendMeasure.Expression, kpi.TrendMeasure.TableName));
                        //use MDX script method for KPIs, not object model (as does latest version of Tabular Editor):
                        measuresCommand.AppendLine(String.Format("CREATE KPI CURRENTCUBE.[{0}] AS Measures.[{0}], ASSOCIATED_MEASURE_GROUP = '{1}', GOAL = Measures.[{2}], STATUS = Measures.[{3}], TREND = Measures.[{4}], STATUS_GRAPHIC = '{5}', TREND_GRAPHIC = '{6}';",
                            kpi.Name, kpi.TableName, kpi.GoalMeasure.Name, kpi.StatusMeasure.Name, kpi.TrendMeasure.Name, kpi.StatusGraphic, kpi.TrendGraphic));

                        //It is possible (if there were existing, unchanged KPIs in target that use AMO, rather than script declaration) that this KPI is missing its calculation property
                        if (!_amoDatabase.Cubes[0].MdxScripts[0].CalculationProperties.Contains(kpi.KpiCalculationReference))
                        {
                            CreateCalculationProperty(kpi.AmoKpiCalculationProperty, kpi.KpiCalculationReference);
                        }
                    }

                    _amoDatabase.Cubes[0].MdxScripts[0].Commands[1].Text = measuresCommand.ToString();
                }
                #endregion

                //Clear AMO KPIs collection (we are using MDX script declared KPIs, not AMO) just in case this cube happened to have them (don't want both versions).
                _amoDatabase.Cubes[0].Kpis.Clear();
            }
        }

        private void CreateMdxScriptCommand(Measure measure)
        {
            StringBuilder measuresCommand = new StringBuilder();

            measuresCommand.AppendLine("----------------------------------------------------------");
            measuresCommand.AppendLine("-- PowerPivot measures command (do not modify manually) --");
            measuresCommand.AppendLine("----------------------------------------------------------");
            measuresCommand.AppendLine("");
            measuresCommand.AppendLine("");

            if (measure is Kpi)
            {
                Kpi kpi = (Kpi)measure;
                measuresCommand.AppendLine(String.Format("CREATE MEASURE '{0}'[{1}]={2}", kpi.TableName, kpi.Name, kpi.Expression));

                measuresCommand.AppendLine(String.Format("CREATE MEASURE '{0}'[{1}]={2}", kpi.TableName, kpi.GoalMeasure.Name, kpi.GoalMeasure.Expression));
                measuresCommand.AppendLine(String.Format("CREATE MEASURE '{0}'[{1}]={2}", kpi.TableName, kpi.StatusMeasure.Name, kpi.StatusMeasure.Expression));
                //measuresCommand.AppendLine(String.Format("CREATE MEASURE '{0}'[{1}]={2}", kpi.TableName, kpi.TrendMeasure.Name, kpi.TrendMeasure.Expression));
                
                //use MDX script method for KPIs, not object model (as does the Tabular Editor since RC0):
                measuresCommand.AppendLine(String.Format("CREATE KPI CURRENTCUBE.[{0}] AS Measures.[{0}], ASSOCIATED_MEASURE_GROUP = '{1}', GOAL = Measures.[{2}], STATUS = Measures.[{3}], STATUS_GRAPHIC = '{4}';",
                    kpi.Name, kpi.TableName, kpi.GoalMeasure.Name, kpi.StatusMeasure.Name, kpi.StatusGraphic));

                //It is possible (if there were existing, unchanged KPIs in target that use AMO, rather than script declaration) that this KPI is missing its calculation property
                if (!_amoDatabase.Cubes[0].MdxScripts[0].CalculationProperties.Contains(kpi.KpiCalculationReference))
                {
                    CreateCalculationProperty(kpi.AmoKpiCalculationProperty, kpi.KpiCalculationReference);
                }
            }
            else
            { 
                //regular measure
                measuresCommand.AppendLine(String.Format("CREATE MEASURE '{0}'[{1}]={2}", measure.TableName, measure.Name, measure.Expression));
            }

            //add on sp1 annotations fluff
            Microsoft.AnalysisServices.Command cmdToAdd = new Microsoft.AnalysisServices.Command(measuresCommand.ToString());
            cmdToAdd.Annotations.Add("FullName", measure.Name.Replace("]]", "]"));
            cmdToAdd.Annotations.Add("Table", measure.TableName);

            _amoDatabase.Cubes[0].MdxScripts[0].Commands.Add(cmdToAdd);
        }

        /// <summary>
        /// Final checks and cleanup to ensure referential integrity between objects.
        /// </summary>
        public void FinalCleanup()
        {
            //check for database permissions to non-existing roles.  Cannot do this when creating/updating dimensions because roles not yet created/deleted.
            foreach (Dimension dimension in _amoDatabase.Dimensions)
            {
                List<string> dimensionPermissionIdsToDelete = new List<string>();

                foreach (DimensionPermission dimensionPermission in dimension.DimensionPermissions)
                {
                    if (!_amoDatabase.Roles.Contains(dimensionPermission.RoleID))
                    {
                        dimensionPermissionIdsToDelete.Add(dimensionPermission.ID);
                    }
                }

                foreach (string dimensionPermissionIdToDelete in dimensionPermissionIdsToDelete)
                {
                    dimension.DimensionPermissions.Remove(dimensionPermissionIdToDelete);
                }
            }

            //check for redundant cube
            if (_amoDatabase.Dimensions.Count == 0)
            {
                _amoDatabase.Cubes.Clear();
                _amoDatabase.DataSourceViews.Clear();
            }
        }

        #endregion

        /// <summary>
        /// Update target tabular model with changes resulting from the comparison. For database deployment, this will fire DeployDatabaseCallBack.
        /// </summary>
        /// <returns>Boolean indicating whether update was successful.</returns>
        public bool Update()
        {
            if (_connectionInfo.UseProject)
            {
                UpdateProject();
            }
            else   //Database deployement
            {
                if (_comparisonInfo.PromptForDatabaseProcessing)
                {
                    //Call back to show deployment form
                    DatabaseDeploymentEventArgs args = new DatabaseDeploymentEventArgs();
                    _parentComparison.OnDatabaseDeployment(args);
                    return args.DeploymentSuccessful;
                }
                else
                {
                    //Simple update target without setting passwords or processing
                    _amoDatabase.Update(UpdateOptions.ExpandFull);
                }
            }

            return true;
        }

        private void UpdateProject()
        {
            _amoDatabase.Update(UpdateOptions.ExpandFull);

            if (_connectionInfo.Project != null)
            {
                EnvDTE._DTE dte = _connectionInfo.Project.DTE;

                //check out bim file if necessary
                if (dte.SourceControl.IsItemUnderSCC(_connectionInfo.BimFileFullName) && !dte.SourceControl.IsItemCheckedOut(_connectionInfo.BimFileFullName))
                {
                    dte.SourceControl.CheckOutItem(_connectionInfo.BimFileFullName);
                }
            }

            //Script out db and write to project file
            string xml = ScriptDatabase(toOverwriteProjectBimFile: true);

            //replace db name with "SemanticModel"
            XmlDocument bimFileDoc = new XmlDocument();
            bimFileDoc.LoadXml(xml);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(bimFileDoc.NameTable);
            nsmgr.AddNamespace("myns1", "http://schemas.microsoft.com/analysisservices/2003/engine");
            XmlNode objectDefinitionDatabaseNameNode = bimFileDoc.SelectSingleNode("//myns1:ObjectDefinition/myns1:Database/myns1:Name", nsmgr);
            objectDefinitionDatabaseNameNode.InnerText = "SemanticModel";

            xml = WriteXmlFromDoc(bimFileDoc);
            File.WriteAllText(_connectionInfo.BimFileFullName, xml);
        }

        #region Database deployment and processing methods

        private const string _deployRowWorkItem = "Deploy metadata";
        private ProcessingTableCollection _tablesToProcess;

        /// <summary>
        /// Perform database deployment and processing of required tables.
        /// </summary>
        /// <param name="tablesToProcess">Tables to process.</param>
        public void DatabaseDeployAndProcess(ProcessingTableCollection tablesToProcess)
        {
            try
            {
                _tablesToProcess = tablesToProcess;

                //Set passwords ready for processing
                foreach (Microsoft.AnalysisServices.DataSource dataSource in _amoDatabase.DataSources)
                {
                    if (dataSource.ImpersonationInfo != null && dataSource.ImpersonationInfo.ImpersonationMode == ImpersonationMode.ImpersonateAccount)
                    {
                        PasswordPromptEventArgs args = new PasswordPromptEventArgs();
                        args.DataSourceName = dataSource.Name;
                        args.Username = dataSource.ImpersonationInfo.Account;
                        _parentComparison.OnPasswordPrompt(args);
                        if (args.UserCancelled)
                        {
                            // Show cancelled for all rows
                            _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(_deployRowWorkItem, "Deployment has been cancelled.", DeploymentStatus.Cancel));
                            foreach (ProcessingTable table in _tablesToProcess)
                            {
                                _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(table.Name, "Cancelled", DeploymentStatus.Cancel));
                            }
                            _parentComparison.OnDeploymentComplete(new DeploymentCompleteEventArgs(DeploymentStatus.Cancel, null));
                            return;
                        }
                        dataSource.ImpersonationInfo.Account = args.Username;
                        dataSource.ImpersonationInfo.Password = args.Password;
                    }
                }

                if (_comparisonInfo.OptionsInfo.OptionTransaction)
                {
                    try
                    {
                        _amoServer.RollbackTransaction();
                    }
                    catch { }
                    _amoServer.BeginTransaction();
                }

                _amoDatabase.Update(UpdateOptions.ExpandFull);

                if (!_comparisonInfo.OptionsInfo.OptionTransaction)
                {
                    _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(_deployRowWorkItem, "Success. Metadata deployed.", DeploymentStatus.Success));
                }

                if (_tablesToProcess.Count > 0)
                {
                    ProcessAsyncDelegate processAsyncCaller = new ProcessAsyncDelegate(Process);
                    processAsyncCaller.BeginInvoke(null, null);
                }
                else
                {
                    if (_comparisonInfo.OptionsInfo.OptionTransaction)
                    {
                        _amoServer.CommitTransaction();
                        _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(_deployRowWorkItem, "Success. Metadata deployed.", DeploymentStatus.Success));
                    }
                    _parentComparison.OnDeploymentComplete(new DeploymentCompleteEventArgs(DeploymentStatus.Success, null));
                }
            }
            catch (Exception exc)
            {
                _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(_deployRowWorkItem, "Error deploying metadata.", DeploymentStatus.Error));
                foreach (ProcessingTable table in _tablesToProcess)
                {
                    _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(table.Name, "Error", DeploymentStatus.Error));
                }
                _parentComparison.OnDeploymentComplete(new DeploymentCompleteEventArgs(DeploymentStatus.Error, exc.Message));
            }
        }

        private bool _stopProcessing;
        string _sessionId;
        internal delegate void ProcessAsyncDelegate();
        private void Process()
        {
            //string x13 = TraceColumn.ObjectName.ToString();
            //string x15 = TraceColumn.ObjectReference.ToString();
            //string x10 = TraceColumn.IntegerData.ToString();

            try
            {
                _stopProcessing = false;
                ProcessType processType = _comparisonInfo.OptionsInfo.OptionProcessingOption == ProcessingOption.Default ? ProcessType.ProcessDefault : ProcessType.ProcessFull;

                //Set up server trace to capture how many rows processed
                _sessionId = _amoServer.SessionID;
                Trace trace = _amoServer.Traces.Add();
                TraceEvent traceEvent = trace.Events.Add(TraceEventClass.ProgressReportCurrent);
                traceEvent.Columns.Add(TraceColumn.ObjectID);
                traceEvent.Columns.Add(TraceColumn.ObjectName);
                traceEvent.Columns.Add(TraceColumn.ObjectReference);
                traceEvent.Columns.Add(TraceColumn.IntegerData);
                traceEvent.Columns.Add(TraceColumn.SessionID);
                traceEvent.Columns.Add(TraceColumn.Spid);
                trace.Update();
                trace.OnEvent += new TraceEventHandler(Trace_OnEvent);
                trace.Start();

                _amoServer.CaptureXml = true;

                if (_comparisonInfo.OptionsInfo.OptionAffectedTables)
                {
                    foreach (ProcessingTable tableToProcess in _tablesToProcess)
                    {
                        Table table = this.Tables.FindByName(tableToProcess.Name);
                        if (table != null && table.AmoDimension.CanProcess(processType))
                        {
                            table.AmoDimension.Process(processType);
                        }
                    }
                }
                else
                {
                    _amoDatabase.Process(processType);
                }

                _amoServer.CaptureXml = false;

                XmlaResultCollection results = _amoServer.ExecuteCaptureLog(true, true);

                try
                {
                    trace.Stop();
                    trace.Drop();
                }
                catch { }

                string errorMessage = "";
                foreach (XmlaResult result in results)
                {
                    foreach (XmlaMessage message in result.Messages)
                    {
                        if (message is XmlaError)
                        {
                            errorMessage += message.Description + System.Environment.NewLine;
                        }
                    }
                }

                if (errorMessage != "")
                {
                    ShowErrorsForAllRows();
                    _parentComparison.OnDeploymentComplete(new DeploymentCompleteEventArgs(DeploymentStatus.Error, errorMessage));
                }
                else
                {
                    if (_comparisonInfo.OptionsInfo.OptionTransaction)
                    {
                        if (_stopProcessing)
                        {
                            //already dealt with rolling back tran and error messages
                            return;
                        }
                        else
                        {
                            _amoServer.CommitTransaction();
                            _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(_deployRowWorkItem, "Success. Metadata deployed.", DeploymentStatus.Success));
                        }
                    }

                    // Show row count for each table
                    foreach (ProcessingTable table in _tablesToProcess)
                    {
                        int rowCount = _connectionInfo.DirectQuery ? 0 : Core.Comparison.FindRowCount(_amoServer, table.Name, _amoDatabase.Name);
                        _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(table.Name, "Success. " + String.Format("{0:#,###0}", rowCount) + " rows transferred.", DeploymentStatus.Success));
                    }
                    _parentComparison.OnDeploymentComplete(new DeploymentCompleteEventArgs(DeploymentStatus.Success, null));
                }
            }
            catch (Exception exc)
            {
                ShowErrorsForAllRows();
                _parentComparison.OnDeploymentComplete(new DeploymentCompleteEventArgs(DeploymentStatus.Error, exc.Message));
            }
        }

        /// <summary>
        /// Stop processing if possible.
        /// </summary>
        public void StopProcessing()
        {
            _stopProcessing = true;

            if (_comparisonInfo.OptionsInfo.OptionTransaction)
            {
                try
                {
                    _amoServer.RollbackTransaction();
                    ShowErrorsForAllRows();
                    _parentComparison.OnDeploymentComplete(new DeploymentCompleteEventArgs(DeploymentStatus.Error, "Rolled back transaction."));
                }
                catch (Exception exc)
                {
                    if (exc is NullReferenceException || exc is InvalidOperationException)
                    {
                        return;
                    }
                    else
                    {
                        ShowErrorsForAllRows();
                        _parentComparison.OnDeploymentComplete(new DeploymentCompleteEventArgs(DeploymentStatus.Error, exc.Message));
                    }
                }
            }
        }

        private void Trace_OnEvent(object sender, TraceEventArgs e)
        {
            if (e.ObjectName != null && e.ObjectReference != null && e.SessionID == _sessionId)
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(e.ObjectReference);

                XmlNodeList partitionIdNodeList = document.GetElementsByTagName("PartitionID");
                XmlNodeList measureGroupIdNodeList = document.GetElementsByTagName("MeasureGroupID");

                if (partitionIdNodeList != null && measureGroupIdNodeList != null)
                {
                    if (_tablesToProcess.ContainsId(measureGroupIdNodeList[0].InnerText))
                    {
                        ProcessingTable table = _tablesToProcess.FindById(measureGroupIdNodeList[0].InnerText);

                        if (!table.ContainsPartition(partitionIdNodeList[0].InnerText))
                        {
                            table.Partitions.Add(new PartitionRowCounter(partitionIdNodeList[0].InnerText));
                        }

                        PartitionRowCounter partition = table.FindPartition(partitionIdNodeList[0].InnerText);
                        partition.RowCount = e.IntegerData;

                        _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(table.Name, "Retreived " + String.Format("{0:#,###0}", table.GetRowCount()) + " rows ...", DeploymentStatus.Deploying));
                    }
                }

                if (_stopProcessing && !_comparisonInfo.OptionsInfo.OptionTransaction) //transactions get cancelled in StopProcessing, not here
                {
                    try
                    {
                        _amoServer.CancelCommand(_sessionId);
                    }
                    catch { }
                }

                ////Doesn't work with Spid, so doing sessionid above
                //int spid;
                //if (_stopProcessing && int.TryParse(e.Spid, out spid))
                //{
                //    try
                //    {
                //        //_amoServer.CancelCommand(e.Spid);
                //        string commandStatement = String.Format("<Cancel xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\"><SPID>{0}</SPID><CancelAssociated>1</CancelAssociated></ Cancel>", e.Spid);
                //        System.Diagnostics.Debug.WriteLine(commandStatement);
                //        _amoServer.Execute(commandStatement);
                //        //_connectionInfo.ExecuteXmlaCommand(_amoServer, commandStatement);
                //    }
                //    catch { }
                //}
            }
        }

        private void ShowErrorsForAllRows()
        {
            // Show error for each item
            if (_comparisonInfo.OptionsInfo.OptionTransaction)
            {
                _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(_deployRowWorkItem, "Error", DeploymentStatus.Error));
            }
            foreach (ProcessingTable table in _tablesToProcess)
            {
                _parentComparison.OnDeploymentMessage(new DeploymentMessageEventArgs(table.Name, "Error", DeploymentStatus.Error));
            }
        }

        #endregion

        /// <summary>
        /// Generate script containing full tabular model definition.
        /// </summary>
        /// <param name="toOverwriteProjectBimFile">Flag indicating whether the operation is to overwrite a project BIM file. If not, may need name substitution.</param>
        /// <returns>XMLA script of tabular model defintion.</returns>
        public string ScriptDatabase(bool toOverwriteProjectBimFile = false)
        {
            string xml = WriteXmlFromDatabase();

            if (_connectionInfo.UseProject)
            {
                //replace db/cube name/id with name of deploymnet db from the project file
                XmlDocument xmlaScriptDoc = new XmlDocument();
                xmlaScriptDoc.LoadXml(xml);
                XmlNamespaceManager nsmgr2 = new XmlNamespaceManager(xmlaScriptDoc.NameTable);
                nsmgr2.AddNamespace("myns1", "http://schemas.microsoft.com/analysisservices/2003/engine");
                XmlNode objectDefinitionDatabaseIdNode = xmlaScriptDoc.SelectSingleNode("//myns1:Object/myns1:DatabaseID", nsmgr2);
                XmlNode objectDefinitionDatabaseId2Node = xmlaScriptDoc.SelectSingleNode("//myns1:ObjectDefinition/myns1:Database/myns1:ID", nsmgr2);
                XmlNode objectDefinitionDatabaseNameNode = xmlaScriptDoc.SelectSingleNode("//myns1:ObjectDefinition/myns1:Database/myns1:Name", nsmgr2);
                XmlNode objectDefinitionCubeIdNode = xmlaScriptDoc.SelectSingleNode("//myns1:ObjectDefinition/myns1:Database/myns1:Cubes/myns1:Cube/myns1:ID", nsmgr2);
                XmlNode objectDefinitionCubeNameNode = xmlaScriptDoc.SelectSingleNode("//myns1:ObjectDefinition/myns1:Database/myns1:Cubes/myns1:Cube/myns1:Name", nsmgr2);

                if (toOverwriteProjectBimFile)
                {
                    objectDefinitionDatabaseIdNode.InnerText = "SemanticModel";
                    objectDefinitionDatabaseId2Node.InnerText = "SemanticModel";
                    objectDefinitionDatabaseNameNode.InnerText = "SemanticModel";
                    if (objectDefinitionCubeIdNode != null) objectDefinitionCubeIdNode.InnerText = "Model";
                    if (objectDefinitionCubeNameNode != null) objectDefinitionCubeNameNode.InnerText = "Model";
                }
                else
                {
                    if (!String.IsNullOrEmpty(_connectionInfo.DeploymentServerDatabase))
                    {
                        objectDefinitionDatabaseIdNode.InnerText = _connectionInfo.DeploymentServerDatabase;
                        objectDefinitionDatabaseId2Node.InnerText = _connectionInfo.DeploymentServerDatabase;
                        objectDefinitionDatabaseNameNode.InnerText = _connectionInfo.DeploymentServerDatabase;
                    }
                    if (!String.IsNullOrEmpty(_connectionInfo.DeploymentServerCubeName))
                    {
                        if (objectDefinitionCubeIdNode != null) objectDefinitionCubeIdNode.InnerText = _connectionInfo.DeploymentServerCubeName;
                        if (objectDefinitionCubeNameNode != null) objectDefinitionCubeNameNode.InnerText = _connectionInfo.DeploymentServerCubeName;
                    }
                }
                xml = WriteXmlFromDoc(xmlaScriptDoc);
            }

            return xml;
        }

        private XmlWriterSettings _settings = new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            Indent = true,
            //IndentChars = "  ",
            NewLineChars = "\r\n",
            NewLineHandling = NewLineHandling.Replace
        };

        private string WriteXmlFromDoc(XmlDocument bimFileDoc)
        {
            string xml;
            StringBuilder builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder, _settings))
            {
                bimFileDoc.Save(writer);
            }
            xml = builder.ToString();
            return xml;
        }

        private string WriteXmlFromDatabase()
        {
            string xml;
            StringBuilder builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder, _settings))
            {
                Scripter.WriteAlter(writer, _amoDatabase, true, true);
            }
            xml = builder.ToString();
            return xml;
        }

        public override string ToString() => this.GetType().FullName;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_amoServer != null)
                    {
                        _amoServer.Dispose();
                    }
                }

                _disposed = true;
            }
        }

    }
}
