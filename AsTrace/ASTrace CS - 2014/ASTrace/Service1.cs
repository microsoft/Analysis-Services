/*============================================================================
  File:    Service1.cs

  Summary: Contains class implementiong ASTrace service

           Part of ASTrace 

  Date:    January 2007
------------------------------------------------------------------------------
  This file is part of the Microsoft SQL Server Code Samples.

  Copyright (C) Microsoft Corporation.  All rights reserved.

  This source code is intended only as a supplement to Microsoft
  Development Tools and/or on-line documentation.  See these other
  materials for detailed information regarding Microsoft code samples.

  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
============================================================================*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.IO;
using Microsoft.SqlServer.Management.Trace;
using Microsoft.SqlServer.Management.Common;

namespace ASTrace
{
    public partial class Trace : ServiceBase
    {
        List<Thread> workers = new List<Thread>();
        string localPath;
        List<string> _ServernamesList;
        int _NumInstance;
        TextWriter writer;
        List<TraceServer> traceServers = new List<TraceServer>();
        
        public Trace()
        {
            //Have seen errors here, normally when configuring for first time
            //Note, the eventlog will need to create a source the very first time it runs and that needs admin privs.
            try
            {
                InitializeComponent();
                // Read registry to find out where the service executable is installed 
                Microsoft.Win32.RegistryKey software, microsoft, astrace;
                software = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE");
                microsoft = software.OpenSubKey("Microsoft");
                astrace = microsoft.OpenSubKey("ASTrace");
                localPath = (string)astrace.GetValue("path");
                writer = new StreamWriter(localPath + "\\ASTraceService.log", true);
                WriteLog(DateTime.Now.ToString() + ":  Service Started in '" + localPath + "'");
            }
            catch (Exception ex)
            {
                StringBuilder messageText = new StringBuilder();

                messageText.Append("Failed to write to log file ").AppendLine();
                messageText.Append("Error: " + ex.Message).AppendLine();

                while (ex.InnerException != null)
                {
                    messageText.Append("INNER EXCEPTION: ");
                    messageText.Append(ex.InnerException.Message).AppendLine();
                    messageText.Append(ex.InnerException.StackTrace).AppendLine();

                    ex = ex.InnerException;
                }

                EventLog.WriteEntry(this.ServiceName, messageText.ToString(), EventLogEntryType.Error);
            }
        }

        protected override void OnStart(string[] args)
        {

            if (Properties.Settings.Default.AppendDateToSQLTable && Properties.Settings.Default.PreserveHistory)
            {
                StringBuilder messageText = new StringBuilder();
                messageText.Append(DateTime.Now.ToString() + ":  Both AppendDateToSQLTable and PreserveHistory cannot be true!");
                EventLog.WriteEntry(this.ServiceName, messageText.ToString(), EventLogEntryType.Error);
                WriteLog(messageText.ToString());
                Stop();
                return;
            }

            //Initial startup so get the server list
            string sServerNames = Properties.Settings.Default.AnalysisServerName + "," + Properties.Settings.Default.AnalysisServerNames;
            _ServernamesList = new List<string>(sServerNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            //Get the number of servers so we can maintain legacy behaviour
            _NumInstance = _ServernamesList.Count;

            foreach (string SSASserver in _ServernamesList)
            {
                //Spin up a thread to write the trace to SQL
                Thread worker = new Thread(DoWork);
                workers.Add(worker);
                //Start the tracing and pass SSAS Server name so we can handle retries
                worker.Start(SSASserver);
            }
        }


        bool ConnectOlap(out TraceServer traceServer, string SSASserver)
        {

            OlapConnectionInfo ci = new OlapConnectionInfo();
            traceServer = new TraceServer();
            StringBuilder messageText = new StringBuilder();

            try
            {
                ci.UseIntegratedSecurity = true;
                ci.ServerName = SSASserver;
                string tracetemplate = localPath + "\\" + Properties.Settings.Default.TraceDefinition;
                traceServer.InitializeAsReader(ci, tracetemplate);

                lock (traceServers)
                {
                    traceServers.Add(traceServer);
                }

                messageText.Append(
                    DateTime.Now.ToString()
                        + ":  Created trace for Analysis Server : '"
                        + SSASserver
                        + "'");

                WriteLog(messageText.ToString());
                EventLog.WriteEntry(this.ServiceName, messageText.ToString(), EventLogEntryType.Information);

                return true;
            }

            catch (Exception e)
            {

                messageText.Append(DateTime.Now.ToString() + ":  Cannot start Analysis Server trace: ").AppendLine();
                messageText.Append(DateTime.Now.ToString() + ":  Analysis Server name: '" + SSASserver + "'").AppendLine();
                messageText.Append(DateTime.Now.ToString() + ":  Trace definition : '" + localPath + "\\" + Properties.Settings.Default.TraceDefinition + "'").AppendLine();
                messageText.Append(DateTime.Now.ToString() + ":  Error: " + e.Message).AppendLine();
                messageText.Append(DateTime.Now.ToString() + ":  Stack Trace: " + e.StackTrace).AppendLine();

                while (e.InnerException != null)
                {
                    messageText.Append("INNER EXCEPTION: ");
                    messageText.Append(e.InnerException.Message).AppendLine();
                    messageText.Append(e.InnerException.StackTrace).AppendLine();

                    e = e.InnerException;
                }

                WriteLog(messageText.ToString());
                EventLog.WriteEntry(this.ServiceName, messageText.ToString(), EventLogEntryType.Error);

                return false;
            }
        }

        bool ConnectSQL(ref TraceServer traceServer, out TraceTable tableWriter, string SSASserver)
        {
            SqlConnectionInfo connInfo = new SqlConnectionInfo(Properties.Settings.Default.SQLServer);
            StringBuilder messageText = new StringBuilder();

            string _AppendInst = "_" + SSASserver;
            string _SQLTable;
            string _AppendDate;

            //Maintains legacy logic where by a server with single instance does not have any inst names appended.
            if (_NumInstance == 1)
                _AppendInst = "";
            //Append data to end of SQL table. Useful as an alternative to preserver SQL but data only survives 1 restart a day and you need cleanup logic in SQL Server
            if (Properties.Settings.Default.AppendDateToSQLTable)
                _AppendDate = "_" + DateTime.Now.ToString("yyyyMMdd");
            else
                _AppendDate = "";

            _SQLTable =
                 Properties.Settings.Default.TraceTableName
                 + _AppendInst
                 + _AppendDate;

            if (Properties.Settings.Default.PreserveHistory)
                PreserveSQLHistory(ref _SQLTable);

            tableWriter = new TraceTable();

            try
            {
                connInfo.DatabaseName = Properties.Settings.Default.SQLServerDatabase;
                tableWriter.InitializeAsWriter(traceServer, connInfo, _SQLTable);

                messageText.Append(DateTime.Now.ToString() + ":  Created Analysis Server trace table: '" + _SQLTable + "' on SQL Server: '" + Properties.Settings.Default.SQLServer
                    + "' in database: " + Properties.Settings.Default.SQLServerDatabase + "'");
                WriteLog(messageText.ToString());
                EventLog.WriteEntry(this.ServiceName, messageText.ToString(), EventLogEntryType.Information);
                return true;
            }

            catch (Exception e)
            {
                messageText.Append(DateTime.Now.ToString() + ":  Cannot create Analysis Server trace table: '" + SSASserver + "'").AppendLine();
                messageText.Append(DateTime.Now.ToString() + ":  SQL Server Name: '" + Properties.Settings.Default.SQLServer + "'").AppendLine();
                messageText.Append(DateTime.Now.ToString() + ":  SQL Server Database : '" + Properties.Settings.Default.SQLServerDatabase + "'").AppendLine();
                messageText.Append(DateTime.Now.ToString() + ":  SQL Server Table : '" + _SQLTable + "'").AppendLine();
                messageText.Append(DateTime.Now.ToString() + ":  Error: " + e.Message).AppendLine();

                while (e.InnerException != null)
                {
                    messageText.Append("INNER EXCEPTION: ");
                    messageText.Append(e.InnerException.Message).AppendLine();
                    messageText.Append(e.InnerException.StackTrace.ToString()).AppendLine();

                    e = e.InnerException;
                }

                WriteLog(messageText.ToString());
                EventLog.WriteEntry(this.ServiceName, messageText.ToString(), EventLogEntryType.Error);

                return false;
            }
        }

        void PreserveSQLHistory(ref string _SQLTable)
        {
            string sHistorySuffix = "History";
            if (_NumInstance > 1)
            {
                sHistorySuffix = "_History";
            }
            StringBuilder messageText = new StringBuilder();
            try
            {

                SqlConnectionInfo connInfo = new SqlConnectionInfo(Properties.Settings.Default.SQLServer);
                IDbConnection conn = connInfo.CreateConnectionObject();
                conn.Open();
                conn.ChangeDatabase(Properties.Settings.Default.SQLServerDatabase);

                string sSQL = @"if object_id('[" + _SQLTable.Replace("'", "''") + @"]') is not null 
                                 select top 1 * from [" + _SQLTable.Replace("]", "]]") + "]";
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sSQL, (System.Data.SqlClient.SqlConnection)conn);
                cmd.CommandTimeout = 0;
                System.Data.SqlClient.SqlDataReader datareader = cmd.ExecuteReader();
                string sColumns = "";
                for (int i = 0; i < datareader.FieldCount; i++)
                {
                    if (!string.IsNullOrEmpty(sColumns)) sColumns += ", ";
                    sColumns += "[" + datareader.GetName(i) + "]";
                }
                datareader.Close();

                if (sColumns != "")
                {
                    sSQL = @"
                                if object_id('[" + _SQLTable.Replace("'", "''") + @"]') is not null
                                begin
                                    if object_id('[" + _SQLTable.Replace("'", "''") + sHistorySuffix + @"]') is null
                                    begin
                                        select * into [" + _SQLTable.Replace("]", "]]") + sHistorySuffix + "] from [" + _SQLTable.Replace("]", "]]") + @"]
                                    end
                                    else
                                    begin
                                        SET IDENTITY_INSERT [" + _SQLTable.Replace("]", "]]") + sHistorySuffix + @"] ON
                                        insert into [" + _SQLTable.Replace("]", "]]") + sHistorySuffix + "] (" + sColumns + @")
                                        select * from [" + _SQLTable.Replace("]", "]]") + @"]
                                        SET IDENTITY_INSERT [" + _SQLTable.Replace("]", "]]") + sHistorySuffix + @"] OFF
                                    end
                                end
                                ";

                    cmd.CommandText = sSQL;
                    int iRowsPreserved = cmd.ExecuteNonQuery();

                    messageText.Append(DateTime.Now.ToString() + ":  Successfully preserved " + iRowsPreserved + " rows of history to table: " + _SQLTable + sHistorySuffix);
                    WriteLog(messageText.ToString());
                    EventLog.WriteEntry(this.ServiceName, messageText.ToString(), EventLogEntryType.Information);
                }

                conn.Close();
                conn.Dispose();
            }
            catch (Exception ex)
            {
                messageText.Append(DateTime.Now.ToString() + ":  Cannot preserve history of trace table. ").AppendLine();
                messageText.Append("Error: " + ex.Message).AppendLine();
                messageText.Append(ex.StackTrace).AppendLine();

                while (ex.InnerException != null)
                {
                    messageText.Append("INNER EXCEPTION: ");
                    messageText.Append(ex.InnerException.Message).AppendLine();
                    messageText.Append(ex.InnerException.StackTrace).AppendLine();

                    ex = ex.InnerException;
                }

                WriteLog(messageText.ToString());
                EventLog.WriteEntry(this.ServiceName, messageText.ToString(), EventLogEntryType.Warning);
            }
        }

        void DoWork(object SSAS)
        {
            string SSASserver = (string)SSAS;
            TraceTable _SQLDestTableWriter = null;
            TraceServer _SSASSourceTraceServer = null;
            int _RetryCounter = 0;
            bool bFirstLoop = true;

            while (bFirstLoop || _RetryCounter < Properties.Settings.Default.RestartRetries)
            {
                bFirstLoop = false;
                try
                {
                    //Grab connection to SSAS
                    bool bSuccess = ConnectOlap(out _SSASSourceTraceServer, SSASserver);

                    if (bSuccess)
                    {
                        //Grab connection to SQL and connect it with the SSAS trace
                        bSuccess = ConnectSQL(ref _SSASSourceTraceServer, out _SQLDestTableWriter, SSASserver);

                        if (bSuccess)
                        {
                            _RetryCounter = 0;

                            while (_SQLDestTableWriter.Write())
                            {
                                if (_SQLDestTableWriter.IsClosed)
                                    throw new Exception("SQL connection closed unexpectedly.");
                                if (_SSASSourceTraceServer.IsClosed)
                                    throw new Exception("SSAS connection closed unexpectedly.");
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    StringBuilder messageText = new StringBuilder();

                    messageText.Append(DateTime.Now.ToString() + ":  Error reading trace: " + ex.Message).AppendLine();
                    messageText.Append(ex.StackTrace).AppendLine();

                    while (ex.InnerException != null)
                    {
                        messageText.Append("INNER EXCEPTION: ");
                        messageText.Append(ex.InnerException.Message).AppendLine();
                        messageText.Append(ex.InnerException.StackTrace).AppendLine();

                        ex = ex.InnerException;
                    }

                    WriteLog(messageText.ToString());
                    EventLog.WriteEntry(this.ServiceName, messageText.ToString(), EventLogEntryType.Warning);
                }

                try
                {
                    _SSASSourceTraceServer.Stop();
                    _SSASSourceTraceServer.Close();
                }
                catch { }
                try
                {
                    _SQLDestTableWriter.Close();
                }
                catch { }


                _RetryCounter++;

                if (_RetryCounter < Properties.Settings.Default.RestartRetries)
                {
                    StringBuilder messageText2 = new StringBuilder();
                    messageText2.Append(DateTime.Now.ToString() + ":  Exception caught tracing server: " + SSASserver + ", retry " + _RetryCounter + " of " + Properties.Settings.Default.RestartRetries
                        + ". Pausing for " + Properties.Settings.Default.RestartDelayMinutes + " minute(s) then restarting automatically"
                       ).AppendLine();
                    WriteLog(messageText2.ToString());
                    EventLog.WriteEntry(this.ServiceName, messageText2.ToString(), EventLogEntryType.Warning);
                    System.Threading.Thread.Sleep(new TimeSpan(0, Properties.Settings.Default.RestartDelayMinutes, 0));
                }
                else
                {
                    WriteLog(DateTime.Now.ToString() + ":  Exceeded the number of allowed retries for server: " + SSASserver);
                }
            }

            //if this one trace exceeded the number of retries so stop the service and stop all traces
            Stop();
        }

        protected override void OnStop()
        {
            try
            {
                WriteLog(DateTime.Now.ToString() + ":  Stopping");
                writer.Close();
                writer.Dispose();
            }
            catch { }

            try
            {
                foreach (TraceServer ts in traceServers)
                {
                    try
                    {
                        ts.Stop();
                        ts.Close();
                        ts.Dispose();
                    }
                    catch { }
                }
            }
            catch { }
        }

        private void WriteLog(string sMessage)
        {
            lock (writer)
            {
                try
                {
                    writer.WriteLine(sMessage);
                    writer.Flush();
                }
                catch { }
            }
        }

    }
}
