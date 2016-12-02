using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Timers;
using Microsoft.AnalysisServices;
using Tom = Microsoft.AnalysisServices.Tabular;
using System.Xml;
using System.Drawing;

namespace ASPerfMon
{
    public partial class ASPerfMon : Form
    {
        public ASPerfMon()
        {
            InitializeComponent();
        }

        private Tom.Server _server = new Tom.Server();
        private System.Timers.Timer _timer;
        private const int _smallestFont = 12;
        private string _serverConnectionString;
        private DateTime _connectionTime;

        //--------------------------------------------------
        //120 X axis markers, one for each array item.
        //Each item holds a dictionary with lookup key of series name (e.g. database name), which returns a SeriesPoint object.
        Dictionary<string, SeriesPoint>[] _seriesData = new Dictionary<string, SeriesPoint>[120];

        class SeriesPoint
        {
            public string XAxisLabel;
            public long MemoryUsedBytes;
            public long MemoryUsedMegabytes
            {
                get
                {
                    return MemoryUsedBytes / 1024 / 1024;
                }
            }

            public SeriesPoint(string xAxisCategoryName)
            {
                this.XAxisLabel = xAxisCategoryName;
                this.MemoryUsedBytes = 0;
            }
        }

        private void ASPerfMon_Load(object sender, EventArgs e)
        {
            InitializeChart(false);
        }

        private void InitializeChart(bool withSampleSeries)
        {
            chartControl.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chartControl.ChartAreas[0].AxisX.Interval = 5;
            chartControl.ChartAreas[0].AxisY.IsMarksNextToAxis = true;
            chartControl.ChartAreas[0].AxisY.LabelStyle.Format = "{0:n0}";
            chartControl.ChartAreas[0].AxisY.Title = "Memory Usage MB";
            chartControl.Palette = ChartColorPalette.Pastel;
            chartControl.ChartAreas[0].AxisY.TitleFont = new Font(Font.FontFamily, _smallestFont + 2);
            chartControl.Legends[0].Font = new Font(Font.FontFamily, _smallestFont);
            chartControl.Legends[0].LegendItemOrder = LegendItemOrder.ReversedSeriesOrder;

            //initialize the chart with blanks
            Dictionary<string, SeriesPoint> pointsSample;
            if (withSampleSeries)
                pointsSample = GetNewXAxisMarkerPointsFromAs();
            else
            {
                pointsSample = new Dictionary<string, SeriesPoint>();
            }

            chartControl.Series.Clear();
            if (withSampleSeries)
            {
                for (int i = _seriesData.Length - 1; i >= 0; i--)
                {
                    foreach (KeyValuePair<string, SeriesPoint> entry in pointsSample)//to get series
                    {
                        entry.Value.XAxisLabel = " ";
                        entry.Value.MemoryUsedBytes = 0;

                        //add the series if not there yet
                        if (chartControl.Series.FindByName(entry.Key) == null)
                        {
                            Series series = new Series(entry.Key);
                            chartControl.Series.Add(series);
                        }
                        chartControl.Series[entry.Key].Points.AddXY(" ", 0);
                    }
                    _seriesData[i] = pointsSample;
                }
                PaintChart();
            }
            else
            {
                chartControl.Series.Add("");
                for (int i = 0; i < _seriesData.Length; i++)
                {
                    chartControl.Series[0].Points.AddXY(" ", 0);
                }
                chartControl.Invalidate();
            }

            _timer = new System.Timers.Timer(Settings.Default.SampleInterval);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
        }

        private void PaintChart()
        {
            //set dynamic scale
            long maxValue = 0;
            for (int i = _seriesData.Length - 1; i >= 0; i--)
            {
                long pointsStack = 0;
                foreach (SeriesPoint point in _seriesData[i].Values)
                {
                    pointsStack += Math.Abs(point.MemoryUsedMegabytes);
                }

                if (pointsStack > maxValue)
                    maxValue = pointsStack;
            }
            chartControl.ChartAreas[0].AxisY.IsStartedFromZero = true;
            chartControl.ChartAreas[0].AxisY.Minimum = double.NaN;
            chartControl.ChartAreas[0].AxisY.Maximum = double.NaN;
            chartControl.ChartAreas[0].RecalculateAxesScale();
            chartControl.Invalidate();
        }

        delegate void SetTimerCallback(Object source, ElapsedEventArgs e);
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            try
            {
                if (chartControl.InvokeRequired)
                {
                    SetTimerCallback callback = new SetTimerCallback(OnTimedEvent);
                    //todo safer invoke look up bism
                    try
                    {
                        Invoke(callback, new object[] { source, e });
                    }
                    catch { }
                }
                else
                {
                    //Todo delete this once http stability fix is done
                    if ((DateTime.Now - _connectionTime).TotalMinutes > 8)
                    {
                        _server.Disconnect();
                        _server.Connect(_serverConnectionString);
                        _connectionTime = DateTime.Now;
                    }

                    _seriesData[0] = GetNewXAxisMarkerPointsFromAs();

                    //rebind
                    chartControl.Series.Clear();

                    //for each X axis marker
                    for (int i = _seriesData.Length - 1; i > 0; i--)
                    {
                        //push back one category
                        _seriesData[i] = _seriesData[i - 1];

                        //foreach series for the X axis marker we are populating: ...
                        foreach (KeyValuePair<string, SeriesPoint> entry in _seriesData[i])
                        {
                            //add the series if not there yet
                            if (chartControl.Series.FindByName(entry.Key) == null)
                            {
                                Series series = new Series(entry.Key);
                                series.ChartType = SeriesChartType.StackedColumn;
                                chartControl.Series.Add(series);
                            }

                            //add the data point for the series
                            DataPoint point = new DataPoint();
                            point.SetValueXY(entry.Value.XAxisLabel, entry.Value.MemoryUsedMegabytes);
                            point.ToolTip = string.Format($"{entry.Value.XAxisLabel} - {entry.Key}: {string.Format("{0:#,###0}", entry.Value.MemoryUsedMegabytes)} MB");
                            chartControl.Series[entry.Key].Points.Add(point);
                        }
                    }
                    PaintChart();
                }
            }
            catch (Exception exc)
            {
                //Workaround for timeout over HTTP. Try to reconnect - todo delete
                try
                {
                    System.Diagnostics.Debug.WriteLine($"Exception occurred at {DateTime.Now}: {exc.Message}");
                    _server.Disconnect();
                    _server.Connect(_serverConnectionString);
                    _connectionTime = DateTime.Now;
                }
                catch
                {
                    _timer.Enabled = false;
                    MessageBox.Show($"Error: {exc.Message}", "AS PerfMon", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private Dictionary<string, SeriesPoint> GetNewXAxisMarkerPointsFromAs()
        {
            Dictionary<string, SeriesPoint> points = new Dictionary<string, SeriesPoint>();
            string xAxisLabel = DateTime.Now.ToString("hh:mm:ss tt");
            string commandStatement = String.Format("SELECT [OBJECT_PARENT_PATH], [OBJECT_MEMORY_NONSHRINKABLE] FROM $SYSTEM.DISCOVER_OBJECT_MEMORY_USAGE");
            XmlNodeList rows = ExecuteXmlaCommand(_server, commandStatement);

            foreach (XmlNode row in rows)
            {
                long result, label;
                if (long.TryParse(row.LastChild.InnerText, out result) && result > 0)
                {
                    string objectName = "";
                    string[] pathMembers = row.ChildNodes[0].InnerText.Split('.');

                    if (pathMembers.Length > 2 && _server.Databases.ContainsName(pathMembers[2]))
                    {
                        //Database name identified
                        objectName = pathMembers[2];
                    }
                    else
                    {
                        string firstMember = row.ChildNodes[0].InnerText;

                        //Ignore if number or blank. Assuming this is a double counted, shared allocation.
                        if (!long.TryParse(firstMember, out label) && firstMember != "")
                        {
                            objectName = firstMember;
                        }
                    }

                    if (objectName != "" && objectName != "Global") //todo: delete global condition
                    {
                        if (!points.ContainsKey(objectName))
                            points.Add(objectName, new SeriesPoint(xAxisLabel));

                        points[objectName].MemoryUsedBytes += Math.Abs(result);
                    }
                }
                else if (result > 0)
                {
                    System.Diagnostics.Debug.WriteLine("We have a problem parsing");
                }
            }
            return points;
        }

        public static XmlNodeList ExecuteXmlaCommand(Microsoft.AnalysisServices.Core.Server amoServer, string commandStatement)
        {
            XmlWriter xmlWriter = amoServer.StartXmlaRequest(XmlaRequestType.Undefined);
            WriteSoapEnvelopeWithCommandStatement(xmlWriter, amoServer.SessionID, commandStatement);
            System.Xml.XmlReader xmlReader = amoServer.EndXmlaRequest();
            xmlReader.MoveToContent();
            string fullEnvelopeResponseFromServer = xmlReader.ReadOuterXml();
            xmlReader.Close();

            XmlDocument documentResponse = new XmlDocument();
            documentResponse.LoadXml(fullEnvelopeResponseFromServer);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(documentResponse.NameTable);
            nsmgr.AddNamespace("myns1", "urn:schemas-microsoft-com:xml-analysis");
            nsmgr.AddNamespace("myns2", "urn:schemas-microsoft-com:xml-analysis:rowset");
            XmlNodeList rows = documentResponse.SelectNodes("//myns1:ExecuteResponse/myns1:return/myns2:root/myns2:row", nsmgr);
            return rows;
        }

        public static void WriteSoapEnvelopeWithCommandStatement(XmlWriter xmlWriter, string sessionId, string commandStatement)
        {
            //--------------------------------------------------------------------------------
            // This is a sample of the XMLA request we'll write:
            //
            // <Envelope xmlns="http://schemas.xmlsoap.org/soap/envelope/">
            //   <Header>
            //     <Session soap:mustUnderstand="1" SessionId="THE SESSION ID HERE" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns="urn:schemas-microsoft-com:xml-analysis" />
            //   </Header>
            //   <Body>
            //      <Execute xmlns="urn:schemas-microsoft-com:xml-analysis">
            //          <Command>
            //              <Statement>
            //                  ...
            //              </Statement>
            //          </Command>
            //          <Properties/>
            //      </Execute>
            //   </Body>
            // </Envelope>
            //--------------------------------------------------------------------------------
            xmlWriter.WriteStartElement("Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
            xmlWriter.WriteStartElement("Header");
            if (sessionId != null)
            {
                xmlWriter.WriteStartElement("Session", "urn:schemas-microsoft-com:xml-analysis");
                xmlWriter.WriteAttributeString("soap", "mustUnderstand", "http://schemas.xmlsoap.org/soap/envelope/", "1");
                xmlWriter.WriteAttributeString("SessionId", sessionId);
                xmlWriter.WriteEndElement(); // </Session>
            }
            xmlWriter.WriteEndElement(); // </Header>
            xmlWriter.WriteStartElement("Body");
            xmlWriter.WriteStartElement("Execute", "urn:schemas-microsoft-com:xml-analysis");
            xmlWriter.WriteStartElement("Command");
            xmlWriter.WriteElementString("Statement", commandStatement);
            xmlWriter.WriteEndElement(); // </Command>
            xmlWriter.WriteStartElement("Properties");
            xmlWriter.WriteEndElement(); // </Properties>
            xmlWriter.WriteEndElement(); // </Execute>
            xmlWriter.WriteEndElement(); // </Body>
            xmlWriter.WriteEndElement(); // </Envelope>
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            Connect();
        }

        private void Connect()
        {
            try
            {
                Connect connForm = new Connect();
                connForm.StartPosition = FormStartPosition.CenterParent;
                connForm.ShowDialog();

                if (connForm.DialogResult == DialogResult.OK)
                {
                    _timer.Enabled = false;

                    if (connForm.IntegratedAuth)
                        _serverConnectionString = $"Provider=MSOLAP;Data Source={connForm.ServerName};";
                    else
                        _serverConnectionString = $"Provider=MSOLAP;Data Source={connForm.ServerName};User ID={connForm.UserName};Password={connForm.Passwrod};Persist Security Info=True;Impersonation Level=Impersonate;";

                    _server = new Tom.Server();
                    _server.Connect(_serverConnectionString);
                    _connectionTime = DateTime.Now;
                    if (_server.ServerProperties.Count == 0)
                    {
                        throw new ConnectionException("User is not AS admin.");
                    }

                    chartControl.Titles.Clear();
                    chartControl.Titles.Add(connForm.ServerName);
                    chartControl.Titles[0].Font = new Font(Font.FontFamily, _smallestFont + 4);

                    InitializeChart(true);
                    _timer.Interval = connForm.SampleInterval;
                    _timer.Enabled = true;

                    Settings.Default.ServerName = connForm.ServerName;
                    Settings.Default.UserName = connForm.UserName;
                    Settings.Default.SampleInterval = connForm.SampleInterval;
                    Settings.Default.IntegratedAuth = connForm.IntegratedAuth;
                    Settings.Default.Save();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Cannot connect.\n{exc.Message}", "AS PerfMon", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ASPerfMon_FormClosing(object sender, FormClosingEventArgs e)
        {
            Disconnect();
        }

        private void Disconnect()
        {
            try
            {
                if (_server != null) _server.Disconnect();
            }
            catch { }
        }

        private void chartControl_GetToolTipText(object sender, ToolTipEventArgs e)
        {
            //Event handler has to be hooked up in order to display tooltip (!)
        }

        private void ASPerfMon_Shown(object sender, EventArgs e)
        {
            Connect();
        }
    }
}
